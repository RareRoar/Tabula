using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Tabula.ViewModels;
using Tabula.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Tabula.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Tabula.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<Profile> _userManager;
        private readonly SignInManager<Profile> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;

        public AccountController(ILogger<AccountController> logger, UserManager<Profile> userManager,
            SignInManager<Profile> signInManager, IEmailSender emailSender, IWebHostEnvironment env)
        {
            _emailSender = emailSender;
            _logger = logger;
            _userManager = userManager;
            _env = env;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userManager.GetUserAsync(User));
        }

        public async Task<IActionResult> SendConfirmationEmail()
        {
            Profile user = await _userManager.GetUserAsync(User);
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { code = code },
                        protocol: HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Confirm your account",
                $"Hi from Tabula! Confirm your account by the <a href='{callbackUrl}'>link</a>");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string code)
        {
            if ( code == null)
            {
                _logger.LogError("Null confirmation email");

                return View("Error");
            }

            Profile user = await _userManager.GetUserAsync(User);
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
            {
                _logger.LogError("Invalid confirmation email");

                return View("Error");
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imagePath = null;
                IFormFile uploadedImage = model.AvatarFile;
                if (uploadedImage != null && uploadedImage.ContentType.ToLower().StartsWith("image/"))
                {
                    imagePath = "/images/avatars/" + model.Name + uploadedImage.FileName;
                    using (var fileStream = new FileStream(_env.WebRootPath + imagePath, FileMode.Create))
                    {
                        await uploadedImage.CopyToAsync(fileStream);
                    }
                }

                Profile user = new Profile { Email = model.Email, UserName = model.Name, Avatar = imagePath };
                
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                
                if (result.Succeeded)
                {
                    _logger.LogDebug("Login succeed");
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Login failed");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}