using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.dll.utilites.Interfaces
{
    public interface IDocument
    {
        decimal Id { get; }
        decimal ParentId { get; }
        string DocTypeName { get; }
        DateTime DateDoc { get; }
        string XToName { get; }
        string XFromName { get; }
    }
}
