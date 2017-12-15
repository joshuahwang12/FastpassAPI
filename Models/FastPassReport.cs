using System;
using System.Collections.Generic;

namespace FastpassAPI.Models
{
    public class FastPassReport
    {
        public string RideDescription {get;set;}
        public List<FastPassReportObject> TimeIntervals {get;set;}
    }
}