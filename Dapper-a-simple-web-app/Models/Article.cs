using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dapper_SimpleWebApp.Models
{
	[Table("article")]
	public class Article
	{
		public long Id { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }

		//[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime Date { get; set; }

		//[ResultColumn]
		//public Author Author { get; set; }

		[DisplayName("Author")]
		public int AuthorId { get; set; }

		//[ResultColumn]
		//public List<Tag> Tags { get; set; }

		public Article()
		{
			Date = DateTime.Now;
			Id = 0;
			//Tags = new List<Tag>();
		}
	}
}