using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PLCDataCollector.API.Models;
using PLCDataCollector.API.Data;
using PLCDataCollector.API.Hubs;
using PLCDataCollector.API.Enums;
using System.IO;
using System.Runtime.InteropServices;
using System.Buffers.Binary;

namespace PLCDataCollector.API.Services
{
    public class AlarmService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AlarmService> _logger;
        private readonly NotificationService _notificationService;
        private readonly IHubContext<PLCDataHub> _hubContext;
        private List<AlarmConfig> _alarmConfigs;

        public AlarmService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<AlarmService> logger,
            NotificationService notificationService,
            IHubContext<PLCDataHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
            _notificationService = notificationService;
            _hubContext = hubContext;
            LoadAlarmConfigs();
        }

        private void LoadAlarmConfigs()
        {
            _alarmConfigs = _configuration.GetSection("Alarms").Get<List<AlarmConfig>>() ?? new List<AlarmConfig>();
        }

        public async Task CheckAlarms(CollectedData data)
        {
            var alarmConfig = _alarmConfigs?.FirstOrDefault(a => 
                a.DeviceName == data.DeviceName && 
                a.Address == data.Address &&
                a.Enabled);

            if (alarmConfig == null) return;

            try
            {
                if (data.Value > alarmConfig.HighLimit)
                {
                    await CreateAlarm(data, data.Value, alarmConfig.HighLimit, AlarmTypeEnum.HighLimit);
                    _logger.LogInformation($"高限报警触发: {data.DeviceName}, 值: {data.Value}, 限值: {alarmConfig.HighLimit}");
                }
                else if (data.Value < alarmConfig.LowLimit)
                {
                    await CreateAlarm(data, data.Value, alarmConfig.LowLimit, AlarmTypeEnum.LowLimit);
                    _logger.LogInformation($"低限报警触发: {data.DeviceName}, 值: {data.Value}, 限值: {alarmConfig.LowLimit}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理报警时出错: {data.DeviceName}, 值: {data.Value}");
            }
        }

        private async Task CreateAlarm(CollectedData data, double value, double limit, AlarmTypeEnum type)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PLCDataContext>();

                var alarm = new AlarmRecord
                {
                    DeviceName = data.DeviceName,
                    Address = data.Address,
                    Description = data.Description,
                    Value = value,
                    Limit = limit,
                    Type = type,
                    Timestamp = DateTime.UtcNow,
                    IsAcknowledged = false
                };

                dbContext.AlarmRecords.Add(alarm);
                await dbContext.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("ReceiveAlarm", alarm);
                
                await _notificationService.SendEmailAlertAsync(alarm);
                await _notificationService.SendSmsAlertAsync(alarm);
                
                _logger.LogWarning($"报警触发: {alarm.DeviceName} {alarm.Description} {alarm.Type}");
            }
        }

        public async Task AcknowledgeAlarm(int alarmId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PLCDataContext>();
                
                var alarm = await dbContext.AlarmRecords.FindAsync(alarmId);
                if (alarm != null)
                {
                    alarm.IsAcknowledged = true;
                    await dbContext.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("AlarmAcknowledged", alarmId);
                }
            }
        }

        public async Task ProcessAlarmAsync(double value)
        {
            // 检查是否超过阈值
            var alarmThreshold = _configuration.GetValue<double>("AlarmThreshold", 100.0);
            if (value > alarmThreshold)
            {
                _logger.LogWarning($"检测到异常值: {value}, 超过阈值: {alarmThreshold}");
                // 执行报警处理逻辑
                // ...
            }
            
            await Task.CompletedTask;
        }

        private async Task SendAlarmNotificationAsync(Models.AlarmRecord alarmRecord)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveAlarm", alarmRecord);
        }
    }
} 