using com.sbh.dll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace com.sbh.gui.references.item.Model
{
    public class Item : INotifyPropertyChanged
    {
        private dimensions.SurfaceDimensionsView surfaceDimensionsView = new dimensions.SurfaceDimensionsView();
        private dll.utilites.OReferences.RefDimensions RefDimensions = dll.utilites.OReferences.RefDimensions.GetInstance;

        public decimal id { get; set; }

        private string _groupname;
        public string groupname
        {
            get { return _groupname; }
            set
            {
                _groupname = value;
                OnPropertyChanged("groupname");
            }
        }

        // Это пиздец, но наименование группы отображается только если свойство именуется как 'Name' O_o
        public string Name { get { return groupname; } }

        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }

        private int _refStatus;
        public int refStatus
        {
            get { return _refStatus; }
            set
            {
                _refStatus = value;
                OnPropertyChanged("refStatus");
            }
        }

        private int _refDimensions;
        public int refDimensions
        {
            get { return _refDimensions; }
            set
            {
                _refDimensions = value;
                OnPropertyChanged("refDimensionsName");
            }
        }

        [XmlIgnore]
        public string refDimensionsName
        {
            get { return RefDimensions.refDimension.Where(x => x.id == refDimensions).SingleOrDefault().namefull; }
        }

        public Item()
        {
            ChangeStatusCommand = new dll.utilites.DelegateCommand(ChangeStatus);
            SaveCommand = new dll.utilites.DelegateCommand(Save);
            ChangeDimensionCommand = new dll.utilites.DelegateCommand(ChangeDimension);
        }

        [XmlIgnore]
        public ICommand ChangeStatusCommand { get; private set; }
        void ChangeStatus(object obj)
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " UPDATE item SET ref_status = @refStatus " +
                                            " WHERE id = @id;";

                    command.Parameters.Add("id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("refStatus", SqlDbType.Int).Value = refStatus == 1 ? 2 : 1;

                    command.ExecuteNonQuery();
                }
            }

            refStatus = refStatus == 1 ? 2 : 1;
        }

        [XmlIgnore]
        public ICommand SaveCommand { get; private set; }
        void Save(object obj)
        {
            var values = (object[])obj;
            string xName = values[0] as string;
            string xGroupname = values[1] as string;

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " UPDATE item SET" +
                                            " name = @name, " +
                                            " groupname = @groupname " +
                                            " WHERE id = @id; ";

                    command.Parameters.Add("id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("name", SqlDbType.NVarChar).Value = xName;
                    command.Parameters.Add("groupname", SqlDbType.NVarChar).Value = xGroupname;

                    command.ExecuteNonQuery();
                }
            }

            name = xName;
            groupname = xGroupname;

            dll.utilites.OReferences.RefItem.GetInstance.reload();
        }

        [XmlIgnore]
        public ICommand ChangeDimensionCommand { get; private set; }
        void ChangeDimension(object obj)
        {

            ((dimensions.ViewModel.SurfaceDimensionsViewModel)surfaceDimensionsView.DataContext).CurDimension = 
                RefDimensions.refDimension.Where(x => x.id == refDimensions).SingleOrDefault();

            dll.resdictionary.View.DialogView window = new dll.resdictionary.View.DialogView(surfaceDimensionsView);
            if (window.ShowDialog() == true)
            {
                using (SqlConnection con = new SqlConnection(GValues.connString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = con;
                        command.CommandText = " UPDATE item SET" +
                                                " ref_dimensions = @ref_dimensions " +
                                                " WHERE id = @id; ";

                        command.Parameters.Add("id", SqlDbType.Int).Value = id;
                        command.Parameters.Add("ref_dimensions", SqlDbType.Int).Value = ((dimensions.ViewModel.SurfaceDimensionsViewModel)surfaceDimensionsView.DataContext).CurDimension.id;

                        command.ExecuteNonQuery();
                    }
                }

                refDimensions = ((dimensions.ViewModel.SurfaceDimensionsViewModel)surfaceDimensionsView.DataContext).CurDimension.id;
            }

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
