using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.Security.Cryptography.Pkcs;

namespace iaas.app.dw.invoices.Application.Base
{
    internal class Certificatex509Lib
    {

        /// <summary>
        /// Firma mensaje
        /// </summary>
        /// <param name="argBytesMsg">Bytes del mensaje</param>
        /// <param name="argCertifSigner">Certificado usado para firmar</param>
        /// <returns>Bytes del mensaje firmado</returns>
        /// <remarks></remarks>
        public static byte[] FirmaBytesMensaje(byte[] argBytesMsg, X509Certificate2 argCertifSigner)
        {
            try
            {
                // PONGO EL MENSAJE EN UN OBJETO CONTENTINFO (REQUERIDO PARA CONSTRUIR EL OBJ SIGNEDCMS)
                ContentInfo infoContent = new ContentInfo(argBytesMsg);
                SignedCms cmsSigned = new SignedCms(infoContent);

                // CREO OBJETO CMSSIGNER QUE TIENE LAS CARACTERISTICAS DEL FIRMANTE
                CmsSigner cmsSigner = new CmsSigner(argCertifSigner);
                cmsSigner.IncludeOption = X509IncludeOption.EndCertOnly;


                // FIRMO EL MENSAJE PKCS #7
                cmsSigned.ComputeSignature(cmsSigner);


                // ENCODEO EL MENSAJE PKCS #7.
                return cmsSigned.Encode();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al firmar: " + ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Lee el certificado
        /// </summary>
        /// <param name="certificateSign"></param>
        /// <param name="secureString"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCertifByFile(byte[] certificateSign, SecureString secureString)
        {
            
            try
            {
                X509Certificate2 objCert = new(certificateSign, secureString);
                //objCert.Import(certificateSign, secureString, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);

                return objCert;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el archivo: " + ex.Message, ex.InnerException);
            }
        }
    }
}
