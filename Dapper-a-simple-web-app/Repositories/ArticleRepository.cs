using Dapper_SimpleWebApp.Models;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using System;

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
		}

		public List<Article> FetchAll(int? tagId = null)
		{
			var articles = new Dictionary<long, Article>();

			// NOTE: I had to create an ArticleTag record because Dapper looks at each joined table in turn and tries to
			// to map it, regardless of whether I only return the columns I want. I do nothing with the articleTag data
			// here as I only need the associated tag data itself.
			var sql = @"select * from article
					left join author on article.authorid = author.id
					left join articleTag on articleTag.articleId = article.id
					left join tag on tag.id = articleTag.tagId";
			object parameters = null;
			if(tagId != null)
			{
				sql += " where tag.id=@tagId";
				parameters = new {tagId = tagId.Value};
			}

			var result = _connection.Query<Article, Author, ArticleTag, Tag, Article>(
				sql,
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
				}, parameters);

			if(articles.Count > 0)
			{
				return articles.Values.ToList();
			}

			return null;
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
				_connection.Execute("delete from articleTag where articleId=@articleId", new {articleId=article.Id});
			}

			if(!string.IsNullOrEmpty(article.TagsCSV))
			{
				var tagsAsArray = article.TagsCSV.Split(",", StringSplitOptions.RemoveEmptyEntries);
				long tagId = 0;
				foreach(var tag in tagsAsArray)
				{
					var trimmedTag = tag.Trim();
					// Create a tag record if this tag doesn't already exist.
					var existingTag = _connection.Query<Tag>("select * from tag where name=@tag", new{tag=trimmedTag}).FirstOrDefault();
					tagId = 0;
					if(existingTag == null)
					{
						tagId = _connection.Insert<Tag>(new Tag(){Name=trimmedTag});
					}
					else
					{
						tagId = existingTag.Id;
					}
					// Now associate the tag with this article
					_connection.Insert<ArticleTag>(new ArticleTag(){ArticleId=article.Id, TagId=tagId});
				}
			}

			return true;
		}

		public bool Delete(Article article)
		{
			return _connection.Delete(article);
		}
	}
}