using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SaiphIamRolesAnywhere
{
    public static class Utility
    {
        private static readonly Regex regionReg = new Regex(@"[a-z]{2}\-([a-z]+\-)+\d{1}");

        /// <summary>
        /// Created because Convert.ToHexString is not available in .netstandard 2.0  
        /// </summary>
        public static string HexEncode(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "");
        }
        /// <summary>
        /// Created because Convert.ToHexString is not available in .netstandard 2.0  
        /// </summary>
        public static string HexEncode(string input)
        {
            var data = Encoding.Default.GetBytes(input);
            return HexEncode(data);
        }

        /// <summary>
        /// Extract region from trust anchor arn
        /// </summary>
        public static string ExtractRegion(string arn)
        {
            if (string.IsNullOrEmpty(arn))
                return "";

            var matches = regionReg.Matches(arn);
            if (matches.Count < 1)
                return "";

            return matches[0].Value;
        }

        /// <summary>
        /// Escape URI characters with uppercase variant. e.g. / => %2f => %2F
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UriEncode(string input)
        {
            var lower = HttpUtility.UrlEncode(input);
            Regex reg = new Regex(@"%[a-f0-9]{2}");
            return reg.Replace(lower, m => m.Value.ToUpperInvariant());
        }

        /// <summary>
        /// encode and hash input string according to AWS documentation
        /// Lowercase(Hex(SHA256(UTF8($input))))
        /// </summary>
        public static string HashText(string input, SHA256 sha)
        {
            return HexEncode(
                        sha.ComputeHash(
                            Encoding.UTF8.GetBytes(input)))
                    .ToLower();
        }

        public static string Sign(RSA rsaPrivateKey, string input)
        {
            using (rsaPrivateKey)
                return HexEncode(
                        rsaPrivateKey.SignData(Encoding.UTF8.GetBytes(input), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                    .ToLower();
        }
    }
}
