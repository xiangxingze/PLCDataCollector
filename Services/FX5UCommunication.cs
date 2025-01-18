
internal class FX5UCommunication : IPLCCommunication
{
    public Task<bool> ConnectAsync(string ipAddress, int port)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DisconnectAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsConnectedAsync()
    {
        throw new NotImplementedException();
    }

    public Task<object> ReadDataAsync(string address)
    {
        throw new NotImplementedException();
    }

    public Task<bool> WriteDataAsync(string address, object value)
    {
        throw new NotImplementedException();
    }
}