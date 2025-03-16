using BusinessLogic.Mappings;
using BusinessLogic.Services;
using DataAccessObject;
using DataAccessObject.Models.Helper;
using DataAccessObject.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
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

// AddScoped Service
builder.Services.AddScoped<IUserService, UserService>();

// Add Authentication (Cookie-based)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/User/AccessDenied"; 
    });

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddRazorPages();

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

// Enable session
app.UseSession();

// Enable authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();