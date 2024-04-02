using System.Text.RegularExpressions;

namespace API.Helpers.Utilities;

public static partial class FunctionUtility
{
    private static string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    /// <summary>
    /// Upload a file to server folder.
    /// </summary>
    /// <param name="file">Uploaded file.</param>
    /// <param name="subfolder">Subfolder. Default: "upload"</param>
    /// <param name="rawFileName">Raw file name. Default: uploaded file name.</param>
    /// <returns>File name.</returns>
    public static async Task<string?> UploadAsync(IFormFile file, string subfolder = "upload", string rawFileName = "")
    {
        if (file == null)
            return null;

        var folderPath = Path.Combine(webRootPath, subfolder);
        var fileName = file.FileName;
        var extension = Path.GetExtension(file.FileName);

        if (string.IsNullOrEmpty(extension))
            return null;

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        if (!string.IsNullOrEmpty(rawFileName))
            fileName = $"{rawFileName}{extension}";

        var filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        try
        {
            using (FileStream fs = File.Create(filePath))
            {
                await file.CopyToAsync(fs);
                await fs.FlushAsync();
            }

            return fileName;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Upload a base64 string file to server folder.
    /// </summary>
    /// <param name="file">Uploaded file.</param>
    /// <param name="subfolder">Subfolder. Default: "upload"</param>
    /// <param name="rawFileName">Raw file name. Default: uploaded file name.</param>
    /// <returns>File name.</returns>
    public static async Task<string?> UploadAsync(string file, string subfolder = "upload", string rawFileName = "")
    {
        if (string.IsNullOrEmpty(file))
            return null;

        var folderPath = Path.Combine(webRootPath, subfolder);
        var extension = $".{file.Split(';')[0].Split('/')[1]}";

        if (string.IsNullOrEmpty(extension))
            return null;

        var fileName = $"{Guid.NewGuid()}{extension}";

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        if (!string.IsNullOrEmpty(rawFileName))
            fileName = $"{rawFileName}{extension}";

        var filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        var base64String = file[(file.IndexOf(',') + 1)..];
        var fileData = Convert.FromBase64String(base64String);

        try
        {
            await File.WriteAllBytesAsync(filePath, fileData);
            return fileName;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    private static readonly string[] VietNamChar =
      [
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
      ];

    public static string RemoveUnicode(string str)
    {
        //Thay thế và lọc dấu từng char
        for (int i = 1; i < VietNamChar.Length; i++)
        {
            for (int j = 0; j < VietNamChar[i].Length; j++)
                str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
        }

        str = MyRegex().Replace(str, " ").ToLower();
        return str;
    }

    public static string GenerateCodeIdentity(string code)
    {
        return (Convert.ToInt32(code) + 1).ToString().PadLeft(5, '0');
    }

    [GeneratedRegex("[^0-9a-zA-Z]+")]
    private static partial Regex MyRegex();
}