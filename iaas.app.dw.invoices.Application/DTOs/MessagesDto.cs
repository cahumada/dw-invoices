namespace iaas.app.dw.invoices.Application.DTOs
{
    /// <summary>
    /// Respuesta de WS
    /// </summary>
    public class MessagesDto
    {
        /// <summary>
        /// Código de AFIP del error
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Mensage asociado al código de error
        /// </summary>
        public string Message { get; set; }
    }
}
