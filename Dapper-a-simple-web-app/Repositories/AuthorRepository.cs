using Dapper_SimpleWebApp.Models;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace Dapper_SimpleWebApp
{
	public class AuthorRepository : BaseRepository
	{
		private IDbConnection _connection { get { return new SQLiteConnection(_connectionString); } }

		public AuthorRepository()
		{
			_connection.Open();
		}

		public Author RetrieveById(int authorId)
		{
			var authors = new Dictionary<long, Author>();

			var result = _connection.Query<Author, Article, Author>(
				"select * from author left join article on article.authorid=author.id where author.id=@Id",
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
				},
				new {Id = authorId});

			if(authors.Count == 1)
			{
				return authors[authorId];
			}
			return null;
		}

		public List<Author> FetchAll()
		{
			//var authors = _connection.Query<Author>("select * from author order by name").ToList();

			var authors = new Dictionary<long, Author>();

			var result = _connection.Query<Author, Article, Author>(
				"select * from author left join article on article.authorid=author.id order by author.name",
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
				});

			if(authors.Count > 0)
			{
				return authors.Values.ToList();
			}
			return null;
		}

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
			/*
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