using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Radus_ConSec.Models
{
    public class QRModel
    {
        public int QRID { get; set; }
        public byte[] QRImage { get; set; }
        public string QRDescription { get; set; }
        public DateTime QRInsertDate { get; set; }
        public DateTime QREditDate { get; set; }
    }
}