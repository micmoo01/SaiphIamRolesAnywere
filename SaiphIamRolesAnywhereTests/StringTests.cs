using SaiphIamRolesAnywhere;
using System.Text;
using System.Web;

namespace SaiphIamRolesAnywhereTests
{
    public class StringTests
    {
        [Theory]
        [InlineData(":bob", "%3Abob")]
        [InlineData("/john", "%2Fjohn")]
        public void UriEncode(string input, string expected)
        {
            var result = Utility.UriEncode(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void UriEncodeQueryString()
        {
            var result = $"profileArn={Utility.UriEncode(TestParams.ProfileArn)}&roleArn={Utility.UriEncode(TestParams.RoleArn)}&trustAnchorArn={Utility.UriEncode(TestParams.TrustAnchorArn)}";
            var expected = "profileArn=arn%3Aaws%3Arolesanywhere%3Aus-east-1%3A12345678%3Aprofile%2Fe6a5cf90-8983-4f32-bacf-75614d7e3343&roleArn=arn%3Aaws%3Aiam%3A%3A12345678%3Arole%2Fsomerole&trustAnchorArn=arn%3Aaws%3Arolesanywhere%3Aus-east-1%3A12345678%3Atrust-anchor%2Fe6a5cf90-fc93-478a-bacf-75614d7e3343";
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("arn:aws:rolesanywhere:us-east-1:12345678:trust-anchor/e6a5cf90-fc93-478a-bacf-75614d7e3343", "us-east-1")]
        [InlineData("us-west-2-bacf-75614d7e3343", "us-west-2")]
        [InlineData("arn:::us-isob-east-1:12345678/75614d7e3343", "us-isob-east-1")]
        [InlineData("no-valid region in string", "")]
        [InlineData(null, "")]
        public void ExtractRegionFromArn(string inputArn, string expectedRegion)
        {
            var result = Utility.ExtractRegion(inputArn);
            Assert.Equal(expectedRegion, result);
        }
    }
}