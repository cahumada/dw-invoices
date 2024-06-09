using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace iaas.app.dw.invoices.API.Controllers
{
    /// <summary>
    /// Tratamiento de puntos de venta en AFIP por cliente 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SalesPointsController : ControllerBase
    {
        private IInvoicesService _invoicesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoicesService"></param>
        public SalesPointsController(IInvoicesService invoicesService)
        {
            _invoicesService = invoicesService;
        }

        /// <summary>
        /// Puntos de venta habilitados por AFIP para el cliente conectado al servicio
        /// </summary>
        /// <param name="clientId">Código de cliente conectado al servicio</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{clientId}")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponseDto<List<PointOfSalesDto>>), description: "Successful Operation")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 500, description: "Server Error")]
        public async Task<IActionResult> GetPointOfSales([FromRoute] string clientId)
        {
            ApiResponseDto<List<PointOfSalesDto>> response = new();

            try
            {
                response = await _invoicesService.GetPointOfSales(clientId);

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
