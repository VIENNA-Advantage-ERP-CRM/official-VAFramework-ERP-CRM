using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VAdvantage.Print
{
    public class TextAttribute : Attributes
    {
        // table of all instances in this class, used by readResolve
        private static Hashtable instanceMap = new Hashtable(29);

        protected TextAttribute(String name) : base(name)
        {

            if (this.GetType() == typeof(TextAttribute))
            {
                if (instanceMap.ContainsKey(name))
                    instanceMap[name] = this;
                else
                    instanceMap.Add(name, this);
            }
        }

        public static TextAttribute FONT = new TextAttribute("font");
        public static TextAttribute FOREGROUND = new TextAttribute("foreground");
        public static TextAttribute UNDERLINE = new TextAttribute("underline");
        public static int UNDERLINE_LOW_ONE_PIXEL = 1;
    }


    [Serializable]
    public class Attributes
    {
        private String name;

        // table of all instances in this class, used by readResolve
        private static Hashtable instanceMap = new Hashtable(7);

        protected Attributes(String name)
        {
            this.name = name;
            if (this.GetType() == typeof(Attribute))
            {
                if (instanceMap.ContainsKey(name))
                    instanceMap[name] = this;
                else
                    instanceMap.Add(name, this);
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.GetType().FullName + "(" + name + ")";
        }

        protected String GetName()
        {
            return name;
        }

        public static Attributes LANGUAGE = new Attributes("language");
        public static Attributes READING = new Attributes("reading");
    }
}
