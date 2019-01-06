using System;

[Serializable]
public class Net_ConnectRequest : NetMessage
{
    public Net_ConnectRequest()
    {
        OperationCode = NetOperationCode.ConnectRequest;
    }

    public int Id { get; set; }

    public string Username { get; set; }

    public string Token { get; set; }

    public bool IsValid()
    {
        bool result = true;

        if (this.Id == 0)
        {
            result = false;
        }

        if (string.IsNullOrEmpty(this.Username))
        {
            result = false;
        }

        if (string.IsNullOrEmpty(this.Token))
        {
            result = false;
        }

        return result;
    }
}