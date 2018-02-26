using System.Collections.Generic;
using NLog;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace DateTimeBroadcaster
{
    internal class DateTimeService : WebSocketBehavior
    {
        #region Log

        private NLog.Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        private Dictionary<string, DateTimeReporter> _clients = new Dictionary<string, DateTimeReporter>();

        protected override void OnOpen()
        {
            logger.Info("OnOpen() ID {0}.", ID);

            DateTimeReporter reporter = new DateTimeReporter(this, ID, Context.QueryString.ToString(), Context.UserEndPoint);
            _clients.Add(ID, reporter);
            reporter?.Init();

            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            logger.Info("OnClose() ID {0}, {1}, {2}, {3}.", ID, e.Code, e.Reason, e.WasClean);

            DateTimeReporter client;
            if (_clients.TryGetValue(ID, out client))
            {
                client?.ShutdownSystem();
            }

            base.OnClose(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            logger.Info("OnError() ID {0}, message {1}.", ID, e.Message);

            DateTimeReporter client;
            if (_clients.TryGetValue(ID, out client))
            {
                client?.ShutdownSystem();
            }

            base.OnError(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            logger.Info("OnMessage() ID {0}, data {1}.", ID, e.Data);
        }

        internal void Send_Internal(string data)
        {
            Send(data);
        }
    }
}
