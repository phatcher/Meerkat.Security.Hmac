using System.Diagnostics;

using log4net;

using NUnit.Framework;

[SetUpFixture]
public class TestInitializer
{
    [OneTimeSetUp]
    public void Initialize()
    {
        log4net.Config.XmlConfigurator.Configure();
        if (LogManager.GetRepository().Configured)
        {
            Debug.WriteLine("configured");
        }
    }
}
