using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using ebay.Base;
using ebay.Data;
using ebay.Filter;
using ebay.Repositories;
using ebay.Serrvices;
using ebay.ServicesBlazor;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

builder.Services.AddControllers(options=>{
    options.Filters.AddService<LogActionFilter>(); // đăng ký filter toàn cục , tất cả api đều áp dụng
});         // Hỗ trợ API Controllers


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // 🔥 Thêm hỗ trợ Authorization header tất cả api
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token vào ô bên dưới theo định dạng: Bearer {token}"
    });

    // 🔥 Định nghĩa yêu cầu sử dụng Authorization trên từng api
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// DI REPOSITORY
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// scpoe: tạo mới mỗi lần request
//


// DI SERVICES
builder.Services.AddScoped<IProductService, ProductService>();



// ĐĂNG KÝ HTTPCLIENT
builder.Services.AddHttpClient();

// Đăng ký LocalStorage
builder.Services.AddBlazoredLocalStorage();

// jwtstate
builder.Services.AddScoped<AuthenticationStateProvider, JwtStateService>();

// DI JWT SERVICE
builder.Services.AddScoped<IJwtAuthService, JwtAuthService>();


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
    options.AddPolicy("allowAny", builder =>
    {
        builder.AllowAnyOrigin()// cho phép bất kỳ domain nào
               .AllowAnyHeader() // heder
               .AllowAnyMethod(); //method
    });
});


// === Câu hình AUTHEN, AUTHOR ===
var privateKey = builder.Configuration["jwt:Serect-Key"];
var Issuer = builder.Configuration["jwt:Issuer"];
var Audience = builder.Configuration["jwt:Audience"];

// cấu hình cơ bản
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(  options =>
{
    // Thiết lập các tham số xác thực token
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        // Kiểm tra và xác nhận Issuer (nguồn phát hành token)
        ValidateIssuer = true,
        ValidIssuer = Issuer, // Biến `Issuer` chứa giá trị của Issuer hợp lệ
                              // Kiểm tra và xác nhận Audience (đối tượng nhận token)
        ValidateAudience = true,
        ValidAudience = Audience, // Biến `Audience` chứa giá trị của Audience hợp lệ
                                  // Kiểm tra và xác nhận khóa bí mật được sử dụng để ký token
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey)),
        // Sử dụng khóa bí mật (`privateKey`) để tạo SymmetricSecurityKey nhằm xác thực chữ ký của token
        // Giảm độ trễ (skew time) của token xuống 0, đảm bảo token hết hạn chính xác
        ClockSkew = TimeSpan.Zero,
        // Xác định claim chứa vai trò của user (để phân quyền)
        RoleClaimType = ClaimTypes.Role,
        // Xác định claim chứa tên của user
        NameClaimType = ClaimTypes.Name,
        // Kiểm tra thời gian hết hạn của token, không cho phép sử dụng token hết hạn
        ValidateLifetime = true
    };
    // cấu hình response theo chuẩn ResponseEntity của dự án
    options.Events = new JwtBearerEvents
    {
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden; // 403 => không có quyền , 401 => chưa xác thực
            context.Response.ContentType = "application/json";
            var response = JsonSerializer.Serialize(ResponseEntity<string>.Fail("Bạn không có quyền truy cập tài nguyên này.", 403),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return context.Response.WriteAsync(response);
        },
        OnChallenge = context => // khi không có token hoặc token không hợp lệ
        {
            context.HandleResponse(); // 
            context.Response.StatusCode = StatusCodes.Status401Unauthorized; // 401
            context.Response.ContentType = "application/json";
            var response = JsonSerializer.Serialize(ResponseEntity<string>.Fail("Yêu cầu xác thực. Vui lòng đăng nhập.", 401),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return context.Response.WriteAsync(response);
        }
    };

});


builder.Services.AddAuthorization();


// Đăng ký Middleware BlockIpMiddleWare
builder.Services.AddScoped<BlockIpMiddleWare>();


// DI  FILTER
builder.Services.AddScoped<LogActionFilter>();

builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddScoped<AuthFilter>();

builder.Services.AddScoped<ResourceFilter>();
builder.Services.AddScoped<ResultFilter>();

var app = builder.Build();

// === CẤU HÌNH MIDDLEWARE PIPELINE ===
if (app.Environment.IsDevelopment())
{
    // Môi trường dev: show trang lỗi chi tiết
    app.UseDeveloperExceptionPage();
// }
// else // Production => lỗi chung format đẹp
// {
    app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        // Đặt response content-type thành JSON
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        // Trả về JSON chứa thông tin lỗi
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("[Middleware] Đã xảy ra lỗi: " + exceptionFeature?.Error.Message);
        Console.ResetColor();
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
app.UseMiddleware<BlockIpMiddleWare>();

app.UseRouting();

app.UseAuthentication(); // Xác thực
app.UseAuthorization();  // Phân quyền

// Sử dụng Middleware chặn IP xấu

// Map các endpoint cho Controller API, RazorPages, Blazor và fallback
app.MapControllers();



app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();