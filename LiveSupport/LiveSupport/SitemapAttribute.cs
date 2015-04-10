using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LiveSupport
{

	[AttributeUsage(AttributeTargets.Method)]
	public class SitemapAttribute : Attribute
	{

		#region Properties

		private string _changeFrequency;
		public string ChangeFrequency
		{
			get { return _changeFrequency; }
			set { _changeFrequency = value; }
		}

		private bool _ignoreAuthorizeAttribute = false;
		public bool IgnoreAuthorizeAttribute
		{
			get { return _ignoreAuthorizeAttribute; }
			set { _ignoreAuthorizeAttribute = value; }
		}

		private DateTime _lastModified;
		public DateTime LastModified
		{
			get { return _lastModified; }
			set { _lastModified = value; }
		}

		private float _priority = 0.5f;
		public float Priority
		{
			get { return _priority; }
			set
			{
				if (value < 0.0f || value > 1.0f)
				{
					throw new ArgumentOutOfRangeException();
				}
				_priority = value;
			}
		}

		private string _queryKey = String.Empty;
		public string QueryKey
		{
			get { return _queryKey; }
			set { _queryKey = value; }
		}

		private string _route = String.Empty;
		public string Route
		{
			get { return _route; }
			set { _route = value; }
		}


		#endregion

		#region Constructors

		public SitemapAttribute()
		{
		
		}

		#endregion


	}

}