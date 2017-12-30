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
		private IArticleRepository _articleRepository;
		private IAuthorRepository _authorRepository;

		public ArticleController(IArticleRepository articleRepository, IAuthorRepository authorRepository)
		{
			_articleRepository = articleRepository;
			_authorRepository = authorRepository;
		}

		public IActionResult Index(int id)
		{
			var article = _articleRepository.RetrieveById(id);

			return View("Details", article);
		}

		public IActionResult Edit(int id)
		{
			var viewModel = new ArticleViewModel();
			var tagRepo = new TagRepository();

			viewModel.Article = _articleRepository.RetrieveById(id);
			if(viewModel.Article == null)
			{
				viewModel.Article = new Article();
			}
			var authors = _authorRepository.Fetch();
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
			if(HttpContext.Request.Form.ContainsKey("delete"))
			{
				if(_articleRepository.Delete(article))
				{
					TempData["Message_Success"] = "Article was deleted";
					return RedirectToAction("List", "Article");
				}
			}
			else if(_articleRepository.Save(article))
			{
				TempData["Message_Success"] = "Article was saved";
			}

			return RedirectToAction("Edit", new {id=article.Id});
		}

		public IActionResult List()
		{
			var articles = _articleRepository.Fetch();

			return View(articles);
		}
	}
}
