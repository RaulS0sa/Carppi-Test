using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Carppi.Clases
{
    public static class ServicePointConfiguration
    {
        private const string SupportedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEApz6lyA7hEmKqc2dwyLFwruhPgV/bIN+QJ3pUC8iGq3VnT47skYUu7AqGleEhOVOCZhF7ISkGlODntcHMcCqUnVbWhbekluP240GF0w4DZC49hcZZbHbAPAv/IcDqnwBvaTaiZ/l+fHIkie72AWqftAn8Ip5OO7uSY1OY86mMOBCfLENXQgbUP4YS581Z8zSEtsjniPO++/OwwbyAK7j3yfLw0rlUKEYioEDEk6fMf+ufL6Tio8lFNzl78ceeZoz+x9Re6wWkqCVMjx9bKecrMkEAESvi+SYMtphfxXsrt/VOj2wJI7VhULWQbqudV2RD95x8MkXpGT9USoM/dllJqwIDAQAB";

        public static void SetUp()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertficate;
        }

        private static bool ValidateServerCertficate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors
        )
        {
            return SupportedPublicKey == certificate?.GetPublicKeyString();
        }
    }
}
