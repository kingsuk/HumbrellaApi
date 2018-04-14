using AutoMapper;
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
using uPLibrary.Networking.M2Mqtt;

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
                        MqttClient client = new MqttClient(configuration["MQTT:MqttServer"]);
                        client.Connect("HumbrellaAPI");
                        client.Publish(
                            configuration["MQTT:MqttPublisherTopicPrefix"] +stationId,
                            Encoding.UTF8.GetBytes(newQRCode)
                        );
                        //client.Subscribe(
                        //    configuration["MQTT:MqttPublisherTopicPrefix"] + stationId,
                        //    stationResponse
                        //);

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

        public ResponseEnity verifyQRCode(string qrCode)
        {
            var stationId = qrCode.Split("_")[0];

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

                if(qrDetails.QRCode.Equals(qrCode))
                {
                    ResponseEnity qrResponse = getQRCode(stationId);

                    if(qrResponse.StatusCode == 1)
                    {
                        string newQRCode = qrResponse.Result.ToString();

                        MqttClient client = new MqttClient(configuration["MQTT:MqttServer"]);
                        client.Connect("HumbrellaAPI");
                        client.Publish(
                            configuration["MQTT:MqttPublisherTopicPrefix"] + stationId,
                            Encoding.UTF8.GetBytes(newQRCode)
                        );

                        ResponseEnity response = new ResponseEnity();
                        response.StatusCode = 1;
                        response.StatusDesc = "Success";
                        return response;
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
                    response.StatusDesc = "Invalid QR code";
                    return response;
                }
            }
            else
            {
                ResponseEnity response = new ResponseEnity();
                response.StatusCode = 0;
                response.StatusDesc = "Invalid QR code. Invalid inventory id.";
                return response;
            }
        }

        private string generateQRCode(int stationId, int partnerId)
        {
            string qrCode = stationId.ToString() + "_" + partnerId.ToString() + "_" + Guid.NewGuid().ToString();
            return qrCode;
        }
    }
}
