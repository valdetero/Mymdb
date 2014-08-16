using System;
using SQLite.Net.Attributes;
using Newtonsoft.Json;
using System.IO;

namespace Mymdb.Model
{
	public class Movie : Interfaces.IBusinessEntity
	{
		[PrimaryKey]
		public int Id { get; set; }

		[JsonProperty(PropertyName = "imdb_id"), Column("ImdbId"), MaxLength(20)]
		public string ImdbId { get; set; }

		[JsonProperty(PropertyName = "title"), MaxLength(20)]
		public string Title { get; set; }

		[JsonProperty(PropertyName = "poster_path"), MaxLength(100)]
		public string ImageUrl { get; set; }

		[JsonProperty(PropertyName = "runtime")]
		public int Runtime { get; set; }

		[JsonProperty(PropertyName = "release_date")]
		public DateTime ReleaseDate { get; set; }

		public bool IsFavorite { get; set; }

		public byte[] Image { get; set; }

		[MaxLength(500)]
		public string ImagePath { get; set; }
	}
}

