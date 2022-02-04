using Microsoft.AspNetCore.Http;
using Ordering.API.Helpers;
using System.Linq;

namespace Ordering.API.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        ///  Parses the claims inside a Json Web Token (Jwt) in the 'Authorization' header of an HttpContext
        /// </summary>
        /// <returns>A string containing the value of the supplied claim type</returns>
        public static string GetClaimValueByType(this HttpContext context, string claimType)
        {
            string value = "";

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var claims = JwtParse.ParseClaimsFromJwt(token);
            value = claims?.FirstOrDefault(c => c.Type == claimType)?.Value;

            return value;
        }

        /// <summary>
        ///  Gets a Json Web Token (Jwt) from the 'Authorization' header of an HttpContext
        /// </summary>
        /// <returns>A string containing a Jwt</returns>
        public static string GetJwtFromContext(this HttpContext context)
        {
            string token = "";

            token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            return token;
        }
    }
}
