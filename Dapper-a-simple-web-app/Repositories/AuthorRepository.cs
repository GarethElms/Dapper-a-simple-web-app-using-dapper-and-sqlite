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
			var author = _connection.Query<Author>("select * from author where id=@id", new {Id = authorId}).SingleOrDefault();

			return author;
			/*return _database.Fetch<Article, Author, Tag, Article>(
				new ArticleRelator().Map,
				"select * from article " + 
				"join author on author.id = article.author_id " +
				"left outer join articleTag on articleTag.articleId = article.id " + 
				"left outer join tag on tag.id=articleTag.tagId " + 
				"where article.id=@0 ", articleId).Single();*/
		}

		public List<Author> FetchAll()
		{
			var authors = _connection.Query<Author>("select * from author order by name").ToList();
			return authors;
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
	}
}