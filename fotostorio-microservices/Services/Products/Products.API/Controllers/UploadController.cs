using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace Products.API.Controllers;

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

        // check that the Azure Storage connection string is available
        if (string.IsNullOrEmpty(azureConnectionString))
        {
            _logger.LogError("Azure Storage connection string is empty or has not been set in config/env variables");
            return BadRequest("Azure Storage connection string is empty or unavailable");
        }

        // check that the Azure Storage container names is available
        if (string.IsNullOrEmpty(azureContainersAllowed))
        {
            _logger.LogError("Azure Storage container names was empty or has not been set in config/env variables");
            return BadRequest("Azure Storage container names was empty or unavailable");
        }

        var formCollection = await Request.ReadFormAsync();

        var requestedContainer = formCollection["container"].ToString();

        // check container name isn't empty
        if (requestedContainer.Length == 0)
        {
            _logger.LogError("Azure container name is empty");
            return BadRequest("Azure container name is empty");
        }

        // check requested container name is in an allowed set of names
        if (!azureContainersAllowed.Contains(requestedContainer.ToLowerInvariant()))
        {
            _logger.LogError("Invalid Azure container name supplied: {requestedContainer}", requestedContainer);
            return BadRequest($"Invalid Azure container name supplied: '{requestedContainer}'");
        }

        var file = formCollection.Files[0];

        var fileExtension = Path.GetExtension(file.FileName.ToLowerInvariant());
        var fileNameLengthLimit = 75;

        List<string> basicFileChecks = BasicFileChecks(file, permittedFileExtensions, fileSizeLimit, fileNameLengthLimit, fileExtension);

        if (basicFileChecks is null)
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
                _logger.LogError("The file '{fileName}' could not be uploaded: {message}", file.FileName, ex.Message);
                return BadRequest($"The file '{file.FileName}' could not be uploaded");
            }
        }
        else
        {
            _logger.LogError("The file '{fileName}' failed basic file checks and could not be uploaded", file.FileName);
            return BadRequest($"The file '{file.FileName}' could not be uploaded");
        }
    }

    private List<string> BasicFileChecks(IFormFile file, string permittedFileExtensions, long fileSizeLimit, int fileNameLengthLimit = 75, string fileExtension = "unknown")
    {
        var filecheckErrors = new List<string>();

        // check the file has an extension
        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            _logger.LogError("'{fileName}' does not appear to have a file extension", file.FileName);
            filecheckErrors.Add($"'{file.FileName}' does not appear to have a file extension");
        }

        // check the file type is allowed
        if (!permittedFileExtensions.Contains(fileExtension))
        {
            _logger.LogError("Upload of '{fileName}' with file type '{fileExtension}' was not allowed", file.FileName, fileExtension);
            filecheckErrors.Add($"Upload of '{file.FileName}' with file type '{fileExtension}' is not allowed");
        }

        // check file isn't 0 bytes
        if (file.Length < 1)
        {
            _logger.LogError("The file '{fileName}' had a file size of 0 bytes", file.FileName);
            filecheckErrors.Add($"'{file.FileName}' has a file size of 0 bytes");
        }

        // check the file size (in bytes) isn't above the limit
        if (file.Length > fileSizeLimit)
        {
            _logger.LogError("The size of '{fileName}' ({fileSize} bytes) was larger than the current file size limit ({sizeLimit} bytes)", file.FileName, file.Length, fileSizeLimit);
            filecheckErrors.Add($"The size of '{file.FileName}' ({file.Length} bytes) is larger than the current file size limit");
        }

        // check the file name length isn't above the limit
        if (file.FileName.Length > fileNameLengthLimit)
        {
            _logger.LogError("The name of '{fileName}' was too long at {fileNameLength} characters. The current limit is {fileNameLengthLimit} characters", file.FileName, file.FileName.Length, fileNameLengthLimit);
            filecheckErrors.Add($"The name of '{file.FileName}' was too long: {file.FileName.Length}  characters");
        }

        if (filecheckErrors.Count == 0) return null!;

        return filecheckErrors;
    }
}
