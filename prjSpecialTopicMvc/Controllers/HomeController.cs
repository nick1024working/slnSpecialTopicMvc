using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;

namespace prjSpecialTopicMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly TeamAProjectContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(TeamAProjectContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 資料庫測試，取得語言實體清單
        /// </summary>
        /// <returns></returns>
        public IActionResult TestList()
        {
            IEnumerable<Language> languageList = _db.Languages
                .AsNoTracking()
                .OrderBy(l => l.Id)
                .ToList();

            return View(languageList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
