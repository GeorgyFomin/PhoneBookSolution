using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAppClaims.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=WebAppClaimsDb;Trusted_Connection=True;"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Account/Register");
    });
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("OnlyForRussia", policy => { policy.RequireClaim(ClaimTypes.Locality, "Россия", "Russia"); });
    opts.AddPolicy("OnlyForScillbox", policy =>
    {
        policy.RequireClaim("company", "Scillbox");
    });
});
// встраиваем сервис AgeHandler
builder.Services.AddTransient<IAuthorizationHandler, AgeHandler>();

builder.Services.AddAuthorization(opts =>
{
    // устанавливаем ограничение по возрасту
    opts.AddPolicy("AgeLimit", policy => policy.Requirements.Add(new AgeRequirement(18)));
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

