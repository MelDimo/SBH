﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.references.orgmodel.Model
{
    public class Branch
    {
        public int id { get; set; }
        public string name { get; set; }
        public int refStatus { get; set; }
        public ObservableCollection<Unit> Units { get; set; }
    }
}
