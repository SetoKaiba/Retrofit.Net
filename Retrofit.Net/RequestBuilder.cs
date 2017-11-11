using System.Linq;
using Castle.Core.Internal;
using RestSharp;
using RestSharp.Serializers;

namespace Retrofit.Net
{
    class RequestBuilder
    {
        private readonly RestMethodInfo _methodInfo;
        private readonly object[] _arguments;

        public RequestBuilder(RestMethodInfo methodInfo, object[] arguments)
        {
            _methodInfo = methodInfo;
            _arguments = arguments;
        }

        public IRestRequest Build()
        {
            var request = new RestRequest(_methodInfo.Path, _methodInfo.Method)
            {
                RequestFormat = DataFormat.Json, // TODO: Allow XML requests?
                JsonSerializer = new JsonNetSerializer()
            };

            _methodInfo.Parameters.ForEach(p => p.Value = _arguments[_methodInfo.Parameters.IndexOf(p)]);

            _methodInfo.BodyParameters.ToList().ForEach(bp => request.AddBody(bp.Value));
            _methodInfo.QueryParameters.ToList().ForEach(bp => request.AddParameter(bp.Name, bp.Value.ToString(), ParameterType.GetOrPost));
            _methodInfo.PathParameters.ToList().ForEach(bp => request.AddUrlSegment(bp.Name, bp.Value.ToString()));
            _methodInfo.HeaderParameters.ToList().ForEach(bp => request.AddHeader(bp.Name, bp.Value.ToString()));

            return request;
        }
    }
}
