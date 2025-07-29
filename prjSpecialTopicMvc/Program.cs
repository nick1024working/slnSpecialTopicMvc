using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Models.MEbook;

var builder = WebApplication.CreateBuilder(args);

// 讀取連線字串
var connectionString = builder.Configuration.GetConnectionString("Default");

// 註冊 DbContext
builder.Services.AddDbContext<TeamAProjectContext>(options =>
{
    options.UseSqlServer(connectionString,
        sql => sql.MigrationsAssembly(typeof(TeamAProjectContext).Assembly.FullName));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// ========== 各自需要的服務於以下註冊 ==========


// 下方的 EbookService 是 IEbookService 的實作類別，請換成您實際的類別名稱
builder.Services.AddScoped<IEbookService, EbookService>();
// !!! 請在這裡加入 IFileService 的註冊 !!!
// 假設您的實作類別叫做 FileService，請根據您的專案修改
builder.Services.AddScoped<IFileService, FileService>();
// 註解：註冊我們的分類服務。注意 using 的是新命名空間

builder.Services.AddScoped<IEBookCategoryService, EBookCategoryService>();

// 【*** 新增此行 ***】
builder.Services.AddScoped<ILabelsService, LabelsService>();






var app = builder.Build();

// ========== 各自註冊的服務在於下啟動(自行注意順序) ==========



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();

// 屬性路由可與傳統路由並存，但要注意衝突與優先順序。
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
