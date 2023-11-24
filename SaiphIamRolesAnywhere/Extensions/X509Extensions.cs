using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SaiphIamRolesAnywhere
{
    public static class X509Extensions
    {
        public static string GetNumericSerial(this X509Certificate certificate)
        {
            return new System.Numerics.BigInteger(certificate.GetSerialNumber()).ToString();
        }
        public static string GetBase64String(this X509Certificate certificate)
        {
            return Convert.ToBase64String(certificate.GetRawCertData());
        }
    }
}
