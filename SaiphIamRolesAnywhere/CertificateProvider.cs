using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SaiphIamRolesAnywhere
{
    public static class CertificateProvider
    {
        //public static (RSA, X509Certificate) ImportWindowsStore(string subject, StoreLocation storeLocation = StoreLocation.LocalMachine)
        //{
        //    foreach (StoreName storeName in (StoreName[])Enum.GetValues(typeof(StoreName)))
        //    {
        //        X509Store store = new X509Store(storeName, storeLocation);
        //        store.Open(OpenFlags.ReadOnly);

        //        Console.WriteLine("Yes    {0,4}  {1}, {2}",
        //            store.Certificates.Count, store.Name, store.Location);

        //        var collection = store.Certificates.Find(X509FindType.FindBySubjectName, subject, true);
        //        if (collection.Count > 0)
        //        {
        //            var cert = collection[0];

        //            //var certPem = cert.GetBase64String();
        //            //var pemReader = new PemReader(new StringReader(certPem));
        //            var key = cert.GetPublicKeyString();
        //            var rsa = cert.GetRSAPublicKey();
        //            Console.WriteLine(rsa.ToString());
        //            //var key = pemReader.read
        //            //var rsaParams = DotNetUtilities.ToRSAParameters(key);

        //            //var rsa = RSA.Create();
        //            //rsa.ImportParameters(rsaParams);

        //            return (null, cert);
        //        }
        //    }

        //    throw new Exception($"Certificate {subject}");
        //}

        public static (RSA, X509Certificate) ImportPfx(string certificatePath)
        {
            X509Certificate2Collection collection = new X509Certificate2Collection();
            collection.Import(certificatePath, null, X509KeyStorageFlags.PersistKeySet);

            var cert = collection[0];
            var rsa = cert.GetRSAPrivateKey();

            return (rsa, cert);
        }

        public static (RSA, X509Certificate) FromPemFiles(string privateKeyPath, string certificatePath)
        {       
            var cert = X509Certificate.CreateFromCertFile(certificatePath);

            var keypem = Encoding.UTF8.GetString(File.ReadAllBytes(privateKeyPath));
            var pemReader = new PemReader(new StringReader(keypem));

            var key = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();
            var rsaParams = DotNetUtilities.ToRSAParameters(key);

            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);

            return (rsa, cert);
        }
    }
}
