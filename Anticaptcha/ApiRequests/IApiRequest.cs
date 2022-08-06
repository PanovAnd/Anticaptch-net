using System.Net.Http;
using Newtonsoft.Json;

namespace Anticaptcha.ApiRequests{
    
    public interface IApiRequest{

        HttpRequestMessage CreateHttpRequest();
    }
}