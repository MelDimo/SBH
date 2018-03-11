using com.sbh.dll.utilites.OReferences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.invoices.Model.DocAbstractFactory.Header
{
    interface IHeader
    {
        decimal id { get; set; }
        decimal docType { get; set; }
        DateTime dateCreate { get; set; }
        decimal refStatus { get; set; }

        RefCounterParty.Counterparty counterpaty { get; set; }
        RefRecipient.Recipient recipient { get; set; }
    }
}
