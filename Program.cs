using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Week5Project.Data;
using Week5Project.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Razor Pages
builder.Services.AddRazorPages();

// 2. Session yapılandırması
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. DbContext
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

// 4. Identity servisi
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<SchoolDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

var app = builder.Build();

// 5. Middleware sıralaması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Session, Authentication ve Authorization
app.UseSession();             
app.UseRouting();

app.UseAuthentication();      // Kimlik doğrulama
app.UseAuthorization();       // Yetkilendirme

// 6. Endpoint’ler
app.MapRazorPages();

app.Run();
