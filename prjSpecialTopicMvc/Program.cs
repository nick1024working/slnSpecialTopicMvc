using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Features.Usedbook.Application.Services;
using prjSpecialTopicMvc.Features.Usedbook.Infrastructure.DataAccess.UnitOfWork;
using prjSpecialTopicMvc.Features.Usedbook.Mapping;
using prjSpecialTopicMvc.Models;

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

// UsedBooks - 註冊 Unit Of Work
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
// UsedBooks - 註冊 AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

// UsedBooks - 註冊應用服務層（Services）
builder.Services.AddScoped<ImageService>();




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
