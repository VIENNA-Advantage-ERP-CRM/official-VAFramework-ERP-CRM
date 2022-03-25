using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIS.Models
{
    public class ShortcutItemModel
    {
        public String Url
        {
            get;
            set;
        }

        public bool HasChild
        {
            get;
            set;
        }

        /// <summary>
        /// entity Action Of Item ('W','P' etc)
        /// </summary>
        /// 
        public String Action
        {
            get;
            set;
        }

        /// <summary>
        /// Key Id Of Action
        /// </summary>
        public int ActionID
        {
            get;
            set;
        }

        public int DynamicActionID
        {
            get;
            set;
        }

        public String ActionName
        {
            get;
            set;
        }

        /// <summary>
        /// class name of form to open
        /// </summary>
        public String SpecialAction
        {
            get;
            set;
        }

        public int KeyID
        {
            get;
            set;
        }

        public String IconUrl
        {
            get;
            set;
        }

        public byte[] IconBytes
        {
            get;
            set;
        }

        public bool HasImage
        {
            get;
            set;
        }

        public bool IsImageByteArray
        {
            get;
            set;
        }

        public string ShortcutName
        {
            get;
            set;
        }
        public bool IsSelected
        { get; set; }



        // public ShortcutItemParamModel ItemParam { get; set; }

    }

    public class ShortcutItemParamModel
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public bool IsEncrypted { get; set; }
    }
}