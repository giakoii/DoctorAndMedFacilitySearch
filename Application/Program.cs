using BusinessLogic.Mappings;
using BusinessLogic.Services;
using DataAccessObject;
using DataAccessObject.Models;
using DataAccessObject.Models.Helper;
using DataAccessObject.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add Db Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Auto Mapper
builder.Services.AddAutoMapper(typeof(AutoMapping));

// AddScoped Base
builder.Services.AddScoped(typeof(IBaseRepository<,,>), typeof(BaseRepository<,,>));
builder.Services.AddScoped(typeof(IBaseService<, ,>), typeof(BaseService<, ,>));
builder.Services.AddScoped<IBaseService<DoctorProfile, int, VwDoctorProfile>, BaseService<DoctorProfile, int, VwDoctorProfile>>();
builder.Services.AddScoped<IBaseService<PatientProfile, int, VwPatientProfile>, BaseService<PatientProfile, int, VwPatientProfile>>();

// AddScoped Service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None; // 🔥
});

// Config Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = new PathString("/signin-google");
        options.SaveTokens = true;
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None; // 🔥 Giữ session khi redirect từ Google
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();