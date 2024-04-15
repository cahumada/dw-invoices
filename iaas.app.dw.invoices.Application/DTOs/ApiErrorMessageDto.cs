
namespace iaas.app.dw.invoices.Application.DTOs
{
    /// <summary>
    /// Mensaje de error // Error of message
    /// </summary>
    public class ApiErrorMessageDto
    {
        public ApiErrorMessageDto()
        {
            
        }
        public ApiErrorMessageDto(string errorMessage)
        {
            ErrorCode = "9999";
            ErrorMessage = errorMessage;
        }

        public ApiErrorMessageDto(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public ApiErrorMessageDto(string errorCode, string errorMessage, string propertyName, string severity, object attemptedValue)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
            Severity = severity;
            AttemptedValue = attemptedValue;
        }

        /// <summary>
        /// Código de error // Error code
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Mensaje del error // Message error
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The name of the property // Nombre de la propiedad
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Custom severity level associated with the error // Nivel de severidad asociado al error
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// The property value that caused the error // Valor de la propiedad asociada al error 
        /// </summary>
        public object AttemptedValue { get; set; }
    }
}
