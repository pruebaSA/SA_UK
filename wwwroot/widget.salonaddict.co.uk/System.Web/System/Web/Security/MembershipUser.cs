namespace System.Web.Security
{
    using System;
    using System.Configuration.Provider;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [Serializable, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class MembershipUser
    {
        private string _Comment;
        private DateTime _CreationDate;
        private string _Email;
        private bool _IsApproved;
        private bool _IsLockedOut;
        private DateTime _LastActivityDate;
        private DateTime _LastLockoutDate;
        private DateTime _LastLoginDate;
        private DateTime _LastPasswordChangedDate;
        private string _PasswordQuestion;
        private string _ProviderName;
        private object _ProviderUserKey;
        private string _UserName;

        protected MembershipUser()
        {
        }

        public MembershipUser(string providerName, string name, object providerUserKey, string email, string passwordQuestion, string comment, bool isApproved, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate)
        {
            if ((providerName == null) || (Membership.Providers[providerName] == null))
            {
                throw new ArgumentException(System.Web.SR.GetString("Membership_provider_name_invalid"), "providerName");
            }
            if (name != null)
            {
                name = name.Trim();
            }
            if (email != null)
            {
                email = email.Trim();
            }
            if (passwordQuestion != null)
            {
                passwordQuestion = passwordQuestion.Trim();
            }
            this._ProviderName = providerName;
            this._UserName = name;
            this._ProviderUserKey = providerUserKey;
            this._Email = email;
            this._PasswordQuestion = passwordQuestion;
            this._Comment = comment;
            this._IsApproved = isApproved;
            this._IsLockedOut = isLockedOut;
            this._CreationDate = creationDate.ToUniversalTime();
            this._LastLoginDate = lastLoginDate.ToUniversalTime();
            this._LastActivityDate = lastActivityDate.ToUniversalTime();
            this._LastPasswordChangedDate = lastPasswordChangedDate.ToUniversalTime();
            this._LastLockoutDate = lastLockoutDate.ToUniversalTime();
        }

        public virtual bool ChangePassword(string oldPassword, string newPassword)
        {
            SecUtility.CheckPasswordParameter(ref oldPassword, 0, "oldPassword");
            SecUtility.CheckPasswordParameter(ref newPassword, 0, "newPassword");
            if (!Membership.Providers[this.ProviderName].ChangePassword(this.UserName, oldPassword, newPassword))
            {
                return false;
            }
            this.UpdateSelf();
            return true;
        }

        internal bool ChangePassword(string oldPassword, string newPassword, bool throwOnError)
        {
            bool flag = false;
            try
            {
                flag = this.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                if (throwOnError)
                {
                    throw;
                }
            }
            catch (MembershipPasswordException)
            {
                if (throwOnError)
                {
                    throw;
                }
            }
            catch (ProviderException)
            {
                if (throwOnError)
                {
                    throw;
                }
            }
            return flag;
        }

        public virtual bool ChangePasswordQuestionAndAnswer(string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            SecUtility.CheckPasswordParameter(ref password, 0, "password");
            SecUtility.CheckParameter(ref newPasswordQuestion, false, true, false, 0, "newPasswordQuestion");
            SecUtility.CheckParameter(ref newPasswordAnswer, false, true, false, 0, "newPasswordAnswer");
            if (!Membership.Providers[this.ProviderName].ChangePasswordQuestionAndAnswer(this.UserName, password, newPasswordQuestion, newPasswordAnswer))
            {
                return false;
            }
            this.UpdateSelf();
            return true;
        }

        public virtual string GetPassword() => 
            Membership.Providers[this.ProviderName].GetPassword(this.UserName, null);

        internal string GetPassword(bool throwOnError) => 
            this.GetPassword(null, false, throwOnError);

        public virtual string GetPassword(string passwordAnswer) => 
            Membership.Providers[this.ProviderName].GetPassword(this.UserName, passwordAnswer);

        internal string GetPassword(string answer, bool throwOnError) => 
            this.GetPassword(answer, true, throwOnError);

        private string GetPassword(string answer, bool useAnswer, bool throwOnError)
        {
            string password = null;
            try
            {
                if (useAnswer)
                {
                    return this.GetPassword(answer);
                }
                password = this.GetPassword();
            }
            catch (ArgumentException)
            {
                if (throwOnError)
                {
                    throw;
                }
            }
            catch (MembershipPasswordException)
            {
                if (throwOnError)
                {
                    throw;
                }
            }
            catch (ProviderException)
            {
                if (throwOnError)
                {
                    throw;
                }
            }
            return password;
        }

        public virtual string ResetPassword() => 
            this.ResetPassword((string) null);

        internal string ResetPassword(bool throwOnError) => 
            this.ResetPassword(null, false, throwOnError);

        public virtual string ResetPassword(string passwordAnswer)
        {
            string str = Membership.Providers[this.ProviderName].ResetPassword(this.UserName, passwordAnswer);
            if (!string.IsNullOrEmpty(str))
            {
                this.UpdateSelf();
            }
            return str;
        }

        internal string ResetPassword(string passwordAnswer, bool throwOnError) => 
            this.ResetPassword(passwordAnswer, true, throwOnError);

        private string ResetPassword(string passwordAnswer, bool useAnswer, bool throwOnError)
        {
            string str = null;
            try
            {
                if (useAnswer)
                {
                    return this.ResetPassword(passwordAnswer);
                }
                str = this.ResetPassword();
            }
            catch (ArgumentException)
            {
                if (throwOnError)
                {
                    throw;
                }
            }
            catch (MembershipPasswordException)
            {
                if (throwOnError)
                {
                    throw;
                }
            }
            catch (ProviderException)
            {
                if (throwOnError)
                {
                    throw;
                }
            }
            return str;
        }

        public override string ToString() => 
            this.UserName;

        public virtual bool UnlockUser()
        {
            if (Membership.Providers[this.ProviderName].UnlockUser(this.UserName))
            {
                this.UpdateSelf();
                return !this.IsLockedOut;
            }
            return false;
        }

        internal virtual void Update()
        {
            Membership.Providers[this.ProviderName].UpdateUser(this);
            this.UpdateSelf();
        }

        private void UpdateSelf()
        {
            MembershipUser user = Membership.Providers[this.ProviderName].GetUser(this.UserName, false);
            if (user != null)
            {
                try
                {
                    this._LastPasswordChangedDate = user.LastPasswordChangedDate.ToUniversalTime();
                }
                catch (NotSupportedException)
                {
                }
                try
                {
                    this.LastActivityDate = user.LastActivityDate;
                }
                catch (NotSupportedException)
                {
                }
                try
                {
                    this.LastLoginDate = user.LastLoginDate;
                }
                catch (NotSupportedException)
                {
                }
                try
                {
                    this._CreationDate = user.CreationDate.ToUniversalTime();
                }
                catch (NotSupportedException)
                {
                }
                try
                {
                    this._LastLockoutDate = user.LastLockoutDate.ToUniversalTime();
                }
                catch (NotSupportedException)
                {
                }
                try
                {
                    this._IsLockedOut = user.IsLockedOut;
                }
                catch (NotSupportedException)
                {
                }
                try
                {
                    this.IsApproved = user.IsApproved;
                }
                catch (NotSupportedException)
                {
                }
                try
                {
                    this.Comment = user.Comment;
                }
                catch (NotSupportedException)
                {
                }
                try
                {
                    this._PasswordQuestion = user.PasswordQuestion;
                }
                catch (NotSupportedException)
                {
                }
                try
                {
                    this.Email = user.Email;
                }
                catch (NotSupportedException)
                {
                }
                try
                {
                    this._ProviderUserKey = user.ProviderUserKey;
                }
                catch (NotSupportedException)
                {
                }
            }
        }

        public virtual string Comment
        {
            get => 
                this._Comment;
            set
            {
                this._Comment = value;
            }
        }

        public virtual DateTime CreationDate =>
            this._CreationDate.ToLocalTime();

        public virtual string Email
        {
            get => 
                this._Email;
            set
            {
                this._Email = value;
            }
        }

        public virtual bool IsApproved
        {
            get => 
                this._IsApproved;
            set
            {
                this._IsApproved = value;
            }
        }

        public virtual bool IsLockedOut =>
            this._IsLockedOut;

        public bool IsOnline
        {
            get
            {
                TimeSpan span = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
                DateTime time = DateTime.UtcNow.Subtract(span);
                return (this.LastActivityDate.ToUniversalTime() > time);
            }
        }

        public virtual DateTime LastActivityDate
        {
            get => 
                this._LastActivityDate.ToLocalTime();
            set
            {
                this._LastActivityDate = value.ToUniversalTime();
            }
        }

        public virtual DateTime LastLockoutDate =>
            this._LastLockoutDate.ToLocalTime();

        public virtual DateTime LastLoginDate
        {
            get => 
                this._LastLoginDate.ToLocalTime();
            set
            {
                this._LastLoginDate = value.ToUniversalTime();
            }
        }

        public virtual DateTime LastPasswordChangedDate =>
            this._LastPasswordChangedDate.ToLocalTime();

        public virtual string PasswordQuestion =>
            this._PasswordQuestion;

        public virtual string ProviderName =>
            this._ProviderName;

        public virtual object ProviderUserKey =>
            this._ProviderUserKey;

        public virtual string UserName =>
            this._UserName;
    }
}

