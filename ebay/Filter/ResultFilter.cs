using ebay.Base;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ebay.Filter;

public class ResultFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context) // trước khi  kết quả được trả về client
    {
        // can thiệp đổi kết quả nếu muốn
       context.Result = new ResponseEntity<string>
        {
            StatusCode = 200,
            Message = "[Result Filter] Kết quả đã được thay đổi trước khi trả về client.",
            Content = "Hồ Thị Phương Nga"
        };
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("[Result Filter] Đã thay đổi kết quả trước khi trả về client.");
        Console.ResetColor();
    }
    // trả kết kết quả về client
     public void OnResultExecuted(ResultExecutedContext context) // sau khi kết quả đã được trả về client
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[Result Filter] Kết quả đã được trả về client.");
        Console.ResetColor();
    }
}