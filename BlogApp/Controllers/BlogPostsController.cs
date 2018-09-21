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
        public ActionResult Create([Bind(Include = "Id,Title,Body,Published,ShortBody")] BlogPost blogPost, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var hash = blogPost.GetHashCode();
                // next line convert file name into web friend (no white spaces and etc) and concate with "-hash" in case if file with the same name already on a sever
                if (Image != null)
                {
                    var fileName = Helpers.SlugConverter.URLFriendly(Path.GetFileNameWithoutExtension(Image.FileName)) + "-" + hash.ToString() + Path.GetExtension(Image.FileName);
                    Image.SaveAs(Path.Combine(Server.MapPath("~/uploads/img/"), fileName));
                    blogPost.MediaURL = "/uploads/img/" + fileName;
                }
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
        //:TODO Add delte image from old image from server!
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Published")] BlogPost blogPost, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var dbBlogPost = db.Posts.FirstOrDefault(post => post.Id == blogPost.Id);
                var hash = blogPost.GetHashCode();

                //check if image was changed
                if (Image != null)
                {
                    //delete previous image first
                    var strFile = Server.MapPath("~/" + dbBlogPost.MediaURL);
                    System.IO.File.Delete(strFile);
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
                dbBlogPost.Body = blogPost.Body;

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

        // GET: post/delete/{slug}
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
        public ActionResult DeleteConfirmed(int id)
        {

            BlogPost blogPost = db.Posts.Find(id);
            var strFile = Server.MapPath("~/"+blogPost.MediaURL);
            System.IO.File.Delete(strFile);
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
