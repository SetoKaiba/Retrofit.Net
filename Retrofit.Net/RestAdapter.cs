using Castle.DynamicProxy;
using RestSharp;

using Retrofit.Net.Interceptors;

namespace Retrofit.Net
{
    public class RestAdapter
    {
        private readonly Authenticator _authenticator;

        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private IRestClient restClient;

        public RestAdapter(string baseUrl)
        {
            restClient = new RestClient(baseUrl);
        }

        public RestAdapter(string baseUrl, Authenticator authenticator) : this(baseUrl)
        {
            restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(authenticator.AccessToken, authenticator.GrantType);
        }

        public RestAdapter(IRestClient client)
        {
            restClient = client;
        }

        public RestAdapter(IRestClient client, Authenticator authenticator) : this (client)
        {
            _authenticator = authenticator;
            restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(authenticator.AccessToken, authenticator.GrantType);
        }

        public string Server { get; set; }

        public T Create<T>() where T : class
        {
            return _generator.CreateInterfaceProxyWithoutTarget<T>(new RestInterceptor(restClient));
        }

        public T Create<T, TR>()
            where T : class
            where TR : class
        {
            var proxyGenerationOptions = new ProxyGenerationOptions
                                         {
                                             Selector = new AsyncVsNonAsyncInterceptorSelector()
                                         };

            return _generator.CreateInterfaceProxyWithoutTarget<T>(
                new RestInterceptor(restClient), 
                new RefreshTokenInterceptor<TR>(restClient, _authenticator), 
                new RestInterceptor(restClient, true));
        }
    }
}