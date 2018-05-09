using com.sbh.dll;
using com.sbh.dll.utilites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace com.sbh.gui.unitsbrowser
{
    public class DBAccess
    {
        private MSG oMsg;

        /// <summary>
        /// Собираем все доступные конечные точки
        /// </summary>
        /// <returns>объект MSG</returns>
        public MSG CollectUnitEx()
        {
            oMsg = new MSG() { IsSuccess = true };
            ObservableCollection<Model.Unit> result = new ObservableCollection<Model.Unit>();

            try
            {
                using (SqlConnection con = new SqlConnection(GValues.connString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = con;
                        command.CommandText = " SELECT u.id AS UnitId, " +
                                            "   u.name AS UnitName, " +
                                            "   b.name AS BranchName, " +
                                            "	o.name AS OrgName " +
                                            " FROM unit u " +
                                            " INNER JOIN branch b ON b.id = u.branch " +
                                            " INNER JOIN organization o  ON o.id = b.organization " +
                                            " WHERE u.ref_status = 1 " +
                                            " ORDER BY o.name, b.name, u.name " +
                                            " FOR XML RAW('Unit'), ROOT('ArrayOfUnit'), ELEMENTS ";

                        XmlReader reader = command.ExecuteXmlReader();
                        while (reader.Read())
                        {
                            result = Support.XMLToObject<ObservableCollection<Model.Unit>>(reader.ReadOuterXml());
                        }
                        oMsg.Obj = result;
                    }
                }
            }
            catch (Exception exc)
            {
                oMsg.IsSuccess = false;
                oMsg.Message = exc.Message;
            }

            return oMsg;
        }

        /// <summary>
        /// Собираем текущие остатки конечной точки
        /// </summary>
        /// <param name="pUnitId">id конечной точки</param>
        /// <returns>объект MSG</returns>
        public MSG CollectUnitItemsBalans(int pUnitId)
        {
            oMsg = new MSG() { IsSuccess = true };
            ObservableCollection<Model.Item> result = new ObservableCollection<Model.Item>();

            try
            {
                using (SqlConnection con = new SqlConnection(GValues.connString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = con;
                        command.CommandText = " SELECT i.name as Name, uib.xcount AS Xcount " +
                                                " FROM unit_item_balans uib" +
                                                " INNER JOIN item i ON i.id = uib.item" +
                                                " WHERE uib.unit = @pUnitId " +
                                                " FOR XML RAW('Item'), ROOT('ArrayOfItem'), ELEMENTS";

                        command.Parameters.Add("pUnitId", SqlDbType.Int).Value = pUnitId;

                        XmlReader reader = command.ExecuteXmlReader();
                        while (reader.Read())
                        {
                            result = Support.XMLToObject<ObservableCollection<Model.Item>>(reader.ReadOuterXml());
                        }
                        oMsg.Obj = result;
                    }
                }
            }
            catch (Exception exc)
            {
                oMsg.IsSuccess = false;
                oMsg.Message = exc.Message;
            }

            return oMsg;
        }
    }
}
