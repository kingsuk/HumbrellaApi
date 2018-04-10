﻿using AutoMapper;
using HumbrellaAPI.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HumbrellaAPI.Models
{
    public class QRCodeAuthentication
    {
        private DBContext dBContext;
        private IConfiguration configuration;

        public QRCodeAuthentication(IConfiguration iconfiguration)
        {
            dBContext = new DBContext();
            configuration = iconfiguration;
        }

        public ResponseEnity getQRCode(string stationId)
        {
            var command = dBContext.Connection.CreateCommand() as SqlCommand;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.prcGetInventoryDetails";

            command.Parameters.Add(new SqlParameter
            {
                ParameterName = "@SID",
                DbType = DbType.String,
                Value = stationId
            });

            List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

            if (result != null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<IDictionary<String, Object>, List<InventoryDetailsEntity>>();
                }).CreateMapper();
                InventoryDetailsEntity inventory = config.Map<List<InventoryDetailsEntity>>(result).FirstOrDefault();

                string newQRCode = generateQRCode(inventory.StationID, inventory.PartnerID);

                var newCommand = dBContext.Connection.CreateCommand() as SqlCommand;
                newCommand.CommandType = CommandType.StoredProcedure;
                newCommand.CommandText = "dbo.prcSaveQR";
                newCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@SID",
                    DbType = DbType.String,
                    Value = inventory.StationID.ToString()
                });
                newCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@QRCODE",
                    DbType = DbType.String,
                    Value = newQRCode
                });

                List<IDictionary<String, Object>> dBResult = dBContext.GetDatabaseResultSet(newCommand);

                if (dBResult != null)
                {
                    var newConfig = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<IDictionary<String, Object>, List<ResponseEnity>>();
                    }).CreateMapper();
                    ResponseEnity dBResponse = newConfig.Map<List<ResponseEnity>>(dBResult).FirstOrDefault();

                    if(dBResponse.StatusCode == 1)
                    {
                        dBResponse.Result = newQRCode;
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
                response.StatusDesc = "No inventory found with this inventory id";
                return response;
            }
        }

        private string generateQRCode(int stationId, int partnerId)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Sid, stationId.ToString()),
                new Claim(ClaimTypes.GroupSid, partnerId.ToString())
            };
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HumbrellaAPI secret key"));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "humbrellaapi",
                audience: "humbrellainventory",
                claims: claims,
                notBefore: DateTime.Now,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}