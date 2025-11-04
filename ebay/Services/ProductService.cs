using AutoMapper;
using ebay.Base;
using ebay.Dtos;
using ebay.Models;
using ebay.Repositories;

namespace ebay.Serrvices;


public interface IProductService
{
    Task<IEnumerable<ProductDTO>> Get20ProductsAsync();
    Task<PagingResult<ProductDTO>> GetProductsPagingAsync(int pageIndex, int pageSize, string? search);
}
public class ProductService(IProductRepository _repo, IMapper _mapper) : IProductService
{
    public async Task<IEnumerable<ProductDTO>> Get20ProductsAsync()
    {
        IEnumerable<Product> products = await _repo.Get20ProductsAsync();
        var res = _mapper.Map<List<ProductDTO>>(products);
        return res;
    }
    public async Task<PagingResult<ProductDTO>> GetProductsPagingAsync(int pageIndex, int pageSize, string? search)
    {
            // bọc code trong try catch để bắt lỗi
            PagingResult<Product> pagingProducts = await _repo.GetProductsPagingAsync(pageIndex, pageSize, search);
            ///TẠO LỖI 

            int a = 10; int b = 0;
            int c = a / b;

            /// 
            /// 
            // map từng phần
            var res = new PagingResult<ProductDTO>();
            res.PageIndex = pagingProducts.PageIndex;
            res.PageSize = pagingProducts.PageSize;
            res.TotalRow = pagingProducts.TotalRow;
            // chuyển danh sách sản phẩm sang DTO
            res.TotalItems = _mapper.Map<List<ProductDTO>>(pagingProducts.TotalItems);
            return res;
   
       


    }
}