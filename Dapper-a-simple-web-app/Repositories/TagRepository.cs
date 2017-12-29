using Dapper_SimpleWebApp.Models;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace Dapper_SimpleWebApp
{
	public class TagRepository : BaseRepository
	{
		private IDbConnection _connection;

		public TagRepository()
		{
			_connection = new SQLiteConnection(_connectionString);
			_connection.Open();
		}

		public List<Tag> FetchAll()
		{
			var result =
				_connection.Query<Tag>(
					@"select tag.*, count(articleTag.tagId) as count from tag left join articleTag on articleTag.tagId = tag.id
						group by articleTag.tagId
						order by tag.name asc");
			if(result != null)
			{
				return result.ToList();
			}
			return null;
		}
	}
}