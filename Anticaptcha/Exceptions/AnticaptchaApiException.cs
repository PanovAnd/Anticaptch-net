using Anticaptcha.ApiRequests;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Anticaptcha.Exceptions {
    [Serializable]
    public class AnticaptchaApiException : Exception {
        internal AnticaptchaApiException() { }
        internal AnticaptchaApiException(string message) : base(message) { }
        internal AnticaptchaApiException(string message, Exception inner) : base(message, inner) { }

        internal AnticaptchaApiException(ErrorResponse response) : base(
            $"Api error code:{response.ErrorCode}\r\nError message:{response.ErrorDescription}") { }

        protected AnticaptchaApiException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}
