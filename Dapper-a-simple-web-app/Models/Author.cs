using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dapper_SimpleWebApp.Models
{
	[Table("author")]
	public class Author
	{
		public long Id { get; set; }
		public string Name { get; set; }

		[Computed]
		public List<Article> Articles {get;set;}
		
		public Author()
		{
			Id = 0;
		}
	}
}