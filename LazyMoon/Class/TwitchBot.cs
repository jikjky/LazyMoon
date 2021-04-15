using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.V5.Models.Subscriptions;
using TwitchLib.Communication.Models;
using Google.Cloud.TextToSpeech.V1;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;

namespace LazyMoon.Class
{
    public delegate void OnMessage(string TTSMessage);

    public class TwitchBot
    {
        Log4NetManager Log = Log4NetManager.GetInstance();
        Global global = Global.GetInstance();

        public class TwitchOAuth
        {
            public string OAuth;
            public string ClientId;
            public string AccessToken;

            public TwitchOAuth(FileInfo OAuthFile)
            {
                if (OAuthFile.Exists == false)
                {
                    return;
                }
                var text = OAuthFile.OpenText();
                OAuth = text.ReadLine();
                ClientId = text.ReadLine();
                AccessToken = text.ReadLine();
                text.Close();
                return;
            }
        }

        public TwitchOAuth twitchOauth;

        public TTS tts;

        TwitchAPI api;

        List<Bot> botList = new List<Bot>();

        //생성자
        public TwitchBot()
        {
            tts = new TTS();
            twitchOauth = new TwitchOAuth(new FileInfo("wwwroot//twitch.oauth"));

            Log.TwitchBotLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Create Api");
            api = new TwitchAPI();
            api.Settings.ClientId = twitchOauth.ClientId;
            api.Settings.AccessToken = twitchOauth.AccessToken;
            Encryption.Encryption.SetDefaultPassword(twitchOauth.AccessToken);

            foreach (var item in global.UserInfo.ToList())
            {
                var bot = new Bot();
                bot.SetBot(this, item.Value.Name);
                botList.Add(bot);
            }
        }

        public Global.TwitchUser Login(string authToken)
        {
            var user = api.V5.Users.GetUserAsync(authToken).Result;

            if (botList.Find(x => x.chanel == user.Name) == null)
            {
                var bot = new Bot();
                bot.SetBot(this, user.Name);
                botList.Add(bot);
            }
            return global.SetUserInfo(user.Name, user.Id);
        }

        public Bot GetBot(string chanel, bool ttsUse = false)
        {
            var bot = botList.Find(x => x.chanel == chanel);
            if (bot == null)
            {
                return null;
            }
            bot.IsOnTTSWakeUp = ttsUse;
            return bot;
        }

        public class Bot
        {
            public bool isRunning;

            public event OnMessage OnMessage;

            public bool IsTTSOn = true;

            public bool IsOnTTSWakeUp = false;

            private string mLastMessage = "";

            TwitchClient client;

            public Class.ValorantRank valorantRank;

            class TTSQueueInfo
            {
                public string message;
                public string name;
            }

            Queue<TTSQueueInfo> TTSQueue = new Queue<TTSQueueInfo>();

            public string chanel = "";

            public TimeSpan tempTimeSpan = TimeSpan.FromSeconds(0);

            TwitchBot twitchBot;

            public TimeSpan Uptime
            {
                get
                {
                    return tempTimeSpan;
                }
            }

            public void SetBot(TwitchBot twitchBot, string chanel)
            {
                isRunning = true;
                this.chanel = chanel;
                this.twitchBot = twitchBot;
                twitchBot.Log.TwitchBotLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Create TwitchBot Instance");
                valorantRank = new Class.ValorantRank();
                valorantRank.LoadData(chanel);

                twitchBot.Log.TwitchBotLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SetTwitchOauth");

                twitchBot.Log.TwitchBotLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Set ClientOption");
                ConnectionCredentials credentials = new ConnectionCredentials(chanel, twitchBot.twitchOauth.OAuth);
                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                };
                WebSocketClient customClient = new WebSocketClient(clientOptions);
                twitchBot.Log.TwitchBotLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Create Client");
                client = new TwitchClient(customClient);
                client.Initialize(credentials, chanel);

                client.OnMessageReceived += Client_OnMessageReceived;

                twitchBot.Log.TwitchBotLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "Client Connetct");
                client.Connect();

                new Thread(() =>
                {
                    while (true)
                    {
                        if (TTSQueue.Count > 0)
                        {
                            if (TTSQueue.Count < 10)
                            {
                                if (IsTTSOn == true)
                                {
                                    var ttsInfo = TTSQueue.Dequeue();
                                    twitchBot.tts.Speak(ttsInfo.message, ttsInfo.name, chanel);
                                }
                                else
                                {
                                    TTSQueue.Dequeue();
                                }
                            }
                            else
                            {
                                TTSQueue.Dequeue();
                                continue;
                            }

                        }
                        Thread.Sleep(100);
                    }
                })
                { IsBackground = true }
                .Start();

                new Thread(() =>
                {
                    TimeSpan lastTime = TimeSpan.FromMilliseconds(0);
                    try
                    {
                        var foundChannelResponse = twitchBot.api.V5.Users.GetUserByNameAsync(chanel).Result;
                        while (true)
                        {

                            var foundChannel = foundChannelResponse.Matches.FirstOrDefault();
                            var a = twitchBot.api.V5.Streams.GetUptimeAsync(foundChannel.Id).Result;

                            if (a != null)
                            {
                                tempTimeSpan = a.Value;
                                lastTime = tempTimeSpan;
                            }
                            else
                            {
                                tempTimeSpan = lastTime;
                            }
                            Thread.Sleep(300);
                        }
                    }
                    catch (Exception e)
                    {
                        twitchBot.Log.TwitchBotLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "Uptime Update Error : " + e.Message);
                    }
                })
                { IsBackground = true }
                .Start();
            }

            /// <summary>
            /// 메세지 적기
            /// </summary>
            /// <param name="message"></param>
            private void SendMessage(string message)
            {
                try
                {
                    if (mLastMessage != message)
                    {
                        client.SendMessage(chanel, message);
                    }
                    else
                    {
                        message = message + "ㅤ";
                        client.SendMessage(chanel, message);
                    }
                }
                catch (Exception e)
                {
                    twitchBot.Log.TwitchBotLog.SetLog(LogManager.Log4NetBase.eLogType.Error, "SendMessage Chanel Error : " + chanel + " Message : " + message + " Exception : " + e.Message);
                    client.JoinChannel(chanel);
                    client.SendMessage(chanel, message);
                }

                twitchBot.Log.TwitchBotLog.SetLog(LogManager.Log4NetBase.eLogType.Info, "SendMessage Chanel : " + chanel + " Message : " + message);
                mLastMessage = message;
            }

            private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
            {
                string speechText = e.ChatMessage.Message;
                if (e.ChatMessage.Username == chanel)
                {
                    if (e.ChatMessage.Message[0] == '!')
                    {
                        var splitText = e.ChatMessage.Message.Split(' ');
                        bool isError = false;

                        int score;
                        if (splitText[0].ToLower() == "!add")
                        {
                            if (splitText.Length > 1)
                            {
                                if (int.TryParse(splitText[1], out score))
                                {
                                    valorantRank.ChangeRank(score, "");
                                }
                                else
                                    isError = true;
                            }
                            else
                                isError = true;
                            if (isError)
                                SendMessage($"!add [0~2147483647]");
                        }
                        else if (splitText[0].ToLower() == "!sub")
                        {
                            if (splitText.Length > 1)
                            {
                                if (int.TryParse(splitText[1], out score))
                                {
                                    valorantRank.ChangeRank(score * -1, "");
                                }
                                else
                                    isError = true;
                            }
                            else
                                isError = true;
                            if (isError)
                                SendMessage($"!sub [0~2147483647]");
                        }
                        else if (splitText[0].ToLower() == "!setrank")
                        {
                            if (splitText.Length > 1)
                            {
                                valorantRank.ChangeRank(0, splitText[1]);
                            }
                            else
                                isError = true;
                            if (isError)
                                SendMessage($"!setrank [rank name]");
                        }
                        else if (splitText[0].ToLower() == "!ttsenable")
                        {
                            if (splitText.Length > 1)
                            {
                                if (splitText[1] == "on")
                                {
                                    twitchBot.global.SetTTSEnable(chanel, true);
                                    SendMessage($"TTS Enable");
                                }
                                else if (splitText[1] == "off")
                                {
                                    twitchBot.global.SetTTSEnable(chanel, false);
                                    SendMessage($"TTS Disabled");
                                }
                                else
                                    isError = true;
                            }
                            else
                                isError = true;

                            if (isError)
                                SendMessage($"!ttsenable [on, off]");
                        }
                        else if (splitText[0].ToLower() == "!ttsrate")
                        {
                            if (splitText.Length > 1)
                            {
                                double value;
                                if (double.TryParse(splitText[1], out value))
                                {
                                    twitchBot.global.SetTTSRate(chanel, value);
                                    SendMessage($"Set TTS Rate {value} [min : 0.25 , max :4.0]");
                                }
                                else
                                    isError = true;
                            }
                            else
                                isError = true;

                            if (isError)
                                SendMessage($"!ttsrate [min : 0.25 , max :4.0]");
                        }
                        else if (splitText[0].ToLower() == "!ttsvolume")
                        {
                            if (splitText.Length > 1)
                            {
                                double value;
                                if (double.TryParse(splitText[1], out value))
                                {
                                    twitchBot.global.SetTTSVolume(chanel, value);
                                    SendMessage($"Set TTS Volume {value} [min : -96 , max :16]");
                                }
                                else
                                    isError = true;
                            }
                            else
                                isError = true;

                            if (isError)
                                SendMessage($"!ttsvolume [min : -96 , max :16]");
                        }
                        else if (splitText[0].ToLower() == "!ttsdefault")
                        {
                            twitchBot.global.SetTTSDefault(chanel);
                            SendMessage($"Set TTS Volume 0, Set TTS Rate 1");
                        }
                        else if (splitText[0].ToLower() == "!help")
                        {
                            SendMessage($"add, sub, setrank, ttsenable, ttsrate, ttsvolume, ttsdefault");
                        }
                    }
                }

                // 유저이름이 봇일 경우 무시
                if (e.ChatMessage.Username == "nightbot" ||
                    e.ChatMessage.Username == "ssakdook" ||
                    e.ChatMessage.Username == "moonlazy" ||
                    e.ChatMessage.Username == "lazymoonbot")
                {
                    return;
                }

                // 보이스 정보 저장 루틴
                if (e.ChatMessage.Message[0] == '!')
                {
                    var splitText = e.ChatMessage.Message.Split(' ');
                    //Pitch
                    if (splitText[0].ToLower() == "!pitch")
                    {
                        bool isError = true;
                        if (splitText.Length > 1)
                        {
                            double temp;
                            if (double.TryParse(splitText[1], out temp) == false)
                            {

                            }
                            else
                            {
                                if (temp >= -20 && temp <= 20)
                                {
                                    twitchBot.global.SetVoicePitch(chanel, e.ChatMessage.Username, temp);
                                    isError = false;
                                    var tempInfo = twitchBot.global.GetVoiceInfo(chanel, e.ChatMessage.Username);
                                    SendMessage($"{e.ChatMessage.Username} Your Setting is \r\nVoice : {tempInfo.Voice.ToString()} \r\nPicth : {tempInfo.Pitch}");
                                }
                            }
                        }
                        if (isError == true)
                        {
                            SendMessage("!pitch [-20, 20]");
                        }
                    }
                    else if (splitText[0].ToLower() == "!voice")
                    {
                        bool isError = true;
                        if (splitText.Length > 1)
                        {
                            if (splitText[1].ToLower() == "a")
                            {
                                twitchBot.global.SetVoice(chanel, e.ChatMessage.Username, Global.EVoice.A);
                                isError = false;
                                var tempInfo = twitchBot.global.GetVoiceInfo(chanel, e.ChatMessage.Username);
                                SendMessage($"{e.ChatMessage.Username} Your Setting is \nVoice : {tempInfo.Voice.ToString()} \nPicth : {tempInfo.Pitch}");
                            }
                            else if (splitText[1].ToLower() == "b")
                            {
                                twitchBot.global.SetVoice(chanel, e.ChatMessage.Username, Global.EVoice.B);
                                isError = false;
                                var tempInfo = twitchBot.global.GetVoiceInfo(chanel, e.ChatMessage.Username);
                                SendMessage($"{e.ChatMessage.Username} Your Setting is \nVoice : {tempInfo.Voice.ToString()} \nPicth : {tempInfo.Pitch}");
                            }
                            else if (splitText[1].ToLower() == "c")
                            {
                                twitchBot.global.SetVoice(chanel, e.ChatMessage.Username, Global.EVoice.C);
                                isError = false;
                                var tempInfo = twitchBot.global.GetVoiceInfo(chanel, e.ChatMessage.Username);
                                SendMessage($"{e.ChatMessage.Username} Your Setting is \nVoice : {tempInfo.Voice.ToString()} \nPicth : {tempInfo.Pitch}");
                            }
                            else if (splitText[1].ToLower() == "d")
                            {
                                twitchBot.global.SetVoice(chanel, e.ChatMessage.Username, Global.EVoice.D);
                                isError = false;
                                var tempInfo = twitchBot.global.GetVoiceInfo(chanel, e.ChatMessage.Username);
                                SendMessage($"{e.ChatMessage.Username} Your Setting is \nVoice : {tempInfo.Voice.ToString()} \nPicth : {tempInfo.Pitch}");
                            }
                        }
                        if (isError == true)
                        {
                            SendMessage("!Voice [a, b, c, d]");
                        }
                    }
                    else if (splitText[0].ToLower() == "!tts")
                    {
                        bool isError = true;
                        if (splitText.Length > 1)
                        {
                            if (splitText[1].ToLower() == "on")
                            {
                                twitchBot.global.SetTTS(chanel, e.ChatMessage.Username, true);
                                isError = false;
                                var tempInfo = twitchBot.global.GetVoiceInfo(chanel, e.ChatMessage.Username);
                                SendMessage($"{e.ChatMessage.Username} TTS On");
                            }
                            else if (splitText[1].ToLower() == "off")
                            {
                                twitchBot.global.SetTTS(chanel, e.ChatMessage.Username, false);
                                isError = false;
                                var tempInfo = twitchBot.global.GetVoiceInfo(chanel, e.ChatMessage.Username);
                                SendMessage($"{e.ChatMessage.Username} TTS Off");
                            }
                        }
                        if (isError == true)
                        {
                            SendMessage("!tts [on, off]");
                        }
                    }
                    return;
                }

                // 문자 처리
                List<char> stringList = new List<char>();
                stringList.Add('ㄱ');
                stringList.Add('ㄴ');
                stringList.Add('ㄷ');
                stringList.Add('ㄹ');
                stringList.Add('ㅁ');
                stringList.Add('ㅂ');
                stringList.Add('ㅅ');
                stringList.Add('ㅇ');
                stringList.Add('ㅈ');
                stringList.Add('ㅊ');
                stringList.Add('ㅋ');
                stringList.Add('ㅍ');
                stringList.Add('ㅌ');
                stringList.Add('ㅎ');
                stringList.Add('ㄳ');

                //자음 2글자 이상이면 2글자만 재생되게
                foreach (var item in stringList)
                {
                    int count = 0;
                    int index = 0;

                    List<int> indexList = new List<int>();
                    foreach (var text in speechText)
                    {
                        if (text == item)
                        {
                            count++;
                        }
                        else
                        {
                            count = 0;
                        }
                        if (count > 2)
                        {
                            indexList.Add(index);
                        }
                        index++;
                    }
                    indexList.Reverse();
                    foreach (var tempIndex in indexList)
                    {
                        speechText = speechText.Remove(tempIndex, 1);
                    }
                }

                // 특정 글자 변경
                speechText = speechText.Replace("ㄳ", "감사");
                speechText = speechText.Replace("ㄱㅅ", "감사");
                speechText = speechText.Replace("ㅅㄱ", "수고");
                speechText = speechText.Replace("ㅎㅇ", "하이");
                speechText = speechText.Replace("ㅂㅇ", "바이");
                speechText = speechText.Replace("ㅁㅇ", "미아");
                speechText = speechText.Replace("ㅇㄴ", "아나");
                speechText = speechText.Replace("ㅅㅂ", "야발");
                speechText = speechText.Replace("ㅄ", "모자란아이");
                speechText = speechText.Replace("시발", "야발");
                speechText = speechText.Replace("병신", "모자란아이");
                speechText = speechText.Replace("못한다", "잘한다");
                speechText = speechText.Replace("ㅈㄴ", "엄청");
                speechText = speechText.Replace("ㄱ", "고");
                speechText = speechText.Replace("ㄴ", "노");
                speechText = speechText.Replace("ㄷ", "덜");
                speechText = speechText.Replace("ㄹ", "리");
                speechText = speechText.Replace("ㅁ", "미");
                speechText = speechText.Replace("ㅂ", "바");
                speechText = speechText.Replace("ㅅ", "샷");
                speechText = speechText.Replace("ㅇ", "응");
                speechText = speechText.Replace("ㅈ", "지");
                speechText = speechText.Replace("ㅊ", "추");
                speechText = speechText.Replace("ㅋ", "키");
                speechText = speechText.Replace("ㅌ", "튀");
                speechText = speechText.Replace("ㅍ", "팜");
                speechText = speechText.Replace("ㅎ", "히");

                speechText = speechText.Replace("***", "나쁜말");

                // 이메일 문자 변환
                Regex pattern = new Regex(@"
                               \b                   #begin of word
                               (?<email>            #name for captured value
                                   [A-Z0-9._%+-]+   #capture one or more symboles mentioned in brackets
                                   @                #@ is required symbol in email per specification
                                   [A-Z0-9.-]+      #capture one or more symboles mentioned in brackets
                                   \.               #required dot
                                   [A-Z]{2,}        #should be more then 2 symboles A-Z at the end of email
                               )
                               \b                   #end of word
                                       ", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

                speechText = pattern.Replace(speechText, "이메일");


                // 링크 문자 변환
                string regex = @"((www\.|(http|https|ftp|news|file)+\:\/\/)[_.a-z0-9-]+\.[a-z0-9\/_:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";

                speechText = Regex.Replace(speechText, regex, "링크");


                // 문자 길이 제한
                if (speechText.Length > 25)
                {
                    speechText = speechText.Remove(25);
                }
                //speechText = ConvertEnglish.Instance.ConvertToEnglish(speechText);
                if (IsOnTTSWakeUp == true)
                    TTSQueue.Enqueue(new TTSQueueInfo() { message = speechText, name = e.ChatMessage.Username });
                if (OnMessage != null)
                    OnMessage(e.ChatMessage.Message);
            }
        }
    }
}
