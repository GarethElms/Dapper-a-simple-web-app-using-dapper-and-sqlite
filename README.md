# Simple ASP.NET Core web app using Dapper and SQLite

Here's a simple ASP.NET Core web app using [Dapper](https://github.com/StackExchange/Dapper) to access a [SQLite](https://www.sqlite.org) database containing authors, articles and tags associated with the articles.

Here's a [short video](https://www.youtube.com/watch?v=2VO-GtiH63E) showing the web app running.

I wanted to figure out how to use Dapper to fetch data in a many-to-many relationship. The tags are stored centrally in the tag table. The many-to-many relationship between tags and articles is managed through the articleTag table which links tag records to article records.

This is the entire SQLite database schema :

![SQLite database schema](http://www.garethelms.org/img/2017/12/SQLite-database-schema.jpg)

Here is how I fetch articles, their authors and the associated tags :

```c#
public List<Article> Fetch(int? tagId = null, int? articleId = null)
{
   var articles = new Dictionary<long, Article>();
   var sql = @"select * from article
         left join author on article.authorid = author.id
         left join articleTag on articleTag.articleId = article.id
         left join tag on tag.id = articleTag.tagId";
   dynamic parameters;
   sql += Fetch_ComposeWhereClause(out parameters, tagId, articleId);

   // NOTE: I had to create an <ArticleTag> type here because Dapper seems to look at each joined table in turn and tries to
   // to map it, regardless of whether I only return the columns I want. I do nothing with the articleTag data
   // here as I only need the associated tag data.
   var result = _connection.Query<Article, Author, ArticleTag, Tag, Article>(
      sql,
      (article, author, articleTag, tag) => {
         if(!articles.ContainsKey(article.Id))
         {
            article.Author = author; // Only one author, they'll all be the same one so just reference it the first time.
            if(tag != null && article.Tags == null)
            {
               article.Tags = new List<Tag>();
            }
            articles[article.Id] = article;
         }
         else
         {
            article = articles[article.Id];
         }
         if(tag != null)
         {
            article.Tags.Add(tag);
         }
         return article;
      }, (object)parameters);

   if(articles.Count > 0)
   {
      return articles.Values.ToList();
   }

   return null;
}
```

The main problem this solves is assigning one or more tag records to a single article record when article and tag data are bundled together in multiple result rows (one for each tag). I solve this by transforming the flat results into a dictionary :

```c#
var articles = new Dictionary<long, Article>();
```

This solution makes sense when you look at some example results from the above SQL command :

![SQLite database results](http://www.garethelms.org/img/2017/12/SQLite-database-results.jpg)

As you can see there is data duplication in the resultset for each tag associated with the article. This is a trade off. The alternative approach is to execute two SQL commands firstly to retrieve the articles/authors (which have a one-to-one relationship) and secondly to retrieve the tags associated with the articles in the resultset ie; two SQL commands rather than one.

Both approaches are pretty simple to do with Dapper but I chose to use the single SQL command. But this could be the wrong approach in this instance because the potentially hefty article content is duplicated for each tag. So if an article has a 5k body and has 10 tags associated with it then the data over the network will be at least 50k. Ouch!

If this was a real app I'd monitor its performance and decide which is the better option. It depends where the concerns are; with network bandwidth or with the number of concurrent database connections.

Also, if you're interested, in [ArticleRepository.cs](https://github.com/GarethElms/Dapper-a-simple-web-app/blob/master/Dapper-a-simple-web-app/Repositories/ArticleRepository.cs#L140) there is an example of using a transaction with SQLite. I use a transaction to ensure that a chain of commands is executed as a unit or not at all.

Note: I wrote a similar project for PetaPoco a few years ago for which I
wrote a [blog post](http://www.garethelms.org/2012/02/many-to-many-relationships-with-petapoco).


