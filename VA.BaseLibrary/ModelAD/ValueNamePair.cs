using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace VAdvantage.Model
{
    public sealed class ValueNamePair : NamePair
    {
        /** The Value       */
        private string _value = null;

        public ValueNamePair()
        {
        }

        public ValueNamePair(string value, string name)
            : base(name)
        {
            _value = value;
            if (_value == null)
                _value = "";
        }

        public new bool Equals(object obj)
        {
            //if (obj instanceof ValueNamePair)
            if (typeof(object) == obj.GetType())
            {
                ValueNamePair pp = (ValueNamePair)obj;
                if (pp.GetName() != null && pp.GetValue() != null &&
                    pp.GetName().Equals(GetName()) && pp.GetValue().Equals(_value))
                    return true;
                return false;
            }
            return false;
        }

        public override String GetID()
        {
            return _value;
        }

        public override object GetKeyID()
        {
            return _value;
        }


        public override object Key
        {
            get
            {
                return _value;
            }
        }


        public string GetValue()
        {
            return _value;
        }

        public int HashCode()
        {
            return _value.GetHashCode();
        }
    }
}
