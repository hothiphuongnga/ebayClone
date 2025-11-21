using ebay.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ebay.Repositories;

public interface IUnitOfWork : IDisposable
{
    // đưa hết repo vào đây nếu dự án nhỏ


    //dự án lớn thì chia ra nhiều unitofwork
    // VD: tạo đơn hàng -> orderDetail -> product  -> UnitOfWorkOrder
    // quản lí kho -> product -> UnitOfWorkInventory
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    // lưu thay đổi
    Task<int> SaveChangesAsync();
    // rollback nếu có lỗi
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

public class UnitOfWork : IUnitOfWork
{
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }

    private readonly EBayDbContext _context;

    public UnitOfWork(EBayDbContext context,
        IProductRepository productRepository,
        IOrderRepository orderRepository)
    {
        _context = context;
        Products = productRepository;
        Orders = orderRepository;

    }
    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        await _context.Database.RollbackTransactionAsync();

    }

}

