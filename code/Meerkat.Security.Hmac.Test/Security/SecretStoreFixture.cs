using Meerkat.Security;

using NUnit.Framework;

namespace Meerkat.Test.Security
{
    [TestFixture]
    public class SecretStoreFixture
    {
        [Test]
        public void ClientPresent()
        {
            var store = new SecretStore();

            store.Assign("client", "A");

            var secret = store.ClientSecret("client");

            Assert.That(secret, Is.EqualTo("A"));
        }

        [Test]
        public void ClientNotPresent()
        {
            var store = new SecretStore();

            var secret = store.ClientSecret("client");

            Assert.That(secret, Is.Null);
        }
    }
}