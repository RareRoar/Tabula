using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tabula.Models;

namespace Tabula.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _db;
        public SearchController(AppDbContext db)
        {
            _db = db;
        }

        private Expression<Func<Pin, bool>> GetSearchSelectExpr(string substr)
        {
            return p => p.Title.ToLower().Replace(' ', '_').Contains(substr.ToLower().Replace(' ', '_'));
        }
        //private bool ContainsSubstring(string str, string substr)
        //{
        //    return str.ToLower().Replace(' ', '_').Contains(substr.ToLower().Replace(' ', '_'));
        //    //return str.ToLower().Contains(substr.ToLower());
        //}

        public IActionResult Index(string searchString)
        {
            var searchSelectExpr = GetSearchSelectExpr(searchString);
            var pinList = _db.Pins.Where(p => p.Title.Contains(searchString)).ToList();
            //var pinList = (from item in _db.Pins where ContainsSubstring(item.Title, searchString) select item).ToList();
            return View(pinList);
        }
    }
}
