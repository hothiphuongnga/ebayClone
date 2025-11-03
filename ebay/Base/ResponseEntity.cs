using Microsoft.AspNetCore.Mvc;

namespace ebay.Base
{
    public class ResponseEntity<T> : IActionResult 
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Content { get; set; }
        public int StatusCode { get; set; }

        public static ResponseEntity<T> Ok(T data, string? message = null)
        {
            return new ResponseEntity<T>
            {
                Success = true,
                Message = message ?? "Success",
                Content = data,
                StatusCode = 200
            };
        }

        public static ResponseEntity<T> Fail(string message, int statusCode = 400)
        {
            return new ResponseEntity<T>
            {
                Success = false,
                Message = message,
                Content = default,
                StatusCode = statusCode
            };
        }
        public ResponseEntity<T> NotFound(string message = "Not Found", int statusCode = 404)
        {
            return new ResponseEntity<T>
            {
                Success = false,
                Message = message,
                Content = default,
                StatusCode = statusCode
            };
        }
        // Authen, author : 401 403
        // server err : 500
        // created : 201
        // no content: 204
        //
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(this)
            {
                StatusCode = this.StatusCode
            };
            await objectResult.ExecuteResultAsync(context);
        }
    }
}
