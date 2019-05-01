using System;

[Serializable]
public class Net_OnWorldEnter : NetMessage
{
    public Net_OnWorldEnter()
    {
        OperationCode = NetOperationCode.OnWorldEnter;
    }

    public byte Success { get; set; }

    public string ErrorMessage { get; set; }
}