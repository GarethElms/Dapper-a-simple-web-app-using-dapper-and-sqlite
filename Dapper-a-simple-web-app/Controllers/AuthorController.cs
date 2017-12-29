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
	public class AuthorController : Controller
	{

		public IActionResult Details(int id)
		{
			var repo = new AuthorRepository();
			var author = repo.RetrieveById(id);

			return View("Details", author);
		}

		public IActionResult Edit(int id)
		{
			var authorRepo = new AuthorRepository();
			var author = authorRepo.RetrieveById(id);
			if(author == null)
			{
				author = new Author();
			}
			return View(author);
		}

		[HttpPost]
		public IActionResult Edit(Author author)
		{
			var repo = new AuthorRepository();
			if(HttpContext.Request.Form.ContainsKey("delete"))
			{
				if(repo.Delete(author))
				{
					TempData["Message_Success"] = "Author was deleted";
					return RedirectToAction("List", "Author");
				}
			}
			else if(repo.Save(author))
			{
				TempData["Message_Success"] = "Author was saved";
			}

			return RedirectToAction("Edit", new {id=author.Id});
		}

		public IActionResult List()
		{
			var repo = new AuthorRepository();
			var articles = repo.Fetch();

			return View(articles);
		}
	}
}
