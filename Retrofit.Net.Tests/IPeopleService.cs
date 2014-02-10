using System.Collections.Generic;
using RestSharp;
using Retrofit.Net.Attributes.Methods;
using Retrofit.Net.Attributes.Parameters;

namespace Retrofit.Net.Tests
{
    public interface IPeopleService
    {
        [Get("people")]
        IRestResponse<List<TestRestCallsIntegration.Person>> GetPeople();

        [Get("people/{id}")]
        IRestResponse<TestRestCallsIntegration.Person> GetPerson([Path("id")] int id);

        [Get("people/{id}")]
        IRestResponse<TestRestCallsIntegration.Person> GetPerson([Path("id")] int id, [Query("limit")] int limit, [Query("test")] string test);

        [Post("people")]
        IRestResponse<TestRestCallsIntegration.Person> AddPerson([Body] TestRestCallsIntegration.Person person);
    }
}