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
		private IAuthorRepository _authorRepository;

		public AuthorController(IAuthorRepository authorRepository)
		{
			_authorRepository = authorRepository;
		}

		public IActionResult Details(int id)
		{
			var author = _authorRepository.RetrieveById(id);

			return View("Details", author);
		}

		public IActionResult Edit(int id)
		{
			var author = _authorRepository.RetrieveById(id);
			if(author == null)
			{
				author = new Author();
			}
			return View(author);
		}

		[HttpPost]
		public IActionResult Edit(Author author)
		{
			if(HttpContext.Request.Form.ContainsKey("delete"))
			{
				if(_authorRepository.Delete(author))
				{
					TempData["Message_Success"] = "Author was deleted";
					return RedirectToAction("List", "Author");
				}
			}
			else if(_authorRepository.Save(author))
			{
				TempData["Message_Success"] = "Author was saved";
			}

			return RedirectToAction("Edit", new {id=author.Id});
		}

		public IActionResult List()
		{
			var articles = _authorRepository.Fetch();

			return View(articles);
		}
	}
}
