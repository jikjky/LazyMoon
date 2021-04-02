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

namespace LazyMoon.Class
{
    public delegate void OnMessage(string TTSMessage);

    public class TwitchBot
    {
        public class TwitchOAuth
        {
            public string OAuth;
            public string ClientId;
            public string AccessToken;

            public TwitchOAuth(FileInfo OAuthFile)
            {
                if (OAuthFile.Exists == false)
                {
                    return ;
                }
                var text = OAuthFile.OpenText();
                OAuth = text.ReadLine();
                ClientId = text.ReadLine();
                AccessToken = text.ReadLine();
                text.Close();
                return ;
            }
        }

        public event OnMessage OnMessage;

        public bool IsTTSOn = true;

        private string mLastMessage = "";

        TwitchClient client;

        TwitchAPI api;

        public Class.ValorantRank valorantRank;

        public TTS tts;

        class TTSQueueInfo
        {
            public string message;
            public string name;
        }

        Queue<TTSQueueInfo> TTSQueue = new Queue<TTSQueueInfo>();

        public TwitchOAuth twitchOauth;

        const string chanel = "jikjky";

        public TimeSpan tempTimeSpan = TimeSpan.FromSeconds(0);

        public TimeSpan Uptime 
        { 
            get 
            {
                return tempTimeSpan;
            } 
        }
        //생성자
        public TwitchBot()
        {

            tts = new TTS();
            valorantRank = new Class.ValorantRank();

            twitchOauth = new TwitchOAuth(new FileInfo("wwwroot//twitch.oauth"));

            ConnectionCredentials credentials = new ConnectionCredentials(chanel, twitchOauth.OAuth);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, chanel);

            client.OnMessageReceived += Client_OnMessageReceived;

            client.Connect();


            api = new TwitchAPI();
            api.Settings.ClientId = twitchOauth.ClientId;
            api.Settings.AccessToken = twitchOauth.AccessToken;

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
                                tts.Speak(ttsInfo.message, ttsInfo.name);
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
                while (true)
                {
                    var foundChannelResponse = api.V5.Users.GetUserByNameAsync(chanel).Result;
                    var foundChannel = foundChannelResponse.Matches.FirstOrDefault();
                    var a = api.V5.Streams.GetUptimeAsync(foundChannel.Id).Result;

                    if (a != null)
                    {
                        tempTimeSpan = a.Value;
                        lastTime = tempTimeSpan;
                    }
                    else
                    {
                        tempTimeSpan = lastTime;
                    }
                    Thread.Sleep(1000);
                }
            })
            { IsBackground = true }
            .Start();
        }

        //Bot

        /// <summary>
        /// 메세지 적기
        /// </summary>
        /// <param name="message"></param>
        private void SendMessage(string message)
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
            mLastMessage = message;
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string speechText = e.ChatMessage.Message;
            if (e.ChatMessage.Username == "jikjky")
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
                                tts.SetTTSEnable(true);
                                SendMessage($"TTS Enable");
                            }
                            else if (splitText[1] == "off")
                            {
                                tts.SetTTSEnable(false);
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
                                tts.SetTTSRate(value);
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
                                tts.SetTTSVolume(value);
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
                        tts.SetTTSDefault();
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
                e.ChatMessage.Username == "moonlazy")
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
                                tts.SetVoicePitch(e.ChatMessage.Username, temp);
                                isError = false;
                                var tempInfo = tts.GetVoiceInfo(e.ChatMessage.Username);
                                SendMessage($"{e.ChatMessage.Username} Your Setting is \r\nGender : {tempInfo.Gender.ToString()} \r\nPicth : {tempInfo.Pitch}");
                            }
                        }
                    }
                    if (isError == true)
                    {
                        SendMessage("!pitch [-20, 20]");
                    }
                }
                else if (splitText[0].ToLower() == "!gender")
                {
                    bool isError = true;
                    if (splitText.Length > 1)
                    {
                        if (splitText[1].ToLower() == "male")
                        {
                            tts.SetVoiceGender(e.ChatMessage.Username, TTS.EGender.Male);
                            isError = false;
                            var tempInfo = tts.GetVoiceInfo(e.ChatMessage.Username);
                            SendMessage($"{e.ChatMessage.Username} Your Setting is \r\nGender : {tempInfo.Gender.ToString()} \r\nPicth : {tempInfo.Pitch}");
                        }
                        else if (splitText[1].ToLower() == "female")
                        {
                            tts.SetVoiceGender(e.ChatMessage.Username, TTS.EGender.Female);
                            isError = false;
                            var tempInfo = tts.GetVoiceInfo(e.ChatMessage.Username);
                            SendMessage($"{e.ChatMessage.Username} Your Setting is \nGender : {tempInfo.Gender.ToString()} \nPicth : {tempInfo.Pitch}");
                        }
                    }
                    if (isError == true)
                    {
                        SendMessage("!gender [male, female]");
                    }
                }
                else if (splitText[0].ToLower() == "!tts")
                {
                    bool isError = true;
                    if (splitText.Length > 1)
                    {
                        if (splitText[1].ToLower() == "on")
                        {
                            tts.SetTTS(e.ChatMessage.Username, true);
                            isError = false;
                            var tempInfo = tts.GetVoiceInfo(e.ChatMessage.Username);
                            SendMessage($"{e.ChatMessage.Username} TTS On");
                        }
                        else if (splitText[1].ToLower() == "off")
                        {
                            tts.SetTTS(e.ChatMessage.Username, false);
                            isError = false;
                            var tempInfo = tts.GetVoiceInfo(e.ChatMessage.Username);
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
            stringList.Add('ㄳ');
            stringList.Add('ㄷ');
            stringList.Add('ㅅ');
            stringList.Add('ㅋ');
            stringList.Add('ㅎ');
            stringList.Add('ㅂ');
            stringList.Add('ㅈ');
            stringList.Add('ㄴ');
            stringList.Add('ㅇ');
            stringList.Add('ㄹ');
            stringList.Add('ㅌ');
            stringList.Add('ㅊ');
            stringList.Add('ㅍ');
            stringList.Add('ㅁ');

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
            speechText = speechText.Replace("ㅅ", "샷");    
            speechText = speechText.Replace("ㄱ", "고");
            speechText = speechText.Replace("ㄷ", "덜");
            speechText = speechText.Replace("ㅋ", "키");
            speechText = speechText.Replace("ㅎ", "히");
            speechText = speechText.Replace("ㅂ", "바");
            speechText = speechText.Replace("ㅈ", "지");
            speechText = speechText.Replace("ㄴ", "노");
            speechText = speechText.Replace("ㅇ", "응");
            speechText = speechText.Replace("ㄹ", "리");
            speechText = speechText.Replace("ㅌ", "튀");
            speechText = speechText.Replace("ㅊ", "추");
            speechText = speechText.Replace("ㅍ", "팜");
            speechText = speechText.Replace("ㅁ", "미");

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
            TTSQueue.Enqueue(new TTSQueueInfo() { message = speechText, name = e.ChatMessage.Username });
            if(OnMessage != null)
                OnMessage(e.ChatMessage.Message);
        }
    }
}
