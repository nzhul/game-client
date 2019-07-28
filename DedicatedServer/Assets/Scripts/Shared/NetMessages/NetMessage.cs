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

    /// <summary>
    /// Sends request for teleport to the dedicated server.
    /// 1. DDServer validates the request and sends back OnTeleport message.
    /// 2. Client listens for OnTeleport messages and execute the teleport.
    ///     - Cases:
    ///         1. self teleport -> the requester is the one that is teleporting
    ///         2. other player -> a player teleports in or out of our map.
    /// </summary>
    public const int TeleportRequest = 5;

    /// <summary>
    /// Sends request for starting a battle to the dedicated server.
    /// 1. DDServer checks if the monster pack is not locked and sends back OnStartBattle message.
    /// 2. Client listens for this message and starts the battle by loading separate "battle" scene.
    /// </summary>
    public const int StartBattleRequest = 6;

    /// <summary>
    /// Each player should send this message after he is done loading the battle scene.
    /// Then we know that the battle can start.
    /// </summary>
    public const int ConfirmLoadingBattleScene = 7;

    /// <summary>
    /// A player that currently have an active turn can send an EndTurnRequest 
    /// to end his turn befire his time expires
    /// The player should not be able to send such request if current turn is not 
    /// his or there is X time left from his turn.
    /// </summary>
    public const int EndTurnRequest = 8;
    #endregion


    #region ServerClient

    public const int OnAuthRequest = 100;

    public const int OnWorldEnter = 101;

    public const int OnMapMovement = 102;

    public const int OnTeleport = 103;

    public const int OnStartBattle = 104;

    public const int SwitchTurnEvent = 105;

    #endregion
}


// NOTES:
// BATTLE MESSAGES:
// BattleMovementRequest -> Moves the init in the battlefield and consumes his movement point.
// BattleActionRequest 
//  -> Unit/Hero perform an action.
//  -> Each action cost action points (AP).
//  -> Units will usually have 1-2 action points availible and their abilities will cost 1-2 AP
//  -> Normal Attack form example will cost 1 AP and the unit can have a pool of 1 AP. This means that the unit can only perform normal attack
//  -> Heroes on other hand will have bigger AP pool. For example 5 AP.
//  -> TODO: consider having separate AP and movement pool or common one.

// Every time an ActionRequest is executed on the server we check the win condition for both players
// if some of them is a winner -> we send EndBattleEvent to both clients and record the outcome of the battle in the database (api call).

// EndBattleEvent -> Notify both looser and winner about the outcome of the battle.
// Winner: Recieve information about his award, experience gain and so on.
// Looser: Recieve information about his loses.

// TODO: Change "END TURN" button to "DEFEND" -> only skip turn of the current creature. Like "DEFEND" button in Heroes 3.
