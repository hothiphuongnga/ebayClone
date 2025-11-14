using ebay.Base;
using Microsoft.AspNetCore.Mvc.Filters;
namespace ebay.Filter;

public class AuthFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new ResponseEntity<string>
            {
                StatusCode = 401,
                Message = "Chưa xác thực. Vui lòng đăng nhập để tiếp tục.",
                Content = "[Filter] Yêu cầu xác thực."
            };
        }

    }
}