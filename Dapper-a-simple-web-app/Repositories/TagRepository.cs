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
		private IDbConnection _connection { get { return new SQLiteConnection(_connectionString); } }

		public TagRepository()
		{
			_connection.Open();
		}

		public List<Tag> FetchAll()
		{
			var result = _connection.Query<Tag>("select * from tag");
			if(result != null)
			{
				return result.ToList();
			}
			return null;
		}
	}
}