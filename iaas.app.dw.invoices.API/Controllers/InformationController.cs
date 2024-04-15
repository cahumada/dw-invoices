using iaas.app.dw.invoices.Application.Base;
using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace iaas.app.dw.invoices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformationController : ControllerBase
    {
        private IInvoicesService _invoicesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoicesService"></param>
        public InformationController(IInvoicesService invoicesService)
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
        [Route("client")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponseDto<PersonsDto>), description: "Successful Operation")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 500, description: "Server Error")]
        public async Task<IActionResult> GetClientInformation([FromQuery] long cuit, [FromQuery] string clientId)
        {
            ApiResponseDto<PersonsDto> response = new();

            try
            {
                response = await _invoicesService.GetClientInformation(clientId, cuit);

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto(ex.Message));

                return BadRequest(response);
            }
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
        [Route("certificate")]
        public async Task<IActionResult> PostCerificate([FromQuery] long cuit, [FromQuery] string clientId, [FromQuery] string key, [FromForm] IFormFile file)
        {
            ApiResponseDto<bool> response = new();

            try
            {
                response = await _invoicesService.SaveCertificate(clientId, "cahumada@diworksoluciones.com.ar", cuit, key, file);

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto(ex.Message));

                return BadRequest(response);
            }
        }


        /// <summary>
        /// Tipos de catálogos obtenidos en AFIP por cliente
        /// </summary>
        /// <param name="clientId">Código de cliente conectado al servicio</param>
        /// <param name="catalog">Catálogo a obtener</param>
        /// <returns></returns>
        [HttpGet]
        [Route("catalog")]
        [SwaggerOperation("catalog")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponseDto<List<DescriptionDto>>), description: "Successful Operation")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 500, description: "Server Error")]
        public async Task<IActionResult> GetCatalog([FromQuery] string clientId, [FromQuery] TypeCatalogEnum catalog)
        {
            ApiResponseDto<List<DescriptionDto>> response = new();

            try
            {
                response = await _invoicesService.GetCatalog(clientId, catalog);

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto(ex.Message));

                return BadRequest(response);
            }
        }

        /// <summary>
        /// Puntos de venta habilitados por AFIP para el cliente conectado al servicio
        /// </summary>
        /// <param name="clientId">Código de cliente conectado al servicio</param>
        /// <returns></returns>
        [HttpGet]
        [Route("pointofsale")]
        [SwaggerOperation("pointofsale")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponseDto<List<PointOfSalesDto>>), description: "Successful Operation")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 500, description: "Server Error")]
        public async Task<IActionResult> GetPointOfSales([FromQuery] string clientId)
        {
            ApiResponseDto<List<PointOfSalesDto>> response = new();

            try
            {
                response = await _invoicesService.GetPointOfSales(clientId);

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
