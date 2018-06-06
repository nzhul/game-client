using System;

namespace BestHTTP.SignalRCore.Messages
{
    [System.Serializable]
    public struct InvocationMessage
    {
        public MessageTypes type;
        public string invocationId;
        public bool nonblocking;
        public string target;
        public object[] arguments;
    }

    [System.Serializable]
    public struct CancelInvocationMessage
    {
        public MessageTypes type { get { return MessageTypes.CancelInvocation; } }
        public string invocationId;
    }
}