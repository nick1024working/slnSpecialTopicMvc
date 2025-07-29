using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;

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
            var teamAProjectContext = _context.ForumPosts.Include(f => f.Filter).Include(f => f.PostCategory).Include(f => f.UidNavigation);
            return View(await teamAProjectContext.ToListAsync());
        }

        // GET: ForumPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumPost = await _context.ForumPosts
                .Include(f => f.Filter)
                .Include(f => f.PostCategory)
                .Include(f => f.UidNavigation)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (forumPost == null)
            {
                return NotFound();
            }

            return View(forumPost);
        }

        // GET: ForumPosts/Create
        public IActionResult Create()
        {
            ViewData["FilterId"] = new SelectList(_context.PostFilters, "PostFilterId", "PostFilterId");
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategories, "PostCategoryId", "PostCategoryId");
            ViewData["Uid"] = new SelectList(_context.Users, "Uid", "Uid");
            return View();
        }

        // POST: ForumPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Uid,PostCategoryId,FilterId,Title,Content,CreatedAt,ViewCount,LikeCount,CommentCount,IsDeleted")] ForumPost forumPost)
        {
            if (ModelState.IsValid)
            {
                _context.Add(forumPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FilterId"] = new SelectList(_context.PostFilters, "PostFilterId", "PostFilterId", forumPost.FilterId);
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategories, "PostCategoryId", "PostCategoryId", forumPost.PostCategoryId);
            ViewData["Uid"] = new SelectList(_context.Users, "Uid", "Uid", forumPost.Uid);
            return View(forumPost);
        }

        // GET: ForumPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumPost = await _context.ForumPosts.FindAsync(id);
            if (forumPost == null)
            {
                return NotFound();
            }
            ViewData["FilterId"] = new SelectList(_context.PostFilters, "PostFilterId", "PostFilterId", forumPost.FilterId);
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategories, "PostCategoryId", "PostCategoryId", forumPost.PostCategoryId);
            ViewData["Uid"] = new SelectList(_context.Users, "Uid", "Uid", forumPost.Uid);
            return View(forumPost);
        }

        // POST: ForumPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Uid,PostCategoryId,FilterId,Title,Content,CreatedAt,ViewCount,LikeCount,CommentCount,IsDeleted")] ForumPost forumPost)
        {
            if (id != forumPost.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forumPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumPostExists(forumPost.PostId))
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
            ViewData["FilterId"] = new SelectList(_context.PostFilters, "PostFilterId", "PostFilterId", forumPost.FilterId);
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategories, "PostCategoryId", "PostCategoryId", forumPost.PostCategoryId);
            ViewData["Uid"] = new SelectList(_context.Users, "Uid", "Uid", forumPost.Uid);
            return View(forumPost);
        }

        // GET: ForumPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumPost = await _context.ForumPosts
                .Include(f => f.Filter)
                .Include(f => f.PostCategory)
                .Include(f => f.UidNavigation)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (forumPost == null)
            {
                return NotFound();
            }

            return View(forumPost);
        }

        // POST: ForumPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forumPost = await _context.ForumPosts.FindAsync(id);
            if (forumPost != null)
            {
                _context.ForumPosts.Remove(forumPost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForumPostExists(int id)
        {
            return _context.ForumPosts.Any(e => e.PostId == id);
        }
    }
}
