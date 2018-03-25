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

        public DBResultEnity login(AuthenticationEntity authenticationEntity)
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
                    Value = authenticationEntity.UserId,
                });
                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@PWD",
                    DbType = DbType.String,
                    Value = hashPassword(authenticationEntity.Pwd),
                });

                List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

                if (result != null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<IDictionary<String, Object>, List<DBResultEnity>>();
                    }).CreateMapper();
                    DBResultEnity dBResult = config.Map<List<DBResultEnity>>(result).FirstOrDefault();

                    if (dBResult.StatusCode == 1)
                    {
                        dBResult.Result = getJWTPacket(authenticationEntity.UserId);
                    }
                    return dBResult;
                }
                else
                {
                    DBResultEnity dBResult = new DBResultEnity();
                    dBResult.StatusCode = -1;
                    return dBResult;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DBResultEnity register(RegistrationEntity registrationEntity)
        {
            try
            {
                if (checkUserIDAvailability(registrationEntity.UserId).StatusCode == 1)
                {
                    if (checkUserEmailAvailability(registrationEntity.EmailId).StatusCode == 1)
                    {
                        var command = dBContext.Connection.CreateCommand() as SqlCommand;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.prcUpSertUserLogin";
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@UID",
                            DbType = DbType.String,
                            Value = registrationEntity.UserId,
                        });
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@EMAIL",
                            DbType = DbType.String,
                            Value = hashPassword(registrationEntity.EmailId),
                        });
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@PWD",
                            DbType = DbType.String,
                            Value = hashPassword(registrationEntity.Pwd),
                        });

                        List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

                        if (result != null)
                        {
                            var config = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<IDictionary<String, Object>, List<DBResultEnity>>();
                            }).CreateMapper();
                            DBResultEnity dBResult = config.Map<List<DBResultEnity>>(result).FirstOrDefault();

                            if (dBResult.StatusCode == 1)
                            {
                                dBResult.Result = getJWTPacket(registrationEntity.UserId);
                            }
                            return dBResult;
                        }
                        else
                        {
                            DBResultEnity dBResult = new DBResultEnity();
                            dBResult.StatusCode = -1;
                            return dBResult;
                        }
                    }
                    else
                    {
                        DBResultEnity dBResultEnity = new DBResultEnity();
                        dBResultEnity.StatusCode = 0;
                        dBResultEnity.StatusDesc = "User email already exists.";

                        return dBResultEnity;
                    }
                }
                else
                {
                    DBResultEnity dBResultEnity = new DBResultEnity();
                    dBResultEnity.StatusCode = 0;
                    dBResultEnity.StatusDesc = "UserId already exists.";

                    return dBResultEnity;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public DBResultEnity checkUserIDAvailability(string UserId)
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
                        cfg.CreateMap<IDictionary<String, Object>, List<DBResultEnity>>();
                    }).CreateMapper();
                    DBResultEnity dBResult = config.Map<List<DBResultEnity>>(result).FirstOrDefault();

                    return dBResult;
                }
                else
                {
                    DBResultEnity dBResult = new DBResultEnity();
                    dBResult.StatusCode = -1;
                    return dBResult;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DBResultEnity checkUserEmailAvailability(string email)
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
                    Value = hashPassword(email),
                });

                List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

                if (result != null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<IDictionary<String, Object>, List<DBResultEnity>>();
                    }).CreateMapper();
                    DBResultEnity dBResult = config.Map<List<DBResultEnity>>(result).FirstOrDefault();

                    return dBResult;
                }
                else
                {
                    DBResultEnity dBResult = new DBResultEnity();
                    dBResult.StatusCode = -1;
                    return dBResult;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string hashPassword(string pwd)
        {
            byte[] salt = Encoding.ASCII.GetBytes("NZsP6NnmfBuYeJrrAKNuVQ==");
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: pwd,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

            return hashed;
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
