using System;

namespace Assets.Scripts.Network.Shared.NetMessages.Users
{
    [Serializable]
    public class Net_AuthRequest : NetMessage
    {
        public Net_AuthRequest()
        {
            OperationCode = NetOperationCode.AuthRequest;
        }

        public int Id { get; set; }

        public string Username { get; set; }

        public int MMR { get; set; }

        public string Token { get; set; }

        public bool IsValid()
        {
            bool result = true;

            if (this.Id == 0)
            {
                result = false;
            }

            if (string.IsNullOrEmpty(this.Username))
            {
                result = false;
            }

            if (string.IsNullOrEmpty(this.Token))
            {
                result = false;
            }

            return result;
        }
    }
}