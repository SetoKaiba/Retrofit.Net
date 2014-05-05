namespace Retrofit.Net
{
    public class Authenticator
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string GrantType { get; set; }

        public int ExpiresIn { get; set; }

        public string AuthenticationEndpoint { get; set; }

        public string AccessTokenParameterName { get; private set; }

        public string RefreshTokenParameterName { get; private set; }

        public string GrantTypeParameterName { get; private set; }

        public string ClientIdParameterName { get; private set; }

        public string ClientId { get; set; }

        public string ExpiresInName { get; private set; }

        public Authenticator()
        {
            AccessTokenParameterName = "access_token";
            RefreshTokenParameterName = "refresh_token";
            GrantTypeParameterName = "grant_type";
            ExpiresInName = "expires_in";
            ClientIdParameterName = "client_id";
        }
    }

    public class RestAdapterBuilder
    {
        private string _endpoint;

        private string _accessTokenName;

        private string _refreshTokenName;

        private string _expiresInName;

        public static RestAdapterBuilder AuthenticatedAt(string endpoint)
        {
            var authenticatorBuilder = new RestAdapterBuilder();
            authenticatorBuilder._endpoint = endpoint;

            return authenticatorBuilder;
        }

        public RestAdapterBuilder AccessTokenName(string accessTokenName)
        {
            _accessTokenName = accessTokenName;
            
            return this;
        }

        public RestAdapterBuilder RefreshTokenName(string refreshTokenName)
        {
            _refreshTokenName = refreshTokenName;
            
            return this;
        }

        public RestAdapterBuilder ExpiresInName(string expiresInName)
        {
            _expiresInName = expiresInName;

            return this;
        }

        //public static implicit operator RestAdapter(RestAdapterBuilder builer)
        //{
        //    new RestAdapter();
        //}
    }
}