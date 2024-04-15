using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace iaas.app.dw.invoices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private IInvoicesService _invoicesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoicesService"></param>
        public InvoiceController(IInvoicesService invoicesService)
        {
            _invoicesService = invoicesService;
        }

        /// <summary>
        /// Consulta de último numero de recibo por cliente, punto de venta y tipo de comprobante
        /// </summary>
        /// <param name="clientId">Código de cliente vinculado al servicio AFIP</param>
        /// <param name="pointOfSale">Punto de venta a consultar</param>
        /// <param name="typeProof">Tipo de comprobante a consultar</param>
        /// <returns></returns>
        [HttpGet]
        [Route("last")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponseDto<PersonsDto>), description: "Successful Operation")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 500, description: "Server Error")]
        public async Task<IActionResult> GetLastInvoice([FromQuery] string clientId, [FromQuery] int pointOfSale, [FromQuery] int typeProof)
        {
            ApiResponseDto<int> response = new();

            try
            {
                response = await _invoicesService.GetLastInvoice(clientId, pointOfSale, typeProof);

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto(ex.Message));

                return BadRequest(response);
            }
        }

    }
}
