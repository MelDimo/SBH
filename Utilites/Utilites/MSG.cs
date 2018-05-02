using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.dll.utilites
{
    public class MSG
    {
        /// <summary>
        /// Код
        /// </summary>
        public int Code;
        /// <summary>
        /// Признак успеха
        /// </summary>
        public bool IsSuccess;
        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message;
        /// <summary>
        /// Объект
        /// </summary>
        public object Obj;

        public MSG()
        { }
    }
}
