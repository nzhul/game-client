namespace Assets.Scripts.Network.Services
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
        void SendWorldEnterRequest(int id, int currentRealmId, int[] regionsForLoading);

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
        void SendTeleportRequest(int heroId, int regionId, int dwellingId);

        /// <summary>
        /// Sends request for entering the matchmaking queue.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="faction"></param>
        /// <param name="heroClass"></param>
        void SendFindOpponentRequest(int userId, string faction, string heroClass);
    }
}
