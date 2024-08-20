namespace SaiphIamRolesAnywhere.DI
{
    public class RolesAnywhereServiceParams
    {
        public string ProfileArn { get; set; }
        public string RoleArn { get; set; }
        public string TrustAnchorArn { get; set; }

		  /// <summary>
		  /// If a certificate path is provided, it will be loaded
        /// from file as first option.
		  /// </summary>
        public string CertificatePath { get; set; }
        /// <summary>
        /// If a certificate path is not provided, the certificate is
        /// retrieved from store by thumbprint as second option.
        /// </summary>
        public string Thumbprint { get; set; }

        public string PrivateKeyPath { get; set; }
    }
}
