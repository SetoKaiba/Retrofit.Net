using RestSharp;

namespace Retrofit.Net.Attributes.Parameters
{
    public class HeaderAttribute : ParameterAttribute
    {
         public HeaderAttribute(string value)
         {
             Value = value;
         }

        public override ParameterType Type
        {
            get
            {
                return ParameterType.HttpHeader;
            }
        }
    }
}