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

        [HttpPost]
        [Route("ticketid={ticketId}&rideid={rideId}")]
        public IActionResult AddFastPass(int ticketId, int rideId)
        {    
            var validFastPass = db.FastPass.Where(n => n.TicketId == ticketId && n.RideId == rideId && n.Time <= DateTime.Now).FirstOrDefault();
            if(validFastPass != null && validFastPass.Time >= DateTime.Now && validFastPass.RedeemedTime == null)
            {
                 return new ContentResult() { Content = "Not Valid", StatusCode = 400};
            }
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

        [Route("[action]ticketid={ticketId}&rideId={rideId}"), HttpPut]
        public IActionResult redeem (int ticketId, int rideId)
        {
            var fastPass = db.FastPass.Where(n => n.TicketId == ticketId && n.RideId == rideId).FirstOrDefault();
            var validFastPassList = db.FastPass.Where(n => n.Time >= DateTime.Now.ToLocalTime() && !n.RedeemedTime.HasValue).ToList();
            var expiredFastPassList = db.FastPass.Where(n => n.Time < DateTime.Now.ToLocalTime() || n.RedeemedTime.HasValue).ToList();
            string contentMessage;
            
            //Valid FastPass
            if(validFastPassList.Any(n => n.Id == fastPass.Id))
            {
                var rideDesc = db.Rides.Where(n => n.RideId == fastPass.RideId).Select(n => n.RideDescription).FirstOrDefault();
                contentMessage = "Success redeeming FastPass for " + rideDesc;
                fastPass.RedeemedTime = DateTime.Now;
                db.Entry(fastPass);
                db.SaveChanges();
                return new ContentResult() { Content = contentMessage, StatusCode = 200};
            }
            //expired FastPass
            else if(fastPass != null && expiredFastPassList.Any(n => n.Id == fastPass.Id))
            {
                var rideDesc = db.Rides.Where(n => n.RideId == fastPass.RideId).Select(n => n.RideDescription).FirstOrDefault();
                contentMessage = "Your FastPass for " + rideDesc + " has expired.";
                return new ContentResult() { Content = contentMessage, StatusCode = 400};
            }
            //Given ticketId does not have a valid fastpass for given rideId
            else
            {
                return new ContentResult() { Content = "No valid FastPass for given TicketId and RideId", StatusCode = 404};
            }
        }

        // [Route("report"), HttpGet]
        // public JsonResult GetFastPassReport()
        // {
        //     var fastPassList = db.FastPass.ToList();
        //     List<FastPassReport> reportList = new List<FastPassReport>();
        // }
        

    }
}