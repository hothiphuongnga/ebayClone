namespace ebay.Controllers
{
    using AutoMapper;
    using ebay.Base;
    using ebay.Data;
    using ebay.Dtos;
    using ebay.Filter;
    using ebay.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    [Route("api/[controller]")]
    [ApiController]
    public class DemoCacheController(
        EBayDbContext _db,
        IMapper _map,
        IMemoryCache _cache
        ) : ControllerBase
    {

        // api lấy ra 100 sp mới nhất
        //  có cache dùng resource filter đã viết 
        [HttpGet("newest")]
        [ServiceFilter(typeof(ResourceFilter))]
        public async Task<IActionResult> GetNewestProduct()
        {
            // lấy ra ds 100 sp mới nhất (id giảm dần)
            var prods = await _db.Products.OrderByDescending(a => a.Id).Take(300).ToListAsync();
            var res = _map.Map<List<ProductDTO>>(prods);


               // lấy ra key từ path + querystring để làm key cache
            // lưu vào cache, tường tự trong filter resource  nếu như không dùng filter 
            return ResponseEntity<List<ProductDTO>>.Ok(res, "Lấy thành công 100 sp mới nhất");
        }


        // khi có sp mới / thêm xoá sửa sp thì cần xoá cache có key /api/DemoCache/newest
        [HttpPost("product")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO prodDto)
        {
            // map dto -> entity
            var prod = _map.Map<Product>(prodDto);
            await _db.Products.AddAsync(prod);
            // lưu vào db
            await _db.SaveChangesAsync();


            // xoá cache có key /api/DemoCache/newest

            string key = "/api/DemoCache/newest";
            // kiểm tra có key này trong cache không
            if (_cache.TryGetValue(key, out IActionResult cachedResponse))
            {
                _cache.Remove(key);   
            }
            return ResponseEntity<Product>.Ok(prod, "Tạo sản phẩm thành công");
        }

    }
}