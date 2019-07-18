using System;

namespace Assets.Scripts.Network.Services
{
    public interface IBattleService
    {
        void SendConfirmLoadingBattleSceneMessage(Guid battleId, int heroId, bool isReady);
    }
}
