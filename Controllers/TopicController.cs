using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

using forum.Models;

namespace forum.Controllers
{
    public class TopicController : Controller
    {
        private Context _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public TopicController(Context context, UserManager<User> userManager, SignInManager<User> signInManager){
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]        
        [HttpPost]
        [Route("topics")]
        public async Task<IActionResult> CreateTopic(Topic newTopic)
        {
            if(ModelState.IsValid){
                User user = await GetCurrentUserAsync();
                newTopic.CreatorId = user.Id;
                _context.Topics.Add(newTopic);
                _context.SaveChanges();
            }else{
                TempData.Put("NewTopic", newTopic);
                List<string> errors = new List<string>();
                foreach(var error in ModelState.Values){
                    if(error.Errors.Count > 0){
                            errors.Add(error.Errors[0].ErrorMessage);
                    }
                }
                TempData["Errors"] = errors;
            }
            return RedirectToAction("ShowCategory", "Category", new {id = newTopic.CategoryId}); 
        }     

        [Authorize]        
        [HttpGet]
        [Route("topics/{id}")]
        public async Task<IActionResult> ShowTopic(int id)
        {
            Comment newComment = TempData.Get<Comment>("NewComment");
            if(newComment == null){
                newComment = new Comment();
                newComment.TopicId = id;
            }
            ViewBag.NewComment = newComment;
            if(TempData["Errors"] != null){
                ViewBag.Errors = TempData["Errors"];
            }
            Topic topic = _context.Topics.Where(t => t.Id == id).Include(t => t.Creator).Include(t => t.Comments).ThenInclude(c => c.Commentor).SingleOrDefault();
            User user = await GetCurrentUserAsync();
            ViewBag.UserId = user.Id;
            ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
            User_Cateogry uc = _context.User_Cateogries.SingleOrDefault(c => c.CategoryId == topic.CategoryId && c.ModeratorId == user.Id);
            if(uc != null){
                ViewBag.IsModerator = true;
            }else{
                ViewBag.IsModerator = false;
            }
            return View("ShowTopic", topic);
        }  

        [Authorize]        
        [HttpDelete]
        [Route("topics/{topicId}")]
        public async Task<IActionResult> DeleteTopic(int topicId)
        {
            User user = await GetCurrentUserAsync();
            user.ModeratedCategories = _context.User_Cateogries.Where(uc => uc.ModeratorId == user.Id).ToList();
            string role = await GetCurrentUserAuthorizationLevelAsync();
            Topic topic = _context.Topics.Where(t => t.Id == topicId).SingleOrDefault();
            if(role == "admin" || user.ModeratedCategories.Any(c => c.CategoryId == topic.CategoryId)){
                _context.Topics.Remove(topic);
                _context.SaveChanges();
                return new ObjectResult(new {success = true, id = topic.CategoryId});
            }
            return new ObjectResult(new {success = false});
        }     

        [Authorize]
        [HttpPost]
        [Route("comments")]
        public async Task<IActionResult> CreateComment(Comment newComment){
            if(ModelState.IsValid){
                User user = await GetCurrentUserAsync();
                newComment.CommentorId = user.Id;
                _context.Add(newComment);
                _context.SaveChanges();
            }else{
                TempData.Put("NewComment", newComment);
                List<string> errors = new List<string>();
                foreach(var error in ModelState.Values){
                    if(error.Errors.Count > 0){
                            errors.Add(error.Errors[0].ErrorMessage);
                    }
                }
                TempData["Errors"] = errors;
            }
            return RedirectToAction("ShowTopic", new {id = newComment.TopicId});
        }

        [Authorize]        
        [HttpDelete]
        [Route("comments/{commentId}")]
        public async Task<bool> DeleteComment(int commentId)
        {
            User user = await GetCurrentUserAsync();
            user.ModeratedCategories = _context.User_Cateogries.Where(uc => uc.ModeratorId == user.Id).ToList();
            string role = await GetCurrentUserAuthorizationLevelAsync();
            Comment comment = _context.Comments.Where(c => c.Id == commentId).Include(c => c.Topic).SingleOrDefault();
            if(role == "admin" || user.ModeratedCategories.Any(c => c.CategoryId == comment.Topic.CategoryId) || comment.CommentorId == user.Id){
                _context.Comments.Remove(comment);
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        //return user if signed in
        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        //return authorization level
        private async Task<string> GetCurrentUserAuthorizationLevelAsync()
        {
            return (await _userManager.GetRolesAsync(await GetCurrentUserAsync()))[0];
        }
    }
}
