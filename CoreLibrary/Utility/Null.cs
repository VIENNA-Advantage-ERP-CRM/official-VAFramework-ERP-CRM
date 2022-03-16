/********************************************************
 * Module/Class Name    : Null (for utility)
 * Purpose        : 
 * Class Used     : 
 * Chronological Development
 * Mukesh Arora     14-May-2009
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Utility
{
    public class Null
    {
        /** Singleton				*/
	public static  Null	NULL = new Null();
	/** String Representation	*/
	public const String	NULLString = "NULLValue";
	
 /// <summary>
 /// Null Constructor
 /// </summary>
 	private Null ()
	{
	}	//	Null
	
	
	/// <summary>
    /// Equals
	/// </summary>
    /// <param name="obj">object</param>
    /// <returns>true if equals</returns>
	public override Boolean Equals (Object obj)
	{
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		return obj.ToString().Equals(ToString());
	}	//	equals

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

	/// <summary>
    /// String Representation
	/// </summary>
	/// <returns>info</returns>
	public override String ToString()
	{
		return NULLString;
	}	//	toString

    }
}
