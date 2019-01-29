using System;
using System.Net;
using System.Threading;
using NLog;

namespace DateTimeBroadcaster
{
    internal class DateTimeReporter
    {
        #region Log

        private Logger logger = LogManager.GetCurrentClassLogger();

        #endregion 

        internal string ID { get; set; }

        internal string QueryString { get; private set; }

        internal IPEndPoint UserEndPoint { get; private set; }

        internal bool IsWorking { get; private set; }

        private DateTimeService _server = null;

        internal DateTimeReporter(DateTimeService server, string id, string queryString, IPEndPoint userEndPoint)
        {
            _server = server;
            ID = id;
            QueryString = queryString;
            UserEndPoint = userEndPoint;

            logger.Info($".ctor() created with ID {ID}, queryString {QueryString}, userEndPoint {UserEndPoint}.");
        }

        internal void Init()
        {
            logger.Info($"Init() #{ID} enter.");
            IsWorking = true;

            new Thread(new ThreadStart(() =>
            {
                while (IsWorking)
                {
                    try
                    {
                        _server.Send_Internal($"{DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss.ffffff")}");
                    }
                    catch (Exception ex)
                    {
                        logger.Info($"DateTimeReporter worker #{ID} error, {ex.Message}.");
                    }
                    finally
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                }

                logger.Info($"Init() worker #{ID} done loop.");
            }))
            { IsBackground = true, Name = $"DateTimeReporter #{ID}" }.Start();
        }

        internal void ShutdownSystem()
        {
            IsWorking = false;

            logger.Info($"ShutdownSystem() #{ID} leave.");
        }
    }
}
