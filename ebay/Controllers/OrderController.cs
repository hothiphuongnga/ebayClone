namespace ebay.Controllers
{
    using ebay.Base;
    using ebay.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService _service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrderDto dto)
        {
            // goij service để xử lý tạo đơn hàng
            var order =  await _service.CreateOrderAsync(dto);
            if(order) return ResponseEntity<bool>.Ok(order, "Tạo đơn hàng thành công");
            return ResponseEntity<bool>.Fail("Tạo đơn hàng thất bại");
        }
    }
}