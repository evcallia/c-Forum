// // using System;
// // using System.Collections.Generic;
// // using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Identity;
// // using Microsoft.AspNetCore.Mvc;
// // using Microsoft.EntityFrameworkCore;
// // using Microsoft.AspNetCore.Authorization;
// // using System.Linq;
// // using Microsoft.AspNetCore.Mvc.ViewFeatures;
// // using Newtonsoft.Json;

// // using forum.Models;

// // namespace forum.Controllers
// // {
// //     public class HomeController : Controller
// //     {
// //         private Context _context;
// //         private readonly UserManager<User> _userManager;
// //         private readonly SignInManager<User> _signInManager;
// //         public HomeController(Context context, UserManager<User> userManager, SignInManager<User> signInManager){
// //             _context = context;
// //             _userManager = userManager;
// //             _signInManager = signInManager;
// //         }

// //         [HttpGet]
// //         [Route("")]
// //         public IActionResult Index()
// //         {
// //             return RedirectToAction("ShowLogin");
// //         }

// //         [HttpGet]
// //         [Route("login")]
// //         public IActionResult ShowLogin()
// //         {
// //             return View("Login");
// //         }

// //         [HttpPost]
// //         [Route("login")]
// //         public async Task<IActionResult> ProcessLogin(RegistrationUser model)
// //         {
// //             var result = await _signInManager.PasswordSignInAsync(model.email, model.password, isPersistent: false, lockoutOnFailure: false);
// //             if(result.Succeeded){
// //                 return RedirectToAction("ShowForum");
// //             }
// //             ModelState.Clear();
// //             ModelState.AddModelError("", "Invalid email or password");
// //             return View("Login", model);
// //         }

// //         [HttpGet]
// //         [Route("register")]
// //         public IActionResult ShowRegister()
// //         {
// //             return View("Register");
// //         }

// //         [HttpPost]
// //         [Route("register")]
// //         public async Task<IActionResult> ProcessRegister(RegistrationUser model)
// //         {
// //             if(ModelState.IsValid)
// //             {
// //                 User existing = await _userManager.FindByEmailAsync(model.email);
// //                 Console.WriteLine(existing);
// //                 if(existing != null)
// //                 {
// //                     ModelState.AddModelError("", "Email already exists");
// //                 }else{
// //                     User newUser = model.transferToUser();
// //                     IdentityResult result = await _userManager.CreateAsync(newUser, model.password);
// //                     if(result.Succeeded)
// //                     {
// //                         await _userManager.AddToRoleAsync(newUser, "basic");
// //                         await _signInManager.SignInAsync(newUser, isPersistent: false);
// //                         return RedirectToAction("ShowForum");
// //                     }
// //                     else{
// //                         foreach(var error in result.Errors)
// //                         {
// //                             ModelState.AddModelError("", error.Description);
// //                         }
// //                     }
// //                 }
// //             }
// //             return View("Register", model);
// //         }

// //         [Authorize]
// //         [HttpGet]
// //         [Route("home")]
// //         public async Task<IActionResult> ShowForum()
// //         {
// //             User user = await GetCurrentUserAsync();
// //             // run query for getting top topics of the day

// //             // run query for getting Recent topics of the day

// //             ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
// //             return View("Home");
// //         }

// //         [Authorize(Roles = "admin")]
// //         [HttpGet]
// //         [Route("users")]
// //         public IActionResult ShowUsers()
// //         {
// //             return View("Users", _context.Users.ToList());
// //         }

// //         [Authorize]        
// //         [HttpGet]
// //         [Route("users/{userId}")]
// //         public async Task<IActionResult> EditUser(string userId)
// //         {
// //             User user;
// //             if(userId == "-1"){
// //                 ViewBag.Self = true;
// //                 user = await GetCurrentUserAsync();
// //             }else{
// //                 ViewBag.Self = false;
// //                 user = _context.Users.Where(u => u.Id == userId).SingleOrDefault();
// //             }
// //             UpdateUser updateUser = new UpdateUser();
// //             updateUser.TransferFromUser(user);

// //             ViewBag.Categories = _context.Categories.ToList();

// //             ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
// //             return View("ShowUser", updateUser);
// //         }

// //         [Authorize]        
// //         [HttpGet]
// //         [Route("categories")]
// //         public async Task<IActionResult> ShowCategories()
// //         {
// //             // // For testing.. Asign admin user
// //             // User user = await GetCurrentUserAsync();
// //             // await _userManager.RemoveFromRoleAsync(user, "basic");
// //             // await _userManager.AddToRoleAsync(user, "admin");
// //             // // create moderator
// //             // User user = await GetCurrentUserAsync();
// //             // var join = new User_Cateogry();
// //             // join.ModeratorId = user.Id;
// //             // join.CategoryId = _context.Categories.FirstOrDefault().Id;
// //             // _context.Add(join);
// //             // _context.SaveChanges();
// //             ViewBag.Error = TempData["error"];
// //             ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
// //             return View("Categories", _context.Categories.Include(c => c.Topics).Include(c => c.Moderators).ThenInclude(m => m.Moderator).ToList());
// //         }

// //         [Authorize]        
// //         [HttpPost]
// //         [Route("categories")]
// //         public IActionResult CreateCategory(Category newCategory)
// //         {
// //             if(ModelState.IsValid){
// //                 Category existing = _context.Categories.Where(c => c.Name.ToLower() == newCategory.Name.ToLower()).FirstOrDefault();
// //                 if(existing != null){
// //                     TempData["error"] = "Category already exists";
// //                 }else{
// //                     _context.Categories.Add(newCategory);
// //                     _context.SaveChanges();
// //                 }
// //             }else{
// //                 TempData["error"] = "A name is required";
// //             }
// //             //TODO: Pass model back in TempData/viewbag so the form is not cleared on error
// //             return RedirectToAction("ShowCategory", new {id = newCategory.Id});
// //         }

// //         [Authorize]        
// //         [HttpGet]
// //         [Route("categories/{id}")]
// //         public async Task<IActionResult> ShowCategory(int id)
// //         {
// //             Category category = _context.Categories.Where(c => c.Id == id).Include(c => c.Topics).ThenInclude(t => t.Creator).SingleOrDefault();
// //             Topic newTopic = TempData.Get<Topic>("NewTopic");
// //             if(newTopic == null){
// //                 newTopic = new Topic();
// //                 newTopic.CategoryId = category.Id;
// //                 ViewBag.NewTopic = newTopic;
// //             }else{
// //                 ViewBag.NewTopic = newTopic;
// //             }
// //             if(TempData["Errors"] != null){
// //                 ViewBag.Errors = TempData["Errors"];
// //             }
// //             ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
// //             return View("ShowCategory", category);
// //         }  

// //         [Authorize]        
// //         [HttpDelete]
// //         [Route("categories/{categoryId}")]
// //         public bool DeleteCategory(int categoryId)
// //         {
// //             Category category = _context.Categories.Where(c => c.Id == categoryId).SingleOrDefault();
// //             _context.Categories.Remove(category);
// //             _context.SaveChanges();
// //             return true;
// //         } 

// //         [Authorize]        
// //         [HttpPost]
// //         [Route("topics")]
// //         public async Task<IActionResult> CreateTopic(Topic newTopic)
// //         {
// //             if(ModelState.IsValid){
// //                 User user = await GetCurrentUserAsync();
// //                 newTopic.CreatorId = user.Id;
// //                 _context.Topics.Add(newTopic);
// //                 _context.SaveChanges();
// //             }else{
// //                 TempData.Put("NewTopic", newTopic);
// //                 List<string> errors = new List<string>();
// //                 foreach(var error in ModelState.Values){
// //                     if(error.Errors.Count > 0){
// //                             errors.Add(error.Errors[0].ErrorMessage);
// //                     }
// //                 }
// //                 TempData["Errors"] = errors;
// //             }
// //             return RedirectToAction("ShowCategory", new {id = newTopic.CategoryId}); 
// //         }     

// //         [Authorize]        
// //         [HttpGet]
// //         [Route("topics/{id}")]
// //         public async Task<IActionResult> ShowTopic(int id)
// //         {
// //             ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
// //             return View("ShowTopic", _context.Topics.Where(t => t.Id == id).Include(t => t.Comments).ThenInclude(c => c.Commentor).SingleOrDefault());
// //         }  

// //         [Authorize]        
// //         [HttpDelete]
// //         [Route("topics/{topicId}")]
// //         public bool DeleteTopic(int topicId)
// //         {
// //             Topic topic = _context.Topics.Where(t => t.Id == topicId).SingleOrDefault();
// //             _context.Topics.Remove(topic);
// //             _context.SaveChanges();
// //             return true;
// //         }     



// //         [HttpGet]
// //         [Route("logout")]
// //         public async Task<IActionResult> Logout()
// //         {
// //             await _signInManager.SignOutAsync();
// //             return RedirectToAction("Index");
// //         }



// //         //return user if signed in
// //         private Task<User> GetCurrentUserAsync()
// //         {
// //             return _userManager.GetUserAsync(HttpContext.User);
// //         }
// //         //return authorization level
// //         private async Task<string> GetCurrentUserAuthorizationLevelAsync()
// //         {
// //             return (await _userManager.GetRolesAsync(await GetCurrentUserAsync()))[0];
// //         }
        

// //     }
//     using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authorization;
// using System.Linq;
// using Microsoft.AspNetCore.Mvc.ViewFeatures;
// using Newtonsoft.Json;

// using forum.Models;

// namespace forum.Controllers
// {
//     public class HomeController : Controller
//     {
//         private Context _context;
//         private readonly UserManager<User> _userManager;
//         private readonly SignInManager<User> _signInManager;
//         public HomeController(Context context, UserManager<User> userManager, SignInManager<User> signInManager){
//             _context = context;
//             _userManager = userManager;
//             _signInManager = signInManager;
//         }

//         [HttpGet]
//         [Route("")]
//         public IActionResult Index()
//         {
//             return RedirectToAction("ShowLogin");
//         }

//         [HttpGet]
//         [Route("login")]
//         public IActionResult ShowLogin()
//         {
//             return View("Login");
//         }

//         [HttpPost]
//         [Route("login")]
//         public async Task<IActionResult> ProcessLogin(RegistrationUser model)
//         {
//             var result = await _signInManager.PasswordSignInAsync(model.email, model.password, isPersistent: false, lockoutOnFailure: false);
//             if(result.Succeeded){
//                 return RedirectToAction("ShowForum");
//             }
//             ModelState.Clear();
//             ModelState.AddModelError("", "Invalid email or password");
//             return View("Login", model);
//         }

//         [HttpGet]
//         [Route("register")]
//         public IActionResult ShowRegister()
//         {
//             return View("Register");
//         }

//         [HttpPost]
//         [Route("register")]
//         public async Task<IActionResult> ProcessRegister(RegistrationUser model)
//         {
//             if(ModelState.IsValid)
//             {
//                 User existing = await _userManager.FindByEmailAsync(model.email);
//                 Console.WriteLine(existing);
//                 if(existing != null)
//                 {
//                     ModelState.AddModelError("", "Email already exists");
//                 }else{
//                     User newUser = model.transferToUser();
//                     IdentityResult result = await _userManager.CreateAsync(newUser, model.password);
//                     if(result.Succeeded)
//                     {
//                         await _userManager.AddToRoleAsync(newUser, "basic");
//                         await _signInManager.SignInAsync(newUser, isPersistent: false);
//                         return RedirectToAction("ShowForum");
//                     }
//                     else{
//                         foreach(var error in result.Errors)
//                         {
//                             ModelState.AddModelError("", error.Description);
//                         }
//                     }
//                 }
//             }
//             return View("Register", model);
//         }

//         [Authorize]
//         [HttpGet]
//         [Route("home")]
//         public async Task<IActionResult> ShowForum()
//         {
//             User user = await GetCurrentUserAsync();
//             // run query for getting top topics of the day

//             // run query for getting Recent topics of the day

//             ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
//             return View("Home");
//         }

//         [Authorize(Roles = "admin")]
//         [HttpGet]
//         [Route("users")]
//         public IActionResult ShowUsers()
//         {
//             return View("Users", _context.Users.ToList());
//         }

//         [Authorize]        
//         [HttpGet]
//         [Route("users/{userId}")]
//         public async Task<IActionResult> EditUser(string userId)
//         {
//             User user;
//             if(userId == "-1"){
//                 ViewBag.Self = true;
//                 user = await GetCurrentUserAsync();
//             }else{
//                 ViewBag.Self = false;
//                 user = _context.Users.Where(u => u.Id == userId).SingleOrDefault();
//             }
//             UpdateUser updateUser = new UpdateUser();
//             updateUser.TransferFromUser(user);

//             ViewBag.Categories = _context.Categories.ToList();

//             ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
//             return View("ShowUser", updateUser);
//         }

//         [Authorize]        
//         [HttpGet]
//         [Route("categories")]
//         public async Task<IActionResult> ShowCategories()
//         {
//             // // For testing.. Asign admin user
//             // User user = await GetCurrentUserAsync();
//             // await _userManager.RemoveFromRoleAsync(user, "basic");
//             // await _userManager.AddToRoleAsync(user, "admin");
//             // // create moderator
//             // User user = await GetCurrentUserAsync();
//             // var join = new User_Cateogry();
//             // join.ModeratorId = user.Id;
//             // join.CategoryId = _context.Categories.FirstOrDefault().Id;
//             // _context.Add(join);
//             // _context.SaveChanges();
//             ViewBag.Error = TempData["error"];
//             ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
//             return View("Categories", _context.Categories.Include(c => c.Topics).Include(c => c.Moderators).ThenInclude(m => m.Moderator).ToList());
//         }

//         [Authorize]        
//         [HttpPost]
//         [Route("categories")]
//         public IActionResult CreateCategory(Category newCategory)
//         {
//             if(ModelState.IsValid){
//                 Category existing = _context.Categories.Where(c => c.Name.ToLower() == newCategory.Name.ToLower()).FirstOrDefault();
//                 if(existing != null){
//                     TempData["error"] = "Category already exists";
//                 }else{
//                     _context.Categories.Add(newCategory);
//                     _context.SaveChanges();
//                 }
//             }else{
//                 TempData["error"] = "A name is required";
//             }
//             //TODO: Pass model back in TempData/viewbag so the form is not cleared on error
//             return RedirectToAction("ShowCategory", new {id = newCategory.Id});
//         }

//         [Authorize]        
//         [HttpGet]
//         [Route("categories/{id}")]
//         public async Task<IActionResult> ShowCategory(int id)
//         {
//             Category category = _context.Categories.Where(c => c.Id == id).Include(c => c.Topics).ThenInclude(t => t.Creator).SingleOrDefault();
//             Topic newTopic = TempData.Get<Topic>("NewTopic");
//             if(newTopic == null){
//                 newTopic = new Topic();
//                 newTopic.CategoryId = category.Id;
//                 ViewBag.NewTopic = newTopic;
//             }else{
//                 ViewBag.NewTopic = newTopic;
//             }
//             if(TempData["Errors"] != null){
//                 ViewBag.Errors = TempData["Errors"];
//             }
//             ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
//             return View("ShowCategory", category);
//         }  

//         [Authorize]        
//         [HttpDelete]
//         [Route("categories/{categoryId}")]
//         public bool DeleteCategory(int categoryId)
//         {
//             Category category = _context.Categories.Where(c => c.Id == categoryId).SingleOrDefault();
//             _context.Categories.Remove(category);
//             _context.SaveChanges();
//             return true;
//         } 

//         [Authorize]        
//         [HttpPost]
//         [Route("topics")]
//         public async Task<IActionResult> CreateTopic(Topic newTopic)
//         {
//             if(ModelState.IsValid){
//                 User user = await GetCurrentUserAsync();
//                 newTopic.CreatorId = user.Id;
//                 _context.Topics.Add(newTopic);
//                 _context.SaveChanges();
//             }else{
//                 TempData.Put("NewTopic", newTopic);
//                 List<string> errors = new List<string>();
//                 foreach(var error in ModelState.Values){
//                     if(error.Errors.Count > 0){
//                             errors.Add(error.Errors[0].ErrorMessage);
//                     }
//                 }
//                 TempData["Errors"] = errors;
//             }
//             return RedirectToAction("ShowCategory", new {id = newTopic.CategoryId}); 
//         }     

//         [Authorize]        
//         [HttpGet]
//         [Route("topics/{id}")]
//         public async Task<IActionResult> ShowTopic(int id)
//         {
//             ViewBag.AuthLevel = await GetCurrentUserAuthorizationLevelAsync();
//             return View("ShowTopic", _context.Topics.Where(t => t.Id == id).Include(t => t.Comments).ThenInclude(c => c.Commentor).SingleOrDefault());
//         }  

//         [Authorize]        
//         [HttpDelete]
//         [Route("topics/{topicId}")]
//         public bool DeleteTopic(int topicId)
//         {
//             Topic topic = _context.Topics.Where(t => t.Id == topicId).SingleOrDefault();
//             _context.Topics.Remove(topic);
//             _context.SaveChanges();
//             return true;
//         }     



//         [HttpGet]
//         [Route("logout")]
//         public async Task<IActionResult> Logout()
//         {
//             await _signInManager.SignOutAsync();
//             return RedirectToAction("Index");
//         }



//         //return user if signed in
//         private Task<User> GetCurrentUserAsync()
//         {
//             return _userManager.GetUserAsync(HttpContext.User);
//         }
//         //return authorization level
//         private async Task<string> GetCurrentUserAuthorizationLevelAsync()
//         {
//             return (await _userManager.GetRolesAsync(await GetCurrentUserAsync()))[0];
//         }
        

//     }
    // public static class TempDataExtensions
    // {
    //     public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
    //     {
    //         tempData[key] = JsonConvert.SerializeObject(value);
    //     }

    //     public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
    //     {
    //         object o;
    //         tempData.TryGetValue(key, out o);
    //         return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
    //     }
    // }
// }

// // }
