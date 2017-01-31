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
    public class UserController : Controller
    {
        private Context _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserController(Context context, UserManager<User> userManager, SignInManager<User> signInManager){
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return RedirectToAction("ShowLogin");
        }

        [HttpGet]
        [Route("login")]
        public IActionResult ShowLogin()
        {
            return View("Login");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> ProcessLogin(RegistrationUser model)
        {
            if(model.password == null){
                model.password = "";
            }
            if(model.email == null){
                model.email = "";
            }
            var result = await _signInManager.PasswordSignInAsync(model.email, model.password, isPersistent: false, lockoutOnFailure: false);
            if(result.Succeeded){
                return RedirectToAction("ShowForum", "Category");
            }
            ModelState.Clear();
            ModelState.AddModelError("", "Invalid email or password");
            return View("Login", model);
        }

        [HttpGet]
        [Route("register")]
        public IActionResult ShowRegister()
        {
            return View("Register");
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> ProcessRegister(RegistrationUser model)
        {
            if(ModelState.IsValid)
            {
                User existing = await _userManager.FindByEmailAsync(model.email);
                Console.WriteLine(existing);
                if(existing != null)
                {
                    ModelState.AddModelError("", "Email already exists");
                }else{
                    User newUser = model.transferToUser();
                    IdentityResult result = await _userManager.CreateAsync(newUser, model.password);
                    if(result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newUser, "basic");
                        await _signInManager.SignInAsync(newUser, isPersistent: false);
                        return RedirectToAction("ShowForum", "Category");
                    }
                    else{
                        foreach(var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            return View("Register", model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("users")]
        public IActionResult ShowUsers()
        {
            return View("Users", _context.Users.ToList());
        }

        [Authorize]        
        [HttpGet]
        [Route("users/{userId}")]
        public async Task<IActionResult> EditUser(string userId)
        {
            User user;
            if(userId != "0" && await GetCurrentUserAuthorizationLevelAsync() == "admin"){
                user = _context.Users.Where(u => u.Id == userId).Include(u => u.ModeratedCategories).SingleOrDefault();
                ViewBag.Self = false;
            }else{
                ViewBag.Self = true;
                user = await GetCurrentUserAsync();
                user.ModeratedCategories = _context.User_Cateogries.Where(c => c.ModeratorId == user.Id).ToList();
            }
            UpdateUser updateUser = new UpdateUser();
            updateUser.TransferFromUser(user);

            ViewBag.OtherAuthLevel = (await _userManager.GetRolesAsync(user))[0];
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
            return View("ShowUser", updateUser);
        }   

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("update-moderator/{categoryId}/{userId}")]
        public async Task<bool> UpdateModerator(int categoryId, string userId)
        {
            User user = _context.Users.Where(u => u.Id == userId).Include(u => u.ModeratedCategories).SingleOrDefault();
            string currentRole = (await _userManager.GetRolesAsync(user))[0];
            if(user.ModeratedCategories.Any(mc => mc.CategoryId == categoryId)){
                _context.User_Cateogries.Remove(user.ModeratedCategories.SingleOrDefault(uc => uc.CategoryId == categoryId));
                if(user.ModeratedCategories.Count == 1){
                    if(currentRole != "admin"){
                        await _userManager.RemoveFromRoleAsync(user, "moderator");
                        await _userManager.AddToRoleAsync(user, "basic");
                    }
                }
            }else{
                User_Cateogry uc = new User_Cateogry();
                uc.ModeratorId = userId;
                uc.CategoryId = categoryId;
                _context.Add(uc);
                if(currentRole != "admin"){
                    await _userManager.RemoveFromRoleAsync(user, "basic");
                    await _userManager.AddToRoleAsync(user, "moderator");
                }
            }
            _context.SaveChanges();
            return true;
        }

        // [Authorize(Roles = "admin")]
        [Authorize]
        [HttpPost]
        [Route("update-user/{userId}")]
        public async Task<IActionResult> UpdateUser(UpdateUser updateUser, string userId)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
            updateUser.ModeratedCategories = _context.User_Cateogries.Where(c => c.ModeratorId == userId).ToList();
            updateUser.Id = userId;

            User currentUser = await GetCurrentUserAsync();
            if(userId == currentUser.Id){
                ViewBag.Self = true;
            }else{
                ViewBag.Self = false;
            }
            ViewBag.OtherAuthLevel = (await _userManager.GetRolesAsync(currentUser))[0];
            String authorization = await GetCurrentUserAuthorizationLevelAsync();
            if(authorization == "admin" || userId == currentUser.Id){
                if(ModelState.IsValid){
                    if(updateUser.OldPassword == null){
                        updateUser.OldPassword = "";
                    }
                    if(userId == currentUser.Id && await _userManager.CheckPasswordAsync(currentUser, updateUser.OldPassword)){ //updating self
                        updateUser.transferToUser(currentUser);
                        await _userManager.UpdateAsync(currentUser);
                        await _userManager.ChangePasswordAsync(currentUser, updateUser.OldPassword, updateUser.Password);
                    }else if(authorization == "admin"){ //updating other user
                        User user = _context.Users.SingleOrDefault(u => u.Id == userId);
                        updateUser.transferToUser(user);
                        await _userManager.UpdateAsync(user);
                        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, resetToken, updateUser.Password);
                        ViewBag.OtherAuthLevel = (await _userManager.GetRolesAsync(user))[0];
                    }else{
                        ModelState.AddModelError("OldPassword", "Old Password is incorrect");
                        return View("ShowUser", updateUser);      
                    }
                }else{
                    return View("ShowUser", updateUser);                
                }
            }
            return RedirectToAction("EditUser", new {userId = userId});
        }

        [AuthorizeAttribute(Roles = "admin")]
        [HttpGet]
        [Route("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            User user = _context.Users.SingleOrDefault(u => u.Id == userId);
            string role = (await _userManager.GetRolesAsync(user))[0];
            if(role != "admin"){
                _context.Comments.RemoveRange(_context.Comments.Where(c => c.CommentorId == userId));
                _context.Topics.RemoveRange(_context.Topics.Where(t => t.CreatorId == userId));
                _context.User_Cateogries.RemoveRange(_context.User_Cateogries.Where(uc => uc.ModeratorId == userId));
                _context.SaveChanges();
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("ShowUsers");
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }



        //return user if signed in
        public Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        //return authorization level
        public async Task<string> GetCurrentUserAuthorizationLevelAsync()
        {
            return (await _userManager.GetRolesAsync(await GetCurrentUserAsync()))[0];
        }
    }
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}
