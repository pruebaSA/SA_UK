namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface IBillingManager
    {
        void DeleteSalonInvoiceAD(SalonInvoiceADDB value);
        bool GenerateSalonInvoice(SalonInvoiceInsertXML invoice);
        int GetReportBillableSalonCount(string currencyCode);
        int GetReportBillableSalonNotificationCount(string currencyCode);
        List<BillableSalonDB> GetReportBillableSalons(string currencyCode);
        List<BillableSalonWTDB> GetReportBillableSalonWT(Guid salonId, string startDate, string endDate);
        List<InactivePaymentMethodDB> GetReportInactivePaymentMethods(string currencyCode);
        List<SalonDB> GetReportNonbillableSalons(string currencyCode, int pageIndex, int pageSize, out int totalRecords);
        List<SalonInvoiceDB> GetReportSalonInvoiceDue(string invoiceType, string currencyCode);
        int GetReportSalonInvoiceDueCount(string invoiceType, string currencyCode);
        List<SalonInvoiceDB> GetReportSalonInvoiceOverdue(string invoiceType, string currencyCode);
        int GetReportSalonInvoiceOverdueCount(string invoiceType, string currencyCode);
        int GetReportSalonInvoiceOverdueCountBySalonId(Guid value, string invoiceType);
        List<SalonInvoiceDB> GetReportSalonInvoicesVoid(string invoiceType, string currencyCode, int pageIndex, int pageSize, out int totalRecords);
        SalonInvoiceWTDB GetSalonInvoceWTById(Guid value);
        SalonInvoiceADDB GetSalonInvoiceADById(Guid value);
        List<SalonInvoiceADDB> GetSalonInvoiceADByInvoiceId(Guid value);
        List<BillingPeriodListItemDB> GetSalonInvoiceBillingPeriods(Guid salonId, int? numberOfItems);
        SalonInvoiceDB GetSalonInvoiceById(Guid value);
        SalonInvoiceDB GetSalonInvoiceByParentId(Guid value);
        int GetSalonInvoiceCount();
        SalonInvoiceDB GetSalonInvoiceCurrent(Guid value, string invoiceType, bool showHidden);
        List<SalonInvoiceDB> GetSalonInvoices(Guid? salonId, string invoiceType, string currencyCode, string invoiceNumber, DateTime startDate, DateTime endDate);
        List<SalonInvoiceDB> GetSalonInvoicesByPlanId(Guid planId, string invoiceType, string CurrencyCode);
        List<SalonInvoiceDB> GetSalonInvoicesIssued(string invoiceType, string currencyCode, int orderBy, int pageIndex, int pageSize, out int totalRecords);
        int GetSalonInvoicesIssuedCount(string invoiceType, string currencyCode);
        int GetSalonInvoiceUnpaidCount(Guid salonId, string invoiceType, string currencyCode);
        List<SalonInvoiceWTDB> GetSalonInvoiceWTByInvoiceId(Guid value, int pageIndex, int pageSize, out int totalRecords);
        WSPDB GetWSPById(Guid value);
        List<WSPDB> GetWSPBySalonId(Guid value);
        WSPDB GetWSPCurrent(Guid value);
        SalonInvoiceDB InsertSalonInvoice(SalonInvoiceDB value);
        SalonInvoiceADDB InsertSalonInvoiceAD(SalonInvoiceADDB value);
        bool InsertSalonInvoiceWTMultiple(List<SalonInvoiceWTDB> value);
        WSPDB InsertWSP(WSPDB value);
        SalonInvoiceDB UpdateSalonInvoice(SalonInvoiceDB value);
        SalonInvoiceADDB UpdateSalonInvoiceAD(SalonInvoiceADDB value);
        WSPDB UpdateWSP(WSPDB value);
        bool UpgradeCurrentWSP(WSPUpgradeXML input);
    }
}

