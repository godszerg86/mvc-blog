using System.Collections.Generic;

namespace BlogApp
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<BlogPost> BlogPosts { get; set; }
    }
}