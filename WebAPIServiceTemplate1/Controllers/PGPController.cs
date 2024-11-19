//using PgpCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPIServiceTemplate1.Business;
using WebAPIServiceTemplate1.Configurations;
using WebAPIServiceTemplate1.Objects.DTOs;
using WebAPIServiceTemplate1.Objects.Requests;
using WebAPIServiceTemplate1.Objects.Responses;

namespace WebAPIServiceTemplate1.Controllers
{
    [GlobalExceptionFilter]
    [RoutePrefix("api/pgpfileAdmin")]
    public class PGPController : ApiController
    {
        private readonly IPGPBL _PGPBL;

        public PGPController()
        {
            this._PGPBL = (IPGPBL)new PGPBL();
        }

        [HttpPost]
        [Route("GenerateKey")]
        public async Task<HttpResponseMessage> CreateKey(
          [FromBody] GenerateKeyRequest request)
        {
            PGPController pgpController = this;
            if (!pgpController.ModelState.IsValid)
                return pgpController.Request.CreateErrorResponseBadRequest(pgpController.ModelState);
            KeyDto keyDto = new KeyDto()
            {
                IdentificationType = request.IdentificationType,
                Identification = request.Identification,
                Email = request.Email, 
                KeyLength = request.KeyLength
            };
            CertificateDto key = pgpController._PGPBL.GenerateKey(keyDto);
            GeneretaKeyResponse generetaKeyResponse = new GeneretaKeyResponse()
            {
                IsGenerated = key.Created,
                PublicKey = key.PublicKey,
                ExpireDate = key.ExpireDate,
                ClientIdentification = request.Identification
            };
            return pgpController.Request.CreateSuccessResponse<GeneretaKeyResponse>(HttpStatusCode.OK, "Key creado con exito", generetaKeyResponse);
        }

        [HttpPost]
        [Route("DecryptFile")]
        public async Task<HttpResponseMessage> DecryptFile(
              [FromBody] DecryptFileRequest request)
        {
            PGPController pgpController = this;
            if (!pgpController.ModelState.IsValid)
                return pgpController.Request.CreateErrorResponseBadRequest(pgpController.ModelState);
            DecryptFileResponse decryptFileResponse = new DecryptFileResponse()
            {
                File = pgpController._PGPBL.DecriptFile(request.Identification, request.IdentificationType, request.File)
            };
            return pgpController.Request.CreateSuccessResponse<DecryptFileResponse>(HttpStatusCode.OK, "Archivo desencriptado", decryptFileResponse);
        }
    }
}
