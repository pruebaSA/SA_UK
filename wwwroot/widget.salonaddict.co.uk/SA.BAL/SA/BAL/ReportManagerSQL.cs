namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Runtime.InteropServices;

    public class ReportManagerSQL : IReportManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public ReportManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        private BillingTotalsDB BillingTotalsMapping(SqlDataReader reader) => 
            new BillingTotalsDB { 
                CountThisMonth = reader.GetInt32(reader.GetOrdinal("CountThisMonth")),
                CountThisWeek = reader.GetInt32(reader.GetOrdinal("CountThisWeek")),
                CountThisYear = reader.GetInt32(reader.GetOrdinal("CountThisYear")),
                CountToday = reader.GetInt32(reader.GetOrdinal("CountToday")),
                SumThisMonth = reader.GetInt32(reader.GetOrdinal("SumThisMonth")),
                SumThisWeek = reader.GetInt32(reader.GetOrdinal("SumThisWeek")),
                SumThisYear = reader.GetInt32(reader.GetOrdinal("SumThisYear")),
                SumToday = reader.GetInt32(reader.GetOrdinal("SumToday"))
            };

        private BookingTotalsDB BookingTotalsMapping(SqlDataReader reader) => 
            new BookingTotalsDB { 
                CountThisMonth = reader.GetInt32(reader.GetOrdinal("CountThisMonth")),
                CountThisWeek = reader.GetInt32(reader.GetOrdinal("CountThisWeek")),
                CountThisYear = reader.GetInt32(reader.GetOrdinal("CountThisYear")),
                CountToday = reader.GetInt32(reader.GetOrdinal("CountToday")),
                SumThisMonth = reader.GetDecimal(reader.GetOrdinal("SumThisMonth")),
                SumThisWeek = reader.GetDecimal(reader.GetOrdinal("SumThisWeek")),
                SumThisYear = reader.GetDecimal(reader.GetOrdinal("SumThisYear")),
                SumToday = reader.GetDecimal(reader.GetOrdinal("SumToday"))
            };

        public BillingTotalsDB GetBillingTotalsReport(Guid? salonId, string currencyCode)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("salonId", "Salon identifier cannot be empty.");
            }
            BillingTotalsDB sdb = null;
            SqlParameter[] parameterArray2 = new SqlParameter[2];
            Guid? nullable2 = salonId;
            parameterArray2[0] = new SqlParameter("@SalonId", nullable2.HasValue ? ((SqlGuid) nullable2.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[1] = new SqlParameter("@CurrencyCode", currencyCode);
            SqlParameter[] commandParameters = parameterArray2;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportBillingTotalsLoad", commandParameters))
            {
                if (reader.Read())
                {
                    sdb = this.BillingTotalsMapping(reader);
                }
            }
            return sdb;
        }

        public BookingTotalsDB GetBookingTotalsReport(Guid? salonId, string currencyCode)
        {
            BookingTotalsDB sdb = null;
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@SalonId", salonId), new SqlParameter("@CurrencyCode", currencyCode) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportBookingTotalsLoad", commandParameters))
            {
                if (reader.Read())
                {
                    sdb = this.BookingTotalsMapping(reader);
                }
            }
            return sdb;
        }

        public List<SalonDB> GetPlanReport(string planType, string currencyCode, int pageIndex, int pageSize, out int totalRecords)
        {
            if (planType == null)
            {
                throw new ArgumentNullException("planType");
            }
            if (planType == string.Empty)
            {
                throw new ArgumentException("planType cannot be empty.");
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
            List<SalonDB> list = new List<SalonDB>();
            SqlParameter parameter = new SqlParameter("@TotalRecords", SqlDbType.Int) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@PlanType", planType), new SqlParameter("@CurrencyCode", currencyCode), new SqlParameter("@PageIndex", pageIndex), new SqlParameter("@PageSize", pageSize), parameter };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportSalonPlanTypeLoad", commandParameters))
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

        public List<PlanTotalsDB> GetPlanTotalsReport(string currencyCode)
        {
            List<PlanTotalsDB> list = new List<PlanTotalsDB>();
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@CurrencyCode", currencyCode) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportPlanTotalLoad", commandParameters))
            {
                while (reader.Read())
                {
                    PlanTotalsDB item = this.PlanTotalsMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public int GetSalonCount(string currencyCode)
        {
            int num = 0;
            new SqlParameter("@CurrencyCode", currencyCode);
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_ReportSalonCount", new object[] { currencyCode }))
            {
                if (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
            }
            return num;
        }

        public int GetSalonUnreadAppointments(Guid salonId)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("order identifier cannot be empty.");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            int num = 0;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportSalonUnreadAppointments", commandParameters))
            {
                if (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
            }
            return num;
        }

        public List<WidgetOfflineSummaryDB> GetWidgetOfflineReport(string currencyCode, int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", "PageIndex cannot be less than zero.");
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", "PageSize cannot be less than zero.");
            }
            List<WidgetOfflineSummaryDB> list = new List<WidgetOfflineSummaryDB>();
            SqlParameter parameter = new SqlParameter("@TotalRecords", SqlDbType.Int) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@CurrencyCode", currencyCode), new SqlParameter("@PageIndex", pageIndex), new SqlParameter("@PageSize", pageSize), parameter };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportWidgetOfflineSummaryLoad", commandParameters))
            {
                while (reader.Read())
                {
                    WidgetOfflineSummaryDB item = this.WidgetOfflineSummaryMapping(reader);
                    list.Add(item);
                }
            }
            totalRecords = int.Parse(parameter.Value.ToString());
            return list;
        }

        public List<WSPExpiringDB> GetWSPExpiryReport(string planType, string currencyCode, int numberOfDays, int pageIndex, int pageSize, out int totalRecords)
        {
            if (planType == null)
            {
                throw new ArgumentNullException("planType");
            }
            if (numberOfDays < 1)
            {
                numberOfDays = 1;
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
            SqlParameter parameter = new SqlParameter("@TotalRecords", SqlDbType.Int) {
                Direction = ParameterDirection.Output
            };
            List<WSPExpiringDB> list = new List<WSPExpiringDB>();
            SqlParameter[] commandParameters = new SqlParameter[] { parameter, new SqlParameter("@PlanType", planType), new SqlParameter("@CurrencyCode", currencyCode), new SqlParameter("@NumberOfDays", numberOfDays), new SqlParameter("@PageIndex", pageIndex), new SqlParameter("@PageSize", pageSize) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ReportWSPExpiringLoad", commandParameters))
            {
                while (reader.Read())
                {
                    WSPExpiringDB item = this.WSPExpiringMapping(reader);
                    list.Add(item);
                }
            }
            totalRecords = int.Parse(parameter.Value.ToString());
            return list;
        }

        private PlanTotalsDB PlanTotalsMapping(SqlDataReader reader) => 
            new PlanTotalsDB { 
                Total = reader.GetInt32(reader.GetOrdinal("Total")),
                PlanType = reader.GetString(reader.GetOrdinal("PlanType"))
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
                TotalRatingSum = reader.GetDouble(reader.GetOrdinal("TotalRatingSum")),
                TotalRatingVotes = reader.GetDouble(reader.GetOrdinal("TotalRatingVotes")),
                TotalReviews = reader.GetInt32(reader.GetOrdinal("TotalReviews")),
                VatNumber = reader.GetString(reader.GetOrdinal("VatNumber")),
                VatNumberStatus = reader.GetString(reader.GetOrdinal("VatNumberStatus")),
                Website = reader.GetString(reader.GetOrdinal("Website")),
                ZipPostalCode = reader.GetString(reader.GetOrdinal("ZipPostalCode"))
            };

        private WidgetOfflineSummaryDB WidgetOfflineSummaryMapping(SqlDataReader reader) => 
            new WidgetOfflineSummaryDB { 
                AddressLine1 = reader.GetString(reader.GetOrdinal("AddressLine1")),
                AddressLine2 = reader.GetString(reader.GetOrdinal("AddressLine2")),
                AddressLine3 = reader.GetString(reader.GetOrdinal("AddressLine3")),
                AddressLine4 = reader.GetString(reader.GetOrdinal("AddressLine4")),
                AddressLine5 = reader.GetString(reader.GetOrdinal("AddressLine5")),
                CreatedOnUtc = reader.GetDateTime(reader.GetOrdinal("CreatedOnUtc")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId"))
            };

        private WSPExpiringDB WSPExpiringMapping(SqlDataReader reader) => 
            new WSPExpiringDB { 
                AddressLine1 = reader.GetString(reader.GetOrdinal("AddressLine1")),
                AddressLine2 = reader.GetString(reader.GetOrdinal("AddressLine2")),
                AddressLine3 = reader.GetString(reader.GetOrdinal("AddressLine3")),
                AddressLine4 = reader.GetString(reader.GetOrdinal("AddressLine4")),
                AddressLine5 = reader.GetString(reader.GetOrdinal("AddressLine5")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                PlanDescription = reader.GetString(reader.GetOrdinal("PlanDescription")),
                PlanEndDate = reader.GetString(reader.GetOrdinal("PlanEndDate")),
                PlanPrice = reader.GetInt32(reader.GetOrdinal("PlanPrice")),
                PlanStartDate = reader.GetString(reader.GetOrdinal("PlanStartDate")),
                PlanType = reader.GetString(reader.GetOrdinal("PlanType")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                PlanId = reader.GetGuid(reader.GetOrdinal("PlanId"))
            };
    }
}

