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
using System.Windows.Controls;
using System.Windows.Input;

namespace com.sbh.gui.references.counterparty.ViewModel
{
    public class UCCounterpartyViewModel : INotifyPropertyChanged
    {

        public View.CounterpartyView CounterpartyView { get; private set; }

        public UCCounterpartyViewModel()
        {
            Counterparty = Model.Counterparty.CollectCounterParty();
            AddCounterpartyCommand = new DelegateCommand(AddCounterparty);
            SelectCounterpartyCommand = new DelegateCommand(SelectCounterparty);
            CounterpartyView = new View.CounterpartyView();
        }

        private Model.Counterparty _currCounterparty;
        public Model.Counterparty currCounterparty
        {
            get { return _currCounterparty; }
            set
            {
                _currCounterparty = value;
                OnPropertyChanged("currCounterparty");
            }
        }

        private ObservableCollection<Model.Counterparty> _conterParty;
        public ObservableCollection<Model.Counterparty> Counterparty
        {
            get { return _conterParty; }
            set
            {
                _conterParty = value;
                OnPropertyChanged("Counterparty");
            }
        }

        public ICommand AddCounterpartyCommand { get; private set; }
        void AddCounterparty(object obj)
        {
            decimal recId;
            Model.Counterparty newCounterparty = new Model.Counterparty()
            { id = 0, name = "- Новый контрагент -", description = "- Новый контрагент -", groupname = "- Новая группа -", refStatus = 1 };

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " INSERT INTO counterparty(name, [description], groupname, ref_status) VALUES(@name, @description, @groupname, @ref_status); " +
                                            " SELECT SCOPE_IDENTITY();";

                    command.Parameters.Add("name", SqlDbType.NVarChar).Value = newCounterparty.name;
                    command.Parameters.Add("description", SqlDbType.NVarChar).Value = newCounterparty.description;
                    command.Parameters.Add("groupname", SqlDbType.NVarChar).Value = newCounterparty.groupname;
                    command.Parameters.Add("ref_status", SqlDbType.NVarChar).Value = newCounterparty.refStatus;

                    recId = (decimal)command.ExecuteScalar();
                }
            }

            Counterparty.Add(newCounterparty);
        }

        public ICommand SelectCounterpartyCommand { get; private set; }
        void SelectCounterparty(object obj)
        {
            return;
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
