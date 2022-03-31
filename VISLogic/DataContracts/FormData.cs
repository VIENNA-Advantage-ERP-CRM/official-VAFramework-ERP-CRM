using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIS.DataContracts
{
    /// <summary>
    /// conatain Form 's Data
    ///- stroe form data as class proerties against AD_Form columns
    ///- 
    /// </summary>
    public class FormDataOut
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public String Help { get; set; }
        public String ClassName { get; set; }
        public int AD_Form_ID { get; set; }

        public bool IsError
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }
        public bool IsReport { get; set; }
        public string DisplayName { get; set; }
        public string Prefix { get; set; }
    }
}