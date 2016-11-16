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
        public async Task SecureNoHmac()
        {
            await OnSecureNoHmac();
        }

        [Test]
        public async Task InvalidClientId()
        {
            await OnInvalidClientId();
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