using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;


namespace prjSpecialTopicMvc.Controllers
{
    public class DonateController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly TeamAProjectContext _context;

        public DonateController(TeamAProjectContext c, IWebHostEnvironment env)
        {
            _context = c;
            _env = env;
        }

        private void LoadCategoryList()
        {
            ViewBag.CategoryList = _context.DonateCategories
                .Select(c => new SelectListItem
                {
                    Value = c.DonateCategoriesId.ToString(),
                    Text = c.CategoriesName
                })
                .ToList();
        }
        public async Task<IActionResult> List(int? categoryId)
        {
            // 1. 下拉式選單用分類資料
            ViewBag.CategoryList = _context.DonateCategories
                .Select(c => new SelectListItem
                {
                    Value = c.DonateCategoriesId.ToString(),
                    Text = c.CategoriesName
                })
                .ToList();

            // 2. 查詢專案資料並包含圖片與分類
            var query = _context.DonateProjects
                .Include(p => p.DonateCategories)
                .Include(p => p.DonateImages)
                .Where(p => !p.IsDeleted);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.DonateCategoriesId == categoryId);
            }

            ViewBag.SelectedCategoryId = categoryId?.ToString() ?? "";
            return View(await query.ToListAsync());
        }

        // GET: Donate/Create
        public IActionResult Create()
        {
            ViewBag.CategoryList = _context.DonateCategories.ToList();
            return View();
        }

        // POST: Donate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonateProject project, IFormFile? ProjectImage)
        {
            if (ModelState.IsValid)
            {
                project.CreatedAt = DateTime.Now;
                project.UpdatedAt = DateTime.Now;
                project.IsDeleted = false;
                project.CurrentAmount = 0;
                //project.Uid = Guid.Parse(User.FindFirst("UserID").Value); 如有會員ID可使用
                project.Uid = Guid.NewGuid();

                _context.Add(project);
                await _context.SaveChangesAsync();

                if (ProjectImage != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProjectImage.FileName);
                    string savePath = Path.Combine(_env.WebRootPath, "img", fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await ProjectImage.CopyToAsync(stream);
                    }

                    DonateImage img = new DonateImage
                    {
                        DonateProjectId = project.DonateProjectId,
                        DonateImagePath = fileName,
                        IsMain = true
                    };
                    _context.Add(img);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "募資專案建立成功！";
                return RedirectToAction(nameof(List));
            }

            ViewBag.CategoryList = _context.DonateCategories.ToList();
            return View(project);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.DonateProjects
                .Include(p => p.DonateCategories)
                .FirstOrDefaultAsync(p => p.DonateProjectId == id && !p.IsDeleted);

            if (project == null) return NotFound();

            return View(project);
        }

        // POST: Donate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.DonateProjects.FindAsync(id);
            if (project != null)
            {
                project.IsDeleted = true;
                _context.Update(project);
                await _context.SaveChangesAsync();
                TempData["Success"] = "專案已刪除！";
            }
            return RedirectToAction(nameof(List));
        }
        // GET: Donate/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.DonateProjects.FindAsync(id);
            if (project == null || project.IsDeleted) return NotFound();

            ViewBag.CategoryList = _context.DonateCategories.ToList();
            return View(project);
        }

        // POST: Donate/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonateProject project, IFormFile? ProjectImage)
        {
            if (id != project.DonateProjectId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.DonateProjects
                        .Include(p => p.DonateImages)
                        .FirstOrDefaultAsync(p => p.DonateProjectId == id);

                    if (existing == null) return NotFound();

                    // 更新欄位
                    existing.DonateCategoriesId = project.DonateCategoriesId;
                    existing.ProjectTitle = project.ProjectTitle;
                    existing.ProjectDescription = project.ProjectDescription;
                    existing.TargetAmount = project.TargetAmount;
                    existing.StartDate = project.StartDate;
                    existing.EndDate = project.EndDate;
                    existing.Status = project.Status;
                    existing.UpdatedAt = DateTime.Now;

                    // 若有新圖片
                    if (ProjectImage != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProjectImage.FileName);
                        string savePath = Path.Combine(_env.WebRootPath, "images", fileName);
                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await ProjectImage.CopyToAsync(stream);
                        }

                        var image = existing.DonateImages.FirstOrDefault(i => i.IsMain == true);
                        if (image != null)
                        {
                            image.DonateImagePath = fileName;
                        }
                        else
                        {
                            existing.DonateImages.Add(new DonateImage
                            {
                                DonateProjectId = id,
                                DonateImagePath = fileName,
                                IsMain = true
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "專案已成功更新！";
                    return RedirectToAction(nameof(List));
                }
                catch (Exception)
                {
                    return BadRequest("更新失敗");
                }
            }

            ViewBag.CategoryList = _context.DonateCategories.ToList();
            return View(project);
        }
    }
}
