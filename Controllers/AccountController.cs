using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Tabula.ViewModels;
using Tabula.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Tabula.Interfaces;

namespace Tabula.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<Profile> _userManager;
        private readonly SignInManager<Profile> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(ILogger<AccountController> logger,
            UserManager<Profile> userManager, SignInManager<Profile> signInManager,
            IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userManager.GetUserAsync(User));
        }

        public async Task<IActionResult> SendConfirmationEmail()
        {
            var user = await _userManager.GetUserAsync(User);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(
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
                return View("Error");
            }
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
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
                byte[] imageData = null;
                IFormFile uploadedImage = model.AvatarFile;
                if (uploadedImage.ContentType.ToLower().StartsWith("image/"))
                {
                    using (var binary = new BinaryReader(uploadedImage.OpenReadStream()))
                    {
                        imageData = binary.ReadBytes((int)uploadedImage.OpenReadStream().Length);
                    }
                }           
                Profile user = new Profile { Email = model.Email, UserName = model.Name, Avatar = imageData };
                
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    // установка куки
                    //return RedirectToAction("Index");
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
                //model.Email = "logintest@gmail.com";
                //model.Password = "Qwe123_";
                var result =
                    await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogDebug("login succeed");
                    // проверяем, принадлежит ли URL приложению
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

        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            //return View();
            return RedirectToAction("Index", "Home");
        }
        /*
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Roles()
        {
            return View();
        }*/
    }
}