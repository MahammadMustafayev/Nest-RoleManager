using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nest_BaskEnd_Project.Models;
using Nest_BaskEnd_Project.Utilities;
using Nest_BaskEnd_Project.ViewModel.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_BaskEnd_Project.Controllers
{
    public class AuthorController : Controller
    {
        private UserManager<AppUser> _userManager { get; }
        private SignInManager<AppUser> _signIn { get; }
        private RoleManager<IdentityRole> _roleManager { get; }

        public AuthorController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signIn,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signIn = signIn;
            _roleManager = roleManager;
        }       
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        
        public async Task<IActionResult> Login(LoginVM loginVM,string ReturnUrl)
        {
            AppUser user;
            if(loginVM.UsernameOrEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            }
            if(user==null)
            {
                ModelState.AddModelError("", "UserName or Password incorrect");
                return View(loginVM);
            }
            var result= await _signIn.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe,true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Sinama cehdini asdiniz,Gozle");
                return View(loginVM);
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password incorrect!");
                return View(loginVM);
            }
            if (ReturnUrl != null) return LocalRedirect(ReturnUrl);
            return RedirectToAction("Index","Home");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            AppUser user = new AppUser
            {
                Name=register.FirstName,
                Surname=register.LastName,
                Email=register.Email,
                UserName=register.UserName
            };
            IdentityResult result = await _userManager.CreateAsync(user,register.Password);
            
            if (!ModelState.IsValid) return View();
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            await _userManager.AddToRoleAsync(user, UserRoles.Member.ToString());

            //await _signIn.SignInAsync(user, true);
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> SignOut()
        {
            await  _signIn.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task CreateRoles()
        {
            foreach (var item in Enum.GetValues(typeof(UserRoles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString())) await _roleManager.CreateAsync(new IdentityRole(item.ToString()));
            }
        }
        public IActionResult ChangePassword(ChangeVM changeVM)
        {
            if (changeVM.OldPassword == null)
            {
                ModelState.AddModelError("", "The password cannot be entered incorrectly");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
