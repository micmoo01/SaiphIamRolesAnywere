using Org.BouncyCastle.Asn1.Ocsp;
using SaiphIamRolesAnywhere;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaiphIamRolesAnywhereTests
{
    public class SignTests
    {
        [Fact]
        public void ConsistentSign()
        {
            const string expected = "a497f86331e6218c4f1e025d52edd6dadc2c66f522749e25ca6bbf9ec75ead2e37bd7f7dee03911e8d9e3e7db9511191a2579f385f0929a637350c3ece43d02ad51417998e8b8fffd71a00fcca6c3deeab2a3d912f20279e31cf8d27a78d5df2f6874e9c853f26fe97cb8a71e8011b21bb373ad3ac1023f0787015cfc6a50d1d7c65ee5b32d37a421212278ef6e57cbcd2f2ebb233662ad91c7f887a7bfc2ad73d63c016b0474716559b19d0c6efd930e220d41c7dcdd289808d66a5cbc6c48c3e49144420b5d38824a9b5b5f1ba9c3bc83196e9a09a692d19b16547a342828fba9ea206e9e09b225a48935c4196a52e06db941d0a7cd96a98370973f18d5867";
            const string input = "some randome string thats consistent\nto test signing output\n is also consistent";

            var pfxPath = Path.GetFullPath("Res\\testcert.pfx");
            var (rsaPrivateKey, _) = CertificateProvider.ImportPfx(pfxPath);

            var signature = Utility.Sign(rsaPrivateKey, input);

            Assert.Equal(expected, signature);
        }

        [Fact]
        public void SignRequest()
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
            Assert.Equal(TestParams.EXPECTED_SIGNATURE, signature);
        }
    }
}
