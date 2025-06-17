using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory_Sys.Models
{
    public class AssignmentViewModel
    {
        public int AssignmentID { get; set; }
        public string DeviceSerialNumber { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedBy { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Returned { get; set; }
        public string Notes { get; set; }
    }

}