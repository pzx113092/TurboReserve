using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TurboReserve.Data;
using TurboReserve;
using TurboReserve.Services;



var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


// Dodajemy Identity z obsługą ról:
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // konfiguracja opcji logowania, haseł itp.
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>() // WAŻNE: ta linijka dodaje RoleManager i wsparcie ról
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IServiceProviderService, ServiceProvidersController>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IServiceService, ServiceService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedDataAsync(services);
}

app.Run();




