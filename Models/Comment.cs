using System;
using System.ComponentModel.DataAnnotations;
// using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace forum.Models
{
    public class Comment : BaseEntity
    {
        public int Id {get; set;}

        [Required(ErrorMessage = "Please leave a non-empty comment")]
        public string Content {get; set;}

        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}

        public string CommentorId {get; set;}
        public User Commentor {get; set;}

        [Required]
        public int TopicId {get; set;}
        public Topic Topic {get; set;}

        public Comment(){
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}