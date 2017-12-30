using System.Collections.Generic;
using Dapper_SimpleWebApp.Models;

namespace Dapper_SimpleWebApp
{
	public interface ITagRepository
	{
		List<Tag> FetchAll();
	}
}