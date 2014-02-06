using RestSharp;

namespace Retrofit.Net.Attributes
{
    public abstract class ParameterAttribute : ValueAttribute
    {
        public abstract ParameterType Type { get; }
    }
}