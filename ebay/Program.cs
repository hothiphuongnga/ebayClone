using System.Net;
using ebay.Base;
using ebay.Data;
using ebay.Repositories;
using ebay.Serrvices;
using ebay.ServicesBlazor;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

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
builder.Services.AddScoped<IProductService, ProductService>();



// ĐĂNG KÝ HTTPCLIENT
builder.Services.AddHttpClient();



// === Đăng ký service state ===
builder.Services.AddScoped<IProductPageService, ProductPageService>();


// Cấu hình cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500","") // cho phép domain nào thì LIỆT KÊ trong này
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// cors cho tất cả 
builder.Services.AddCors(options =>
{
    options.AddPolicy("allowAny",builder =>
    {
        builder.AllowAnyOrigin()// cho phép bất kỳ domain nào
               .AllowAnyHeader() // heder
               .AllowAnyMethod(); //method
    });
});



var app = builder.Build();

// === CẤU HÌNH MIDDLEWARE PIPELINE ===
if (app.Environment.IsDevelopment())
{
    // Môi trường dev: show trang lỗi chi tiết
    app.UseDeveloperExceptionPage();
}
else // Production => lỗi chung format đẹp
{
    app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        // Đặt response content-type thành JSON
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        // Trả về JSON chứa thông tin lỗi
        var errorResponse = new ResponseEntity<string>
        {
            StatusCode = context.Response.StatusCode,
            Message = "Đã xảy ra lỗi trên server.",
            Content = exceptionFeature?.Error.Message
        };


        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});
    // Môi trường production: sử dụng trang lỗi chung chung
    // app.UseExceptionHandler("/Error");
    // Bảo vệ ứng dụng khỏi các cuộc tấn công XSS, clickjacking, v.v.
    app.UseHsts();
}



//CORS
// cái nào trước thì áp dụng trước
app.UseCors("allowAny"); // đổi tên policy tương ứng
app.UseCors("AllowLocal"); // đổi tên policy tương ứng


// exception handling middleware

// Kích hoạt Swagger & giao diện Swagger UI cho API docs & thử nghiệm
app.UseSwagger();
app.UseSwaggerUI();
// middleware blockip => chặn ip xấu, chặn ddos spam request  

// Tự động chuyển hướng HTTP sang HTTPS (bảo mật)
app.UseHttpsRedirection();

// Cho phép truy cập các file tĩnh (CSS, JS, ảnh, ...)
// cho  phép truy cập file trong wwwroot 
// nhứng không có mã hoá , không cần biên dịch

// app.UseStaticFiles(); // truy cập trực tiếp các file tinh trong wwwroot

//custom static file location 
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Files")),

    RequestPath = "/media",// truy cập file tĩnh thông qua đường dẫn /Files => ví dụ: url:port/abc.png
    OnPrepareResponse = a =>
    {
        int duration = 60 * 60 * 24 * 7; // cache trong 7 ngày
        a.Context.Response.Headers["Cache-Control"] = "public,max-age=" + duration;
        var path = a.File.PhysicalPath; // đường dẫn vật lý của file
        // có thể kiểm tra định dạng file néu là html thì không cho truy cập
        if(path.EndsWith(".html"))
        {
            a.Context.Response.StatusCode = (int)HttpStatusCode.NotFound; // trả về 404
            a.Context.Response.ContentLength = 0; // không có nội dung
            a.Context.Response.Body = Stream.Null;// không trả về nội dung
        }
    }

});

// Kích hoạt định tuyến
app.UseRouting();

// Map các endpoint cho Controller API, RazorPages, Blazor và fallback
app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();