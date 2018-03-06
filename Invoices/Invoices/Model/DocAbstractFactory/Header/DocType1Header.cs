using com.sbh.dll.utilites.OReferences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.invoices.Model.DocAbstractFactory.Header
{
    class DocType1Header : IHeader
    {
        public decimal id { get; set; }
        public decimal docType { get; set; }
        public DateTime dateCreate { get; set; }
        public decimal refStatus { get; set; }

        public RefCounterParty.Counterparty counterpaty;
        public RefRecipient.Recipient recipient;

        public DocType1Header()
        { }
    }
}
