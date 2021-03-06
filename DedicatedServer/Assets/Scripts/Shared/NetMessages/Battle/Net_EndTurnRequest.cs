﻿using System;

namespace Assets.Scripts.Shared.NetMessages.Battle
{
    [Serializable]
    public class Net_EndTurnRequest : NetMessage
    {
        public Net_EndTurnRequest()
        {
            this.OperationCode = NetOperationCode.EndTurnRequest;
        }

        public Guid BattleId { get; set; }

        public int RequesterHeroId { get; set; }

        public int RequesterUnitId { get; set; }

        public bool IsDefend { get; set; }

        public bool IsValid()
        {
            if (this.BattleId == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }
}
