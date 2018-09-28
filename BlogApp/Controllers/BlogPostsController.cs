using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using BlogApp;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;

namespace BlogApp.Models
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page)
        {
            int pageSize = 3; // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);
            var orderedList = db.Posts.OrderByDescending(item => item.Created).ToList();
            ViewBag.FirstItem = orderedList.FirstOrDefault();
            orderedList.RemoveAt(0);
            return View(orderedList.ToPagedList(pageNumber, pageSize));
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }


            return View(blogPost);
        }

        // GET: post/{slug}
        public ActionResult DetailsSlug(string slug)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts
                .Include(item => item.Category)
                .Include(item => item.Comments)
                .FirstOrDefault(item => item.Slug == slug);


            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View("Details", blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName");
            return View();

        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Title,Body,Published,ShortBody,CategoryId")] BlogPost blogPost, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var userDisplayName = db.Users.FirstOrDefault(item => item.UserName == User.Identity.Name).DisplayName;
                var hash = blogPost.GetHashCode();
                // next line convert file name into web friend (no white spaces and etc) and concate with "-hash" in case if file with the same name already on a sever

                if (Image != null)
                {
                    var fileName = Helpers.SlugConverter.URLFriendly(Path.GetFileNameWithoutExtension(Image.FileName)) + "-" + hash.ToString() + Path.GetExtension(Image.FileName);
                    Image.SaveAs(Path.Combine(Server.MapPath("~/uploads/img/"), fileName));
                    blogPost.MediaURL = "/uploads/img/" + fileName;
                }
                else
                {
                    blogPost.MediaURL = "/assets/img/500.png";
                }

                blogPost.Slug = Helpers.SlugConverter.URLFriendly(blogPost.Title) + "-" + hash;
                blogPost.PostAuthor = userDisplayName;
                if (blogPost.Body == null)
                {
                    blogPost.ShortBody = "Here should be a article text of the post";
                    blogPost.Body = "<strong>Here should be a article text of the post</strong>";
                }

                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName");
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", blogPost.CategoryId);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        //:TODO Add delte image from old image from server!
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Published,CategoryId,ShortBody")] BlogPost blogPost, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var dbBlogPost = db.Posts.FirstOrDefault(post => post.Id == blogPost.Id);
                var hash = blogPost.GetHashCode();

                //check if image was changed
                if (Image != null)
                {
                    //delete previous image first only if it is not a placeholder image from assets folder
                    if (!dbBlogPost.MediaURL.EndsWith("/assets/img/500.png"))
                    {
                        var strFile = Server.MapPath("~/" + dbBlogPost.MediaURL);
                        System.IO.File.Delete(strFile);
                    }
                    //upload new image
                    var fileName = Helpers.SlugConverter.URLFriendly(Path.GetFileNameWithoutExtension(Image.FileName)) + "-" + hash + Path.GetExtension(Image.FileName);
                    Image.SaveAs(Path.Combine(Server.MapPath("~/uploads/img/"), fileName));
                    dbBlogPost.MediaURL = "/uploads/img/" + fileName;
                }
                // if title changed - generate new Slug. If not keep the same
                if (dbBlogPost.Title != blogPost.Title)
                {
                    dbBlogPost.Slug = Helpers.SlugConverter.URLFriendly(blogPost.Title) + "-" + hash;
                }

                dbBlogPost.Title = blogPost.Title;

                if (blogPost.Body == null)
                {
                    dbBlogPost.ShortBody = "Here should be a article text of the post";
                    dbBlogPost.Body = "<strong>Here should be a article text of the post</strong>";
                }
                else
                {
                    dbBlogPost.ShortBody = blogPost.ShortBody;
                    dbBlogPost.Body = blogPost.Body;
                }
                dbBlogPost.CategoryId = blogPost.CategoryId;
                dbBlogPost.Published = blogPost.Published;
                dbBlogPost.Updated = DateTimeOffset.Now;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: post/edit/{slug}
        [Authorize(Roles = "Admin")]
        public ActionResult EditSlug(string slug)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(post => post.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", blogPost.CategoryId);
            return View("Edit", blogPost);
        }





        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: post/delete/{slug}
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteSlug(string slug)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(post => post.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View("Delete", blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {

            BlogPost blogPost = db.Posts.Find(id);
            if (!blogPost.MediaURL.EndsWith("/assets/img/500.png"))
            {

                var strFile = Server.MapPath("~/" + blogPost.MediaURL);
                System.IO.File.Delete(strFile);
            }
            db.Posts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // create a comment on the post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateComment(string body, string slug)
        {


            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var blogPost = db.Posts.FirstOrDefault(item => item.Slug == slug);

            if (blogPost == null)
            {
                return HttpNotFound();
            }

            if (string.IsNullOrWhiteSpace(body) || string.IsNullOrEmpty(body))
            {
                TempData["ErrorMsg"] = "Comment couldn't be empty.";
                return RedirectToAction("DetailsSlug", new { slug = blogPost.Slug });
            }

            var comment = new Comments();
            comment.Body = body;
            comment.BlogPostId = blogPost.Id;
            comment.Created = DateTimeOffset.Now;
            comment.AuthorId = User.Identity.GetUserId();

            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("DetailsSlug", new { slug = blogPost.Slug });
        }


        //serch for posts
        [HttpGet]
        public ActionResult SearchPost(int? page, string query)
        {

            int pageSize = 3; // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);
            List<BlogPost> postList;
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrEmpty(query))
            {
                postList = db.Posts.ToList();
            }
            else
            {
                postList = db.Posts.Where(item => item.Title.Contains(query) ||
                                                     item.Body.Contains(query))
                                                     .ToList()
                                                     .OrderByDescending(item => item.Created).ToList();

            }


            ViewBag.searchText = query;
            return View("SearchResults", postList.ToPagedList(pageNumber, pageSize));
        }


        //contact me page
        public ActionResult ContactMe()
        {
            return View();
        }


        //POST
        //Send email from contact page

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactMe([Bind(Include = "FromName,FromEmail,Subject,Body")]MyEmailService modelMail)
        {
            if (ModelState.IsValid)
            {
                var newMail = new MailMessage(modelMail.FromEmail, WebConfigurationManager.AppSettings["username"]);
                newMail.Subject = modelMail.Subject;
                newMail.Body = $"<h3>This is email from {modelMail.FromName}. E-mail: {modelMail.FromEmail}</h3><p>{modelMail.Body}<p/>";
                newMail.IsBodyHtml = true;

                await PersonalEmail.SendAsync(newMail);

                //var smtpClient = new SmtpClient();
                // smtpClient.Send(newMail);
                return View();
            }

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
