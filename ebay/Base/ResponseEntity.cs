namespace ebay.Base
{
    public class ResponseEntity<T>
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
    }
}
