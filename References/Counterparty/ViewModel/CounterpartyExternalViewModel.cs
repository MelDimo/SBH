﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using com.sbh.dll.utilites.OReferences;

namespace com.sbh.gui.references.counterparty.ViewModel
{
    public class CounterpartyExternalViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<dll.utilites.SelectableItemWrapper<RefCounterParty.Counterparty>> _counterpartys;
        public ObservableCollection<dll.utilites.SelectableItemWrapper<RefCounterParty.Counterparty>> Counterpartys
        {
            get { return _counterpartys; }
            set { _counterpartys = value; OnPropertyChanged("Counterpartys"); }
        }

        public ObservableCollection<RefCounterParty.Counterparty> GetSelectedItems()
        {
            var selected = Counterpartys
                .Where(p => p.IsSelected)
                .Select(p => p.Item)
                .ToList();
            return new ObservableCollection<RefCounterParty.Counterparty>(selected);
        }

        public CounterpartyExternalViewModel()
        {
            Counterpartys = new ObservableCollection<dll.utilites.SelectableItemWrapper<RefCounterParty.Counterparty>>();

            RefCounterParty RefCounterParty = RefCounterParty.GetInstance;

            foreach (RefCounterParty.Counterparty item in RefCounterParty.CounterPartys)
            {
                Counterpartys.Add(new dll.utilites.SelectableItemWrapper<RefCounterParty.Counterparty>() { IsSelected = true, Item = item });
            }

            DialogView_SaveOnClickCommand = new dll.utilites.DelegateCommand(DialogView_SaveOnClick);
            DialogView_BackOnClickCommand = new dll.utilites.DelegateCommand(DialogView_BackOnClick);
        }


        #region DialogView command

        public ICommand DialogView_SaveOnClickCommand { get; private set; }
        void DialogView_SaveOnClick(object obj)
        {
            ((Window)obj).DialogResult = true;
        }

        public ICommand DialogView_BackOnClickCommand { get; private set; }
        void DialogView_BackOnClick(object obj)
        {
            ((Window)obj).DialogResult = false;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
