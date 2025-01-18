using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using PLCDataCollector.API.Models;
using PLCDataCollector.API.Enums;

namespace PLCDataCollector.API.Services
{
    public class NotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;
        private readonly SmtpClient _smtpClient;
        private readonly string _smsApiKey;
        private readonly HttpClient _httpClient;

        public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            // 配置SMTP客户端
            var smtpConfig = configuration.GetSection("Smtp");
            _smtpClient = new SmtpClient
            {
                Host = smtpConfig["Host"],
                Port = int.Parse(smtpConfig["Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    smtpConfig["Username"],
                    smtpConfig["Password"])
            };

            // 配置SMS
            _smsApiKey = configuration["SmsApiKey"];
            _httpClient = new HttpClient();
        }

        public async Task SendEmailAlertAsync(AlarmRecord alarm)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Smtp:Username"]),
                    Subject = $"PLC报警通知 - {alarm.DeviceName}",
                    Body = $@"
                        设备: {alarm.DeviceName}
                        描述: {alarm.Description}
                        类型: {GetAlarmTypeDescription(alarm.Type)}
                        当前值: {alarm.Value}
                        限值: {alarm.Limit}
                        时间: {alarm.Timestamp}
                    ",
                    IsBodyHtml = false
                };

                foreach (var recipient in _configuration.GetSection("AlertRecipients:Email").Get<string[]>())
                {
                    mailMessage.To.Add(recipient);
                }

                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送报警邮件失败");
            }
        }

        private string GetAlarmTypeDescription(PLCDataCollector.API.Enums.AlarmTypeEnum alarmType)
        {
            return alarmType switch
            {
                PLCDataCollector.API.Enums.AlarmTypeEnum.HighLimit => "高限报警",
                PLCDataCollector.API.Enums.AlarmTypeEnum.LowLimit => "低限报警",
                PLCDataCollector.API.Enums.AlarmTypeEnum.Warning => "警告",
                PLCDataCollector.API.Enums.AlarmTypeEnum.Error => "错误",
                PLCDataCollector.API.Enums.AlarmTypeEnum.Critical => "严重",
                _ => "未知类型"
            };
        }

        public async Task SendSmsAlertAsync(AlarmRecord alarm)
        {
            try
            {
                var message = $"PLC报警: {alarm.DeviceName} {alarm.Description} {alarm.Value}";
                var recipients = _configuration.GetSection("AlertRecipients:Sms").Get<string[]>();

                foreach (var phoneNumber in recipients)
                {
                    var response = await _httpClient.PostAsync(
                        "https://api.sms-service.com/send",
                        new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("apikey", _smsApiKey),
                            new KeyValuePair<string, string>("phone", phoneNumber),
                            new KeyValuePair<string, string>("message", message)
                        }));

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"SMS API返回错误: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送报警短信失败");
            }
        }
    }
} 