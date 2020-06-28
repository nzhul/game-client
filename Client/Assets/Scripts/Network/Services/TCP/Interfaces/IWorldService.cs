using Assets.Scripts.Shared.Models;

namespace Assets.Scripts.Network.Services.TCP.Interfaces
{
    public interface IWorldService
    {
        /// <summary>
        /// Force the dedicated server to:
        /// 1. Load current user avatar
        /// 2. Update users current realm
        /// 3. Load user regions inMemory.
        /// </summary>
        /// <param name="id">Current user id in the database</param>
        /// <param name="currentRealmId"></param>
        /// <param name="regionsForLoading"></param>
        //void WorldEnterRequest(int id, int currentRealmId, int[] regionsForLoading);

        /// <summary>
        /// Sends request for teleport to the dedicated server.
        /// 1. DDServer validates the request and sends back OnTeleport message.
        /// 2. Client listens for OnTeleport messages and execute the teleport.
        ///     - Cases:
        ///         1. self teleport -> the requester is the one that is teleporting
        ///         2. other player -> a player teleports in or out of our map.
        /// </summary>
        /// <param name="heroId">Id of the hero that is teleporting</param>
        /// <param name="regionId">Target region. The destination of the teleport</param>
        /// <param name="dwellingId">Target dwelling. The destination of the teleport. Usually it will a waypoint dwelling, but can also be Castle or other</param>
        void TeleportRequest(int heroId, int regionId, int dwellingId);

        /// <summary>
        /// Sends request for entering the matchmaking pool.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="faction"></param>
        /// <param name="heroClass"></param>
        void FindOpponentRequest(CreatureType heroClass);

        /// <summary>
        /// Sends CancelFindOpponentRequest message for leaving the matchmaking pool. 
        /// </summary>
        void CancelFindOpponentRequest();
    }
}
