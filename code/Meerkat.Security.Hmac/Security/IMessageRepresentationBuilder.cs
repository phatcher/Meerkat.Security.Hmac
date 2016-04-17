using System.Net.Http;

namespace Meerkat.Security
{
    public interface IMessageRepresentationBuilder
    {
        string BuildRequestRepresentation(HttpRequestMessage requestMessage);
    }
}