//using Assets.Scripts.InGame.Map.Entities;
//using Assets.Scripts.Shared.Models;
//using UnityEngine;

//public class NPCView : NodeContent
//{
//    public Army npc;

//    public override void Interact(AliveContent interactingHero)
//    {
//        // 1. Check if monster pack is locked.
//        if (npc.NPCData.IsLocked)
//        {
//            // TODO: display pop notification that user cannot interact with the monster until X minutes are passed.
//            return;
//        }

//        // 2. Open ConfirmBattle modal
//        ConfirmBattleModal.Instance.Open(BattleScenario.HUvsAI, interactingHero.rawEntity as Army, this.npc);
//    }

//    public void Init(Army monster, Vector3 worldPosition)
//    {
//        this.npc = monster;
//        gameObject.name = "Monster (" + monster.X + "," + monster.Y + ")";
//        gameObject.transform.position = worldPosition;

//        // InitGraphic();
//    }
//}