using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dapper_SimpleWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dapper_SimpleWebApp.Controllers
{
	public class TagController : Controller
	{
		public IActionResult List()
		{
			var repo = new TagRepository();
			var tags = repo.FetchAll();

			return View(tags);
		}

		public IActionResult Articles(int id)
		{
			var repo = new ArticleRepository();
			var articles = repo.FetchAll(tagId:id);

			return View("~/views/article/list.cshtml", articles);
		}
	}
}
