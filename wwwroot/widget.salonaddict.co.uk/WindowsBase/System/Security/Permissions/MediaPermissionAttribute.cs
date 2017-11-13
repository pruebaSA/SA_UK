namespace System.Security.Permissions
{
    using System;
    using System.Security;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
    public sealed class MediaPermissionAttribute : CodeAccessSecurityAttribute
    {
        private MediaPermissionAudio _mediaPermissionAudio;
        private MediaPermissionImage _mediaPermissionImage;
        private MediaPermissionVideo _mediaPermissionVideo;

        public MediaPermissionAttribute(SecurityAction action) : base(action)
        {
        }

        public override IPermission CreatePermission()
        {
            if (base.Unrestricted)
            {
                return new MediaPermission(PermissionState.Unrestricted);
            }
            return new MediaPermission(this._mediaPermissionAudio, this._mediaPermissionVideo, this._mediaPermissionImage);
        }

        public MediaPermissionAudio Audio
        {
            get => 
                this._mediaPermissionAudio;
            set
            {
                MediaPermission.VerifyMediaPermissionAudio(value);
                this._mediaPermissionAudio = value;
            }
        }

        public MediaPermissionImage Image
        {
            get => 
                this._mediaPermissionImage;
            set
            {
                MediaPermission.VerifyMediaPermissionImage(value);
                this._mediaPermissionImage = value;
            }
        }

        public MediaPermissionVideo Video
        {
            get => 
                this._mediaPermissionVideo;
            set
            {
                MediaPermission.VerifyMediaPermissionVideo(value);
                this._mediaPermissionVideo = value;
            }
        }
    }
}

