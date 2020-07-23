using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Model
{
    [Serializable]
    public abstract class NamePair : Object, IComparer, IComparer<Object>
    {
        /// <summary>
        /// added so NamePair can be serialized by GWT
        /// </summary>
        public NamePair() { }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="name">(Display) Name of the Pair</param>
        protected NamePair(String name)
        {
            SetName(name);
        }   //  NamePair


        /** The Name        		*/
        private String _name;
        /**	Sort by Name			*/
        private bool _sortByName = true;


        /// <summary>
        /// Returns display value
        /// </summary>
        /// <returns></returns>
        public String GetName()
        {
            return _name;
        }   //  getName

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Set Name
        /// </summary>
        /// <param name="name">name</param>
        public void SetName(String name)
        {
            _name = name;
            if (_name == null)
                _name = "";
        }	//	setName

        /// <summary>
        /// Get Sort By Name
        /// </summary>
        /// <returns>true if by name</returns>
        public bool IsSortByName()
        {
            return _sortByName;
        }	//	isSortByName

        /// <summary>
        /// Set Sort By Name
        /// </summary>
        /// <param name="sortByName">true if by name, false by ID</param>
        public void SetSortByName(bool sortByName)
        {
            _sortByName = sortByName;
        }	//	setSortByName

        /// <summary>
        /// Returns Key or Value as String
        /// </summary>
        /// <returns>String or null</returns>
        public abstract String GetID();

        public abstract object GetKeyID();

        public virtual object Key
        {
            get;
            set;
        }

        /// <summary>
        /// IComparer Interface (based on toString value)
        /// </summary>
        /// <param name="o1">Object 1</param>
        /// <param name="o2">Object 2</param>
        /// <returns>compareTo value</returns>
        public int Compare(Object o1, Object o2)
        {
            String s1 = o1 == null ? "" : o1.ToString();
            String s2 = o2 == null ? "" : o2.ToString();
            return s1.CompareTo(s2);    //  sort order ??
        }

        /// <summary>
        /// IComparer Interface (based on Name/ID value)
        /// </summary>
        /// <param name="p1">Value 1</param>
        /// <param name="p2">Value 2</param>
        /// <returns>compareTo value</returns>
        public int Compare(NamePair p1, NamePair p2)
        {
            String s1 = p1 == null ? "" : (_sortByName ? p1.GetName() : p1.GetID());
            String s2 = p2 == null ? "" : (_sortByName ? p2.GetName() : p2.GetID());
            return s1.CompareTo(s2);    //  sort order ??
        }	//	compare

        /// <summary>
        /// IComparable Interface (based on toString value)
        /// </summary>
        /// <param name="o"> o the Object to be compared.</param>
        /// <returns>a negative integer, zero, or a positive integer as this object 
        /// is less than, equal to, or greater than the specified object.
        /// </returns>
        public int CompareTo(Object o)
        {
            return Compare(this, o);
        }	//	compareTo

        /// <summary>
        /// IComparable Interface (based on toString value)
        /// </summary>
        /// <param name="o"> o the Object to be compared.</param>
        /// <returns>a negative integer, zero, or a positive integer as this object 
        /// is less than, equal to, or greater than the specified object.
        /// </returns>
        public int CompareTo(NamePair o)
        {
            return Compare(this, o);
        }	//	compareTo


        /// <summary>
        /// To String - returns name
        /// </summary>
        /// <returns>Name</returns>
        public override string ToString()
        {
            return _name;
        }

        /// <summary>
        /// Returns the index of the first option found for the key, or -1 if not found.
        /// </summary>
        public static int IndexOfKey(ArrayList options, String key)
        {
            if (options == null)
                return -1;
            if (key == null)
                key = "";

            int result = -1;
            for (int i = 0; i < options.Count; ++i)
            {
                //in KeyNamePair case, getID can be null
                if (key.Equals(((NamePair)options[i]).GetID()) || ("".Equals(key) && ((NamePair)options[i]).GetID() == null))
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the index of the first option found for the key, or -1 if not found.
        /// </summary>
        public static int IndexOfKey(List<NamePair> options, String key)
        {
            if (options == null)
                return -1;
            if (key == null)
                key = "";

            int result = -1;
            for (int i = 0; i < options.Count; ++i)
            {
                //in KeyNamePair case, getID can be null
                if (key.Equals(((NamePair)options[i]).GetID()) || ("".Equals(key) && ((NamePair)options[i]).GetID() == null))
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the index of the first option found for the key, or -1 if not found.
        /// </summary>
        public static int IndexOfValue(List<NamePair> options, String value)
        {
            if (value == null)
                value = "";

            int result = -1;
            for (int i = 0; i < options.Count; ++i)
            {
                if (value.Equals(((NamePair)options[i]).GetName()))
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the index of the first option found for the key, or -1 if not found.
        /// </summary>
        public static int IndexOfValue(ArrayList options, String value)
        {
            if (value == null)
                value = "";

            int result = -1;
            for (int i = 0; i < options.Count; ++i)
            {
                if (value.Equals(((NamePair)options[i]).GetName()))
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        public String ToStringX()
        {
            StringBuilder sb = new StringBuilder(GetID());
            sb.Append("=").Append(_name);
            return sb.ToString();
        }	//	toStringX

    }
}
