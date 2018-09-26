using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogApp
{
    public class Comments
    {
        
        
        public int Id { get; set; }


        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public int BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }

        [Required(ErrorMessage = "The Comment field is required for comment.")]
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string UpdateReason { get; set; }

    }
}