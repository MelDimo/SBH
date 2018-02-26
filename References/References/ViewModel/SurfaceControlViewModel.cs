using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace com.sbh.gui.references.ViewModel
{
    public class SurfaceControlViewModel : INotifyPropertyChanged
    {
        private UserControl mCounterparty;
        private UserControl mOrgModel;
        private UserControl mMainView;
        private UserControl mReferenceContainerView;
        private UserControl mItem;

        private UserControl _curReference;
        public UserControl CurReference
        {
            get { return _curReference; }
            private set
            {
                _curReference = value;
                OnPropertyChanged("CurUserControl");
            }
        }

        private UserControl _curUserControl;
        public UserControl CurUserControl
        {
            get { return _curUserControl; }
            private set
            {
                _curUserControl = value;
                OnPropertyChanged("CurUserControl");
            }
        }

        public SurfaceControlViewModel()
        {

            ItemOnClickCommand = new DelegateCommand(ItemOnClick);
            BackOnClickCommand = new DelegateCommand(BackOnClick);

            mCounterparty = new counterparty.UCCounterpartyView();
            mOrgModel = new orgmodel.UCOrgModel();
            mItem = new item.SurfaceItemView();

            mMainView = new View.MainView();

            mReferenceContainerView = new View.ReferenceContainerView();

            CurUserControl = mMainView;
        }

        public ICommand BackOnClickCommand { get; private set; }
        void BackOnClick(object obj)
        {
            CurUserControl = mMainView;
        }

        public ICommand ItemOnClickCommand { get; private set; }
        void ItemOnClick(object obj)
        {
            switch (obj as string)
            {
                case "ORGMODEL":
                    CurReference = mOrgModel;
                    break;

                case "COUNTERPARTY":
                    CurReference = mCounterparty;
                    break;

                case "ITEM":
                    CurReference = mItem;
                    break;
            }

            CurUserControl = mReferenceContainerView;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
