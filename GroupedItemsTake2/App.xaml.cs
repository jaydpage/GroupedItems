using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using GroupedItemsTake2.Logging;
using log4net;

namespace GroupedItemsTake2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ILog _logger;

        public App()
        {
            _logger = new LogFactory().Create();
            _logger.Info(string.Format("Application Started, Logger resolved"));            
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _logger.Info(string.Format("Application Exited"));
            base.OnExit(e);
        }
       
    }
}
