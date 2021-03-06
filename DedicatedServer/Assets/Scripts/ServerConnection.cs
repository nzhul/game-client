﻿
using System.Linq;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.DataModels.Units;
/// <summary>
/// This class will hold the information for every player connection:
/// His connectionId, Token, UserInformation, Loaded Regions and other information.
/// </summary>
public class ServerConnection
{
    /// <summary>
    /// Database Id
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Unity connection Id
    /// </summary>
    public int ConnectionId { get; set; }

    public string Username { get; set; }

    public string Token { get; set; }

    public int CurrentRealmId { get; set; }

    public UserAvatar Avatar { get; set; }

    public int[] RegionIds => Avatar?.Heroes?.Select(h => h.RegionId).ToArray();
}
