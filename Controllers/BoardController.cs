using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tabula.Models;
using Tabula.Interfaces;
using Tabula.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Tabula.Controllers
{
    public class BoardController : Controller
    {
        private const string archiveBoardTitle = "Archived pins";
        private const string archiveBoardDescription = "A board that keeps idle pins.";
        private readonly ILogger<AccountController> _logger;
        private readonly IApplicationDbContext _db;
        private readonly UserManager<Profile> _userManager;
        private readonly IUniqueIdGenerator _idGen;
        public BoardController(ILogger<AccountController> logger, IApplicationDbContext db, UserManager<Profile> userManager, IUniqueIdGenerator idGen)
        {
            _idGen = idGen;
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User);
            var boardList = (from item in _db.Boards
                             where item.Profile.Id == userId
                             select item).ToList();
            var pinDict = new Dictionary<Board, List<Pin>>();
            foreach (var board in boardList)
            {
                var temp = (from item in _db.Pins where item.Board == board select item).Take(5).ToList();
                pinDict.Add(board, temp);
            }
            ViewBag.FirstPinsMap = pinDict;
            return View(boardList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Board board)
        {
            //board.Id = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            board.Profile = await _userManager.GetUserAsync(User);
            _db.Boards.Add(board);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Remove")]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            if (id != null)
            {
                Board boardToDelete = await _db.Boards.FirstOrDefaultAsync(p => p.Id == id);
                if (boardToDelete != null)
                    return View(boardToDelete);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Remove(string id)
        {
            if (id != null)
            {
                Board boardToDelete = await _db.Boards.FirstOrDefaultAsync(p => p.Id == id);
                boardToDelete.Profile = await (from item in _db.Boards where item.Id == id select item.Profile).FirstOrDefaultAsync();
                if (boardToDelete.Title == archiveBoardTitle)
                {
                    return RedirectToAction("Index");
                }
                if (boardToDelete != null)
                {
                    var archiveBoard = await (from item in _db.Boards where item.Title == archiveBoardTitle && item.Profile == boardToDelete.Profile select item).FirstOrDefaultAsync();
                    if (archiveBoard == null)
                    {
                        archiveBoard = new Board
                        {
                            Id = _idGen.GenerateUniqueId(),
                            Title = archiveBoardTitle,
                            Description = archiveBoardDescription
                        };
                        archiveBoard.Profile = boardToDelete.Profile;
                        _db.Boards.Add(archiveBoard);
                    }
                    foreach(var archivePin in (from item in _db.Pins where item.Board == boardToDelete select item))
                    {
                        archivePin.Board = archiveBoard;
                        _db.Pins.Update(archivePin);
                    }
                    _db.Boards.Remove(boardToDelete);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            Board board = await _db.Boards.FirstOrDefaultAsync(p => p.Id == id);
            return View(new BoardViewModel { Id = board.Id, Title = board.Title, Description = board.Description});
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BoardViewModel model)
        {
            if (ModelState.IsValid)
            {
                Board board = new Board { Id = model.Id, Title = model.Title, Description = model.Description };
                board.Profile = await _userManager.GetUserAsync(User);
                _db.Boards.Update(board);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Board editing failed");
            }
            return View(model);
        }
        public async Task<IActionResult> Watch(string id)
        {
            _logger.LogDebug("boardId " + id);
            var pins = (from item in _db.Pins where item.Board.Id == id select item).ToArray();
            ViewBag.Pins = pins;
            Board board = await _db.Boards.FirstOrDefaultAsync(p => p.Id == id);
            return View(board);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public IActionResult Moderate()
        {
            var tupleQuery = from item in _db.Boards select new { Review = item, item.Profile };
            var boardList = new List<Board>();
            foreach (var item in tupleQuery)
            {
                var buffer = item.Review;
                buffer.Profile = item.Profile;
                boardList.Add(buffer);
            }
            return View(boardList);
        }
    }
}
