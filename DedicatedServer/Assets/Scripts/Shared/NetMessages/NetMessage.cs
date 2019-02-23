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
    public const int None = 0;

    #region ClientServer

    public const int AuthRequest = 1;
    public const int LoadRegionsRequest = 2;

    /// <summary>
    /// World enter request will force the dedicated server to load all the information about the user avatar
    /// together with the required regions
    /// </summary>
    public const int WorldEnterRequest = 3;

    /// <summary>
    /// Request entity movement
    /// Do validation and if success -> send OnMovement message that contains the updated coordinates.
    /// </summary>
    public const int MapMovementRequest = 4;
    #endregion


    #region ServerClient

    public const int OnAuthRequest = 100;

    public const int OnWorldEnter = 101;

    /// <summary>
    /// Sends a collection of entities what need to be moved.
    /// </summary>
    public const int OnMapMovement = 102;

    #endregion
}
