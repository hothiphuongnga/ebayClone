
// // ACTION FILTER
// using System.Diagnostics;
// using Microsoft.AspNetCore.Mvc.Filters;
// namespace ebay.Filter;

// // LOG RA THỜI GIAN GỌI ACTION VÀ THỜI GIAN KẾT THÚC

// // kế thừa từ ActionFilterAttribute :  khong cần DI trong program 
// public class LogActionFilter : ActionFilterAttribute
// {
//     private Stopwatch _timer;
//     // Stopwatch: 
//     // 
//     // Bắt đầu 
//     public override void OnActionExecuting(ActionExecutingContext context)
//     {
//         _timer = Stopwatch.StartNew(); // khởi động đồng hồ
//         Console.ForegroundColor = ConsoleColor.Green;
//         Console.WriteLine("[Action Filter] Bắt đầu gọi action: " + context.ActionDescriptor.DisplayName);
//         Console.ResetColor();
//     }


//     // khi kết thúc
//     public override void OnActionExecuted(ActionExecutedContext context)
//     {
//         // dừng đồng hồ
//         _timer.Stop();
//         Console.ForegroundColor = ConsoleColor.DarkYellow;
//         Console.WriteLine("[Action Filter] Thời gian thực thi: " + _timer.ElapsedMilliseconds + " ms");
//         Console.ResetColor();
//     }


// }






// ACTION FILTER
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
namespace ebay.Filter;

// LOG RA THỜI GIAN GỌI ACTION VÀ THỜI GIAN KẾT THÚC

// kế thừa từ IActionFilter :  khong cần DI trong program
public class LogActionFilter : IActionFilter
{
    private Stopwatch _timer;
    // Stopwatch: 
    // 
    // Bắt đầu 
    public void OnActionExecuting(ActionExecutingContext context)
    {
        _timer = Stopwatch.StartNew(); // khởi động đồng hồ
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[Action Filter] Bắt đầu gọi action: " + context.ActionDescriptor.DisplayName);
        Console.ResetColor();
    }


    // khi kết thúc
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // dừng đồng hồ
        _timer.Stop();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("[Action Filter] Thời gian thực thi: " + _timer.ElapsedMilliseconds + " ms");
        Console.ResetColor();
    }


}