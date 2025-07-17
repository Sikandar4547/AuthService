namespace AuthService.DTOs
{
    public class ApiResponseDto<T>
    {
        public ApiStatusCode StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public T? Data { get; set; }
    }
}