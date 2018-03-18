using AutoMapper;
using HumbrellaAPI.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Models
{
    public class UserDetails
    {
        private DBContext dBContext;

        public UserDetails()
        {
            dBContext = new DBContext();
        }

        public ResponseEntity pushUserDetails(UserDetailsEntity userDetailsEntity)
        {
            try
            {
                ResponseEntity responseEntity = new ResponseEntity();

                var command = dBContext.Connection.CreateCommand() as SqlCommand;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.prcUpSertUserDetails";
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@UID",
                    DbType = DbType.String,
                    Value = userDetailsEntity.UserId,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@FNAME",
                    DbType = DbType.String,
                    Value = userDetailsEntity.FirstName,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@MNAME",
                    DbType = DbType.String,
                    Value = userDetailsEntity.MiddleName,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@LNAME",
                    DbType = DbType.String,
                    Value = userDetailsEntity.LastName,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@GENDER",
                    DbType = DbType.String,
                    Value = userDetailsEntity.Gender,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@MAIL",
                    DbType = DbType.String,
                    Value = userDetailsEntity.PersonalMailAddress,
                });

                List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

                if (result != null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<IDictionary<String, Object>, List<DBResultEnity>>();
                    }).CreateMapper();
                    List<DBResultEnity> dBResult = config.Map<List<DBResultEnity>>(result);

                    DBResultEnity dBResultEnity = dBResult.FirstOrDefault();

                    responseEntity.StatusCode = dBResultEnity.STATUSCODE;
                    responseEntity.StatusMessage = dBResultEnity.STATUSDESC;
                    return responseEntity;
                }
                else
                {
                    responseEntity.StatusCode = 0;
                    responseEntity.StatusMessage = "Failed";
                    return responseEntity;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
