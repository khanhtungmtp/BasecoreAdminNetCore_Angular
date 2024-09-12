using API._Services.Interfaces.System;

namespace API._Services.Services.System;
public class S_Storage: I_Storage
{
    private readonly string _userContentFolder;
    public S_Storage(IWebHostEnvironment hostingEnvironment)
    {
        // Kiểm tra xem `WebRootPath` có giá trị null hay không và đưa ra cảnh báo hoặc thiết lập giá trị mặc định.
        if (string.IsNullOrEmpty(hostingEnvironment.WebRootPath))
        ArgumentNullException.ThrowIfNull(hostingEnvironment.WebRootPath);

        _userContentFolder = Path.Combine(hostingEnvironment.WebRootPath, USER_CONTENT_FOLDER_NAME);
    }
    private const string USER_CONTENT_FOLDER_NAME = "user-attachments";

    public string GetFileUrl(string fileName)
    {
        return $"/{USER_CONTENT_FOLDER_NAME}/{fileName}";
    }

    public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
    {
        if (!Directory.Exists(_userContentFolder))
            Directory.CreateDirectory(_userContentFolder);

        string? filePath = Path.Combine(_userContentFolder, fileName);
        using var output = new FileStream(filePath, FileMode.Create);
        await mediaBinaryStream.CopyToAsync(output);
    }

    public async Task DeleteFileAsync(string fileName)
    {
        string? filePath = Path.Combine(_userContentFolder, fileName);
        if (File.Exists(filePath))
        {
            await Task.Run(() => File.Delete(filePath));
        }
    }
}
