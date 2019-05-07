using Assets.Scripts.InGame.Map.Entities;
using Assets.Scripts.Network;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World;
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

        // 2. Send StartBattleRequest message to the server.
        Net_StartBattleRequest msg = new Net_StartBattleRequest
        {
            HeroId = interactingHero.hero.id,
            MonsterId = this.monster.id
        };

        NetworkClient.Instance.SendServer(msg);
    }

    public void Init(MonsterPack monster, Vector3 worldPosition)
    {
        this.monster = monster;
        gameObject.name = "Monster (" + monster.x + "," + monster.y + ")";
        gameObject.transform.position = worldPosition;

        // InitGraphic();
    }
}