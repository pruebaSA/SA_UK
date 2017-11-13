namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class BillingManagerSQL : IBillingManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public BillingManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        private BillableSalonDB BillableSalonMapping(SqlDataReader reader) => 
            new BillableSalonDB { 
                AddressLine1 = reader.GetString(reader.GetOrdinal("AddressLine1")),
                AddressLine2 = reader.GetString(reader.GetOrdinal("AddressLine2")),
                AddressLine3 = reader.GetString(reader.GetOrdinal("AddressLine3")),
                AddressLine4 = reader.GetString(reader.GetOrdinal("AddressLine4")),
                AddressLine5 = reader.GetString(reader.GetOrdinal("AddressLine5")),
                BillEndDate = reader.GetString(reader.GetOrdinal("BillEndDate")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId"))
            };

        private BillableSalonWTDB BillableSalonWTMapping(SqlDataReader reader) => 
            new BillableSalonWTDB { 
                AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
                AppointmentId = reader.GetGuid(reader.GetOrdinal("AppointmentId")),
                AppointmentTime = reader.GetTimeSpan(reader.GetOrdinal("AppointmentTime")),
                ItemPrice = reader.GetInt32(reader.GetOrdinal("ItemPrice")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                ServiceName = reader.GetString(reader.GetOrdinal("ServiceName")),
                TimeStamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
            };

        private BillingPeriodListItemDB BillingPeriodListItemMapping(SqlDataReader reader) => 
            new BillingPeriodListItemDB { 
                BillEndDate = reader.GetString(reader.GetOrdinal("BillEndDate")),
                InvoiceId = reader.GetGuid(reader.GetOrdinal("InvoiceId"))
            };

        public void DeleteSalonInvoiceAD(SalonInvoiceADDB value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.AdjustmentId == Guid.Empty)
            {
                throw new ArgumentException("Adjustment identifier cannot be empty.");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@AdjustmentId", value.AdjustmentId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceADDeleteById", commandParameters);
        }

        public bool GenerateSalonInvoice(SalonInvoiceInsertXML invoice)
        {
            if (invoice == null)
            {
                new ArgumentNullException("invoice cannot be null.");
            }
            if (invoice.BillEndDate == null)
            {
                throw new ArgumentException("bill end date cannot be null.");
            }
            if (invoice.BillEndDate == string.Empty)
            {
                throw new ArgumentException("bill end date cannot be empty.");
            }
            if (!Regex.IsMatch(invoice.BillEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(invoice.BillEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            if (invoice.BillStartDate == null)
            {
                throw new ArgumentException("bill start date cannot be null.");
            }
            if (invoice.BillStartDate == string.Empty)
            {
                throw new ArgumentException("bill start date cannot be empty.");
            }
            if (!Regex.IsMatch(invoice.BillStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(invoice.BillStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            if (invoice.InvoiceNumber == null)
            {
                throw new ArgumentException("Invoice number cannot be null.");
            }
            if (invoice.InvoiceNumber == string.Empty)
            {
                throw new ArgumentException("Invoice number cannot be empty.");
            }
            if (invoice.PlanFee < 0)
            {
                throw new ArgumentException("Plan fee cannot be less than zero.");
            }
            if (invoice.PlanStartDate == null)
            {
                throw new ArgumentException("plan start date cannot be null.");
            }
            if (invoice.PlanStartDate == string.Empty)
            {
                throw new ArgumentException("plan start date cannot be empty.");
            }
            if (!Regex.IsMatch(invoice.PlanStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(invoice.PlanStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            if (invoice.PlanEndDate == null)
            {
                throw new ArgumentException("plan end date cannot be null.");
            }
            if (invoice.PlanEndDate == string.Empty)
            {
                throw new ArgumentException("plan end date cannot be empty.");
            }
            if (!Regex.IsMatch(invoice.PlanEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(invoice.PlanEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            if (invoice.ParentId == Guid.Empty)
            {
                throw new ArgumentException("Parent identifier cannot be empty.");
            }
            if (invoice.SalonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            if (invoice.TaxRate == null)
            {
                throw new ArgumentException("Tax rate cannot be null.");
            }
            if (invoice.TaxRate == string.Empty)
            {
                throw new ArgumentException("Tax rate cannot be empty.");
            }
            if (invoice.TotalPlan < 0)
            {
                throw new ArgumentException("Total plan cannot be less than zero.");
            }
            if (invoice.TotalTax < 0)
            {
                throw new ArgumentException("Total tax cannot be less than zero.");
            }
            if (invoice.TotalWidget < 0)
            {
                throw new ArgumentException("Total widget cannot be less than zero.");
            }
            if (invoice.TotalWidgetCount < 0)
            {
                throw new ArgumentException("Total widget count cannot be less than zero.");
            }
            if (invoice.Transactions == null)
            {
                throw new ArgumentException("Transactions cannot be null.");
            }
            for (int i = 0; i < invoice.Transactions.Count; i++)
            {
                BillableSalonWTDB nwtdb = invoice.Transactions[i];
                DateTime appointmentDate = nwtdb.AppointmentDate;
                if (nwtdb.AppointmentId == Guid.Empty)
                {
                    throw new ArgumentException("appointment identifier cannot be empty.");
                }
                TimeSpan appointmentTime = nwtdb.AppointmentTime;
                nwtdb.FirstName = nwtdb.FirstName ?? string.Empty;
                nwtdb.LastName = nwtdb.LastName ?? string.Empty;
                nwtdb.ServiceName = nwtdb.ServiceName ?? string.Empty;
            }
            invoice.BillingAddressLine1 = invoice.BillingAddressLine1 ?? string.Empty;
            invoice.BillingAddressLine2 = invoice.BillingAddressLine2 ?? string.Empty;
            invoice.BillingAddressLine3 = invoice.BillingAddressLine3 ?? string.Empty;
            invoice.BillingAddressLine4 = invoice.BillingAddressLine4 ?? string.Empty;
            invoice.BillingAddressLine5 = invoice.BillingAddressLine5 ?? string.Empty;
            invoice.BillingCompany = invoice.BillingCompany ?? string.Empty;
            invoice.BillingEmail = invoice.BillingEmail ?? string.Empty;
            invoice.BillingFirstName = invoice.BillingFirstName ?? string.Empty;
            invoice.BillingLastName = invoice.BillingLastName ?? string.Empty;
            invoice.BillingMobile = invoice.BillingMobile ?? string.Empty;
            invoice.BillingPhoneNumber = invoice.BillingPhoneNumber ?? string.Empty;
            invoice.BillingPhoneNumberType = invoice.BillingPhoneNumberType ?? string.Empty;
            invoice.CreatedBy = invoice.CreatedBy ?? string.Empty;
            invoice.CurrencyCode = invoice.CurrencyCode ?? string.Empty;
            invoice.InvoiceNumber = invoice.InvoiceNumber ?? string.Empty;
            invoice.InvoiceType = invoice.InvoiceType ?? string.Empty;
            invoice.PaymentDueDate = invoice.PaymentDueDate ?? string.Empty;
            invoice.Status = invoice.Status ?? string.Empty;
            invoice.VATNumber = invoice.VATNumber ?? string.Empty;
            XDocument document = new XDocument(new object[] { new XElement("Invoice") });
            XElement content = new XElement("Bill", new object[] { 
                new XElement("ParentId", invoice.ParentId), new XElement("SalonId", invoice.SalonId), new XElement("InvoiceNumber", invoice.InvoiceNumber), new XElement("InvoiceType", invoice.InvoiceType), new XElement("PlanId", invoice.PlanId), new XElement("PlanDescription", invoice.PlanDescription), new XElement("PlanType", invoice.PlanType), new XElement("PlanFee", invoice.PlanFee), new XElement("PlanStartDate", invoice.PlanStartDate), new XElement("PlanEndDate", invoice.PlanEndDate), new XElement("BillingFirstName", invoice.BillingFirstName), new XElement("BillingLastName", invoice.BillingLastName), new XElement("BillingCompany", invoice.BillingCompany), new XElement("BillingPhoneNumberType", invoice.BillingPhoneNumberType), new XElement("BillingPhoneNumber", invoice.BillingPhoneNumber), new XElement("BillingMobile", invoice.BillingMobile),
                new XElement("BillingEmail", invoice.BillingEmail), new XElement("BillingAddressLine1", invoice.BillingAddressLine1), new XElement("BillingAddressLine2", invoice.BillingAddressLine2), new XElement("BillingAddressLine3", invoice.BillingAddressLine3), new XElement("BillingAddressLine4", invoice.BillingAddressLine4), new XElement("BillingAddressLine5", invoice.BillingAddressLine5), new XElement("BillStartDate", invoice.BillStartDate), new XElement("BillEndDate", invoice.BillEndDate), new XElement("PaymentDueDate", invoice.PaymentDueDate), new XElement("CurrencyCode", invoice.CurrencyCode), new XElement("VATNumber", invoice.VATNumber), new XElement("TaxRate", invoice.TaxRate), new XElement("SubtotalExclTax", invoice.SubtotalExclTax), new XElement("TotalPlan", invoice.TotalPlan), new XElement("TotalTax", invoice.TotalTax), new XElement("TotalAdjustment", invoice.TotalAdjustment),
                new XElement("TotalWidget", invoice.TotalWidget), new XElement("TotalWidgetCount", invoice.TotalWidgetCount), new XElement("TotalExclTax", invoice.TotalExclTax), new XElement("TotalInclTax", invoice.TotalInclTax), new XElement("TotalOverdue", invoice.TotalOverdue), new XElement("TotalAmountDue", invoice.TotalAmountDue), new XElement("Status", invoice.Status), new XElement("Published", invoice.Published), new XElement("CreatedOn", invoice.CreatedOn.ToString("yyyy-MM-ddTHH:mm:ss.fff")), new XElement("CreatedBy", invoice.CreatedBy)
            });
            XElement element2 = new XElement("Trans");
            foreach (BillableSalonWTDB nwtdb2 in invoice.Transactions)
            {
                object[] objArray3 = new object[8];
                objArray3[0] = new XElement("Timestamp", nwtdb2.TimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                objArray3[1] = new XElement("FirstName", nwtdb2.FirstName);
                objArray3[2] = new XElement("LastName", nwtdb2.LastName);
                objArray3[3] = new XElement("AppointmentId", nwtdb2.AppointmentId);
                objArray3[4] = new XElement("AppointmentDate", nwtdb2.AppointmentDate.ToString("yyyy-MM-dd"));
                DateTime time4 = new DateTime(nwtdb2.AppointmentTime.Ticks);
                objArray3[5] = new XElement("AppointmentTime", time4.ToString("HH:mm"));
                objArray3[6] = new XElement("ServiceName", nwtdb2.ServiceName);
                objArray3[7] = new XElement("ItemPrice", nwtdb2.ItemPrice);
                element2.Add(new XElement("Tran", objArray3));
            }
            document.Root.Add(content);
            document.Root.Add(element2);
            document.Root.Add(new object[] { new XElement("NextPlanStartDate", invoice.NextPlanStartDate), new XElement("NextPlanEndDate", invoice.NextPlanEndDate), new XElement("NextBillStartDate", invoice.NextBillStartDate), new XElement("NextBillEndDate", invoice.NextBillEndDate) });
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("Invoice_XML", new SqlXml(document.CreateReader())) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceGenerate", commandParameters) <= 0)
            {
                return false;
            }
            return true;
        }

        public int GetReportBillableSalonCount(string currencyCode)
        {
            int num = 0;
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@CurrencyCode", currencyCode) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportBillableSalonLoadCount", commandParameters))
            {
                if (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
            }
            return num;
        }

        public int GetReportBillableSalonNotificationCount(string currencyCode)
        {
            int num = 0;
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@CurrencyCode", currencyCode) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportBillingNotificationsLoadCount", commandParameters))
            {
                if (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
            }
            return num;
        }

        public List<BillableSalonDB> GetReportBillableSalons(string currencyCode)
        {
            List<BillableSalonDB> list = new List<BillableSalonDB>();
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@CurrencyCode", currencyCode) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportBillableSalonLoad", commandParameters))
            {
                while (reader.Read())
                {
                    BillableSalonDB item = this.BillableSalonMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public List<BillableSalonWTDB> GetReportBillableSalonWT(Guid salonId, string startDate, string endDate)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            if (startDate == null)
            {
                throw new ArgumentException("startvdate cannot be null.");
            }
            if (startDate == string.Empty)
            {
                throw new ArgumentException("start date cannot be empty.");
            }
            if (!Regex.IsMatch(startDate, @"(\d{4})-(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("start date is invalid.");
            }
            if (endDate == null)
            {
                throw new ArgumentException("end date cannot be null.");
            }
            if (endDate == string.Empty)
            {
                throw new ArgumentException("end date cannot be empty.");
            }
            if (!Regex.IsMatch(endDate, @"(\d{4})-(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("end date is invalid.");
            }
            List<BillableSalonWTDB> list = new List<BillableSalonWTDB>();
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@SalonId", salonId), new SqlParameter("@StartDate", startDate), new SqlParameter("@EndDate", endDate) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportBillableSalonWTLoad", commandParameters))
            {
                while (reader.Read())
                {
                    BillableSalonWTDB item = this.BillableSalonWTMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public List<InactivePaymentMethodDB> GetReportInactivePaymentMethods(string currencyCode)
        {
            List<InactivePaymentMethodDB> list = new List<InactivePaymentMethodDB>();
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@CurrencyCode", currencyCode) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportInactivePaymentMethodLoad", commandParameters))
            {
                while (reader.Read())
                {
                    InactivePaymentMethodDB item = this.InactivePaymentMethodMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public List<SalonDB> GetReportNonbillableSalons(string currencyCode, int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 1;
            }
            totalRecords = 0;
            List<SalonDB> list = new List<SalonDB>();
            SqlParameter parameter = new SqlParameter("@TotalRecords", SqlDbType.Int) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@CurrencyCode", currencyCode), new SqlParameter("@PageIndex", pageIndex), new SqlParameter("@PageSize", pageSize), parameter };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportNonBillableSalons", commandParameters))
            {
                while (reader.Read())
                {
                    SalonDB item = this.SalonMapping(reader);
                    list.Add(item);
                }
            }
            totalRecords = int.Parse(parameter.Value.ToString());
            return list;
        }

        public List<SalonInvoiceDB> GetReportSalonInvoiceDue(string invoiceType, string currencyCode)
        {
            List<SalonInvoiceDB> list = new List<SalonInvoiceDB>();
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@CurrencyCode", currencyCode) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportSalonInvoiceDueLoad", commandParameters))
            {
                while (reader.Read())
                {
                    SalonInvoiceDB item = this.SalonInvoiceMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public int GetReportSalonInvoiceDueCount(string invoiceType, string currencyCode)
        {
            int num = 0;
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@CurrencyCode", currencyCode) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportSalonInvoiceDueLoadCount", commandParameters))
            {
                if (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
            }
            return num;
        }

        public List<SalonInvoiceDB> GetReportSalonInvoiceOverdue(string invoiceType, string currencyCode)
        {
            List<SalonInvoiceDB> list = new List<SalonInvoiceDB>();
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@CurrencyCode", currencyCode) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportSalonInvoiceOverdueLoad", commandParameters))
            {
                while (reader.Read())
                {
                    SalonInvoiceDB item = this.SalonInvoiceMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public int GetReportSalonInvoiceOverdueCount(string invoiceType, string currencyCode)
        {
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@CurrencyCode", currencyCode) };
            int num = 0;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportSalonInvoiceOverdueLoadCount", commandParameters))
            {
                if (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
            }
            return num;
        }

        public int GetReportSalonInvoiceOverdueCountBySalonId(Guid value, string invoiceType)
        {
            int num = 0;
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@SalonId", value), new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportSalonInvoiceOverdueCountBySalonId", commandParameters))
            {
                if (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
            }
            return num;
        }

        public List<SalonInvoiceDB> GetReportSalonInvoicesVoid(string invoiceType, string currencyCode, int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 1;
            }
            totalRecords = 0;
            List<SalonInvoiceDB> list = new List<SalonInvoiceDB>();
            SqlParameter parameter = new SqlParameter("@TotalRecords", SqlDbType.Int) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@CurrencyCode", currencyCode), new SqlParameter("@PageIndex", pageIndex), new SqlParameter("@PageSize", pageSize), parameter };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportSalonInvoiceVoidLoad", commandParameters))
            {
                while (reader.Read())
                {
                    SalonInvoiceDB item = this.SalonInvoiceMapping(reader);
                    list.Add(item);
                }
            }
            totalRecords = int.Parse(parameter.Value.ToString());
            return list;
        }

        public SalonInvoiceWTDB GetSalonInvoceWTById(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("invoice item identifier cannot be empty.");
            }
            SalonInvoiceWTDB ewtdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@InvoiceItemId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonInvoiceWTLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    ewtdb = this.SalonInvoiceWTMapping(reader);
                }
            }
            return ewtdb;
        }

        public SalonInvoiceADDB GetSalonInvoiceADById(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("Adjustment identifier cannot be empty.");
            }
            SalonInvoiceADDB eaddb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@AdjustmentId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonInvoiceADLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    eaddb = this.SalonInvoiceADMapping(reader);
                }
            }
            return eaddb;
        }

        public List<SalonInvoiceADDB> GetSalonInvoiceADByInvoiceId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("Adjustment identifier cannot be empty.");
            }
            List<SalonInvoiceADDB> list = new List<SalonInvoiceADDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@InvoiceId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonInvoiceADLoadByInvoiceId", parameterValues))
            {
                while (reader.Read())
                {
                    SalonInvoiceADDB item = this.SalonInvoiceADMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public List<BillingPeriodListItemDB> GetSalonInvoiceBillingPeriods(Guid salonId, int? numberOfItems)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            List<BillingPeriodListItemDB> list = new List<BillingPeriodListItemDB>();
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@SalonId", salonId), new SqlParameter("@RowCount", numberOfItems) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceLoadBillingPeriods", commandParameters))
            {
                while (reader.Read())
                {
                    BillingPeriodListItemDB item = this.BillingPeriodListItemMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public SalonInvoiceDB GetSalonInvoiceById(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("invoice identifier cannot be empty.");
            }
            SalonInvoiceDB edb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@InvoiceId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonInvoiceLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    edb = this.SalonInvoiceMapping(reader);
                }
            }
            return edb;
        }

        public SalonInvoiceDB GetSalonInvoiceByParentId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("parent identifier cannot be empty.");
            }
            SalonInvoiceDB edb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@ParentId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonInvoiceLoadByParentId", parameterValues))
            {
                if (reader.Read())
                {
                    edb = this.SalonInvoiceMapping(reader);
                }
            }
            return edb;
        }

        public int GetSalonInvoiceCount()
        {
            int num = 0;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceLoadCount", new SqlParameter[0]))
            {
                if (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
            }
            return num;
        }

        public SalonInvoiceDB GetSalonInvoiceCurrent(Guid value, string invoiceType, bool showHidden)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            SalonInvoiceDB edb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", value), new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@Deleted", showHidden) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonInvoiceLoadCurrent", parameterValues))
            {
                if (reader.Read())
                {
                    edb = this.SalonInvoiceMapping(reader);
                }
            }
            return edb;
        }

        public List<SalonInvoiceDB> GetSalonInvoices(Guid? salonId, string invoiceType, string currencyCode, string invoiceNumber, DateTime startDate, DateTime endDate)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            List<SalonInvoiceDB> list = new List<SalonInvoiceDB>();
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@SalonId", salonId ?? SqlGuid.Null), new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@CurrencyCode", currencyCode), new SqlParameter("@InvoiceNumber", invoiceNumber ?? SqlString.Null), new SqlParameter("@StartDate", startDate), new SqlParameter("@EndDate", endDate) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceLoad", commandParameters))
            {
                while (reader.Read())
                {
                    SalonInvoiceDB item = this.SalonInvoiceMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public List<SalonInvoiceDB> GetSalonInvoicesByPlanId(Guid planId, string invoiceType, string currencyCode)
        {
            if (planId == Guid.Empty)
            {
                throw new ArgumentException("plan identifier cannot be empty.");
            }
            List<SalonInvoiceDB> list = new List<SalonInvoiceDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@PlanId", planId), new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@CurrencyCode", currencyCode) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonInvoiceLoadByPlanId", parameterValues))
            {
                while (reader.Read())
                {
                    SalonInvoiceDB item = this.SalonInvoiceMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public List<SalonInvoiceDB> GetSalonInvoicesIssued(string invoiceType, string currencyCode, int orderBy, int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 1;
            }
            totalRecords = 0;
            List<SalonInvoiceDB> list = new List<SalonInvoiceDB>();
            SqlParameter parameter = new SqlParameter("@TotalRecords", SqlDbType.Int) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@CurrencyCode", currencyCode), new SqlParameter("@OrderBy", orderBy.ToString()), new SqlParameter("@PageIndex", pageIndex), new SqlParameter("@PageSize", pageSize), parameter };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceIssuedLoad", commandParameters))
            {
                while (reader.Read())
                {
                    SalonInvoiceDB item = this.SalonInvoiceMapping(reader);
                    list.Add(item);
                }
            }
            totalRecords = int.Parse(parameter.Value.ToString());
            return list;
        }

        public int GetSalonInvoicesIssuedCount(string invoiceType, string currencyCode)
        {
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@CurrencyCode", currencyCode) };
            int num = 0;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceIssuedLoadCount", commandParameters))
            {
                if (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
            }
            return num;
        }

        public int GetSalonInvoiceUnpaidCount(Guid salonId, string invoiceType, string currencyCode)
        {
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@SalonId", salonId), new SqlParameter("@InvoiceType", invoiceType ?? SqlString.Null), new SqlParameter("@CurrencyCode", currencyCode) };
            int num = 0;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceUnpaidLoadCount", commandParameters))
            {
                if (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
            }
            return num;
        }

        public List<SalonInvoiceWTDB> GetSalonInvoiceWTByInvoiceId(Guid value, int pageIndex, int pageSize, out int totalRecords)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("invoice identifier cannot be empty.");
            }
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 1;
            }
            totalRecords = 0;
            List<SalonInvoiceWTDB> list = new List<SalonInvoiceWTDB>();
            SqlParameter parameter = new SqlParameter("@TotalRecords", SqlDbType.Int) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@InvoiceId", value), new SqlParameter("@PageIndex", pageIndex), new SqlParameter("@PageSize", pageSize), parameter };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceWTLoadByInvoiceId", commandParameters))
            {
                while (reader.Read())
                {
                    SalonInvoiceWTDB item = this.SalonInvoiceWTMapping(reader);
                    list.Add(item);
                }
            }
            totalRecords = int.Parse(parameter.Value.ToString());
            return list;
        }

        public WSPDB GetWSPById(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("widget subscription plan identifier cannot be empty.");
            }
            WSPDB wspdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@PlanId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_WSPLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    wspdb = this.WSPMapping(reader);
                }
            }
            return wspdb;
        }

        public List<WSPDB> GetWSPBySalonId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            List<WSPDB> list = new List<WSPDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_WSPLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    WSPDB item = this.WSPMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public WSPDB GetWSPCurrent(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            string key = $"WSP-SALON-{"CURRENT-" + value}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (WSPDB) obj2;
            }
            WSPDB wspdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_WSPCurrentLoad", parameterValues))
            {
                if (reader.Read())
                {
                    wspdb = this.WSPMapping(reader);
                }
            }
            this._cacheManager.Add(key, wspdb);
            return wspdb;
        }

        private InactivePaymentMethodDB InactivePaymentMethodMapping(SqlDataReader reader) => 
            new InactivePaymentMethodDB { 
                Alias = reader.GetString(reader.GetOrdinal("Alias")),
                CardExpirationMonth = reader.GetString(reader.GetOrdinal("CardExpirationMonth")),
                CardExpirationYear = reader.GetString(reader.GetOrdinal("CardExpirationYear")),
                MaskedCardNumber = reader.GetString(reader.GetOrdinal("MaskedCardNumber")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                SalonName = reader.GetString(reader.GetOrdinal("SalonName")),
                SalonPaymentMethodId = reader.GetGuid(reader.GetOrdinal("SalonPaymentMethodId"))
            };

        public SalonInvoiceDB InsertSalonInvoice(SalonInvoiceDB value)
        {
            if (value == null)
            {
                new ArgumentNullException("invoice cannot be null.");
            }
            if (value.BillEndDate == null)
            {
                throw new ArgumentException("bill end date cannot be null.");
            }
            if (value.BillStartDate == string.Empty)
            {
                throw new ArgumentException("bill start date cannot be empty.");
            }
            if (!Regex.IsMatch(value.BillEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.BillEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            if (value.BillStartDate == null)
            {
                throw new ArgumentException("bill start date cannot be null.");
            }
            if (value.BillStartDate == string.Empty)
            {
                throw new ArgumentException("bill start date cannot be empty.");
            }
            if (!Regex.IsMatch(value.BillStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.BillStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            if (value.InvoiceNumber == null)
            {
                throw new ArgumentException("invoice number cannot be null.");
            }
            if (value.InvoiceNumber == string.Empty)
            {
                throw new ArgumentException("invoice number cannot be empty.");
            }
            if (value.PlanId == Guid.Empty)
            {
                throw new ArgumentException("plan identifier cannot be empty.");
            }
            if (value.PlanFee < 0)
            {
                throw new ArgumentException("plan fee cannot be less than zero.");
            }
            if (value.PlanStartDate == null)
            {
                throw new ArgumentException("plan start date cannot be null.");
            }
            if (value.PlanStartDate == string.Empty)
            {
                throw new ArgumentException("plan start date cannot be empty.");
            }
            if (!Regex.IsMatch(value.PlanStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.PlanStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            if (value.PlanEndDate == null)
            {
                throw new ArgumentException("plan end date cannot be null.");
            }
            if (value.PlanEndDate == string.Empty)
            {
                throw new ArgumentException("plan end date cannot be empty.");
            }
            if (!Regex.IsMatch(value.PlanEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.PlanEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            if (value.ParentId == Guid.Empty)
            {
                throw new ArgumentException("parent identifier cannot be empty.");
            }
            if (value.SalonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            if (value.SubtotalExclTax < 0)
            {
                throw new ArgumentException("subtotal excluding tax cannot be less than zero.");
            }
            if (value.TotalPaid < 0)
            {
                throw new ArgumentException("total paid cannot be less than zero.");
            }
            if (value.TotalPlan < 0)
            {
                throw new ArgumentException("total plan cannot be less than zero.");
            }
            if (value.TotalTax < 0)
            {
                throw new ArgumentException("total tax due cannot be less than zero.");
            }
            if (value.TotalWidget < 0)
            {
                throw new ArgumentException("total widget cannot be less than zero.");
            }
            if (value.TotalWidgetCount < 0)
            {
                throw new ArgumentException("total widget count cannot be less than zero.");
            }
            value.AdminComment = value.AdminComment ?? string.Empty;
            value.AuthorizationTransactionCode = value.AuthorizationTransactionCode ?? string.Empty;
            value.AuthorizationTransactionId = value.AuthorizationTransactionId ?? string.Empty;
            value.AuthorizationTransactionResult = value.AuthorizationTransactionResult ?? string.Empty;
            value.BillingAddressLine1 = value.BillingAddressLine1 ?? string.Empty;
            value.BillingAddressLine2 = value.BillingAddressLine2 ?? string.Empty;
            value.BillingAddressLine3 = value.BillingAddressLine3 ?? string.Empty;
            value.BillingAddressLine4 = value.BillingAddressLine4 ?? string.Empty;
            value.BillingAddressLine5 = value.BillingAddressLine5 ?? string.Empty;
            value.BillingCompany = value.BillingCompany ?? string.Empty;
            value.BillingEmail = value.BillingEmail ?? string.Empty;
            value.BillEndDate = value.BillEndDate ?? string.Empty;
            value.BillingFirstName = value.BillingFirstName ?? string.Empty;
            value.BillingLastName = value.BillingLastName ?? string.Empty;
            value.BillingMobile = value.BillingMobile ?? string.Empty;
            value.BillingPhoneNumber = value.BillingPhoneNumber ?? string.Empty;
            value.BillingPhoneType = value.BillingPhoneType ?? string.Empty;
            value.BillStartDate = value.BillStartDate ?? string.Empty;
            value.CaptureTransactionId = value.CaptureTransactionId ?? string.Empty;
            value.CaptureTransactionResult = value.CaptureTransactionResult ?? string.Empty;
            value.CardAlias = value.CardAlias ?? string.Empty;
            value.CardName = value.CardName ?? string.Empty;
            value.CardNumber = value.CardNumber ?? string.Empty;
            value.CardType = value.CardType ?? string.Empty;
            value.Comment = value.Comment ?? string.Empty;
            value.CreatedBy = value.CreatedBy ?? string.Empty;
            value.CurrencyCode = value.CurrencyCode ?? string.Empty;
            value.InvoiceNumber = value.InvoiceNumber ?? string.Empty;
            value.InvoiceType = value.InvoiceType ?? string.Empty;
            value.MaskedCardNumber = value.MaskedCardNumber ?? string.Empty;
            value.PlanDescription = value.PlanDescription ?? string.Empty;
            value.PaymentDueDate = value.PaymentDueDate ?? string.Empty;
            value.Status = value.Status ?? string.Empty;
            value.UpdatedBy = value.UpdatedBy ?? string.Empty;
            value.VATNumber = value.VATNumber ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@InvoiceId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[60];
            parameterArray2[0] = parameter;
            parameterArray2[1] = new SqlParameter("@AdminComment", value.AdminComment);
            parameterArray2[2] = new SqlParameter("@AuthorizationTransactionCode", value.AuthorizationTransactionCode);
            parameterArray2[3] = new SqlParameter("@AuthorizationTransactionId", value.AuthorizationTransactionId);
            parameterArray2[4] = new SqlParameter("@AuthorizationTransactionResult", value.AuthorizationTransactionResult);
            parameterArray2[5] = new SqlParameter("@BillingAddressLine1", value.BillingAddressLine1);
            parameterArray2[6] = new SqlParameter("@BillingAddressLine2", value.BillingAddressLine2);
            parameterArray2[7] = new SqlParameter("@BillingAddressLine3", value.BillingAddressLine3);
            parameterArray2[8] = new SqlParameter("@BillingAddressLine4", value.BillingAddressLine4);
            parameterArray2[9] = new SqlParameter("@BillingAddressLine5", value.BillingAddressLine5);
            parameterArray2[10] = new SqlParameter("@BillingCompany", value.BillingCompany);
            parameterArray2[11] = new SqlParameter("@BillingEmail", value.BillingEmail);
            parameterArray2[12] = new SqlParameter("@BillEndDate", value.BillEndDate);
            parameterArray2[13] = new SqlParameter("@BillingFirstName", value.BillingFirstName);
            parameterArray2[14] = new SqlParameter("@BillingLastName", value.BillingLastName);
            parameterArray2[15] = new SqlParameter("@BillingMobile", value.BillingMobile);
            parameterArray2[0x10] = new SqlParameter("@BillingPhoneNumber", value.BillingPhoneNumber);
            parameterArray2[0x11] = new SqlParameter("@BillingPhoneNumberType", value.BillingPhoneType);
            parameterArray2[0x12] = new SqlParameter("@BillStartDate", value.BillStartDate);
            parameterArray2[0x13] = new SqlParameter("@CaptureTransactionId", value.CaptureTransactionId);
            parameterArray2[20] = new SqlParameter("@CaptureTransactionResult", value.CaptureTransactionResult);
            parameterArray2[0x15] = new SqlParameter("@CardAlias", value.CardAlias);
            parameterArray2[0x16] = new SqlParameter("@CardName", value.CardName);
            parameterArray2[0x17] = new SqlParameter("@CardNumber", value.CardNumber);
            parameterArray2[0x18] = new SqlParameter("@CardType", value.CardType);
            parameterArray2[0x19] = new SqlParameter("@Comment", value.Comment);
            parameterArray2[0x1a] = new SqlParameter("@CreatedBy", value.CreatedBy);
            parameterArray2[0x1b] = new SqlParameter("@CreatedOn", value.CreatedOn);
            parameterArray2[0x1c] = new SqlParameter("@CurrencyCode", value.CurrencyCode);
            parameterArray2[0x1d] = new SqlParameter("@Deleted", value.Deleted);
            parameterArray2[30] = new SqlParameter("@InvoiceNumber", value.InvoiceNumber);
            parameterArray2[0x1f] = new SqlParameter("@InvoiceType", value.InvoiceType);
            parameterArray2[0x20] = new SqlParameter("@MaskedCardNumber", value.MaskedCardNumber);
            parameterArray2[0x21] = new SqlParameter("@PaidOn", value.PaidOn);
            parameterArray2[0x22] = new SqlParameter("@PaymentDueDate", value.PaymentDueDate);
            parameterArray2[0x23] = new SqlParameter("@PlanId", value.PlanId);
            parameterArray2[0x24] = new SqlParameter("@PlanFee", value.PlanFee);
            parameterArray2[0x25] = new SqlParameter("@PlanType", value.PlanType);
            parameterArray2[0x26] = new SqlParameter("@PlanDescription", value.PlanDescription);
            parameterArray2[0x27] = new SqlParameter("@PlanStartDate", value.PlanStartDate);
            parameterArray2[40] = new SqlParameter("@PlanEndDate", value.PlanEndDate);
            Guid? parentId = value.ParentId;
            parameterArray2[0x29] = new SqlParameter("@ParentId", parentId.HasValue ? ((SqlGuid) parentId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[0x2a] = new SqlParameter("@Published", value.Published);
            parameterArray2[0x2b] = new SqlParameter("@SalonId", value.SalonId);
            parameterArray2[0x2c] = new SqlParameter("@Status", value.Status);
            parameterArray2[0x2d] = new SqlParameter("@SubtotalExclTax", value.SubtotalExclTax);
            parameterArray2[0x2e] = new SqlParameter("@TaxRate", value.TaxRate);
            parameterArray2[0x2f] = new SqlParameter("@TotalAmountDue", value.TotalAmountDue);
            parameterArray2[0x30] = new SqlParameter("@TotalAdjustment", value.TotalAdjustment);
            parameterArray2[0x31] = new SqlParameter("@TotalExclTax", value.TotalExclTax);
            parameterArray2[50] = new SqlParameter("@TotalInclTax", value.TotalInclTax);
            parameterArray2[0x33] = new SqlParameter("@TotalOverdue", value.TotalOverdue);
            parameterArray2[0x34] = new SqlParameter("@TotalPaid", value.TotalPaid);
            parameterArray2[0x35] = new SqlParameter("@TotalPlan", value.TotalPlan);
            parameterArray2[0x36] = new SqlParameter("@TotalTax", value.TotalTax);
            parameterArray2[0x37] = new SqlParameter("@TotalWidget", value.TotalWidget);
            parameterArray2[0x38] = new SqlParameter("@TotalWidgetCount", value.TotalWidgetCount);
            parameterArray2[0x39] = new SqlParameter("@UpdatedBy", value.UpdatedBy);
            parameterArray2[0x3a] = new SqlParameter("@UpdatedOn", value.UpdatedOn);
            parameterArray2[0x3b] = new SqlParameter("@VATNumber", value.VATNumber);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid guid = new Guid(parameter.Value.ToString());
            value = this.GetSalonInvoiceById(guid);
            return value;
        }

        public SalonInvoiceADDB InsertSalonInvoiceAD(SalonInvoiceADDB value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.InvoiceId == Guid.Empty)
            {
                throw new ArgumentException("Invoice identifier cannot be empty.");
            }
            value.Description = value.Description ?? string.Empty;
            value.CreatedBy = value.CreatedBy ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@AdjustmentId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { parameter, new SqlParameter("@InvoiceId", value.InvoiceId), new SqlParameter("@Amount", value.Amount), new SqlParameter("@Description", value.Description), new SqlParameter("@CreatedOn", value.CreatedOn), new SqlParameter("@CreatedBy", value.CreatedBy) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceADInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid guid = new Guid(parameter.Value.ToString());
            value = this.GetSalonInvoiceADById(guid);
            return value;
        }

        public bool InsertSalonInvoiceWTMultiple(List<SalonInvoiceWTDB> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("invoice items cannot be null.");
            }
            if (value.Count >= 1)
            {
                for (int i = 0; i < value.Count; i++)
                {
                    SalonInvoiceWTDB ewtdb = value[i];
                    if (ewtdb.AppointmentDate == null)
                    {
                        throw new ArgumentException("appointment date cannot be null.");
                    }
                    if (ewtdb.AppointmentDate == string.Empty)
                    {
                        throw new ArgumentException("appointment date cannot be empty.");
                    }
                    if (!Regex.IsMatch(ewtdb.AppointmentDate, @"(\d{2})-(\d{2})"))
                    {
                        throw new ArgumentException("appointment date is invalid.");
                    }
                    try
                    {
                        DateTime.ParseExact(ewtdb.AppointmentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new ArgumentException("appointment date is invalid.");
                    }
                    if (ewtdb.AppointmentId == Guid.Empty)
                    {
                        throw new ArgumentException("appointment identifier cannot be empty.");
                    }
                    if (ewtdb.AppointmentTime == null)
                    {
                        throw new ArgumentException("time cannot be invalid.");
                    }
                    if (ewtdb.AppointmentTime == string.Empty)
                    {
                        throw new ArgumentException("time cannot be empty.");
                    }
                    if (!Regex.IsMatch(ewtdb.AppointmentTime, @"(\d{2}):(\d{2})"))
                    {
                        throw new ArgumentException("time is invalid.");
                    }
                    try
                    {
                        DateTime.ParseExact(ewtdb.AppointmentTime, "HH:mm", CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new ArgumentException("time is invalid.");
                    }
                    if (ewtdb.ItemPrice < 0)
                    {
                        throw new ArgumentException("item price cannot be less than zero.");
                    }
                    if (ewtdb.InvoiceId == Guid.Empty)
                    {
                        throw new ArgumentException("invoice identifier cannot be empty.");
                    }
                    ewtdb.FirstName = ewtdb.FirstName ?? string.Empty;
                    ewtdb.LastName = ewtdb.LastName ?? string.Empty;
                    ewtdb.ServiceName = ewtdb.ServiceName ?? string.Empty;
                }
                XDocument document = new XDocument(new object[] { new XElement("Items") });
                foreach (SalonInvoiceWTDB ewtdb2 in value)
                {
                    document.Root.Add(new XElement("Item", new object[] { new XElement("InvoiceId", ewtdb2.InvoiceId), new XElement("Timestamp", ewtdb2.TimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff")), new XElement("FirstName", ewtdb2.FirstName), new XElement("LastName", ewtdb2.LastName), new XElement("AppointmentId", ewtdb2.AppointmentId), new XElement("AppointmentDate", ewtdb2.AppointmentDate), new XElement("AppointmentTime", ewtdb2.AppointmentTime), new XElement("ServiceName", ewtdb2.ServiceName), new XElement("ItemPrice", ewtdb2.ItemPrice) }));
                }
                SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("Items_XML", new SqlXml(document.CreateReader())) };
                if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceWTInsertMultiple", commandParameters) <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        public WSPDB InsertWSP(WSPDB value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.BillEndDate == null)
            {
                throw new ArgumentException("bill end date cannot be null.");
            }
            if (value.BillEndDate == string.Empty)
            {
                throw new ArgumentException("bill end date cannot be empty.");
            }
            if (!Regex.IsMatch(value.BillEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.BillEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            if (value.BillStartDate == null)
            {
                throw new ArgumentException("bill start date cannot be null.");
            }
            if (value.BillStartDate == string.Empty)
            {
                throw new ArgumentException("bill start date cannot be empty.");
            }
            if (!Regex.IsMatch(value.BillStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.BillStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            if (!string.IsNullOrEmpty(value.CancelDate))
            {
                if (!Regex.IsMatch(value.CancelDate, @"(\d{2})-(\d{2})"))
                {
                    throw new ArgumentException("cancellation date is invalid.");
                }
                try
                {
                    DateTime.ParseExact(value.CancelDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("cancellation date is invalid.");
                }
            }
            if (value.Description == null)
            {
                throw new ArgumentException("description cannot be null.");
            }
            if (value.Description == string.Empty)
            {
                throw new ArgumentException("description cannot be empty.");
            }
            if (value.ExcessFeeWT < 0)
            {
                throw new ArgumentException("excess fee cannot be less than zero.");
            }
            if (value.ExcessLimitWT < 0)
            {
                throw new ArgumentException("excess limit cannot be less than zero.");
            }
            if (value.ParentId == Guid.Empty)
            {
                throw new ArgumentException("parent identifier cannot be empty.");
            }
            if (value.PlanPrice < 0)
            {
                throw new ArgumentException("plan price cannot be less than zero.");
            }
            if (value.PlanType == null)
            {
                throw new ArgumentException("plan type cannot be null.");
            }
            if (value.PlanType == string.Empty)
            {
                throw new ArgumentException("plan type cannot be empty.");
            }
            if (value.PlanEndDate == null)
            {
                throw new ArgumentException("plan end date cannot be null.");
            }
            if (value.PlanEndDate == string.Empty)
            {
                throw new ArgumentException("plan end date cannot be empty.");
            }
            if (!Regex.IsMatch(value.PlanEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.PlanEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            if (value.PlanStartDate == null)
            {
                throw new ArgumentException("plan start date cannot be null.");
            }
            if (value.PlanStartDate == string.Empty)
            {
                throw new ArgumentException("plan start date cannot be empty.");
            }
            if (!Regex.IsMatch(value.PlanStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.PlanStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            if (value.SalonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            value.CancelDate = value.CancelDate ?? string.Empty;
            value.UpdatedBy = value.UpdatedBy ?? string.Empty;
            value.CreatedBy = value.CreatedBy ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@PlanId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { 
                parameter, new SqlParameter("@ParentId", value.ParentId ?? SqlGuid.Null), new SqlParameter("@SalonId", value.SalonId), new SqlParameter("@Description", value.Description), new SqlParameter("@PlanType", value.PlanType), new SqlParameter("@PlanPrice", value.PlanPrice), new SqlParameter("@PlanStartDate", value.PlanStartDate), new SqlParameter("@PlanEndDate", value.PlanEndDate), new SqlParameter("@BillStartDate", value.BillStartDate), new SqlParameter("@BillEndDate", value.BillEndDate), new SqlParameter("@ExcessFeeWT", value.ExcessFeeWT), new SqlParameter("@ExcessLimitWT", value.ExcessLimitWT), new SqlParameter("@Prorate", value.Prorate), new SqlParameter("@CancelDate", value.CancelDate), new SqlParameter("@Active", value.Active), new SqlParameter("@CreatedOn", value.CreatedOn),
                new SqlParameter("@CreatedBy", value.CreatedBy), new SqlParameter("@UpdatedOn", value.UpdatedOn), new SqlParameter("@UpdatedBy", value.UpdatedBy)
            };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_WSPInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid guid = new Guid(parameter.Value.ToString());
            string key = $"WSP-SALON-{"CURRENT-" + value}";
            this._cacheManager.Remove(key);
            value = this.GetWSPById(guid);
            return value;
        }

        private SalonInvoiceADDB SalonInvoiceADMapping(SqlDataReader reader) => 
            new SalonInvoiceADDB { 
                Amount = reader.GetInt32(reader.GetOrdinal("Amount")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                AdjustmentId = reader.GetGuid(reader.GetOrdinal("AdjustmentId")),
                InvoiceId = reader.GetGuid(reader.GetOrdinal("InvoiceId"))
            };

        private SalonInvoiceDB SalonInvoiceMapping(SqlDataReader reader) => 
            new SalonInvoiceDB { 
                AdminComment = reader.GetString(reader.GetOrdinal("AdminComment")),
                AuthorizationTransactionCode = reader.GetString(reader.GetOrdinal("AuthorizationTransactionCode")),
                AuthorizationTransactionId = reader.GetString(reader.GetOrdinal("AuthorizationTransactionId")),
                AuthorizationTransactionResult = reader.GetString(reader.GetOrdinal("AuthorizationTransactionResult")),
                BillEndDate = reader.GetString(reader.GetOrdinal("BillEndDate")),
                BillingAddressLine1 = reader.GetString(reader.GetOrdinal("BillingAddressLine1")),
                BillingAddressLine2 = reader.GetString(reader.GetOrdinal("BillingAddressLine2")),
                BillingAddressLine3 = reader.GetString(reader.GetOrdinal("BillingAddressLine3")),
                BillingAddressLine4 = reader.GetString(reader.GetOrdinal("BillingAddressLine4")),
                BillingAddressLine5 = reader.GetString(reader.GetOrdinal("BillingAddressLine5")),
                BillingCompany = reader.GetString(reader.GetOrdinal("BillingCompany")),
                BillingEmail = reader.GetString(reader.GetOrdinal("BillingEmail")),
                BillingFirstName = reader.GetString(reader.GetOrdinal("BillingFirstName")),
                BillingLastName = reader.GetString(reader.GetOrdinal("BillingLastName")),
                BillingMobile = reader.GetString(reader.GetOrdinal("BillingMobile")),
                BillingPhoneNumber = reader.GetString(reader.GetOrdinal("BillingPhoneNumber")),
                BillingPhoneType = reader.GetString(reader.GetOrdinal("BillingPhoneNumberType")),
                BillStartDate = reader.GetString(reader.GetOrdinal("BillStartDate")),
                CaptureTransactionId = reader.GetString(reader.GetOrdinal("CaptureTransactionId")),
                CaptureTransactionResult = reader.GetString(reader.GetOrdinal("CaptureTransactionResult")),
                CardAlias = reader.GetString(reader.GetOrdinal("CardAlias")),
                CardName = reader.GetString(reader.GetOrdinal("CardName")),
                CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                CardType = reader.GetString(reader.GetOrdinal("CardType")),
                Comment = reader.GetString(reader.GetOrdinal("Comment")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                CurrencyCode = reader.GetString(reader.GetOrdinal("CurrencyCode")),
                Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted")),
                InvoiceId = reader.GetGuid(reader.GetOrdinal("InvoiceId")),
                InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                InvoiceType = reader.GetString(reader.GetOrdinal("InvoiceType")),
                MaskedCardNumber = reader.GetString(reader.GetOrdinal("MaskedCardNumber")),
                PaidOn = reader.GetValue(reader.GetOrdinal("PaidOn")) as DateTime?,
                ParentId = reader.GetValue(reader.GetOrdinal("ParentId")) as Guid?,
                PaymentDueDate = reader.GetString(reader.GetOrdinal("PaymentDueDate")),
                PlanDescription = reader.GetString(reader.GetOrdinal("PlanDescription")),
                PlanEndDate = reader.GetString(reader.GetOrdinal("PlanEndDate")),
                PlanFee = reader.GetInt32(reader.GetOrdinal("PlanFee")),
                PlanId = reader.GetGuid(reader.GetOrdinal("PlanId")),
                PlanStartDate = reader.GetString(reader.GetOrdinal("PlanStartDate")),
                PlanType = reader.GetString(reader.GetOrdinal("PlanType")),
                Published = reader.GetBoolean(reader.GetOrdinal("Published")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                SubtotalExclTax = reader.GetInt32(reader.GetOrdinal("SubtotalExclTax")),
                TaxRate = reader.GetString(reader.GetOrdinal("TaxRate")),
                TotalAmountDue = reader.GetInt32(reader.GetOrdinal("TotalAmountDue")),
                TotalAdjustment = reader.GetInt32(reader.GetOrdinal("TotalAdjustment")),
                TotalExclTax = reader.GetInt32(reader.GetOrdinal("TotalExclTax")),
                TotalInclTax = reader.GetInt32(reader.GetOrdinal("TotalInclTax")),
                TotalOverdue = reader.GetInt32(reader.GetOrdinal("TotalOverdue")),
                TotalPaid = reader.GetInt32(reader.GetOrdinal("TotalPaid")),
                TotalPlan = reader.GetInt32(reader.GetOrdinal("TotalPlan")),
                TotalTax = reader.GetInt32(reader.GetOrdinal("TotalTax")),
                TotalWidget = reader.GetInt32(reader.GetOrdinal("TotalWidget")),
                TotalWidgetCount = reader.GetInt32(reader.GetOrdinal("TotalWidgetCount")),
                UpdatedBy = reader.GetString(reader.GetOrdinal("UpdatedBy")),
                UpdatedOn = reader.GetDateTime(reader.GetOrdinal("UpdatedOn")),
                VATNumber = reader.GetString(reader.GetOrdinal("VATNumber"))
            };

        private SalonInvoiceWTDB SalonInvoiceWTMapping(SqlDataReader reader) => 
            new SalonInvoiceWTDB { 
                AppointmentDate = reader.GetString(reader.GetOrdinal("AppointmentDate")),
                AppointmentId = reader.GetGuid(reader.GetOrdinal("AppointmentId")),
                AppointmentTime = reader.GetString(reader.GetOrdinal("AppointmentTime")),
                ItemPrice = reader.GetInt32(reader.GetOrdinal("ItemPrice")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                InvoiceId = reader.GetGuid(reader.GetOrdinal("InvoiceId")),
                InvoiceItemId = reader.GetGuid(reader.GetOrdinal("InvoiceItemId")),
                ServiceName = reader.GetString(reader.GetOrdinal("ServiceName")),
                TimeStamp = reader.GetDateTime(reader.GetOrdinal("TimeStamp"))
            };

        public SalonDB SalonMapping(SqlDataReader reader) => 
            new SalonDB { 
                Abbreviation = reader.GetString(reader.GetOrdinal("Abbreviation")),
                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                AddressLine1 = reader.GetString(reader.GetOrdinal("AddressLine1")),
                AddressLine2 = reader.GetString(reader.GetOrdinal("AddressLine2")),
                AddressLine3 = reader.GetString(reader.GetOrdinal("AddressLine3")),
                AddressLine4 = reader.GetString(reader.GetOrdinal("AddressLine4")),
                AddressLine5 = reader.GetString(reader.GetOrdinal("AddressLine5")),
                AdminComment = reader.GetString(reader.GetOrdinal("AdminComment")),
                BookOnWeb = reader.GetBoolean(reader.GetOrdinal("BookOnWeb")),
                BookOnWidget = reader.GetBoolean(reader.GetOrdinal("BookOnWidget")),
                BookOnMobile = reader.GetBoolean(reader.GetOrdinal("BookOnMobile")),
                CityTown = reader.GetString(reader.GetOrdinal("CityTown")),
                Company = reader.GetString(reader.GetOrdinal("Company")),
                Country = reader.GetString(reader.GetOrdinal("Country")),
                County = reader.GetString(reader.GetOrdinal("County")),
                CreatedOnUtc = reader.GetDateTime(reader.GetOrdinal("CreatedOnUtc")),
                Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted")),
                Directions = reader.GetString(reader.GetOrdinal("Directions")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                FaxNumber = reader.GetString(reader.GetOrdinal("FaxNumber")),
                FullDescription = reader.GetString(reader.GetOrdinal("FullDescription")),
                Latitude = reader.GetString(reader.GetOrdinal("Latitude")),
                Longitude = reader.GetString(reader.GetOrdinal("Longitude")),
                MetaDescription = reader.GetString(reader.GetOrdinal("MetaDescription")),
                MetaKeywords = reader.GetString(reader.GetOrdinal("MetaKeywords")),
                MetaTitle = reader.GetString(reader.GetOrdinal("MetaTitle")),
                Mobile = reader.GetString(reader.GetOrdinal("Mobile")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                PictureId = reader.GetValue(reader.GetOrdinal("PictureId")) as Guid?,
                Published = reader.GetBoolean(reader.GetOrdinal("Published")),
                RealexPayerRef = reader.GetString(reader.GetOrdinal("RealexPayerRef")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                SEName = reader.GetString(reader.GetOrdinal("SEName")),
                ShortDescription = reader.GetString(reader.GetOrdinal("ShortDescription")),
                ShowOnWeb = reader.GetBoolean(reader.GetOrdinal("ShowOnWeb")),
                ShowOnWidget = reader.GetBoolean(reader.GetOrdinal("ShowOnWidget")),
                ShowOnMobile = reader.GetBoolean(reader.GetOrdinal("ShowOnMobile")),
                StateProvince = reader.GetString(reader.GetOrdinal("StateProvince")),
                TotalTicketCount = reader.GetInt32(reader.GetOrdinal("TotalTicketCount")),
                TotalRatingSum = reader.GetInt32(reader.GetOrdinal("TotalRatingSum")),
                TotalRatingVotes = reader.GetInt32(reader.GetOrdinal("TotalRatingVotes")),
                TotalReviews = reader.GetInt32(reader.GetOrdinal("TotalReviews")),
                VatNumber = reader.GetString(reader.GetOrdinal("VatNumber")),
                VatNumberStatus = reader.GetString(reader.GetOrdinal("VatNumberStatus")),
                Website = reader.GetString(reader.GetOrdinal("Website")),
                ZipPostalCode = reader.GetString(reader.GetOrdinal("ZipPostalCode"))
            };

        public SalonInvoiceDB UpdateSalonInvoice(SalonInvoiceDB value)
        {
            if (value == null)
            {
                new ArgumentNullException("invoice cannot be null.");
            }
            if (value.InvoiceId == Guid.Empty)
            {
                new ArgumentException("invoice identifier cannot be empty.");
            }
            if (value.BillEndDate == null)
            {
                throw new ArgumentException("bill end date cannot be null.");
            }
            if (value.BillStartDate == string.Empty)
            {
                throw new ArgumentException("bill start date cannot be empty.");
            }
            if (!Regex.IsMatch(value.BillEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.BillEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            if (value.BillStartDate == null)
            {
                throw new ArgumentException("bill start date cannot be null.");
            }
            if (value.BillStartDate == string.Empty)
            {
                throw new ArgumentException("bill start date cannot be empty.");
            }
            if (!Regex.IsMatch(value.BillStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.BillStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            if (value.InvoiceNumber == null)
            {
                throw new ArgumentException("invoice number cannot be null.");
            }
            if (value.InvoiceNumber == string.Empty)
            {
                throw new ArgumentException("invoice number cannot be empty.");
            }
            if (value.PlanId == Guid.Empty)
            {
                throw new ArgumentException("plan identifier cannot be empty.");
            }
            if (value.PlanFee < 0)
            {
                throw new ArgumentException("plan fee cannot be less than zero.");
            }
            if (value.PlanStartDate == null)
            {
                throw new ArgumentException("plan start date cannot be null.");
            }
            if (value.PlanStartDate == string.Empty)
            {
                throw new ArgumentException("plan start date cannot be empty.");
            }
            if (!Regex.IsMatch(value.PlanStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.PlanStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            if (value.PlanEndDate == null)
            {
                throw new ArgumentException("plan end date cannot be null.");
            }
            if (value.PlanEndDate == string.Empty)
            {
                throw new ArgumentException("plan end date cannot be empty.");
            }
            if (!Regex.IsMatch(value.PlanEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.PlanEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            if (value.ParentId == Guid.Empty)
            {
                throw new ArgumentException("parent identifier cannot be empty.");
            }
            if (value.SalonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            if (value.SubtotalExclTax < 0)
            {
                throw new ArgumentException("subtotal excluding tax cannot be less than zero.");
            }
            if (value.TotalPaid < 0)
            {
                throw new ArgumentException("total paid cannot be less than zero.");
            }
            if (value.TotalPlan < 0)
            {
                throw new ArgumentException("total plan cannot be less than zero.");
            }
            if (value.TotalTax < 0)
            {
                throw new ArgumentException("total tax due cannot be less than zero.");
            }
            if (value.TotalWidget < 0)
            {
                throw new ArgumentException("total widget cannot be less than zero.");
            }
            if (value.TotalWidgetCount < 0)
            {
                throw new ArgumentException("total widget count cannot be less than zero.");
            }
            value.AdminComment = value.AdminComment ?? string.Empty;
            value.AuthorizationTransactionCode = value.AuthorizationTransactionCode ?? string.Empty;
            value.AuthorizationTransactionId = value.AuthorizationTransactionId ?? string.Empty;
            value.AuthorizationTransactionResult = value.AuthorizationTransactionResult ?? string.Empty;
            value.BillingAddressLine1 = value.BillingAddressLine1 ?? string.Empty;
            value.BillingAddressLine2 = value.BillingAddressLine2 ?? string.Empty;
            value.BillingAddressLine3 = value.BillingAddressLine3 ?? string.Empty;
            value.BillingAddressLine4 = value.BillingAddressLine4 ?? string.Empty;
            value.BillingAddressLine5 = value.BillingAddressLine5 ?? string.Empty;
            value.BillingCompany = value.BillingCompany ?? string.Empty;
            value.BillingEmail = value.BillingEmail ?? string.Empty;
            value.BillEndDate = value.BillEndDate ?? string.Empty;
            value.BillingFirstName = value.BillingFirstName ?? string.Empty;
            value.BillingLastName = value.BillingLastName ?? string.Empty;
            value.BillingMobile = value.BillingMobile ?? string.Empty;
            value.BillingPhoneNumber = value.BillingPhoneNumber ?? string.Empty;
            value.BillingPhoneType = value.BillingPhoneType ?? string.Empty;
            value.BillStartDate = value.BillStartDate ?? string.Empty;
            value.CaptureTransactionId = value.CaptureTransactionId ?? string.Empty;
            value.CaptureTransactionResult = value.CaptureTransactionResult ?? string.Empty;
            value.CardAlias = value.CardAlias ?? string.Empty;
            value.CardName = value.CardName ?? string.Empty;
            value.CardNumber = value.CardNumber ?? string.Empty;
            value.CardType = value.CardType ?? string.Empty;
            value.Comment = value.Comment ?? string.Empty;
            value.CreatedBy = value.CreatedBy ?? string.Empty;
            value.CurrencyCode = value.CurrencyCode ?? string.Empty;
            value.InvoiceNumber = value.InvoiceNumber ?? string.Empty;
            value.InvoiceType = value.InvoiceType ?? string.Empty;
            value.MaskedCardNumber = value.MaskedCardNumber ?? string.Empty;
            value.PlanDescription = value.PlanDescription ?? string.Empty;
            value.PaymentDueDate = value.PaymentDueDate ?? string.Empty;
            value.Status = value.Status ?? string.Empty;
            value.UpdatedBy = value.UpdatedBy ?? string.Empty;
            value.VATNumber = value.VATNumber ?? string.Empty;
            SqlParameter[] parameterArray2 = new SqlParameter[60];
            parameterArray2[0] = new SqlParameter("@InvoiceId", value.InvoiceId);
            parameterArray2[1] = new SqlParameter("@AdminComment", value.AdminComment);
            parameterArray2[2] = new SqlParameter("@AuthorizationTransactionCode", value.AuthorizationTransactionCode);
            parameterArray2[3] = new SqlParameter("@AuthorizationTransactionId", value.AuthorizationTransactionId);
            parameterArray2[4] = new SqlParameter("@AuthorizationTransactionResult", value.AuthorizationTransactionResult);
            parameterArray2[5] = new SqlParameter("@BillingAddressLine1", value.BillingAddressLine1);
            parameterArray2[6] = new SqlParameter("@BillingAddressLine2", value.BillingAddressLine2);
            parameterArray2[7] = new SqlParameter("@BillingAddressLine3", value.BillingAddressLine3);
            parameterArray2[8] = new SqlParameter("@BillingAddressLine4", value.BillingAddressLine4);
            parameterArray2[9] = new SqlParameter("@BillingAddressLine5", value.BillingAddressLine5);
            parameterArray2[10] = new SqlParameter("@BillingCompany", value.BillingCompany);
            parameterArray2[11] = new SqlParameter("@BillingEmail", value.BillingEmail);
            parameterArray2[12] = new SqlParameter("@BillEndDate", value.BillEndDate);
            parameterArray2[13] = new SqlParameter("@BillingFirstName", value.BillingFirstName);
            parameterArray2[14] = new SqlParameter("@BillingLastName", value.BillingLastName);
            parameterArray2[15] = new SqlParameter("@BillingMobile", value.BillingMobile);
            parameterArray2[0x10] = new SqlParameter("@BillingPhoneNumber", value.BillingPhoneNumber);
            parameterArray2[0x11] = new SqlParameter("@BillingPhoneNumberType", value.BillingPhoneType);
            parameterArray2[0x12] = new SqlParameter("@BillStartDate", value.BillStartDate);
            parameterArray2[0x13] = new SqlParameter("@CaptureTransactionId", value.CaptureTransactionId);
            parameterArray2[20] = new SqlParameter("@CaptureTransactionResult", value.CaptureTransactionResult);
            parameterArray2[0x15] = new SqlParameter("@CardAlias", value.CardAlias);
            parameterArray2[0x16] = new SqlParameter("@CardName", value.CardName);
            parameterArray2[0x17] = new SqlParameter("@CardNumber", value.CardNumber);
            parameterArray2[0x18] = new SqlParameter("@CardType", value.CardType);
            parameterArray2[0x19] = new SqlParameter("@Comment", value.Comment);
            parameterArray2[0x1a] = new SqlParameter("@CreatedBy", value.CreatedBy);
            parameterArray2[0x1b] = new SqlParameter("@CreatedOn", value.CreatedOn);
            parameterArray2[0x1c] = new SqlParameter("@CurrencyCode", value.CurrencyCode);
            parameterArray2[0x1d] = new SqlParameter("@Deleted", value.Deleted);
            parameterArray2[30] = new SqlParameter("@InvoiceNumber", value.InvoiceNumber);
            parameterArray2[0x1f] = new SqlParameter("@InvoiceType", value.InvoiceType);
            parameterArray2[0x20] = new SqlParameter("@MaskedCardNumber", value.MaskedCardNumber);
            parameterArray2[0x21] = new SqlParameter("@PaidOn", value.PaidOn);
            parameterArray2[0x22] = new SqlParameter("@PaymentDueDate", value.PaymentDueDate);
            parameterArray2[0x23] = new SqlParameter("@PlanId", value.PlanId);
            parameterArray2[0x24] = new SqlParameter("@PlanFee", value.PlanFee);
            parameterArray2[0x25] = new SqlParameter("@PlanType", value.PlanType);
            parameterArray2[0x26] = new SqlParameter("@PlanDescription", value.PlanDescription);
            parameterArray2[0x27] = new SqlParameter("@PlanStartDate", value.PlanStartDate);
            parameterArray2[40] = new SqlParameter("@PlanEndDate", value.PlanEndDate);
            Guid? parentId = value.ParentId;
            parameterArray2[0x29] = new SqlParameter("@ParentId", parentId.HasValue ? ((SqlGuid) parentId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[0x2a] = new SqlParameter("@Published", value.Published);
            parameterArray2[0x2b] = new SqlParameter("@SalonId", value.SalonId);
            parameterArray2[0x2c] = new SqlParameter("@Status", value.Status);
            parameterArray2[0x2d] = new SqlParameter("@SubtotalExclTax", value.SubtotalExclTax);
            parameterArray2[0x2e] = new SqlParameter("@TaxRate", value.TaxRate);
            parameterArray2[0x2f] = new SqlParameter("@TotalAmountDue", value.TotalAmountDue);
            parameterArray2[0x30] = new SqlParameter("@TotalAdjustment", value.TotalAdjustment);
            parameterArray2[0x31] = new SqlParameter("@TotalExclTax", value.TotalExclTax);
            parameterArray2[50] = new SqlParameter("@TotalInclTax", value.TotalInclTax);
            parameterArray2[0x33] = new SqlParameter("@TotalOverdue", value.TotalOverdue);
            parameterArray2[0x34] = new SqlParameter("@TotalPaid", value.TotalPaid);
            parameterArray2[0x35] = new SqlParameter("@TotalPlan", value.TotalPlan);
            parameterArray2[0x36] = new SqlParameter("@TotalTax", value.TotalTax);
            parameterArray2[0x37] = new SqlParameter("@TotalWidget", value.TotalWidget);
            parameterArray2[0x38] = new SqlParameter("@TotalWidgetCount", value.TotalWidgetCount);
            parameterArray2[0x39] = new SqlParameter("@UpdatedBy", value.UpdatedBy);
            parameterArray2[0x3a] = new SqlParameter("@UpdatedOn", value.UpdatedOn);
            parameterArray2[0x3b] = new SqlParameter("@VATNumber", value.VATNumber);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceUpdate", commandParameters) <= 0)
            {
                return null;
            }
            value = this.GetSalonInvoiceById(value.InvoiceId);
            return value;
        }

        public SalonInvoiceADDB UpdateSalonInvoiceAD(SalonInvoiceADDB value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.AdjustmentId == Guid.Empty)
            {
                throw new ArgumentException("Adjustment identifier cannot be empty.");
            }
            if (value.InvoiceId == Guid.Empty)
            {
                throw new ArgumentException("Invoice identifier cannot be empty.");
            }
            value.Description = value.Description ?? string.Empty;
            value.CreatedBy = value.CreatedBy ?? string.Empty;
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@AdjustmentId", value.AdjustmentId), new SqlParameter("@InvoiceId", value.InvoiceId), new SqlParameter("@Amount", value.Amount), new SqlParameter("@Description", value.Description), new SqlParameter("@CreatedOn", value.CreatedOn), new SqlParameter("@CreatedBy", value.CreatedBy) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInvoiceADUpdate", commandParameters) <= 0)
            {
                return null;
            }
            value = this.GetSalonInvoiceADById(value.AdjustmentId);
            return value;
        }

        public WSPDB UpdateWSP(WSPDB value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.PlanId == Guid.Empty)
            {
                throw new ArgumentException("widget subscription plan identifier cannot be empty.");
            }
            if (value.BillEndDate == null)
            {
                throw new ArgumentException("bill end date cannot be null.");
            }
            if (value.BillEndDate == string.Empty)
            {
                throw new ArgumentException("bill end date cannot be empty.");
            }
            if (!Regex.IsMatch(value.BillEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.BillEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            if (value.BillStartDate == null)
            {
                throw new ArgumentException("bill start date cannot be null.");
            }
            if (value.BillStartDate == string.Empty)
            {
                throw new ArgumentException("bill start date cannot be empty.");
            }
            if (!Regex.IsMatch(value.BillStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.BillStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            if (!string.IsNullOrEmpty(value.CancelDate))
            {
                if (!Regex.IsMatch(value.CancelDate, @"(\d{2})-(\d{2})"))
                {
                    throw new ArgumentException("cancellation date is invalid.");
                }
                try
                {
                    DateTime.ParseExact(value.CancelDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("cancellation date is invalid.");
                }
            }
            if (value.Description == null)
            {
                throw new ArgumentException("description cannot be null.");
            }
            if (value.Description == string.Empty)
            {
                throw new ArgumentException("description cannot be empty.");
            }
            if (value.ExcessFeeWT < 0)
            {
                throw new ArgumentException("excess fee cannot be less than zero.");
            }
            if (value.ExcessLimitWT < 0)
            {
                throw new ArgumentException("excess limit cannot be less than zero.");
            }
            if (value.ParentId == Guid.Empty)
            {
                throw new ArgumentException("parent identifier cannot be empty.");
            }
            if (value.PlanPrice < 0)
            {
                throw new ArgumentException("plan price cannot be less than zero.");
            }
            if (value.PlanType == null)
            {
                throw new ArgumentException("plan type cannot be null.");
            }
            if (value.PlanType == string.Empty)
            {
                throw new ArgumentException("plan type cannot be empty.");
            }
            if (value.PlanEndDate == null)
            {
                throw new ArgumentException("plan end date cannot be null.");
            }
            if (value.PlanEndDate == string.Empty)
            {
                throw new ArgumentException("plan end date cannot be empty.");
            }
            if (!Regex.IsMatch(value.PlanEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.PlanEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            if (value.PlanStartDate == null)
            {
                throw new ArgumentException("plan start date cannot be null.");
            }
            if (value.PlanStartDate == string.Empty)
            {
                throw new ArgumentException("plan start date cannot be empty.");
            }
            if (!Regex.IsMatch(value.PlanStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(value.PlanStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            if (value.SalonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            value.UpdatedBy = value.UpdatedBy ?? string.Empty;
            value.CreatedBy = value.CreatedBy ?? string.Empty;
            SqlParameter[] commandParameters = new SqlParameter[] { 
                new SqlParameter("@PlanId", value.PlanId), new SqlParameter("@ParentId", value.ParentId ?? SqlGuid.Null), new SqlParameter("@SalonId", value.SalonId), new SqlParameter("@Description", value.Description), new SqlParameter("@PlanType", value.PlanType), new SqlParameter("@PlanPrice", value.PlanPrice), new SqlParameter("@PlanStartDate", value.PlanStartDate), new SqlParameter("@PlanEndDate", value.PlanEndDate), new SqlParameter("@BillStartDate", value.BillStartDate), new SqlParameter("@BillEndDate", value.BillEndDate), new SqlParameter("@ExcessFeeWT", value.ExcessFeeWT), new SqlParameter("@ExcessLimitWT", value.ExcessLimitWT), new SqlParameter("@Prorate", value.Prorate), new SqlParameter("@CancelDate", value.CancelDate), new SqlParameter("@Active", value.Active), new SqlParameter("@CreatedOn", value.CreatedOn),
                new SqlParameter("@CreatedBy", value.CreatedBy), new SqlParameter("@UpdatedOn", value.UpdatedOn), new SqlParameter("@UpdatedBy", value.UpdatedBy)
            };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_WSPUpdate", commandParameters) <= 0)
            {
                return null;
            }
            string key = $"WSP-SALON-{"CURRENT-" + value}";
            this._cacheManager.Remove(key);
            value = this.GetWSPById(value.PlanId);
            return value;
        }

        public bool UpgradeCurrentWSP(WSPUpgradeXML input)
        {
            if (input == null)
            {
                new ArgumentNullException("input cannot be null.");
            }
            if (input.PlanParentId == Guid.Empty)
            {
                throw new ArgumentException("plan parent identity cannot be empty.");
            }
            if (input.BillEndDate == null)
            {
                throw new ArgumentException("bill end date cannot be null.");
            }
            if (input.BillEndDate == string.Empty)
            {
                throw new ArgumentException("bill end date cannot be empty.");
            }
            if (!Regex.IsMatch(input.BillEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(input.BillEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill end date is invalid.");
            }
            if (input.BillStartDate == null)
            {
                throw new ArgumentException("bill start date cannot be null.");
            }
            if (input.BillStartDate == string.Empty)
            {
                throw new ArgumentException("bill start date cannot be empty.");
            }
            if (!Regex.IsMatch(input.BillStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(input.BillStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("bill start date is invalid.");
            }
            if (input.ExcessFeeWT < 0)
            {
                throw new ArgumentException("Excess fee (widget transaction) cannot be less than zero.");
            }
            if (input.ExcessLimitWT < 0)
            {
                throw new ArgumentException("Excess limit (widget transaction) cannot be less than zero.");
            }
            if (input.InvoiceNumber == null)
            {
                throw new ArgumentException("Invoice number cannot be null.");
            }
            if (input.InvoiceNumber == string.Empty)
            {
                throw new ArgumentException("Invoice number cannot be empty.");
            }
            if (input.PaymentDueDate == null)
            {
                throw new ArgumentException("payment due date  cannot be null.");
            }
            if (input.PaymentDueDate == string.Empty)
            {
                throw new ArgumentException("payment due date  cannot be empty.");
            }
            if (!Regex.IsMatch(input.PaymentDueDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("payment due date is invalid.");
            }
            try
            {
                DateTime.ParseExact(input.PaymentDueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("payment due date is invalid.");
            }
            if (input.PlanFee < 0)
            {
                throw new ArgumentException("Plan fee cannot be less than zero.");
            }
            if (input.PlanStartDate == null)
            {
                throw new ArgumentException("plan start date cannot be null.");
            }
            if (input.PlanStartDate == string.Empty)
            {
                throw new ArgumentException("plan start date cannot be empty.");
            }
            if (!Regex.IsMatch(input.PlanStartDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            try
            {
                DateTime.ParseExact(input.PlanStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan start date is invalid.");
            }
            if (input.PlanEndDate == null)
            {
                throw new ArgumentException("plan end date cannot be null.");
            }
            if (input.PlanEndDate == string.Empty)
            {
                throw new ArgumentException("plan end date cannot be empty.");
            }
            if (!Regex.IsMatch(input.PlanEndDate, @"(\d{2})-(\d{2})"))
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            try
            {
                DateTime.ParseExact(input.PlanEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("plan end date is invalid.");
            }
            if (input.PlanType == null)
            {
                throw new ArgumentNullException("plan type cannot be null.");
            }
            if (input.SalonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            if (input.TaxRate == null)
            {
                throw new ArgumentException("Tax rate cannot be null.");
            }
            if (input.TaxRate == string.Empty)
            {
                throw new ArgumentException("Tax rate cannot be empty.");
            }
            if (input.TotalPlan < 0)
            {
                throw new ArgumentException("Total plan cannot be less than zero.");
            }
            if (input.TotalTax < 0)
            {
                throw new ArgumentException("Total tax cannot be less than zero.");
            }
            input.BillingAddressLine1 = input.BillingAddressLine1 ?? string.Empty;
            input.BillingAddressLine2 = input.BillingAddressLine2 ?? string.Empty;
            input.BillingAddressLine3 = input.BillingAddressLine3 ?? string.Empty;
            input.BillingAddressLine4 = input.BillingAddressLine4 ?? string.Empty;
            input.BillingAddressLine5 = input.BillingAddressLine5 ?? string.Empty;
            input.BillingCompany = input.BillingCompany ?? string.Empty;
            input.BillingEmail = input.BillingEmail ?? string.Empty;
            input.BillingFirstName = input.BillingFirstName ?? string.Empty;
            input.BillingLastName = input.BillingLastName ?? string.Empty;
            input.BillingMobile = input.BillingMobile ?? string.Empty;
            input.BillingPhoneNumber = input.BillingPhoneNumber ?? string.Empty;
            input.BillingPhoneNumberType = input.BillingPhoneNumberType ?? string.Empty;
            input.CreatedBy = input.CreatedBy ?? string.Empty;
            input.CurrencyCode = input.CurrencyCode ?? string.Empty;
            input.InvoiceNumber = input.InvoiceNumber ?? string.Empty;
            input.InvoiceType = input.InvoiceType ?? string.Empty;
            input.PaymentDueDate = input.PaymentDueDate ?? string.Empty;
            input.PlanDescription = input.PlanDescription ?? string.Empty;
            input.Status = input.Status ?? string.Empty;
            input.VATNumber = input.VATNumber ?? string.Empty;
            XDocument document = new XDocument(new object[] { new XElement("Upgrade", new object[] { 
                new XElement("PlanParentId", input.PlanParentId), new XElement("SalonId", input.SalonId), new XElement("InvoiceNumber", input.InvoiceNumber), new XElement("InvoiceType", input.InvoiceType), new XElement("PlanDescription", input.PlanDescription), new XElement("PlanType", input.PlanType), new XElement("PlanFee", input.PlanFee), new XElement("ExcessFeeWT", input.ExcessFeeWT), new XElement("ExcessLimitWT", input.ExcessLimitWT), new XElement("PlanStartDate", input.PlanStartDate), new XElement("PlanEndDate", input.PlanEndDate), new XElement("BillingFirstName", input.BillingFirstName), new XElement("BillingLastName", input.BillingLastName), new XElement("BillingCompany", input.BillingCompany), new XElement("BillingPhoneNumberType", input.BillingPhoneNumberType), new XElement("BillingPhoneNumber", input.BillingPhoneNumber),
                new XElement("BillingMobile", input.BillingMobile), new XElement("BillingEmail", input.BillingEmail), new XElement("BillingAddressLine1", input.BillingAddressLine1), new XElement("BillingAddressLine2", input.BillingAddressLine2), new XElement("BillingAddressLine3", input.BillingAddressLine3), new XElement("BillingAddressLine4", input.BillingAddressLine4), new XElement("BillingAddressLine5", input.BillingAddressLine5), new XElement("BillStartDate", input.BillStartDate), new XElement("BillEndDate", input.BillEndDate), new XElement("PaymentDueDate", input.PaymentDueDate), new XElement("CurrencyCode", input.CurrencyCode), new XElement("VATNumber", input.VATNumber), new XElement("TaxRate", input.TaxRate), new XElement("SubtotalExclTax", input.SubtotalExclTax), new XElement("TotalPlan", input.TotalPlan), new XElement("TotalTax", input.TotalTax),
                new XElement("TotalExclTax", input.TotalExclTax), new XElement("TotalInclTax", input.TotalInclTax), new XElement("TotalOverdue", input.TotalOverdue), new XElement("TotalAmountDue", input.TotalAmountDue), new XElement("Status", input.Status), new XElement("CreatedOn", input.CreatedOn.ToString("yyyy-MM-ddTHH:mm:ss.fff")), new XElement("CreatedBy", input.CreatedBy)
            }) });
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("Upgrade_XML", new SqlXml(document.CreateReader())) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_WSPUpgrade", commandParameters) <= 0)
            {
                return false;
            }
            return true;
        }

        private WSPDB WSPMapping(SqlDataReader reader) => 
            new WSPDB { 
                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                BillEndDate = reader.GetString(reader.GetOrdinal("BillEndDate")),
                BillStartDate = reader.GetString(reader.GetOrdinal("BillStartDate")),
                CancelDate = reader.GetString(reader.GetOrdinal("CancelDate")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                ExcessFeeWT = reader.GetInt32(reader.GetOrdinal("ExcessFeeWT")),
                ExcessLimitWT = reader.GetInt32(reader.GetOrdinal("ExcessLimitWT")),
                ParentId = reader.GetValue(reader.GetOrdinal("ParentId")) as Guid?,
                PlanEndDate = reader.GetString(reader.GetOrdinal("PlanEndDate")),
                PlanPrice = reader.GetInt32(reader.GetOrdinal("PlanPrice")),
                PlanStartDate = reader.GetString(reader.GetOrdinal("PlanStartDate")),
                PlanType = reader.GetString(reader.GetOrdinal("PlanType")),
                Prorate = reader.GetBoolean(reader.GetOrdinal("Prorate")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                UpdatedBy = reader.GetString(reader.GetOrdinal("UpdatedBy")),
                UpdatedOn = reader.GetDateTime(reader.GetOrdinal("UpdatedOn")),
                PlanId = reader.GetGuid(reader.GetOrdinal("PlanId"))
            };
    }
}

