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

namespace Tabula.Controllers
{
    [Authorize(Roles = "Admin,Moderator,User")]
    public class PinController : Controller
    {
        private readonly IUniqueIdGenerator _idGenerator;
        private readonly ILogger<PinController> _logger;
        private readonly IApplicationDbContext _db;
        private readonly UserManager<Profile> _userManager;
        
        public PinController(ILogger<PinController> logger, IApplicationDbContext db,
            UserManager<Profile> userManager, IUniqueIdGenerator idGenerator)
        {
            _idGenerator = idGenerator;
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string id)
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
                byte[] imageData = null;
                IFormFile uploadedImage = model.Image;
                if (uploadedImage.ContentType.ToLower().StartsWith("image/"))
                {
                    using (var binary = new BinaryReader(uploadedImage.OpenReadStream()))
                    {
                        imageData = binary.ReadBytes((int)uploadedImage.OpenReadStream().Length);
                    }
                }
                Pin pin = new Pin
                {
                    Title = model.Title, 
                    Image = imageData
                };
                pin.Id = _idGenerator.GenerateUniqueId();
                pin.Board = await _db.Boards.FirstOrDefaultAsync(p => p.Title == model.BoardTitle);

                _db.Pins.Add(pin);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Added pin {pin.Title}");
            }
            return RedirectToAction("Index", "Board");
        }
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            if (id != null)
            {
                Pin pinToDelete = await _db.Pins.FirstOrDefaultAsync(p => p.Id == id);
                if (pinToDelete != null)

                    return View(pinToDelete);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (id != null)
            {
                Pin pinToDelete = await _db.Pins.FirstOrDefaultAsync(p => p.Id == id);
                if (pinToDelete != null)
                {
                    _db.Pins.Remove(pinToDelete);
                    await _db.SaveChangesAsync();

                    _logger.LogInformation($"Deleted pin {pinToDelete.Title}");

                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
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
                byte[] imageData = null;
                IFormFile uploadedImage = model.Image;
                if (uploadedImage.ContentType.ToLower().StartsWith("image/"))
                {
                    using (var binary = new BinaryReader(uploadedImage.OpenReadStream()))
                    {
                        imageData = binary.ReadBytes((int)uploadedImage.OpenReadStream().Length);
                    }
                }
                Pin pin = new Pin { Id = model.Id, Title = model.Title, Image = imageData };
                pin.Board = await _db.Boards.FirstOrDefaultAsync(p => p.Title == model.BoardTitle);

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
            var tupleQuery = from item in _db.Pins
                             select new 
                             { 
                                 Pin = item,
                                 Board = item.Board,
                                 Profile = item.Board.Profile
                             };
            var pinList = new List<Pin>();
            foreach (var item in tupleQuery)
            {
                var buffer = item.Pin;
                buffer.Board = item.Board;
                buffer.Board.Profile = item.Profile;
                pinList.Add(buffer);
            }

            return View(pinList);
        }
    }
}
