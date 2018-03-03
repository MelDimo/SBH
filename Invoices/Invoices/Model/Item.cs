using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.invoices.Model
{
    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public string groupname { get; set; }
        public string Name { get { return groupname; } }
        public decimal count { get; set; }
        public decimal price { get; set; }
    }
}
