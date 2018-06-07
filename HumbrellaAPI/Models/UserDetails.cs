﻿using AutoMapper;
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

        public ResponseEntity pushUserDetails(UpdateUserDetailsEntity updateUserDetailsEntity, string userId)
        {
            try
            {
                var command = dBContext.Connection.CreateCommand() as SqlCommand;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.prcUpSertUserDetails";
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@UID",
                    DbType = DbType.String,
                    Value = userId,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@FNAME",
                    DbType = DbType.String,
                    Value = updateUserDetailsEntity.FirstName,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@MNAME",
                    DbType = DbType.String,
                    Value = updateUserDetailsEntity.MiddleName,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@LNAME",
                    DbType = DbType.String,
                    Value = updateUserDetailsEntity.LastName,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@GENDER",
                    DbType = DbType.String,
                    Value = updateUserDetailsEntity.Gender,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@MAIL",
                    DbType = DbType.String,
                    Value = updateUserDetailsEntity.PersonalMailAddress,
                });

                List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

                if (result != null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<IDictionary<String, Object>, List<ResponseEntity>>();
                    }).CreateMapper();
                    ResponseEntity dBResponse = config.Map<List<ResponseEntity>>(result).FirstOrDefault();

                    return dBResponse;
                }
                else
                {
                    ResponseEntity response = new ResponseEntity();
                    response.StatusCode = -1;
                    return response;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
