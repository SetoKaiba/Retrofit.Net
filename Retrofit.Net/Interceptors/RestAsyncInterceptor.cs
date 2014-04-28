using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Castle.DynamicProxy;

using RestSharp;

namespace Retrofit.Net.Interceptors
{
    public class RestAsyncInterceptor : IAsyncInterceptor
    {
        private readonly IRestClient _restClient;

        private readonly MethodInfo _executeMethod;
        private readonly MethodInfo _executeAsyncMethod;

        private readonly bool _lastInterceptorInPipeline;

        public RestAsyncInterceptor(IRestClient restClient, bool lastInterceptorInPipeline = false)
        {
            _lastInterceptorInPipeline = lastInterceptorInPipeline;
            _restClient = restClient;
        }


        public void Intercept(IInvocation invocation)
        {
            throw new NotImplementedException();
        }

        private static Type GetUnderlyingReturnType(Type responseType)
        {
            var genericTypeArgument = responseType;
            if (genericTypeArgument.GetGenericTypeDefinition() == typeof(Task<>))
            {
                genericTypeArgument = genericTypeArgument.GetGenericArguments()[0];
            }

            if (genericTypeArgument.IsGenericTypeDefinition && genericTypeArgument.GetGenericTypeDefinition() == typeof(IRestResponse<>))
            {
                genericTypeArgument = genericTypeArgument.GetGenericArguments()[0];
            }

            return genericTypeArgument;
        }
    }
}