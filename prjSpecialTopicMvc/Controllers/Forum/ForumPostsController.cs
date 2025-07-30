using prjSpecialTopicMvc.ViewModels.Forum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace prjSpecialTopicMvc.Controllers.Forum
{

    public class ForumPostsController : Controller
    {
        private readonly TeamAProjectContext _context;

        public ForumPostsController(TeamAProjectContext context)
        {
            _context = context;
        }

        // GET: ForumPosts
        public async Task<IActionResult> Index()
        {
            var teamAProjectContext = _context.ForumPosts
                .Include(f => f.Filter)
                .Include(f => f.PostCategory)
                .Include(f => f.UidNavigation).Where(f => f.IsDeleted == false);
            return View(await teamAProjectContext.ToListAsync());
        }

        // GET: ForumPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var forumPost = await _context.ForumPosts
                .Include(f => f.Filter)
                .Include(f => f.PostCategory)
                .Include(f => f.UidNavigation).Where(f => f.IsDeleted == false)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (forumPost == null) return NotFound();

            return View(forumPost);
        }

        // GET: ForumPosts/Create
        public IActionResult Create()
        {
            var viewModel = new ForumPostViewModel
            {
                Categories = _context.PostCategories
                    .Select(c => new SelectListItem
                    {
                        Value = c.PostCategoryId.ToString(),
                        Text = c.PostCategoryName
                    }),
                Filters = _context.PostFilters
                    .Select(f => new SelectListItem
                    {
                        Value = f.PostFilterId.ToString(),
                        Text = f.FilterName
                    })
            };
            return View(viewModel);
        }

        // POST: ForumPosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ForumPostViewModel viewModel)
        {
            Console.WriteLine("🟡 進入 Create POST");

            if (viewModel.SelectedCategoryId == null)
            {
                ModelState.AddModelError(nameof(viewModel.SelectedCategoryId), "分類未選擇");
                Console.WriteLine("❌ 未選擇分類");
            }

            if (viewModel.SelectedFilterId == null)
            {
                ModelState.AddModelError(nameof(viewModel.SelectedFilterId), "標籤未選擇");
                Console.WriteLine("❌ 未選擇標籤");
            }

            if (string.IsNullOrWhiteSpace(viewModel.ForumPost.Title))
            {
                ModelState.AddModelError("ForumPost.Title", "標題為必填");
                Console.WriteLine("❌ 標題為空");
            }

            if (string.IsNullOrWhiteSpace(viewModel.ForumPost.Content))
            {
                ModelState.AddModelError("ForumPost.Content", "內容為必填");
                Console.WriteLine("❌ 內容為空");
            }
            if (!ModelState.IsValid)
            {
                Console.WriteLine("🔴 ModelState 錯誤清單：");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"欄位：{key}，錯誤：{error.ErrorMessage}");
                    }
                }
            }


            if (ModelState.IsValid)
            {
                Console.WriteLine("✅ ModelState 通過，準備新增");

                viewModel.ForumPost.PostCategoryId = viewModel.SelectedCategoryId.Value;
                viewModel.ForumPost.FilterId = viewModel.SelectedFilterId.Value;

                // 模擬 UID（請改成登入後的 Session）
                //viewModel.ForumPost.Uid = Guid.NewGuid();
                viewModel.ForumPost.Uid = Guid.Parse("98C1B4DA-677D-416A-87C3-00104AF158F5");
                viewModel.ForumPost.CreatedAt = DateTime.Now;
                viewModel.ForumPost.ViewCount = 0;
                viewModel.ForumPost.LikeCount = 0;
                viewModel.ForumPost.CommentCount = 0;
                viewModel.ForumPost.IsDeleted = false;

                _context.Add(viewModel.ForumPost);
                await _context.SaveChangesAsync();

                TempData["Success"] = "文章已成功新增！";
                Console.WriteLine("✅ 成功寫入資料庫");
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("🔴 ModelState 無效，回傳表單");

            // 若 ModelState 無效，重新載入下拉
            viewModel.Categories = _context.PostCategories
                .Select(c => new SelectListItem { Value = c.PostCategoryId.ToString(), Text = c.PostCategoryName });
            viewModel.Filters = _context.PostFilters
                .Select(f => new SelectListItem { Value = f.PostFilterId.ToString(), Text = f.FilterName });

            return View(viewModel);
        }

        // GET: ForumPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var forumPost = await _context.ForumPosts.FindAsync(id);
            if (forumPost == null) return NotFound();

            var viewModel = new ForumPostViewModel
            {
                ForumPost = forumPost,
                SelectedCategoryId = forumPost.PostCategoryId,
                SelectedFilterId = forumPost.FilterId,
                Categories = _context.PostCategories
                    .Select(c => new SelectListItem
                    {
                        Value = c.PostCategoryId.ToString(),
                        Text = c.PostCategoryName
                    }),
                Filters = _context.PostFilters
                    .Select(f => new SelectListItem
                    {
                        Value = f.PostFilterId.ToString(),
                        Text = f.FilterName
                    })
            };

            return View(viewModel);
        }


        // POST: ForumPosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ForumPostViewModel viewModel)
        {
            if (id != viewModel.ForumPost.PostId) return NotFound();

            if (viewModel.SelectedCategoryId == null || viewModel.SelectedFilterId == null)
            {
                ModelState.AddModelError(string.Empty, "請選擇分類與標籤");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    viewModel.ForumPost.PostCategoryId = viewModel.SelectedCategoryId.Value;
                    viewModel.ForumPost.FilterId = viewModel.SelectedFilterId.Value;

                    _context.Update(viewModel.ForumPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumPostExists(viewModel.ForumPost.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // 若失敗則重新提供選項
            viewModel.Categories = _context.PostCategories
                .Select(c => new SelectListItem { Value = c.PostCategoryId.ToString(), Text = c.PostCategoryName });
            viewModel.Filters = _context.PostFilters
                .Select(f => new SelectListItem { Value = f.PostFilterId.ToString(), Text = f.FilterName });

            return View(viewModel);
        }
        // GET: ForumPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.ForumPosts
                .Include(f => f.PostCategory)
                .Include(f => f.Filter)
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null || post.IsDeleted) return NotFound();

            return View(post);
        }

        // POST: ForumPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.ForumPosts.FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
                return NotFound();

            post.IsDeleted = true; // 軟刪除
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ForumPostExists(int id)
        {
            return _context.ForumPosts.Any(e => e.PostId == id);
        }
    
    }
}
