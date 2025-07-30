namespace prjSpecialTopicMvc.Models.MEbook
{
    // 註解：實作 IFileService，注入 IWebHostEnvironment 來處理實體路徑。
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subfolder)
        {
            var uploadsRootFolder = Path.Combine(_webHostEnvironment.WebRootPath, "EBook", subfolder);
            if (!Directory.Exists(uploadsRootFolder))
            {
                Directory.CreateDirectory(uploadsRootFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsRootFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/EBook/{subfolder}/{uniqueFileName}";
        }

        public void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath) || relativePath == "N/A") return;

            // 註解：將網站相對路徑轉換為伺服器上的實體路徑。
            // 使用 TrimStart 確保路徑組合正確，即使開頭有或沒有 /。
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
