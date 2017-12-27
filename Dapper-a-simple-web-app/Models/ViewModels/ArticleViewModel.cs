using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Dapper_SimpleWebApp.Models
{
    public class ArticleViewModel
    {
        public Article Article { get; set; }
        public List<SelectListItem> Authors {get;set;}
    }
}