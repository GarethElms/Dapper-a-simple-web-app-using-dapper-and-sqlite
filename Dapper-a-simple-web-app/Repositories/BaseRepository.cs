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
	public class BaseRepository
	{
		protected readonly string _connectionString;
		public BaseRepository()
		{
			_connectionString = "Data Source=app.db";
		}
	}
}