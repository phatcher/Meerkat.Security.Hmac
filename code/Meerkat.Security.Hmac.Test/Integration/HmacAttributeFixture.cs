using System.Threading.Tasks;

using NUnit.Framework;

namespace Meerkat.Test.Integration
{
    [TestFixture]
    public class HmacAttributeFixture : HmacFixture
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

        public override void SetUp()
        {
            base.SetUp();

            Sample.Web.Startup.UseOwinHmac = false;
        }
    }
}