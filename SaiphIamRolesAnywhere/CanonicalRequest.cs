using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SaiphIamRolesAnywhere
{
    public class CanonicalRequest
    {
        const string EMPTY_SHA256 = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
        const string DURATION_SHA256 = "1a15f67f6619aa540b13e9a4c37d149fb2c9bdea25d82b73a3878826694dfe27";
        const string DURATION_BODY = "{\"durationSeconds\":3600}";

        public string RequestString {  get; set; }
        public string HashedRequestString { get; set; }
        public string StringToSign { get; set; }
        public string Scope { get; set; }
        public string QueryString { get; set; }

        private Dictionary<string, string> Headers { get; set; }
        private X509Certificate Certificate { get; set; }
        private string Region { get; set; }

        public async Task<AwsCredentialResponse> Send(string signature, bool debug = false)
        {
            var auth = GetAuth(signature);
            if (debug) { Console.WriteLine($"----evaluated auth header----\n" + auth + "---------\n"); }

            var uri = $"https://rolesanywhere.{Region}.amazonaws.com/sessions?{QueryString}";

            using (var client = new HttpClient())
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                foreach (var header in Headers)
                    requestMessage.Headers.Add(header.Key, header.Value);

                requestMessage.Headers.TryAddWithoutValidation("Authorization", auth);

                requestMessage.Content = new StringContent(DURATION_BODY);
                requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var res = await client.SendAsync(requestMessage);
                var json = await res.Content.ReadAsStringAsync();

                if (AwsCredentialResponse.TryGetErrorMessage(json, out var error))
                    throw new Exception(error);
                else
                    return JsonSerializer.Deserialize<AwsCredentialResponse>(json, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }

        public string GetAuth(string signature)
        {
            var creds = $"{Certificate.GetNumericSerial()}/{Scope}";
            return $"AWS4-X509-RSA-SHA256 Credential={creds}, SignedHeaders=content-type;host;x-amz-date;x-amz-x509, Signature={signature}";
        }

        public static CanonicalRequest Create(
            X509Certificate certificate,
            string profileArn,
            string roleArn,
            string trustAnchorArn,
            DateTime? dateTime = null,
            bool debug = false
        )
        {
            var request = new CanonicalRequest()
            {
                Certificate = certificate,
            };
            string cert64 = certificate.GetBase64String();

            // sanity check
            if (string.IsNullOrEmpty(profileArn))
            {

            }

            // eval region from trust anchor arn
            request.Region = Utility.ExtractRegion(trustAnchorArn);
            if (debug) { Console.WriteLine($"evaluated region form trust anchor arn: {request.Region}"); }

            if (!dateTime.HasValue)
            {
                dateTime = DateTime.UtcNow;
            }
            request.Scope = dateTime.Value.ToString("yyyyMMdd") + $"/{request.Region}/rolesanywhere/aws4_request";
            if (debug) { Console.WriteLine($"evaluated scope stirng: {request.Scope}"); }

            string tmz = dateTime.Value.ToString(@"yyyyMMdd\THHmmss\Z");
            if (debug) { Console.WriteLine($"evaluated date stirng: {tmz}"); }

            request.QueryString = $"profileArn={Utility.UriEncode(profileArn)}&roleArn={Utility.UriEncode(roleArn)}&trustAnchorArn={Utility.UriEncode(trustAnchorArn)}";
            if (debug) { Console.WriteLine($"evaluated query string: {request.QueryString}"); }

            using (SHA256 sha = SHA256.Create())
            {
                request.RequestString = "POST\n" +
                    "/sessions\n" +
                    request.QueryString + "\n" +
                    "content-type:application/json\n" +
                    $"host:rolesanywhere.{request.Region}.amazonaws.com\n" +
                    "x-amz-date:" + tmz + "\n" + 
                    "x-amz-x509:" + cert64 + "\n\n" +
                    "content-type;host;x-amz-date;x-amz-x509\n" +
                    DURATION_SHA256;
                if (debug) { Console.WriteLine("----evaluated request string----\n" + request.RequestString + "\n----------------"); }

                request.HashedRequestString = Utility.HashText(request.RequestString, sha);
                if (debug) { Console.WriteLine("----evaluated hashed request----\n" + request.HashedRequestString + "\n----------------"); }

                //
                request.StringToSign = Encoding.UTF8.GetString(
                    Encoding.Default.GetBytes(
                        "AWS4-X509-RSA-SHA256\n" +
                        tmz + "\n" +
                        request.Scope + "\n"
                    )) +
                    request.HashedRequestString;
                if (debug) { Console.WriteLine("----evaluated string to sign----\n" + request.StringToSign + "\n----------------"); }

                request.Headers = new Dictionary<string, string>
                {
                    { "X-Amz-Date", tmz },
                    { "X-Amz-X509", cert64 },
                    { "User-Agent", "SaiphRolesAnywhere1.0 (windows; amd64)" },
                    { "Transfer-Encoding", "chunked" },
                    { "Accept-Encoding", "gzip" }
                };
                return request;
            }
        }
    }
}
