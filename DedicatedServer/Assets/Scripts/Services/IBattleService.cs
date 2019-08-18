using Assets.Scripts.Shared.NetMessages.Battle.Models;

namespace Assets.Scripts.Services
{
    public interface IBattleService
    {
        void SwitchTurn(Battle battle);

        void NullifyHeroPoints(int heroId, bool isDefend);

        void NullifyUnitPoints(int heroId, int unitId, bool isDefend);
    }
}
