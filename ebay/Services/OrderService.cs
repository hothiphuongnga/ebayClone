using AutoMapper;
using ebay.Models;
using ebay.Repositories;

namespace ebay.Services;
public interface IOrderService
{
    Task<bool> CreateOrderAsync(CreateOrderDto createOrderDto);
}

public class OrderService(
    IUnitOfWork _unitOfWork,
    IMapper _map
    ) : IOrderService
{


    public async Task<bool> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        // bắt đầu transaction
        await _unitOfWork.BeginTransactionAsync();
        try
        {
             // thêm order 
        var order = new Order
        {
            BuyerId = createOrderDto.BuyerId,
            TotalAmount = createOrderDto.TotalAmount,
            Status = "Pending",
            CreatedAt = DateTime.Now,
            Deleted = false,
            OrderDetails = createOrderDto.OrderDetails.Select(od => new OrderDetail
            {
                CreatedAt = DateTime.Now,
                ProductId = od.ProductId,
                UnitPrice = od.UnitPrice,
                Quantity = od.Quantity,
                Deleted = false,
            }).ToList()
        };
        await _unitOfWork.Orders.AddOrderAsync(order);
        // luc nau chua co save change 
        // TH thêm order và order detail thành công 

        // update stock sản phẩm
        // chạy loop trong order detail để update stock SP
        // SP bị hết hàng , không đủ hàng => rollback 
        //
        foreach(ProductOrderDto item in createOrderDto.OrderDetails)
        {
            // mỗi item có productid, quantity, unitprice
            // mình cần update stock với productid và quantity
            var success = await _unitOfWork.Products.UpdateStock(item.ProductId, item.Quantity);
                // nếu thành công thì tiếp tục
                // không thì rollback
                if (!success)
                {
                    await _unitOfWork.RollbackAsync();
                    return false;
                }
        }
        await _unitOfWork.SaveChangesAsync(); // lưu tất cả thay đổi , giống commit code chứ chưa push
        await _unitOfWork.CommitAsync(); // xác nhận tất cả thay đổi và đưa vào DB
        return true;
        }
        catch(Exception ex)
        {
            // log lỗi
            await _unitOfWork.RollbackAsync();
            throw ex;
        }
    }
}