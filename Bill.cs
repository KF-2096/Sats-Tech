using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Variedades
{
    public class Bill
    {
        public int BillNumber { get; set; } 
        public float BillAmount { get; set; } 
        public String CustomerSID { get; set; }
        public String CustomerMobile { get; set; }
        public String CustomerType { get; set; }
        public DateTime BillDate { get; set; } 
        public String PackageDesc { get; set; }
        public float PackageAmt { get; set; }
        public String AddOnDesc { get; set; }
        public float AddOnAmt { get; set; }
        public String ExtraChargeDesc { get; set; }
        public float ExtraChargeAmt { get; set; }
    }
}
