using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SaiphIamRolesAnywhere
{

    public class AwsCredential
    {
        public string AccessKeyId { get; set; }

        public string SecretAccessKey { get; set; }

        public string SessionToken { get; set; }

        public DateTime Expiration {  get; set; }
    }

    public class AwsCredentialItem
    {
        //object AssumedRoleUser { get; set; }
        public AwsCredential Credentials { get; set; }
    }

    public class AwsErrorResponse
    {
        public string Message { get; set; }
    }

    public class AwsCredentialResponse
    {
        public List<AwsCredentialItem> CredentialSet { get; set; }

        public static bool TryGetErrorMessage(string json, out string message)
        {
            message = null;

            if (json.StartsWith("{\"message\":"))
            {
                var res = JsonSerializer.Deserialize<AwsErrorResponse>(json, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                message = res.Message;
                return true;
            }

            return false;
        }
    }
}
