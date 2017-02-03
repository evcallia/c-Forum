using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace forum.Models
{
    public class Topic : BaseEntity
    {
        public int Id {get; set;}

        [Required(ErrorMessage = "Please fill out a topic name")]
        public string Name {get; set;}

        [Required(ErrorMessage = "Please enter a description for the topic")]
        public string Description {get; set;}

        public int Views {get; set;}

        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}



        public string CreatorId {get; set;}
        public User Creator {get; set;}

        public int CategoryId {get; set;}
        public Category Category {get; set;}

        public List<Comment> Comments {get; set;}

        public Topic(){
            Comments = new List<Comment>();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}