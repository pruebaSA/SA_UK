namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface ISalonManager
    {
        void DeleteClosingDay(ClosingDayDB closingDay);
        void DeleteSalonPaymentMethod(SalonPaymentMethodDB paymentMethod);
        void DeleteSalonReview(SalonReviewDB review);
        ClosingDayDB GetClosingDayById(Guid closingDayId);
        List<ClosingDayDB> GetClosingDaysBySalonId(Guid salonId);
        OpeningHoursDB GetOpeningHoursById(Guid openingHoursId);
        OpeningHoursDB GetOpeningHoursBySalonId(Guid salonId);
        SalonDB GetSalonById(Guid salonId);
        SalonDB GetSalonBySEName(string sename);
        SalonPaymentMethodDB GetSalonPaymentMethodById(Guid paymentMethodId);
        List<SalonPaymentMethodDB> GetSalonPaymentMethodsBySalonId(Guid salonId);
        SalonRatingDB GetSalonRatingById(Guid value);
        SalonRatingDB GetSalonRatingBySalonId(Guid value);
        SalonReviewDB GetSalonReviewById(Guid value);
        List<SalonReviewDB> GetSalonReviewsBySalonId(Guid salonId, bool approvedOnly, string orderBy, int pageIndex, int pageSize, out int totalRecords);
        List<SalonDB> GetSalonsBySalonAttribute(string key, string value);
        List<SalonDB> GetSalonsThatStartWith(string value, string currencyCode);
        ClosingDayDB InsertClosingDay(ClosingDayDB closingDay);
        bool InsertClosingDaysXML(List<ClosingDayDB> closingDays);
        OpeningHoursDB InsertOpeningHours(OpeningHoursDB hours);
        SalonDB InsertSalon(SalonDB salon);
        SalonPaymentMethodDB InsertSalonPaymentMethod(SalonPaymentMethodDB paymentMethod);
        SalonRatingDB InsertSalonRating(SalonRatingDB rating);
        SalonReviewDB InsertSalonReview(SalonReviewDB review);
        ClosingDayDB UpdateClosingDay(ClosingDayDB closingDay);
        OpeningHoursDB UpdateOpeningHours(OpeningHoursDB hours);
        SalonDB UpdateSalon(SalonDB salon);
        SalonPaymentMethodDB UpdateSalonPaymentMethod(SalonPaymentMethodDB paymentMethod);
        SalonRatingDB UpdateSalonRating(SalonRatingDB rating);
        SalonReviewDB UpdateSalonReview(SalonReviewDB review);
    }
}

