using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace iaas.app.dw.invoices.API.Controllers
{
    /// <summary>
    /// Tratamiento de contribuyentes en AFIP
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TaxPayerController : ControllerBase
    {
        private IInvoicesService _invoicesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoicesService"></param>
        public TaxPayerController(IInvoicesService invoicesService)
        {
            _invoicesService = invoicesService;
        }

        /// <summary>
        /// Consulta de inscripción en AFIP
        /// </summary>
        /// <param name="cuit">Código de CUIT a consultar</param>
        /// <param name="clientId">Código de cliente vinculado al servicio AFIP</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{clientId}")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponseDto<PersonsDto>), description: "Successful Operation")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 500, description: "Server Error")]
        public async Task<IActionResult> GetClientInformation([FromQuery] long cuit, [FromRoute] string clientId)
        {
            ApiResponseDto<PersonsDto> response = new();

            try
            {
                response = await _invoicesService.GetClientInformation(clientId, cuit);

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
