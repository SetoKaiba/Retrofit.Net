using Castle.Core.Internal;
using RestSharp;

namespace Retrofit.Net
{
    class RequestBuilder
    {
        private readonly RestMethodInfo methodInfo;
        private readonly object[] arguments;

        public RequestBuilder(RestMethodInfo methodInfo, object[] arguments)
        {
            methodInfo = methodInfo;
            arguments = arguments;
        }

        public IRestRequest Build()
        {
            var request =  new RestRequest(methodInfo.Path, methodInfo.Method)
                               {
                                   RequestFormat = DataFormat.Json // TODO: Allow XML requests?
                               };

            methodInfo.Parameters.ForEach(p => p.Value = arguments[methodInfo.Parameters.IndexOf(p)]);

            methodInfo.BodyParameters.ForEach(bp => request.AddBody(bp.Value));
            methodInfo.QueryParameters.ForEach(bp => request.AddParameter(bp.Name, bp.Value.ToString()));
            methodInfo.PathParameters.ForEach(bp => request.AddUrlSegment(bp.Name, bp.Value.ToString()));
            methodInfo.HeaderParameters.ForEach(bp => request.AddHeader(bp.Name, bp.Value.ToString()));

            

            return request;
        }
    }
}
