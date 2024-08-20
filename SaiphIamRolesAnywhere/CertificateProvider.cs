using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using SaiphIamRolesAnywhere.DI;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SaiphIamRolesAnywhere
{
    public static class CertificateProvider
	{
		public static IPasswordFinder NoPassword = new NullPassword();
		public static (RSA, X509Certificate) ImportPfx(string certificatePath) => ImportPfx(certificatePath, NoPassword);
		public static (RSA, X509Certificate) ImportPfx(string certificatePath, IPasswordFinder finder)
		{
			X509Certificate2Collection collection = new X509Certificate2Collection();
			collection.Import(certificatePath, new string(finder.GetPassword()), X509KeyStorageFlags.PersistKeySet);

			var cert = collection[0];
			var rsa = cert.GetRSAPrivateKey();

			return (rsa, cert);
		}

		public static (RSA, X509Certificate) FromPemFiles(string privateKeyPath, string certificatePath, IPasswordFinder finder)
		{
			var cert = X509Certificate.CreateFromCertFile(certificatePath);

			var keypem = Encoding.UTF8.GetString(File.ReadAllBytes(privateKeyPath));
			var pemReader = new PemReader(new StringReader(keypem), finder);

			var key = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();
			var rsaParams = DotNetUtilities.ToRSAParameters(key);

			var rsa = RSA.Create();
			rsa.ImportParameters(rsaParams);

			return (rsa, cert);
		}

      public static (RSA, X509Certificate) FromStore(string thumbprint, IPasswordFinder finder)
		{
         X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
			store.Open(OpenFlags.ReadOnly);
			try
			{
				X509Certificate2Collection cers = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
				if (cers.Count == 0)
				{
					throw new CertificateNotFoundException(thumbprint);
				}
				var cert = cers[0];
				var rsa = cert.GetRSAPrivateKey();
				return (rsa, cert);
			}
			finally
			{
				store.Close();
			}
		}
	}
	public class NullPassword : IPasswordFinder
	{
		public char[] GetPassword() => null;
	}

}
