using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using com.sbh.dll.utilites;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    public class UnitViewModel : DependencyObject, IViewModel, INotifyPropertyChanged
    {
        //private invoices.DTO.DataModel dataModel;

        private invoices.ViewModel.SurfaceControlViewModel surfaceControlViewModel;
        public invoices.ViewModel.SurfaceControlViewModel SurfaceControlViewModel
        {
            get { return surfaceControlViewModel; }
            set { surfaceControlViewModel = value; OnPropertyChanged(); }
        }

        private DBAccess dbAccess = new DBAccess();

        private bool showLoader;
        public bool ShowLoader
        {
            get { return showLoader; }
            set { showLoader = value; OnPropertyChanged(); }
        }

        public ICollectionView CollectionItemBalansView
        {
            get { return (ICollectionView)GetValue(CollectionItemBalansProperty); }
            set { SetValue(CollectionItemBalansProperty, value); }
        }


        #region Фильтр наименования позиции

        public static readonly DependencyProperty CollectionItemBalansProperty =
           DependencyProperty.Register("CollectionItemBalansView", typeof(ICollectionView), typeof(UnitViewModel), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for FilterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register("FilterText", typeof(string), typeof(UnitViewModel), new PropertyMetadata("", FilterText_Changed));

        private static void FilterText_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UnitViewModel current)
            {
                current.CollectionItemBalansView.Filter = null;
                current.CollectionItemBalansView.Filter = current.FilterItem;
            }
        }

        private bool FilterItem(object obj)
        {
            bool result = true;
            Model.Item current = obj as Model.Item;
            if (!string.IsNullOrWhiteSpace(FilterText) && current != null && !current.Name.ToUpper().Contains(FilterText.ToUpper()))
            {
                result = false;
            }
            return result;
        }

        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        #endregion

        public UnitViewModel()
        {
            BaseViewModel = BaseViewModel.GetInstance;

            IsBackBtnEnable = true;

            ViewHeader = string.Format("{0} / {1} / {2}", BaseViewModel.CurrentUnitEx.OrgName, BaseViewModel.CurrentUnitEx.BranchName, BaseViewModel.CurrentUnitEx.UnitName);

            TabSelectedIndex = 0;
        }

        private int tabSelectedIndex;
        public int TabSelectedIndex
        {
            get { return tabSelectedIndex; }
            set
            {
                tabSelectedIndex = value;
                switch (tabSelectedIndex)
                {
                    case 0:
                        CollectUnitItemsBalansAsync();
                        break;
                        
                    case 1:
                        CollectDocumentHistory();
                        break;
                }
                OnPropertyChanged();
            }
        }

        private async void CollectUnitItemsBalansAsync()
        {
            ShowLoader = true;

            await Task.Run(() =>
            {

                Msg = dbAccess.CollectUnitItemsBalans(BaseViewModel.CurrentUnitEx.UnitId);
                if (Msg.IsSuccess)
                {
                    BaseViewModel.CollectionItemBalans = msg.Obj as ObservableCollection<Model.Item>;
                    if (BaseViewModel.CollectionItemBalans.Count == 0) Msg.Message = "Нет данных!";
                }
                else { Msg.Message = String.Format("Ошибка получения данных:\n{0}", Msg.Message); return; }
            });

            ShowLoader = false;

            if (Msg.IsSuccess)
            {
                CollectionItemBalansView = CollectionViewSource.GetDefaultView(BaseViewModel.CollectionItemBalans);
                CollectionItemBalansView.Filter = FilterItem;
            }
        }

        private async void CollectDocumentHistory()
        {
            SurfaceControlViewModel = new invoices.ViewModel.SurfaceControlViewModel();

            await Task.Delay(1000);
        }


        #region IViewModel Members

        public bool IsBackBtnEnable { get; set; }
        public string ViewHeader { get; set; }
        public BaseViewModel BaseViewModel { get; set; }
        private MSG msg;
        public MSG Msg
        {
            get { return msg; }
            set { msg = value; OnPropertyChanged(); }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
