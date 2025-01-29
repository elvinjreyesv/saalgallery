using SaalGalleryApi.Models.Request;
using SaalGalleryApi.Models.Response;
using SaalGalleryApi.Services.Interfaces;
using Supabase.Gotrue;

namespace SaalGalleryApi.Services;

public class AccountService(Supabase.Client supabase,
        ILogger<AccountService> logger,
        IHttpContextAccessor httpContextAccessor) : IAccountService
{
    private readonly string _userId = httpContextAccessor.HttpContext.Items["UserId"] as string;
    private readonly string _email = httpContextAccessor.HttpContext.Items["Email"] as string;

    public async Task<UserRegisterResponseModel> Register(UserRequestModel request)
    {
        try
        {
            var result = await supabase.Auth.SignUp(request.Email, request.Password);

            return new UserRegisterResponseModel
            {
                Token = result?.AccessToken ?? string.Empty,
                Status = true
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(Register));
            return new UserRegisterResponseModel
            {
                Status = false,
                ResponseMessage = ex.Message
            };
        }
    }

    public async Task<UserRegisterResponseModel> Authenticate(UserRequestModel request)
    {
        try
        {
            var result = await supabase.Auth.SignIn(request.Email, request.Password);

            return new UserRegisterResponseModel
            {
                Token = result?.AccessToken ?? string.Empty,
                Status = true
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(Authenticate));
            return new UserRegisterResponseModel
            {
                Status = false,
                ResponseMessage = ex.Message
            };
        }
    }

    public async Task<bool> Logout()
    {
        try
        {
            await supabase.Auth.SignOut();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(Logout));
            return false;
        }
    }

    public async Task<User> RetrieveUser()
    {
        try
        {
            var attrs = new UserAttributes { Email = _email };
            return await supabase.Auth.Update(attrs);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(RetrieveUser));
            return null;
        }
    }
}
