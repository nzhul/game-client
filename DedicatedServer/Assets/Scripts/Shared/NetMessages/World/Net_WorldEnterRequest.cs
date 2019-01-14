using System;

[Serializable]
public class Net_WorldEnterRequest : NetMessage
{
    public Net_WorldEnterRequest()
    {
        OperationCode = NetOperationCode.WorldEnterRequest;
    }

    public int UserId { get; set; }

    public int CurrentRealmId { get; set; }

    public int[] RegionsForLoading { get; set; }
}