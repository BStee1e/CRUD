using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CRUD
{
    public class CRUD
    {
        private string conn = string.Empty;
        private string em = string.Empty;
        private bool err = false;

        // public error handlers for sql errors
        public string ErrorMsg { get { return em; } }
        public bool isErr { get { return err; } }

        public string ConnStr { get { return conn; } set { conn = value; } }

        // initialization
        public CRUD()
        {

        }

        // initialization with a connection string
        public CRUD(string ConnectionString)
        {
            conn = ConnectionString;
        }

        // execute all non-query sql statements, i.e. updates, deletes, etc.
        public void executeNonQuery(string sql, List<SqlParameter> parms)
        {
            resetErrorProperties();
            SqlConnection conn = new SqlConnection();
            try
            {   // add any parameters to the statement
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddRange(parms.ToArray());
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                err = true;
                em = ex.Message + " " + ex.Source;
            }
            finally { conn.Close(); }
        }

        // retrieves data from database and retuns it within a datatable object
        public DataTable getDataTable(string sql, List<SqlParameter> parms)
        {
            resetErrorProperties();
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.Parameters.AddRange(parms.ToArray());
                dt.Load(cmd.ExecuteReader());
            }
            catch (SqlException ex)
            {
                err = true;
                em = ex.Message + " " + ex.Source;
            }
            finally { conn.Close(); }
            return dt;
        }

        // return a data dictionary of objects with key/value from db
        public Dictionary<string, string> getDataDictionary(string sql, List<SqlParameter> parms)
        {
            resetErrorProperties();
            Dictionary<string, string> dicObj = new Dictionary<string, string>();
            SqlConnection conn = new SqlConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.Parameters.AddRange(parms.ToArray());
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        dicObj.Add(dr.GetValue(0).ToString(), dr.GetValue(1).ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                err = true;
                em = ex.Message + " " + ex.Source;
            }
            finally { conn.Close(); }
            return dicObj;
        }

        // Look up method to get quick down and dirty values from the database
        // returns a single string or a string of parsable values
        public string getLookUpValue(string sql, List<SqlParameter> parms)
        {
            resetErrorProperties();
            string retVal = "";
            SqlConnection conn = new SqlConnection();
            try
            {
                // execute the sql statement and get what should be just 
                // one value, otherwise, it will be the last value
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.Parameters.AddRange(parms.ToArray());
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {   // if more than one value, only the last will be returned
                        // not to mention that only the first item (if more than one)
                        // at ordinal 0 will be gotten.
                        retVal = dr[0].ToString();
                    }
                }
            }
            catch (SqlException sqlex)
            {
                err = true;
                em = sqlex.Message + ": " + sqlex.Source;
            }
            finally { conn.Close(); }
            return retVal;
        }

        // reset error property state at each method execution
        private void resetErrorProperties()
        {
            err = false;
            em = String.Empty;
        }

    }
}
