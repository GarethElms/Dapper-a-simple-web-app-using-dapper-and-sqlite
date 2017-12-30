using System.Collections.Generic;
using Dapper_SimpleWebApp.Models;

namespace Dapper_SimpleWebApp
{
	public interface IArticleRepository
	{
		bool Delete(Article article);
		List<Article> Fetch(int? tagId = null, int? articleId = null);
		Article RetrieveById(int articleId);
		bool Save(Article article);
	}
}