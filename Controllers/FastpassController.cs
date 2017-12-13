using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FastpassAPI.Models;
using System.Linq;

namespace FastpassAPI.Controllers
{
    [Route("api/[controller]")]
    public class FastpassController : Controller
    {
        private readonly FastpassContext _context;

        public FastpassController(FastpassContext context)
        {
            _context = context;

        }
    }
}