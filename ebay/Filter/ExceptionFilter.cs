// EXCEPTION FILTER
// bắt lỗi  

// UseExceptionHandler: middleware

// ExceptionFilter: filter 

using ebay.Base;
using Microsoft.AspNetCore.Mvc.Filters;

public class ExceptionFilter : IExceptionFilter
{
    // khi có lỗi xảy ra trong action method hoặc trong quá trình xử lý request
    public void OnException(ExceptionContext context)
    {
        // log lỗi ra console
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[Exception Filter] Đã xảy ra lỗi: " + context.Exception.Message);
        Console.ResetColor();

        // có thể tùy chỉnh phản hồi trả về client nếu muốn
        context.Result = new ResponseEntity<string>
        {
            StatusCode = 500,
            Message = "Đã xảy ra lỗi trên server (từ Exception Filter).",
            Content = context.Exception.Message
        };
        context.ExceptionHandled = true; // đánh dấu lỗi đã được xử lý
    }
}