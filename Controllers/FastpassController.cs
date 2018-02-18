using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FastpassAPI.Models;
using System.Linq;
using System;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Globalization;

namespace FastpassAPI.Controllers
{
    [Route("[controller]")]
    public class FastpassController : Controller
    {

        private readonly FastPassApiContext db;
        
    
        public FastpassController(FastPassApiContext context)
        {
            db = context;

        }

        [HttpPost]
        [Route("ticketid={ticketId}&rideid={rideId}")]
        public async Task<IActionResult> AddFastPass(int ticketId, int rideId)
        {    
            //var validFastPass = db.FastPass.Where(n => n.TicketId == ticketId && n.Time >= DateTime.Now.ToLocalTime()).FirstOrDefault();
            var validFastPass = db.FastPass.Where(n => n.TicketId == ticketId && n.FastPassFlag == true).FirstOrDefault();
            
            if(validFastPass != null)
            {
                 return new ContentResult() { Content = "Not Valid", StatusCode = 400};
            }
            else
            {
                string contentMessage;
                FastPass newFp = new FastPass() {
                    TicketId = ticketId,
                    RideId = rideId,
                    FastPassFlag = true
                };
                // newFp.TicketId = ticketId;
                // newFp.RideId = rideId;
                // newFp.FastPassFlag = true;

                newFp.Time = DateTime.Now.AddHours(1);
                await db.AddAsync(newFp);
                await db.SaveChangesAsync();
                
                contentMessage = "Please return at " + DateTime.Now.AddMinutes(30) + ". It will expire at " + DateTime.Now.AddHours(1);

                return new ContentResult() { Content = contentMessage, StatusCode = 201 };
                    
            } 
        }

        [Route("ticketId={ticketId}"), HttpGet]
        public async Task<IActionResult> GetFastPass(int ticketId)
        {
            var fastpass = db.FastPass.Where(n => n.TicketId == ticketId && n.Time >= DateTime.Now.ToLocalTime()).FirstOrDefault();
            string contentMessage;
            if(fastpass != null)
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
        public async Task<IActionResult> redeem (int ticketId, int rideId)
        {
            var fastPass = db.FastPass.Where(n => n.TicketId == ticketId && n.RideId == rideId).FirstOrDefault();
            var fastPassList = db.FastPass.Where(n => n.Time >= DateTime.Now.ToLocalTime()).ToList();
            var expiredFastPassList = db.FastPass.Where(n => n.Time < DateTime.Now.ToLocalTime() || n.RedeemedTime.HasValue).ToList();
            string contentMessage;
            
            //Valid FastPass
            if(fastPassList.Any((n => n.Id == fastPass.Id && !n.RedeemedTime.HasValue || n.MasterFastPass)))
            {
                var rideDesc = db.Rides.Where(n => n.RideId == fastPass.RideId).Select(n => n.RideDescription).FirstOrDefault();
                contentMessage = "Success redeeming FastPass for " + rideDesc;
                fastPass.RedeemedTime = DateTime.Now;
                db.Entry(fastPass);
                await db.SaveChangesAsync();
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

        [Route("report"), HttpGet]
        public JsonResult GetFastPassReport()
        {
            var ridesList = db.Rides.ToList();

            List<FastPassReport> reportList = new List<FastPassReport>();
            foreach(var ride in ridesList)
            {
                List<FastPassReportObject> fastPassList = new List<FastPassReportObject>();

                for(int i = 0; i < 48; i++)
                {
                    DateTime today = DateTime.Now.Date;
                    DateTime start = today.AddMinutes(i * 30);
                    DateTime end = today.AddMinutes((i + 1) * 30);

                    int fps = db.FastPass.Where(n => n.RedeemedTime >= start && n.RedeemedTime <= end).Count();
                    FastPassReportObject temp = new FastPassReportObject
                    {
                        TimeIntervals = start.ToShortTimeString() + "-" + end.ToShortTimeString(),
                        RedeemedFastPassCount = fps
                    };
                    fastPassList.Add(temp);
                }
                reportList.Add(new FastPassReport {
                    RideDescription = ride.RideDescription,
                    TimeIntervals = fastPassList,                    
                });
            }
            return Json(reportList);
            
        }           
                
    }
}