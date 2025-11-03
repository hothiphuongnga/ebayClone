// lưu ý: DI HOÁ
// interface
// class : interface
using System.Threading.Tasks;
using ebay.Base;
using ebay.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ebay.ServicesBlazor;

public interface IProductPageService
{
    event Action? Onchange;
    Task LoadData();
    int PageIndex { get; set; }
    int PageSize { get; set; }
    string? Search { get; set; }
    public PagingResult<ProductDTO> PagingResult { get; set; }
    Task SetPageIndex(int pageIndex);
    Task SetSearch();
    bool IsLoading { get; set; }
    int TotalPages => (int)Math.Ceiling((double)PagingResult.TotalRow / PageSize);
    // lấy phần nguyên +1 nếu có dư
    // 5,1 => 6
    // 5.9 => 6
}

public class ProductPageService : IProductPageService
{
    public event Action? Onchange;
    // ịnect httpclient
    private readonly HttpClient _httpClient;
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
    public PagingResult<ProductDTO> PagingResult { get; set; }
    public bool IsLoading { get; set; }


    public ProductPageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        PageIndex = 1;
        PageSize = 10;
        Search = null;
        PagingResult = new PagingResult<ProductDTO>();
        IsLoading = false;
    }

    public async Task LoadData()
    {
        IsLoading = true;
        NotifyStateChanged();
        string url = $"http://localhost:5242/api/Product/paging?pageIndex={PageIndex}&pageSize={PageSize}";

        // kiểm tra search 
        if (!string.IsNullOrEmpty(Search?.Trim()))
        {
            url += $"&search={Search.Trim()}";
        }

        // gọi api
        var resonse = await _httpClient.GetFromJsonAsync<ResponseEntity<PagingResult<ProductDTO>>>(url);

        // kiểm tra kết quả
        if (resonse != null && resonse.Success && resonse.Content != null)
        {
            PagingResult = resonse.Content;
            IsLoading = false;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("PagingResult" + PagingResult.TotalItems.Count);
            Console.ResetColor();
            NotifyStateChanged();

        }
    }


    // đổi page index
    public async Task SetPageIndex(int pageIndex)
    {
        PageIndex = pageIndex;
        await LoadData();
    }
    // đổi search
    public async Task SetSearch()
    {
        PageIndex = 1; // reset về trang 1
        await LoadData();
    }



    public void NotifyStateChanged() => Onchange?.Invoke();


}