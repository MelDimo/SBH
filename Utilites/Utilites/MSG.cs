using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.dll.utilites
{
    public class MSG : INotifyPropertyChanged
    {
        /// <summary>
        /// Код
        /// </summary>
        private int code;
        public int Code { get { return code; } set { code = value; OnPropertyChanged(); } }
        /// <summary>
        /// Признак успеха
        /// </summary>
        private bool isSuccess;
        public bool IsSuccess { get { return isSuccess; } set { isSuccess = value; OnPropertyChanged(); } }
        /// <summary>
        /// Сообщение
        /// </summary>
        private string message;
        public string Message { get { return message; } set { message = value; OnPropertyChanged(); } }
        /// <summary>
        /// Объект
        /// </summary>
        private object obj;
        public object Obj { get { return obj; } set { obj = value; OnPropertyChanged(); } }

        public MSG() { isSuccess = false; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
