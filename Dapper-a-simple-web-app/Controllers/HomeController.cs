using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dapper_SimpleWebApp.Models;

namespace Dapper_SimpleWebApp.Controllers
{
	public class HomeController:Controller
	{
		public IActionResult Index()
		{
			var test = new ArticleRepository();
			var article = test.RetrieveById(1);

			return View();
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
