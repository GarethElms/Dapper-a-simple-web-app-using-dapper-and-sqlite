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
			//var result = _connection.Query<Author>("select * from author where id=@id", new {Id = authorId}).SingleOrDefault();
			var authors = new Dictionary<long, Author>();

			var result = _connection.Query<Author, Article, Author>(
				"select * from author  join article on article.authorid=author.id where author.id=@Id",
				(author, article) => {
					if(!authors.ContainsKey(author.Id))
					{
						authors[author.Id] = author;
						author.Articles = new List<Article>();
					}
					else
					{
						author = authors[author.Id];
					}
					author.Articles.Add(article); return author;
				},
				new {Id = authorId});

			if(authors.Count == 1)
			{
				return authors[authorId];
			}
			return null;

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