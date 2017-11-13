namespace System.Security.Permissions
{
    using System;
    using System.Security;
    using System.Windows;

    [Serializable]
    public sealed class MediaPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        private MediaPermissionAudio _mediaPermissionAudio;
        private MediaPermissionImage _mediaPermissionImage;
        private MediaPermissionVideo _mediaPermissionVideo;

        public MediaPermission()
        {
            this.InitDefaults();
        }

        public MediaPermission(MediaPermissionAudio permissionAudio)
        {
            VerifyMediaPermissionAudio(permissionAudio);
            this.InitDefaults();
            this._mediaPermissionAudio = permissionAudio;
        }

        public MediaPermission(MediaPermissionImage permissionImage)
        {
            VerifyMediaPermissionImage(permissionImage);
            this.InitDefaults();
            this._mediaPermissionImage = permissionImage;
        }

        public MediaPermission(MediaPermissionVideo permissionVideo)
        {
            VerifyMediaPermissionVideo(permissionVideo);
            this.InitDefaults();
            this._mediaPermissionVideo = permissionVideo;
        }

        public MediaPermission(PermissionState state)
        {
            if (state == PermissionState.Unrestricted)
            {
                this._mediaPermissionAudio = MediaPermissionAudio.AllAudio;
                this._mediaPermissionVideo = MediaPermissionVideo.AllVideo;
                this._mediaPermissionImage = MediaPermissionImage.AllImage;
            }
            else
            {
                if (state != PermissionState.None)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidPermissionState"));
                }
                this._mediaPermissionAudio = MediaPermissionAudio.NoAudio;
                this._mediaPermissionVideo = MediaPermissionVideo.NoVideo;
                this._mediaPermissionImage = MediaPermissionImage.NoImage;
            }
        }

        public MediaPermission(MediaPermissionAudio permissionAudio, MediaPermissionVideo permissionVideo, MediaPermissionImage permissionImage)
        {
            VerifyMediaPermissionAudio(permissionAudio);
            VerifyMediaPermissionVideo(permissionVideo);
            VerifyMediaPermissionImage(permissionImage);
            this._mediaPermissionAudio = permissionAudio;
            this._mediaPermissionVideo = permissionVideo;
            this._mediaPermissionImage = permissionImage;
        }

        public override IPermission Copy() => 
            new MediaPermission(this._mediaPermissionAudio, this._mediaPermissionVideo, this._mediaPermissionImage);

        private bool EqualsLevel(MediaPermissionAudio audioLevel, MediaPermissionVideo videoLevel, MediaPermissionImage imageLevel) => 
            (((this._mediaPermissionAudio == audioLevel) && (this._mediaPermissionVideo == videoLevel)) && (this._mediaPermissionImage == imageLevel));

        public override void FromXml(SecurityElement securityElement)
        {
            if (securityElement == null)
            {
                throw new ArgumentNullException("securityElement");
            }
            string str = securityElement.Attribute("class");
            if ((str == null) || (str.IndexOf(base.GetType().FullName, StringComparison.Ordinal) == -1))
            {
                throw new ArgumentNullException("securityElement");
            }
            string str2 = securityElement.Attribute("Unrestricted");
            if ((str2 != null) && bool.Parse(str2))
            {
                this._mediaPermissionAudio = MediaPermissionAudio.AllAudio;
                this._mediaPermissionVideo = MediaPermissionVideo.AllVideo;
                this._mediaPermissionImage = MediaPermissionImage.AllImage;
            }
            else
            {
                this.InitDefaults();
                string str3 = securityElement.Attribute("Audio");
                if (str3 == null)
                {
                    throw new ArgumentException(System.Windows.SR.Get("BadXml", new object[] { "audio" }));
                }
                this._mediaPermissionAudio = (MediaPermissionAudio) Enum.Parse(typeof(MediaPermissionAudio), str3);
                string str4 = securityElement.Attribute("Video");
                if (str4 == null)
                {
                    throw new ArgumentException(System.Windows.SR.Get("BadXml", new object[] { "video" }));
                }
                this._mediaPermissionVideo = (MediaPermissionVideo) Enum.Parse(typeof(MediaPermissionVideo), str4);
                string str5 = securityElement.Attribute("Image");
                if (str5 == null)
                {
                    throw new ArgumentException(System.Windows.SR.Get("BadXml", new object[] { "image" }));
                }
                this._mediaPermissionImage = (MediaPermissionImage) Enum.Parse(typeof(MediaPermissionImage), str5);
            }
        }

        private void InitDefaults()
        {
            this._mediaPermissionAudio = MediaPermissionAudio.SafeAudio;
            this._mediaPermissionVideo = MediaPermissionVideo.SafeVideo;
            this._mediaPermissionImage = MediaPermissionImage.SafeImage;
        }

        public override IPermission Intersect(IPermission target)
        {
            if (target == null)
            {
                return null;
            }
            MediaPermission permission = target as MediaPermission;
            if (permission == null)
            {
                throw new ArgumentException(System.Windows.SR.Get("TargetNotMediaPermissionLevel"));
            }
            MediaPermissionAudio permissionAudio = (this._mediaPermissionAudio < permission._mediaPermissionAudio) ? this._mediaPermissionAudio : permission._mediaPermissionAudio;
            MediaPermissionVideo permissionVideo = (this._mediaPermissionVideo < permission._mediaPermissionVideo) ? this._mediaPermissionVideo : permission._mediaPermissionVideo;
            MediaPermissionImage permissionImage = (this._mediaPermissionImage < permission._mediaPermissionImage) ? this._mediaPermissionImage : permission._mediaPermissionImage;
            if (((permissionAudio == MediaPermissionAudio.NoAudio) && (permissionVideo == MediaPermissionVideo.NoVideo)) && (permissionImage == MediaPermissionImage.NoImage))
            {
                return null;
            }
            return new MediaPermission(permissionAudio, permissionVideo, permissionImage);
        }

        public override bool IsSubsetOf(IPermission target)
        {
            if (target == null)
            {
                return this.EqualsLevel(MediaPermissionAudio.NoAudio, MediaPermissionVideo.NoVideo, MediaPermissionImage.NoImage);
            }
            MediaPermission permission = target as MediaPermission;
            if (permission == null)
            {
                throw new ArgumentException(System.Windows.SR.Get("TargetNotMediaPermissionLevel"));
            }
            return (((this._mediaPermissionAudio <= permission._mediaPermissionAudio) && (this._mediaPermissionVideo <= permission._mediaPermissionVideo)) && (this._mediaPermissionImage <= permission._mediaPermissionImage));
        }

        public bool IsUnrestricted() => 
            this.EqualsLevel(MediaPermissionAudio.AllAudio, MediaPermissionVideo.AllVideo, MediaPermissionImage.AllImage);

        public override SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("IPermission");
            element.AddAttribute("class", base.GetType().AssemblyQualifiedName);
            element.AddAttribute("version", "1");
            if (this.IsUnrestricted())
            {
                element.AddAttribute("Unrestricted", bool.TrueString);
                return element;
            }
            element.AddAttribute("Audio", this._mediaPermissionAudio.ToString());
            element.AddAttribute("Video", this._mediaPermissionVideo.ToString());
            element.AddAttribute("Image", this._mediaPermissionImage.ToString());
            return element;
        }

        public override IPermission Union(IPermission target)
        {
            if (target == null)
            {
                return this.Copy();
            }
            MediaPermission permission = target as MediaPermission;
            if (permission == null)
            {
                throw new ArgumentException(System.Windows.SR.Get("TargetNotMediaPermissionLevel"));
            }
            MediaPermissionAudio permissionAudio = (this._mediaPermissionAudio > permission._mediaPermissionAudio) ? this._mediaPermissionAudio : permission._mediaPermissionAudio;
            MediaPermissionVideo permissionVideo = (this._mediaPermissionVideo > permission._mediaPermissionVideo) ? this._mediaPermissionVideo : permission._mediaPermissionVideo;
            MediaPermissionImage permissionImage = (this._mediaPermissionImage > permission._mediaPermissionImage) ? this._mediaPermissionImage : permission._mediaPermissionImage;
            if (((permissionAudio == MediaPermissionAudio.NoAudio) && (permissionVideo == MediaPermissionVideo.NoVideo)) && (permissionImage == MediaPermissionImage.NoImage))
            {
                return null;
            }
            return new MediaPermission(permissionAudio, permissionVideo, permissionImage);
        }

        internal static void VerifyMediaPermissionAudio(MediaPermissionAudio level)
        {
            if ((level < MediaPermissionAudio.NoAudio) || (level > MediaPermissionAudio.AllAudio))
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPermissionLevel"));
            }
        }

        internal static void VerifyMediaPermissionImage(MediaPermissionImage level)
        {
            if ((level < MediaPermissionImage.NoImage) || (level > MediaPermissionImage.AllImage))
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPermissionLevel"));
            }
        }

        internal static void VerifyMediaPermissionVideo(MediaPermissionVideo level)
        {
            if ((level < MediaPermissionVideo.NoVideo) || (level > MediaPermissionVideo.AllVideo))
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPermissionLevel"));
            }
        }

        public MediaPermissionAudio Audio =>
            this._mediaPermissionAudio;

        public MediaPermissionImage Image =>
            this._mediaPermissionImage;

        public MediaPermissionVideo Video =>
            this._mediaPermissionVideo;
    }
}

