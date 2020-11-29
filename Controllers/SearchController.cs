using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tabula.Models;
using Tabula.Interfaces;
using Microsoft.Extensions.Logging;

namespace Tabula.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly IApplicationDbContext _db;
        public SearchController(IApplicationDbContext db, ILogger<SearchController> logger)
        {
            _logger = logger;
            _db = db;
        }


        public IActionResult Index(string searchString)
        {
            var pinList = _db.Pins.Where(p => p.Title.Contains(searchString)).ToList();

            _logger.LogDebug($"Searching for {searchString}");

            return View(pinList);
        }
    }
}
