using Microsoft.EntityFrameworkCore;
using prjSpecialTopicMvc.Models;
using prjSpecialTopicMvc.Models.MEbook;

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


// �U�誺 EbookService �O IEbookService ����@���O�A�д����z��ڪ����O�W��
builder.Services.AddScoped<IEbookService, EbookService>();
// !!! �Цb�o�̥[�J IFileService �����U !!!
// ���]�z����@���O�s�� FileService�A�Юھڱz���M�׭ק�
builder.Services.AddScoped<IFileService, FileService>();
// ���ѡG���U�ڭ̪������A�ȡC�`�N using ���O�s�R�W�Ŷ�

builder.Services.AddScoped<IEBookCategoryService, EBookCategoryService>();

// �i*** �s�W���� ***�j
builder.Services.AddScoped<ILabelsService, LabelsService>();






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
