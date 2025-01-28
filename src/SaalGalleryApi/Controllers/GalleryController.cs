using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaalGallery.Services.Interfaces;
using SaalGallery.Utilities.Constants;

namespace SaalGallery.Controllers
{
    [Route(ApiConstants.ApiRoute)]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GalleryController(IGalleryService galleryService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet(ApiConstants.Ping)]
        public ActionResult Ping() => Ok("OK");

        [HttpPost("UploadImage")]
        public async Task<ActionResult> UploadImage(byte[] imageContent)
            => Ok(await galleryService.UploadImage(imageContent));

        [HttpDelete("DeleteAllImages")]
        public async Task<ActionResult> DeleteAllImages()
            => Ok(await galleryService.DeleteAllImages());

        [HttpDelete("DeleteImage")]
        public async Task<ActionResult> DeleteImage(string image)
            => Ok(await galleryService.DeleteImage(image));

        [HttpGet("ListAllUserImages")]
        public async Task<ActionResult> ListAllUserImages()
            => Ok(await galleryService.ListAllUserImages());
    }
}
