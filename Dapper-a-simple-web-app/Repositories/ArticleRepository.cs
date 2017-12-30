using Dapper_SimpleWebApp.Models;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using System;
using System.Dynamic;
using System.Text;

namespace Dapper_SimpleWebApp
{
	public class ArticleRepository : BaseRepository, IArticleRepository
	{
		private IDbConnection _connection;

		public ArticleRepository()
		{
			_connection = new SQLiteConnection(_connectionString);
			_connection.Open();
		}
		
		/// <summary>
		/// Fetch a single article including the author and all tags associated with this article.
		/// </summary>
		/// <param name="articleId"></param>
		/// <returns></returns>
		public Article RetrieveById(int articleId)
		{
			var result = Fetch(articleId:articleId);
			if(result != null)
			{
				return result[0];
			}
			return null;
		}

		/// <summary>
		/// Fetch a list of article records including the author records and all tags associated with the articles.
		/// </summary>
		/// <param name="tagId"></param>
		/// <param name="articleId"></param>
		/// <returns></returns>
		public List<Article> Fetch(int? tagId = null, int? articleId = null)
		{
			var articles = new Dictionary<long, Article>();
			var sql = @"select * from article
					left join author on article.authorid = author.id
					left join articleTag on articleTag.articleId = article.id
					left join tag on tag.id = articleTag.tagId";
			dynamic parameters;
			sql += Fetch_ComposeWhereClause(out parameters, tagId, articleId);

			// NOTE: I had to create an <ArticleTag> type here because Dapper seems to look at each joined table in turn and tries to
			// to map it, regardless of whether I only return the columns I want. I do nothing with the articleTag data
			// here as I only need the associated tag data.
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
				}, (object)parameters);

			if(articles.Count > 0)
			{
				return articles.Values.ToList();
			}

			return null;
		}

		/// <summary>
		/// The Fetch() method can return all articles or articles filtered by article id or tag.
		/// At this stage there are no further requirements for the where clause.
		/// This method is responsible for composing the where clause.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="tagId"></param>
		/// <param name="articleId"></param>
		/// <returns></returns>
		private string Fetch_ComposeWhereClause(out dynamic parameters, int? tagId = null, int? articleId = null)
		{
			parameters = new ExpandoObject();
			var whereClauses = new List<string>();
			var whereClauseCount = 0;
			if(articleId != null)
			{
				whereClauses.Add("article.id=@articleId");
				whereClauseCount ++;
				parameters.articleId = articleId.Value;
			}
			else if(tagId != null)
			{
				whereClauses.Add("tag.id=@tagId");
				whereClauseCount ++;
				parameters.tagId = tagId.Value;
			}
			StringBuilder whereClause = new StringBuilder();
			for(int i=0, count=whereClauses.Count; i<count; i++)
			{
				if(i==0)
				{
					whereClause.Append(" where ");
				}
				else // Not necessary at the moment, but maybe will change if more where clauses are added in the future.
				{
					whereClause.Append(" and ");
				}
				whereClause.Append(whereClauses[i]);
			}
			if(whereClauseCount == 0)
			{
				parameters = null;
			}
			return whereClause.ToString();
		}

		/// <summary>
		/// Inserts or updates an article record and also tears down and rebuilds the associated tags. Any new tags are inserted.
		/// </summary>
		/// <param name="article"></param>
		/// <returns></returns>
		public bool Save(Article article)
		{
			// Use a transaction here because there are multiple operations that need to all happen successfully or not at all :
			//
			// 1) article record added/updated
			// 2) Existing articleTag records deleted
			// 3) Any new tags are added to the tag tables
			// 4) articleTag records added
			//
			using (var transaction = (SQLiteTransaction)_connection.BeginTransaction())
			{
				if(article.Id == 0)
				{
					article.Id = _connection.Insert<Article>(article, transaction);
				}
				else
				{
					_connection.Update<Article>(article, transaction);
					_connection.Execute("delete from articleTag where articleId=@articleId", new {articleId=article.Id}, transaction);
				}

				if(!string.IsNullOrEmpty(article.TagsCSV))
				{
					var tagsAsArray = article.TagsCSV.Split(",", StringSplitOptions.RemoveEmptyEntries);
					long tagId = 0;
					foreach(var tag in tagsAsArray)
					{
						var trimmedTag = tag.Trim();
						// Create a tag record if this tag doesn't already exist.
						var existingTag = _connection.Query<Tag>("select * from tag where name=@tag", new{tag=trimmedTag}, transaction).FirstOrDefault();
						tagId = 0;
						if(existingTag == null)
						{
							tagId = _connection.Insert<Tag>(new Tag(){Name=trimmedTag}, transaction);
						}
						else
						{
							tagId = existingTag.Id;
						}
						// Now associate the tag with this article
						_connection.Insert<ArticleTag>(new ArticleTag(){ArticleId=article.Id, TagId=tagId}, transaction);
					}
				}
				transaction.Commit();
			}

			return true;
		}

		/// <summary>
		/// Just deletes an article
		/// </summary>
		/// <param name="article"></param>
		/// <returns></returns>
		public bool Delete(Article article)
		{
			return _connection.Delete(article);
		}
	}
}