namespace System.Web.ClientServices.Providers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.Threading;
    using System.Web.ClientServices;
    using System.Web.Resources;
    using System.Web.Security;

    public class ClientWindowsAuthenticationMembershipProvider : MembershipProvider
    {
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotSupportedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotSupportedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotSupportedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotSupportedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotSupportedException();
        }

        public void Logout()
        {
            Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        public override bool UnlockUser(string username)
        {
            throw new NotSupportedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotSupportedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            if (!string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(AtlasWeb.ArgumentMustBeNull, "password");
            }
            if (!string.IsNullOrEmpty(username) && (string.Compare(username, current.Name, StringComparison.OrdinalIgnoreCase) != 0))
            {
                throw new ArgumentException(AtlasWeb.ArgumentMustBeNull, "username");
            }
            Thread.CurrentPrincipal = new ClientRolePrincipal(current);
            return true;
        }

        public override string ApplicationName
        {
            get => 
                "";
            set
            {
            }
        }

        public override bool EnablePasswordReset =>
            false;

        public override bool EnablePasswordRetrieval =>
            false;

        public override int MaxInvalidPasswordAttempts =>
            0x7fffffff;

        public override int MinRequiredNonAlphanumericCharacters =>
            0;

        public override int MinRequiredPasswordLength =>
            1;

        public override int PasswordAttemptWindow =>
            0x7fffffff;

        public override MembershipPasswordFormat PasswordFormat =>
            MembershipPasswordFormat.Hashed;

        public override string PasswordStrengthRegularExpression =>
            "*";

        public override bool RequiresQuestionAndAnswer =>
            false;

        public override bool RequiresUniqueEmail =>
            false;
    }
}

