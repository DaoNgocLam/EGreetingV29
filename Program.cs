/*------------DEVELOP AND SOURCE CODE RUBY EGREETING TEAM(APTECH EDU) davedthien@gmail.com && ngoclam.dao@gmail.com FOLLOW US ON LINKED------------ */

using EGreeting.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using EGreeting.Models;
using EGreeting.Services;


//using Microsoft.Extensions.DependencyInjection;

// Ask the service provider for the configuration abstraction.
using IHost host = Host.CreateDefaultBuilder(args).Build();
IConfiguration _configuration = host.Services.GetRequiredService<IConfiguration>();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();


// Đăng ký SendMailService với kiểu Transient, mỗi lần gọi dịch
// vụ ISendMailService một đới tượng SendMailService tạo ra (đã inject config)
builder.Services.AddTransient<ISendMailService, SendMailService>();



// To MVC USE using EnableEndpointRouting = false
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
builder.Services.AddControllersWithViews();

//Add Session

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    //options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Read config
var mailsettings = _configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsettings);
builder.Services.Configure<IdentityOptions>(options =>
{
    //options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");   
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "areas",
        template: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
        );
});


app.MapRazorPages();

app.Run();
