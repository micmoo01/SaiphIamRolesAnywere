using Org.BouncyCastle.OpenSsl;
using SaiphIamRolesAnywhere;
using SaiphIamRolesAnywhere.DI;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SaiphIamRolesAnywhereTests
{
    public class StoreTests
    {
        [Theory]
        //4a874a36947b30b0263552c270ce24da9f53ffc3
        [InlineData("4a874a36947b30b0263552c270ce24da9f53ffc3",
            "arn:aws:rolesanywhere:us-east-1:211125586441:profile/b6fd7904-d70d-40bf-9eab-83868967365c", "arn:aws:iam::211125586441:role/drk-tst-on-prem-aws-access", "arn:aws:rolesanywhere:us-east-1:211125586441:trust-anchor/e7a6496d-c37a-44ac-83b7-188b49d14ffa", "drk-tst")]
        public void TestSignature(string thumbprint, string profileArn, string roleArn, string trustAnchorArn, string keyPass)
        {
            var svp = new RolesAnywhereServiceParams()
            {
                ProfileArn = profileArn, // "arn:aws:rolesanywhere:us-east-2:649547622198:profile/3232b00d-ab4e-420c-a175-f2af1d51cbbd",
                RoleArn = roleArn, // "arn:aws:iam::649547622198:role/rolesanywhere-skunkx",
                TrustAnchorArn = trustAnchorArn, // "arn:aws:rolesanywhere:us-east-2:649547622198:trust-anchor/8ae47236-3eee-4c45-ab5a-66de414acf7a",
                Thumbprint = thumbprint
            };
            var svc = new RolesAnywhereService()
            {
                Params = svp,
                PasswordFinder = new LocalPasswordFinder(keyPass)
            };

            RSA rsaPrivateKey = null;
            X509Certificate certificate = null;

            (rsaPrivateKey, certificate) = svc.LoadFromSource(svc.PasswordFinder);

            var request = CanonicalRequest.Create(
                certificate,
                svp.ProfileArn,
                svp.RoleArn,
                svp.TrustAnchorArn);

            var signature = Utility.Sign(rsaPrivateKey, request.StringToSign);

        }
    }

    internal class LocalPasswordFinder : IPasswordFinder
    {
        private string _pass;
        public LocalPasswordFinder(string pass)
        {
            _pass = pass;
        }
        public char[] GetPassword() => _pass.ToCharArray();
    }
}
