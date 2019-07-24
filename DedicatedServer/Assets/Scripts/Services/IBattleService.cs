using Assets.Scripts.Shared.NetMessages.Battle.Models;

namespace Assets.Scripts.Services
{
    public interface IBattleService
    {
        void SwitchTurn(Battle battle);
    }
}
