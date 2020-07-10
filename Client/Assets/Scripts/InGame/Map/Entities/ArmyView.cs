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

    public override void Interact(AliveEntityView interactingHero)
    {
        //// 1. Check if monster pack is locked.
        //if (npc.NPCData.IsLocked)
        //{
        //    // TODO: display pop notification that user cannot interact with the monster until X minutes are passed.
        //    return;
        //}

        // 2. Open ConfirmBattle modal
        ConfirmBattleModal.Instance.Open(BattleScenario.HUvsAI, interactingHero.rawEntity as Army, base.rawEntity as Army);
    }
}
