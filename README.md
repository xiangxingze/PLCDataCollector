# PLC数据采集系统 (PLCDataCollector)

## 项目简介
PLCDataCollector 是一个基于.NET Core开发的工业自动化数据采集系统，用于实时采集、监控和管理PLC设备数据。系统具备报警监控、数据可视化、实时通知等功能，为工业自动化提供完整的数据采集解决方案。

## 主要功能
- 实时数据采集：支持多种PLC设备的数据采集
- 报警监控：支持高低限报警配置和实时报警推送
- 实时通知：支持邮件和短信报警通知
- 数据可视化：使用SignalR实现实时数据展示
- 用户认证：完整的用户认证和授权系统
- 报表导出：支持数据导出功能

## 技术栈
- 后端框架：.NET Core
- 数据库：Entity Framework Core
- 实时通信：SignalR
- 认证授权：JWT Token
- 日志管理：ILogger
- 配置管理：IConfiguration

## 系统架构
- API层：提供RESTful API接口
- 服务层：包含业务逻辑处理
- 数据访问层：负责数据持久化
- 实时通信层：处理WebSocket通信
- 报警服务：处理设备报警逻辑
- 通知服务：处理邮件和短信通知

## 快速开始

### 环境要求
- .NET Core 6.0+
- SQL Server
- Visual Studio 2019+

### 安装步骤
1. 克隆代码库
```bash
git clone [repository-url]
```

2. 配置数据库连接字符串
在`appsettings.json`中修改数据库连接字符串：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string"
  }
}
```

3. 配置报警设置
在`appsettings.json`中配置报警参数：
```json
{
  "Alarms": [
    {
      "DeviceName": "Device1",
      "Address": "D100",
      "HighLimit": 100,
      "LowLimit": 0,
      "Enabled": true
    }
  ]
}
```

4. 运行项目
```bash
dotnet run
```

## API文档
API文档访问地址：https://localhost:44341/swagger/index.html

## 项目结构
```
PLCDataCollector.API/
├── Controllers/       # API控制器
├── Services/         # 业务服务层
├── Models/           # 数据模型
├── Data/            # 数据访问层
├── Hubs/            # SignalR集线器
├── Enums/           # 枚举定义
└── Program.cs       # 程序入口
```

## 许可证
[MIT License](LICENSE)

## 联系方式
如有问题或建议，请提交 Issue 或 Pull Request。 