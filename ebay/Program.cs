using ebay.Data;
using ebay.Repositories;
using ebay.Serrvices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === ĐĂNG KÝ CÁC SERVICE (DEPENDENCY INJECTION) ===

// Đăng ký DbContext, cấu hình sử dụng SQL Server với chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<EBayDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
    options.UseSqlServer(connectionString);
});


// Đăng ký AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(RatingMapper));


builder.Services.AddRazorPages();          // Hỗ trợ Razor Pages
builder.Services.AddServerSideBlazor();    // Hỗ trợ Blazor Server
builder.Services.AddSwaggerGen();          // Hỗ trợ Swagger (OpenAPI) cho tài liệu API
builder.Services.AddControllers();         // Hỗ trợ API Controllers

// DI REPOSITORY
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// scpoe: tạo mới mỗi lần request
//


// DI SERVICES
builder.Services.AddScoped<IProductService,ProductService>();



var app = builder.Build();

// === CẤU HÌNH MIDDLEWARE PIPELINE ===

// Kích hoạt Swagger & giao diện Swagger UI cho API docs & thử nghiệm
app.UseSwagger();
app.UseSwaggerUI();

// Tự động chuyển hướng HTTP sang HTTPS (bảo mật)
app.UseHttpsRedirection();

// Cho phép truy cập các file tĩnh (CSS, JS, ảnh, ...)
app.UseStaticFiles();

// Kích hoạt định tuyến
app.UseRouting();

// Map các endpoint cho Controller API, RazorPages, Blazor và fallback
app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();