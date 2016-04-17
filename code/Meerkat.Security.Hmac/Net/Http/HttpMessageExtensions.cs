using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Meerkat.Net.Http
{
    public static class HttpMessageExtensions
    {
        public static T GetFirstOrDefaultValue<T>(this HttpRequestHeaders headers, string name)
        {
            IEnumerable<string> values;

            if (!headers.TryGetValues(name, out values))
            {
                return default(T);
            }

            // NB Must exist as the TryGetValues succeeded
            var candidate = values.First();

            return (T)Convert.ChangeType(candidate, typeof(T));
        }

        /// <summary>
        /// Get the request date as a string if present.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string MessageDate(this HttpRequestHeaders headers)
        {
            if (!headers.Date.HasValue)
            {
                return null;
            }
            var date = headers.Date.Value.UtcDateTime;

            return date.ToString("u");
        }

        /// <summary>
        /// Provides a representation of custom headers content, name of which are in another header
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="customHeaderName"></param>
        /// <returns>Custom header values separated by \\n, otherwise string.Empty if no custom headers, null if any custom header is missing</returns>
        public static string CustomHeadersRepresentation(this HttpRequestHeaders headers, string customHeaderName)
        {
            var customHeaders = headers.GetFirstOrDefaultValue<string>(customHeaderName);
            if (string.IsNullOrEmpty(customHeaders))
            {
                // No custom headers required, return empty, not null, to avoid failing the signature
                return string.Empty;
            }

            var values = new List<string>();
            foreach (var headerName in customHeaders.Split(' '))
            {
                var value = headers.GetFirstOrDefaultValue<string>(headerName);
                if (value == null)
                {
                    // Fail on first null
                    //Logger.Debug("Missing custom header: " + headerName);
                    return null;
                }
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
    }
}