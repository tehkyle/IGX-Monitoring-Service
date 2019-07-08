﻿using NLog;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ingeniux.Monitoring
{
	public class MonitorBase
	{
		Logger Logger;

		public MonitorBase(DocumentMonitorOptions opts)
		{
			Logger = opts.Logger;
		}

		//protected Logger ConfigureLogger(DocumentMonitorOptions opts)
		//{
		//	var config = new NLog.Config.LoggingConfiguration();
		//	var logFile = new NLog.Targets.FileTarget("Ingeniux.Monitoring.Log")
		//	{
		//		FileName = "Ingeniux.Monitoring.log"
		//	};

		//	if (opts.LogLevel == null)
		//		opts.LogLevel = LogLevel.Debug;

		//	config.AddRule(opts.LogLevel, LogLevel.Fatal, logFile);

		//	NLog.LogManager.Configuration = config;
		//	return NLog.LogManager.GetCurrentClassLogger();
		//}

		public void Log(string message, LogLevel level = null)
		{
			var mgr = System.Configuration.ConfigurationManager;
			if (level == null)
				level = LogLevel.Info;
			//message = $"[{level.ToString()}][{DateTime.Now.ToString()}] -- {message}";
			Logger.Log(level, message);
		}
	}

	public class DocumentMonitor : MonitorBase
	{
		DocumentStore Store;
		DocumentMonitorOptions Options;
		

		public DocumentMonitor(DocumentMonitorOptions opts) : base(opts)
		{
			Options = opts;

			Store = new DocumentStore();
			Store.ParseConnectionString(Options.ConnectionString);
			Store.Changes()
				.ForAllDocuments()
				.Subscribe(change =>
				{
					ParseChange(change);
				});
		}

		protected void ParseChange(DocumentChangeNotification change)
		{
			var docId = change.Id;
			foreach (var search in Options.DocumentIdRegexes)
			{
				if (search.Regex.IsMatch(docId))
				{
					var type = change.Type;
					Log($"({search.Name}) Change detected on {docId}: {type.ToString()} [{change.Message}]");
					if (search.ReportDocument)
					{
						JsonDocument doc = Store.DatabaseCommands.Get(docId);
						Log($"Document Contents: {doc.ToString()}", LogLevel.Debug);
					}
					break;
				}
			}
		}
	}
}