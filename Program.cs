using System;
using NLog.Web;
using System.IO;
using System.Linq;

namespace BlogsConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();

        static void Main(string[] args)
        {
            string choice;

            do
            {
                logger.Info("Program started");

                Console.WriteLine("Welcome to the blogging application");
                Console.WriteLine("Choose an Option");
                Console.WriteLine("1) Display all Blogs");
                Console.WriteLine("2) Add Blog");
                Console.WriteLine("3) Create Post");
                Console.WriteLine("4) Display Posts");
                Console.WriteLine("Press enter to quit");

                choice = Console.ReadLine();
                var db = new BloggingContext();

                if (choice == "1")
                {
                    logger.Info("User choice: 1 - Display all Blogs");
                    try
                    {
                        var query = db.Blogs.OrderBy(b => b.Name);

                        Console.WriteLine("All blogs in the database:");
                        Console.WriteLine($"{query.Count()} blogs returned");
                        if (query.Count() != 0)
                        {
                            foreach (var item in query)
                            {
                                Console.WriteLine(item.Name);
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No blogs exist");
                            logger.Info("No blogs exist");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }

                }
                else if (choice == "2")
                {
                    logger.Info("User choice: 2 - Add Blogs");
                    try
                    {
                        
                        Console.Write("Enter a name for a new Blog: ");
                        var name = Console.ReadLine();

                        if (name != "" && name != null)
                        {

                            var blog = new Blog { Name = name };

                            db.AddBlog(blog);
                            logger.Info("Blog added - {name}", name);
                        }
                        else
                        {
                            logger.Info("Invalid name entered");
                            Console.WriteLine("Enter a valid blog name");
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }
                }
                else if (choice == "3")
                {
                    logger.Info("User choice: 3 - Create Post");
                    
                    try
                    {
                        Console.WriteLine("Select the Blog for your Post");

                        
                        var query = db.Blogs.OrderBy(b => b.BlogId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($@"{item.BlogId}) {item.Name}");
                        }
                        Post post = new Post();
                        string blogID = Console.ReadLine();
                        char firstCharacter = blogID[0];
                        bool isNumber = Char.IsDigit(firstCharacter);

                        if (!isNumber)
                        {
                            Console.WriteLine("Enter a valid number");
                            logger.Info("Entry was not an integer");
                        }
                        else
                        {
                            int postBlogID = int.Parse(blogID);
                            post.BlogId = postBlogID;
                            var blogList = db.Blogs.Select(b => b.BlogId);
                            if (blogList.Contains<int>(postBlogID))
                            {
                                Console.WriteLine("Enter Post title");

                                string title = Console.ReadLine();
                                if (title != "" && title != null)
                                {
                                    post.Title = title;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid title entered");
                                    logger.Info("Invalid title entry");
                                }

                                Console.WriteLine("Enter Post content");
                                string content = Console.ReadLine();

                                if (content != "" && content != null)
                                {
                                    post.Content = content;
                                }
                                else
                                {
                                    Console.WriteLine("Post must contain content");
                                    logger.Info("No post contents entered");
                                }
                                db.AddPost(post);
                                logger.Info($"Post added - {post.Title}");
                            }
                            else
                            {
                                Console.WriteLine("That blog doesn't exist");
                                logger.Info("Invalide blog selection");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        logger.Error(ex.StackTrace);
                    }
                }
                else if (choice == "4")
                {
                    logger.Info("User choice: 4 - Display Posts");
                    Console.WriteLine("Select the Blog to see Posts");
                    var query = db.Blogs.OrderBy(b => b.BlogId);
                    foreach (var item in query)
                    {
                        Console.WriteLine($@"{item.BlogId}) {item.Name}");
                    }

                    int blogChoice = int.Parse(Console.ReadLine());


                    var postDisplay = db.Posts.Where(p => p.BlogId.Equals(blogChoice));
                    var numberOfPosts = postDisplay.Count();
                    var blogName = db.Blogs.Where(b => b.BlogId.Equals(blogChoice)).Select(b => b.Name).FirstOrDefault();
                    Console.WriteLine($"All posts in the {blogName} blog:");
                    Console.WriteLine($"Number of Posts: {numberOfPosts}");
                    if (numberOfPosts != 0)
                    {
                        foreach (var item in postDisplay)
                        {
                            Console.WriteLine($"Blog: {blogName}\nPost title: {item.Title}\nPost Contents: {item.Content}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("There are no posts for this blog");
                        logger.Info("No posts exist");
                    }
                }
            }
            while (choice == "1" || choice == "2" || choice == "3" || choice == "4");
            logger.Info("Program ended");
        }
    }
}