using com.sbh.dll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace com.sbh.gui.references.item.ViewModel
{
    public class SurfaceItemViewModel : INotifyPropertyChanged
    {

        public View.ItemView itemView { get; private set; }

        private Model.Item _curItem;
        public Model.Item CurItem
        {
            get { return _curItem; }
            set
            {
                _curItem = value;
                OnPropertyChanged("GroupName");
                OnPropertyChanged("CurItem");
            }
        }

        private ObservableCollection<Model.Item> _items;
        public ObservableCollection<Model.Item> Items
        {
            get { return _items; }
            private set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public List<string> GroupName
        {
            get
            {
                return _items.OrderBy(x => x.groupname).Select(x => x.groupname).Distinct().ToList();
            }
        }

        public SurfaceItemViewModel()
        {
            AddItemCommand = new dll.utilites.DelegateCommand(AddItem);
            collectItems();
            itemView = new View.ItemView();
        }

        private void collectItems()
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT id, name, groupname, ref_status AS refStatus, ref_dimensions AS refDimensions" +
                                            " FROM item " +
                                            " FOR XML RAW('Item'), ROOT('ArrayOfItem'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        Items = dll.utilites.Support.XMLToObject<ObservableCollection<Model.Item>>(reader.ReadOuterXml());
                    }
                }
            }

            if (Items == null) Items = new ObservableCollection<Model.Item>();
        }

        public ICommand AddItemCommand { get; private set; }
        void AddItem(object obj)
        {
            decimal recId;
            Model.Item newItem = new Model.Item()
            { id = 0, name = "- Новый товар -", groupname = "- Новая группа -", refStatus = 1, refDimensions = 1 };

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " INSERT INTO item(name, groupname, ref_status, ref_dimensions) VALUES(@name, @groupname, @ref_status, @ref_dimensions); " +
                                            " SELECT SCOPE_IDENTITY();";

                    command.Parameters.Add("name", SqlDbType.NVarChar).Value = newItem.name;
                    command.Parameters.Add("groupname", SqlDbType.NVarChar).Value = newItem.groupname;
                    command.Parameters.Add("ref_status", SqlDbType.Int).Value = newItem.refStatus;
                    command.Parameters.Add("ref_dimensions", SqlDbType.Int).Value = newItem.refDimensions;

                    recId = (decimal)command.ExecuteScalar();
                }
            }

            Items.Add(newItem);
        }




        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
