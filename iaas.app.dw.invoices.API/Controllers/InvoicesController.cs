using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace iaas.app.dw.invoices.API.Controllers
{
    /// <summary>
    /// Tratamiento de comprobantes en AFIP por cliente
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private IInvoicesService _invoicesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoicesService"></param>
        public InvoicesController(IInvoicesService invoicesService)
        {
            _invoicesService = invoicesService;
        }

        /// <summary>
        /// Generación de comprobante en AFIP
        /// </summary>
        /// <param name="clientId">Código de cliente vinculado al servicio AFIP</param>
        /// <param name="invoice">Comprobante a emitir en AFIP</param>
        /// <returns></returns>
        [HttpPost("{clientId}")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponseDto<InvoicesDto>), description: "Successful Operation")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 500, description: "Server Error")]
        public async Task<IActionResult> PostInvoice([FromRoute] string clientId, [FromBody] InvoicesDto invoice)
        {
            ApiResponseDto<InvoicesDto> response = new();

            try
            {
                response = await _invoicesService.GenerateInvoice(clientId, invoice);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto()
                {
                    Severity = "Critical",
                    ErrorCode = "9999",
                    ErrorMessage = ex.Message
                });

                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Consulta de último numero de recibo por cliente, punto de venta y tipo de comprobante
        /// </summary>
        /// <param name="clientId">Código de cliente vinculado al servicio AFIP</param>
        /// <param name="pointOfSale">Punto de venta a consultar</param>
        /// <param name="typeProof">Tipo de comprobante a consultar</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{clientId}/last")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponseDto<PersonsDto>), description: "Successful Operation")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 500, description: "Server Error")]
        public async Task<IActionResult> GetLastInvoice([FromRoute] string clientId, [FromQuery] int pointOfSale, [FromQuery] int typeProof)
        {
            ApiResponseDto<int> response = new();

            try
            {
                response = await _invoicesService.GetLastInvoice(clientId, pointOfSale, typeProof);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto()
                {
                    Severity = "Critical",
                    ErrorCode = "9999",
                    ErrorMessage = ex.Message
                });

                return StatusCode(500, response);
            }
        }

    }
}
