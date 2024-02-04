using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Model; // Make sure this using directive points to the correct namespace where AuthDbContext is located
using WebApplication3.Services;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the configuration object
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();

// Add Identity services and specify the RegisterData and role types
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Password.RequiredLength = 12; // Minimum password length
    options.Password.RequireDigit = true; // Require at least one digit
    options.Password.RequireLowercase = true; // Require at least one lowercase letter
    options.Password.RequireUppercase = true; // Require at least one uppercase letter
    options.Password.RequireNonAlphanumeric = true; // Require at least one non-alphanumeric character
    options.User.RequireUniqueEmail = true;

}).AddEntityFrameworkStores<AuthDbContext>();


builder.Services.AddDbContext<AuthDbContext>(options =>
{
    // Configure your AuthDbContext options here if needed
});


// Add session services

builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.Cookie.IsEssential = true;
    Config.Cookie.SameSite = SameSiteMode.None;
    Config.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    Config.ExpireTimeSpan = TimeSpan.FromSeconds(30);
    Config.LoginPath = "/Login";
});
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.IdleTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<AuditLogService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseSession();
app.Use(async (context, next) =>
{
    var session = context.Session;
    if (session.Keys.Contains("UserId") && !session.IsAvailable)
    {
        context.Response.Redirect("/Login");
        return;
    }

    await next();
});



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();


app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.MapRazorPages();

app.Run();
