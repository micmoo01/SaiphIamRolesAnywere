# SaiphIamRolesAnywere
C# library port of AWS roles anywhere credential helper

## References
[Rolesanywhere golang project](https://github.com/aws/rolesanywhere-credential-helper)

[Rolesanywhere signing documentation](https://docs.aws.amazon.com/rolesanywhere/latest/userguide/authentication-sign-process.html)

### Prerequisites
Have AWS IAMRolesAnywhere TrustAnchor and Profile setup. [see details](https://docs.aws.amazon.com/rolesanywhere/latest/userguide/getting-started.html)  
X509 certificate compatible with RolesAnyhere signature validation. [see details](https://docs.aws.amazon.com/rolesanywhere/latest/userguide/trust-model.html#signature-verification)  
Certificate can either be in **PFX** with private key included or seperate **PEM** certificate file and private key file  
Currently only RSA2048 private keys are supported.

## Usage
### Acquire Certificate and Private Key
from PFX certificate file
> [!NOTE]
> file should be password protected.
```
var pfxPath = Path.GetFullPath("Res\\testcert.pfx");
var (rsaPrivateKey, certificate) = CertificateProvider.ImportPfx(pfxPath);
```
from seperate PEM files
```
var path = Path.GetFullPath("Res");
var (rsaPrivateKey, certificate) = CertificateProvider.FromPemFiles(Path.Combine(path, "testkey.pem"), Path.Combine(path, "testcert.pem"));
```

### Create Canonical Request
```
const string profileArn = "arn:aws:rolesanywhere:us-east-1:12345678:profile/e6a5cf90-8983-4f32-bacf-75614d7e3343";
const string roleArn = "arn:aws:iam::12345678:role/somerole";
const string anchorArn = "arn:aws:rolesanywhere:us-east-1:12345678:trust-anchor/e6a5cf90-fc93-478a-bacf-75614d7e3343";

var request = CanonicalRequest.Create(
    certificate,
    profileArn,
    roleArn,
    anchorArn);
```

### Sign Request
> [!NOTE]
> rsaPrivateKey gets disposed after signing
```
var signature = Utility.Sign(rsaPrivateKey, request.StringToSign);
```

### Send Request for Credentials
```
try
{
    var authRes = await request.Send(signature);
    var credentials = authRes.CredentialSet[0].Credentials;
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
```

## Dependency Injection Example
```
using SaiphIamRolesAnywhere;
using SaiphIamRolesAnywhere.DI;

// SETUP
var services = new ServiceCollection();
services.AddRolesAnywhere(new RolesAnywhereServiceParams()
{
    ProfileArn = profileArn,
    RoleArn = roleArn,
    TrustAnchorArn = anchorArn,
    CertificatePath = "testcert.pfx"
});
services.AddSingleton<App>();
// -------

// RUN APP
await services
    .BuildServiceProvider()
    .GetService<App>()
    .RunAsync();
// -------

// APP Class
class App
{
    private readonly IRolesAnywhereService _roles;

    public App(IRolesAnywhereService rolesAnywhere)
    {
        _roles = rolesAnywhere;
    }

    internal async Task RunAsync()
    {
        try
        {
            var creds = await _roles.GetAwsCredentials();
            Console.WriteLine(creds.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
```
