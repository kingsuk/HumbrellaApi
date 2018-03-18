using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Models
{
    public class DBContext
    {
        public SqlConnection Connection { get; set; }

        public DBContext()
        {
            Connection = new SqlConnection("Server=139.59.28.88;Database=Humbrella;User Id=rajarshi;Password = Humbrella1; ");
        }

        public void Dispose()
        {
            Connection.Close();
        }

        public List<IDictionary<String, Object>> GetDatabaseResultSet(SqlCommand sqlCommand)
        {
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                try
                {
                    if (reader.HasRows)
                    {
                        List<IDictionary<String, Object>> result = new List<IDictionary<String, Object>>();
                        var fields = new List<String>();

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            fields.Add(reader.GetName(i));
                        }

                        while (reader.Read())
                        {
                            var item = new ExpandoObject() as IDictionary<String, Object>;

                            for (var i = 0; i < fields.Count; i++)
                            {
                                item.Add(fields[i], reader[fields[i]]);
                            }
                            result.Add(item);
                        }
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch(Exception e)
                {
                    throw e;
                }
                finally
                {
                    reader.Close();
                }
            }
        }
    }
}
