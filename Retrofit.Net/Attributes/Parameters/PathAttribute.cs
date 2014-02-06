using RestSharp;

namespace Retrofit.Net.Attributes.Parameters
{
    public class PathAttribute : ParameterAttribute
    {
        public PathAttribute(string value)
        {
            Value = value;
        }

        public override ParameterType Type
        {
            get
            {
                return ParameterType.UrlSegment;
            }
        }
    }
}
