
using ebay.Data;
using ebay.Models;
using Microsoft.EntityFrameworkCore;

public class BlockIpMiddleWare(EBayDbContext _contextdb) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        // lấy ra ip thật , nếu không xác định  thì giá trị ip là Unknown
        if (ip == "Unknown")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Unknown Ip Address.");
            return;
        }
        // ip spam nhiều lần trong 1 khoảng thời gian ngắn. VD 10p 5 request ...

        DateTime now = DateTime.UtcNow; // utc nó dạng 
        DateTime windowStart = now.AddMinutes(-10); // 10 phút trước
        int requestCount = 10; // số request tối đa trong khoảng thời gian
        // luuw ds i[p bij block 
        // ds luuw ip


        // kieerm tra xem cos ip trong khoang thoi gian ko
        var recentConnections = await _contextdb.ConnectionCountLogs
            .SingleOrDefaultAsync(c => c.IpAddress == ip && c.ConnectionTime >= windowStart);


        // luuw ip khi cos request
        if (recentConnections == null)
        {
            // lưu lại
            var newLog = new ConnectionCountLog
            {
                IpAddress = ip,
                ConnectionTime = now,
                ConnectionCount = 1,
                CreatedAt = now,
                UpdatedAt = now
            };
            await _contextdb.ConnectionCountLogs.AddAsync(newLog);
        }
        else
        {
            // đã có ip trong khoang thoi gian
            recentConnections.ConnectionCount += 1; // tăng số lần kết nối
            recentConnections.UpdatedAt = now; // cập nhật thời gian
            _contextdb.ConnectionCountLogs.Update(recentConnections); // cập nhật vào db

            if (recentConnections.ConnectionCount > requestCount)
            {
                // block ip
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Your IP has been blocked due to excessive requests.");
                return; // dừng 
            }
        }
        await _contextdb.SaveChangesAsync();
        await next(context);
    }
}