using Microsoft.EntityFrameworkCore;
using MunicipalMvcApp.Data; 

var builder = WebApplication.CreateBuilder(args);

// MVC controllers + views
builder.Services.AddControllersWithViews();

// --- SQLite connection (App_Data/municipal.db) ---
var configured = builder.Configuration.GetConnectionString("DefaultConnection");

// Ensure App_Data exists and build an absolute path
var dataDir = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDir);
var dbPath = Path.Combine(dataDir, "municipal.db");

// If the connection string points to App_Data, replace with absolute path
var connectionString = string.IsNullOrWhiteSpace(configured)
    ? $"Data Source={dbPath}"
    : configured.Contains("App_Data", StringComparison.OrdinalIgnoreCase)
        ? $"Data Source={dbPath}"
        : configured;

// Register EF Core + SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));



var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//to /Home/Status/{code}
app.UseStatusCodePagesWithReExecute("/Home/Status/{0}");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
