using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Spice.Models;

namespace Spice.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        //Add Role Manager to use it for roles.... step 1
        private readonly RoleManager<IdentityRole> myRoleManager;

        //Add the Role Manger in constructor for dependecy injection
        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, 
            //Step2
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            //Step 3
            myRoleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string  State { get; set; }
            public string Village { get; set; }

        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }
   
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // lets read the radion selection value to know whihc user role has been selected.
            string userRole = Request.Form["rdRole"].ToString();
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {//Code started from here
                var user = new ApplicationUser {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name= Input.Name, 
                    Village=Input.Village,
                    State= Input.State,
                    Address= Input.Address
                     };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // Below we create those roles only once.
                    if( ! await myRoleManager.RoleExistsAsync(SC.ManagerRole))
                    {
                        await myRoleManager.CreateAsync(new IdentityRole(SC.ManagerRole));
                    }

                    if ( ! await myRoleManager.RoleExistsAsync(SC.FrontDeskRole))
                    {
                        await myRoleManager.CreateAsync(new IdentityRole(SC.FrontDeskRole));
                    }

                    if (! await myRoleManager.RoleExistsAsync(SC.CustomerRole))
                    {
                        await myRoleManager.CreateAsync(new IdentityRole(SC.CustomerRole));
                    }
                    if (!await myRoleManager.RoleExistsAsync(SC.KitchenRole))
                    {
                        await myRoleManager.CreateAsync(new IdentityRole(SC.KitchenRole));
                    }
                    //Below we add the user to selected roles and the the default is customer.
                    // lets try to add the user to admin rol the first user.
                    //The user is added with password Abc_123
                    //AddtoRole is singular

                    if (userRole == SC.ManagerRole)
                    {
                        await _userManager.AddToRoleAsync(user, SC.ManagerRole);
                    }
                    else if (userRole == SC.FrontDeskRole)
                    {
                        await _userManager.AddToRoleAsync(user, SC.FrontDeskRole);

                    }
                    else if (userRole == SC.KitchenRole)
                    {
                        await _userManager.AddToRoleAsync(user, SC.KitchenRole);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SC.CustomerRole);
                        // Also we only want to go login the user automatically if it is Customer, so we will bring the sign In statement 
                        // from below here
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        //It will redirect only the Customer back to the URL
                        return LocalRedirect(returnUrl);
                    }

                    //We  want the Admin to go back to user Index Page

                    return RedirectToAction("Index", "User", new { area = "Admin" });
                    //Custome Code Ended here


                    //_logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    // This method is moved up
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return LocalRedirect(returnUrl);

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
