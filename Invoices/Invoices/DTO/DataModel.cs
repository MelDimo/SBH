using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.invoices.DTO
{
    public class DataModel : INotifyPropertyChanged
    {
        public Model.Filter Filter { get; set; }

        private ObservableCollection<Model.Document> _documents;
        public ObservableCollection<Model.Document> Documents
        {
            get { return _documents; }
            set { _documents = value; OnPropertyChanged("Documents"); }
        }

        private Model.Document _curDocument;
        public Model.Document CurDocument
        {
            get { return _curDocument; }
            set
            {
                if (_curDocument != null)
                    foreach (Model.Position pos in _curDocument.DocumentPositions) pos.isAvalForEdit = false;

                _curDocument = value;

                foreach (Model.Position pos in _curDocument.DocumentPositions) pos.isAvalForEdit = true;

                OnPropertyChanged("CurDocument");
            }
        }

        public DataModel()
        {
            Filter = new Model.Filter();
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
