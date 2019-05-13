using Assets.Scripts.InGame.Map.Entities;
using Assets.Scripts.Shared.DataModels;
using UnityEngine;

public class MonsterView : MonoBehaviour, IInteractable
{
    public MonsterPack monster;

    public void Interact(HeroView interactingHero)
    {
        // 1. Check if monster pack is locked.
        if (monster.isLocked)
        {
            // TODO: display pop notification that user cannot interact with the monster until X minutes are passed.
            return;
        }

        // 2. Open ConfirmBattle modal
        ConfirmBattleModal.Instance.Open(interactingHero, this.monster);
    }

    public void Init(MonsterPack monster, Vector3 worldPosition)
    {
        this.monster = monster;
        gameObject.name = "Monster (" + monster.x + "," + monster.y + ")";
        gameObject.transform.position = worldPosition;

        // InitGraphic();
    }
}