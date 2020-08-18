using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace VAdvantage.Utility
{
    public class ViewUtil
    {
     /// <summary>
     ///Read the sql statements from the input stream	 * 	
     /// </summary>
     /// <param name="input stream">inn</param>
     /// <returns>array of sql commands</returns>
	//public static List<String> ReadSqlFromFile(InputStream in)
    public static List<String> ReadSqlFromFile(String filename)
	{
		if (filename=="" || filename == null)
        {
			return null;
		}
		// Initialization
		//BufferedReader input = null;
        StreamReader input=null;
		String line = null;
		String command = null;
		StringBuilder sqlStatement = null;
		int commentIndex = -1;
		List<String> retStr = new List<String>();
		try
		{	
			// Create the reader to read the input sql statements line by line
			//input = new BufferedReader(new InputStreamReader(in));
            input = new StreamReader(filename);
			while ((line = input.ReadLine()) != null)
			{             
              // Trim the line
				line = line.Trim();
				// Zero length.  Can't start with space
                if (line.Length == 0)
                {
                    continue;
                }
				// Handle C style comments which "/*" has to be at beginning of line
				if (line.StartsWith("/*"))
				{
					// Ensure it isn't one line comment
                    if (line.IndexOf("*/") != -1)
                    {
                        continue;
                    }
					// Ignore all the comments
                    while ((line = input.ReadLine()) != null && line.IndexOf("*/") == -1)
                    {
                        ;
                    }
					// Start with next line
					continue;  
				}

				//  Comment or separator
				if (line.StartsWith("--") || line.StartsWith(";") || line.StartsWith("/"))
					continue;

				// Don't care about the PL/SQL stuff
				if (line.ToUpper().StartsWith("SET SERVEROUTPUT ON"))
					continue;

				// Exit 
				if (line.ToUpper().StartsWith("EXIT"))
					break;

				// Strip off the comments - isn't at beginning, must be at the end
				commentIndex = line.IndexOf("--");
				if (commentIndex != -1)
					line = line.Substring(0, commentIndex);

				// Trim the line
				line = line.Trim();

				// Single line command
				if (line.EndsWith(";") || line.EndsWith("/"))
				{
					// Strip out the ";" before send thru jdbc
					command = line.Substring(0, line.Length - 1);
				}
				else
				{
					// Check what kind of commands it is
					bool notProcedure = true;
					if (line.ToUpper().StartsWith("CREATE OR REPLACE PROCEDURE") ||
							line.ToUpper().StartsWith("CREATE OR REPLACE FUNCTION") ||
							line.ToUpper().StartsWith("CREATE PROCEDURE") ||
							line.ToUpper().StartsWith("CREATE FUNCTION") ||
							line.ToUpper().StartsWith("DECLARE") ||
							line.ToUpper().StartsWith("BEGIN"))
						notProcedure = false;

					// Loop thru the multi-line or nested statements
					sqlStatement = new StringBuilder(line);
					while ((line = input.ReadLine()) != null)
					{
						// Trim the line
						//line = line.trim();

						// Check empty
						if (line.Length == 0)
							continue; 

						// Ignore the comments
						if (line.StartsWith("--"))
							continue;

						// Handle C style comments which "/*" has to be at beginning of line
						if (line.StartsWith("/*"))
						{
							// Ensure it isn't one line comment
							if (line.IndexOf("*/") != -1)
								continue;
							// Ignore all the comments
							while ((line = input.ReadLine()) != null && line.IndexOf("*/") == -1)
								;
							// Start with next line
							continue;  
						}

						// End of sql statement
						if (line.StartsWith(";") || line.StartsWith("/") || line.StartsWith("GO") && !line.StartsWith("GOTO"))
							break;

						// Strip off the comments
						commentIndex = line.IndexOf("--");
						if (commentIndex != -1 && line[(commentIndex - 1)] != '\'')
							line = line.Substring(0, commentIndex);

						// Trim the line again after removed the "--" comments
						//line = line.trim();

						sqlStatement.Append(" ").Append(line);

						// For non function/procedure, ";" is end of statement
						if (notProcedure && (line.EndsWith(";") || line.StartsWith("GO") && DataBase.DatabaseType.IsMSSql))
							break;
					} // while ((line = input.readLine()) != null)
					command = sqlStatement.ToString();

					// For non function/procedure, strip off the trailing ";"
					if (!command.ToUpper().StartsWith("CREATE OR REPLACE PROCEDURE") 
						&& !command.ToUpper().StartsWith("CREATE OR REPLACE FUNCTION") 
						&& !command.ToUpper().StartsWith("DECLARE") 
						&& !command.ToUpper().StartsWith("BEGIN") 
						&& command.EndsWith(";"))
						command = command.Substring(0, command.Length - 1);
				}  // if (line.endsWith(";") || line.endsWith("/"))	
				
				retStr.Add(command);
			} // while ((line = input.readLine()) != null)
		}
		catch (Exception ex)
		{
		  ex.StackTrace.ToString();// . printStackTrace();
		}
		
		return retStr;
	} 
    }
}
