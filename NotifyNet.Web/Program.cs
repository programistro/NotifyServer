using AXO.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NotifyNet.Application.Interface;
using NotifyNet.Application.Service;
using NotifyNet.Core.Interface;
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
builder.Services.AddSingleton<ConnectionManager>();
builder.Services.AddDbContextFactory<AppDbConetxt>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = false; 
});
builder.Services.AddIdentity<Employee, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbConetxt>()
    .AddSignInManager<SignInManager<Employee>>();

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

app.MapControllers();

app.Run();