using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogApp
{
    public class BlogPost
    {
        public BlogPost()
        {
            Comments = new HashSet<Comments>();
            Created = DateTimeOffset.Now;
        }

        public int Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        [Required(ErrorMessage = "The Title field is required for Post.")]
        public string Title { get; set; }

        public string Slug { get; set; }

        [NotMapped]
        [RegularExpression(@"(.)+(.png|.jpg|.gif|.jpeg)$", ErrorMessage = "Only Image files allowed. (.png, .jpg, .gif, .jpeg)")]
        public HttpPostedFileBase Image { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public string PostAuthor { get; set; }
      

        public string ShortBody { get; set; }
        public string MediaURL { get; set; }
        public bool Published { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string CategoryName { get; set; }
    }
}