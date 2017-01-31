using System;
// using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace forum.Models
{
    public class BaseEntity{}
    public class RegistrationUser : BaseEntity
    {
        [Required(ErrorMessage = "First Name is required")]
        [MinLength(2, ErrorMessage = "First Name must be at least 2 characters")]
        public string firstName {get; set;} 

        [Required(ErrorMessage = "Last Name is required")]
        [MinLength(2, ErrorMessage = "Last Name must be at least 2 characters")]
        public string lastName {get; set;} 

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        public string email {get; set;}

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string password {get; set;}

        [Compare("password", ErrorMessage = "Password Confirmation must match Password")]
        [DataType(DataType.Password)]
        public string password_confirmation {get; set;}    

        public User transferToUser(){
            User user = new User();
            user.FirstName = this.firstName;
            user.LastName = this.lastName;
            user.Email = this.email;
            user.UserName = this.email;
            return user;
        }    
    }
}