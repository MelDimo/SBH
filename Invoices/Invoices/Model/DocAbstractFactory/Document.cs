using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.invoices.Model.DocAbstractFactory
{
    class Document
    {
        private Header.IHeader header;
        private Items.IItems items;

        public Document(Factory.IFactory factory)
        {
            header = factory.Createheader();
            items = factory.CreateItems();
        }
    }
}
