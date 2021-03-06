﻿using com.sbh.dll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace com.sbh.gui.invoices.Model
{
    public class Position : INotifyPropertyChanged
    {
        // Изменяю свойство после формирования списка объектов
        public bool isAvalForEdit = false;

        // Сохраняю последнюю валюту как дефолтную для последующего добавления позиции
        public static decimal lastCurrency;

        public decimal id { get; set; }

        private int _itemId;
        public int itemId
        {
            get { return _itemId; }
            set
            {
                if (_itemId != value && isAvalForEdit)
                    using (SqlConnection con = new SqlConnection(GValues.connString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = con;
                            command.CommandText = " UPDATE document_items SET item = @item WHERE id = @id; ";

                            command.Parameters.Add("id", SqlDbType.Int).Value = id;
                            command.Parameters.Add("item", SqlDbType.Int).Value = value;

                            command.ExecuteNonQuery();
                        }

                        dimensionId = dll.utilites.OReferences.RefItem.GetInstance.refItem.SingleOrDefault(x => x.id == value).refDimensions;
                    }
                _itemId = value;
                OnPropertyChanged("itemId");
            }
        }

        private int _dimensionId;
        public int dimensionId
        {
            get { return _dimensionId; }
            set
            {
                if (_dimensionId != value && isAvalForEdit)
                    using (SqlConnection con = new SqlConnection(GValues.connString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = con;
                            command.CommandText = " UPDATE document_items SET ref_dimensions = @dimensionId WHERE id = @id; ";

                            command.Parameters.Add("id", SqlDbType.Int).Value = id;
                            command.Parameters.Add("dimensionId", SqlDbType.Int).Value = value;

                            command.ExecuteNonQuery();
                        }
                    }
                _dimensionId = value;
                OnPropertyChanged("dimensionId");
            }
        }

        private decimal _xcount;
        public decimal xcount
        {
            get { return _xcount; }
            set
            {
                if (_xcount != value && isAvalForEdit)
                    using (SqlConnection con = new SqlConnection(GValues.connString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = con;
                            command.CommandText = " UPDATE document_items SET xcount = @xcount WHERE id = @id; ";

                            command.Parameters.Add("id", SqlDbType.Int).Value = id;
                            command.Parameters.Add("xcount", SqlDbType.Int).Value = value;

                            command.ExecuteNonQuery();
                        }
                    }
                _xcount = value;
                OnPropertyChanged("xsumm");
            }
        }

        public decimal xcountAval { get; set; }

        private decimal _currencyId;
        public decimal currencyId
        {
            get { return _currencyId; }
            set
            {
                if (_currencyId != value && isAvalForEdit)
                    using (SqlConnection con = new SqlConnection(GValues.connString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = con;
                            command.CommandText = " UPDATE document_items SET currency = @currency WHERE id = @id; ";

                            command.Parameters.Add("id", SqlDbType.Int).Value = id;
                            command.Parameters.Add("currency", SqlDbType.Int).Value = value;

                            command.ExecuteNonQuery();
                        }
                    }

                _currencyId = value;
                lastCurrency = value;

                OnPropertyChanged("currencyId");
                
            }
        }

        private decimal _xprice;
        public decimal xprice
        {
            get { return _xprice; }
            set
            {
                if (_xcount != value && isAvalForEdit)
                    using (SqlConnection con = new SqlConnection(GValues.connString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = con;
                            command.CommandText = " UPDATE document_items SET xprice = @xprice WHERE id = @id; ";

                            command.Parameters.Add("id", SqlDbType.Int).Value = id;
                            command.Parameters.Add("xprice", SqlDbType.Int).Value = value;

                            command.ExecuteNonQuery();
                        }
                    }
                _xprice = value;
                OnPropertyChanged("xsumm");
            }
        }

        [XmlIgnore]
        public decimal xsumm
        {
            get { return Math.Round(xcount * xprice, 3); }
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
