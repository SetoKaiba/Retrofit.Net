using Castle.DynamicProxy;
using RestSharp;

namespace Retrofit.Net
{
    public class RestAdapter
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private IRestClient restClient;

        public RestAdapter(string baseUrl)
        {
            restClient = new RestClient(baseUrl);
        }

        public RestAdapter(string baseUrl, Authenticator authenticator) : this(baseUrl)
        {
            restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(authenticator.AccessToken, authenticator.TokenType);
        }

        public RestAdapter(IRestClient client)
        {
            restClient = client;
        }

        public RestAdapter(IRestClient client, Authenticator authenticator) : this (client)
        {
            restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(authenticator.AccessToken, authenticator.TokenType);
        }

        public string Server { get; set; }

        public T Create<T>() where T : class
        {
            return _generator.CreateInterfaceProxyWithoutTarget<T>(new RestInterceptor(restClient));
        }
    }
}