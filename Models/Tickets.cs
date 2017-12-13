using System;
using System.Collections.Generic;

namespace FastpassAPI.Models
{
    public partial class Tickets
    {
        public int TicketId { get; set; }
        public int RideId { get; set; }
        public DateTime? FastpassTime { get; set; }
    }
}
