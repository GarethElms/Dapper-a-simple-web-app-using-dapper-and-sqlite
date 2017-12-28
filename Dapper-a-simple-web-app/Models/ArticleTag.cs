using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dapper_SimpleWebApp.Models
{
	[Table("articleTag")]
	public class ArticleTag
	{
		public long Id {get;set;}
		public long ArticleId { get; set; }
		public long TagId { get; set; }
		
		public ArticleTag()
		{
			Id = 0;
		}
	}
}