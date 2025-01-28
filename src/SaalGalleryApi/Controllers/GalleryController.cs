using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaalGallery.Utilities.Constants;

namespace SaalGallery.Controllers
{
    [Route(ApiConstants.ApiRoute)]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GalleryController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet(ApiConstants.Ping)]
        public ActionResult Ping() => Ok("OK");
    }
}
