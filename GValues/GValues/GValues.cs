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

        public static string connString =
            @"Data Source=HPPC\SQL2008EXPRESS;Initial Catalog=sbh;User ID=sa;Password=74563";

        public static bool IsUseAnimation = false;

        //public static string connString =
        //    @"Data Source=192.168.1.104\SQLMAIN_SBS;Initial Catalog=sbh;User ID=sa;Password=74563";

    }
}
