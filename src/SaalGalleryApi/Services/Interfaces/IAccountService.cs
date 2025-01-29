using SaalGalleryApi.Models.Request;
using SaalGalleryApi.Models.Response;
using Supabase.Gotrue;

namespace SaalGalleryApi.Services.Interfaces;

public interface IAccountService
{
    Task<UserRegisterResponseModel> Register(UserRequestModel request);
    Task<UserRegisterResponseModel> Authenticate(UserRequestModel request);
    Task<bool> Logout();
    Task<User> RetrieveUser();
}
