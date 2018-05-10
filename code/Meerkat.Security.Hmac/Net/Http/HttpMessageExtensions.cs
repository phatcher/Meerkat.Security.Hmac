using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Meerkat.Net.Http
{
    /// <summary>
    /// Helper methods for HttpRequestMessage and headers
    /// </summary>
    public static class HttpMessageExtensions
    {
        /// <summary>
        /// Return typed values from the headers collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="headers"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>(this HttpRequestHeaders headers, string name)
        {
            if (!headers.TryGetValues(name, out var values))
            {
                yield break;
            }

            foreach (var value in values)
            {
                yield return (T) Convert.ChangeType(value, typeof(T));
            }
        }

        /// <summary>
        /// Get the request date as a string if present.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string MessageDate(this HttpRequestHeaders headers)
        {
            var date = headers.Date?.UtcDateTime;

            // Use RFC 1123 so we are the same as the header value.
            return date?.ToString("r");
        }

        /// <summary>
        /// Provides a representation of custom headers content, name of which are in another header
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="customHeaderName"></param>
        /// <returns>Custom header values separated by \\n, otherwise string.Empty if no custom headers, null if any custom header is missing</returns>
        /// <remarks>If multiple values exist for a custom header they are joined with ", " since HttpClient will follow this policy and it also complies with RFC2616</remarks>
        public static string CustomHeadersRepresentation(this HttpRequestHeaders headers, string customHeaderName)
        {
            var customHeaders = headers.GetValues<string>(customHeaderName).FirstOrDefault();
            if (string.IsNullOrEmpty(customHeaders))
            {
                // No custom headers required, return empty, not null, to avoid failing the signature
                return string.Empty;
            }

            var values = new List<string>();
            foreach (var headerName in customHeaders.Split(' '))
            {
                var hv = headers.GetValues<string>(headerName).ToList();
                if (hv.Count == 0)
                {
                    return null;
                }
                // TODO: Do we need to iterate over to make sure we don't have empty/null values?
                var value = string.Join(", ", hv);

                values.Add(value);
            }

            var result = string.Join("\n", values);
            return result;
        }

        /// <summary>
        /// Checks that the message request date is within bounds of the validity period
        /// </summary>
        /// <param name="requestMessage">Request to check</param>
        /// <param name="validityPeriod">Validity period in minutes</param>
        /// <returns></returns>
        public static bool IsMessageDateValid(this HttpRequestMessage requestMessage, int validityPeriod)
        {
            var utcNow = DateTime.UtcNow;
            var requestDate = requestMessage.Headers.Date;
            if (requestDate.HasValue == false)
            {
                // No date present
                return false;
            }

            var date = requestDate.Value.UtcDateTime;

            // Check it's in the period allowing for clock skew
            return date < utcNow.AddMinutes(validityPeriod)
                && date > utcNow.AddMinutes(-validityPeriod);
        }

        /// <summary>
        /// Process headers for a specified header and convert the values into claims
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="headerName">Header name to search for</param>
        /// <param name="claimType">Claim type to create</param>
        /// <param name="issuer">Issuer to use</param>
        /// <param name="separator">Separator for multiple values in one header</param>
        /// <param name="trim">Whether to trim the values, needed as by default HttpClient delimits values with ", "</param>
        /// <returns></returns>
        public static IEnumerable<Claim> ToClaims(this HttpRequestHeaders headers, string headerName, string claimType, string issuer = null, char separator = ',', bool trim = true)
        {
            if (!headers.TryGetValues(headerName, out var values))
            {
                yield break;
            }

            foreach (var header in values)
            {
                foreach (var value in header.Split(separator))
                {
                    var cv = trim ? value.Trim() : value;
                    yield return new Claim(claimType, cv, issuer);
                }
            }
        }
    }
}