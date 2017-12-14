using System;

namespace FastpassAPI.Models
{
    public class FastPassReport
    {
        public string RideDescription {get;set;}
        public DateTime TimeIntervals {get;set;}
        public int RedeemedCount {get;set;}
    }
}