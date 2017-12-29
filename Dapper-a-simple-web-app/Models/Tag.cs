using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dapper_SimpleWebApp.Models
{
	[Table("tag")]
	public class Tag
	{
		public long Id { get; set; }
		public string Name { get; set; }

		[Computed]
		public int Count {get;set;}

		[Computed]
		public List<Article> Articles {get;set;}
		
		public Tag()
		{
			Id = 0;
		}
	}
}