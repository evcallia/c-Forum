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
    public class CategoryController : Controller
    {
        private Context _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public CategoryController(Context context, UserManager<User> userManager, SignInManager<User> signInManager){
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpGet]
        [Route("home")]
        public async Task<IActionResult> ShowForum()
        {
            User user = await GetCurrentUserAsync();
            // run query for getting top topics of the day

            // run query for getting Recent topics of the day

            ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
            return View("Home");
        }

        [Authorize]        
        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> ShowCategories()
        {
            ViewBag.Error = TempData["error"];
            ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
            return View("Categories", _context.Categories.Include(c => c.Topics).Include(c => c.Moderators).ThenInclude(m => m.Moderator).ToList());
        }

        [Authorize]        
        [HttpPost]
        [Route("categories")]
        public IActionResult CreateCategory(Category newCategory)
        {
            if(ModelState.IsValid){
                Category existing = _context.Categories.Where(c => c.Name.ToLower() == newCategory.Name.ToLower()).FirstOrDefault();
                if(existing != null){
                    TempData["error"] = "Category already exists";
                }else{
                    _context.Categories.Add(newCategory);
                    _context.SaveChanges();
                }
            }else{
                TempData["error"] = "A name is required";
            }
            //TODO: Pass model back in TempData/viewbag so the form is not cleared on error
            return RedirectToAction("ShowCategory", new {id = newCategory.Id});
        }

        [Authorize]        
        [HttpGet]
        [Route("categories/{id}")]
        public async Task<IActionResult> ShowCategory(int id)
        {
            Category category = _context.Categories.Where(c => c.Id == id).Include(c => c.Topics).ThenInclude(t => t.Creator).SingleOrDefault();
            Topic newTopic = TempData.Get<Topic>("NewTopic");
            if(newTopic == null){
                newTopic = new Topic();
                newTopic.CategoryId = category.Id;
            }
            ViewBag.NewTopic = newTopic;
            if(TempData["Errors"] != null){
                ViewBag.Errors = TempData["Errors"];
            }
            User user = await GetCurrentUserAsync();
            User_Cateogry uc = _context.User_Cateogries.SingleOrDefault(c => c.CategoryId == id && c.ModeratorId == user.Id);
            if(uc != null){
                ViewBag.IsModerator = true;
            }else{
                ViewBag.IsModerator = false;
            }
            ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
            return View("ShowCategory", category);
        }  

        [Authorize]        
        [HttpDelete]
        [Route("categories/{categoryId}")]
        public bool DeleteCategory(int categoryId)
        {
            Category category = _context.Categories.Where(c => c.Id == categoryId).SingleOrDefault();
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return true;
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
