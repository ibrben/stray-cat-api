namespace StrayCat.Application.Settings
{
    public class PaymentSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public AllowedMethod AllowedMethods { get; set; } = new();
    }

    public class AllowedMethod
    {
        public bool QrPromptPay { get; set; } = true;
        public bool Card { get; set; } = false;
        public bool EWallets { get; set; } = false;
        public bool MobileBanking { get; set; } = true;
    }
}
