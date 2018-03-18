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

        public ResponseEntity login(AuthenticationEntity authenticationEntity)
        {
            try
            {
                ResponseEntity responseEntity = new ResponseEntity();

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
                    List<DBResultEnity> dBResult = config.Map<List<DBResultEnity>>(result);

                    DBResultEnity dBResultEnity = dBResult.FirstOrDefault();

                    responseEntity.StatusCode = dBResultEnity.STATUSCODE;
                    responseEntity.StatusMessage = dBResultEnity.STATUSDESC;
                    if (dBResultEnity.STATUSCODE == 1)
                    {
                        responseEntity.ResponseResult = getJWTPacket(authenticationEntity.UserId);
                    }
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

        public ResponseEntity register(AuthenticationEntity authenticationEntity)
        {
            try
            {
                ResponseEntity responseEntity = new ResponseEntity();

                if (checkUserIDAvailability(authenticationEntity.UserId).StatusCode == 0)
                {
                    var command = dBContext.Connection.CreateCommand() as SqlCommand;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "dbo.prcUpSertUserLogin";
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
                        List<DBResultEnity> dBResult = config.Map<List<DBResultEnity>>(result);

                        DBResultEnity dBResultEnity = dBResult.FirstOrDefault();

                        responseEntity.StatusCode = dBResultEnity.STATUSCODE;
                        responseEntity.StatusMessage = dBResultEnity.STATUSDESC;
                        if (dBResultEnity.STATUSCODE == 1)
                        {
                            responseEntity.ResponseResult = getJWTPacket(authenticationEntity.UserId);
                        }
                        return responseEntity;
                    }
                    else
                    {
                        responseEntity.StatusCode = 0;
                        responseEntity.StatusMessage = "Failed";
                        return responseEntity;
                    }
                }
                else
                {
                    responseEntity.StatusCode = 0;
                    responseEntity.StatusMessage = "User already exists";
                    return responseEntity;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public ResponseEntity checkUserIDAvailability(string UserId)
        {
            try
            {
                ResponseEntity responseEntity = new ResponseEntity();

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
