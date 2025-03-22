
using System;
using GiaoDienAdmin.Areas.Admin.Data;
using GiaoDienAdmin.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Th?i gian timeout c?a session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<IVnPayService, VnPayService>();
// 4. Đăng ký dịch vụ Email
//Thêm đóng này để lấy thư viện chạy JW
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache(); // Bộ nhớ tạm
builder.Services.AddSession(); // Thêm Session Middleware

/*builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session tồn tại
    options.Cookie.HttpOnly = true; // Chỉ truy cập bằng HTTP, không truy cập từ JavaScript
    options.Cookie.IsEssential = true; // Đảm bảo session luôn hoạt động
});
*/
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<ProductCategoriesService>();
builder.Services.AddHttpClient<ProductService>(); // Đăng ký HttpClient cho ProductService
builder.Services.AddHttpClient<CustomerService>(); // Đăng ký HttpClient cho ProductService
builder.Services.AddHttpClient<EmployeeService>(); // Đăng ký HttpClient cho ProductService
builder.Services.AddHttpClient<ProductCustomersService>(); // Đăng ký HttpClient cho ProductService


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/LoginPhone"; // Trang đăng nhập
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

// Đăng ký HttpClient cho ProductService
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();  // Cấu hình xác thực
app.UseAuthorization();   // Cấu hình phân quyền

app.MapControllerRoute(
   name: "areas",
   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ProductCustomers}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
 pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
