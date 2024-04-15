
namespace iaas.app.dw.invoices.Application.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool IsSuccess { get; set; } = true;

        public T Data { get; set; }

        public List<ApiErrorMessageDto> Errors { get; set; }

        public ApiResponseDto()
        {
            Errors = new List<ApiErrorMessageDto>();
        }

        public ApiResponseDto(T data)
        {
            Data = data;
            Errors = new List<ApiErrorMessageDto>();
        }
    }
}
