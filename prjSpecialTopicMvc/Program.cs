using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Features.Usedbook.Application.Services;
using prjSpecialTopicMvc.Features.Usedbook.Infrastructure.DataAccess.UnitOfWork;
using prjSpecialTopicMvc.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicMvc.Features.Usedbook.Mapping;
using prjSpecialTopicMvc.Models;

var builder = WebApplication.CreateBuilder(args);

// Ū���s�u�r��
var connectionString = builder.Configuration.GetConnectionString("Default");

// ���U DbContext
builder.Services.AddDbContext<TeamAProjectContext>(options =>
{
    options.UseSqlServer(connectionString,
        sql => sql.MigrationsAssembly(typeof(TeamAProjectContext).Assembly.FullName));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// ========== �U�ۻݭn���A�ȩ�H�U���U ==========
// ���U����
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/usedbooks/auth";
        options.AccessDeniedPath = "/usedbooks/auth/denied";
        options.Cookie.Name = "usedbook_auth";
    });

// ���U���v
builder.Services.AddAuthorization();
// UsedBooks - ���U Unit Of Work
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
// UsedBooks - ���U AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });
// UsedBooks - ���U���ΪA�ȼh�]Services�^
// UsedBooks �A�� Lookup ��
builder.Services.AddScoped<BookBindingRepository>();
builder.Services.AddScoped<BookConditionRatingRepository>();
builder.Services.AddScoped<ContentRatingRepository>();
builder.Services.AddScoped<CountyRepository>();
builder.Services.AddScoped<DistrictRepository>();
builder.Services.AddScoped<LanguageRepository>();
// UsedBooks �A�Ȩϥ�
builder.Services.AddScoped<BookSaleTagRepository>();
builder.Services.AddScoped<UsedBookRepository>();
builder.Services.AddScoped<BookSaleTagRepository>();
builder.Services.AddScoped<BookCategoryRepository>();
builder.Services.AddScoped<UsedBookImageRepository>();
// ���U���ΪA�ȼh�]Services�^
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<LookupService>();
builder.Services.AddScoped<UsedBookService>();
builder.Services.AddScoped<BookCategoryService>();
builder.Services.AddScoped<BookSaleTagService>();
builder.Services.AddScoped<UsedBookImageService>();





var app = builder.Build();

// ========== �U�۵��U���A�Ȧb��U�Ұ�(�ۦ�`�N����) ==========



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

// �ݩʸ��ѥi�P�ǲθ��Ѩæs�A���n�`�N�Ĭ�P�u�����ǡC
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
