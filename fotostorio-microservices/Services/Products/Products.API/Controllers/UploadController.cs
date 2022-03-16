using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Products.API.Controllers
{
    public class UploadController : BaseApiController
    {
        private readonly IConfiguration _config;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IConfiguration config, ILogger<UploadController> logger)
        {
            _config = config;
            _logger = logger;
        }

        // POST api/upload
        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            var azureConnectionString = _config["AzureUploadSettings:AzureStorageConnectionString"];
            var azureContainersAllowed = _config["AzureUploadSettings:AzureContainersAllowed"];
            var permittedFileExtensions = _config["AzureUploadSettings:FileUploadTypesAllowed"];
            var fileSizeLimit = _config.GetValue<long>("AzureUploadSettings:MaxFileUploadSize");

            var formCollection = await Request.ReadFormAsync();
            var file = formCollection.Files[0];

            var requestedContainer = formCollection["container"].ToString();
            var fileExtension = Path.GetExtension(file.FileName.ToLowerInvariant());

            // check the file type is allowed
            if (!permittedFileExtensions.Contains(fileExtension) && fileExtension != "")
            {
                _logger.LogError("The file type '" + fileExtension + "' is not allowed");
                return BadRequest("The file type '" + fileExtension + "' is not allowed");
            }

            // check the file size (in bytes)
            if (file.Length > fileSizeLimit)
            {
                _logger.LogError("The file size of " + file.Length + " bytes was too large");
                return BadRequest("The file size of " + file.Length + " bytes was too large");
            }

            // check the file name length isn't excessive
            if (file.FileName.Length > 75)
            {
                _logger.LogError("The file name is too long: " + file.FileName.Length + " characters");
                return BadRequest("The file name is too long: " + file.FileName.Length + " characters");
            }

            // check container name isn't empty
            if (requestedContainer.Length == 0)
            {
                _logger.LogError("Azure container name is empty");
                return BadRequest("Azure container name is empty");
            }

            // check requested container name is in an allowed set of names
            if (!azureContainersAllowed.Contains(requestedContainer.ToLowerInvariant()))
            {
                _logger.LogError("Invalid Azure container name supplied: " + requestedContainer);
                return BadRequest("Invalid Azure container name supplied: " + requestedContainer);
            }

            if (file.Length > 0)
            {
                try
                {
                    var azureContainer = new BlobContainerClient(azureConnectionString, requestedContainer);
                    var createResponse = await azureContainer.CreateIfNotExistsAsync();

                    // in case the container doesn't exist
                    if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    {
                        await azureContainer.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                    }

                    // generate a unique upload file name
                    // [original_filename_without_extension]_[8_random_chars].[original_filename_extension]
                    // eg. filename_xgh38tye.jpg
                    var fileName = HttpUtility.HtmlEncode(Path.GetFileNameWithoutExtension(file.FileName)) +
                        "_" + Path.GetRandomFileName().Substring(0, 8) + Path.GetExtension(file.FileName);

                    var blob = azureContainer.GetBlobClient(fileName);
                    //await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

                    // set the content type (which may or may not have been provided by the client)
                    var blobHttpHeader = new BlobHttpHeaders();

                    if (file.ContentType != null)
                    {
                        blobHttpHeader.ContentType = file.ContentType;
                    }
                    else
                    {
                        blobHttpHeader.ContentType = fileExtension switch
                        {
                            ".jpg" => "image/jpeg",
                            ".jpeg" => "image/jpeg",
                            ".png" => "image/png",
                            _ => null
                        };
                    }

                    using (var fileStream = file.OpenReadStream())
                    {
                        await blob.UploadAsync(fileStream, blobHttpHeader);
                    }

                    return Ok(new
                    {
                        filename = blob.Name,
                        container = blob.BlobContainerName,
                        uri = blob.Uri
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError("The file could not be uploaded: {message}", ex.Message);
                    return BadRequest("The file could not be uploaded");
                }
            }

            _logger.LogError("The file '" + file.FileName + "' could not be uploaded");
            return BadRequest("The file could not be uploaded");
        }
    }
}
