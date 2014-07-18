﻿using System;
using System.Windows.Media;
using Caliburn.Micro;
using CoreTweet;
using Newtonsoft.Json;

namespace Chicken4WP8.Controllers.Implemention.Base
{
    public class UserModel : PropertyChangedBase, IUserModel, IImageSource
    {
        #region private
        private User user;
        #endregion

        public UserModel(User user)
        {
            this.user = user;
        }

        public User User
        {
            get { return user; }
        }

        public long? Id
        {
            get { return user.Id; }
        }

        public string Name
        {
            get { return user.Name; }
        }

        public string ScreenName
        {
            get { return user.ScreenName; }
        }

        public string Description
        {
            get { return user.Description; }
        }

        public DateTime CreatedAt
        {
            get { return user.CreatedAt.LocalDateTime; }
        }

        public string Location
        {
            get { return user.Location; }
        }

        public bool IsFollowing
        {
            get { return false; }
        }

        public bool IsVerified
        {
            get { return user.IsVerified; }
        }

        public bool IsPrivate
        {
            get { return user.IsProtected; }
        }

        public bool IsTranslator
        {
            get { return user.IsTranslator; }
        }

        public string ProfileImageUrl
        {
            get
            {
                if (user.ProfileImageUrl != null)
                    return user.ProfileImageUrl.AbsoluteUri.Replace("_normal", "_bigger");
                return string.Empty;
            }
        }

        private ImageSource profileImage;

        [JsonIgnore]
        public ImageSource ImageSource
        {
            get { return profileImage; }
            set
            {
                profileImage = value;
                NotifyOfPropertyChange(() => ImageSource);
            }
        }
    }
}