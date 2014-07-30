using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mymdb.Model
{
	public class MovieDTO
	{
		[JsonProperty(PropertyName = "page")]
		public int Page { get; set; }

		[JsonProperty(PropertyName = "results")]
		public IEnumerable<Movie> Movies { get; set; }
	}
}

