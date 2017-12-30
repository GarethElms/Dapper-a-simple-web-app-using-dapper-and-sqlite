using System.Collections.Generic;
using Dapper_SimpleWebApp.Models;

namespace Dapper_SimpleWebApp
{
	public interface IAuthorRepository
	{
		bool Delete(Author author);
		List<Author> Fetch(int? authorId = null);
		Author RetrieveById(int authorId);
		bool Save(Author author);
	}
}