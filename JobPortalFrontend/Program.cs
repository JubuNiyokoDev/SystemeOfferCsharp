using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Ajouter HttpClient
builder.Services.AddHttpClient();


// Ajouter les services MVC avec vues
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Redirige les utilisateurs non authentifi�s
        options.AccessDeniedPath = "/Account/AccessDenied"; // En cas d'acc�s interdit
    });

var app = builder.Build();

// Configurer le pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Activer HSTS pour la s�curit�
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Activer les fichiers statiques (CSS, JS, etc.)
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// Configurer les routes par d�faut
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
