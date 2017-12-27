using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dapper_SimpleWebApp.Models
{
	[Table("author")]
	public class Author
	{
		public long Id { get; set; }
		public string Name { get; set; }
		
		public Author()
		{
			Id = 0;
		}
	}
}