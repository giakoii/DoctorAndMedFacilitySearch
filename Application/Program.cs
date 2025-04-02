using Application.Hubs;
using BusinessLogic.Logics.MomoLogics;
using BusinessLogic.Mappings;
using BusinessLogic.Services;
using Client.Logics.Commons.MomoLogics;
using DataAccessObject;
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
builder.Services.AddScoped(typeof(IBaseService<,,>), typeof(BaseService<,,>));
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
// AddScoped Service
builder.Services.AddSignalR();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IMedicalFacilityService, MedicalFacilityService>();
builder.Services.AddScoped<IDoctorProfileService, DoctorProfileService>();
builder.Services.AddScoped<IPatientProfileService, PatientProfileService>();
builder.Services.AddScoped<IDoctorAppointmentsService, DoctorAppointmentsService>();
builder.Services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
builder.Services.AddScoped<BusinessLogic.Services.Appointment.IAppointmentService, BusinessLogic.Services.Appointment.AppointmentService>();

builder.Services.AddScoped<IMomoService, MomoService>();
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));

builder.Services.AddScoped<IReviewService, ReviewService>();

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
app.MapHub<ReviewHub>("/reviewHub");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapFallbackToPage("/Care");
});

app.Run();