using Org.BouncyCastle.OpenSsl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SaiphIamRolesAnywhere.DI
{
    public class RolesAnywhereServiceParams
    {
        public string ProfileArn { get; set; }
        public string RoleArn { get; set; }
        public string TrustAnchorArn { get; set; }

        public string CertificatePath { get; set; }
        public string PrivateKeyPath { get; set; }
    }

    public interface IRolesAnywhereService
    {
        Task<AwsCredential> GetAwsCredentials();
    }

    public class RolesAnywhereService : IRolesAnywhereService
    {
        const string PFX_EXTENSION = ".pfx";

        public RolesAnywhereServiceParams Params { get; set; }
        /// <summary>
        /// Password finder should be implemented by client to retrieve private keys
        /// </summary>
        public IPasswordFinder PasswordFinder { get; set; } = new MyPasswordFinder();
        public async Task<AwsCredential> GetAwsCredentials()
        {
            RSA rsaPrivateKey;
            X509Certificate certificate;

            // get certificate and key
            var file = new System.IO.FileInfo(Params.CertificatePath);
            if (file.Extension == PFX_EXTENSION)
                (rsaPrivateKey, certificate) = CertificateProvider.ImportPfx(file.FullName, PasswordFinder);
            else
                (rsaPrivateKey, certificate) = CertificateProvider.FromPemFiles(Params.PrivateKeyPath, file.FullName, PasswordFinder);

            var request = CanonicalRequest.Create(
                certificate,
                Params.ProfileArn,
                Params.RoleArn,
                Params.TrustAnchorArn);

            var signature = Utility.Sign(rsaPrivateKey, request.StringToSign);

            var authRes = await request.Send(signature);
            return authRes.CredentialSet[0].Credentials;
        }
    }

	public class MyPasswordFinder : IPasswordFinder
	{
		public char[] GetPassword() => null;
	}

}
