using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Infrastructure.Data;
using VeterinaryClinic.Application.Services;
using VeterinaryClinic.Application.Interfaces;
using VeterinaryClinic.Application.Services.AnimalServices;
using VeterinaryClinic.Infrastructure.Repositories;
using VeterinaryClinic.Infrastructure.Identity;
using ClinicDbContext = VeterinaryClinic.Infrastructure.Data.ApplicationDbContext;
using ClinicUser = VeterinaryClinic.Infrastructure.Identity.ApplicationUser;
using VeterinaryClinic.Application.Users;
using VeterinaryClinic.Web.Identity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ClinicDbContext>(options =>
    options.UseSqlServer(connectionString, options =>
    options.MigrationsAssembly("VeterinaryClinic.Web")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<ClinicUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ClinicDbContext>()
    .AddSignInManager<ActiveUserSignInManager>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IGreetingService, GreetingService>();
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IUserAdministrationService, UserAdministrationService>();
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySetup.EnsureRolesAsync(roleManager);
    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ClinicUser>>();
    var initialAdmin = app.Configuration.GetSection("InitialAdmin");
    var enabled = bool.TryParse(
        initialAdmin["Enabled"], out var value) && value;
    await InitialAdminSetup.EnsureAsync(
        userManager,
        enabled,
        initialAdmin["Email"],
        initialAdmin["Password"]);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
