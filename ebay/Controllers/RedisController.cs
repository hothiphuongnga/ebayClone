namespace ebay.Controllers
{
    using System.Text.Json;
    using AutoMapper;
    using ebay.Base;
    using ebay.Data;
    using ebay.Dtos;
    using ebay.Helper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Distributed;

    [Route("api/[controller]")]
    [ApiController]
    public class RedisController(
        EBayDbContext _db,
        IMapper _map,
        IDistributedCache _cache,
        RedisHelper _redisHelper
    ) : ControllerBase
    {
        
        // cache product list 
        [HttpGet("cache-products")]
        public async Task<IActionResult> GetNewestProduct()
        {
            // key cache-products-100
            string cacheKey = "cache-products-100";
            // kiểm tra cache 
            string? cacheData = await _cache.GetStringAsync(cacheKey);
            if(cacheData is not null)
            {
                var resCache = JsonSerializer.Deserialize<List<ProductDTO>>(cacheData);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[REDIS - CACHE HIT]");
                Console.ResetColor();
                return ResponseEntity<List<ProductDTO>?>.Ok(resCache, "Lấy từ cache");
            }

            // lấy ra ds 100 sp mới nhất (id giảm dần)
            var prods = await _db.Products
            .OrderByDescending(a => a.Id)
            .Take(100)
            .Include(p => p.ProductImages)
            .Include(p => p.Ratings)
            .ToListAsync();
            var res = _map.Map<List<ProductDTO>>(prods);

            // lưu vào redis
            var resString = JsonSerializer.Serialize(res);
            await _cache.SetStringAsync(cacheKey, resString, new DistributedCacheEntryOptions
            {
                // đặt thời gian hết hạn cache
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });// tham số thử 3 có thể có hoặc không
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[REDIS - CACHE SAVE]");
            Console.ResetColor();


               // lấy ra key từ path + querystring để làm key cache
            // lưu vào cache, tường tự trong filter resource  nếu như không dùng filter 
            return ResponseEntity<List<ProductDTO>>.Ok(res, "Lấy thành công 100 sp mới nhất");
        }

        [HttpGet("cache-product-v2")]
        public async Task<IActionResult> GetNewestProductV2()
        {
            // key cache-products-100
            string cacheKey = "cache-products-100-v2";
            var cacheData = await _redisHelper.GetAsync<List<ProductDTO>>(cacheKey);
            if(cacheData is not null)
            {
                Console.WriteLine("[REDIS - HELPER]");
                return ResponseEntity<List<ProductDTO>?>.Ok(cacheData, "Lấy từ redis v2");
            }
            // lấy ra ds 100 sp mới nhất (id giảm dần)
            var prods = await _db.Products
            .OrderByDescending(a => a.Id)
            .Take(100)
            .Include(p => p.ProductImages)
            .Include(p => p.Ratings)
            .ToListAsync();
            var res = _map.Map<List<ProductDTO>>(prods);

            // lưu vào redis
            await _redisHelper.SetAsync(cacheKey, res);
            Console.WriteLine("[REDIS - HELPER SAVE]");
            return ResponseEntity<List<ProductDTO>>.Ok(res, "Lấy thành công 100 sp mới nhất và lưu vào redis v2");
        }
    }
}