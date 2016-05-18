using System.Net.Http;

namespace Meerkat.Security
{
    /// <summary>
    /// Responsible for constructing a canonical representation of a message.
    /// </summary>
    public interface IMessageRepresentationBuilder
    {
        /// <summary>
        /// Build a canonical representation of a message
        /// </summary>
        /// <param name="requestMessage">Message to use</param>
        /// <returns></returns>
        string BuildRequestRepresentation(HttpRequestMessage requestMessage);
    }
}