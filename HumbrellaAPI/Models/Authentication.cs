using AutoMapper;
using HumbrellaAPI.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HumbrellaAPI.Models
{
    public class Authentication
    {
        private DBContext dBContext;

        public Authentication()
        {
            dBContext = new DBContext();
        }

        public ResponseEnity login(AuthenticationEntity authenticationEntity)
        {
            try
            {
                var command = dBContext.Connection.CreateCommand() as SqlCommand;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.prcCheckUserLogin";
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@UID",
                    DbType = DbType.String,
                    Value = authenticationEntity.UserID,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@PWD",
                    DbType = DbType.String,
                    Value = Helper.hashValue(authenticationEntity.Pwd),
                });

                List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

                if (result != null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<IDictionary<String, Object>, List<ResponseEnity>>();
                    }).CreateMapper();
                    ResponseEnity dBResponse = config.Map<List<ResponseEnity>>(result).FirstOrDefault();

                    if (dBResponse.StatusCode == 1)
                    {
                        dBResponse.Result = getJWTPacket(authenticationEntity.UserID);
                    }
                    return dBResponse;
                }
                else
                {
                    ResponseEnity response = new ResponseEnity();
                    response.StatusCode = -1;
                    return response;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ResponseEnity register(RegistrationEntity registrationEntity)
        {
            try
            {
                if (checkUserIDAvailability(registrationEntity.UserID).StatusCode == 1)
                {
                    if (checkUserEmailAvailability(registrationEntity.EmailID).StatusCode == 1)
                    {
                        var command = dBContext.Connection.CreateCommand() as SqlCommand;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.prcUpSertUserLogin";
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@UID",
                            DbType = DbType.String,
                            Value = registrationEntity.UserID,
                        });
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@EMAIL",
                            DbType = DbType.String,
                            Value = Helper.hashValue(registrationEntity.EmailID),
                        });
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@PWD",
                            DbType = DbType.String,
                            Value = Helper.hashValue(registrationEntity.Pwd),
                        });

                        List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

                        if (result != null)
                        {
                            var config = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<IDictionary<String, Object>, List<ResponseEnity>>();
                            }).CreateMapper();
                            ResponseEnity dBResponse = config.Map<List<ResponseEnity>>(result).FirstOrDefault();

                            if (dBResponse.StatusCode == 1)
                            {
                                dBResponse.Result = getJWTPacket(registrationEntity.UserID);
                            }
                            return dBResponse;
                        }
                        else
                        {
                            ResponseEnity response = new ResponseEnity();
                            response.StatusCode = -1;
                            return response;
                        }
                    }
                    else
                    {
                        ResponseEnity response = new ResponseEnity();
                        response.StatusCode = 0;
                        response.StatusDesc = "User email already exists.";
                        return response;
                    }
                }
                else
                {
                    ResponseEnity response = new ResponseEnity();
                    response.StatusCode = 0;
                    response.StatusDesc = "UserId already exists.";
                    return response;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public ResponseEnity checkUserIDAvailability(string UserId)
        {
            try
            {
                var command = dBContext.Connection.CreateCommand() as SqlCommand;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.prcCheckUserIDAvailability";
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@UID",
                    DbType = DbType.String,
                    Value = UserId,
                });

                List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

                if (result != null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<IDictionary<String, Object>, List<ResponseEnity>>();
                    }).CreateMapper();
                    ResponseEnity dBResponse = config.Map<List<ResponseEnity>>(result).FirstOrDefault();

                    return dBResponse;
                }
                else
                {
                    ResponseEnity response = new ResponseEnity();
                    response.StatusCode = -1;
                    return response;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ResponseEnity checkUserEmailAvailability(string email)
        {
            try
            {
                var command = dBContext.Connection.CreateCommand() as SqlCommand;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.prcCheckUserEmailAvailability";
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@EMAIL",
                    DbType = DbType.String,
                    Value = Helper.hashValue(email),
                });

                List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

                if (result != null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<IDictionary<String, Object>, List<ResponseEnity>>();
                    }).CreateMapper();
                    ResponseEnity dBResponse = config.Map<List<ResponseEnity>>(result).FirstOrDefault();

                    return dBResponse;
                }
                else
                {
                    ResponseEnity response = new ResponseEnity();
                    response.StatusCode = -1;
                    return response;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string getJWTPacket(string username)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, username)
            };
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HumbrellaAPI secret key"));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "humbrellaapi",
                audience: "humbrellauser",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
