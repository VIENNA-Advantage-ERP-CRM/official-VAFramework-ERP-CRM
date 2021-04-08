using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIS.DataContracts
{
    public class ThemeData
    {
        public string Primary { get; set; }
        public string OnPrimary { get; set; }
        public string Seconadary { get; set; }
        public string OnSecondary { get; set; }
        public bool IsDefault { get; set; }
        public int Id { get; set; }

        public string Name { get; set; }

    }
}