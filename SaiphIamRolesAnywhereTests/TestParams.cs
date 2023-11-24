using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaiphIamRolesAnywhereTests
{
    public static class TestParams
    {
        public static DateTime DATETIME = new DateTime(2023, 11, 19, 15, 0, 0, DateTimeKind.Utc);

        public static string ProfileArn = "arn:aws:rolesanywhere:us-east-1:12345678:profile/e6a5cf90-8983-4f32-bacf-75614d7e3343";
        public static string RoleArn = "arn:aws:iam::12345678:role/somerole";
        public static string TrustAnchorArn = "arn:aws:rolesanywhere:us-east-1:12345678:trust-anchor/e6a5cf90-fc93-478a-bacf-75614d7e3343";


        public static string EXPECTED_REQUESTSTRING = @"POST
/sessions
profileArn=arn%3Aaws%3Arolesanywhere%3Aus-east-1%3A12345678%3Aprofile%2Fe6a5cf90-8983-4f32-bacf-75614d7e3343&roleArn=arn%3Aaws%3Aiam%3A%3A12345678%3Arole%2Fsomerole&trustAnchorArn=arn%3Aaws%3Arolesanywhere%3Aus-east-1%3A12345678%3Atrust-anchor%2Fe6a5cf90-fc93-478a-bacf-75614d7e3343
content-type:application/json
host:rolesanywhere.us-east-1.amazonaws.com
x-amz-date:20231119T150000Z
x-amz-x509:MIIDEzCCAfugAwIBAgIUQXYkjgaJIHLxSYrZQ3Tw6gRXfkMwDQYJKoZIhvcNAQELBQAwGTEXMBUGA1UEAwwOQ29tbW9uVGVzdE5hbWUwHhcNMjMxMTIzMjA0NzI3WhcNMzMxMTIwMjA0NzI3WjAZMRcwFQYDVQQDDA5Db21tb25UZXN0TmFtZTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAOH5+DcCcqhGmiFNtzvKHwUtx3D5NHWYQmGfS6EvunnEnM2cKmLvrwT/nVg8JFb3Ptxdzi/zRe+dP/mrKBlkjVuZ04WaAFrND1+XRNVM99hsQD16AaTqLbPuKAg39FbVRXeRUUvTVO7yxi5I8fr30+d6dc4oxH78Esz6c372vxiZq2SX//AQ7Ohzq4zxuV4RUq4FQrQ0NkDG4A07XXXZlJBa2Mfk6smqfHwNALtev1N3dQeT6w1n13LxRPXhhxqfVVmIFASimn2CqtLwzrUhQFTiwvL+KMjN15tk+NfX87xTeGl1VSj2JGIMxeWT/GRkFr1a8exVZAfv3oLLe3h6FkECAwEAAaNTMFEwHQYDVR0OBBYEFN9pl1e/WnNdgQx+/H5WsiAs9DBWMB8GA1UdIwQYMBaAFN9pl1e/WnNdgQx+/H5WsiAs9DBWMA8GA1UdEwEB/wQFMAMBAf8wDQYJKoZIhvcNAQELBQADggEBAAVBMJJBxvt9SisZIwIntQECuQbk7JYjUW2xijSNWXoQv2Rr8aKdcGlOjPOUZfvL3Ck8Yseh3I+FxI231Th2ftWo/hZkNBSdmYI7/Thl60FvbEx4huEMVbpqH7NUHYxsrb5U0j/qLq+7HWmKQqjI7OJqNa8Au3ILFmPBXbTNCk7RbdAPizbmWybhTeUO0B2vU9RD/uaBlyKeKB2qbhSf5lbZpkN3CIM/5x/21kE5eTwrQY34BCQTZs6+fFezbXsxSEWLQATjuY7cwCcfwrJtqDfDgOOqkaf2UunaWvqt3xYAL/dAXp8M0Jx0e+kwsOFDhsgPcT9zF501ckhc2SJmto0=

content-type;host;x-amz-date;x-amz-x509
1a15f67f6619aa540b13e9a4c37d149fb2c9bdea25d82b73a3878826694dfe27".Replace("\r\n", "\n");
        public static string EXPECTED_HASHEDREQUEST = "03a16b347ff6082fcf78172e28156e22825713a4508b585111ce8891a63990ca";

        public static string EXPECTED_STRINGTOSIGN = @"AWS4-X509-RSA-SHA256
20231119T150000Z
20231119/us-east-1/rolesanywhere/aws4_request
03a16b347ff6082fcf78172e28156e22825713a4508b585111ce8891a63990ca".Replace("\r\n", "\n");
        public static string EXPECTED_SIGNATURE = "468a912018ab256fc64392c0a30ab4f7e281dd00b94548e0a46ab7b13d08d5273811a8960a3e7e6036823dfe54b23d9c0fc8ab317db2200e1c70f83db552c141c695a63643b14a3603c64584c7e3064380bd7932fb3b2bf3a16b1c0c6fc8549722730e5f1f28f473bf698474d23da2b2d604535d20eaec157f99aa6771e6e54d85bad12b6388ae2de8de9963b455ad8fcea841a0056acae6de974ce7487f450a4263205454f6b901085438dc6f0c54eb7283c30ad93fec3c53dc7ab77667b985f182b5acedf6756891e277a740cc47bcf0da695d7829cd83a7504e0605415d0f77cac557fee034ef9b18076537865fe40aab27ae5f07b6139225c52a72435c6f";
        public static string EXPECTED_AUTH = "AWS4-X509-RSA-SHA256 Credential=373719072408053199536118061820892065223142768195/20231119/us-east-1/rolesanywhere/aws4_request, SignedHeaders=content-type;host;x-amz-date;x-amz-x509, Signature=468a912018ab256fc64392c0a30ab4f7e281dd00b94548e0a46ab7b13d08d5273811a8960a3e7e6036823dfe54b23d9c0fc8ab317db2200e1c70f83db552c141c695a63643b14a3603c64584c7e3064380bd7932fb3b2bf3a16b1c0c6fc8549722730e5f1f28f473bf698474d23da2b2d604535d20eaec157f99aa6771e6e54d85bad12b6388ae2de8de9963b455ad8fcea841a0056acae6de974ce7487f450a4263205454f6b901085438dc6f0c54eb7283c30ad93fec3c53dc7ab77667b985f182b5acedf6756891e277a740cc47bcf0da695d7829cd83a7504e0605415d0f77cac557fee034ef9b18076537865fe40aab27ae5f07b6139225c52a72435c6f";
    }
}
