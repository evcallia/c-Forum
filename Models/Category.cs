using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace forum.Models
{
    public class Category : BaseEntity
    {
        public int Id {get; set;}

        [Required(ErrorMessage = "A name is required")]
        public string Name {get; set;}

        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}


        public List<User_Cateogry> Moderators {get; set;}

        public List<Topic> Topics {get; set;}


        public Category(){
            Moderators = new List<User_Cateogry>();
            Topics = new List<Topic>();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}