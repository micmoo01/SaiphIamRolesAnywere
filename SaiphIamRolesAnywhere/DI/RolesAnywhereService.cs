using Org.BouncyCastle.OpenSsl;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SaiphIamRolesAnywhere.DI
{

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
        private static Dictionary<string, KeyCache> CertificateCache = new Dictionary<string, KeyCache>();

        public async Task<AwsCredential> GetAwsCredentials()
        {
            RSA rsaPrivateKey = null;
            X509Certificate certificate = null;

            (rsaPrivateKey, certificate) = LoadFromSource(PasswordFinder);

            var request = CanonicalRequest.Create(
                certificate,
                Params.ProfileArn,
                Params.RoleArn,
                Params.TrustAnchorArn);

            var signature = Utility.Sign(rsaPrivateKey, request.StringToSign);

            var authRes = await request.Send(signature);
            return authRes.CredentialSet[0].Credentials;
        }

        public (RSA, X509Certificate) LoadFromSource(IPasswordFinder finder)
        {
            RSA rsaPrivateKey = null;
            X509Certificate certificate = null;
            // check requirements
            if (string.IsNullOrEmpty(Params.CertificatePath) && string.IsNullOrEmpty(Params.Thumbprint))
            {
                throw new PathOrSubjectRequiredException();
            }

            // get the cached cert, if it exists
            KeyCache cache;
            var key = GetLocalCacheKey();
            if (CertificateCache.TryGetValue(key, out cache))
            {
                return (cache.PrivateKey, cache.Certificate);
            }

            // get certificate and key
            if (string.IsNullOrEmpty(Params.CertificatePath))
            {
                (rsaPrivateKey, certificate) = CertificateProvider.FromStore(Params.Thumbprint, PasswordFinder);
            }
            else
            {
                var file = new System.IO.FileInfo(Params.CertificatePath);
                if (file.Extension == PFX_EXTENSION)
                    (rsaPrivateKey, certificate) = CertificateProvider.ImportPfx(file.FullName, PasswordFinder);
                else
                    (rsaPrivateKey, certificate) = CertificateProvider.FromPemFiles(Params.PrivateKeyPath, file.FullName, PasswordFinder);
            }
            // add the cert to cachke
            CertificateCache[key] = new KeyCache(rsaPrivateKey, certificate);
            return (rsaPrivateKey, certificate);
        }

        private string GetLocalCacheKey() => string.IsNullOrEmpty(Params.CertificatePath)
            ? Params.Thumbprint 
            : Params.CertificatePath;
    }



    public class MyPasswordFinder : IPasswordFinder
	 {
        public char[] GetPassword() => null;
	 }

    internal class KeyCache
    {
        internal KeyCache(RSA key, X509Certificate cert)
        {
            PrivateKey = key;
            Certificate = cert;
        }
        internal RSA PrivateKey { get; set; }
        internal X509Certificate Certificate { get; set; }
    }
}
