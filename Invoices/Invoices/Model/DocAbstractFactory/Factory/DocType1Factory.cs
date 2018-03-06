using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.invoices.Model.DocAbstractFactory.Factory
{
    class DocType1Factory : IFactory
    {
        public Header.IHeader Createheader()
        {
            return new Header.DocType1Header();
        }

        public Items.IItems CreateItems()
        {
            throw new NotImplementedException();
        }
    }
}
