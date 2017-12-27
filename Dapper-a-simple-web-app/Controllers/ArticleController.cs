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
	public class ArticleController : Controller
	{
		public IActionResult Index(int id)
		{
			var repo = new ArticleRepository();
			var article = repo.RetrieveById(id);

			return View("Details", article);
		}

		public IActionResult Edit(int id)
		{
			var viewModel = new ArticleViewModel();
			var articleRepo = new ArticleRepository();
			var authorRepo = new AuthorRepository();

			viewModel.Article = articleRepo.RetrieveById(id);
			var authors = authorRepo.FetchAll();
			viewModel.Authors = new List<SelectListItem>();
			foreach(var author in authors)
			{
				viewModel.Authors.Add(new SelectListItem(){Value=author.Id.ToString(), Text=author.Name});
			}

			return View(viewModel);
		}

		[HttpPost]
		public IActionResult Edit(Article article)
		{
			var repo = new ArticleRepository();
			if(repo.Save(article))
			{
				TempData["Message_Success"] = "Article was saved";
			}

			return RedirectToAction("Edit");
		}

		public IActionResult List()
		{
			var repo = new ArticleRepository();
			var articles = repo.FetchAll();

			return View(articles);
		}
	}
}
