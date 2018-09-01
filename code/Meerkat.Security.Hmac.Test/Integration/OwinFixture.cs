using System.Threading.Tasks;

using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;

using NUnit.Framework;

namespace Meerkat.Hmac.Test.Integration
{
    [TestFixture]
    public class OwinFixture : HmacFixture
    {
        [Test]
        public async Task AllowAnonymous()
        {
            await OnAllowAnonymous();
        }

        [Test]
        public async Task Secured()
        {
            await OnSecured();
        }

        [Test]
        public async Task SecuredZeroLength()
        {
            await OnSecuredZeroLength();
        }

        [Test]
        public async Task SecureNoHmac()
        {
            await OnSecureNoHmac();
        }

        [Test]
        public async Task Post()
        {
            await OnPost();
        }

        [Test]
        public async Task PostZeroLength()
        {
            await OnPostZeroLength();
        }

        [Test]
        public async Task PostNoHmac()
        {
            await OnPostNoHmac();
        }

        [Test]
        public async Task InvalidClientId()
        {
            await OnInvalidClientId();
        }

        [Test]
        public async Task InvalidScheme()
        {
            await OnInvalidScheme();
        }

        [Test]
        public async Task MissingSignature()
        {
            await OnMissingSignature();
        }

        //[Test]
        //[Ignore("Can't do it fast enough")]
        //public async Task ReplayAttack()
        //{
        //    await OnReplayAttack();
        //}

        //public async Task NoncedRequests()
        //{
        //    await OnNoncedRequests();
        //}

        //[Test]
        //[Ignore("Can't set Date, client overrwrites")]
        //public async Task MessageDateTooEarly()
        //{
        //    await OnMessageDateTooEarly();
        //}

        //[Test]
        //[Ignore("Can't set Date, client overrwrites")]
        //public async Task MessageDateTooLate()
        //{
        //    await OnMessageDateTooLate();
        //}

        [Test]
        public void ResolveSecretStore()
        {
            Container = Sample.Web.UnityConfig.GetConfiguredContainer();
            CheckSingleton<ISecretStore>();
        }

        [Test]
        public void ResolveSecretRepository()
        {
            Container = Sample.Web.UnityConfig.GetConfiguredContainer();
            CheckSingleton<ISecretRepository>();
        }

        [Test]
        public void ResolveMessageRepresentationBuilder()
        {
            Container = Sample.Web.UnityConfig.GetConfiguredContainer();
            CheckTransient<IMessageRepresentationBuilder>();
        }

        [Test]
        public void ResolveSignatureCalculator()
        {
            Container = Sample.Web.UnityConfig.GetConfiguredContainer();
            CheckTransient<ISignatureCalculator>();
        }

        [Test]
        public void ResolveSignatureValidator()
        {
            Container = Sample.Web.UnityConfig.GetConfiguredContainer();
            CheckTransient<ISignatureValidator>();
        }

        [Test]
        public void ResolveHmacAuthenticator()
        {
            Container = Sample.Web.UnityConfig.GetConfiguredContainer();
            CheckTransient<IHmacAuthenticator>();
        }

        public override void SetUp()
        {
            base.SetUp();

            Sample.Web.Startup.UseOwinHmac = true;
        }
    }
}