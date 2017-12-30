using Dapper_SimpleWebApp.Models;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using System.Dynamic;
using System.Text;

namespace Dapper_SimpleWebApp
{
	public class AuthorRepository : BaseRepository, IAuthorRepository
	{
		private IDbConnection _connection;

		public AuthorRepository()
		{
			_connection = new SQLiteConnection(_connectionString);
			_connection.Open();
		}

		/// <summary>
		/// Fetch a single author including the article records not including the articles' tags.
		/// </summary>
		/// <param name="authorId"></param>
		/// <returns></returns>
		public Author RetrieveById(int authorId)
		{
			var result = Fetch(authorId:authorId);
			if(result != null)
			{
				return result[0];
			}
			return null;
		}

		/// <summary>
		/// Fetch a list of author records including the article records not including the articles' tags.
		/// </summary>
		/// <param name="authorId"></param>
		/// <returns></returns>
		public List<Author> Fetch(int? authorId = null)
		{
			var authors = new Dictionary<long, Author>();
			dynamic parameters;
			var result = _connection.Query<Author, Article, Author>(
				"select * from author left join article on article.authorid=author.id" + Fetch_ComposeWhereClause(out parameters, authorId:authorId) + " order by author.name",
				(author, article) => {
					if(!authors.ContainsKey(author.Id))
					{
						authors[author.Id] = author;
						if(article != null)
						{
							author.Articles = new List<Article>();
						}
					}
					else
					{
						author = authors[author.Id];
					}
					if(article != null)
					{
						author.Articles.Add(article);
					}
					return author;
				}, (object)parameters);

			if(authors.Count > 0)
			{
				return authors.Values.ToList();
			}
			return null;
		}

		/// <summary>
		/// The Fetch() method can return all authors or a specific author filtered by authorId.
		/// At this stage there are no further requirements for the where clause.
		/// This method is responsible for composing the where clause.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="authorId"></param>
		/// <returns></returns>
		private string Fetch_ComposeWhereClause(out dynamic parameters, int? authorId = null)
		{
			parameters = new ExpandoObject();
			var whereClauses = new List<string>();
			var whereClauseCount = 0;
			if(authorId != null)
			{
				whereClauses.Add("author.id=@authorId");
				whereClauseCount ++;
				parameters.authorId = authorId.Value;
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
		/// Inserts or updates an author record.
		/// </summary>
		/// <param name="author"></param>
		/// <returns></returns>
		public bool Save(Author author)
		{
			if(author.Id == 0)
			{
				author.Id = _connection.Insert<Author>(author);
			}
			else
			{
				_connection.Update<Author>(author);
			}
			return true;
		}

		public bool Delete(Author author)
		{
			/* TODO : Delete all author data eg; articles and articleTag associations
			 * This is hwo I did it in PetaPoco :
			 * 
			using( var scope = _database.GetTransaction())
			{
				_database.Execute( "delete from articleTag where articleTag.articleId in (select id from article where article.author_id=@0)", authorId);
				_database.Execute( "delete from article where author_id=@0", authorId);
				_database.Execute( "delete from author where id=@0", authorId);

				scope.Complete();
			}
			*/
			return _connection.Delete(author);
		}
	}
}