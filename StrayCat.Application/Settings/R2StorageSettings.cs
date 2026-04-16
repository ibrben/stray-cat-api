using Amazon.S3;

namespace StrayCat.Application.Settings
{
    public class R2StorageSettings
    {
        public string AccessKeyId { get; set; } = string.Empty;
        public string SecretAccessKey { get; set; } = string.Empty;
        public string ServiceUrl { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public HttpVerb Verb { get; set; } = HttpVerb.PUT;
        public Protocol Protocol { get; set; } = Protocol.HTTPS;
    }
}
