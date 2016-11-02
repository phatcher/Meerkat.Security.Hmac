using System;
using System.Net.Http;
using System.Windows.Forms;

using Meerkat.Net.Http;

using Microsoft.Practices.Unity;

namespace Sample.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private IUnityContainer Container
        {
            get { return UnityConfig.GetConfiguredContainer(); }
        }

        public void Get()
        {
            using (var client = new HttpClient(CreateHandler()))
            {
                client.BaseAddress = new Uri("http://localhost:25883/");
            }
        }

        public HttpMessageHandler CreateHandler()
        {
            var client = new HttpClientHandler();

            var hmac = Container.Resolve<HmacSigningHandler>();
            hmac.InnerHandler = client;

            var md5 = new RequestContentMd5Handler
            {
                InnerHandler = hmac
            };

            return md5;
        }
    }
}
