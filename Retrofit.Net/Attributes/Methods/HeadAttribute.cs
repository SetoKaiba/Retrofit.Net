using RestSharp;

namespace Retrofit.Net.Attributes.Methods
{
    [RestMethod(Method.HEAD)]
    public class HeadAttribute : ValueAttribute
    {
        public HeadAttribute(string path)
        {
            Value = path;
        }
    }
}