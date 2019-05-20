using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;


namespace VAdvantage.Model
{
    public class X_Ref_Quantity_Type
    {
        #region Private Variables
        public static int AD_Reference_ID = 533;
        private String value;
        // Allocated = A 
        public const string ALLOCATED = "A";
        // Dedicated = D 
        public const string DEDICATED = "D";
        // Expected = E 
        public const string EXPECTED = "E";
        // On Hand = H 
        public const string ON_HAND = "H";
        // Ordered = O 
        public const string ORDERED = "O";
        // Reserved = R 
        public const string RESERVED = "R";
        #endregion

        public static List<String> Get()
        {
            List<String> list = new List<String>();
            list.Add(ALLOCATED);
            list.Add(DEDICATED);
            list.Add(EXPECTED);
            list.Add(ON_HAND);
            list.Add(ORDERED);
            list.Add(RESERVED);
            return list;
        }

        private X_Ref_Quantity_Type(String value)
        {
            this.value = value;

        }

        public String GetValue()
        {
            return this.value;

        }

        public static Boolean IsValid(String test)
        {
            foreach (var v in X_Ref_Quantity_Type.Get())
            {
                if (v.ToString().Equals(test))
                {
                    return true;
                }
            }
            return false;

            /*
             Array values = Enum.GetValues(typeof(X_Ref_Quantity_Type));
            for (int i = 0; i < values.Length; i++)
            {
                if (values.GetValue(i).ToString() == test)
                {
                    return true;
                }
            }
            return false;
             */
        }

        public static String GetEnum(String value)
        {
            foreach (var v in X_Ref_Quantity_Type.Get())
            {
                if (v.ToString().Equals(value))
                {
                    return v;
                }
            }
            return null;

            /*
             Array values = Enum.GetValues(typeof(X_Ref_Quantity_Type));
            for (int i = 0; i < values.Length; i++)
            {
                if (values.GetValue(i).ToString() == test)
                {
                    return X_Ref_Quantity_TypeEnum;
                }
            }
            return null;
             */
        }
    }
}
