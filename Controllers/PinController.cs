using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tabula.Models;
using Tabula.ViewModels;
using Tabula.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Tabula.Controllers
{
    [Authorize(Roles = "Admin,Moderator,User")]
    public class PinController : Controller
    {
        private readonly ILogger<PinController> _logger;
        private readonly IApplicationDbContext _db;
        private readonly UserManager<Profile> _userManager;
        private readonly IWebHostEnvironment _env;
        
        public PinController(ILogger<PinController> logger, IApplicationDbContext db,
            UserManager<Profile> userManager, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int id)
        {
            var pin = await _db.Pins.FirstOrDefaultAsync(p => p.Id == id);
            var tupleQuery = from item in _db.Reviews
                             where item.Pin == pin
                             select new 
                             {
                                 Review = item,
                                 item.Profile 
                             };
            var reviewList = new List<Review>();

            foreach (var item in tupleQuery)
            {
                var buffer = item.Review;
                buffer.Profile = item.Profile;
                reviewList.Add(buffer);
            }
            ViewBag.Reviews = reviewList;

            return View(pin);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var boardTitles = (from item in _db.Boards
                               where item.Profile == currentUser
                               select item.Title)
                               .ToArray();
            ViewBag.BoardTitles = new SelectList(boardTitles);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PinViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                string imagePath = null;
                IFormFile uploadedImage = model.Image;
                if (uploadedImage != null && uploadedImage.ContentType.ToLower().StartsWith("image/"))
                {
                    imagePath = "/images/pins/" + user.UserName + uploadedImage.FileName;
                    using (var fileStream = new FileStream(_env.WebRootPath + imagePath, FileMode.Create))
                    {
                        await uploadedImage.CopyToAsync(fileStream);
                    }
                }
                Pin pin = new Pin
                {
                    Title = model.Title, 
                    Image = imagePath
                };
                pin.Board = await _db.Boards.FirstOrDefaultAsync(p => p.Title == model.BoardTitle);

                _db.Pins.Add(pin);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Added pin {pin.Title}");
            }
            return RedirectToAction("Index", "Board");
        }
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            Pin pinToDelete = await _db.Pins.FirstOrDefaultAsync(p => p.Id == id);
            if (pinToDelete != null)

                return View(pinToDelete);
            else
                return NotFound();         
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Pin pinToDelete = await _db.Pins.FirstOrDefaultAsync(p => p.Id == id);
            if (pinToDelete != null)
            {
                // [begin] remove on fixing ON DELETE CASCADE
                var reviews = from item in _db.Reviews
                              where item.Pin == pinToDelete
                              select item;
                foreach (var review in reviews)
                {
                    _db.Reviews.Remove(review);
                }
                await _db.SaveChangesAsync();
                //[end]

                string fullPath = _env.WebRootPath + pinToDelete.Image;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                _db.Pins.Remove(pinToDelete);
                await _db.SaveChangesAsync();


                _logger.LogInformation($"Deleted pin {pinToDelete.Title}");

                return RedirectToAction("Index");
            }
            else
                return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var boardTitles = (from item in _db.Boards
                               select item.Title)
                               .ToArray();
            ViewBag.BoardTitles = new SelectList(boardTitles);
            var pin = await _db.Pins.FirstOrDefaultAsync(p => p.Id == id);

            return View(new PinViewModel 
            { 
                Id = pin.Id,
                Title = pin.Title
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(PinViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imagePath = null;
                IFormFile uploadedImage = model.Image;
                if (uploadedImage != null && uploadedImage.ContentType.ToLower().StartsWith("image/"))
                {
                    imagePath = "/images/pins/" + uploadedImage.FileName;
                    using (var fileStream = new FileStream(_env.WebRootPath + imagePath, FileMode.Create))
                    {
                        await uploadedImage.CopyToAsync(fileStream);
                    }
                }
                Pin pin = new Pin {
                    Id = model.Id,
                    Title = model.Title,
                    Image = imagePath
                };
                pin.Board = await _db.Boards.FirstOrDefaultAsync(p => p.Title == model.BoardTitle);

                string fullPath = _env.WebRootPath + pin.Image;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                _db.Pins.Update(pin);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Pin editing failed");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Moderate()
        {
            var pinList = (from item in _db.Pins.Include(p => p.Board).Include(p => p.Board.Profile) select item).ToList();
            return View(pinList);
        }
    }
}
