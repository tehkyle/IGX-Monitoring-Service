using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;
using NLog;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Imports.Newtonsoft.Json;
using Raven.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
			if (level == null)
				level = LogLevel.Info;
			//message = $"[{level.ToString()}][{DateTime.Now.ToString()}] -- {message}";
			Logger.Log(level, message);
		}
	}

	public class DocumentMonitor : MonitorBase, IDisposable
	{
		DocumentStore Store;
		DocumentMonitorOptions Options;
		string DocumentsDirectory;

		public DocumentMonitor(DocumentMonitorOptions opts) : base(opts)
		{
			Options = opts;

			DocumentsDirectory = Path.Combine(Options.OutputDirectory, "DocumentsHistory");
			if (!Directory.Exists(DocumentsDirectory))
				Directory.CreateDirectory(DocumentsDirectory);

			try
			{
				Store = new DocumentStore();
				Store.ParseConnectionString(Options.ConnectionString);
				Store.Initialize();
				Store.Changes()
					.ForAllDocuments()
					.Subscribe(change =>
					{
						Log($"Change Detected: {change.Id}", LogLevel.Debug);
						ParseChange(change);
					});
			}
			catch (Exception e)
			{
				Log(e.ToString(), LogLevel.Error);
			}

			Log("Listening...");
		}

		public void Dispose()
		{
			Log("Shutting Down...");
			Store.Dispose();
		}

		protected void ParseChange(DocumentChangeNotification change)
		{
			try
			{
				var docId = change.Id;
				foreach (var search in Options.DocumentIdRegexes)
				{
					if (search.Regex.IsMatch(docId))
					{
						var type = change.Type;
						var msg = !string.IsNullOrEmpty(change.Message) ? $"[{change.Message}]" : "";
						Log($"({search.Name}) Change detected on {docId}: {type.ToString()} {msg}");
						if (search.TrackChanges)
						{
							RavenJObject doc = Store.DatabaseCommands.Get(docId).ToJson();
							string docDir = Path.Combine(DocumentsDirectory, docId);
							if (!Directory.Exists(docDir))
								Directory.CreateDirectory(docDir);

							var existingFiles = Directory.EnumerateFiles(docDir, "*.json").Select(f => new FileInfo(f)).OrderByDescending(f => f.CreationTime).ToArray();
							var lastFile = existingFiles.FirstOrDefault();

							string docPath = Path.Combine(docDir, DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".json");
							string jsonStr = doc.ToString();
							File.WriteAllText(docPath, jsonStr);

							Log($"Document contents written to: {docPath}", LogLevel.Debug);

							if (lastFile != null)
							{
								string oldJsonStr = File.ReadAllText(lastFile.FullName);
								var jdp = new JsonDiffPatch();
								var oldJson = JToken.Parse(oldJsonStr);
								var newJson = JToken.Parse(jsonStr);
								var diff = jdp.Diff(oldJson, newJson);

								Log($"Detected Differences are: {diff.ToString()}", LogLevel.Info);
							}

							if (search.VersionLimit > 0 && existingFiles.Length >= search.VersionLimit)
							{
								var filesToRemove = existingFiles.Skip(search.VersionLimit - 1).ToArray();
								foreach (var file in filesToRemove)
									file.Delete();
							}
						}
						break;
					}
				}
			}
			catch (Exception e)
			{
				Log(e.ToString(), LogLevel.Error);
				throw e;
			}
		}
	}
}
