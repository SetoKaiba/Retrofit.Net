using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

        private IAuthorizedResourceEndpoint _authorizedResourceEndpoint;

        public class AuthorizedResource : IResource
        {
        }

        public interface IAuthorizedResourceEndpoint
        {
            [Get("/resource/{id}")]
            Task<IRestResponse<AuthorizedResource>> GetAuthorizedResourceAsync([Path("id")] int id);

            [Get("/resource/{id}")]
            IRestResponse<AuthorizedResource> GetById([Path("id")] int id);
        }

        [SetUp]
        public void SetUp()
        {
            _restClient = Substitute.For<IRestClient>();
            _authenticator = new Authenticator
                             {
                                 AuthenticationEndpoint = "/oauth2/token",
                                 AccessToken = "token",
                                 GrantType = "bearer",
                                 RefreshToken = "refresh_token"
                             };
            _restAdapter = new RestAdapter(_restClient, _authenticator);
            _authorizedResourceEndpoint = _restAdapter.Create<IAuthorizedResourceEndpoint, AuthorizedResource>();
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

        [Test]
        public void ExpiredToken_refreshes_the_token_using_the_refresh_token()
        {
            var unauthorizedResponse = new RestResponse<AuthorizedResource> { StatusCode = HttpStatusCode.Unauthorized };
            var authorizedResponse = new RestResponse<AuthorizedResource> { StatusCode = HttpStatusCode.OK };
            _restClient
                .Execute<AuthorizedResource>(
                    Arg.Is<RestRequest>(rr =>
                        rr.Resource == "/resource/{id}" &&
                        rr.Parameters.Any(p => p.Name == "id" && (string)p.Value == "1")))
                .Returns(unauthorizedResponse, authorizedResponse);

            var restResponse = _authorizedResourceEndpoint.GetById(1);

            _restClient
                .Received()
                .Execute<Authenticator>(Arg.Is<RestRequest>(rr => 
                    rr.Method == Method.POST 
                    && rr.Parameters.Any(p => p.Name == "refresh_token")
                    && rr.Parameters.Any(p => (p.Name == "token_type") && ((string)p.Value == "refresh_token"))));
        }

        [Test]
        public void ExpiredToken_with_async_call_will_still_call_refresh_token()
        {
            var unauthorizedResponseTask = Task.Factory.StartNew(() => (IRestResponse<AuthorizedResource>) new RestResponse<AuthorizedResource> { StatusCode = HttpStatusCode.Unauthorized });
            var authorizedResponseTask = Task.Factory.StartNew(() => (IRestResponse<AuthorizedResource>) new RestResponse<AuthorizedResource> { StatusCode = HttpStatusCode.OK });

            _restClient.ExecuteTaskAsync<AuthorizedResource>(
                Arg.Is<RestRequest>(
                    rr =>
                        rr.Resource == "/resource/{id}"
                        && rr.Parameters.Any(p => p.Name == "id" && (string)p.Value == "1")))
                .Returns(unauthorizedResponseTask, authorizedResponseTask);

            var restResponse = _authorizedResourceEndpoint.GetAuthorizedResourceAsync(1);

            _restClient.Received().Execute<Authenticator>(Arg.Is<RestRequest>(rr =>
                rr.Method == Method.POST
                && rr.Parameters.Any(p => p.Name == "refresh_token")
                && rr.Parameters.Any(p => (p.Name == "token_type") && ((string)p.Value == "refresh_token"))));
        }
    }
}