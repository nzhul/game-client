using Assets.Scripts.InGame.Map.Entities;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World.Models;
using UnityEngine;

public class NPCView : MonoBehaviour, IInteractable
{
    public Hero npc;

    public void Interact(HeroView interactingHero)
    {
        // 1. Check if monster pack is locked.
        if (npc.NpcData.IsLocked)
        {
            // TODO: display pop notification that user cannot interact with the monster until X minutes are passed.
            return;
        }

        // 2. Open ConfirmBattle modal
        ConfirmBattleModal.Instance.Open(BattleScenario.HUvsMonsterAI, interactingHero.hero, this.npc);
    }

    public void Init(Hero monster, Vector3 worldPosition)
    {
        this.npc = monster;
        gameObject.name = "Monster (" + monster.X + "," + monster.Y + ")";
        gameObject.transform.position = worldPosition;

        // InitGraphic();
    }
}