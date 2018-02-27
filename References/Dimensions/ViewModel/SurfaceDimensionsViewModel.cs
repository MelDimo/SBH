using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace com.sbh.gui.references.dimensions.ViewModel
{
    public class SurfaceDimensionsViewModel : INotifyPropertyChanged
    {

        private dll.utilites.OReferences.RefDimensions.Dimension _curDimension;
        public dll.utilites.OReferences.RefDimensions.Dimension CurDimension
        {
            get { return _curDimension; }
            set
            {
                _curDimension = value;
                OnPropertyChanged("CurDimension");
            }
        }

        public List<dll.utilites.OReferences.RefDimensions.Dimension> Dimensions { get; private set; }

        public SurfaceDimensionsViewModel()
        {
            DialogView_BackOnClickCommand = new dll.utilites.DelegateCommand(DialogView_BackOnClick);
            DialogView_SaveOnClickCommand = new dll.utilites.DelegateCommand(DialogView_SaveOnClick);

            Dimensions = dll.utilites.OReferences.RefDimensions.GetInstance.refDimension;
        }

        #region External dialog Button overview

        public ICommand DialogView_BackOnClickCommand { get; private set; }
        void DialogView_BackOnClick(object obj)
        {
            ((Window)obj).DialogResult = false;
        }

        public ICommand DialogView_SaveOnClickCommand { get; private set; }
        void DialogView_SaveOnClick(object obj)
        {
            ((Window)obj).DialogResult = true;
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
