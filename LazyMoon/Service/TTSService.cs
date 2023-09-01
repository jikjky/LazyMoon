using Google.Cloud.TextToSpeech.V1;
using Google.Protobuf.WellKnownTypes;
using LazyMoon.Class;
using LazyMoon.Model;
using LazyMoon.Model.DTO;
using LazyMoon.Pages.Bot;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client.Events;


namespace LazyMoon.Service
{
    public class TTSService
    {
        TextToSpeechClient ttsClient;

        public Action<string, string> OnSpeak;
        public Action<string> OnMessage;

        private readonly DBTTSService _dbTTSService;
        private readonly DBVoiceService _dbVoiceService;
        private readonly TwitchBotService _twitchBotService;
        private readonly IConfiguration _configuration;

        private string Chanel { get; set; } = "";


        public TTSService(DBTTSService dbTTSService, DBVoiceService dbVoiceService, TwitchBotService twitchBotService, IConfiguration configuration)
        {
            _dbTTSService = dbTTSService;
            _dbVoiceService = dbVoiceService;
            _twitchBotService = twitchBotService;
            _configuration = configuration;

            SetTTS(_configuration.GetValue<string>("TTS:jsonPath"));
        }

        public void SetBot(string chanel)
        {
            Chanel = chanel;
            _twitchBotService.SetBot(chanel, TwitchBotService.EBotUseService.TTS);
            var eventList = _twitchBotService.OnMessageReceived?.GetInvocationList();
            if (eventList != null)
            {
                foreach (var item in eventList)
                {
                    _twitchBotService.OnMessageReceived -= (EventHandler<OnMessageReceivedArgs>)item;
                }
            }
            _twitchBotService.OnMessageReceived += async (o, s) => await Client_OnMessageReceived(o, s);
            _twitchBotService.OnMessageReceived += async (o, s) => await OnMessageReceived(o, s);
        }

        private void SetTTS(string jsonPath)
        {
            var builder = new TextToSpeechClientBuilder
            {
                CredentialsPath = jsonPath
            };

            ttsClient = builder.Build();
            return;
        }

        public async Task SpeakAsync(string setText, string name, string chanel)
        {
            var ttsModel = await _dbTTSService.GetTTSByChanelOrNullAsync(chanel);
            Model.Voice voiceModel = await _dbVoiceService.GetVoiceOrNullAsync(ttsModel, name);

            var input = new SynthesisInput
            {
                Text = setText
            };

            var rd = new Random();

            EVoice eVoice = (EVoice)rd.Next(1, 5);
            VoiceSelectionParams voice;

            voice = new VoiceSelectionParams
            {
                LanguageCode = "ko-KR",
                Name = "ko-KR-Standard-" + eVoice.ToString(),
            };

            var config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3,
            };

            if (ttsModel != null && voiceModel != null)
            {
                if (ttsModel.TTSEnable == false || voiceModel.Use == false)
                {
                    return;
                }
                config.Pitch = voiceModel.Pitch;
                config.SpeakingRate = ttsModel.Rate;
                config.VolumeGainDb = ttsModel.Volume;
                voice.Name = "ko-KR-Standard-" + voiceModel.VoiceMode.ToString();
            }
            try
            {
                var response = await ttsClient.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
                {
                    Input = input,
                    Voice = voice,
                    AudioConfig = config
                });
                NotifyTTSSpeakEvent(response.AudioContent.ToBase64(), chanel);
            }
            catch
            {

            }

        }

        private void NotifyTTSSpeakEvent(string audioBase64, string chanel)
        {
            OnSpeak?.Invoke(audioBase64, chanel);
        }

        private void SendMessage(string message)
        {
            _twitchBotService.SendMessage(Chanel, message);
        }

        private async Task Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            await Client_OnMessageProcess(sender, e);
        }
        private async Task OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            await OnMessageProcess(sender, e);
        }

        private async Task OnMessageProcess(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Channel != Chanel)
                return;
            string chanel = Chanel;
            string speechText = e.ChatMessage.Message;

            // 유저이름이 봇일 경우 무시
            if (e.ChatMessage.Username == "nightbot" ||
                e.ChatMessage.Username == "ssakdook" ||
                e.ChatMessage.Username == "moonlazy" ||
                e.ChatMessage.Username == "lazymoonbot" ||
                e.ChatMessage.Message[0] == '!'
                )
            {
                return;
            }

            // 문자 처리
            var stringList = new List<char>
            {
                'ㄱ',
                'ㄴ',
                'ㄷ',
                'ㄹ',
                'ㅁ',
                'ㅂ',
                'ㅅ',
                'ㅇ',
                'ㅈ',
                'ㅊ',
                'ㅋ',
                'ㅍ',
                'ㅌ',
                'ㅎ',
                'ㄳ',
                'ㅄ',
                'ㄽ'
            };


            //자음 2글자 이상이면 2글자만 재생되게
            foreach (var item in stringList)
            {
                int count = 0;
                int index = 0;

                var indexList = new List<int>();
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
            speechText = speechText.Replace("ㅂㅅ", "모자란아이");
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
            var pattern = new Regex(@"
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
            if (OnMessage != null)
            {
                OnMessage(e.ChatMessage.Message);
                await SpeakAsync(speechText, e.ChatMessage.Username, chanel);
            }
        }

        private async Task Client_OnMessageProcess(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Channel != Chanel)
                return;
            string chanel = Chanel;
            if (e.ChatMessage.Username == chanel)
            {
                if (e.ChatMessage.Message[0] == '!')
                {
                    var splitText = e.ChatMessage.Message.Split(' ');
                    bool isError = false;

                    if (splitText[0].ToLower() == "!ttsenable")
                    {
                        if (splitText.Length > 1)
                        {
                            if (splitText[1] == "on")
                            {
                                await _dbTTSService.SetTTSEnableOrNullAsync(chanel, true);
                                SendMessage($"TTS Enable");
                            }
                            else if (splitText[1] == "off")
                            {
                                await _dbTTSService.SetTTSEnableOrNullAsync(chanel, false);
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
                            if (double.TryParse(splitText[1], out double value))
                            {
                                await _dbTTSService.SetTTSRateOrNullAsync(chanel, value);
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
                            if (double.TryParse(splitText[1], out double value))
                            {
                                await _dbTTSService.SetTTSVolumeOrNullAsync(chanel, value);
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
                        await _dbTTSService.SetTTSDefaultOrNullAsync(chanel);
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
                    bool isError = false;
                    if (splitText.Length > 1)
                    {
                        if (double.TryParse(splitText[1], out double score) == true)
                        {
                            if (score >= -20 && score <= 20)
                            {
                                await _dbVoiceService.SetVoicePitchOrNullAsync(chanel, e.ChatMessage.Username, score);

                                var tempInfo = _dbVoiceService.GetVoiceOrNullAsync(chanel, e.ChatMessage.Username).Result;
                                SendMessage($"{e.ChatMessage.Username} Your Setting is \r\nVoice : {tempInfo.VoiceMode} \r\nPicth : {tempInfo.Pitch}");
                            }
                        }
                        else
                            isError = true;
                    }
                    if (isError == true)
                    {
                        SendMessage("!pitch [-20, 20]");
                    }
                }
                else if (splitText[0].ToLower() == "!voice")
                {
                    bool isError = false;
                    if (splitText.Length > 1)
                    {
                        if (splitText[1].ToLower() == "a")
                            await _dbVoiceService.SetVoiceModeOrNullAsync(chanel, e.ChatMessage.Username, EVoice.A);
                        else if (splitText[1].ToLower() == "b")
                            await _dbVoiceService.SetVoiceModeOrNullAsync(chanel, e.ChatMessage.Username, EVoice.B);
                        else if (splitText[1].ToLower() == "c")
                            await _dbVoiceService.SetVoiceModeOrNullAsync(chanel, e.ChatMessage.Username, EVoice.C);
                        else if (splitText[1].ToLower() == "d")
                            await _dbVoiceService.SetVoiceModeOrNullAsync(chanel, e.ChatMessage.Username, EVoice.D);
                        else
                            isError = true;
                    }
                    if (isError == true)
                    {
                        SendMessage("!Voice [a, b, c, d]");
                    }
                    else
                    {
                        var tempInfo = await _dbVoiceService.GetVoiceOrNullAsync(chanel, e.ChatMessage.Username);
                        SendMessage($"{e.ChatMessage.Username} Your Setting is \nVoice : {tempInfo.VoiceMode} \nPicth : {tempInfo.Pitch}");
                    }
                }
                else if (splitText[0].ToLower() == "!tts")
                {
                    bool isError = false;
                    if (splitText.Length > 1)
                    {
                        if (splitText[1].ToLower() == "on")
                        {
                            await _dbVoiceService.SetVoiceEnableOrNullAsync(chanel, e.ChatMessage.Username, true);
                            SendMessage($"{e.ChatMessage.Username} TTS On");
                        }
                        else if (splitText[1].ToLower() == "off")
                        {
                            await _dbVoiceService.SetVoiceEnableOrNullAsync(chanel, e.ChatMessage.Username, false);
                            SendMessage($"{e.ChatMessage.Username} TTS Off");
                        }
                        else
                            isError = true;
                    }
                    if (isError == true)
                    {
                        SendMessage("!tts [on, off]");
                    }
                    else
                    {

                    }
                }
                return;
            }
        }
    }
}