using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RestSharp;
using Retrofit.Net.Attributes;
using Retrofit.Net.Attributes.Methods;

namespace Retrofit.Net
{
    class RestMethodInfo
    {
        private readonly MethodInfo _methodInfo;
        protected object RequestMethod { get; set; }
        public Method Method { get; set; }
        public string Path { get; set; }

        public List<Parameter> Parameters { get; set; }

        public IEnumerable<Parameter> BodyParameters
        {
            get
            {
                return Parameters.Where(p => p.Type == ParameterType.RequestBody);
            }
        }

        public IEnumerable<Parameter> HeaderParameters
        {
            get
            {
                return Parameters.Where(p => p.Type == ParameterType.HttpHeader);
            }
        }

        public IEnumerable<Parameter> QueryParameters
        {
            get
            {
                return Parameters.Where(p => p.Type == ParameterType.QueryString);
            }
        }

        public IEnumerable<Parameter> PathParameters
        {
            get
            {
                return Parameters.Where(p => p.Type == ParameterType.UrlSegment);
            }
        }

        public RestMethodInfo(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            Init(); 
        }

        private void Init()
        {
            ParseMethodAttributes();
            ParseParameters();
        }

        private void ParseMethodAttributes()
        {
            foreach (ValueAttribute attribute in _methodInfo.GetCustomAttributes(true))
            {
                var innerAttributes = attribute.GetType().GetCustomAttributes(false);

                // Find the request method attribute, if present.
                var methodAttribute = innerAttributes.FirstOrDefault(theAttribute => theAttribute.GetType() == typeof(RestMethodAttribute)) as RestMethodAttribute;

                if (methodAttribute != null)
                {
                    if (RequestMethod != null)
                    {
                        throw new ArgumentException("Method " + _methodInfo.Name + " contains multiple HTTP methods. Found " + RequestMethod  + " and " + methodAttribute.Method);
                    }

                    Method = methodAttribute.Method;
                    Path = attribute.Value;
                }
            }
        }

        private void ParseParameters()
        {
            Parameters = new List<Parameter>();
            
            foreach (ParameterInfo parameter in _methodInfo.GetParameters())
            {
                var parameterAttribute = (ParameterAttribute) parameter.GetCustomAttributes(false).FirstOrDefault();
                if (parameterAttribute == null)
                {
                    throw new ArgumentException("No annotation found on parameter " + parameter.Name + " of " + _methodInfo.Name);
                }
                Parameters.Add(new Parameter { Name = parameterAttribute.Value ?? parameter.Name, Type = parameterAttribute.Type });
            }
        }

    }
}
