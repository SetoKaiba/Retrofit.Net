using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using RestSharp;

namespace Retrofit.Net
{
    public class RestInterceptor : IInterceptor
    {
        private readonly IRestClient _restClient;

        public RestInterceptor(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public void Intercept(IInvocation invocation)
        {
            // Build Request
            var methodInfo = new RestMethodInfo(invocation.Method); // TODO: Memoize these objects in a hash for performance
            var request = new RequestBuilder(methodInfo, invocation.Arguments).Build();

            // Execute request
            var responseType = invocation.Method.ReturnType;
            var genericTypeArgument = responseType.GetGenericArguments()[0];
            // We have to find the method manually due to limitations of GetMethod()
            var methods = _restClient.GetType().GetMethods();
            MethodInfo method = methods.Where(m => m.Name == "Execute").First(m => m.IsGenericMethod);
            MethodInfo generic = method.MakeGenericMethod(genericTypeArgument);
            invocation.ReturnValue =  generic.Invoke(_restClient, new object[] { request });

        }
    }
}