using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    public abstract class Media : EnumSyntax
    {
        protected Media(int value)
            : base(value)
        {
            
        }

        public String GetName()
        {
            return "media";
        }
    }
}
