﻿using dnd.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace dnd.Controllers
{
    public class UserController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        public UserController(UserManager<User> um, SignInManager<User> sm)
        {
            userManager = um;
            signInManager = sm;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            // If the model is imported without issue...
            if (ModelState.IsValid)
            {
                // Take these user properties from the model 
                var user = new User{
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                // create a user password pair
                var result = await userManager.CreateAsync(user, model.Password);

                // if the user was created in the user manager successfully
                if (result.Succeeded)
                {
                    // sign in the user with sign in manager
                    await signInManager.SignInAsync(user, isPersistent: false);

                    // return to the index action of the characters controller
                    return RedirectToAction("Index", "Characters");
                }
                // if the user was not created successfully
                else
                {
                    // take all the errors from the result object
                    foreach (var error in result.Errors)
                    {
                        // add a model error with description
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            // returns to the create a user view with the model properties
            return View("CreateUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            // If the model is imported without issue...
            if (ModelState.IsValid)
            {
                // Take these user properties from the model 
                var result = await signInManager.PasswordSignInAsync(
                    model.UserName,
                    model.Password,
                    isPersistent: model.RememberMe,
                    lockoutOnFailure: false);

                // If SignInAsync succeeds
                if (result.Succeeded)
                {
                    // Redirect to the index page, home controller
                    return RedirectToAction("Index", "Home");
                }
            }
            // This will run only if the model is invalid
            ModelState.AddModelError(string.Empty, "Invalid username/password");
            return View("Index",model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync(); // Call the sign in manager's sign out method
            return RedirectToAction("Index","Home"); // return to index action of home controller
        }
    }
}
