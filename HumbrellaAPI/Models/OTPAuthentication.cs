using AutoMapper;
using HumbrellaAPI.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace HumbrellaAPI.Models
{
    public class OTPAuthentication
    {
        private DBContext dBContext;
        private IConfiguration configuration;

        public OTPAuthentication(IConfiguration iconfiguration)
        {
            dBContext = new DBContext();
            configuration = iconfiguration;
        }

        public ResponseEnity SendMobileOTP(string mobileNumber)
        {
            var OTP = generateOTP();

            var command = dBContext.Connection.CreateCommand() as SqlCommand;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.prcUpSertOTP";
            command.Parameters.Add(new SqlParameter
            {
                ParameterName = "@ID",
                DbType = DbType.String,
                Value = mobileNumber,
            });
            command.Parameters.Add(new SqlParameter
            {
                ParameterName = "@OTP",
                DbType = DbType.String,
                Value = OTP,
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
                    TwilioClient.Init(
                            configuration["PhoneNumberAuthentication:TwilioAccountSID"],
                            configuration["PhoneNumberAuthentication:TwilioAccountTOKEN"]
                        );

                    var to = new PhoneNumber("+91" + mobileNumber);
                    var message = MessageResource.Create(
                        to,
                        from: new PhoneNumber(configuration["PhoneNumberAuthentication:TwilioPhoneNumber"]),
                        body: "Your mobile varification OTP for Humbrella is " + OTP);

                    if (message.Sid != null)
                    {
                        ResponseEnity response = new ResponseEnity();
                        response.StatusCode = 1;
                        return response;
                    }
                    else
                    {
                        ResponseEnity response = new ResponseEnity();
                        response.StatusCode = 0;
                        response.StatusDesc = "Message could not be sent to the number";
                        return response;
                    }
                }
                else
                    return dBResponse;
            }
            else
            {
                ResponseEnity response = new ResponseEnity();
                response.StatusCode = -1;
                return response;
            }
        }

        public ResponseEnity VerifyMobileOTP(string mobileNumber, string OTP)
        {
            var command = dBContext.Connection.CreateCommand() as SqlCommand;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.prcGetOTPDetails";
            command.Parameters.Add(new SqlParameter
            {
                ParameterName = "@ID",
                DbType = DbType.String,
                Value = mobileNumber,
            });

            List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

            if (result != null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<IDictionary<String, Object>, List<OTPDetailsEntity>>();
                }).CreateMapper();
                OTPDetailsEntity otpDetails = config.Map<List<OTPDetailsEntity>>(result).FirstOrDefault();

                if(otpDetails.OTP == OTP)
                {
                    TimeSpan timeSpan = DateTime.UtcNow.Subtract(otpDetails.SentTime);
                    if(timeSpan.TotalMinutes <= 15)
                    {
                        ResponseEnity response = new ResponseEnity();
                        response.StatusCode = 1;
                        response.StatusDesc = "Valid";
                        return response;
                    }
                    else
                    {
                        ResponseEnity response = new ResponseEnity();
                        response.StatusCode = 0;
                        response.StatusDesc = "OTP time expired";
                        return response;
                    }
                }
                else
                {
                    ResponseEnity response = new ResponseEnity();
                    response.StatusCode = 0;
                    response.StatusDesc = "Invalid OTP";
                    return response;
                }
            }
            else
            {
                ResponseEnity response = new ResponseEnity();
                response.StatusCode = -1;
                return response;
            }
        }

        public ResponseEnity SendEmailOTP(string email)
        {
            var OTP = generateOTP();

            var command = dBContext.Connection.CreateCommand() as SqlCommand;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.prcUpSertOTP";
            command.Parameters.Add(new SqlParameter
            {
                ParameterName = "@ID",
                DbType = DbType.String,
                Value = Helper.hashValue(email),
            });
            command.Parameters.Add(new SqlParameter
            {
                ParameterName = "@OTP",
                DbType = DbType.String,
                Value = OTP,
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
                    SmtpClient client = new SmtpClient(configuration["EmailAuthentication:ClientHost"]);
                    client.Port = Convert.ToInt32(configuration["EmailAuthentication:ClientPort"]);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(
                                                configuration["EmailAuthentication:ClientUserName"],
                                                configuration["EmailAuthentication:ClientPassword"]
                                             );

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(configuration["EmailAuthentication:ClientEmailAddress"]);
                    mailMessage.To.Add(email);
                    mailMessage.Body = "Your email varification OTP for Humbrella is " + OTP;
                    mailMessage.Subject = "Email varification for Humbrella";
                    client.Send(mailMessage);

                    return dBResponse;
                }
                else
                    return dBResponse;
            }
            else
            {
                ResponseEnity response = new ResponseEnity();
                response.StatusCode = -1;
                return response;
            }
        }

        public ResponseEnity VerifyEmailOTP(string email, string OTP)
        {
            var command = dBContext.Connection.CreateCommand() as SqlCommand;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.prcGetOTPDetails";
            command.Parameters.Add(new SqlParameter
            {
                ParameterName = "@ID",
                DbType = DbType.String,
                Value = Helper.hashValue(email),
            });

            List<IDictionary<String, Object>> result = dBContext.GetDatabaseResultSet(command);

            if (result != null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<IDictionary<String, Object>, List<OTPDetailsEntity>>();
                }).CreateMapper();
                OTPDetailsEntity otpDetails = config.Map<List<OTPDetailsEntity>>(result).FirstOrDefault();

                if (otpDetails.OTP == OTP)
                {
                    TimeSpan timeSpan = DateTime.UtcNow.Subtract(otpDetails.SentTime);
                    if (timeSpan.TotalMinutes <= 15)
                    {
                        ResponseEnity response = new ResponseEnity();
                        response.StatusCode = 1;
                        response.StatusDesc = "Valid";
                        return response;
                    }
                    else
                    {
                        ResponseEnity response = new ResponseEnity();
                        response.StatusCode = 0;
                        response.StatusDesc = "OTP time expired";
                        return response;
                    }
                }
                else
                {
                    ResponseEnity response = new ResponseEnity();
                    response.StatusCode = 0;
                    response.StatusDesc = "Invalid OTP";
                    return response;
                }
            }
            else
            {
                ResponseEnity response = new ResponseEnity();
                response.StatusCode = -1;
                return response;
            }
        }

        private string generateOTP()
        {
            Random random = new Random();
            string OTP = "";

            for(int i=1; i<=4; i++)
            {
                OTP += random.Next(0, 9).ToString();
            }
            return OTP;
        }
    }
}
