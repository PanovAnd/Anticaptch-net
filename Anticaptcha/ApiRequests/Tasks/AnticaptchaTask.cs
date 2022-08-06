using Newtonsoft.Json;

namespace Anticaptcha.ApiRequests.Tasks{
    public abstract class AnticaptchaTask{
        [JsonProperty("type")]
        internal abstract string Type{ get; }
    }
}