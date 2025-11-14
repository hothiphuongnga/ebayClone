// RESOURCE FILTER
// chạy NGAY SAU AUTH FILTER
// trước action method

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
namespace ebay.Filter;

public class ResourceFilter : IResourceFilter
{
    private IMemoryCache _cache;
    public ResourceFilter(IMemoryCache cache)
    {
        _cache = cache;
    }
    // dictionary<key,value>
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        // request get product 
        //lấy ra key từ path + querystring để làm key cache
        var key = context.HttpContext.Request.Path.ToString() + context.HttpContext.Request.QueryString.ToString();
        // kiểm tra ds lưu trữ (cache) có key này chưa
        //key = product?page=1&size=10
        if (_cache.TryGetValue(key, out IActionResult cachedResponse))
        {
            // có trong cache => trả về kết quả từ cache
            context.Result = cachedResponse;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Resource Filter] Trả về kết quả từ cache cho key: " + key);
            Console.ResetColor();
        }
        // sẽ tiếp tục xuống action method


    }
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        // lưu kết quả vào cache
        var key = context.HttpContext.Request.Path.ToString() + context.HttpContext.Request.QueryString.ToString();
        _cache.Set(key, context.Result, TimeSpan.FromSeconds(30)); // lưu trong 30s
    }
}
