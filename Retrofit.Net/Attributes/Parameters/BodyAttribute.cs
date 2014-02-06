using RestSharp;

namespace Retrofit.Net.Attributes.Parameters
{
    public class BodyAttribute : ParameterAttribute
    {
        public override ParameterType Type
        {
            get
            {
                return ParameterType.RequestBody;
            }
        }
    }
}
