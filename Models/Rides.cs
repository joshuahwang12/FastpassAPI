using System;
using System.Collections.Generic;

namespace FastpassAPI.Models
{
    public partial class Rides
    {
        public int Id { get; set; }
        public int RideId { get; set; }
        public string RideDescription { get; set; }
        public int? QueueTime { get; set; }
    }
}
