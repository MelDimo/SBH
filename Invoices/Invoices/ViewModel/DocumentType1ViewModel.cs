using com.sbh.dll.utilites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace com.sbh.gui.invoices.ViewModel
{
    public class DocumentType1ViewModel : INotifyPropertyChanged
    {

        dll.resdictionary.View.DialogView dialogView;

        public DocumentType1ViewModel()
        {
            AddOnClickCommand = new DelegateCommand(AddOnClick);

            BackOnClickCommand = new DelegateCommand(BackOnClick);

            DialogView_BackOnClickCommand = new DelegateCommand(DialogView_BackOnClick);
            DialogView_SaveOnClickCommand = new DelegateCommand(DialogView_SaveOnClick);
        }

        public ICommand AddOnClickCommand { get; private set; }
        void AddOnClick(object obj)
        {
            dialogView = new dll.resdictionary.View.DialogView()
            {
                DataContext = this
            };
            dialogView.ShowDialog();
        }

        public ICommand BackOnClickCommand { get; private set; }
        void BackOnClick(object obj)
        {
            SurfaceControlViewModel.BackOnClickCommand.Execute(null);
        }

        public ICommand DialogView_SaveOnClickCommand { get; private set; }
        void DialogView_SaveOnClick(object obj)
        {
            dialogView.Close();
        }

        public ICommand DialogView_BackOnClickCommand { get; private set; }
        void DialogView_BackOnClick(object obj)
        {
            dialogView.Close();
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
