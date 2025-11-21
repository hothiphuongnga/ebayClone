namespace ebay.Controllers
{
    using AutoMapper;
    using ebay.Base;
    using ebay.Data;
    using ebay.Dtos;
    using ebay.Filter;
    using ebay.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductController(IProductService _service) : ControllerBase
    {
        // private readonly EBayDbContext _context;
        // public ProductController(EBayDbContext context)
        // {
        //     _context = context;
        // }
        // [HttpGet]
        // public async Task<IActionResult> Get()
        // {
        //     // Get 20 products from database
        //     var products = await _context.Products
        //     .Include(a => a.Ratings)
        //     .Include(a => a.ProductImages)
        //     .Include(a => a.OrderDetails)
        //     .Take(20)
        //     .ToListAsync();

        //     // chuyển về DTO để không lỗi vòng lặp vô hạn khi trả về JSON
        //     // dùng automapper vveer chuyển về dto 
        //     var res = _mapper.Map<List<ProductDTO>>(products);

        //     return Ok(res); // => 200 là oke
        // }


        [HttpGet("test")]
        // [filterA] xử lý filter
        public async Task<IActionResult> Get20()
        {
            var res = await _service.Get20ProductsAsync();
            return ResponseEntity<List<ProductDTO>>.Ok(res.ToList(), "Lấy thành công"); // => 200 là oke
        }

        [HttpGet("paging")]
        // gắn filter cache cho api này
        [ServiceFilter(typeof(ResourceFilter))]
        // [ServiceFilter(typeof(ResultFilter))]
        public async Task<IActionResult> GetPaging(int pageIndex = 1,int pageSize = 10,string? search = null)
        {
            var res = await _service.GetProductsPagingAsync(pageIndex, pageSize, search);
            return ResponseEntity<PagingResult<ProductDTO>>.Ok(res, "Lấy thành công");
        }
    }
}


// size=10 , page=1 => lưu cache
// size=10 , page=1, search = camera => lưu cache
// size=10 , page=1, search = phone => lưu cache


// 



            // ProductDTO input = new ProductDTO()
            // {
            //     Id = 1,
            //     Name = "Sản phẩm 1",
            //     Description = "Mô tả sản phẩm 1",
            //     Price = 100.000M,
            //     Stock = 50
            // };
            // ProductDTO output = new ProductDTO();

            // _mapper.Map(output,input);