using log4net;
using log4net.Config;

namespace GroupedItemsTake2
{
    public class LogFactory
    {
        public ILog Create()
        {
            XmlConfigurator.Configure();
            return LogManager.GetLogger(typeof(App));
        }
    }
}
