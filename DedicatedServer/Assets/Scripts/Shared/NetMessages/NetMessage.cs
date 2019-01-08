using System;

[Serializable]
public abstract class NetMessage
{
    public byte OperationCode { get; set; }

    public NetMessage()
    {
        OperationCode = NetOperationCode.None;
    }
}

public static class NetOperationCode
{
    // Shared messages
    public const int None = 0;

    // Client -> Server messages
    public const int AuthRequest = 2;
    public const int LoadRegionsRequest = 3;

    // Server -> Client messages
    public const int OnAuthRequest = 4;
}
