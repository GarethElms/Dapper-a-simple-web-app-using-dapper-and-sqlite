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
		private ITagRepository _tagRepository;
		private IArticleRepository _articleRepository;

		public TagController(ITagRepository tagRepository, IArticleRepository articleRepository)
		{
			_tagRepository = tagRepository;
			_articleRepository = articleRepository;
		}

		public IActionResult List()
		{
			var tags = _tagRepository.FetchAll();

			return View(tags);
		}

		public IActionResult Articles(int id)
		{
			var articles = _articleRepository.Fetch(tagId:id);

			return View("~/views/article/list.cshtml", articles);
		}
	}
}
