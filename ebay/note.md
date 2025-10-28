# Khởi tạo init source

- tạo dự án
    
    ```csharp
    dotnet new webapi -o ebay
    ```
    
- cài đặt
    
    ```csharp
    // web api
    dotnet add package Swashbuckle.AspNetCore --version 9.0.0 
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer 
    dotnet add package Microsoft.EntityFrameworkCore.Tools 
    dotnet add package Microsoft.EntityFrameworkCore 
    dotnet add package Microsoft.EntityFrameworkCore.Design
    
    // có trong goi core roi khong can cai them
    dotnet add package Microsoft.AspNetCore.Components.Web
    dotnet add package Microsoft.AspNetCore.Components.Server
    dotnet add package Microsoft.AspNetCore.StaticFiles
    dotnet add package Microsoft.AspNetCore.SignalR.Core
    ```
    
- cấu hình program
    
    ```csharp
    using Microsoft.EntityFrameworkCore;
    
    var builder = WebApplication.CreateBuilder(args);
    
    // === ĐĂNG KÝ CÁC SERVICE (DEPENDENCY INJECTION) ===
    
    // Đăng ký DbContext, cấu hình sử dụng SQL Server với chuỗi kết nối từ appsettings.json
    // builder.Services.AddDbContext<EBayDbContext>(options =>
    // {
    //     var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
    //     options.UseSqlServer(connectionString);
    // });
    
    builder.Services.AddRazorPages();          // Hỗ trợ Razor Pages
    builder.Services.AddServerSideBlazor();    // Hỗ trợ Blazor Server
    builder.Services.AddControllers();         // Hỗ trợ API Controllers
    builder.Services.AddSwaggerGen();          // Hỗ trợ Swagger (OpenAPI) cho tài liệu API
    
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
    ```
    
- kết nối db
    
    ```csharp
     "ConnectionStrings": {
       "ConnectionString":"Server=103.97.125.207,1433;Database=eBayDB;User Id=dotnetcybersoft;Password=cybersoft321@;TrustServerCertificate=True;"
     },
    ```
    
- scaff db
    
    ```csharp
    dotnet ef dbcontext scaffold "Name=ConnectionString" Microsoft.EntityFrameworkCore.SqlServer -o Models --context-dir Data -f
    ```
    
- Tạo viewmodel mẫu để cài Automapper
    - Tạo DTO . VD : Dtos/ProductDto.cs
    - ProductDTO.cs
        
        ```csharp
        public class ProductDTO
        {
             public int Id { get; set; }
        
            public string? Name { get; set; }
        
            public string? Description { get; set; }
        
            public decimal? Price { get; set; }
        
            public int? Stock { get; set; }
        
            public DateTime? CreatedAt { get; set; }
        }
        ```
        
    
    ```csharp
    dotnet add package AutoMapper
    ```
    
    - Mapping
        - Có thể tạo 1 file hoặc nhiều file
        - ProductMapper.cs
            
            ```csharp
            using AutoMapper;
            using ebay.Dtos;
            using ebay.Models;
            
            public class ProductMapper : Profile
            {
                public ProductMapper()
                {
                    // Constructor
                    // Map 2 chiều nếu tên thuộc tính trùng (ReverseMap)
                    CreateMap<Product, ProductDTO>(); 
                    CreateMap<ProductDTO, Product>(); 
            
                }
            }
            ```
            
        - DI trong program
            
            ```csharp
            // Đăng ký AutoMapper với các profile mapping từ assembly hiện tại
            // Chỉ cần dùng 1 trong các mapper để đăng ký là được , nó sẽ tự tìm các mapper còn lại
            builder.Services.AddAutoMapper(cfg => { }, typeof(CateroryMapper));
            ```
            
- Controller
    - TestController.cs
        
        ```csharp
        
        ```
        
- BLAZOR - Tạo thêm phần cấu trúc như project blazor
    - Pages/HeaderComponent.razor
        
        ```csharp
        @namespace ebay.Pages
        @inject NavigationManager Navigation
        
        <nav class="navbar navbar-expand-lg navbar-light bg-light shadow-sm">
            <div class="container">
                <a class="navbar-brand" href="/">Ebay-Cybersoft</a>
        
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                    <span class="navbar-toggler-icon"></span>
                </button>
        
                <div class="collapse navbar-collapse justify-content-end" id="navbarNav">
                    <ul class="navbar-nav align-items-center">
                        <li class="nav-item">
                            <a class="nav-link" href="/">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="/register">Register</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="/login">Login</a>
                        </li>
                      
                        <li class="nav-item">
                            <a class="nav-link position-relative" href="/cart">
                                <i class="fa fa-cart-plus fs-5"></i> 
                                <span class="badge bg-danger position-absolute top-0 start-100 translate-middle">
                                    @CartItemCount
                                </span>
                            </a>
                        </li>
                        <li class="nav-item ms-2">
                            <span class="text-muted">Tổng: <strong>@TotalPrice.ToString("N0") ₫</strong></span>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
        
        @code {
            // Mock dữ liệu - có thể bind từ service/cart state
            int CartItemCount = 3;
            decimal TotalPrice = 1250000;
        }
        ```
        
    - Shared/MainLayout.razor
        
        ```csharp
        @namespace ebay.Shared
        @inherits LayoutComponentBase
        @using ebay.Pages
        
        <div>
          <HeaderComponent />
            <main>
                @Body
            </main>
        </div>
        ```
        
    - Pages/ _Host.cshtml
        
        ```csharp
        @page "/"
        @namespace ebay.Pages
        @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
        
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="utf-8" />
            <title>Blazor Server</title>
            <base href="~/" />
            <link rel="stylesheet" href="css/site.css" />
        
            <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
        
            <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.1.1/css/all.css" integrity="sha384-O8whS3fhG2OnA5Kas0Y9l3cfpmYjapjI0E4theH4iuMD+pLhbf6JI0jIMfYcK3yZ" crossorigin="anonymous">
        
        </head>
        <body>
            <app>
                <component type="typeof(App)" render-mode="ServerPrerendered" />
            </app>
        
            <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>
            
            <script src="_framework/blazor.server.js"></script>
        
        </body>
        </html>
        ```
        
    - _Imports.razor
        
        ```csharp
        @using System.Net.Http
        @using Microsoft.AspNetCore.Authorization
        @using Microsoft.AspNetCore.Components.Authorization
        @using Microsoft.AspNetCore.Components.Forms
        @using Microsoft.AspNetCore.Components.Routing
        @using Microsoft.AspNetCore.Components.Web
        @using Microsoft.AspNetCore.Components.Web.Virtualization
        @using Microsoft.JSInterop
        @using Microsoft.AspNetCore.Components
        @using System.ComponentModel.DataAnnotations
        @using ebay
        @using ebay.Shared
        @using ebay.Pages
        @using ebay.Models
        @using System.Text.Json
        ```
        
    - App.razor
        
        ```csharp
        <Router AppAssembly="@typeof(Program).Assembly">
            <Found Context="routeData">
                <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
            </Found>
            <NotFound>
                <LayoutView Layout="@typeof(MainLayout)">
                    <p>Không tìm thấy trang</p>
                </LayoutView>
            </NotFound>
        </Router>
        ```