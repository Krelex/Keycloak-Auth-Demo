using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppDemo.Models
{
	public class Authority
	{
		public string Url { get; set; }

		public string EndpointAddress { get; set; }

		public string RealmName { get; set; }

		public string Protocol { get; set; }

		public string ClientId { get; set; }

		public string ClientSecret { get; set; }
	}
}
