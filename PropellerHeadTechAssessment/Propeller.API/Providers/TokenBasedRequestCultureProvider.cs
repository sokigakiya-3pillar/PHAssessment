using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace Propeller.API.Providers
{
    public class TokenBasedRequestCultureProvider : RequestCultureProvider
    {
        public int MaximumAcceptLanguageHeaderValuesToTry { get; set; } = 3;

        public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            // Try to retrieve the locale from the Bearer Token
            // Retrieve the Bearer Token
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (token != null)
            {
                token = token.Replace("Bearer ", "").ToString();

                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                if (jwt != null && jwt.Claims != null)
                {
                    var localeClaim = jwt.Claims.First(c => c.Type == Constants.LocaleClaim).Value;

                    if (!string.IsNullOrEmpty(localeClaim))
                    {
                        return Task.FromResult(new ProviderCultureResult(localeClaim));
                    }
                }
            }

            // If we're unable to retrieve from Bearer Token, try to retrieve it from the
            // Accepted-Language header
            var acceptLanguageHeader = httpContext.Request.GetTypedHeaders().AcceptLanguage;

            if (acceptLanguageHeader == null || acceptLanguageHeader.Count == 0)
            {
                return NullProviderCultureResult;
            }

            var languages = acceptLanguageHeader.AsEnumerable();

            if (MaximumAcceptLanguageHeaderValuesToTry > 0)
            {
                // We take only the first configured number of languages from the header and then order those that we
                // attempt to parse as a CultureInfo to mitigate potentially spinning CPU on lots of parse attempts.
                languages = languages.Take(MaximumAcceptLanguageHeaderValuesToTry);
            }

            var orderedLanguages = languages.OrderByDescending(h => h, StringWithQualityHeaderValueComparer.QualityComparer)
                .Select(x => x.Value).ToList();

            if (orderedLanguages.Count > 0)
            {
                return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(orderedLanguages));
            }

            //var identity = httpContext.User.Identity as ClaimsIdentity;
            //if (identity != null)
            //{
            //    IEnumerable<Claim> claims = identity.Claims;
            //    // or
            //    // identity.FindFirst("ClaimName").Value;

            //}

            //var localeClaim = httpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.LocaleClaim);

            //if (localeClaim != null)
            //{
            //    if (!string.IsNullOrEmpty(localeClaim.Value))
            //    {
            //        return Task.FromResult(new ProviderCultureResult(localeClaim.Value));
            //    }
            //}

            //             new ProviderCultureResult("es-MX")
            //          var c=httpContext.User.cul
            // TODO: Investigate about this warning

            // Default
            return NullProviderCultureResult;
            // return Task.FromResult(new ProviderCultureResult("en-NZ"));

            // User
            //      throw new NotImplementedException();
        }
    }
}
