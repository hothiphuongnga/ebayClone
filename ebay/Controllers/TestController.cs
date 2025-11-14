namespace ebay.Controllers
{
    using System.Text;
    using System.Text.Json;
    using ebay.Base;
    using ebay.Dtos;
    using Microsoft.AspNetCore.Mvc;
    using ebay.Filter;

    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // filter : lọc request, xác thực, phân quyền, logging, caching
        [HttpPost]// routing
        // [LogActionFilter] // áp dụng filter , không dùng DI
        // khi gọi api này sẽ xử lý filter trước khi vào action method
        [ServiceFilter(typeof(LogActionFilter))] // dùng DI trong filter
                [ServiceFilter(typeof(AuthFilter))]

        public async Task<IActionResult> Get([FromBody] UserLoginDTO model) // model biding
        {
            // xử lý Action Method Execution
            Console.WriteLine("🔥 Đang xử lý trong Action Method với id = ");
            // RESULT
            return ResponseEntity<string>.Ok("oke"); // => 200 là oke 
        }

        [HttpPost("demo/{id}")]
        // api/test/demo/2?name=abc
        public async Task<IActionResult> Demo([FromRoute] int id, [FromQuery] string name, [FromHeader] string token) // 
        {

            var idContext = HttpContext.Request.RouteValues["id"];
            string paramContext = HttpContext.Request.Query["name"].ToString();
            // method, : GET POST, PUT, DELETE
            // url, path, query string, headers, body

            var tokenContext = HttpContext.Request.Headers["token"].ToString();
            // clienip
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();


            // lấy thông tin của body từ HttpContext
            // hơi khó hơn vì body có thể là json, xml, formdata, text, ...
            // HttpContext.Request.EnableBuffering(); // cho phép đọc lại body nhiều lần
            // using var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
            // string bodyString = await reader.ReadToEndAsync();
            // HttpContext.Request.Body.Position = 0;
            // Console.WriteLine($"🟢 Body nhận được: {bodyString}");
            // // Parse thủ công nếu muốn
            // var model2 = JsonSerializer.Deserialize<RatingDTO>(bodyString);


            Console.ForegroundColor = ConsoleColor.Green;


            Console.WriteLine("ID from Route: " + idContext);
            Console.WriteLine("Name from Query: " + paramContext);
            Console.WriteLine("Token from Header: " + tokenContext);
            Console.WriteLine("Client IP: " + clientIp);


            // 198.203.203.90:2003
            // chặn ip ngươid dùng : 
            // can thiệp xử lý response  
            HttpContext.Response.Headers.Add("resspon-pnga", "hello bro");

            Console.ResetColor();
            return Ok(
                new
                {
                    Id = id,
                    Name = name,
                    Token = token,
                    ClientIp = clientIp,
                    // Body = model2
                }
            );
        }

        [HttpGet("exFilter")]
        [ServiceFilter(typeof(ExceptionFilter))]
        public async Task<IActionResult> ExFilter()
        {
            throw new Exception("Lỗi thử nghiệm");
            // tạo lỗi để test filter
        }

        [HttpGet("authFilter")]
        [ServiceFilter(typeof(AuthFilter))]
        public async Task<IActionResult> AuthFilter()
        {
            Console.WriteLine("🔥 Đã qua được AuthFilter, đang ở trong Action Method");
            return ResponseEntity<string>.Ok("Bạn đã xác thực thành công và vào được action method");
        }
    }
    // HTTPCONTEXT: lưu trữ thông tin request, response, user, session, ...
    // bao gồm tất cả thông tin gửi lên từ client và trả về từ server trong 1 phiên làm việc - request
    
    // cầu nối trung gian giữa client và server
    // HttpContext được tạo mới cho mỗi request
    // httcontext là "gói hang"
    // middleware là "trạm vận chuyển"



}