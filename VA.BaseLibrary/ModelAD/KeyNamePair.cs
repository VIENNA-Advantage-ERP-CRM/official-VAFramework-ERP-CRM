using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Model
{
    public sealed class KeyNamePair : NamePair
    {
        public KeyNamePair()
        {
        }	//	KeyNamePair

        public KeyNamePair(int key, String name)
            : base(name)
        {
            
            _key = key;
        }   //  KeyNamePair


        public KeyNamePair(Object key, String name)
            : base(name)
        {

            _keyID = key;
        }   //  KeyNamePair

        /** The Key         */
        private int _key = 0;

        private object _keyID = null;

        /// <summary>
        /// Get Key
        /// </summary>
        /// <returns>key</returns>
        public int GetKey()
        {
            return _key;
        }	//	getKey 


        public override String GetID()
        {
            if (_key == -1)
                return null;
            return _key.ToString();
        }	//	getID

        public override object GetKeyID()
        {
            if (_keyID == null || _keyID.ToString() == "-1")
                return DBNull.Value;
            return _keyID;
            
        }

        public override object Key
        {
            get
            {
                //if (_keyID == null || _keyID.ToString() == "-1")
                 //   return DBNull.Value;
                return _key;
            }
            set
            {
                _key = (int)value;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj) 
        {
            if (obj.GetType() == typeof(KeyNamePair))
            {
                KeyNamePair pp = (KeyNamePair)obj;
                if (pp.GetKey() == _key
                    && pp.GetName() != null
                    && pp.GetName().Equals(GetName()))
                    return true;
                return false;
            }
            return false;
        }	//	equals

        /// <summary>
        /// IComparer Interface (based on Name/ID value)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public int Compare(KeyNamePair p1, KeyNamePair p2)
        {
            if (IsSortByName())
                return base.Compare(p1, p2);
            int i1 = p1.GetKey();
            int i2 = p2.GetKey();
            return i1.CompareTo(i2);
        }

        /// <summary>
        /// IComparer Interface (based on Name/ID value)
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(KeyNamePair o)
        {
            if (IsSortByName())
                return base.Compare(this, o);
            if (o == null)
                return -1;
            int i1 = int.Parse(_key.ToString());
            int i2 = o.GetKey();
            return i1.CompareTo(i2);
        }	//	compareTo

        public override int GetHashCode()
        {
            return _key;
        }

    }
}
