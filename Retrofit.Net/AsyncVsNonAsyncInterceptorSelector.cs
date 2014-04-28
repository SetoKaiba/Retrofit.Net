using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Castle.DynamicProxy;

using Retrofit.Net.Interceptors;

namespace Retrofit.Net
{
    public class AsyncVsNonAsyncInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var useAsyncInterceptors = method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

            return useAsyncInterceptors 
                ? interceptors.OfType<IAsyncInterceptor>().Cast<IInterceptor>().ToArray()
                : interceptors.OfType<ISyncInterceptor>().Cast<IInterceptor>().ToArray();
        }
    }
}