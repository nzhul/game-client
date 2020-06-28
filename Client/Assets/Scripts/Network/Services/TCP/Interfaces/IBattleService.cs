using System;

namespace Assets.Scripts.Network.Services.TCP.Interfaces
{
    public interface IBattleService
    {
        void SendConfirmLoadingBattleSceneMessage(Guid battleId, int armyId, bool isReady);

        void SendEndTurnRequest(Guid battleId, int currentArmyId, int currentUnitId);
    }
}
