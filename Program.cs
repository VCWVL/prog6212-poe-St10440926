using QuestPDF.Infrastructure;
using Microsoft.EntityFrameworkCore;
using st10440926_poeparttwo.Data;


var builder = WebApplication.CreateBuilder(args);

// ? REQUIRED BY QUESTPDF — FREE COMMUNITY LICENSE
QuestPDF.Settings.License = LicenseType.Community;

// MVC + Session
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// -----------------------------------------
// DATABASE CONNECTION (EF CORE)
// -----------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CMCSdb")));

var app = builder.Build();




app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();



/*
------------------------------------------------------------
REFERENCES
------------------------------------------------------------

Microsoft (2024) ASP.NET Core MVC overview. Microsoft Learn. 
Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/overview 
(Accessed: 22 October 2025).

Microsoft (2024) File uploads in ASP.NET Core. Microsoft Learn. 
Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads 
(Accessed: 22 October 2025).

Microsoft (2024) System.Text.Json Namespace (Serialization and Deserialization). 
Microsoft Docs. 
Available at: https://learn.microsoft.com/en-us/dotnet/api/system.text.json 
(Accessed: 22 October 2025).

Microsoft (2024) System.Security.Cryptography Namespace (AES Encryption). 
Microsoft Docs. 
Available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography 
(Accessed: 22 October 2025).

Microsoft (2024) Unit testing C# code with MSTest. Microsoft Learn. 
Available at: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest 
(Accessed: 22 October 2025).

Microsoft (2024) TempData, ViewData, and ViewBag in ASP.NET Core MVC. 
Microsoft Learn. 
Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state 
(Accessed: 22 October 2025).

TutorialsTeacher (2024) ASP.NET Core MVC – Controllers, Views and Models Overview. 
TutorialsTeacher. 
Available at: https://www.tutorialsteacher.com/core/aspnet-core-mvc 
(Accessed: 22 October 2025).

------------------------------------------------------------
End of References
------------------------------------------------------------
*/
