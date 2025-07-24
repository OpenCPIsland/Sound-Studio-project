using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using DI.HTTP.Security.Pinning;

namespace DI.HTTP.Security
{
    public class CertificatePolicyHandler
    {
        private static CertificatePolicyHandler instance = null;

        private IPinset pinset = null;

        private CertificatePolicyHandler()
        {
            SetPinset(DefaultPinsetFactory.getFactory().getPinset());

            // Register the certificate validation callback ONCE
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
        }

        public IPinset GetPinset()
        {
            return pinset;
        }

        public void SetPinset(IPinset pinset)
        {
            this.pinset = pinset;
        }

        public static CertificatePolicyHandler GetPolicyHandler()
        {
            if (instance == null)
            {
                instance = new CertificatePolicyHandler();
            }
            return instance;
        }

        private static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine("Certificate: {0}", certificate?.GetCertHashString());

            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Add pinning logic here if you want
            return false;
        }
    }
}
