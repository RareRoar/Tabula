using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tabula.Hubs;
using Tabula.Models;
using Tabula.ViewModels;

namespace Tabula.Controllers
{
    public class ReviewController : Controller
    {
        private readonly UserManager<Profile> _userManager;
        private readonly AppDbContext _db;
        private readonly IHubContext<NotificationHub> _notifHub;
        private readonly ILogger<ReviewController> _logger;
        public ReviewController(AppDbContext db, UserManager<Profile> userManager,
            IHubContext<NotificationHub> notifHub, ILogger<ReviewController> logger)
        {
            _logger = logger;
            _notifHub = notifHub;
            _userManager = userManager;
            _db = db;
        }
                
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create(int pinId)
        {
            return View(new ReviewViewModel { PinId = pinId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            var pin = await _db.Pins.FirstOrDefaultAsync(p => p.Id == model.PinId);
            Review review = new Review
            {  
                Liked = model.Liked,
                Comment = model.Comment, 
                Pin = pin 
            };

            var currentUser = await _userManager.GetUserAsync(User);
            review.Profile = currentUser;

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Written review by {currentUser.UserName}");

            string emotion = model.Liked ? "liked" : "disliked";
            string message = string.Format("User {0} {1} your pin \'{2}\': {3}",
                review.Profile.UserName,
                emotion,
                pin.Title,
                model.Comment);
            var recieverBoard =  await (from item in _db.Pins
                                        where item.Id == model.PinId
                                        select item.Board)
                                        .FirstOrDefaultAsync();
            var recieverId = recieverBoard.ProfileId;

            _logger.LogDebug($"Sending message to {recieverId}");

            if (recieverId != currentUser.Id)
                await _notifHub.Clients.User(recieverId).SendAsync("DisplayNotification", message);

            return RedirectToAction("Index", "Pin", new { id = model.PinId });
        }
    }
}
