﻿using System.Net;
using System.Threading.Tasks;

using Castle.DynamicProxy;

using RestSharp;

namespace Retrofit.Net.Interceptors
{
    public class RefreshTokenInterceptor<TR> : IInterceptor
        where TR : class
    {
        private readonly IRestClient _restClient;

        private readonly Authenticator _authenticator;

        public RefreshTokenInterceptor(IRestClient restClient, Authenticator authenticator)
        {
            _restClient = restClient;
            _authenticator = authenticator;
        }

        public async Task InterceptAsync(Task<IRestResponse<TR>> previousInvocation)
        {
            await previousInvocation;

            await AuthorizeRequestAsync();
        }

        public void Intercept(IInvocation invocation)
        {
            HttpStatusCode firstCallStatusCode;

            if (invocation.ReturnValue.GetType().GetGenericTypeDefinition() == typeof(Task<>))
            {
                ((Task)invocation.ReturnValue).Wait();
                var restResponse = ((Task<IRestResponse<TR>>)invocation.ReturnValue).Result;
                firstCallStatusCode = restResponse.StatusCode;
            }
            else
            {
                firstCallStatusCode = ((IRestResponse)invocation.ReturnValue).StatusCode;
            }
            
            if (firstCallStatusCode == HttpStatusCode.Unauthorized)
            {
                AuthorizeRequest();
                invocation.Proceed();
            }
        }

        private void AuthorizeRequest()
        {
            var refreshTokenRequest = new RestRequest(_authenticator.AuthenticationEndpoint, Method.POST);
            refreshTokenRequest.AddParameter(_authenticator.RefreshTokenParameterName, _authenticator.RefreshToken);
            refreshTokenRequest.AddParameter(_authenticator.GrantTypeParameterName, "refresh_token");
            refreshTokenRequest.AddParameter(_authenticator.ClientIdParameterName, _authenticator.ClientId);
            
            _restClient.Execute<Authenticator>(refreshTokenRequest);
        }

        private Task AuthorizeRequestAsync()
        {
            var refreshTokenRequest = new RestRequest(_authenticator.AuthenticationEndpoint, Method.POST);
            refreshTokenRequest.AddParameter(_authenticator.RefreshTokenParameterName, _authenticator.RefreshToken);
            refreshTokenRequest.AddParameter(_authenticator.GrantTypeParameterName, "refresh_token");

            return _restClient.ExecuteTaskAsync<Authenticator>(refreshTokenRequest);
        }
    }
}