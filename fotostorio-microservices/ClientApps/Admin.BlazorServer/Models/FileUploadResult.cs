namespace Admin.BlazorServer.Models;

public class FileUploadResult
{
    public string FileName { get; set; } = string.Empty;
    public string Container { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public int ErrorCode { get; set; }
}
