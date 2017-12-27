using Dapper_SimpleWebApp.Models;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Linq;

namespace Dapper_SimpleWebApp
{
	public class BaseRepository
	{
		protected readonly string _connectionString;
		public BaseRepository()
		{
			_connectionString = "Data Source=app.db";
		}
	}

	public interface IArticleRepository
	{
		Article RetrieveById(int articleId);
	}

	public class ArticleRepository : BaseRepository, IArticleRepository
	{
		private IDbConnection _connection { get { return new SQLiteConnection(_connectionString); } }

		public ArticleRepository()
		{
			_connection.Open();
		}
		public Article RetrieveById(int articleId)
		{
			var article = _connection.Query<Article>("select * from article where id=@id", new {Id = articleId}).SingleOrDefault();

			return article;
			/*return _database.Fetch<Article, Author, Tag, Article>(
				new ArticleRelator().Map,
				"select * from article " + 
				"join author on author.id = article.author_id " +
				"left outer join articleTag on articleTag.articleId = article.id " + 
				"left outer join tag on tag.id=articleTag.tagId " + 
				"where article.id=@0 ", articleId).Single();*/
		}
	}
}