﻿using com.sbh.dll.utilites.OReferences;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.invoices.Model
{
    public class Document : INotifyPropertyChanged
    {
        public decimal id { get; set; }
        public decimal docType { get; set; }
        public DateTime dateCreate { get; set; }
        public DateTime dateDoc { get; set; }
        public decimal refStatus { get; set; }

        public Document()
        {
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
