namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class SalonUserDB
    {
        public bool Active { get; set; }

        public string AdminComment { get; set; }

        public string ConfirmationToken { get; set; }

        public DateTime? ConfirmationTokenExpirationUtc { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string Currency { get; set; }

        public bool Deleted { get; set; }

        public string DisplayText { get; set; }

        public string Email { get; set; }

        public int FailedPasswordAttemptCount { get; set; }

        public DateTime? FailedPasswordWindowStartDateUtc { get; set; }

        public string FirstName { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsApproved { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsGuest { get; set; }

        public bool IsLockedOut { get; set; }

        public bool IsTaxExempt { get; set; }

        public string Language { get; set; }

        public DateTime? LastActivityDateUtc { get; set; }

        public string LastIPAddress { get; set; }

        public DateTime? LastLockoutDateUtc { get; set; }

        public DateTime? LastLoginDateUtc { get; set; }

        public string LastName { get; set; }

        public DateTime? LastPasswordChangeDateUtc { get; set; }

        public DateTime? LastPasswordFailureDateUtc { get; set; }

        public string Mobile { get; set; }

        public string MobilePIN { get; set; }

        public string Password { get; set; }

        public string PasswordFormat { get; set; }

        public string PasswordSalt { get; set; }

        public string PasswordVerificationToken { get; set; }

        public DateTime? PasswordVerificationTokenExpirationUtc { get; set; }

        public string PhoneNumber { get; set; }

        public Guid SalonId { get; set; }

        public string TimeZone { get; set; }

        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string VatNumber { get; set; }

        public string VatNumberStatus { get; set; }
    }
}

