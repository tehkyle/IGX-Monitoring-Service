using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Configuration;

namespace Ingeniux.Monitoring
{
	public class MonitorOptions
	{
		public string ConnectionString { get; set; }
		public NLog.Logger Logger { get; set; }
	}

	public class DocumentMonitorOptions : MonitorOptions
	{
		public DocumentQueryOptions[] DocumentIdRegexes { get; set; }
	}

	[Serializable]
	public class DocumentQueryOptions
	{
		public string Name { get; set; }
		public Regex Regex { get; set; }
		public bool ReportDocument { get; set; }
	}

	[SettingsSerializeAsAttribute(SettingsSerializeAs.Xml)]
	public class DocumentQueryCollection : System.Collections.IList
	{
		public DocumentQueryCollection()
		{
			_queryOpts = new List<DocumentQueryOptions>();
		}

		public List<DocumentQueryOptions> _queryOpts = new List<DocumentQueryOptions>();

		public object this[int index] { get => _queryOpts[index]; set => _queryOpts[index] = value as DocumentQueryOptions; }

		public bool IsFixedSize => false;

		public bool IsReadOnly => false;

		public int Count => _queryOpts.Count;

		public bool IsSynchronized => false;

		public object SyncRoot => null;

		public int Add(object value)
		{
			_queryOpts.Add(value as DocumentQueryOptions);
			return Count;
		}

		public void Clear()
		{
			_queryOpts.Clear();
		}

		public bool Contains(object value)
		{
			return _queryOpts.Contains(value as DocumentQueryOptions);
		}

		public void CopyTo(Array array, int index)
		{
			_queryOpts.CopyTo(array as DocumentQueryOptions[], index);
		}

		public IEnumerator GetEnumerator()
		{
			return _queryOpts.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			return _queryOpts.IndexOf(value as DocumentQueryOptions);
		}

		public void Insert(int index, object value)
		{
			_queryOpts.Insert(index, value as DocumentQueryOptions);
		}

		public void Remove(object value)
		{
			_queryOpts.Remove(value as DocumentQueryOptions);
		}

		public void RemoveAt(int index)
		{
			_queryOpts.RemoveAt(index);
		}
	}
}
