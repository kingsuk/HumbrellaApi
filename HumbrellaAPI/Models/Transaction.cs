using AutoMapper;
using HumbrellaAPI.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HumbrellaAPI.Models
{
    public class Transaction
    {
        private DBContext dBContext;
        private IConfiguration configuration;

        public Transaction(IConfiguration iconfiguration)
        {
            dBContext = new DBContext();
            configuration = iconfiguration;
        }

        public ResponseEntity getProduct(string lastQRCode, string userID)
        {
            var stationId = lastQRCode.Split("_")[0];

            var command = dBContext.Connection.CreateCommand() as SqlCommand;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.prcGetQRDetails";

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
                    cfg.CreateMap<IDictionary<String, Object>, List<QRDetailsEntity>>();
                }).CreateMapper();
                QRDetailsEntity qrDetails = config.Map<List<QRDetailsEntity>>(result).FirstOrDefault();

                if (qrDetails.QRCode.Equals(lastQRCode))
                {
                    ResponseEntity response = new ResponseEntity();
                    response.StatusCode = 0;
                    response.StatusDesc = "Invalid QR code";
                    return response;
                }
                else
                {
                    ResponseEntity response = new ResponseEntity();
                    response.StatusCode = 0;
                    response.StatusDesc = "Invalid QR code";
                    return response;
                }
            }
            else
            {
                ResponseEntity response = new ResponseEntity();
                response.StatusCode = 0;
                response.StatusDesc = "Invalid QR code. Invalid inventory id.";
                return response;
            }
        }
    }
}
