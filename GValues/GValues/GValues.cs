using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace com.sbh.dll
{
    public static class GValues
    {
        public static string AppNameFull = "SmartBuissinesHelper";
        public static string AppNameShort = "SBH";

        public static Window MainWindow { get; set; }

        //public static string connString = 
        //    @"Data Source=127.0.0.1\SQLMAIN;Initial Catalog=sbh;User ID=sa;Password=Q1234567";

        public static string connString =
            @"Data Source=192.168.1.104\SQLMAIN_SBS;Initial Catalog=sbh;User ID=sa;Password=74563";

    }
}
