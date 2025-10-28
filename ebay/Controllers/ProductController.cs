namespace ebay.Controllers
{
    using AutoMapper;
    using ebay.Data;
    using ebay.Dtos;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(EBayDbContext _context, IMapper _mapper) : ControllerBase
    {
        // private readonly EBayDbContext _context;
        // public ProductController(EBayDbContext context)
        // {
        //     _context = context;
        // }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get 20 products from database
            var products = await _context.Products
            .Include(a => a.Ratings)
            .Take(20)
            .ToListAsync();

            // chuyển về DTO để không lỗi vòng lặp vô hạn khi trả về JSON
            // dùng automapper vveer chuyển về dto 
            var res = _mapper.Map<List<ProductDTO>>(products);
            return Ok(res); // => 200 là oke
        }
    }
}




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