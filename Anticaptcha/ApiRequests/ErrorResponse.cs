using System;
using Newtonsoft.Json;

namespace Anticaptcha.ApiRequests{
    public abstract class ErrorResponse{
        [JsonProperty("errorId")]
        public int ErrorId;

        [JsonProperty("errorDescription")]
        public string ErrorDescription;

        [JsonProperty("errorCode")]
        public string ErrorCode;

        [JsonIgnore]
        internal bool Failed => ErrorId > 0;

        public AnticaptchaApiException ExtractException(){
            if (!Failed) throw new Exception();

            return new AnticaptchaApiException(this);
        }
    }
}