using com.sbh.dll.utilites;
using com.sbh.dll.utilites.OReferences;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace com.sbh.gui.references.orgmodel.ViewModel
{
    public class UnitExternalViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SelectableItemWrapper<RefRecipient.Recipient>> _recipients;
        public ObservableCollection<SelectableItemWrapper<RefRecipient.Recipient>> Recipients
        {
            get { return _recipients; }
            set { _recipients = value; OnPropertyChanged("Recipients"); }
        }

        public ObservableCollection<RefRecipient.Recipient> GetSelectedItems()
        {
            var selected = Recipients
                .Where(p => p.IsSelected)
                .Select(p => p.Item)
                .ToList();
            return new ObservableCollection<RefRecipient.Recipient>(selected);
        }

        public UnitExternalViewModel()
        {
            Recipients = new ObservableCollection<SelectableItemWrapper<RefRecipient.Recipient>>();

            RefRecipient refRecipient = RefRecipient.GetInstance;

            foreach (RefRecipient.Recipient item in refRecipient.Recipients)
            {
                Recipients.Add(new SelectableItemWrapper<RefRecipient.Recipient>() { IsSelected = true, Item = item });
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
