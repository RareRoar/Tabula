using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.Mvc;
using Tabula.Hubs;
using Tabula.Interfaces;
using Tabula.Models;
using Microsoft.Extensions.Logging;

namespace Tabula.Controllers
{
    public class TestController : Controller
    {
        private readonly IAppDbInitData _idgen;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<AccountController> _logger;

        public TestController(IHubContext<NotificationHub> hubContext, IAppDbInitData gen, ILogger<AccountController> logger)
        {
            _logger = logger;
            _hubContext = hubContext;
            _idgen = gen;
        }
        public async Task<IActionResult> Index()
        {
            int b = 1;
            int a = 0;
            int c = b / a;
            //Response.Body.Write("<script language=javascript>alert('Message here.')</script>");
            await _hubContext.Clients.All.SendAsync("added", "!!!");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync()
        {
            await _hubContext.Clients.All.SendAsync("added", "!!!");
            return View("Index");
        }
    }
}
