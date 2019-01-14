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
        /// <param name="id"></param>
        /// <param name="currentRealmId"></param>
        /// <param name="regionsForLoading"></param>
        void SendWorldEnterRequest(int id, int currentRealmId, int[] regionsForLoading);
    }
}
