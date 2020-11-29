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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Tabula.Controllers
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private readonly IWebHostEnvironment _env;
        public TestController(ILogger<TestController> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (_env.IsProduction())
            {
                return View("Error");
            }

            _logger.LogDebug("Testing");

            return View();
        }

    }
}
