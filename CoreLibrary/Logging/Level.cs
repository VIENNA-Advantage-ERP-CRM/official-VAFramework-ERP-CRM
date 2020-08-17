/********************************************************
 * Module Name    :     Log
 * Purpose        :     Maintain the Log (Levels to be maintained)
 * Author         :     Jagmohan Bhatt
 * Date           :     3-Aug-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using ViennaLogger.Logging;


namespace VAdvantage.Logging
{
    /// <summary>
    /// The Level class defines a set of standard logging levels that
    /// can be used to control logging output.  The logging Level objects
    /// are ordered and are specified by ordered integers.  Enabling logging
    /// at a given level also enables logging at all higher levels.
    /// <para>
    /// Clients should normally use the predefined Level constants such
    /// as Level.SEVERE.
    /// </para>
    /// The levels in descending order are:
    /// <list type="bullet">
    /// <item><description>SEVERE</description></item>
    /// <item><description>WARNING</description></item>
    /// <item><description>INFO</description></item>
    /// <item><description>CONFIG</description></item>
    /// <item><description>FINE</description></item>
    /// <item><description>FINER</description></item>
    /// <item><description>FINEST</description></item>
    /// </list>
    /// </summary>
    [Serializable()]
    public class Level
    {
        
        public static ArrayList known = new ArrayList();
        //value of the level
        private int value;
        //name of the level
        private string name;

        public static Level OFF = new Level("OFF", int.MaxValue);
        public static Level SEVERE = new Level("SEVERE", 1000);
        public static Level WARNING = new Level("WARNING", 900);
        public static Level INFO = new Level("INFO", 800);
        public static Level CONFIG = new Level("CONFIG", 700);
        public static Level FINE = new Level("FINE", 600);
        public static Level FINER = new Level("FINER", 500);
        public static Level FINEST = new Level("FINEST", 400);
        public static Level ALL = new Level("ALL", int.MinValue);


        /// <summary>
        /// Name of the Level
        /// </summary>
        /// <returns>Gets the name of the level</returns>
        public string GetName()
        {
            
            return name;
        }

        /// <summary>
        /// Value of the Level
        /// </summary>
        /// <returns>Gets the value of the Level</returns>
        public int GetValue()
        {
            return value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_name">Name of the Level</param>
        /// <param name="_value">Value of the Level</param>
        public Level(string _name, int _value)
        {
            if (string.IsNullOrEmpty(_name))
                return;

            this.name = _name;
            this.value = _value;

            lock (this)
            {
                known.Add(this);
            }
        }

        /// <summary>
        /// Gets the integer value of the current level
        /// </summary>
        /// <returns>int value of the level</returns>
        public int IntValue()
        {
            return value;
        }

        /// <summary>
        /// Gets the Level available
        /// </summary>
        /// <returns></returns>
        public static KeyNamePair[] GetLevels()
        {
            KeyNamePair[] retValue = null;
            System.Collections.Generic.List<KeyNamePair> _list = new List<KeyNamePair>();
            for (int i = 0; i <= known.Count - 1; i++)
            {
                Level l = (Level)known[i];
                KeyNamePair p = new KeyNamePair(l.value, l.name);
                _list.Add(p);
            }
            retValue = new KeyNamePair[_list.Count];
            retValue = _list.ToArray();
             
            return retValue;
        }
    }
}
