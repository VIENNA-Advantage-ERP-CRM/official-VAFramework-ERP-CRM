using System;
using System.Data;
using System.Collections;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;

namespace VAdvantage.SqlExec.Oracle
{
    /// <summary>
    /// The SqlHelper class is intended to encapsulate high performance, scalable best practices for 
    /// common uses of SqlClient.
    /// </summary>
    public sealed class OracleHelper
    {
#pragma warning disable 612, 618
        private static VAdvantage.Logging.VLogger log = VAdvantage.Logging.VLogger.GetVLogger(typeof(OracleHelper).FullName);

        #region private utility methods & constructors

        //Since this class provides only static methods, make the default constructor private to prevent 
        //instances from being created with "new SqlHelper()".
        private OracleHelper() { }



        /// <summary>
        /// This method is used to attach array of OracleParameters to a OracleCommand.
        /// 
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// 
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">an array of OracleParameters tho be added to command</param>

        private static void AttachParameters(OracleCommand command, OracleParameter[] commandParameters)
        {
            foreach (OracleParameter p in commandParameters)
            {
                p.ParameterName = p.ParameterName.Replace("@", "");
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }
                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of OracleParameters.
        /// </summary>
        /// <param name="commandParameters">array of OracleParameters to be assigned values</param>
        /// <param name="parameterValues">array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(OracleParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                //do nothing if we get no data
                return;
            }

            // we must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            //iterate through the OracleParameters, assigning the values from the corresponding position in the 
            //value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command.
        /// </summary>
        /// <param name="command">the OracleCommand to be prepared</param>
        /// <param name="connection">a valid OracleConnection, on which to execute this command</param>
        /// <param name="transaction">a valid OracleTransaction, or 'null'</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of OracleParameters to be associated with the command or 'null' if no parameters are required</param>
        private static void PrepareCommand(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //associate the connection with the command
            command.Connection = connection;

            //set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            //if we were provided a transaction, assign it.
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }

            return;
        }


        #endregion private utility methods & constructors

        #region ExecuteNonQuery


        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of OracleParameters
            return ExecuteNonQuery(connectionString, commandType, commandText, (OracleParameter[])null);
        }


        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //create & open a OracleConnection, and dispose of it after we are done.
            using (OracleConnection cn = new OracleConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
            }
        }

        //public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of OracleParameters
        //        return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
        //    }
        //}


        //public static int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText)
        //{
        //    //pass through the call providing null for the set of OracleParameters
        //    return ExecuteNonQuery(connection, commandType, commandText, (OracleParameter[])null);
        //}


        public static int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            if (commandParameters != null)
            {
                for (int i = 0; i <= commandParameters.Length - 1; i++)
                    commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
            }

            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
           // cmd.CommandTimeout = 150;
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters);

            int retval = 0;
            try
            {
                //finally, execute the command.
                retval = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                if (retval == -1)
                    retval = 1;
            }
            catch (Exception ex)
            {
                connection.Close();
                cmd.Parameters.Clear();
                // log.SaveError("DBExecuteError", ex);
                retval = -1;
                throw ex;
                //connection.Dispose();
            }

            // detach the OracleParameters from the command object, so they can be used again.

            return retval;
        }

        public static int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText, OracleTransaction trx, params OracleParameter[] commandParameters)
        {
            if (commandParameters != null)
            {
                for (int i = 0; i <= commandParameters.Length - 1; i++)
                    commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
            }

            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, trx, commandType, commandText, commandParameters);

            //finally, execute the command.
            int retval = 0;
            try
            {
                retval = cmd.ExecuteNonQuery();
                if (retval == -1)
                    retval = 1;
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                cmd.Parameters.Clear();
                retval = -1;
                VAdvantage.Logging.VLogger.Get().Severe(e.Message + " [Query NQTrx]" + commandText);
                throw e;
            }

            // detach the OracleParameters from the command object, so they can be used again.
            return retval;
        }


        //public static int ExecuteNonQuery(OracleConnection connection, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of OracleParameters
        //        return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
        //    }
        //}


        public static int ExecuteNonQuery(OracleTransaction transaction, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of OracleParameters
            return ExecuteNonQuery(transaction, commandType, commandText, (OracleParameter[])null);
        }


        public static int ExecuteNonQuery(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            if (commandParameters != null)
            {
                for (int i = 0; i <= commandParameters.Length - 1; i++)
                    commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
            }

            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            //finally, execute the command.
            int retval = 0;
            try
            {
                retval = cmd.ExecuteNonQuery();
                // detach the OracleParameters from the command object, so they can be used again.
                cmd.Parameters.Clear();
                if (retval == -1)
                    retval = 1;
            }
            catch (Exception e)
            {
                retval = -1;
                cmd.Parameters.Clear();
                VAdvantage.Logging.VLogger.Get().Severe(e.Message + " [Query NQTrx]" + commandText);
                throw e;
            }
            return retval;
        }


        //public static int ExecuteNonQuery(OracleTransaction transaction, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of OracleParameters
        //        return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
        //    }
        //}


        #endregion ExecuteNonQuery

        #region ExecuteDataSet

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        //public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        //{
        //    //pass through the call providing null for the set of OracleParameters
        //    return ExecuteDataset(connectionString, commandType, commandText, (OracleParameter[])null);
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //create & open a OracleConnection, and dispose of it after we are done.
            using (OracleConnection cn = new OracleConnection(connectionString))
            {


                //call the overload that takes a connection in place of the connection string
                return ExecuteDataset(cn, commandType, commandText, commandParameters);
            }
        }


        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, int pageSize, int pageNumber, params OracleParameter[] commandParameters)
        {
            //create & open a OracleConnection, and dispose of it after we are done.
            using (OracleConnection cn = new OracleConnection(connectionString))
            {

                //call the overload that takes a connection in place of the connection string
                return ExecuteDataset(cn, commandType, commandText, pageSize, pageNumber, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        //public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of OracleParameters
        //        return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
        //    }
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the provided OracleConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        //public static DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText)
        //{
        //    //pass through the call providing null for the set of OracleParameters
        //    return ExecuteDataset(connection, commandType, commandText, (OracleParameter[])null);
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        private static DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            if (commandParameters != null)
            {
                for (int i = 0; i <= commandParameters.Length - 1; i++)
                    commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
            }

            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            try
            {
                da.Fill(ds);
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                Logging.VLogger.Get().Severe(ex.Message + " [Query D] =>" + commandText);
                cmd.Parameters.Clear();
                return null;
            }

            // detach the OracleParameters from the command object, so they can be used again.			


            //return the dataset
            return ds;
        }



        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        private static DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText, int pageSize, int pageNumber, params OracleParameter[] commandParameters)
        {
            if (commandParameters != null)
            {
                for (int i = 0; i <= commandParameters.Length - 1; i++)
                    commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
            }


//            commandText = @"SELECT * FROM (
//                     
//                         SELECT a.*, rownum r__
//                         FROM
//                              ( " + commandText + @" ) a
//                            
//                            WHERE rownum < ((" + pageNumber + @" * " + pageSize + @") + 1 )
//                        )
//                        WHERE r__ >= (((" + pageNumber + @"-1) * " + pageSize + @") + 1)";



            commandText = "select * FROM (SELECT t.*, rownum AS row_num FROM (" + commandText + ") t ) WHERE row_num BETWEEN (((" + pageNumber + " - 1) * " + pageSize + ") + 1) AND ((" + pageNumber + " * " + pageSize + "))";



            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            try
            {
               // da.Fill(ds, (pageNumber - 1) * pageSize, pageSize, "Data");
                //cmd.Parameters.Clear();
                da.Fill(ds);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Columns.IndexOf("row_num") > -1)
                    {
                        ds.Tables[0].Columns.Remove("row_num");
                    }

                }
            }
            catch (Exception ex)
            {
                cmd.Parameters.Clear();
                Logging.VLogger.Get().Severe(ex.Message + " [Query DP] =>" + commandText);
                return null;
            }

            // detach the OracleParameters from the command object, so they can be used again.			


            //return the dataset
            return ds;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="trx"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText, OracleTransaction trx, params OracleParameter[] commandParameters)
        {
            if (commandParameters != null)
            {
                for (int i = 0; i <= commandParameters.Length - 1; i++)
                    commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
            }

            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, trx, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            DataSet ds = new DataSet();

            //fill the DataSet using default values for DataTable names, etc.
            try
            {
                da.Fill(ds);
            }
            catch (Exception e)
            {
                VAdvantage.Logging.VLogger.Get().Severe(e.Message + " [Query DTrx] " + commandText);
            }

            // detach the OracleParameters from the command object, so they can be used again.			
            cmd.Parameters.Clear();

            //return the dataset
            return ds;
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        //public static DataSet ExecuteDataset(OracleConnection connection, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of OracleParameters
        //        return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
        //    }
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the provided OracleTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">a valid OracleTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        //public static DataSet ExecuteDataset(OracleTransaction transaction, CommandType commandType, string commandText)
        //{
        //    //pass through the call providing null for the set of OracleParameters
        //    return ExecuteDataset(transaction, commandType, commandText, (OracleParameter[])null);
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid OracleTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        //private static DataSet ExecuteDataset(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        //{
        //    if (commandParameters != null)
        //    {
        //        for (int i = 0; i <= commandParameters.Length - 1; i++)
        //            commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
        //    }

        //    //create a command and prepare it for execution
        //    OracleCommand cmd = new OracleCommand();
        //    PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

        //    //create the DataAdapter & DataSet
        //    OracleDataAdapter da = new OracleDataAdapter(cmd);
        //    DataSet ds = new DataSet();

        //    //fill the DataSet using default values for DataTable names, etc.
        //    da.Fill(ds);

        //    // detach the OracleParameters from the command object, so they can be used again.
        //    cmd.Parameters.Clear();

        //    //return the dataset
        //    return ds;
        //}

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified 
        /// OracleTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid OracleTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        //public static DataSet ExecuteDataset(OracleTransaction transaction, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of OracleParameters
        //        return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
        //    }
        //}

        #endregion ExecuteDataSet

        #region ExecuteReader

        /// <summary>
        /// this enum is used to indicate whether the connection was provided by the caller, or created by SqlHelper, so that
        /// we can set the appropriate CommandBehavior when calling ExecuteReader()
        /// </summary>
        private enum OracleConnectionOwnership
        {
            /// <summary>Connection is owned and managed by SqlHelper</summary>
            Internal,
            /// <summary>Connection is owned and managed by the caller</summary>
            External
        }

        /// <summary>
        /// Create and prepare a OracleCommand, and call ExecuteReader with the appropriate CommandBehavior.
        /// </summary>
        /// <remarks>
        /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        /// 
        /// If the caller provided the connection, we want to leave it to them to manage.
        /// </remarks>
        /// <param name="connection">a valid OracleConnection, on which to execute this command</param>
        /// <param name="transaction">a valid OracleTransaction, or 'null'</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of OracleParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="connectionOwnership">indicates whether the connection parameter was provided by the caller, or created by SqlHelper</param>
        /// <returns>OracleDataReader containing the results of the command</returns>
        private static OracleDataReader ExecuteReader(OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters, OracleConnectionOwnership connectionOwnership)
        {
            if (commandParameters != null)
            {
                for (int i = 0; i <= commandParameters.Length - 1; i++)
                    commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
            }

            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

            //create a reader
            OracleDataReader dr = null;

            try
            {
                if (connectionOwnership == OracleConnectionOwnership.External)
                {
                    dr = cmd.ExecuteReader();
                }
                else
                {
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                VAdvantage.Logging.VLogger.Get().Severe(e.Message + " [Query DR] " + commandText);
                cmd.Parameters.Clear();
                throw e;
            }
            // detach the OracleParameters from the command object, so they can be used again.


            return dr;

        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of OracleParameters
            return ExecuteReader(connectionString, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {

            //create & open a OracleConnection
            OracleConnection cn = new OracleConnection(connectionString);


            try
            {
                cn.Open();
                //call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(cn, null, commandType, commandText, commandParameters, OracleConnectionOwnership.Internal);
            }
            catch (Exception e)
            {
                //if we fail to return the SqlDatReader, we need to close the connection ourselves
                cn.Close();
                VAdvantage.Logging.VLogger.Get().Severe(e.Message + " [Query DR] " + commandText);
                throw e;
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a OracleDataReader containing the resultset generated by the command</returns>
        //public static OracleDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of OracleParameters
        //        return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
        //    }
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the provided OracleConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a OracleDataReader containing the resultset generated by the command</returns>
        //public static OracleDataReader ExecuteReader(OracleConnection connection, CommandType commandType, string commandText)
        //{
        //    //pass through the call providing null for the set of OracleParameters
        //    return ExecuteReader(connection, commandType, commandText, (OracleParameter[])null);
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a OracleDataReader containing the resultset generated by the command</returns>
        //public static OracleDataReader ExecuteReader(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        //{
        //    //pass through the call to the private overload using a null transaction value and an externally owned connection
        //    return ExecuteReader(connection, (OracleTransaction)null, commandType, commandText, commandParameters, OracleConnectionOwnership.External);
        //}


        public static OracleDataReader ExecuteReader(OracleConnection connection, CommandType commandType, string commandText, OracleTransaction trx, params OracleParameter[] commandParameters)
        {
            //pass through the call to the private overload using a null transaction value and an externally owned connection
            return ExecuteReader(connection, trx, commandType, commandText, commandParameters, OracleConnectionOwnership.External);
        }


        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a OracleDataReader containing the resultset generated by the command</returns>
        //public static OracleDataReader ExecuteReader(OracleConnection connection, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

        //        AssignParameterValues(commandParameters, parameterValues);

        //        return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteReader(connection, CommandType.StoredProcedure, spName);
        //    }
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the provided OracleTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">a valid OracleTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a OracleDataReader containing the resultset generated by the command</returns>
        //public static OracleDataReader ExecuteReader(OracleTransaction transaction, CommandType commandType, string commandText)
        //{
        //    //pass through the call providing null for the set of OracleParameters
        //    return ExecuteReader(transaction, commandType, commandText, (OracleParameter[])null);
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///   OracleDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid OracleTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a OracleDataReader containing the resultset generated by the command</returns>
        //public static OracleDataReader ExecuteReader(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        //{
        //    //pass through to private overload, indicating that the connection is owned by the caller
        //    return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, OracleConnectionOwnership.External);
        //}

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified
        /// OracleTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid OracleTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a OracleDataReader containing the resultset generated by the command</returns>
        //public static OracleDataReader ExecuteReader(OracleTransaction transaction, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

        //        AssignParameterValues(commandParameters, parameterValues);

        //        return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
        //    }
        //}

        #endregion ExecuteReader

        #region ExecuteScalar

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        //public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        //{
        //    //pass through the call providing null for the set of OracleParameters
        //    return ExecuteScalar(connectionString, commandType, commandText, (OracleParameter[])null);
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //create & open a OracleConnection, and dispose of it after we are done.
            using (OracleConnection cn = new OracleConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string
                return ExecuteScalar(cn, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a 1x1 resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        //public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of OracleParameters
        //        return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
        //    }
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset and takes no parameters) against the provided OracleConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        //public static object ExecuteScalar(OracleConnection connection, CommandType commandType, string commandText)
        //{
        //    //pass through the call providing null for the set of OracleParameters
        //    return ExecuteScalar(connection, commandType, commandText, (OracleParameter[])null);
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        private static object ExecuteScalar(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            if (commandParameters != null)
            {
                for (int i = 0; i <= commandParameters.Length - 1; i++)
                    commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
            }
            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters);

            //execute the command & return the results
            object retval = DBNull.Value;
            try
            {
                retval = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                connection.Close();
                cmd.Parameters.Clear();
                VAdvantage.Logging.VLogger.Get().Severe(e.Message + "[Query SLR]" + commandText);
                throw e;
            }

            // detach the OracleParameters from the command object, so they can be used again.

            return retval;

        }

        public static object ExecuteScalar(OracleConnection connection, CommandType commandType, string commandText, OracleTransaction trx, params OracleParameter[] commandParameters)
        {
            if (commandParameters != null)
            {
                for (int i = 0; i <= commandParameters.Length - 1; i++)
                    commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
            }
            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, trx, commandType, commandText, commandParameters);

            //execute the command & return the results
            object retval = DBNull.Value;
            try
            {
                retval = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                cmd.Parameters.Clear();
                VAdvantage.Logging.VLogger.Get().Severe(e.Message + "[Query SLRTrx]" + commandText);
                throw e;
            }

            // detach the OracleParameters from the command object, so they can be used again.

            return retval;

        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a 1x1 resultset) against the specified OracleConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        //public static object ExecuteScalar(OracleConnection connection, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of OracleParameters
        //        return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
        //    }
        //}

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset and takes no parameters) against the provided OracleTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="transaction">a valid OracleTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of OracleParameters
            return ExecuteScalar(transaction.Connection, commandType, commandText, transaction, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset) against the specified OracleTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid OracleTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        //private static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        //{

        //    return ExecuteScalar(transaction.Connection, commandType, commandText,transaction, commandParameters);
        //    //if (commandParameters != null)
        //    //{
        //    //    for (int i = 0; i <= commandParameters.Length - 1; i++)
        //    //        commandText = commandText.Replace(commandParameters[i].ParameterName, commandParameters[i].ParameterName.Replace("@", ":"));
        //    //}
        //    ////create a command and prepare it for execution
        //    //OracleCommand cmd = new OracleCommand();
        //    //PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

        //    ////execute the command & return the results
        //    //object retval = cmd.ExecuteScalar();

        //    //// detach the OracleParameters from the command object, so they can be used again.
        //    //cmd.Parameters.Clear();
        //    //return retval;
        //}

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a 1x1 resultset) against the specified
        /// OracleTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid OracleTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        //public static object ExecuteScalar(OracleTransaction transaction, string spName, params object[] parameterValues)
        //{
        //    //if we receive parameter values, we need to figure out where they go
        //    if ((parameterValues != null) && (parameterValues.Length > 0))
        //    {
        //        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
        //        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

        //        //assign the provided values to these parameters based on parameter order
        //        AssignParameterValues(commandParameters, parameterValues);

        //        //call the overload that takes an array of OracleParameters
        //        return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
        //    }
        //    //otherwise we can just call the SP without params
        //    else
        //    {
        //        return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        //    }
        //}

        #endregion ExecuteScalar

    }

    /// <summary>
    /// SqlHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
    /// ability to discover parameters for stored procedures at run-time.
    /// </summary>
    public sealed class SqlHelperParameterCache
    {
        #region private methods, variables, and constructors

        //Since this class provides only static methods, make the default constructor private to prevent 
        //instances from being created with "new SqlHelperParameterCache()".
        private SqlHelperParameterCache() { }

        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// resolve at run time the appropriate set of OracleParameters for a stored procedure
        /// </summary>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">whether or not to include their return value parameter</param>
        /// <returns></returns>
        private static OracleParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            using (OracleConnection cn = new OracleConnection(connectionString))
            using (OracleCommand cmd = new OracleCommand(spName, cn))
            {
                cn.Open();
                cmd.CommandType = CommandType.StoredProcedure;

                OracleCommandBuilder.DeriveParameters(cmd);

                if (!includeReturnValueParameter)
                {
                    cmd.Parameters.RemoveAt(0);
                }

                OracleParameter[] discoveredParameters = new OracleParameter[cmd.Parameters.Count]; ;

                cmd.Parameters.CopyTo(discoveredParameters, 0);

                return discoveredParameters;
            }
        }

        //deep copy of cached OracleParameter array
        private static OracleParameter[] CloneParameters(OracleParameter[] originalParameters)
        {
            OracleParameter[] clonedParameters = new OracleParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (OracleParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion private methods, variables, and constructors

        #region caching functions

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters to be cached</param>
        public static void CacheParameterSet(string connectionString, string commandText, params OracleParameter[] commandParameters)
        {
            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        /// retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an array of SqlParamters</returns>
        public static OracleParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            string hashKey = connectionString + ":" + commandText;

            OracleParameter[] cachedParameters = (OracleParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion caching functions

        #region Parameter Discovery Functions

        /// <summary>
        /// Retrieves the set of OracleParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <returns>an array of OracleParameters</returns>
        public static OracleParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>
        /// Retrieves the set of OracleParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a OracleConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">a bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>an array of OracleParameters</returns>
        public static OracleParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            string hashKey = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            OracleParameter[] cachedParameters;

            cachedParameters = (OracleParameter[])paramCache[hashKey];

            if (cachedParameters == null)
            {
                cachedParameters = (OracleParameter[])(paramCache[hashKey] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
            }

            return CloneParameters(cachedParameters);
        }

        #endregion Parameter Discovery Functions
#pragma warning restore 612, 618
    }
}