using System;
using System.ComponentModel.DataAnnotations.Schema;
// using System.ComponentModel.DataAnnotations;
// using System.Collections.Generic;

namespace forum.Models
{
    public class User_Cateogry : BaseEntity
    {
        public int Id {get; set;}
        
        public string ModeratorId {get; set;}
        public User Moderator {get; set;}

        public int CategoryId {get; set;}
        public Category Category {get; set;}

        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}

        public User_Cateogry(){
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}