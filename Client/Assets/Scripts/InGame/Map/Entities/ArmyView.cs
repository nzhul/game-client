using Assets.Scripts.Data;
using Assets.Scripts.InGame.Map.Entities;
using Assets.Scripts.InGame.Pathfinding;
using Assets.Scripts.Shared.Models;
using UnityEngine;

[RequireComponent(typeof(UnitMotor))]
public class ArmyView : AliveEntityView, IPathRequester
{
    public override bool IsFriendly
    {
        get
        {
            return DataManager.Instance.Avatar.Team == DataManager.Instance.ActiveGame.GetArmy(rawEntity.Id).Team;
        }
    }
}
