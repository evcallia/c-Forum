using System;
// using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

namespace forum.Models
{
    public class User : IdentityUser
    {
        public string FirstName {get; set;}
        public string LastName {get; set;} 

        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}



        public List<User_Cateogry> ModeratedCategories {get; set;}


        public User(){
            ModeratedCategories = new List<User_Cateogry>();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        internal User Include(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}