using Azure;
using ebay.Base;
using ebay.Data;
using ebay.Models;
using Microsoft.EntityFrameworkCore;

namespace ebay.Repositories;


// Interface 
public interface IProductRepository
{
    Task<IEnumerable<Product>> Get20ProductsAsync();
    //IEnumerable : đại diện cho một tập hợp các đối tượng có thể được lặp qua.
    // .ToList() : chuyển đổi IEnumerable thành List
    // crud
    // -->  repobase<T> =>

    // paging , trang nào , lấy bao nhiêu , tìm search gì
    Task<PagingResult<Product>> GetProductsPagingAsync(int pageIndex, int pageSize, string? search);

}


// class : interface
public class ProductRepository(EBayDbContext _context) : IProductRepository
{
    // private readonly EBayDbContext _context;
    // public ProductRepository(EBayDbContext context)
    // {
    //     _context = context;
    // }
    public async Task<IEnumerable<Product>> Get20ProductsAsync()
    {
        var products = await _context.Products
           .Include(a => a.Ratings)
           .Include(a => a.ProductImages)
           .Take(20)
           .ToListAsync();
        return products;
    }


    public async Task<PagingResult<Product>> GetProductsPagingAsync(int pageIndex, int pageSize, string? search)
    {
        // lấy hết
        var products = await _context.Products
           .Include(a => a.Ratings)
           .Include(a => a.ProductImages)
           .AsNoTracking() // không theo dõi thay đổi để tăng hiệu suất
           .ToListAsync();

        // kiểm tra rỗng của search
        if (!string.IsNullOrEmpty(search))
        {
            products = products.Where(p => p.Name.Contains(search)).ToList();
        }
        // ds tất cả bản ghi phù hợp -> tổng bản ghi => TotalRow
        int totalRow = products.Count;
        // lấy phân trang
        // trang 1  lay 10 sp
        // bo qua 0 10
        // (1 - 1) *10 = 0 dong ; 
        // trang 2 lay 10 sp    
        // bo qua 10, lay 10
        // (2 - 1) *10 = 10 dong ;
        var pagedProducts = products
            .Skip((pageIndex - 1) * pageSize) // bỏ qua các bản ghi của các trang trước
            .Take(pageSize) // lấy số bản ghi của trang hiện tại
            .ToList();

        PagingResult<Product> res = new PagingResult<Product>()
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalRow = totalRow,
            TotalItems = pagedProducts
        };
        return res;
    }
}

