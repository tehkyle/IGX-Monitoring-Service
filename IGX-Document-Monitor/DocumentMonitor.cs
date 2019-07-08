using Ingeniux.Monitoring;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace IGX_Document_Monitor
{
	public partial class DocumentMonitor : ServiceBase
	{
		Ingeniux.Monitoring.DocumentMonitor Monitor;

		public DocumentMonitor()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			var opts = new DocumentMonitorOptions();
			opts.ConnectionString = ConfigurationManager.ConnectionStrings["RavenDB"].ConnectionString;
			opts.Logger = NLog.LogManager.GetLogger("igxMonitoring");
			var settingsRoot = XElement.Load(new XmlNodeReader(Properties.Settings.Default.DocumentSettings));
			var settings = settingsRoot.Elements("DocumentSetting");
			List<DocumentQueryOptions> docQueries = new List<DocumentQueryOptions>();
			foreach (var setting in settings)
				docQueries.Add(new DocumentQueryOptions()
				{
					Name = setting.Element("Name")?.Value,
					ReportDocument = setting.Element("ReportDocument")?.Value.ToLowerInvariant() == "true",
					Regex = new Regex(setting.Element("Regex")?.Value, RegexOptions.IgnoreCase)
				});
			opts.DocumentIdRegexes = docQueries.ToArray();

			Monitor = new Ingeniux.Monitoring.DocumentMonitor(opts);
		}

		protected override void OnStop()
		{
			Monitor.Dispose();
		}
	}
}
