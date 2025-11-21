public class OrderDto
{
    public int Id { get; set; }

    public int BuyerId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }
}

// Tạo đơn hàng
// buyeId ,
// totalamount, ,
// có nhiều mã SP, giá , số lượng mưa => list<T>
public class CreateOrderDto
{
    public int BuyerId { get; set; }

    public decimal TotalAmount { get; set; }

    public List<ProductOrderDto> OrderDetails { get; set; } = new List<ProductOrderDto>();
}

public class ProductOrderDto
{
    public int ProductId { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
}