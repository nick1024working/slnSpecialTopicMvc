using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.ViewModels.EBook;

namespace prjSpecialTopicMvc.Models.MEbook
{
    // 註解：實作 IEbookService，注入資料庫上下文和檔案服務。
    public class EbookService : IEbookService
    {
        private readonly TeamAProjectContext _context;
        private readonly IFileService _fileService;

        public EbookService(TeamAProjectContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<EbookIndexViewModel> GetEbooksForListAsync(string? keyword, string availabilityFilter)
        {
            var viewModel = new EbookIndexViewModel
            {
                Keyword = keyword,
                AvailabilityFilter = availabilityFilter
            };

            IQueryable<EBookMain> query = _context.EBookMains;
            switch (availabilityFilter)
            {
                case "AvailableOnly": query = query.Where(e => e.IsAvailable); break;
                case "UnavailableOnly": query = query.Where(e => !e.IsAvailable); break;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(e => e.EbookName.Contains(keyword) || e.Author.Contains(keyword));
            }

            viewModel.Ebooks = await query.OrderBy(e => e.EbookId).ToListAsync();
            return viewModel;
        }

        public async Task<EBookMain?> GetEbookForDetailsAsync(long id)
        {
           
            return await _context.EBookMains
        .Include(e => e.Category)
            .ThenInclude(c => c.ParentCategory)
        .Include(e => e.EBookImages)
        // 註解：【*** 新增此行 ***】
        // 在查詢 EBookMain 的同時，使用 Include() 將其關聯的所有 Labels 紀錄一併載入。
        .Include(e => e.Labels)
        .FirstOrDefaultAsync(m => m.EbookId == id);
        }

        public async Task<EbookFormViewModel> GetEbookForEditAsync(long id)
        {
            var ebook = await _context.EBookMains.FindAsync(id);
            if (ebook == null)
            {
                // 在真實應用中，可以拋出一個自訂的 NotFoundException
                throw new Exception("Ebook not found");
            }

            return new EbookFormViewModel
            {
                Ebook = ebook,
                // 註解：將 Ebook 的 CategoryId 賦值給 ViewModel 中的 SelectedCategoryId
                // 這樣 View 載入時，下拉選單才能正確顯示當前的分類。
                SelectedCategoryId = ebook.CategoryId,
                CategoryOptions = await GetCategoryOptionsAsync(),
                ExistingImages = await _context.EBookImages.Where(i => i.EBookId == id).ToListAsync()
            };
        }

        public async Task<EbookFormViewModel> GetNewEbookFormAsync()
        {
            return new EbookFormViewModel
            {
                CategoryOptions = await GetCategoryOptionsAsync()
            };
        }

        public async Task CreateEbookAsync(EbookFormViewModel viewModel)
        {
            var newEbook = new EBookMain();

            // 處理檔案
            if (viewModel.EbookFile != null)
            {
                newEbook.EBookPosition = await _fileService.SaveFileAsync(viewModel.EbookFile, "EBook");
                newEbook.EBookDataType = Path.GetExtension(viewModel.EbookFile.FileName)!.ToUpper().Replace(".", "");
            }
            else
            {
                newEbook.EBookPosition = "N/A";
                newEbook.EBookDataType = "N/A";
            }

            // 處理分類
            if (!string.IsNullOrWhiteSpace(viewModel.NewCategoryName))
            {
                var newCategoryNameTrimmed = viewModel.NewCategoryName.Trim();
                var existingCategory = await _context.EBookCategories.FirstOrDefaultAsync(c => c.CategoryName.ToUpper() == newCategoryNameTrimmed.ToUpper());
                if (existingCategory != null)
                {
                    newEbook.CategoryId = existingCategory.CategoryId;
                }
                else
                {
                    var newCategory = new EBookCategory { CategoryName = newCategoryNameTrimmed };
                    _context.EBookCategories.Add(newCategory);
                    await _context.SaveChangesAsync();
                    newEbook.CategoryId = newCategory.CategoryId;
                }
            }
            else if (viewModel.SelectedCategoryId.HasValue) // 使用新的 SelectedCategoryId
            {
                newEbook.CategoryId = viewModel.SelectedCategoryId.Value;
            }
            else
            {
                // 如果使用者既沒選也沒輸入新分類，我們需要手動報錯
                // 雖然這種情況前端應該不會發生，但後端驗證是最後一道防線
                // 這裡可以選擇拋出例外或賦予一個預設分類 ID
                // 為簡單起見，我們暫時假設使用者一定會選一個
                // 若要更嚴謹，應在此處加入錯誤處理
                // 例如: throw new ArgumentException("必須選擇一個分類或輸入新分類。");
            }

            // 註解：補全所有從 viewModel 複製到 newEbook 的屬性
            newEbook.EbookName = viewModel.Ebook.EbookName;
            newEbook.Author = viewModel.Ebook.Author;
            newEbook.BookDescription = viewModel.Ebook.BookDescription;
            newEbook.Publisher = viewModel.Ebook.Publisher;
            newEbook.PublishedDate = viewModel.Ebook.PublishedDate;
            newEbook.Translator = viewModel.Ebook.Translator;
            newEbook.Language = viewModel.Ebook.Language;
            newEbook.Isbn = viewModel.Ebook.Isbn;
            newEbook.Eisbn = viewModel.Ebook.Eisbn;
            newEbook.PublishedCountry = viewModel.Ebook.PublishedCountry;
            newEbook.FixedPrice = viewModel.Ebook.FixedPrice;
            newEbook.ActualPrice = viewModel.Ebook.ActualPrice;
            newEbook.IsAvailable = viewModel.Ebook.IsAvailable;

            // 註解：檢查 viewModel.Ebook.MaturityRating 是否在 0 到 2 的有效範圍內。
            // 如果是 (true)，就使用 viewModel.Ebook.MaturityRating 的值。
            // 如果不是 (false)，就使用預設值 0。
            // (byte)0 是為了確保型別一致，因為 MaturityRating 是 byte 型別。
            newEbook.MaturityRating = (viewModel.Ebook.MaturityRating >= 0 && viewModel.Ebook.MaturityRating <= 2)
                                      ? viewModel.Ebook.MaturityRating
                                      : (byte)0;
            // 銷售與觀看數通常由系統計算，但在後台建立時，我們可以允許直接從表單輸入
            newEbook.Weeksales = viewModel.Ebook.Weeksales;
            newEbook.Monthsales = viewModel.Ebook.Monthsales;
            newEbook.Totalsales = viewModel.Ebook.Totalsales;
            newEbook.Weekviews = viewModel.Ebook.Weekviews;
            newEbook.Monthviews = viewModel.Ebook.Monthviews;
            newEbook.Totalviews = viewModel.Ebook.Totalviews;

            _context.EBookMains.Add(newEbook);
            await _context.SaveChangesAsync();

            // 處理封面圖
            if (viewModel.CoverImages != null && viewModel.CoverImages.Count > 0)
            {
                bool isFirstImage = true;
                foreach (var imageFile in viewModel.CoverImages)
                {
                    var imagePath = await _fileService.SaveFileAsync(imageFile, "Images");
                    _context.EBookImages.Add(new EBookImage { EBookId = newEbook.EbookId, ImagePath = imagePath, IsPrimary = isFirstImage });
                    if (isFirstImage)
                    {
                        newEbook.PrimaryCoverPath = imagePath;
                        isFirstImage = false;
                    }
                }
                await _context.SaveChangesAsync();
            }
        }

        // 在 EbookService.cs 中


        public async Task UpdateEbookAsync(long id, EbookFormViewModel viewModel)
        {
            var ebookToUpdate = await _context.EBookMains
        .Include(e => e.EBookImages)
        .FirstOrDefaultAsync(e => e.EbookId == id);

            if (ebookToUpdate == null) return;

            // 【*** 以下是本次修正的核心 ***】
            // 註解：處理新的電子書檔案上傳
            if (viewModel.EbookFile != null)
            {
                // 1. 先刪除舊的檔案 (如果存在且路徑有效)
                _fileService.DeleteFile(ebookToUpdate.EBookPosition);

                // 2. 儲存新檔案並更新路徑
                ebookToUpdate.EBookPosition = await _fileService.SaveFileAsync(viewModel.EbookFile, "EBook");
                ebookToUpdate.EBookDataType = Path.GetExtension(viewModel.EbookFile.FileName)!.ToUpper().Replace(".", "");
            }


            // --- 圖片處理開始 ---

            // 1. 優先處理主封面的切換
            if (viewModel.NewPrimaryImageId.HasValue)
            {
                foreach (var img in ebookToUpdate.EBookImages) { img.IsPrimary = false; }
                var newPrimaryImage = ebookToUpdate.EBookImages.FirstOrDefault(i => i.ImageId == viewModel.NewPrimaryImageId.Value);
                if (newPrimaryImage != null)
                {
                    newPrimaryImage.IsPrimary = true;
                }
            }

            // 2. 處理要刪除的既有圖片
            if (viewModel.ImagesToDelete != null && viewModel.ImagesToDelete.Any())
            {
                // 找出要刪除的圖片實體
                var imagesToDelete = ebookToUpdate.EBookImages
                    .Where(i => viewModel.ImagesToDelete.Contains(i.ImageId))
                    .ToList(); // 建立一個副本以供操作

                foreach (var image in imagesToDelete)
                {
                    _fileService.DeleteFile(image.ImagePath);
                    _context.EBookImages.Remove(image); // 從 DbContext 中移除
                    ebookToUpdate.EBookImages.Remove(image); // 同時從記憶體的集合中移除
                }
            }

            // 3. 處理新上傳的封面圖
            if (viewModel.CoverImages != null && viewModel.CoverImages.Any())
            {
                bool hasPrimary = ebookToUpdate.EBookImages.Any(i => i.IsPrimary);
                foreach (var imageFile in viewModel.CoverImages)
                {
                    var imagePath = await _fileService.SaveFileAsync(imageFile, "Images");
                    var newImage = new EBookImage { EBookId = id, ImagePath = imagePath, IsPrimary = !hasPrimary };
                    _context.EBookImages.Add(newImage);
                    // 將新圖片也加入記憶體集合，以便最終同步時能找到它
                    ebookToUpdate.EBookImages.Add(newImage);

                    if (!hasPrimary) hasPrimary = true;
                }
            }

            // 【*** 核心修正：最終同步 PrimaryCoverPath ***】
            // 註解：在所有圖片操作 (切換、刪除、新增) 完成後，
            // 根據 EBookImages 集合的最終狀態，來決定 PrimaryCoverPath 的值。
            var finalPrimaryImage = ebookToUpdate.EBookImages.FirstOrDefault(i => i.IsPrimary);
            if (finalPrimaryImage != null)
            {
                // 如果存在主封面，就將其路徑設為主路徑
                ebookToUpdate.PrimaryCoverPath = finalPrimaryImage.ImagePath;
            }
            else
            {
                // 如果在所有操作後，已沒有任何主封面，則確保主路徑為 null
                ebookToUpdate.PrimaryCoverPath = null;
            }

            // --- 圖片處理結束 ---

            // 更新從表單傳來的其他屬性...
            ebookToUpdate.EbookName = viewModel.Ebook.EbookName;
            ebookToUpdate.Author = viewModel.Ebook.Author;
            ebookToUpdate.BookDescription = viewModel.Ebook.BookDescription;
            ebookToUpdate.CategoryId = viewModel.SelectedCategoryId ?? ebookToUpdate.CategoryId;
            ebookToUpdate.Publisher = viewModel.Ebook.Publisher;
            ebookToUpdate.PublishedDate = viewModel.Ebook.PublishedDate;
            ebookToUpdate.Translator = viewModel.Ebook.Translator;
            ebookToUpdate.Language = viewModel.Ebook.Language;
            ebookToUpdate.Isbn = viewModel.Ebook.Isbn;
            ebookToUpdate.Eisbn = viewModel.Ebook.Eisbn;
            ebookToUpdate.PublishedCountry = viewModel.Ebook.PublishedCountry;
            ebookToUpdate.FixedPrice = viewModel.Ebook.FixedPrice;
            ebookToUpdate.ActualPrice = viewModel.Ebook.ActualPrice;
            ebookToUpdate.IsAvailable = viewModel.Ebook.IsAvailable;
            // 註解：檢查 viewModel.Ebook.MaturityRating 是否在 0 到 2 的有效範圍內。
            // 如果是 (true)，就使用 viewModel.Ebook.MaturityRating 的值。
            // 如果不是 (false)，就使用預設值 0。
            // (byte)0 是為了確保型別一致，因為 MaturityRating 是 byte 型別。
            ebookToUpdate.MaturityRating = (viewModel.Ebook.MaturityRating >= 0 && viewModel.Ebook.MaturityRating <= 2)
                                      ? viewModel.Ebook.MaturityRating
                                      : (byte)0;

            ebookToUpdate.Weeksales = viewModel.Ebook.Weeksales;
            ebookToUpdate.Monthsales = viewModel.Ebook.Monthsales;
            ebookToUpdate.Totalsales = viewModel.Ebook.Totalsales;
            ebookToUpdate.Weekviews = viewModel.Ebook.Weekviews;
            ebookToUpdate.Monthviews = viewModel.Ebook.Monthviews;
            ebookToUpdate.Totalviews = viewModel.Ebook.Totalviews;

            await _context.SaveChangesAsync();
        }

       

        public async Task ToggleAvailabilityAsync(long id)
        {
            var ebook = await _context.EBookMains.FindAsync(id);
            if (ebook != null)
            {
                ebook.IsAvailable = !ebook.IsAvailable;
                _context.Update(ebook);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<EBookMain?> FindEbookByIdAsync(long id)
        {
            return await _context.EBookMains.FindAsync(id);
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteEbookAsync(long id)
        {
            // 1. 取得要刪除的完整資料，包含所有需要一併處理的關聯
            var eBookMain = await _context.EBookMains
                .Include(e => e.EBookImages) // 包含封面圖
                .Include(e => e.Labels)      // 包含標籤
                .Include(e => e.OrderItems)    // 包含訂單項目
                .FirstOrDefaultAsync(e => e.EbookId == id);

            if (eBookMain == null)
            {
                return (false, "找不到指定的書籍。");
            }

            // 2. 【核心修正】執行刪除前的「預先檢查」
            // 檢查是否有訂單關聯，這是絕對不能刪除的情況
            if (eBookMain.OrderItems.Any())
            {
                return (false, "無法刪除此書，因為已有相關的訂單紀錄。請考慮使用『下架』功能。");
            }

            // 3. 如果檢查通過，才開始執行刪除操作
            try
            {
                // A. 先收集所有要刪除的實體檔案路徑
                var imagePathsToDelete = eBookMain.EBookImages.Select(i => i.ImagePath).ToList();
                var ebookFilePathToDelete = eBookMain.EBookPosition;

                // B. 從 Context 中移除資料庫紀錄
                _context.EBookImages.RemoveRange(eBookMain.EBookImages); // 移除所有關聯圖片
                                                                         // 註解：對於標籤，EF Core 會自動處理中間表 EBook_Labels 的紀錄，我們不需手動移除 Labels 本身
                                                                         // 【*** 新增此行：核心修正 ***】
                                                                         // 註解：在刪除 EBookMain 主紀錄之前，先清除其與 Labels 的多對多關聯。
                                                                         // EF Core 在 SaveChanges 時會自動去刪除 EBook_Labels 中間表的對應紀錄。
                eBookMain.Labels.Clear();

                _context.EBookMains.Remove(eBookMain); // 移除書籍主紀錄

                // C. 執行資料庫儲存，這是關鍵的交易步驟
                await _context.SaveChangesAsync();

                // D. 只有在資料庫成功儲存後，才開始刪除實體檔案
                foreach (var path in imagePathsToDelete)
                {
                    _fileService.DeleteFile(path);
                }
                _fileService.DeleteFile(ebookFilePathToDelete);

                // E. 回傳成功
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                // 捕捉其他可能的未知錯誤
                return (false, $"刪除過程中發生未知錯誤: {ex.Message}");
            }
        }

        // 輔助方法
        private async Task<List<SelectListItem>> GetCategoryOptionsAsync()
        {
            var categories = await _context.EBookCategories.Include(c => c.ParentCategory).OrderBy(c => c.CategoryName).ToListAsync();
            return categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = (c.ParentCategory != null) ? $"({c.ParentCategory.CategoryName}) {c.CategoryName}" : c.CategoryName
            }).ToList();
        }


        // ... 在 EbookService 類別中，加入以下兩個新方法 ...

        public async Task<ManageEbookTagsViewModel?> GetTagsForEbookAsync(long ebookId)
        {
            // 1. 找到書籍，並同時載入它目前已有的標籤
            var ebook = await _context.EBookMains
                                      .Include(e => e.Labels)
                                      .FirstOrDefaultAsync(e => e.EbookId == ebookId);

            if (ebook == null) return null;

            // 2. 取得資料庫中所有的標籤
            var allLabels = await _context.Labels.ToListAsync();

            // 3. 建立 ViewModel
            var viewModel = new ManageEbookTagsViewModel
            {
                EbookId = ebook.EbookId,
                EbookName = ebook.EbookName,
                // 4. 遍歷所有標籤，產生包含「是否已指派」狀態的列表
                AllLabels = allLabels.Select(label => new AssignedLabelViewModel
                {
                    LabelId = label.LabelId,
                    LabelName = label.LabelName,
                    // 檢查這本書現有的標籤中，是否有任何一個的 ID 和當前遍歷到的標籤 ID 相同
                    IsAssigned = ebook.Labels.Any(l => l.LabelId == label.LabelId)
                }).ToList()
            };

            return viewModel;
        }

        public async Task UpdateEbookTagsAsync(long ebookId, IEnumerable<int> selectedLabelIds)
        {
            // 確保 selectedLabelIds 不是 null，如果前端沒有選任何 checkbox，它會是空的，而不是 null
            selectedLabelIds ??= new List<int>();

            // 1. 找到要更新的書籍，並載入其目前的標籤
            var ebookToUpdate = await _context.EBookMains
                                              .Include(e => e.Labels)
                                              .FirstOrDefaultAsync(e => e.EbookId == ebookId);

            if (ebookToUpdate == null) return;

            // 2. 取得所有被選中的標籤的實體
            var selectedLabels = await _context.Labels
                                               .Where(l => selectedLabelIds.Contains(l.LabelId))
                                               .ToListAsync();

            // 3. 將書籍目前的標籤列表，完全替換為使用者本次提交的標籤列表
            ebookToUpdate.Labels = selectedLabels;

            // 4. 儲存變更，EF Core 會自動處理中間表 (EBook_Labels) 的新增與刪除
            await _context.SaveChangesAsync();
        }
    }
}
