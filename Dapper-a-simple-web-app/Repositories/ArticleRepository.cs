using Dapper_SimpleWebApp.Models;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

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

	/*public interface IArticleRepository
	{
		Article RetrieveById(int articleId);
		List<Article> FetchAll();
	}*/

	public class ArticleRepository : BaseRepository // , IArticleRepository
	{
		private IDbConnection _connection { get { return new SQLiteConnection(_connectionString); } }

		public ArticleRepository()
		{
			_connection.Open();
		}
		public Article RetrieveById(int articleId)
		{
			var articles = new Dictionary<long, Article>();

			// NOTE: I had to create an ArticleTag record because Dapper looks at each joined table in turn and tries to
			// to map it, regardless of whether I only return the columns I want. I do nothing with the articleTag data
			// as I only need the associated tag data itself.
			var result = _connection.Query<Article, Author, ArticleTag, Tag, Article>(
				@"select * from article
					left join author on article.authorid = author.id
					left join articleTag on articleTag.articleId = article.id
					left join tag on tag.id = articleTag.tagId
					where article.id = @Id",
				(article, author, articleTag, tag) => {
					if(!articles.ContainsKey(article.Id))
					{
						article.Author = author; // Only one author, they'll all be the same one so just reference it the first time.
						if(tag != null && article.Tags == null)
						{
							article.Tags = new List<Tag>();
						}
						articles[article.Id] = article;
					}
					else
					{
						article = articles[article.Id];
					}
					if(tag != null)
					{
						article.Tags.Add(tag);
					}
					return article;
				},
				new {Id = articleId});
			if(articles.Count > 0)
			{
				return articles[articleId];
			}
			return null;

			/*var result = _connection.Query<Article, Author, Article>("select * from article left join author on article.authorid = author.id where article.id=@id",
				(article, author) =>
				{
					article.Author = author;
					return article;
				},
				new {Id = articleId}).SingleOrDefault();

			return result;
			*/
			/*return _database.Fetch<Article, Author, Tag, Article>(
				new ArticleRelator().Map,
				"select * from article " + 
				"join author on author.id = article.author_id " +
				"left outer join articleTag on articleTag.articleId = article.id " + 
				"left outer join tag on tag.id=articleTag.tagId " + 
				"where article.id=@0 ", articleId).Single();*/
		}

		public List<Article> FetchAll()
		{
			//var articles = _connection.Query<Article>("select * from article order by date desc").ToList();

			// TODO: Retrieve tags

			var articles = new Dictionary<long, Article>();

			// NOTE: I had to create an ArticleTag record because Dapper looks at each joined table in turn and tries to
			// to map it, regardless of whether I only return the columns I want. I do nothing with the articleTag data
			// as I only need the associated tag data itself.
			var result = _connection.Query<Article, Author, ArticleTag, Tag, Article>(
				@"select * from article
					left join author on article.authorid = author.id
					left join articleTag on articleTag.articleId = article.id
					left join tag on tag.id = articleTag.tagId",
				(article, author, articleTag, tag) => {
					if(!articles.ContainsKey(article.Id))
					{
						article.Author = author; // Only one author, they'll all be the same one so just reference it the first time.
						if(tag != null && article.Tags == null)
						{
							article.Tags = new List<Tag>();
						}
						articles[article.Id] = article;
					}
					else
					{
						article = articles[article.Id];
					}
					if(tag != null)
					{
						article.Tags.Add(tag);
					}
					return article;
				});
			if(articles.Count > 0)
			{
				return articles.Values.ToList();
			}

			return null;

			/*var result = _connection.Query<Article, Author, Article>("select * from article left join author on article.authorid = author.id",
				(article, author) =>
				{
					article.Author = author;
					return article;
				});

			return result.ToList();*/
		}

		public bool Save(Article article)
		{
			if(article.Id == 0)
			{
				article.Id = _connection.Insert<Article>(article);
			}
			else
			{
				_connection.Update<Article>(article);
			}
			return true;
		}

		public bool Delete(Article article)
		{
			return _connection.Delete(article);
		}
	}
}