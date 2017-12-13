using System;

namespace FastpassAPI.Models
{
    public class Ticket
    {
        public int TicketId {get;set;}
        public int RideId {get;set;}

        public DateTime FastpassTime {get;set;}
    }
}