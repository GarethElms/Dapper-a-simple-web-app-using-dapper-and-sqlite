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
			var tagRepo = new TagRepository();

			viewModel.Article = articleRepo.RetrieveById(id);
			if(viewModel.Article == null)
			{
				viewModel.Article = new Article();
			}
			var authors = authorRepo.Fetch();
			viewModel.Authors = new List<SelectListItem>();
			foreach(var author in authors)
			{
				viewModel.Authors.Add(new SelectListItem(){Value=author.Id.ToString(), Text=author.Name});
			}
			viewModel.AllTags = tagRepo.FetchAll();

			return View(viewModel);
		}

		[HttpPost]
		public IActionResult Edit(Article article)
		{
			var repo = new ArticleRepository();
			if(HttpContext.Request.Form.ContainsKey("delete"))
			{
				if(repo.Delete(article))
				{
					TempData["Message_Success"] = "Article was deleted";
					return RedirectToAction("List", "Article");
				}
			}
			else if(repo.Save(article))
			{
				TempData["Message_Success"] = "Article was saved";
			}

			return RedirectToAction("Edit", new {id=article.Id});
		}

		public IActionResult List()
		{
			var repo = new ArticleRepository();
			var articles = repo.Fetch();

			return View(articles);
		}
	}
}
