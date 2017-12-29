using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dapper_SimpleWebApp.Models
{
	[Table("article")]
	public class Article
	{
		public long Id { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }
		public DateTime Date { get; set; }

		[Computed]
		public Author Author { get; set; }

		[Computed]
		public List<Tag> Tags { get; set; }

		[DisplayName("Author")]
		public int AuthorId { get; set; }

		public Article()
		{
			Date = DateTime.Now;
			Id = 0;
		}

		[Computed]
		public string TagsCSV {get;set;}

		public string TagsForDisplay()
		{
			if(Tags != null && Tags.Count > 0)
			{
				var csv = String.Join(", ", Tags.Select(tag => tag.Name).ToArray());
				return csv;
			}
			return "";
		}
	}
}