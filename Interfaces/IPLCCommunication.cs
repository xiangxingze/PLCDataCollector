public interface IPLCCommunication
{
    Task<bool> ConnectAsync(string ipAddress, int port);
    Task<bool> DisconnectAsync();
    Task<object> ReadDataAsync(string address);
    Task<bool> WriteDataAsync(string address, object value);
    Task<bool> IsConnectedAsync();
} 