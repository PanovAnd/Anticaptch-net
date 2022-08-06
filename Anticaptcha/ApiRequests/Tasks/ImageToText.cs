using Newtonsoft.Json;

namespace Anticaptcha.ApiRequests.Tasks{
    public enum NumericOptions{
        NoRequirements = 0,
        OnlyNumbers = 1,
        MixValue = 2
    }

    public class ImageToTextTask : AnticaptchaTask{
        [JsonProperty("body")]
        internal readonly string ImageBase64;

        [JsonProperty("numeric")]
        internal readonly NumericOptions Numeric = NumericOptions.NoRequirements;

        [JsonProperty("phrase")]
        internal readonly bool Phrase = false;

        [JsonProperty("case")]
        internal readonly bool CaseSensitive = false;

        [JsonProperty("math")]
        internal readonly bool MathOperation = false;

        [JsonProperty("minLength")]
        internal readonly int MinLength;

        [JsonProperty("maxLength")]
        internal readonly int MaxLength;

        public ImageToTextTask(string imgBase64){
            ImageBase64 = imgBase64;
        }

        public ImageToTextTask(string imageBase64, int captchaLength):this(imageBase64,captchaLength,captchaLength){ }
        public ImageToTextTask(string imgBase64, int minLength, int maxLength) : this(imgBase64){
            MinLength = minLength;
            MaxLength = maxLength;
        }

        internal override string Type => "ImageToTextTask";
    }
    
    
    public class ImageToTextResult:ITaskResult{
        [JsonProperty("text")]
        public string Text{ get; internal set; }

        [JsonProperty("url")]
        public string Url{ get; internal set; }
    }
}