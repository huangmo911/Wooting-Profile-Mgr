using System.Collections.Generic;
using System.IO;
using System.Threading;
using ProtoBuf;
using WootingProtocol.Protocol;

namespace WootingProtocol
{
    [ProtoContract]
    public class JLAI_WootingProfile
    {
        [ProtoMember(1)] public KeyboardProfile KeyboardProfile { get; set; }
        [ProtoMember(2)] public ProfileMetadata ProfileMetadata { get; set; }
        [ProtoMember(3)] public byte[] ActuationProfile { get; set; }
        [ProtoMember(4)] public byte[] FullRapidTriggerProfile { get; set; }
    }

    [ProtoContract]
    public class JlaiWootingProfileList
    {
        [ProtoMember(1)]
        public List<JLAI_WootingProfile> Profiles { get; set; } = new();
        [ProtoMember(2)]
        public int CurrentProfileId { get; set; }
    }

    public static class WootingProfileEx
    {
        /// <summary>  
        /// 保存当前Wooting配置文件到指定路径  
        /// </summary>  
        /// <param name="wootingProtocol"></param>  
        /// <param name="filePath"></param>  
        public static void Save(this WootingProtocol wootingProtocol, string filePath)
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
                    ActuationProfile = actuationProfile,
                    FullRapidTriggerProfile = wootingProtocol.GetFullRapidTriggerProfile(i)
                });
            }

            File.WriteAllBytes(filePath, Serialize(jlaiWootingProfileList));
        }

        /// <summary>  
        /// 从指定路径加载Wooting配置文件  
        /// </summary>  
        /// <param name="wootingProtocol"></param>  
        /// <param name="filePath"></param>  
        public static void Load(this WootingProtocol wootingProtocol, string filePath)
        {
            if (!File.Exists(filePath)) return;

            var jlaiWootingProfileList = Deserialize<JlaiWootingProfileList>(File.ReadAllBytes(filePath));
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

                wootingProtocol.SetActuationProfile(i, profile.ActuationProfile);
                Thread.Sleep(200);

                wootingProtocol.SetFullRapidTriggerProfile(i, profile.FullRapidTriggerProfile);
            }

            wootingProtocol.ActivateProfile(jlaiWootingProfileList.CurrentProfileId);
        }

        private static byte[] Serialize<T>(T obj)
        {
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, obj);
            return ms.ToArray();
        }

        private static T Deserialize<T>(byte[] data)
        {
            using var ms = new MemoryStream(data);
            return Serializer.Deserialize<T>(ms);
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