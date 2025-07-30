using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;

namespace prjSpecialTopicMvc.Controllers
{
    public class MemberController : Controller
    {
        private readonly TeamAProjectContext _context;

        public MemberController(TeamAProjectContext context)
        {
            _context = context;
        }

        // GET: Member
        public async Task<IActionResult> Index(string searchString)
        {
            var users = _context.Users.AsQueryable();
            users = users.Where(u => u.Status == 1);

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerSearch = searchString.ToLower();
                users = users.Where(u =>
                    (u.Phone != null && u.Phone.ToLower().Contains(lowerSearch)) ||
                    (u.Name != null && u.Name.ToLower().Contains(lowerSearch)) ||
                    (u.Email != null && u.Email.ToLower().Contains(lowerSearch)));
            }

            ViewBag.SearchString = searchString;
            return View(await users.ToListAsync());
        }

        // GET: Member/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Uid == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // GET: Member/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Member/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Uid,Phone,Password,Name,Email,Gender,Birthday,Address,RegisterDate,LastLoginDate,Status,Level,AvatarUrl,IsAuthor,AuthorStatus")] User user)
        {
            // 檢查電話是否已存在
            bool phoneExists = await _context.Users.AnyAsync(u => u.Phone == user.Phone);
            if (phoneExists)
            {
                ModelState.AddModelError("Phone", "該電話號碼已被註冊");
            }

            // 檢查Email是否已存在
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "該Email已被註冊");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // 一切OK，設定必要欄位並加入資料庫
            user.Uid = Guid.NewGuid();
            user.Status = 1;
            user.RegisterDate = DateTime.Now;
            user.LastLoginDate = DateTime.Now;
            user.Level = 1;

            // 密碼可在此雜湊，略過
            _context.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Member/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Uid,Phone,Password,Name,Email,Gender,Birthday,Address,RegisterDate,LastLoginDate,Status,Level,AvatarUrl,IsAuthor,AuthorStatus")] User user)
        {
            if (id != user.Uid) return NotFound();

            var originalUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Uid == id);
            if (originalUser == null) return NotFound();

            // 標準化使用者輸入（防止空格、大小寫差異）
            var normalizedPhone = user.Phone?.Trim().ToLower();
            var normalizedEmail = user.Email?.Trim().ToLower();

            // 電話是否已被其他人使用
            bool phoneExists = await _context.Users
                .AnyAsync(u => u.Uid != id && u.Phone.ToLower() == normalizedPhone);
            if (phoneExists)
            {
                ModelState.AddModelError("Phone", "該電話號碼已被其他會員使用");
            }

            // Email 是否已被其他人使用
            bool emailExists = await _context.Users
                .AnyAsync(u => u.Uid != id && u.Email.ToLower() == normalizedEmail);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "該Email已被其他會員使用");
            }

            // 密碼若沒填寫，保留原密碼並移除錯誤訊息
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = originalUser.Password;
                ModelState.Remove(nameof(user.Password));
            }

            // 地址未填寫 → 保留原值
            if (string.IsNullOrWhiteSpace(user.Address))
            {
                user.Address = originalUser.Address;
            }

            // 保留不應被更改的欄位
            user.RegisterDate = originalUser.RegisterDate;
            user.LastLoginDate = originalUser.LastLoginDate;
            user.Status = originalUser.Status;

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Uid)) return NotFound();
                else throw;
            }
        }

        // GET: Member/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Uid == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
      
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                // 設為停用狀態(軟刪除)
                user.Status = 0;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Uid == id);
        }
    }
}
