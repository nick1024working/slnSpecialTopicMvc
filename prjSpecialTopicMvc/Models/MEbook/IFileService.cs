namespace prjSpecialTopicMvc.Models.MEbook
{
    // 註解：定義檔案服務的合約，包含儲存和刪除檔案的功能。
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string subfolder);
        void DeleteFile(string? relativePath);
    }
}
