using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.invoices.Model
{
    public class DocumentType1
    {
        public Document doc;
        public dll.utilites.OReferences.RefCounterParty.Counterparty counterpaty;
        public dll.utilites.OReferences.RefRecipient.Recipient recipient;

        public DocumentType1()
        {
            doc = new Document();
            counterpaty = new dll.utilites.OReferences.RefCounterParty.Counterparty();
            recipient = new dll.utilites.OReferences.RefRecipient.Recipient();
        }
    }
}
