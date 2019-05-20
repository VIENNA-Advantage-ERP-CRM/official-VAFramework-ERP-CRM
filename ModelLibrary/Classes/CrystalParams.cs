using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using System.Runtime.Serialization;

namespace VAdvantage
{
   
   public  class CrystalParams
    {
   
       public int AD_Form_ID
       {
           get;
           set;
       }

        
       public int AD_CrystalInstance_ID
       {
           get;
           set;
       }
        
       public int AD_Client_ID
       {
           get;
           set;
       }
        
       public string ProcedureName
       {
           get;
           set;
       }

      
       public List<String> ColNames
       {
           get;
           set;
       }

      
       public List<Object> ColValues
       {
           get;
           set;
       }

      
       public String Culture
       {
           get;
           set;
       }



    }

   public class ConnectionInfo
   {
       /** Name of Connection  */
       private String m_name = "Standard";

       public String Name
       {
           get { return m_name; }
           set { m_name = value; }
       }

       /** Application Host    */
       private String m_apps_host = "MyAppsServer";

       public String Apps_host
       {
           get { return m_apps_host; }
           set
           {
               m_apps_host = value;
               m_name = ToString();
           }
       }
       /** Application Port    */
       private int m_apps_port = 2090;

       public int Apps_port
       {
           get { return m_apps_port; }
           set { m_apps_port = value; }
       }

       /** Application Type       */
       private String appsType = "aa";// VEnvironment.APPSTYPE_VSERVER;

       public String AppsType
       {
           get { return appsType; }
           set { appsType = value; }
       }


       /** DB User name        */
       private String m_db_uid = "vienna";

       public String Db_uid
       {
           get { return m_db_uid; }
           set { m_db_uid = value; }
       }
       /** DB User password    */
       private String m_db_pwd = "vienna";

       public String Db_pwd
       {
           get { return m_db_pwd; }
           set { m_db_pwd = value; }
       }

       /** Database Type       */
       private String m_type  = DatabaseType.DB_ORACLE;// GlobalVariable.ORCL_VALUE_NAME;

       public String Db_Type
       {
           get { return m_type; }
           set { m_type = value; }
       }
       /** Database Host       */
       private String m_db_host = "MyDBServer";

       public String Db_host
       {
           get { return m_db_host; }
           set { m_db_host = value; }
       }

       private string m_db_port = "1521";

       public string Db_port
       {
           get { return m_db_port; }
           set { m_db_port = value; }
       }

       /** Database name       */
       private String m_db_name = "vienna";

       public String Db_name
       {
           get { return m_db_name; }
           set { m_db_name = value; }
       }

       private static ConnectionInfo obj;

       public static ConnectionInfo Get()
       {
           if (obj == null)
               obj = new ConnectionInfo();
           return obj;
       }


       public void SetAttributes(String attributes)
       {
           try
           {
               int index = attributes.IndexOf("HOST=");
               Db_host = Substring(attributes, index + 5, attributes.IndexOf(")", index));

               index = attributes.IndexOf("PORT=");
               Db_port = Substring(attributes, attributes.IndexOf("PORT=") + 5, attributes.IndexOf(")", index));
               index = attributes.IndexOf("SERVICE_NAME=");
               Db_name = Substring(attributes, attributes.IndexOf("SERVICE_NAME=") + 13, attributes.IndexOf(")", index));
               index = attributes.IndexOf("User Id=");
               Db_uid = Substring(attributes, attributes.IndexOf("User Id=") + 8, attributes.IndexOf(";", index));
               Db_pwd = attributes.Substring(attributes.IndexOf("Password=") + 9);
               //
           }
           catch (Exception e)
           {
              VAdvantage.Logging.VLogger.Get().Severe(attributes + " - " + e.ToString());
           }
       }	//  setAttributes

       private string Substring(string var, int startindex, int endindex)
       {
           endindex = (endindex - startindex);
           return var.Substring(startindex, endindex);
       }


   }
}
