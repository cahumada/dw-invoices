using AutoMapper;
using iaas.app.dw.invoices.Application.Base;
using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Application.Services.Interfaces;
using iaas.app.dw.invoices.Domain.Entities;
using iaas.app.dw.invoices.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Xml;

namespace iaas.app.dw.invoices.Application.Services
{
    public class InvoicesService : IInvoicesService
    {
        private readonly string _loginTicketRequestTemplate;
        private static uint _globalUniqueID = 0;

        private ICertificatesRepository _certificatesRepository;
        private ITokensRepository _tokensRepository;


        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly MemoryCacher _memoryCacher;

        public InvoicesService(IConfiguration configuration,
                               ICertificatesRepository certificatesRepository,
                               ITokensRepository tokensRepository,
                               IMapper mapper,
                               ILogger logger,
                               MemoryCacher memoryCacher)
        {
            _loginTicketRequestTemplate = configuration.GetSection("Invoices:LoginTicketRequestTemplate").Value ?? "";

            _certificatesRepository = certificatesRepository;
            _tokensRepository = tokensRepository;

            _mapper = mapper;
            _logger = logger;
            _memoryCacher = memoryCacher;
        }

        #region "Login WS"
        private async Task<Tokens> LoginAfip(string clientId, TypeWebServicesEnum typeWebService = TypeWebServicesEnum.wsfe)
        {
            XmlDocument loginTicketRequest;
            Encoding EncodedMsg = Encoding.UTF8;

            // SE OBTIENE EL CERTIFICADO ASOCIADO AL CLIENTE
            var certificate = await _certificatesRepository.Get(clientId);

            if (certificate == null)
                throw new Exception($"No se encontró el certificado asociado al cliente: {clientId}");

            // SE ALMACENA LA CLAVE DEL CERTIFICADO
            var secureString = new SecureString();
            var certificateKey = AesEncryption.DecryptFromString(certificate.CertificateKey);
            foreach (char c in certificateKey)
            {
                secureString.AppendChar(c);
            }

            _globalUniqueID++;

            loginTicketRequest = new XmlDocument();
            loginTicketRequest.LoadXml(_loginTicketRequestTemplate);

            var uniqueId = loginTicketRequest.SelectSingleNode("//uniqueId");
            uniqueId.InnerText = Convert.ToString(_globalUniqueID);

            var generationTime = loginTicketRequest.SelectSingleNode("//generationTime");
            generationTime.InnerText = DateTime.Now.AddMinutes((certificate.ExpirationMinutes ?? 10) * -1).ToString("s");

            var expirationTime = loginTicketRequest.SelectSingleNode("//expirationTime");
            expirationTime.InnerText = DateTime.Now.AddMinutes((certificate.ExpirationMinutes ?? 10)).ToString("s");

            var service = loginTicketRequest.SelectSingleNode("//service");
            service.InnerText = typeWebService.ToString();

            // SE EL CERTIFICADO FIRMADO
            byte[] bytesCertificate = Convert.FromBase64String(certificate.FilePfx);

            //DWCustomLogs.NewLog("GetLoginAfip", certificate.Pfx, "CERTIFICADO1");

            //DWCustomLogs.NewLog("GetLoginAfip", BitConverter.ToString(bytesCertificate), "CERTIFICADO2");

            var certificatSigned = Certificatex509Lib.GetCertifByFile(bytesCertificate, secureString);

            //DWCustomLogs.NewLog("GetLoginAfip", certificatSigned, "CERTIFICADO3");

            // SE FIRMA MENSAJE CON EL CERTIFICADO
            var msgBytes = EncodedMsg.GetBytes(loginTicketRequest.OuterXml);
            var encodedSignedCms = Certificatex509Lib.FirmaBytesMensaje(msgBytes, certificatSigned);
            var cmsSignedBase64 = Convert.ToBase64String(encodedSignedCms);

            // SE INVOCA AL WS DE AFIP PARA OBTENER EL TOKEN DE ACCESO
            using (var ws = new WSLogin.LoginCMSClient())
            {
                var loginTicketResponse = await ws.loginCmsAsync(cmsSignedBase64);

                // SE ANALIZA EL TICKET RECIBIDO
                var loginTicket = new XmlDocument();
                loginTicket.LoadXml(loginTicketResponse.loginCmsReturn);

                var _uniqueId = UInt32.Parse(loginTicket.SelectSingleNode("//uniqueId").InnerText);
                var _generationTime = DateTime.Parse(loginTicket.SelectSingleNode("//generationTime").InnerText);
                var _expirationTime = DateTime.Parse(loginTicket.SelectSingleNode("//expirationTime").InnerText);
                var _sign = loginTicket.SelectSingleNode("//sign").InnerText;
                var _token = loginTicket.SelectSingleNode("//token").InnerText;

                var token = new Tokens()
                {
                    ClientId = clientId,
                    Expiration = _expirationTime,
                    Sign = _sign,
                    Token = _token,
                    Cuit = certificate.Cuit,
                    TypeWebService = (int)typeWebService
                };

                return token;
            }
        }

        private async Task<TokensDto> GetToken(string clientId, TypeWebServicesEnum typeWebService = TypeWebServicesEnum.wsfe)
        {
            TokensDto tokenDto = new();
            // SE OBTIENE EL ÚLTIMO TOKEN ALMACENADO PARA EL CLIENTE EN TRATAMIENTO
            Tokens token = await _tokensRepository.Get(clientId, (short)typeWebService);

            // SI EL TOKEN NO EXISTE O EXPIRÓ
            if (token == null || token.Expiration <= DateTime.Now)
            {
                // SE INVOCA SERVICIO WSLOGIN PARA OBTENER NUEVO TOKEN DESDE AFIP :: TODO
                var newToken = await LoginAfip(clientId, typeWebService);

                if (token == null)
                {
                    token = newToken;
                    token.Id = 0;
                }
                else
                {
                    // SE ACTUALIZA TOKEN EXISTENTE
                    token.Expiration = newToken.Expiration;
                    token.Sign = newToken.Sign;
                    token.Token = newToken.Token;
                }

                // SE CREA O ACTUALIZA EL REGISTRO CON NUEVO TOKEN
                token = await _tokensRepository.CreUpdToken(token, "cahumada@diworksoluciones.com.ar");

            }

            // MAPPER
            tokenDto = _mapper.Map<TokensDto>(token);

            return tokenDto;
        }
        #endregion

        #region Certificate
        /// <summary>
        /// Almacena un certificado PFX de AFIP en la base de datos
        /// </summary>
        /// <param name="cuit">Código de CUIT del cliente vinculado al servicio AFIP</param>
        /// <param name="clientId">Código de cliente vinculado al servicio AFIP</param>
        /// <param name="key">Clave del certificado</param>
        /// <param name="file">Certificado vinculado a AFIP</param>
        /// <returns></returns>
        public async Task<ApiResponseDto<bool>> SaveCertificate(string clientId, string userId, long cuit, string key, IFormFile file)
        {
            ApiResponseDto<bool> response = new();
            byte[] fileBytes;

            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new Exception("Archivo no enviado o vacío");
                }

                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    throw new Exception("Error al cargar el archivo adjunto");
                }

                var filepfx = Convert.ToBase64String(fileBytes);

                var certificate = new Certificates()
                {
                    ClientId = clientId,
                    Cuit = cuit,
                    ExpirationMinutes = 10,
                    CertificateKey = AesEncryption.EncryptToString(key),
                    FilePfx = filepfx
                };


                var result = await _certificatesRepository.InsCertificate(certificate, userId);

                response.Data = result > 0;

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto(ex.Message));

                _logger.Error(ex.Message, ex);
                return response;
            }


        }
        #endregion

        #region "Request Client Information"
        /// <summary>
        /// Consulta de inscripción en AFIP
        /// </summary>
        /// <param name="cuit">Código de CUIT a consultar</param>
        /// <param name="clientId">Código de cliente vinculado al servicio AFIP</param>
        /// <returns></returns>
        public async Task<ApiResponseDto<PersonsDto>> GetClientInformation(string clientId, long cuit)
        {
            ApiResponseDto<PersonsDto> response = new();

            try
            {
                using (var ws = new WSPersonQuery.PersonaServiceA5Client())
                {
                    var token = await GetToken(clientId, TypeWebServicesEnum.ws_sr_constancia_inscripcion);

                    if (token == null)
                        throw new Exception("No se pudo obtener el token de acceso");

                    var wsResult = await ws.getPersonaAsync(token.Token, token.Sign, token.Cuit, cuit);

                    if (wsResult.personaReturn.datosGenerales == null)
                        throw new Exception("No se pudo obtener la información, por favor verifique los datos ingresados");

                    int typePersonId = wsResult.personaReturn.datosGenerales.tipoPersona == "FISICA" ? 1 : 2;
                    string typePerson = wsResult.personaReturn.datosGenerales.tipoPersona;

                    int typeDocumentId = wsResult.personaReturn.datosGenerales.tipoClave == "CUIT" ? 1 : 2;
                    string typeDocument = wsResult.personaReturn.datosGenerales.tipoClave;

                    int typeIvaConditionId = (wsResult.personaReturn.datosGenerales.tipoPersona == "FISICA" && wsResult.personaReturn.datosMonotributo != null) ? 2 : 3;
                    string typeIvaCondition = (wsResult.personaReturn.datosGenerales.tipoPersona == "FISICA" && wsResult.personaReturn.datosMonotributo != null) ? "MONOTRIBUTO" : "RESP. INSCRIPTO";

                    var result = new PersonsDto()
                    {
                        Id = wsResult.personaReturn.datosGenerales.idPersona,
                        FirstName = wsResult.personaReturn.datosGenerales.nombre,
                        LastName = wsResult.personaReturn.datosGenerales.apellido,
                        TypeDocument = new DescriptionDto(typeDocumentId, typeDocument),
                        IsActive = wsResult.personaReturn.datosGenerales.estadoClave == "ACTIVO",
                        SocialReason = wsResult.personaReturn.datosGenerales.razonSocial,
                        TypePerson = new DescriptionDto(typePersonId, typePerson),
                        TypeIvaCondition = new DescriptionDto(typeIvaConditionId, typeIvaCondition),
                        LegalAddress = wsResult.personaReturn.datosGenerales.domicilioFiscal.direccion + " " + wsResult.personaReturn.datosGenerales.domicilioFiscal.descripcionProvincia + " (" + wsResult.personaReturn.datosGenerales.domicilioFiscal.codPostal + ")"
                    };

                    _logger.Debug("Proceso consulta");

                    response.Data = result;
                    return response;
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto(ex.Message));

                _logger.Error(ex.Message, ex);
                return response;
            }
        }
        #endregion


        #region "Request Catalogs"
        /// <summary>
        /// Tipos de catálogos obtenidos en AFIP por cliente
        /// </summary>
        /// <param name="clientId">Código de cliente conectado al servicio</param>
        /// <param name="catalog">Catálogo a obtener</param>
        /// <returns></returns>
        public async Task<ApiResponseDto<List<DescriptionDto>>> GetCatalog(string clientId, TypeCatalogEnum catalog)
        {
            ApiResponseDto<List<DescriptionDto>> response = new();

            try
            {
                var result = await _memoryCacher.GetValue<List<DescriptionDto>>($"AFIP-GetType{catalog}_{clientId}");

                if (result == null)
                {
                    using (var ws = new WSFE.ServiceSoapClient(WSFE.ServiceSoapClient.EndpointConfiguration.ServiceSoap))
                    {
                        var token = await GetToken(clientId, TypeWebServicesEnum.wsfe);
                        var authRequest = new WSFE.FEAuthRequest();

                        if (token == null)
                            throw new Exception("No se pudo obtener el token de acceso");

                        authRequest.Token = token.Token;
                        authRequest.Sign = token.Sign;
                        authRequest.Cuit = token.Cuit;

                        switch (catalog)
                        {
                            case TypeCatalogEnum.concepts:
                                {
                                    var wsResult = await ws.FEParamGetTiposConceptoAsync(authRequest);
                                    result = wsResult.Body.FEParamGetTiposConceptoResult.ResultGet.Select(m => new DescriptionDto(m.Id, m.Desc)).ToList();
                                    break;
                                }
                            case TypeCatalogEnum.proofs:
                                {
                                    var wsResult = await ws.FEParamGetTiposCbteAsync(authRequest);
                                    result = wsResult.Body.FEParamGetTiposCbteResult.ResultGet.Select(m => new DescriptionDto(m.Id, m.Desc)).ToList();
                                    break;
                                }
                            case TypeCatalogEnum.vats:
                                {
                                    var wsResult = await ws.FEParamGetTiposIvaAsync(authRequest);
                                    result = wsResult.Body.FEParamGetTiposIvaResult.ResultGet.Select(m => new DescriptionDto(Convert.ToInt32(m.Id), m.Desc)).ToList();
                                    break;
                                }
                            case TypeCatalogEnum.documents:
                                {
                                    var wsResult = await ws.FEParamGetTiposDocAsync(authRequest);
                                    result = wsResult.Body.FEParamGetTiposDocResult.ResultGet.Select(m => new DescriptionDto(Convert.ToInt32(m.Id), m.Desc)).ToList();
                                    break;
                                }
                            case TypeCatalogEnum.currencies:
                                {
                                    var wsResult = await ws.FEParamGetTiposMonedasAsync(authRequest);
                                    result = wsResult.Body.FEParamGetTiposMonedasResult.ResultGet.Select(m => new DescriptionDto(Convert.ToInt32(m.Id), m.Desc)).ToList();
                                    break;
                                }
                            case TypeCatalogEnum.othetTaxes:
                                {
                                    var wsResult = await ws.FEParamGetTiposTributosAsync(authRequest);
                                    result = wsResult.Body.FEParamGetTiposTributosResult.ResultGet.Select(m => new DescriptionDto(Convert.ToInt32(m.Id), m.Desc)).ToList();
                                    break;
                                }
                            default:
                                break;
                        }


                        await _memoryCacher.Add($"AFIP-GetType{catalog}_{clientId}", result, DateTime.Now.AddDays(30));

                    }
                }

                //_logger.Debug("Proceso consulta");

                response.Data = result;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto(ex.Message));

                _logger.Error(ex.Message, ex);
                return response;
            }
        }

        /// <summary>
        /// Puntos de venta habilitados por AFIP para el cliente conectado al servicio
        /// </summary>
        /// <param name="clientId">Código de cliente conectado al servicio</param>
        /// <returns></returns>
        public async Task<ApiResponseDto<List<PointOfSalesDto>>> GetPointOfSales(string clientId)
        {
            ApiResponseDto<List<PointOfSalesDto>> response = new();

            try
            {
                var result = await _memoryCacher.GetValue<List<PointOfSalesDto>>($"AFIP-GetPointOfSales_{clientId}");

                if (result == null)
                {
                    using (var ws = new WSFE.ServiceSoapClient(WSFE.ServiceSoapClient.EndpointConfiguration.ServiceSoap))
                    {
                        var token = await GetToken(clientId, TypeWebServicesEnum.wsfe);
                        var authRequest = new WSFE.FEAuthRequest();

                        if (token == null)
                            throw new Exception("No se pudo obtener el token de acceso");

                        authRequest.Token = token.Token;
                        authRequest.Sign = token.Sign;
                        authRequest.Cuit = token.Cuit;

                        var wsResult = await ws.FEParamGetPtosVentaAsync(authRequest);
                        result = wsResult.Body.FEParamGetPtosVentaResult.ResultGet.Select(m => new PointOfSalesDto()
                        {
                            Number = m.Nro,
                            EmissionType = m.EmisionTipo,
                            IsBlocked = m.Bloqueado == "Y",
                            DeleteDate = m.FchBaja
                        }).ToList();


                        await _memoryCacher.Add($"AFIP-GetPointOfSales_{clientId}", result, DateTime.Now.AddDays(30));

                    }
                }
                
                response.Data = result;
                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto(ex.Message));

                _logger.Error(ex.Message, ex);
                return response;
            }
        }

        #endregion

        /// <summary>
        /// Consulta de último numero de recibo por cliente, punto de venta y tipo de comprobante
        /// </summary>
        /// <param name="clientId">Código de cliente vinculado al servicio AFIP</param>
        /// <param name="pointOfSale">Punto de venta a consultar</param>
        /// <param name="typeProof">Tipo de comprobante a consultar</param>
        /// <returns></returns>
        public async Task<ApiResponseDto<int>> GetLastInvoice(string clientId, int pointOfSale, int typeProof)
        {
            ApiResponseDto<int> response = new();

            try
            {
                using (var ws = new WSFE.ServiceSoapClient(WSFE.ServiceSoapClient.EndpointConfiguration.ServiceSoap))
                {
                    var token = await GetToken(clientId, TypeWebServicesEnum.wsfe);
                    var authRequest = new WSFE.FEAuthRequest();

                    if (token == null)
                        throw new Exception("No se pudo obtener el token de acceso");

                    authRequest.Token = token.Token;
                    authRequest.Sign = token.Sign;
                    authRequest.Cuit = token.Cuit;

                    var wsResult = await ws.FECompUltimoAutorizadoAsync(authRequest, pointOfSale, typeProof);

                    var result = wsResult.Body.FECompUltimoAutorizadoResult.CbteNro;

                    response.Data = result;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors.Add(new ApiErrorMessageDto(ex.Message));

                _logger.Error(ex.Message, ex);
                return response;
            }
        }

    }
}
