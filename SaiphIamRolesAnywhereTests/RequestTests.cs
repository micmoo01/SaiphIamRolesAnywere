using SaiphIamRolesAnywhere;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaiphIamRolesAnywhereTests
{
    public class RequestTests
    {
        [Fact]
        public void BuildRequestString()
        {
            var pfxPath = Path.GetFullPath("Res\\testcert.pfx");
            var (_, certificate) = CertificateProvider.ImportPfx(pfxPath);

            var request = CanonicalRequest.Create(
                certificate,
                TestParams.ProfileArn,
                TestParams.RoleArn,
                TestParams.TrustAnchorArn,
                TestParams.DATETIME);

            Assert.Equal(TestParams.EXPECTED_REQUESTSTRING, request.RequestString);
        }

        [Fact]
        public void BuildStringToSign()
        {
            var pfxPath = Path.GetFullPath("Res\\testcert.pfx");
            var (_, certificate) = CertificateProvider.ImportPfx(pfxPath);

            var request = CanonicalRequest.Create(
                certificate,
                TestParams.ProfileArn,
                TestParams.RoleArn,
                TestParams.TrustAnchorArn,
                TestParams.DATETIME);

            Assert.Equal(TestParams.EXPECTED_HASHEDREQUEST, request.HashedRequestString);
            Assert.Equal(TestParams.EXPECTED_STRINGTOSIGN, request.StringToSign);
        }

        [Fact]
        public void BuildAuth()
        {
            var pfxPath = Path.GetFullPath("Res\\testcert.pfx");
            var (rsaPrivateKey, certificate) = CertificateProvider.ImportPfx(pfxPath);

            var request = CanonicalRequest.Create(
                certificate,
                TestParams.ProfileArn,
                TestParams.RoleArn,
                TestParams.TrustAnchorArn,
                TestParams.DATETIME);

            var signature = Utility.Sign(rsaPrivateKey, request.StringToSign);
            var auth = request.GetAuth(signature);

            Assert.Equal(TestParams.EXPECTED_AUTH, auth);
        }

        [Fact]
        public void HashDuration()
        {
            const string json = "{\"durationSeconds\":3600}";
            const string expected = "1a15f67f6619aa540b13e9a4c37d149fb2c9bdea25d82b73a3878826694dfe27";

            using System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create();
            string result = Utility.HashText(json, sha);
            Assert.Equal(expected, result);
        }
    }
}
