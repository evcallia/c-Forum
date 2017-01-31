using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace forum.Models
{
    public class UpdateUser : BaseEntity
    {
        public UpdateUser(){
            this.ModeratedCategories = new List<User_Cateogry>();
        }
        public string Id {get; set;}

        [Required(ErrorMessage = "First Name is required")]
        [MinLength(2, ErrorMessage = "First Name must be at least 2 characters")]
        public string FirstName {get; set;} 

        [Required(ErrorMessage = "Last Name is required")]
        [MinLength(2, ErrorMessage = "Last Name must be at least 2 characters")]
        public string LastName {get; set;} 

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        public string Email {get; set;}

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string Password {get; set;}

        [Compare("Password", ErrorMessage = "Password Confirmation must match Password")]
        [DataType(DataType.Password)]
        public string PasswordConfirmation {get; set;}    
        [DataType(DataType.Password)]
        public string OldPassword {get; set;}

        public List<User_Cateogry> ModeratedCategories {get; set;}


        public void transferToUser(User user){
            user.FirstName = this.FirstName;
            user.LastName = this.LastName;
            user.Email = this.Email;
            user.UserName = this.Email;
        }  

        public void TransferFromUser(User user){
            this.Id = user.Id;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Email = user.Email;
            this.ModeratedCategories = user.ModeratedCategories;
        }  
    }
}