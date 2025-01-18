using Microsoft.Extensions.DependencyInjection;
using PLCDataCollector.API.Data;
using PLCDataCollector.API.Models;
using PLCDataCollector.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using PLCDataCollector.API.Services;
using PLCDataCollector.Models;

namespace PLCDataCollector.API.Services
{
    public class DataCollectionService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DataCollectionService> _logger;
        private readonly IHubContext<PLCDataHub> _hubContext;
        private readonly List<PLCDevice> _devices;
        private readonly int _intervalSeconds;

        public DataCollectionService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<DataCollectionService> logger,
            IHubContext<PLCDataHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
            _hubContext = hubContext;
            
            _devices = _configuration.GetSection("PLCDevices").Get<List<PLCDevice>>() ?? new List<PLCDevice>();
            _intervalSeconds = _configuration.GetValue<int>("DataCollection:IntervalSeconds");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CollectData();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "数据采集出错");
                }

                await Task.Delay(TimeSpan.FromSeconds(_intervalSeconds), stoppingToken);
            }
        }

        private async Task CollectData()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PLCDataContext>();
            var alarmService = scope.ServiceProvider.GetRequiredService<AlarmService>();

            foreach (var device in _devices)
            {
                try
                {
                    // 模拟数据采集
                    var data = new CollectedData
                    {
                        DeviceName = device.Name,
                        Address = "D100",
                        Description = "温度",
                        Value = Random.Shared.NextDouble() * 100,
                        DataType = "Float",
                        Timestamp = DateTime.UtcNow
                    };

                    dbContext.CollectedData.Add(data);
                    await dbContext.SaveChangesAsync();

                    // 发送实时数据
                    await _hubContext.Clients.All.SendAsync("ReceiveData", data);

                    // 检查报警
                    await alarmService.CheckAlarms(data);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"从设备 {device.Name} 采集数据时出错");
                }
            }
        }
    }
} 