using System;
using System.Threading.Tasks;

namespace PLCDataCollector.Communications
{
    public class SimulatedPLCCommunication : IPLCCommunication
    {
        private bool _isConnected;
        private readonly Random _random = new Random();
        private readonly Dictionary<string, double> _values = new Dictionary<string, double>();

        public async Task<bool> ConnectAsync(string ipAddress, int port)
        {
            _isConnected = true;
            return true;
        }

        public async Task<bool> DisconnectAsync()
        {
            _isConnected = false;
            return true;
        }

        public async Task<bool> IsConnectedAsync()
        {
            return _isConnected;
        }

        public async Task<object> ReadDataAsync(string address)
        {
            if (!_values.ContainsKey(address))
            {
                _values[address] = _random.NextDouble() * 100;
            }
            else
            {
                // 添加一些随机波动
                _values[address] += (_random.NextDouble() - 0.5) * 2;
            }

            return _values[address];
        }

        public async Task<bool> WriteDataAsync(string address, object value)
        {
            if (value is double doubleValue)
            {
                _values[address] = doubleValue;
                return true;
            }
            return false;
        }
    }
} 