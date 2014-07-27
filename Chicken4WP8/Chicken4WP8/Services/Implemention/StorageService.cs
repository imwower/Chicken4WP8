﻿using System.IO;
using System.Linq;
using Chicken4WP8.Common;
using Chicken4WP8.Controllers;
using Chicken4WP8.Entities;
using Chicken4WP8.Models.Setting;
using Chicken4WP8.Services.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Chicken4WP8.Services.Implemention
{
    public class StorageService : IStorageService
    {
        #region properties
        private static JsonSerializer serializer;
        private ChickenDataContext context;

        static StorageService()
        {
            serializer = JsonSerializer.Create(Const.JsonSettings);

            var ctx = new ChickenDataContext();
            if (!ctx.DatabaseExists())
            {
                ctx.CreateDatabase();
            }
        }

        public StorageService()
        {
            this.context = new ChickenDataContext();
        }
        #endregion

        public UserSetting GetCurrentUserSetting()
        {
            var entity = context.Settings.FirstOrDefault(s => s.Category == SettingCategory.CurrentUserSetting && s.IsCurrentlyInUsed);
            if (entity == null || entity.Data == null)
                return null;
            return DeserializeObject<UserSetting>(entity.Data);
        }

        public void UpdateCurrentUserSetting(UserSetting setting)
        {
            var entity = context.Settings.FirstOrDefault(s => s.Category == SettingCategory.CurrentUserSetting && s.IsCurrentlyInUsed);
            if (entity == null) //add new
            {
                entity = new Setting
                {
                    Category = SettingCategory.CurrentUserSetting,
                    Name = setting.Name,
                };
                context.Settings.InsertOnSubmit(entity);
            }
            entity.IsCurrentlyInUsed = true;
            entity.Data = SerializeObject(setting);
            context.SubmitChanges();
        }

        public string GetCurrentLanguage()
        {
            var setting = context.Settings.FirstOrDefault(s => s.Category == SettingCategory.LanguageSetting && s.IsCurrentlyInUsed);
            if (setting != null)
                return setting.Name;
            return string.Empty;
        }

        public void UpdateLanguage(string name)
        {
            var setting = context.Settings.FirstOrDefault(s => s.Category == SettingCategory.LanguageSetting && s.IsCurrentlyInUsed);
            if (setting == null)
            {
                setting = new Setting
                {
                    Category = SettingCategory.LanguageSetting,
                };
                context.Settings.InsertOnSubmit(setting);
            }
            setting.IsCurrentlyInUsed = true;
            setting.Name = name;
            context.SubmitChanges();
        }

        public ITweetModel GetTempTweet()
        {
            var entity = context.TempDatas.FirstOrDefault(t => t.Type == TempType.TweetDetail);
            if (entity == null || entity.Data == null)
                return null;
            return DeserializeObject<ITweetModel>(entity.Data);
        }

        public void UpdateTempTweet(ITweetModel tweet)
        {
            var entity = context.TempDatas.FirstOrDefault(t => t.Type == TempType.TweetDetail);
            if (entity == null)
            {
                entity = new TempData { Type = TempType.TweetDetail };
                context.TempDatas.InsertOnSubmit(entity);
            }
            entity.Data = SerializeObject(tweet);
            context.SubmitChanges();
        }

        public IUserModel GetTempUser()
        {
            var entity = context.TempDatas.FirstOrDefault(t => t.Type == TempType.UserProfile);
            if (entity == null || entity.Data == null)
                return null;
            return DeserializeObject<IUserModel>(entity.Data);
        }

        public void UpdateTempUser(IUserModel profile)
        {
            var entity = context.TempDatas.FirstOrDefault(t => t.Type == TempType.UserProfile);
            if (entity == null)
            {
                entity = new TempData { Type = TempType.UserProfile };
                context.TempDatas.InsertOnSubmit(entity);
            }
            entity.Data = SerializeObject(profile);
            context.SubmitChanges();
        }

        public byte[] GetCachedImage(string id)
        {
            CachedImage image = null;
            if (string.IsNullOrEmpty(id))
                image = context.CachedImages.FirstOrDefault(c => c.Id == id);
            if (image != null)
                return image.Data;
            return null;
        }

        public void AddOrUpdateImageCache(string id, byte[] data)
        {
            var image = context.CachedImages.FirstOrDefault(c => c.Id == id);
            if (image == null)
            {
                image = new CachedImage { Id = id };
                context.CachedImages.InsertOnSubmit(image);
            }
            image.Data = data;
            context.SubmitChanges();
        }

        #region private
        private byte[] SerializeObject(object value)
        {
            using (var memoryStream = new MemoryStream())
            {
                BsonWriter writer = new BsonWriter(memoryStream);
                serializer.Serialize(writer, value);
                return memoryStream.ToArray();
            }
        }

        private T DeserializeObject<T>(byte[] data)
        {
            var result = default(T);
            using (var memoryStream = new MemoryStream(data))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                BsonReader reader = new BsonReader(memoryStream);
                result = serializer.Deserialize<T>(reader);
            }
            return result;
        }
        #endregion
    }
}
