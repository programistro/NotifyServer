using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NotifyNet.Application.Interface;
using NotifyNet.Application.Service;
using NotifyNet.Core.Interface;
using NotifyNet.Core.Models;
using NotifyNet.Infrastructure.Data;
using NotifyNet.Infrastructure.Repository;
using NotifyNet.Web;
using NotifyNet.Web.Hubs;
using NotifyNet.Web.Service;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.UseUrls("http://0.0.0.0:8080");
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<CheckerBackgroundService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<CheckerBackgroundService>());
builder.Services.AddDbContext<AppDbConetxt>();

builder.Services.AddIdentity<Employee, Permission>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    // options.Password.RequireNonLetterOrDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.SignIn.RequireConfirmedAccount = false;
    options.Lockout.AllowedForNewUsers = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 10;
}).
AddEntityFrameworkStores<AppDbConetxt>().
AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(90);
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true; 
    });

builder.Services.AddAuthorization();

builder.Services.AddCors();
builder.Services.AddSignalR();
builder.Services.AddControllers();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseCors(builder => builder.WithOrigins("https://api.re.souso.ru/").AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<OrderHub>("/orderHub");

app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.MapControllers();

app.Run();