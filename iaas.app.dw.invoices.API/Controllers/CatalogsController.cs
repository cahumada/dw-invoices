using iaas.app.dw.invoices.Application.Base;
using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace iaas.app.dw.invoices.API.Controllers
{
    /// <summary>
    /// Tratamiento de cátalogos en AFIP
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogsController : ControllerBase
    {
        private IInvoicesService _invoicesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoicesService"></param>
        public CatalogsController(IInvoicesService invoicesService)
        {
            _invoicesService = invoicesService;
        }

        /// <summary>
        /// Tipos de catálogos obtenidos en AFIP por cliente
        /// </summary>
        /// <param name="clientId">Código de cliente conectado al servicio</param>
        /// <param name="catalog">Catálogo a obtener</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{clientId}")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponseDto<List<DescriptionDto>>), description: "Successful Operation")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 500, description: "Server Error")]
        public async Task<IActionResult> GetCatalog([FromRoute] string clientId, [FromQuery] TypeCatalogEnum catalog)
        {
            ApiResponseDto<List<DescriptionDto>> response = new();

            try
            {
                response = await _invoicesService.GetCatalog(clientId, catalog);

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
