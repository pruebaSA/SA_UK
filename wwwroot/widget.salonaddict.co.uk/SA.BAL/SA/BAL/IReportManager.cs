namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface IReportManager
    {
        BillingTotalsDB GetBillingTotalsReport(Guid? salonId, string currencyCode);
        BookingTotalsDB GetBookingTotalsReport(Guid? salonId, string currencyCode);
        List<SalonDB> GetPlanReport(string planType, string currencyCode, int pageIndex, int pageSize, out int totalRecords);
        List<PlanTotalsDB> GetPlanTotalsReport(string currencyCode);
        int GetSalonCount(string currencyCode);
        int GetSalonUnreadAppointments(Guid salonId);
        List<WidgetOfflineSummaryDB> GetWidgetOfflineReport(string currencyCode, int pageIndex, int pageSize, out int totalRecords);
        List<WSPExpiringDB> GetWSPExpiryReport(string planType, string currencyCode, int numberOfDays, int pageIndex, int pageSize, out int totalRecords);
    }
}

