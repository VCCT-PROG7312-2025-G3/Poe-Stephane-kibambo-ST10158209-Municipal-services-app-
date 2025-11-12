using Microsoft.EntityFrameworkCore;
using Municipal_services_app.Models;
using MunicipalMvcApp.Data;
using Municipal_services_app.Services;

var builder = WebApplication.CreateBuilder(args);

// --- MVC controllers + views ---
builder.Services.AddControllersWithViews();

// --- SQLite connection ---
var dataDir = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDir);
var dbPath = Path.Combine(dataDir, "municipal.db");
var connectionString = $"Data Source={dbPath}";

// --- Register EF Core + SQLite ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// --- Register stores/services as scoped for DI ---
builder.Services.AddScoped<EventStore>();
builder.Services.AddScoped<RequestStore>();

// Authentication + Authorization (Cookie)
builder.Services.AddAuthentication("MunicipalCookie")
    .AddCookie("MunicipalCookie", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "Municipal.Auth";
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// --- Ensure DB exists and seed data ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // create DB & tables if missing
    db.Database.EnsureCreated();
    Seeder.EnsureSeedData(db);

    // Preload EventStore and RequestStore so in-memory indexes are ready
    var eventStore = scope.ServiceProvider.GetRequiredService<EventStore>();
    // If EventStore has a public LoadFromDatabase() method, call it to build indexes.
    // If not needed (constructor already does it), this call is harmless if present.
    eventStore.LoadFromDatabase();

    var requestStore = scope.ServiceProvider.GetRequiredService<RequestStore>();
    requestStore.LoadFromDatabase();
}

// --- Middleware / pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Status/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// authentication must come before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();