﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TurboReserve.Data; // Namespace do Twojego kontekstu
using TurboReserve.Models; // Namespace do Twojego modelu ServiceProvider

namespace TurboReserve.Areas.Identity.Pages.Account
{
    public class RegisterServiceProviderModel : PageModel
    {
        public class RegisterServiceProviderInputModel
        {
            [Required]
            public string BusinessName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            [Required, DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }

            public string Address { get; set; }
            public string City { get; set; }
            public string ZipCode { get; set; }
            [Required]
            public double Latitude { get; set; }

            [Required]
            public double Longitude { get; set; }
        }


        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterServiceProviderModel> _logger;

        public RegisterServiceProviderModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            ILogger<RegisterServiceProviderModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public RegisterServiceProviderInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // Przypisz rolę "ServiceProvider"
                    await _userManager.AddToRoleAsync(user, "ServiceProvider");

                    // Teraz dodaj rekord w tabeli ServiceProviders
                    var serviceProvider = new Models.ServiceProvider
                    {
                        IdentityUserId = user.Id,
                        BusinessName = Input.BusinessName,
                        Address = Input.Address,
                        City = Input.City,
                        ZipCode = Input.ZipCode,
                        Latitude = Input.Latitude,
                        Longitude = Input.Longitude
                        // ... inne pola jeżeli są wymagane
                    };
                    _context.ServiceProviders.Add(serviceProvider);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User created a new account with password and assigned as ServiceProvider.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
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