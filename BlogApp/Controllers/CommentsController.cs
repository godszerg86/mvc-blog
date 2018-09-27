using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogApp;

namespace BlogApp.Models
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Author).Include(c => c.BlogPost);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
            return View(comments);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.BlogPostId = new SelectList(db.Posts, "Id", "Title");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PostId,AuthorId,BlogPostId,Body,Created,Updated,UpdateReason")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comments.AuthorId);
            ViewBag.BlogPostId = new SelectList(db.Posts, "Id", "Title", comments.BlogPostId);
            return View(comments);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
         
            return View(comments);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Body,UpdateReason")] Comments comments)
        {
            if (ModelState.IsValid)
            {

                var dbComment = db.Comments.FirstOrDefault(item => item.Id == comments.Id);
                dbComment.Body = comments.Body;
                dbComment.Updated = DateTimeOffset.Now;
                dbComment.UpdateReason = comments.UpdateReason;

                db.SaveChanges();
                return RedirectToAction("DetailsSlug", "BlogPosts", new { slug = dbComment.BlogPost.Slug });
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comments.AuthorId);
            ViewBag.BlogPostId = new SelectList(db.Posts, "Id", "Title", comments.BlogPostId);
            return View(comments);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
            return View(comments);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comments comments = db.Comments.Find(id);
            var slug = comments.BlogPost.Slug;
            db.Comments.Remove(comments);
            db.SaveChanges();
            return RedirectToAction("DetailsSlug", "BlogPosts",new { slug });
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
