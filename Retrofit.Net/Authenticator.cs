namespace Retrofit.Net
{
    public class Authenticator
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string TokenType { get; set; }

        public string AuthenticationEndpoint { get; set; }
    }
}