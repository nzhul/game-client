using Assets.Scripts.Data;
using Assets.Scripts.InGame.Map.Entities;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.Models.Units;
using UnityEngine;

[RequireComponent(typeof(UnitMotor))]
public class UnitView : AliveEntityView // N: Maybe i should rename UnitView to EntityView and use it as a base.
{
    public override bool IsFriendly
    {
        get
        {
            var team = DataManager.Instance.ActiveGame.GetArmy((rawEntity as Unit).ArmyId.Value).Team;
            return team == DataManager.Instance.Avatar.Team;
        }
    }
}
