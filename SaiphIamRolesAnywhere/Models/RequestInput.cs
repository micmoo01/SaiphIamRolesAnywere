using System;
using System.Collections.Generic;
using System.Text;

namespace SaiphIamRolesAnywhere
{
    public class CanonicalRequestInput
    {
        public string ProfileArn { get; set; }
        public string RoleArn { get; set; }

        public string TrustAnchorArn { get; set; }
    }
}
