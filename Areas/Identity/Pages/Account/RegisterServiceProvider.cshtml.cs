using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Tasks;
using TurboReserve.Data;
using TurboReserve.Models;

namespace TurboReserve.Areas.Identity.Pages.Account
{
    public class RegisterServiceProviderModel : PageModel
    {
        public class RegisterServiceProviderInputModel
        {
            [Required(ErrorMessage = "Nazwa firmy jest wymagana.")]
            public string BusinessName { get; set; }

            [Required(ErrorMessage = "Email jest wymagany."), EmailAddress(ErrorMessage = "Podaj poprawny adres email.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Hasło jest wymagane."), DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane."), DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Hasło i potwierdzenie hasła muszą być takie same.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Adres jest wymagany.")]
            public string Address { get; set; }

            [Required(ErrorMessage = "Miasto jest wymagane.")]
            public string City { get; set; }

            [Required(ErrorMessage = "Kod pocztowy jest wymagany.")]
            public string ZipCode { get; set; }

            [Required(ErrorMessage = "Szerokość geograficzna jest wymagana.")]
            public double Latitude { get; set; }

            [Required(ErrorMessage = "Długość geograficzna jest wymagana.")]
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

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "ServiceProvider");

                    // Parsowanie Latitude i Longitude z InvariantCulture
                    var latitude = double.Parse(Input.Latitude.ToString(), CultureInfo.InvariantCulture);
                    var longitude = double.Parse(Input.Longitude.ToString(), CultureInfo.InvariantCulture);

                    var serviceProvider = new Models.ServiceProvider
                    {
                        IdentityUserId = user.Id,
                        BusinessName = Input.BusinessName,
                        Address = Input.Address,
                        City = Input.City,
                        ZipCode = Input.ZipCode,
                        Latitude = latitude,
                        Longitude = longitude
                    };
                    _context.ServiceProviders.Add(serviceProvider);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Użytkownik utworzył nowe konto z hasłem i został przypisany jako ServiceProvider.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }


    }
}
