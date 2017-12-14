using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FastpassAPI.Models;
using System.Linq;
using System;
using System.Web;
using System.Net.Http;
using System.Web.Http;

namespace FastpassAPI.Controllers
{
    [Route("api/[controller]")]
    public class FastpassController : Controller
    {
        private readonly FastPassApiContext db;
        
    
        public FastpassController(FastPassApiContext context)
        {
            db = context;

        }
        [HttpGet]
        public string Ping()
        {
            return "yo";
        }

        [HttpPost]
        [Route("ticketid={ticketId}&rideid={rideId}")]
        public IActionResult AddFastPass(int ticketId, int rideId)
        {    
            var validFastPass = db.FastPass.Where(n => n.TicketId == ticketId && n.RideId == rideId).FirstOrDefault();
            if(validFastPass != null && validFastPass.Time >= DateTime.Now)
            {
                 return new ContentResult() { Content = "Not Valid", StatusCode = 400};
            }
            // var validFastPassList = db.FastPass.Where(n => n.Time >= DateTime.Now).ToList();
            // //check if ticket has valid fastpass
            // var validTicket = validFastPassList.Where(n => n.TicketId == ticketId).FirstOrDefault();
            // if(validTicket != null)
            // {
            //     return new ContentResult() { Content = "Not Valid", StatusCode = 400};
            // }
            else
            {
                string contentMessage;
                FastPass newFp = new FastPass();
                newFp.TicketId = ticketId;
                newFp.RideId = rideId;
                newFp.Time = DateTime.Now.AddHours(1);
                db.Add(newFp);
                db.SaveChanges();
                
                contentMessage = "Please return at " + DateTime.Now.AddMinutes(30) + ". It will expire at " + DateTime.Now.AddHours(1);

                return new ContentResult() { Content = contentMessage, StatusCode = 201 };
                    
            } 
        }

        [Route("ticketId={ticketId}"), HttpGet]
        public IActionResult GetFastPass(int ticketId)
        {
            var fastpass = db.FastPass.Where(n => n.TicketId == ticketId).FirstOrDefault();
            string contentMessage;
            if (fastpass != null)
            {
                var ride = db.Rides.Where(n => n.RideId == fastpass.RideId).FirstOrDefault();
                contentMessage = "Ride: " + ride.RideDescription + ". Queue Time: " + ride.QueueTime + "minutes. Expiration Time: " + fastpass.Time; 
                return new ContentResult(){ Content = contentMessage, StatusCode = 200};
            }
            else
            {
                return new ContentResult() { Content = "Given TicketId doest not have a FastPass", StatusCode = 404};
            }
        }

        [Route("redeem?ticketid={ticketId}&rideId={rideId}"), HttpPut]
        public IActionResult RedeemFastPass(int ticketId, int rideId)
        {
            var validFastPassList = db.FastPass.Where(n => n.Time >= DateTime.Now).ToList();
        }
        

    }
}