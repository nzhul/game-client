using Assets.Scripts.Shared.Models;

namespace Assets.Scripts.Network.Services.TCP.Interfaces
{
    public interface IBattleService
    {
        void SwitchTurn(Battle battle);

        void NullifyHeroPoints(int heroId, bool isDefend);

        void NullifyUnitPoints(int heroId, int unitId, bool isDefend);

        void EndBattle(Battle battle, int winnerId);
    }
}
