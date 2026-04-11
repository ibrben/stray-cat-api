namespace StrayCat.Application.Interfaces
{
    public interface IUrlService
    {
        string BuildAuthCallbackUrl(string token);
        string BuildErrorUrl(string errorMessage);
        string BuildGoogleAuthUrl();
    }
}
