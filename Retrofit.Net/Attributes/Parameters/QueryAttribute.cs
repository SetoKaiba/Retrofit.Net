using RestSharp;

namespace Retrofit.Net.Attributes.Parameters
{
    public class QueryAttribute : ParameterAttribute
    {
        public QueryAttribute(string value)
        {
            Value = value;
        }

        public override ParameterType Type
        {
            get
            {
                return ParameterType.GetOrPost;
            }
        }
    }
}
