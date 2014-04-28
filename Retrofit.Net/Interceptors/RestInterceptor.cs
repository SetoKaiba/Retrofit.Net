using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Castle.DynamicProxy;

using RestSharp;

namespace Retrofit.Net.Interceptors
{
    public class RestInterceptor : ISyncInterceptor
    {
        private readonly IRestClient _restClient;

        private readonly MethodInfo _executeMethod;
        private readonly MethodInfo _executeAsyncMethod;

        private readonly bool _lastInterceptorInPipeline;

        public RestInterceptor(IRestClient restClient, bool lastInterceptorInPipeline = false)
        {
            _lastInterceptorInPipeline = lastInterceptorInPipeline;
            _restClient = restClient;
            _executeMethod = _restClient.GetType().GetMethods().First(m => m.Name == "Execute" && m.IsGenericMethod);
            _executeAsyncMethod = _restClient.GetType().GetMethods().First(m => m.Name == "ExecuteTaskAsync" && m.IsGenericMethod && (m.GetParameters().Count() == 1));
        }

        public void Intercept(IInvocation invocation)
        {
            var methodInfo = new RestMethodInfo(invocation.Method); // TODO: Memoize these objects in a hash for performance
            var request = new RequestBuilder(methodInfo, invocation.Arguments).Build();
            
            Type responseType = invocation.Method.ReturnType;
            var genericTypeArgument = GetUnderlyingReturnType(responseType);

            MethodInfo method = responseType.GetGenericTypeDefinition() == typeof(Task<>) 
                ? _executeAsyncMethod
                : _executeMethod;

            MethodInfo generic = method.MakeGenericMethod(genericTypeArgument);
            invocation.ReturnValue = generic.Invoke(_restClient, new object[] { request });

            if (!_lastInterceptorInPipeline)
            {
                invocation.Proceed();
            }
        }

        private static Type GetUnderlyingReturnType(Type responseType)
        {
            var genericTypeArgument = responseType;
            if (genericTypeArgument.GetGenericTypeDefinition() == typeof(Task<>))
            {
                genericTypeArgument = genericTypeArgument.GetGenericArguments()[0];
            }

            if (genericTypeArgument.IsGenericType && genericTypeArgument.GetGenericTypeDefinition() == typeof(IRestResponse<>))
            {
                genericTypeArgument = genericTypeArgument.GetGenericArguments()[0];
            }

            return genericTypeArgument;
        }
    }
}