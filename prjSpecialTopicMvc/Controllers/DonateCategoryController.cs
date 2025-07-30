using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;

namespace prjSpecialTopicMvc.Controllers
{
    public class DonateCategoryController : Controller
    {
        private readonly TeamAProjectContext _context;
        public DonateCategoryController(TeamAProjectContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> CategoriesList()
        {
            var categories = await _context.DonateCategories.ToListAsync();
            return View(categories);
        }

        // 新增表單
        public IActionResult CategoriesCreate()
        {
            return View();
        }

        // 處理新增
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoriesCreate(DonateCategory category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "新增成功！";
                return RedirectToAction("CategoriesList");
            }
            return View(category);
        }

        // 刪除
        public async Task<IActionResult> CategoriesDelete(int id)
        {
            var category = await _context.DonateCategories.FindAsync(id);
            if (category != null)
            {
                _context.DonateCategories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "刪除成功！";
            }
            return RedirectToAction("CategoriesList");
        }
    }
}
