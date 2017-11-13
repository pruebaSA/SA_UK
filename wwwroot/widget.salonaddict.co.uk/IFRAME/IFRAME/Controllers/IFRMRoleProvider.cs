namespace IFRAME.Controllers
{
    using SA.BAL;
    using System;
    using System.Web.Security;

    public class IFRMRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
            if (workingUser == null)
            {
                workingUser = IoC.Resolve<IUserManager>().GetSalonUserByUsername(username);
            }
            if (workingUser.IsAdmin)
            {
                return new string[] { "OWNER" };
            }
            if (!workingUser.IsGuest)
            {
                return new string[] { "SALON_OWNER" };
            }
            return new string[] { "API" };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string role)
        {
            SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
            switch (role)
            {
                case "API":
                    return workingUser.IsGuest;

                case "SALON_OWNER":
                    return (!workingUser.IsGuest && !workingUser.IsAdmin);

                case "OWNER":
                    return workingUser.IsAdmin;
            }
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

