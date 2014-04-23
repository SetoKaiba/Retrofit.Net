using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;

using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

using RestSharp;

using Retrofit.Net.Attributes.Methods;
using Retrofit.Net.Attributes.Parameters;

namespace Retrofit.Net.Tests
{
    [TestFixture]
    public class AuthorizedResourceEndpointTests
    {
        private IRestClient _restClient;

        private Authenticator _authenticator;

        private RestAdapter _restAdapter;

        public class AuthorizedResource
        {
        }

        public class AuthorizedEndpointTests
        {
            public interface IAuthorizedResourceEndpoint
            {
                [Get("/resource/{id}")]
                Task<IRestResponse<AuthorizedResource>> GetAuthorizedResourceAsync([Path("id")] int id);
            }
        }

        [SetUp]
        public void SetUp()
        {
            _restClient = Substitute.For<IRestClient>();
            _authenticator = new Authenticator
                             {
                                 AuthenticationEndpoint = "/oauth2/token",
                                 AccessToken = "token",
                                 TokenType = "bearer",
                                 RefreshToken = "refresh_token"
                             };
            _restAdapter = new RestAdapter(_restClient, _authenticator);
        }

        [Test]
        public void RestClient_will_have_the_authenticator_set()
        {
            var oauthAuthenticator = (OAuth2AuthorizationRequestHeaderAuthenticator)_restClient.Authenticator;

            Assert.AreEqual(_authenticator.AccessToken, oauthAuthenticator.AccessToken);
        }

        [Test]
        public void RestClientAuthenticator_correctly_signs_the_requests()
        {
            var authenticator = _restClient.Authenticator;
            var restRequest = new RestRequest();

            authenticator.Authenticate(_restClient, restRequest);

            Assert.IsTrue(restRequest.Parameters.Count(p => p.Type == ParameterType.HttpHeader) == 1);
            Assert.AreEqual("bearer token", restRequest.Parameters.First(p => p.Type == ParameterType.HttpHeader).Value);
        }
    }
}