using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaalGallery.Utilities.Constants;
using SaalGalleryApi.Models.Request;
using SaalGalleryApi.Services.Interfaces;

namespace SaalGallery.Controllers
{
    [Route(ApiConstants.ApiRoute)]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController(
        Supabase.Client supabase,
        IHttpContextAccessor httpContextAccessor,
        IAccountService accountService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRequestModel request)
            => Ok(await accountService.Register(request));

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate(UserRequestModel request)
            => Ok(await accountService.Authenticate(request));

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
            => Ok(await accountService.Logout());

        [HttpGet("RetrieveUser")]
        public async Task<ActionResult> RetrieveUser()
            => Ok(await accountService.RetrieveUser());
    }
}
