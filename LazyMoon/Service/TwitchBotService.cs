using System.IO;
using System;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Communication.Models;
using TwitchLib.Client.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using LazyMoon.Class;
using System.Threading;
using TwitchLib.Communication.Events;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace LazyMoon.Service
{
    public class TwitchBotService
    {
        public enum EBotUseService
        {
            TTS,
            ValorantRank
        }
        public EventHandler<OnMessageReceivedArgs> OnMessageReceived;
        public string Chanel { get; set; } = "";

        TwitchClient client;
        public string OAuth { get; set; }

        private string mLastMessage = "";

        private readonly IConfiguration configuration;

        public bool ExistTTS { get; set; } = false;
        public bool ExistValorantRank { get; set; } = false;
        public TwitchBotService(IConfiguration configuration)
        {
            this.configuration = configuration;
            SetOAuth(this.configuration.GetValue<string>("TwitchOAuth:OAuth"));
        }
        public void SetOAuth(string oAuth)
        {
            OAuth = oAuth;
        }
        public bool SetBot(string chanel, EBotUseService eBotUseService)
        {
            if (client == null)
            {
                switch (eBotUseService)
                {
                    case EBotUseService.TTS:
                        ExistTTS = true;
                        break;
                    case EBotUseService.ValorantRank:
                        ExistValorantRank = true;
                        break;
                    default:
                        break;
                }
                if (Chanel == chanel)
                    return false;
                Chanel = chanel;

                try
                {
                    ConnectionCredentials credentials = new ConnectionCredentials(chanel, OAuth);
                    var clientOptions = new ClientOptions
                    {
                        MessagesAllowedInPeriod = 750,
                        ThrottlingPeriod = TimeSpan.FromSeconds(30)
                    };
                    WebSocketClient customClient = new WebSocketClient(clientOptions);
                    client = new TwitchClient(customClient);
                    client.Initialize(credentials, this.Chanel);
                    client.OnMessageReceived += async (s, e) => { await Client_OnMessageReceived(s, e); };
                    client.OnDisconnected += Client_OnDisconnected;
                    client.Connect();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            {
                if (client.IsConnected == false)
                    client.Connect();
                client.JoinChannel(chanel);
            }
            return true;
        }

        class ChanelTime
        {
            public string Chanel { get; set; }
            public DateTime Time { get; set; }
        }
        List<ChanelTime> ChanelTimeList = new List<ChanelTime>();

        public void SendMessage(string chanel, string message)
        {
            var chanelTime = ChanelTimeList.SingleOrDefault(x => x.Chanel == chanel);
            if (chanelTime != null)
            {
                if (chanelTime.Time > DateTime.Now)
                {
                    return;
                }
                else
                {
                    ChanelTimeList.Remove(chanelTime);
                }
            }
            try
            {
                if (mLastMessage == message)
                    message = message + "ㅤ";
                client.SendMessage(chanel, message);
            }
            catch (Exception e)
            {
                client.JoinChannel(Chanel);
                client.SendMessage(chanel, message);
            }
            mLastMessage = message;
            ChanelTimeList.Add(new ChanelTime() { Chanel = chanel, Time = DateTime.Now + TimeSpan.FromMilliseconds(200) });
        }

        private async Task Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            await Task.Factory.StartNew(() => OnMessageReceived?.Invoke(sender, e));
        }

        private void Client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            int tryCount = 0;
            while (tryCount < 100)
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected == true)
                    {
                        client.JoinChannel(Chanel);
                        break;
                    }
                }
                catch
                {

                }
                tryCount++;
                Thread.Sleep(100);
            }
        }
    }
}
