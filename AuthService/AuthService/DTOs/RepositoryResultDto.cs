namespace AuthService.DTOs
{
    public class RepositoryResultDto<T>
    {
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}