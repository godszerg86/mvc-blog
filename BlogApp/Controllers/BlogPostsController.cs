using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogApp;

namespace BlogApp.Models
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index()
        {
            return View(db.Posts.ToList());
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
            BlogPost blogPost = db.Posts.FirstOrDefault(item => item.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View("Details", blogPost);
        }

        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,Published,ShortBody")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var hash = blogPost.GetHashCode();
                // next line convert file name into web friend (no white spaces and etc) and concate with "-hash" in case if file with the same name already on a sever
                var fileName = Helpers.SlugConverter.URLFriendly(Path.GetFileNameWithoutExtension(image.FileName)) + "-" + hash.ToString() + Path.GetExtension(image.FileName);
                image.SaveAs(Path.Combine(Server.MapPath("~/uploads/img/"), fileName));
                blogPost.MediaURL = "/uploads/img/" + fileName;
                blogPost.Slug = Helpers.SlugConverter.URLFriendly(blogPost.Title) + "-" + hash;
                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
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
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,MediaURL,Published")] BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                var dbBlogPost = db.Posts.FirstOrDefault(post => post.Id == blogPost.Id);
                // if title changed - generate new Slug. If not keep the same
                if (dbBlogPost.Title != blogPost.Title)
                {
                    var hash = blogPost.GetHashCode();
                    dbBlogPost.Slug = Helpers.SlugConverter.URLFriendly(blogPost.Title) + "-" + hash;
                }

                dbBlogPost.Title = blogPost.Title;
                dbBlogPost.Body = blogPost.Body;
                dbBlogPost.MediaURL = blogPost.MediaURL;
                dbBlogPost.Published = blogPost.Published;
                dbBlogPost.Updated = DateTimeOffset.Now;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: post/edit/{slug}
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
            return View("Edit", blogPost);
        }





        // GET: BlogPosts/Delete/5
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

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
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
