using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace iaas.app.dw.invoices.API.Controllers
{
    /// <summary>
    /// Tratamiento de certificados de acceso en AFIP
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private IInvoicesService _invoicesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoicesService"></param>
        public CertificatesController(IInvoicesService invoicesService)
        {
            _invoicesService = invoicesService;
        }

        /// <summary>
        /// Almacena un certificado PFX de AFIP en la base de datos
        /// </summary>
        /// <param name="cuit">Código de CUIT del cliente vinculado al servicio AFIP</param>
        /// <param name="clientId">Código de cliente vinculado al servicio AFIP</param>
        /// <param name="key">Clave del certificado</param>
        /// <param name="file">Certificado vinculado a AFIP</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{clientId}")]
        public async Task<IActionResult> PostCerificate([FromRoute] string clientId, [FromQuery] long cuit, [FromQuery] string key, [FromForm] IFormFile file)
        {
            ApiResponseDto<bool> response = new();

            try
            {
                response = await _invoicesService.SaveCertificate(clientId, "cahumada@diworksoluciones.com.ar", cuit, key, file);

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
