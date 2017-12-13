using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FastpassAPI.Models;
using System.Linq;
using System;


namespace FastpassAPI.Controllers
{
    [Route("api/[controller]")]
    public class FastpassController : Controller
    {
        private readonly FastPassApiContext _context;

        public FastpassController(FastPassApiContext context)
        {
            _context = context;

        }

        [HttpPost("/?ticketid={ticketId}&rideid={rideId}")]
        public IActionResult AddFastPass(int ticketId, int rideId)
        {
            var fastpass = _context.Tickets.Where(n => n.TicketId == ticketId && n.RideId == rideId).FirstOrDefault();
            var rides = _context.Rides.ToList();

            if (fastpass == null)
            {
                return NotFound();
            }
            else
            {
                // _context.Tickets.Add(new Ticket {
                //     RideId = rideId,
                //     FastpassTime = DateTime.Now.AddHours(1)
                // });
                _context.SaveChanges();
            }
            return new ObjectResult(ticketId);
        }

        // [HttpGet (/"FastPass/?ticketId={ticketId}")]
        // public IEnumerable<JsonResult> GetFastPass(int ticketId)
        // {
        //     var fastpass = _context.Tickets.Where(n => n.TicketId == ticketId).FirstOrDefault();

        //     if(fastpass == null)
        //     {
        //         return new JsonResult[] {'not found'};
        //     }
        //     else
        //     {
        //         return new string[] {
        //             'ticketId' : fastpass.TicketId,

        //         }
        //     }
        // }



    }
}