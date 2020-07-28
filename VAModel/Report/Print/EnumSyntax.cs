using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    public abstract class EnumSyntax : ICloneable
    {
        /// <summary>
        /// This enumeration value's integer value.
        /// </summary>
        private int value;

        /// <summary>
        /// Construct a new enumeration value with the given integer value.
        /// </summary>
        /// <param name="value">Integer value.</param>
        protected EnumSyntax(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Returns this enumeration value's integer value.
        /// </summary>
        /// <returns></returns>
        public int GetValue()
        {
            return value;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public Object Clone()
        {
            return this;
        }
    }
}
