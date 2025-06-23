using JLCommonality;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using WootingProtocol.Protocol;
using static System.Net.Mime.MediaTypeNames;

namespace WootingProtocol
{
    [ProtoContract]
    public class JLAI_WootingProfile
    {
        public KeyboardProfile KeyboardProfile { get; set; }
        public ProfileMetadata ProfileMetadata { get; set; }
        public string ActuationProfile { get; set; }
        public string FullRapidTriggerProfile { get; set; }
    }

    [ProtoContract]
    public class JlaiWootingProfileList
    {
        public List<JLAI_WootingProfile> Profiles { get; set; } = new();
        public int CurrentProfileId { get; set; }
    }

    public static class WootingProfileEx
    {
        /// <summary>  
        /// 保存当前Wooting配置文件到指定路径  
        /// </summary>  
        /// <param name="wootingProtocol"></param>  
        /// <param name="filePath"></param>  
        public static void Save(this WootingProtocol wootingProtocol, string filePath, bool encryption)
        {
            var profileCount = wootingProtocol.GetProfileCount();

            var jlaiWootingProfileList = new JlaiWootingProfileList
            {
                CurrentProfileId = wootingProtocol.GetCurrentKeyboardProfileIndex()
            };

            for (var i = 0; i < profileCount; i++)
            {
                var profileMetadata = wootingProtocol.GetProfileMetadata(i);
                var keyboardProfile = wootingProtocol.GetKeyboardProfile(i);
                var actuationProfile = wootingProtocol.GetActuationProfile(i);

                if (profileMetadata == null || keyboardProfile == null || actuationProfile == null) continue;

                jlaiWootingProfileList.Profiles.Add(new JLAI_WootingProfile
                {
                    KeyboardProfile = keyboardProfile,
                    ProfileMetadata = profileMetadata,
                    ActuationProfile = BitConverter.ToString(actuationProfile),
                    FullRapidTriggerProfile = BitConverter.ToString(wootingProtocol.GetFullRapidTriggerProfile(i))
                });
            }

            var text = JObject.FromObject(jlaiWootingProfileList).ToString();
            if (encryption)
            {
                text = WebComm.EncryptString(text);
            }

            File.WriteAllText(filePath, text);
        }

        /// <summary>  
        /// 从指定路径加载Wooting配置文件  
        /// </summary>  
        /// <param name="wootingProtocol"></param>  
        /// <param name="filePath"></param>  
        public static void Load(this WootingProtocol wootingProtocol, string filePath, bool encryption)
        {
            if (!File.Exists(filePath)) return;

            var text = File.ReadAllText(filePath);

            if (encryption)
            {
                text = WebComm.DecryptString(text);
            }

            var jlaiWootingProfileList = JObject.Parse(text).Value<JlaiWootingProfileList>();
            if (jlaiWootingProfileList?.Profiles == null || jlaiWootingProfileList.Profiles.Count == 0) return;

            AdjustProfileCount(wootingProtocol, jlaiWootingProfileList.Profiles.Count);

            Thread.Sleep(200);

            for (var i = 0; i < jlaiWootingProfileList.Profiles.Count; i++)
            {
                var profile = jlaiWootingProfileList.Profiles[i];

                wootingProtocol.SaveProfileMetadata(profile.ProfileMetadata, new ProfileReport { ProfileIndex = i, Save = true });
                Thread.Sleep(200);

                wootingProtocol.SetKeyboardProfile(profile.KeyboardProfile, new ProfileReport { ProfileIndex = i, Save = true });
                Thread.Sleep(200);

                wootingProtocol.SetActuationProfile(i, StringToByteArray(profile.ActuationProfile));
                Thread.Sleep(200);

                wootingProtocol.SetFullRapidTriggerProfile(i, StringToByteArray(profile.FullRapidTriggerProfile));
            }

            wootingProtocol.ActivateProfile(jlaiWootingProfileList.CurrentProfileId);
        }
        public static byte[] StringToByteArray(string hexString)
        {
            string[] hexValues = hexString.Split('-');
            byte[] bytes = new byte[hexValues.Length];

            for (int i = 0; i < hexValues.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexValues[i], 16);
            }

            return bytes;
        }
        private static void AdjustProfileCount(WootingProtocol wootingProtocol, int targetCount)
        {
            var currentCount = wootingProtocol.GetProfileCount();

            if (targetCount > currentCount)
            {
                for (var i = currentCount; i < targetCount; i++)
                {
                    wootingProtocol.CreateProfile(i);
                }
            }
            else if (targetCount < currentCount)
            {
                for (var i = targetCount; i < currentCount; i++)
                {
                    wootingProtocol.DeleteProfile(i);
                }
            }
        }
    }
}