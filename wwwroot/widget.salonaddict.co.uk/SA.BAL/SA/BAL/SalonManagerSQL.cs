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

    public class SalonManagerSQL : ISalonManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public SalonManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        public ClosingDayDB ClosingDayMapping(SqlDataReader reader) => 
            new ClosingDayDB { 
                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                ClosingDayId = reader.GetGuid(reader.GetOrdinal("ClosingDayId")),
                CycleLength = reader.GetInt32(reader.GetOrdinal("CycleLength")),
                CyclePeriodType = reader.GetInt32(reader.GetOrdinal("CyclePeriodType")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                StartDateUtc = reader.GetDateTime(reader.GetOrdinal("StartDateUtc")),
                TotalCycles = reader.GetValue(reader.GetOrdinal("TotalCycles")) as int?
            };

        public void DeleteClosingDay(ClosingDayDB closingDay)
        {
            if (closingDay == null)
            {
                throw new ArgumentNullException("closingDay");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@ClosingDayId", closingDay.ClosingDayId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ClosingDayDeleteById", commandParameters);
            string key = $"CLOSING-DAY-{"SALON-" + closingDay.SalonId}";
            this._cacheManager.Remove(key);
        }

        public void DeleteSalonPaymentMethod(SalonPaymentMethodDB paymentMethod)
        {
            if (paymentMethod == null)
            {
                throw new ArgumentNullException("paymentMethod");
            }
            if (paymentMethod.SalonPaymentMethodId == Guid.Empty)
            {
                throw new ArgumentException("Salon payment method identifier cannot be empty");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@SalonPaymentMethodId", paymentMethod.SalonPaymentMethodId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonPaymentMethodDeleteById", commandParameters);
        }

        public void DeleteSalonReview(SalonReviewDB review)
        {
            if (review == null)
            {
                throw new ArgumentNullException("review");
            }
            if (review.SalonReviewId == Guid.Empty)
            {
                throw new ArgumentException("SalonReviewId cannot be empty");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@SalonReviewId", review.SalonReviewId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonReviewDeleteById", commandParameters);
        }

        public ClosingDayDB GetClosingDayById(Guid closingDayId)
        {
            ClosingDayDB ydb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@ClosingDayId", closingDayId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_ClosingDayLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    ydb = this.ClosingDayMapping(reader);
                }
            }
            return ydb;
        }

        public List<ClosingDayDB> GetClosingDaysBySalonId(Guid salonId)
        {
            List<ClosingDayDB> list = new List<ClosingDayDB>();
            string key = $"CLOSING-DAY-{"SALON-" + salonId}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<ClosingDayDB>) obj2;
            }
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_ClosingDayLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    ClosingDayDB item = this.ClosingDayMapping(reader);
                    list.Add(item);
                }
            }
            this._cacheManager.Add(key, list);
            return list;
        }

        public OpeningHoursDB GetOpeningHoursById(Guid openingHoursId)
        {
            if (openingHoursId == Guid.Empty)
            {
                throw new ArgumentException("Identifier cannot be empty.");
            }
            OpeningHoursDB sdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@OpeningHoursId", openingHoursId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_OpeningHoursLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    sdb = this.OpeningHoursMapping(reader);
                }
            }
            return sdb;
        }

        public OpeningHoursDB GetOpeningHoursBySalonId(Guid salonId)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("Identifier cannot be empty.");
            }
            string key = $"OPENING_HOURS-{"SALON-" + salonId}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (OpeningHoursDB) obj2;
            }
            OpeningHoursDB sdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_OpeningHoursLoadBySalonId", parameterValues))
            {
                if (reader.Read())
                {
                    sdb = this.OpeningHoursMapping(reader);
                }
            }
            this._cacheManager.Add(key, sdb);
            return sdb;
        }

        public SalonDB GetSalonById(Guid salonId)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            string key = $"SALON-{salonId}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (SalonDB) obj2;
            }
            SalonDB ndb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    ndb = this.SalonMapping(reader);
                }
            }
            this._cacheManager.Add(key, ndb);
            return ndb;
        }

        public SalonDB GetSalonBySEName(string sename)
        {
            if (string.IsNullOrEmpty(sename))
            {
                return null;
            }
            string key = $"SALON-{sename}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (SalonDB) obj2;
            }
            SalonDB ndb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SEName", sename) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonLoadBySEName", parameterValues))
            {
                if (reader.Read())
                {
                    ndb = this.SalonMapping(reader);
                }
            }
            this._cacheManager.Add(key, ndb);
            return ndb;
        }

        public SalonPaymentMethodDB GetSalonPaymentMethodById(Guid paymentMethodId)
        {
            if (paymentMethodId == Guid.Empty)
            {
                throw new ArgumentException("salon payment method identifier cannot be empty.");
            }
            SalonPaymentMethodDB ddb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonPaymentMethodId", paymentMethodId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonPaymentMethodLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    ddb = this.SalonPaymentMethodMapping(reader);
                }
            }
            return ddb;
        }

        public List<SalonPaymentMethodDB> GetSalonPaymentMethodsBySalonId(Guid salonId)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            List<SalonPaymentMethodDB> list = new List<SalonPaymentMethodDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonPaymentMethodLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    SalonPaymentMethodDB item = this.SalonPaymentMethodMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public SalonRatingDB GetSalonRatingById(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("SalonRatingId cannot be empty.");
            }
            SalonRatingDB gdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonRatingId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonRatingLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    gdb = this.SalonRatingMapping(reader);
                }
            }
            return gdb;
        }

        public SalonRatingDB GetSalonRatingBySalonId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("SalonId cannot be empty.");
            }
            SalonRatingDB gdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonRatingLoadBySalonId", parameterValues))
            {
                if (reader.Read())
                {
                    gdb = this.SalonRatingMapping(reader);
                }
            }
            return gdb;
        }

        public SalonReviewDB GetSalonReviewById(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("SalonReviewId cannot be empty.");
            }
            SalonReviewDB wdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonReviewId", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonReviewLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    wdb = this.SalonReviewMapping(reader);
                }
            }
            return wdb;
        }

        public List<SalonReviewDB> GetSalonReviewsBySalonId(Guid salonId, bool approvedOnly, string orderBy, int pageIndex, int pageSize, out int totalRecords)
        {
            string str;
            totalRecords = 0;
            if (salonId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("salonId", "SalonId cannot be empty.");
            }
            if ((orderBy != null) && (((str = orderBy) == null) || (((str != "HIGHEST") && (str != "LOWEST")) && ((str != "OLDEST") && (str != "NEWEST")))))
            {
                throw new ArgumentOutOfRangeException("orderBy", "OrderBy is invalid.");
            }
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", "PageIndex cannot be less than zero.");
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", "PageSize cannot be less than zero.");
            }
            if (pageSize > 50)
            {
                throw new ArgumentOutOfRangeException("pageSize", "PageSize cannot be greater than 50.");
            }
            orderBy = orderBy ?? "NEWEST";
            List<SalonReviewDB> list = new List<SalonReviewDB>();
            SqlParameter parameter = new SqlParameter("@TotalRecords", SqlDbType.Int) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@SalonId", salonId), new SqlParameter("@ApprovedOnly", approvedOnly), new SqlParameter("@OrderBy", orderBy), new SqlParameter("@PageIndex", pageIndex), new SqlParameter("@PageSize", pageSize), parameter };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonReviewLoadBySalonIdPaged", commandParameters))
            {
                while (reader.Read())
                {
                    SalonReviewDB item = this.SalonReviewMapping(reader);
                    list.Add(item);
                }
            }
            totalRecords = int.Parse(parameter.Value.ToString());
            return list;
        }

        public List<SalonDB> GetSalonsBySalonAttribute(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (key == string.Empty)
            {
                throw new ArgumentException("key cannot be empty.");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value == string.Empty)
            {
                throw new ArgumentException("value cannot be empty.");
            }
            List<SalonDB> list = new List<SalonDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@Key", key), new SqlParameter("@Value", value) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonLoadBySalonAttribute", parameterValues))
            {
                while (reader.Read())
                {
                    SalonDB item = this.SalonMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public List<SalonDB> GetSalonsThatStartWith(string value, string currencyCode)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Search term cannot be null or empty.");
            }
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SearchTerm", value), new SqlParameter("@CurrencyCode", currencyCode) };
            List<SalonDB> list = new List<SalonDB>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonLoadByStartsWith", parameterValues))
            {
                while (reader.Read())
                {
                    list.Add(this.SalonMapping(reader));
                }
            }
            return list;
        }

        public ClosingDayDB InsertClosingDay(ClosingDayDB closingDay)
        {
            if (closingDay == null)
            {
                throw new ArgumentNullException("closingDay");
            }
            closingDay.Category = closingDay.Category ?? string.Empty;
            closingDay.Description = closingDay.Description ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@ClosingDayId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[9];
            parameterArray2[0] = parameter;
            parameterArray2[1] = new SqlParameter("@Active", closingDay.Active);
            parameterArray2[2] = new SqlParameter("@Category", closingDay.Category);
            parameterArray2[3] = new SqlParameter("@CycleLength", closingDay.CycleLength);
            parameterArray2[4] = new SqlParameter("@CyclePeriodType", closingDay.CyclePeriodType);
            parameterArray2[5] = new SqlParameter("@Description", closingDay.Description);
            parameterArray2[6] = new SqlParameter("@SalonId", closingDay.SalonId);
            parameterArray2[7] = new SqlParameter("@StartDateUtc", closingDay.StartDateUtc.Date);
            int? totalCycles = closingDay.TotalCycles;
            parameterArray2[8] = new SqlParameter("@TotalCycles", totalCycles.HasValue ? ((SqlInt32) totalCycles.GetValueOrDefault()) : SqlInt32.Null);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ClosingDayInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid closingDayId = new Guid(parameter.Value.ToString());
            closingDay = this.GetClosingDayById(closingDayId);
            string key = $"CLOSING-DAY-{"SALON-" + closingDay.SalonId}";
            this._cacheManager.Remove(key);
            return closingDay;
        }

        public bool InsertClosingDaysXML(List<ClosingDayDB> closingDays)
        {
            if (closingDays == null)
            {
                throw new ArgumentNullException("closingDays");
            }
            XDocument document = new XDocument(new object[] { new XElement("Items") });
            foreach (ClosingDayDB ydb in closingDays)
            {
                object[] content = new object[] { new XElement("SalonId", ydb.SalonId), new XElement("Category", ydb.Category ?? string.Empty), new XElement("Description", ydb.Description ?? string.Empty), new XElement("CycleLength", ydb.CycleLength), new XElement("CyclePeriodType", ydb.CyclePeriodType), new XElement("TotalCycles", ydb.TotalCycles), new XElement("StartDateUtc", ydb.StartDateUtc.Date.ToString("yyyy-MM-ddTHH:mm:ss.fff")), new XElement("Active", ydb.Active ? 1 : 0) };
                document.Root.Add(new XElement("Item", content));
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@ClosingDay_XML", new SqlXml(document.CreateReader())) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ClosingDayInsertXML", commandParameters) <= 0)
            {
                return false;
            }
            closingDays.ForEach(delegate (ClosingDayDB item) {
                string key = $"CLOSING-DAY-{"SALON-" + item.SalonId}";
                this._cacheManager.Remove(key);
            });
            return true;
        }

        public OpeningHoursDB InsertOpeningHours(OpeningHoursDB hours)
        {
            if (hours == null)
            {
                throw new ArgumentNullException("hours");
            }
            if (hours.SunClosed)
            {
                hours.SunStart1 = null;
                hours.SunEnd1 = null;
                hours.SunStart2 = null;
                hours.SunEnd2 = null;
            }
            if (hours.MonClosed)
            {
                hours.MonStart1 = null;
                hours.MonEnd1 = null;
                hours.MonStart2 = null;
                hours.MonEnd2 = null;
            }
            if (hours.TueClosed)
            {
                hours.TueStart1 = null;
                hours.TueEnd1 = null;
                hours.TueStart2 = null;
                hours.TueEnd2 = null;
            }
            if (hours.WedClosed)
            {
                hours.WedStart1 = null;
                hours.WedEnd1 = null;
                hours.WedStart2 = null;
                hours.WedEnd2 = null;
            }
            if (hours.ThuClosed)
            {
                hours.ThuStart1 = null;
                hours.ThuEnd1 = null;
                hours.ThuStart2 = null;
                hours.ThuEnd2 = null;
            }
            if (hours.FriClosed)
            {
                hours.FriStart1 = null;
                hours.FriEnd1 = null;
                hours.FriStart2 = null;
                hours.FriEnd2 = null;
            }
            if (hours.SatClosed)
            {
                hours.SatStart1 = null;
                hours.SatEnd1 = null;
                hours.SatStart2 = null;
                hours.SatEnd2 = null;
            }
            SqlParameter parameter = new SqlParameter("@OpeningHoursId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { 
                parameter, new SqlParameter("@FriClosed", hours.FriClosed), new SqlParameter("@FriEnd1", hours.FriEnd1 ?? DBNull.Value), new SqlParameter("@FriEnd2", hours.FriEnd2 ?? DBNull.Value), new SqlParameter("@FriStart1", hours.FriStart1 ?? DBNull.Value), new SqlParameter("@FriStart2", hours.FriStart2 ?? DBNull.Value), new SqlParameter("@MonClosed", hours.MonClosed), new SqlParameter("@MonEnd1", hours.MonEnd1 ?? DBNull.Value), new SqlParameter("@MonEnd2", hours.MonEnd2 ?? DBNull.Value), new SqlParameter("@MonStart1", hours.MonStart1 ?? DBNull.Value), new SqlParameter("@MonStart2", hours.MonStart2 ?? DBNull.Value), new SqlParameter("@SalonId", hours.SalonId), new SqlParameter("@SatClosed", hours.SatClosed), new SqlParameter("@SatEnd1", hours.SatEnd1 ?? DBNull.Value), new SqlParameter("@SatEnd2", hours.SatEnd2 ?? DBNull.Value), new SqlParameter("@SatStart1", hours.SatStart1 ?? DBNull.Value),
                new SqlParameter("@SatStart2", hours.SatStart2 ?? DBNull.Value), new SqlParameter("@ShowOnWeb", hours.ShowOnWeb), new SqlParameter("@ShowOnMobile", hours.ShowOnMobile), new SqlParameter("@ShowOnWidget", hours.ShowOnWidget), new SqlParameter("@SunClosed", hours.SunClosed), new SqlParameter("@SunEnd1", hours.SunEnd1 ?? DBNull.Value), new SqlParameter("@SunEnd2", hours.SunEnd2 ?? DBNull.Value), new SqlParameter("@SunStart1", hours.SunStart1 ?? DBNull.Value), new SqlParameter("@SunStart2", hours.SunStart2 ?? DBNull.Value), new SqlParameter("@ThuClosed", hours.ThuClosed), new SqlParameter("@ThuEnd1", hours.ThuEnd1 ?? DBNull.Value), new SqlParameter("@ThuEnd2", hours.ThuEnd2 ?? DBNull.Value), new SqlParameter("@ThuStart1", hours.ThuStart1 ?? DBNull.Value), new SqlParameter("@ThuStart2", hours.ThuStart2 ?? DBNull.Value), new SqlParameter("@TueClosed", hours.TueClosed), new SqlParameter("@TueEnd1", hours.TueEnd1 ?? DBNull.Value),
                new SqlParameter("@TueEnd2", hours.TueEnd2 ?? DBNull.Value), new SqlParameter("@TueStart1", hours.TueStart1 ?? DBNull.Value), new SqlParameter("@TueStart2", hours.TueStart2 ?? DBNull.Value), new SqlParameter("@WedClosed", hours.WedClosed), new SqlParameter("@WedEnd1", hours.WedEnd1 ?? DBNull.Value), new SqlParameter("@WedEnd2", hours.WedEnd2 ?? DBNull.Value), new SqlParameter("@WedStart1", hours.WedStart1 ?? DBNull.Value), new SqlParameter("@WedStart2", hours.WedStart2 ?? DBNull.Value)
            };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_OpeningHoursInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid openingHoursId = new Guid(parameter.Value.ToString());
            hours = this.GetOpeningHoursById(openingHoursId);
            return hours;
        }

        public SalonDB InsertSalon(SalonDB salon)
        {
            if (salon == null)
            {
                throw new ArgumentNullException("salon");
            }
            if (salon.Name == null)
            {
                throw new ArgumentNullException("salon.Name");
            }
            if (salon.SEName == null)
            {
                throw new ArgumentNullException("salon.SEName");
            }
            if (salon.Latitude == null)
            {
                throw new ArgumentNullException("salon.Latitude");
            }
            if (salon.Longitude == null)
            {
                throw new ArgumentNullException("salon.Longitude");
            }
            if (salon.TotalTicketCount < 0)
            {
                throw new ArgumentOutOfRangeException("salon.TotalTicketCount", salon.TotalTicketCount, "total ticket count cannot be less than 0.");
            }
            if (salon.TotalRatingSum < 0.0)
            {
                throw new ArgumentOutOfRangeException("salon.TotalRatingSum", salon.TotalRatingSum, "total rating sum order cannot be less than 0.");
            }
            if (salon.TotalRatingVotes < 0.0)
            {
                throw new ArgumentOutOfRangeException("salon.TotalRatingVotes", salon.TotalRatingVotes, "total rating votes cannot be less than 0.");
            }
            if (salon.TotalReviews < 0)
            {
                throw new ArgumentOutOfRangeException("salon.TotalReviews", salon.TotalReviews, "total reviews cannot be less than 0.");
            }
            salon.Abbreviation = salon.Abbreviation ?? string.Empty;
            salon.AddressLine1 = salon.AddressLine1 ?? string.Empty;
            salon.AddressLine2 = salon.AddressLine2 ?? string.Empty;
            salon.AddressLine3 = salon.AddressLine3 ?? string.Empty;
            salon.AddressLine4 = salon.AddressLine4 ?? string.Empty;
            salon.AddressLine5 = salon.AddressLine5 ?? string.Empty;
            salon.AdminComment = salon.AdminComment ?? string.Empty;
            salon.CityTown = salon.CityTown ?? string.Empty;
            salon.Company = salon.Company ?? string.Empty;
            salon.Country = salon.Country ?? string.Empty;
            salon.County = salon.County ?? string.Empty;
            salon.Directions = salon.Directions ?? string.Empty;
            salon.Email = salon.Email ?? string.Empty;
            salon.FaxNumber = salon.FaxNumber ?? string.Empty;
            salon.FullDescription = salon.FullDescription ?? string.Empty;
            salon.Latitude = salon.Latitude ?? string.Empty;
            salon.Longitude = salon.Longitude ?? string.Empty;
            salon.MetaDescription = salon.MetaDescription ?? string.Empty;
            salon.MetaKeywords = salon.MetaKeywords ?? string.Empty;
            salon.MetaTitle = salon.MetaTitle ?? string.Empty;
            salon.Mobile = salon.Mobile ?? string.Empty;
            salon.Name = salon.Name ?? string.Empty;
            salon.PhoneNumber = salon.PhoneNumber ?? string.Empty;
            salon.RealexPayerRef = salon.RealexPayerRef ?? string.Empty;
            salon.SEName = salon.SEName ?? string.Empty;
            salon.ShortDescription = salon.ShortDescription ?? string.Empty;
            salon.StateProvince = salon.StateProvince ?? string.Empty;
            salon.VatNumber = salon.VatNumber ?? string.Empty;
            salon.VatNumberStatus = salon.VatNumberStatus ?? string.Empty;
            salon.Website = salon.Website ?? string.Empty;
            salon.ZipPostalCode = salon.ZipPostalCode ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@SalonId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[0x30];
            parameterArray2[0] = parameter;
            parameterArray2[1] = new SqlParameter("@Abbreviation", salon.Abbreviation);
            parameterArray2[2] = new SqlParameter("@Active", salon.Active);
            parameterArray2[3] = new SqlParameter("@AddressLine1", salon.AddressLine1);
            parameterArray2[4] = new SqlParameter("@AddressLine2", salon.AddressLine2);
            parameterArray2[5] = new SqlParameter("@AddressLine3", salon.AddressLine3);
            parameterArray2[6] = new SqlParameter("@AddressLine4", salon.AddressLine4);
            parameterArray2[7] = new SqlParameter("@AddressLine5", salon.AddressLine5);
            parameterArray2[8] = new SqlParameter("@AdminComment", salon.AdminComment);
            parameterArray2[9] = new SqlParameter("@BookOnWeb", salon.BookOnWeb);
            parameterArray2[10] = new SqlParameter("@BookOnWidget", salon.BookOnWidget);
            parameterArray2[11] = new SqlParameter("@BookOnMobile", salon.BookOnMobile);
            parameterArray2[12] = new SqlParameter("@CityTown", salon.CityTown);
            parameterArray2[13] = new SqlParameter("@Company", salon.Company);
            parameterArray2[14] = new SqlParameter("@Country", salon.Country);
            parameterArray2[15] = new SqlParameter("@County", salon.County);
            parameterArray2[0x10] = new SqlParameter("@CreatedOnUtc", salon.CreatedOnUtc);
            parameterArray2[0x11] = new SqlParameter("@Deleted", salon.Deleted);
            parameterArray2[0x12] = new SqlParameter("@Directions", salon.Directions);
            parameterArray2[0x13] = new SqlParameter("@Email", salon.Email);
            parameterArray2[20] = new SqlParameter("@FaxNumber", salon.FaxNumber);
            parameterArray2[0x15] = new SqlParameter("@FullDescription", salon.FullDescription);
            parameterArray2[0x16] = new SqlParameter("@Latitude", salon.Latitude);
            parameterArray2[0x17] = new SqlParameter("@Longitude", salon.Longitude);
            parameterArray2[0x18] = new SqlParameter("@MetaDescription", salon.MetaDescription);
            parameterArray2[0x19] = new SqlParameter("@MetaKeywords", salon.MetaKeywords);
            parameterArray2[0x1a] = new SqlParameter("@MetaTitle", salon.MetaTitle);
            parameterArray2[0x1b] = new SqlParameter("@Mobile", salon.Mobile);
            parameterArray2[0x1c] = new SqlParameter("@Name", salon.Name);
            parameterArray2[0x1d] = new SqlParameter("@PhoneNumber", salon.PhoneNumber);
            Guid? pictureId = salon.PictureId;
            parameterArray2[30] = new SqlParameter("@PictureId", pictureId.HasValue ? ((SqlGuid) pictureId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[0x1f] = new SqlParameter("@Published", salon.Published);
            parameterArray2[0x20] = new SqlParameter("@CurrencyCode", salon.CurrencyCode);
            parameterArray2[0x21] = new SqlParameter("@RealexPayerRef", salon.RealexPayerRef);
            parameterArray2[0x22] = new SqlParameter("@SEName", salon.SEName);
            parameterArray2[0x23] = new SqlParameter("@ShortDescription", salon.ShortDescription);
            parameterArray2[0x24] = new SqlParameter("@ShowOnWeb", salon.ShowOnWeb);
            parameterArray2[0x25] = new SqlParameter("@ShowOnWidget", salon.ShowOnWidget);
            parameterArray2[0x26] = new SqlParameter("@ShowOnMobile", salon.ShowOnMobile);
            parameterArray2[0x27] = new SqlParameter("@StateProvince", salon.StateProvince);
            parameterArray2[40] = new SqlParameter("@TotalTicketCount", salon.TotalTicketCount);
            parameterArray2[0x29] = new SqlParameter("@TotalRatingSum", salon.TotalRatingSum);
            parameterArray2[0x2a] = new SqlParameter("@TotalRatingVotes", salon.TotalRatingVotes);
            parameterArray2[0x2b] = new SqlParameter("@TotalReviews", salon.TotalReviews);
            parameterArray2[0x2c] = new SqlParameter("@VatNumber", salon.VatNumber);
            parameterArray2[0x2d] = new SqlParameter("@VatNumberStatus", salon.VatNumberStatus);
            parameterArray2[0x2e] = new SqlParameter("@Website", salon.Website);
            parameterArray2[0x2f] = new SqlParameter("@ZipPostalCode", salon.ZipPostalCode);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid salonId = new Guid(parameter.Value.ToString());
            salon = this.GetSalonById(salonId);
            return salon;
        }

        public SalonPaymentMethodDB InsertSalonPaymentMethod(SalonPaymentMethodDB paymentMethod)
        {
            if (paymentMethod == null)
            {
                throw new ArgumentNullException("paymentMethod");
            }
            if (paymentMethod.RealexPayerRef == null)
            {
                throw new ArgumentException("Realex player reference cannot be null.");
            }
            if (paymentMethod.RealexPayerRef == string.Empty)
            {
                throw new ArgumentException("Realex player reference cannot be empty.");
            }
            if (paymentMethod.RealexCardRef == null)
            {
                throw new ArgumentException("Realex card reference cannot be null.");
            }
            if (paymentMethod.RealexCardRef == string.Empty)
            {
                throw new ArgumentException("Realex card reference cannot be empty.");
            }
            if (paymentMethod.SalonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            paymentMethod.Alias = paymentMethod.Alias ?? string.Empty;
            paymentMethod.CardCvv2 = paymentMethod.CardCvv2 ?? string.Empty;
            paymentMethod.CardExpirationMonth = paymentMethod.CardExpirationMonth ?? string.Empty;
            paymentMethod.CardExpirationYear = paymentMethod.CardExpirationYear ?? string.Empty;
            paymentMethod.CardName = paymentMethod.CardName ?? string.Empty;
            paymentMethod.CardNumber = paymentMethod.CardNumber ?? string.Empty;
            paymentMethod.CardType = paymentMethod.CardType ?? string.Empty;
            paymentMethod.CreatedBy = paymentMethod.CreatedBy ?? string.Empty;
            paymentMethod.MaskedCardNumber = paymentMethod.MaskedCardNumber ?? string.Empty;
            paymentMethod.RealexPayerRef = paymentMethod.RealexPayerRef ?? string.Empty;
            paymentMethod.RealexCardRef = paymentMethod.RealexCardRef ?? string.Empty;
            paymentMethod.UpdatedBy = paymentMethod.UpdatedBy ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@SalonPaymentMethodId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { 
                parameter, new SqlParameter("@RealexCardRef", paymentMethod.RealexCardRef), new SqlParameter("@RealexPayerRef", paymentMethod.RealexPayerRef), new SqlParameter("@SalonId", paymentMethod.SalonId), new SqlParameter("@Alias", paymentMethod.Alias), new SqlParameter("@CardType", paymentMethod.CardType), new SqlParameter("@CardName", paymentMethod.CardName), new SqlParameter("@CardNumber", paymentMethod.CardNumber), new SqlParameter("@MaskedCardNumber", paymentMethod.MaskedCardNumber), new SqlParameter("@CardCvv2", paymentMethod.CardCvv2), new SqlParameter("@CardExpirationMonth", paymentMethod.CardExpirationMonth), new SqlParameter("@CardExpirationYear", paymentMethod.CardExpirationYear), new SqlParameter("@IsPrimary", paymentMethod.IsPrimary), new SqlParameter("@Active", paymentMethod.Active), new SqlParameter("@CreatedOn", paymentMethod.CreatedOn), new SqlParameter("@CreatedBy", paymentMethod.CreatedBy),
                new SqlParameter("@UpdatedOn", paymentMethod.UpdatedOn), new SqlParameter("@UpdatedBy", paymentMethod.UpdatedBy)
            };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonPaymentMethodInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid paymentMethodId = new Guid(parameter.Value.ToString());
            paymentMethod = this.GetSalonPaymentMethodById(paymentMethodId);
            return paymentMethod;
        }

        public SalonRatingDB InsertSalonRating(SalonRatingDB rating)
        {
            if (rating == null)
            {
                throw new ArgumentNullException("rating");
            }
            if (rating.SalonId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("rating.SalonId", "SalonId cannot be empty.");
            }
            if (rating.EmployeeOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.EmployeeOverallRating", "EmployeeOverallRating cannot be less than zero.");
            }
            if (rating.EmployeeOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.EmployeeOverallSum", "EmployeeOverallSum cannot be less than zero.");
            }
            if (rating.EmployeeOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.EmployeeOverallTotal", "EmployeeOverallTotal cannot be less than zero.");
            }
            if (rating.GoAgainOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.GoAgainOverallRating", "GoAgainOverallRating cannot be less than zero.");
            }
            if (rating.GoAgainOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.GoAgainOverallSum", "GoAgainOverallSum cannot be less than zero.");
            }
            if (rating.GoAgainOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.GoAgainOverallTotal", "GoAgainOverallTotal cannot be less than zero.");
            }
            if (rating.SalonOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SalonOverallRating", "SalonOverallRating cannot be less than zero.");
            }
            if (rating.SalonOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SalonOverallSum", "SalonOverallSum cannot be less than zero.");
            }
            if (rating.SalonOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SalonOverallTotal", "SalonOverallTotal cannot be less than zero.");
            }
            if (rating.SatisfactionOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SatisfactionOverallRating", "SatisfactionOverallRating cannot be less than zero.");
            }
            if (rating.SatisfactionOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SatisfactionOverallSum", "SatisfactionOverallSum cannot be less than zero.");
            }
            if (rating.SatisfactionOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SatisfactionOverallTotal", "SatisfactionOverallTotal cannot be less than zero.");
            }
            if (rating.ServiceOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ServiceOverallRating", "ServiceOverallRating cannot be less than zero.");
            }
            if (rating.ServiceOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ServiceOverallSum", "ServiceOverallSum cannot be less than zero.");
            }
            if (rating.ServiceOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ServiceOverallTotal", "ServiceOverallTotal cannot be less than zero.");
            }
            if (rating.ValueOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ValueOverallRating", "ValueOverallRating cannot be less than zero.");
            }
            if (rating.ValueOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ValueOverallSum", "ValueOverallSum cannot be less than zero.");
            }
            if (rating.ValueOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ValueOverallTotal", "ValueOverallTotal cannot be less than zero.");
            }
            if (rating.WeightedOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.WeightedOverallRating", "WeightedOverallRating cannot be less than zero.");
            }
            if (rating.WeightedOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.WeightedOverallSum", "WeightedOverallSum cannot be less than zero.");
            }
            if (rating.WeightedOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.WeightedOverallTotal", "WeightedOverallTotal cannot be less than zero.");
            }
            if (rating.ReviewCount < 0)
            {
                throw new ArgumentOutOfRangeException("rating.ReviewCount", "ReviewCount cannot be less than zero.");
            }
            if (!string.IsNullOrEmpty(rating.LastReviewerDate))
            {
                if (!Regex.IsMatch(rating.LastReviewerDate, @"(\d{4})-(\d{2})-(\d{2})"))
                {
                    throw new ArgumentException("LastReviewerDate is an invalid date.");
                }
                try
                {
                    DateTime.ParseExact(rating.LastReviewerDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("LastReviewerDate is an invalid date.");
                }
            }
            if (!string.IsNullOrEmpty(rating.LastReviewerTime))
            {
                if (!Regex.IsMatch(rating.LastReviewerTime, @"(\d{2}):(\d{2})"))
                {
                    throw new ArgumentException("LastReviewerTime is an invalid time.");
                }
                try
                {
                    DateTime.ParseExact(rating.LastReviewerTime, "HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("LastReviewerTime is an invalid time.");
                }
            }
            rating.LastReviewerDate = rating.LastReviewerDate ?? string.Empty;
            rating.LastReviewerTime = rating.LastReviewerTime ?? string.Empty;
            rating.LastReviewerDescription = rating.LastReviewerDescription ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@SalonRatingId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { 
                parameter, new SqlParameter("@SalonId", rating.SalonId), new SqlParameter("@SalonOverallSum", rating.SalonOverallSum), new SqlParameter("@SalonOverallTotal", rating.SalonOverallTotal), new SqlParameter("@SalonOverallRating", rating.SalonOverallRating), new SqlParameter("@EmployeeOverallSum", rating.EmployeeOverallSum), new SqlParameter("@EmployeeOverallTotal", rating.EmployeeOverallTotal), new SqlParameter("@EmployeeOverallRating", rating.EmployeeOverallRating), new SqlParameter("@ServiceOverallSum", rating.ServiceOverallSum), new SqlParameter("@ServiceOverallTotal", rating.ServiceOverallTotal), new SqlParameter("@ServiceOverallRating", rating.ServiceOverallRating), new SqlParameter("@SatisfactionOverallSum", rating.SatisfactionOverallSum), new SqlParameter("@SatisfactionOverallTotal", rating.SatisfactionOverallTotal), new SqlParameter("@SatisfactionOverallRating", rating.SatisfactionOverallRating), new SqlParameter("@ValueOverallSum", rating.ValueOverallSum), new SqlParameter("@ValueOverallTotal", rating.ValueOverallTotal),
                new SqlParameter("@ValueOverallRating", rating.ValueOverallRating), new SqlParameter("@GoAgainOverallSum", rating.GoAgainOverallSum), new SqlParameter("@GoAgainOverallTotal", rating.GoAgainOverallTotal), new SqlParameter("@GoAgainOverallRating", rating.GoAgainOverallRating), new SqlParameter("@WeightedOverallSum", rating.WeightedOverallSum), new SqlParameter("@WeightedOverallTotal", rating.WeightedOverallTotal), new SqlParameter("@WeightedOverallRating", rating.WeightedOverallRating), new SqlParameter("@ReviewCount", rating.ReviewCount), new SqlParameter("@LastReviewerDate", rating.LastReviewerDate), new SqlParameter("@LastReviewerDescription", rating.LastReviewerDescription), new SqlParameter("@LastReviewerTime", rating.LastReviewerTime)
            };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonRatingInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid guid = new Guid(parameter.Value.ToString());
            rating = this.GetSalonRatingById(guid);
            return rating;
        }

        public SalonReviewDB InsertSalonReview(SalonReviewDB review)
        {
            if (review == null)
            {
                throw new ArgumentNullException("review");
            }
            if (review.AppointmentId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.AppointmentId", "AppointmentId cannot be empty.");
            }
            if (review.EmployeeId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.EmployeeId", "EmployeeId cannot be empty.");
            }
            if (review.EmployeeRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.EmployeeRating", "EmployeeRating cannot be less than zero.");
            }
            if (review.EmployeeRatingSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.EmployeeRatingSum", "EmployeeRatingSum cannot be less than zero.");
            }
            if (review.EmployeeRatingTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.EmployeeRatingTotal", "EmployeeRatingTotal cannot be less than zero.");
            }
            if (review.SalonId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.SalonId", "SalonId cannot be empty.");
            }
            if (review.SalonRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SalonRating", "SalonRating cannot be less than zero.");
            }
            if (review.SalonRatingSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SalonRatingSum", "SalonRatingSum cannot be less than zero.");
            }
            if (review.SalonRatingTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SalonRatingTotal", "SalonRatingTotal cannot be less than zero.");
            }
            if (review.SatisfactionRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SatisfactionRating", "SatisfactionRating cannot be less than zero.");
            }
            if (review.SatisfactionRatingSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SatisfactionRatingSum", "SatisfactionRatingSum cannot be less than zero.");
            }
            if (review.SatisfactionRatingTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SatisfactionRatingTotal", "SatisfactionRatingTotal cannot be less than zero.");
            }
            if (review.ServiceId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.ServiceId", "ServiceId cannot be empty.");
            }
            if (review.ServiceRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ServiceRating", "ServiceRating cannot be less than zero.");
            }
            if (review.ServiceRatingSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ServiceRatingSum", "ServiceRatingSum cannot be less than zero.");
            }
            if (review.ServiceRatingTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ServiceRatingTotal", "ServiceRatingTotal cannot be less than zero.");
            }
            if (review.ValueRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ValueRating", "ValueRating cannot be less than zero.");
            }
            if (review.ValueRatingSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ValueRatingSum", "ValueRatingSum cannot be less than zero.");
            }
            if (review.ValueRatingTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ValueRatingTotal", "ValueRatingTotal cannot be less than zero.");
            }
            if (review.WeightedRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.WeightedRating", "WeightedRating cannot be less than zero.");
            }
            if (review.WeightedSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.WeightedSum", "WeightedSum cannot be less than zero.");
            }
            if (review.WeightedTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.WeightedTotal", "WeightedTotal cannot be less than zero.");
            }
            if (review.UserId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.UserId", "UserId cannot be empty.");
            }
            if (!string.IsNullOrEmpty(review.AppointmentDate))
            {
                if (!Regex.IsMatch(review.AppointmentDate, @"(\d{4})-(\d{2})-(\d{2})"))
                {
                    throw new ArgumentException("AppointmentDate is an invalid date.");
                }
                try
                {
                    DateTime.ParseExact(review.AppointmentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("AppointmentDate is an invalid date.");
                }
            }
            if (!string.IsNullOrEmpty(review.AppointmentTime))
            {
                if (!Regex.IsMatch(review.AppointmentTime, @"(\d{2}):(\d{2})"))
                {
                    throw new ArgumentException("AppointmentTime is an invalid time.");
                }
                try
                {
                    DateTime.ParseExact(review.AppointmentTime, "HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("AppointmentTime is an invalid time.");
                }
            }
            if (!string.IsNullOrEmpty(review.ReviewerDate))
            {
                if (!Regex.IsMatch(review.ReviewerDate, @"(\d{4})-(\d{2})-(\d{2})"))
                {
                    throw new ArgumentException("ReviewerDate is an invalid date.");
                }
                try
                {
                    DateTime.ParseExact(review.ReviewerDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("ReviewerDate is an invalid date.");
                }
            }
            if (!string.IsNullOrEmpty(review.ReviewerTime))
            {
                if (!Regex.IsMatch(review.ReviewerTime, @"(\d{2}):(\d{2})"))
                {
                    throw new ArgumentException("ReviewerTime is an invalid time.");
                }
                try
                {
                    DateTime.ParseExact(review.ReviewerTime, "HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("ReviewerTime is an invalid time.");
                }
            }
            if (!string.IsNullOrEmpty(review.ApproverDate))
            {
                if (!Regex.IsMatch(review.ApproverDate, @"(\d{4})-(\d{2})-(\d{2})"))
                {
                    throw new ArgumentException("ApproverDate is an invalid date.");
                }
                try
                {
                    DateTime.ParseExact(review.ApproverDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("ApproverDate is an invalid date.");
                }
            }
            if (!string.IsNullOrEmpty(review.ApproverTime))
            {
                if (!Regex.IsMatch(review.ApproverTime, @"(\d{2}):(\d{2})"))
                {
                    throw new ArgumentException("ApproverTime is an invalid time.");
                }
                try
                {
                    DateTime.ParseExact(review.ApproverTime, "HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("ApproverTime is an invalid time.");
                }
            }
            review.ApproverDescription = review.ApproverDescription ?? string.Empty;
            review.Comments = review.Comments ?? string.Empty;
            review.CreatedBy = review.CreatedBy ?? string.Empty;
            review.EmployeeDescription = review.EmployeeDescription ?? string.Empty;
            review.ReviewerDescription = review.ReviewerDescription ?? string.Empty;
            review.SalonDescription = review.SalonDescription ?? string.Empty;
            review.ServiceDescription = review.ServiceDescription ?? string.Empty;
            review.UpdatedBy = review.UpdatedBy ?? string.Empty;
            review.UserDescription = review.UserDescription ?? string.Empty;
            review.ApproverDate = review.ApproverDate ?? string.Empty;
            review.ApproverTime = review.ApproverTime ?? string.Empty;
            review.AppointmentDate = review.AppointmentDate ?? string.Empty;
            review.AppointmentTime = review.AppointmentTime ?? string.Empty;
            review.ReviewerDate = review.ReviewerDate ?? string.Empty;
            review.ReviewerTime = review.ReviewerTime ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@SalonReviewId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[0x2a];
            parameterArray2[0] = parameter;
            parameterArray2[1] = new SqlParameter("@AppointmentDate", review.AppointmentDate);
            parameterArray2[2] = new SqlParameter("@AppointmentId", review.AppointmentId ?? SqlGuid.Null);
            parameterArray2[3] = new SqlParameter("@AppointmentTime", review.AppointmentTime);
            parameterArray2[4] = new SqlParameter("@ApproverDate", review.ApproverDate);
            parameterArray2[5] = new SqlParameter("@ApproverDescription", review.ApproverDescription);
            parameterArray2[6] = new SqlParameter("@ApproverTime", review.ApproverTime);
            parameterArray2[7] = new SqlParameter("@Comments", review.Comments);
            parameterArray2[8] = new SqlParameter("@CreatedBy", review.CreatedBy);
            parameterArray2[9] = new SqlParameter("@CreatedOn", review.CreatedOn);
            parameterArray2[10] = new SqlParameter("@EmployeeDescription", review.EmployeeDescription);
            Guid? employeeId = review.EmployeeId;
            parameterArray2[11] = new SqlParameter("@EmployeeId", employeeId.HasValue ? ((SqlGuid) employeeId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[12] = new SqlParameter("@EmployeeRating", review.EmployeeRating);
            parameterArray2[13] = new SqlParameter("@EmployeeRatingSum", review.EmployeeRatingSum);
            parameterArray2[14] = new SqlParameter("@EmployeeRatingTotal", review.EmployeeRatingTotal);
            parameterArray2[15] = new SqlParameter("@GoAgain", review.GoAgain);
            parameterArray2[0x10] = new SqlParameter("@ReviewerDate", review.ReviewerDate);
            parameterArray2[0x11] = new SqlParameter("@ReviewerDescription", review.ReviewerDescription);
            parameterArray2[0x12] = new SqlParameter("@ReviewerTime", review.ReviewerTime);
            parameterArray2[0x13] = new SqlParameter("@SalonDescription", review.SalonDescription);
            parameterArray2[20] = new SqlParameter("@SalonId", review.SalonId);
            parameterArray2[0x15] = new SqlParameter("@SalonRating", review.SalonRating);
            parameterArray2[0x16] = new SqlParameter("@SalonRatingSum", review.SalonRatingSum);
            parameterArray2[0x17] = new SqlParameter("@SalonRatingTotal", review.SalonRatingTotal);
            parameterArray2[0x18] = new SqlParameter("@SatisfactionRating", review.SatisfactionRating);
            parameterArray2[0x19] = new SqlParameter("@SatisfactionRatingSum", review.SatisfactionRatingSum);
            parameterArray2[0x1a] = new SqlParameter("@SatisfactionRatingTotal", review.SatisfactionRatingTotal);
            parameterArray2[0x1b] = new SqlParameter("@ServiceDescription", review.ServiceDescription);
            parameterArray2[0x1c] = new SqlParameter("@ServiceId", review.ServiceId ?? SqlGuid.Null);
            parameterArray2[0x1d] = new SqlParameter("@ServiceRating", review.ServiceRating);
            parameterArray2[30] = new SqlParameter("@ServiceRatingSum", review.ServiceRatingSum);
            parameterArray2[0x1f] = new SqlParameter("@ServiceRatingTotal", review.ServiceRatingTotal);
            parameterArray2[0x20] = new SqlParameter("@UpdatedBy", review.UpdatedBy);
            parameterArray2[0x21] = new SqlParameter("@UpdatedOn", review.UpdatedOn);
            parameterArray2[0x22] = new SqlParameter("@UserDescription", review.UserDescription);
            parameterArray2[0x23] = new SqlParameter("@WeightedSum", review.WeightedSum);
            parameterArray2[0x24] = new SqlParameter("@WeightedTotal", review.WeightedTotal);
            parameterArray2[0x25] = new SqlParameter("@WeightedRating", review.WeightedRating);
            parameterArray2[0x26] = new SqlParameter("@UserId", review.UserId ?? SqlGuid.Null);
            parameterArray2[0x27] = new SqlParameter("@ValueRating", review.ValueRating);
            parameterArray2[40] = new SqlParameter("@ValueRatingSum", review.ValueRatingSum);
            parameterArray2[0x29] = new SqlParameter("@ValueRatingTotal", review.ValueRatingTotal);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonReviewInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid guid = new Guid(parameter.Value.ToString());
            review = this.GetSalonReviewById(guid);
            return review;
        }

        public OpeningHoursDB OpeningHoursMapping(SqlDataReader reader) => 
            new OpeningHoursDB { 
                FriClosed = reader.GetBoolean(reader.GetOrdinal("FriClosed")),
                FriEnd1 = reader.GetValue(reader.GetOrdinal("FriEnd1")) as TimeSpan?,
                FriEnd2 = reader.GetValue(reader.GetOrdinal("FriEnd2")) as TimeSpan?,
                FriStart1 = reader.GetValue(reader.GetOrdinal("FriStart1")) as TimeSpan?,
                FriStart2 = reader.GetValue(reader.GetOrdinal("FriStart2")) as TimeSpan?,
                MonClosed = reader.GetBoolean(reader.GetOrdinal("MonClosed")),
                MonEnd1 = reader.GetValue(reader.GetOrdinal("MonEnd1")) as TimeSpan?,
                MonEnd2 = reader.GetValue(reader.GetOrdinal("MonEnd2")) as TimeSpan?,
                MonStart1 = reader.GetValue(reader.GetOrdinal("MonStart1")) as TimeSpan?,
                MonStart2 = reader.GetValue(reader.GetOrdinal("MonStart2")) as TimeSpan?,
                OpeningHoursId = reader.GetGuid(reader.GetOrdinal("OpeningHoursId")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                SatClosed = reader.GetBoolean(reader.GetOrdinal("SatClosed")),
                SatEnd1 = reader.GetValue(reader.GetOrdinal("SatEnd1")) as TimeSpan?,
                SatEnd2 = reader.GetValue(reader.GetOrdinal("SatEnd2")) as TimeSpan?,
                SatStart1 = reader.GetValue(reader.GetOrdinal("SatStart1")) as TimeSpan?,
                SatStart2 = reader.GetValue(reader.GetOrdinal("SatStart2")) as TimeSpan?,
                ShowOnWeb = reader.GetBoolean(reader.GetOrdinal("ShowOnWeb")),
                ShowOnMobile = reader.GetBoolean(reader.GetOrdinal("ShowOnMobile")),
                ShowOnWidget = reader.GetBoolean(reader.GetOrdinal("ShowOnWidget")),
                SunClosed = reader.GetBoolean(reader.GetOrdinal("SunClosed")),
                SunEnd1 = reader.GetValue(reader.GetOrdinal("SunEnd1")) as TimeSpan?,
                SunEnd2 = reader.GetValue(reader.GetOrdinal("SunEnd2")) as TimeSpan?,
                SunStart1 = reader.GetValue(reader.GetOrdinal("SunStart1")) as TimeSpan?,
                SunStart2 = reader.GetValue(reader.GetOrdinal("SunStart2")) as TimeSpan?,
                ThuClosed = reader.GetBoolean(reader.GetOrdinal("ThuClosed")),
                ThuEnd1 = reader.GetValue(reader.GetOrdinal("ThuEnd1")) as TimeSpan?,
                ThuEnd2 = reader.GetValue(reader.GetOrdinal("ThuEnd2")) as TimeSpan?,
                ThuStart1 = reader.GetValue(reader.GetOrdinal("ThuStart1")) as TimeSpan?,
                ThuStart2 = reader.GetValue(reader.GetOrdinal("ThuStart2")) as TimeSpan?,
                TueClosed = reader.GetBoolean(reader.GetOrdinal("TueClosed")),
                TueEnd1 = reader.GetValue(reader.GetOrdinal("TueEnd1")) as TimeSpan?,
                TueEnd2 = reader.GetValue(reader.GetOrdinal("TueEnd2")) as TimeSpan?,
                TueStart1 = reader.GetValue(reader.GetOrdinal("TueStart1")) as TimeSpan?,
                TueStart2 = reader.GetValue(reader.GetOrdinal("TueStart2")) as TimeSpan?,
                WedClosed = reader.GetBoolean(reader.GetOrdinal("WedClosed")),
                WedEnd1 = reader.GetValue(reader.GetOrdinal("WedEnd1")) as TimeSpan?,
                WedEnd2 = reader.GetValue(reader.GetOrdinal("WedEnd2")) as TimeSpan?,
                WedStart1 = reader.GetValue(reader.GetOrdinal("WedStart1")) as TimeSpan?,
                WedStart2 = reader.GetValue(reader.GetOrdinal("WedStart2")) as TimeSpan?
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
                CurrencyCode = reader.GetString(reader.GetOrdinal("CurrencyCode")),
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

        private SalonPaymentMethodDB SalonPaymentMethodMapping(SqlDataReader reader) => 
            new SalonPaymentMethodDB { 
                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                Alias = reader.GetString(reader.GetOrdinal("Alias")),
                CardCvv2 = reader.GetString(reader.GetOrdinal("CardCvv2")),
                CardExpirationMonth = reader.GetString(reader.GetOrdinal("CardExpirationMonth")),
                CardExpirationYear = reader.GetString(reader.GetOrdinal("CardExpirationYear")),
                CardName = reader.GetString(reader.GetOrdinal("CardName")),
                CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                CardType = reader.GetString(reader.GetOrdinal("CardType")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                IsPrimary = reader.GetBoolean(reader.GetOrdinal("IsPrimary")),
                MaskedCardNumber = reader.GetString(reader.GetOrdinal("MaskedCardNumber")),
                RealexPayerRef = reader.GetString(reader.GetOrdinal("RealexPayerRef")),
                RealexCardRef = reader.GetString(reader.GetOrdinal("RealexCardRef")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                SalonPaymentMethodId = reader.GetGuid(reader.GetOrdinal("SalonPaymentMethodId")),
                UpdatedBy = reader.GetString(reader.GetOrdinal("UpdatedBy")),
                UpdatedOn = reader.GetDateTime(reader.GetOrdinal("UpdatedOn"))
            };

        private SalonRatingDB SalonRatingMapping(SqlDataReader reader) => 
            new SalonRatingDB { 
                EmployeeOverallRating = (float) reader.GetDouble(reader.GetOrdinal("EmployeeOverallRating")),
                EmployeeOverallSum = (float) reader.GetDouble(reader.GetOrdinal("EmployeeOverallSum")),
                EmployeeOverallTotal = (float) reader.GetDouble(reader.GetOrdinal("EmployeeOverallTotal")),
                GoAgainOverallRating = (float) reader.GetDouble(reader.GetOrdinal("GoAgainOverallRating")),
                GoAgainOverallSum = (float) reader.GetDouble(reader.GetOrdinal("GoAgainOverallSum")),
                GoAgainOverallTotal = (float) reader.GetDouble(reader.GetOrdinal("GoAgainOverallTotal")),
                LastReviewerDate = reader.GetString(reader.GetOrdinal("LastReviewerDate")),
                LastReviewerDescription = reader.GetString(reader.GetOrdinal("LastReviewerDescription")),
                LastReviewerTime = reader.GetString(reader.GetOrdinal("LastReviewerTime")),
                ReviewCount = reader.GetInt32(reader.GetOrdinal("ReviewCount")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                SalonOverallRating = (float) reader.GetDouble(reader.GetOrdinal("SalonOverallRating")),
                SalonOverallSum = (float) reader.GetDouble(reader.GetOrdinal("SalonOverallSum")),
                SalonOverallTotal = (float) reader.GetDouble(reader.GetOrdinal("SalonOverallTotal")),
                SalonRatingId = reader.GetGuid(reader.GetOrdinal("SalonRatingId")),
                SatisfactionOverallRating = (float) reader.GetDouble(reader.GetOrdinal("SatisfactionOverallRating")),
                SatisfactionOverallSum = (float) reader.GetDouble(reader.GetOrdinal("SatisfactionOverallSum")),
                SatisfactionOverallTotal = (float) reader.GetDouble(reader.GetOrdinal("SatisfactionOverallTotal")),
                ServiceOverallRating = (float) reader.GetDouble(reader.GetOrdinal("ServiceOverallRating")),
                ServiceOverallSum = (float) reader.GetDouble(reader.GetOrdinal("ServiceOverallSum")),
                ServiceOverallTotal = (float) reader.GetDouble(reader.GetOrdinal("ServiceOverallTotal")),
                ValueOverallRating = (float) reader.GetDouble(reader.GetOrdinal("ValueOverallRating")),
                ValueOverallSum = (float) reader.GetDouble(reader.GetOrdinal("ValueOverallSum")),
                ValueOverallTotal = (float) reader.GetDouble(reader.GetOrdinal("ValueOverallTotal")),
                WeightedOverallRating = (float) reader.GetDouble(reader.GetOrdinal("WeightedOverallRating")),
                WeightedOverallSum = (float) reader.GetDouble(reader.GetOrdinal("WeightedOverallSum")),
                WeightedOverallTotal = (float) reader.GetDouble(reader.GetOrdinal("WeightedOverallTotal"))
            };

        private SalonReviewDB SalonReviewMapping(SqlDataReader reader) => 
            new SalonReviewDB { 
                ApproverDate = reader.GetString(reader.GetOrdinal("ApproverDate")),
                ApproverDescription = reader.GetString(reader.GetOrdinal("ApproverDescription")),
                ApproverTime = reader.GetString(reader.GetOrdinal("ApproverTime")),
                AppointmentDate = reader.GetString(reader.GetOrdinal("AppointmentDate")),
                AppointmentId = reader.GetValue(reader.GetOrdinal("AppointmentId")) as Guid?,
                AppointmentTime = reader.GetString(reader.GetOrdinal("AppointmentTime")),
                Comments = reader.GetString(reader.GetOrdinal("Comments")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                EmployeeDescription = reader.GetString(reader.GetOrdinal("EmployeeDescription")),
                EmployeeId = reader.GetValue(reader.GetOrdinal("EmployeeId")) as Guid?,
                EmployeeRating = (float) reader.GetDouble(reader.GetOrdinal("EmployeeRating")),
                EmployeeRatingSum = (float) reader.GetDouble(reader.GetOrdinal("EmployeeRatingSum")),
                EmployeeRatingTotal = (float) reader.GetDouble(reader.GetOrdinal("EmployeeRatingTotal")),
                GoAgain = reader.GetBoolean(reader.GetOrdinal("GoAgain")),
                ReviewerDate = reader.GetString(reader.GetOrdinal("ReviewerDate")),
                ReviewerDescription = reader.GetString(reader.GetOrdinal("ReviewerDescription")),
                ReviewerTime = reader.GetString(reader.GetOrdinal("ReviewerTime")),
                SalonDescription = reader.GetString(reader.GetOrdinal("SalonDescription")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                SalonRating = (float) reader.GetDouble(reader.GetOrdinal("SalonRating")),
                SalonRatingSum = (float) reader.GetDouble(reader.GetOrdinal("SalonRatingSum")),
                SalonRatingTotal = (float) reader.GetDouble(reader.GetOrdinal("SalonRatingTotal")),
                SalonReviewId = reader.GetGuid(reader.GetOrdinal("SalonReviewId")),
                SatisfactionRating = (float) reader.GetDouble(reader.GetOrdinal("SatisfactionRating")),
                SatisfactionRatingSum = (float) reader.GetDouble(reader.GetOrdinal("SatisfactionRatingSum")),
                SatisfactionRatingTotal = (float) reader.GetDouble(reader.GetOrdinal("SatisfactionRatingTotal")),
                ServiceDescription = reader.GetString(reader.GetOrdinal("ServiceDescription")),
                ServiceId = reader.GetValue(reader.GetOrdinal("ServiceId")) as Guid?,
                ServiceRating = (float) reader.GetDouble(reader.GetOrdinal("ServiceRating")),
                ServiceRatingSum = (float) reader.GetDouble(reader.GetOrdinal("ServiceRatingSum")),
                ServiceRatingTotal = (float) reader.GetDouble(reader.GetOrdinal("ServiceRatingTotal")),
                UpdatedBy = reader.GetString(reader.GetOrdinal("UpdatedBy")),
                UpdatedOn = reader.GetDateTime(reader.GetOrdinal("UpdatedOn")),
                UserDescription = reader.GetString(reader.GetOrdinal("UserDescription")),
                UserId = reader.GetValue(reader.GetOrdinal("UserId")) as Guid?,
                ValueRating = (float) reader.GetDouble(reader.GetOrdinal("ValueRating")),
                ValueRatingSum = (float) reader.GetDouble(reader.GetOrdinal("ValueRatingSum")),
                ValueRatingTotal = (float) reader.GetDouble(reader.GetOrdinal("ValueRatingTotal"))
            };

        public ClosingDayDB UpdateClosingDay(ClosingDayDB closingDay)
        {
            if (closingDay == null)
            {
                throw new ArgumentNullException("closingDay");
            }
            closingDay.Category = closingDay.Category ?? string.Empty;
            closingDay.Description = closingDay.Description ?? string.Empty;
            SqlParameter[] parameterArray2 = new SqlParameter[9];
            parameterArray2[0] = new SqlParameter("@ClosingDayId", closingDay.ClosingDayId);
            parameterArray2[1] = new SqlParameter("@Active", closingDay.Active);
            parameterArray2[2] = new SqlParameter("@Category", closingDay.Category);
            parameterArray2[3] = new SqlParameter("@CycleLength", closingDay.CycleLength);
            parameterArray2[4] = new SqlParameter("@CyclePeriodType", closingDay.CyclePeriodType);
            parameterArray2[5] = new SqlParameter("@Description", closingDay.Description);
            parameterArray2[6] = new SqlParameter("@SalonId", closingDay.SalonId);
            parameterArray2[7] = new SqlParameter("@StartDateUtc", closingDay.StartDateUtc.Date);
            int? totalCycles = closingDay.TotalCycles;
            parameterArray2[8] = new SqlParameter("@TotalCycles", totalCycles.HasValue ? ((SqlInt32) totalCycles.GetValueOrDefault()) : SqlInt32.Null);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ClosingDayUpdate", commandParameters) <= 0)
            {
                return null;
            }
            string key = $"CLOSING-DAY-{"SALON-" + closingDay.SalonId}";
            this._cacheManager.Remove(key);
            closingDay = this.GetClosingDayById(closingDay.ClosingDayId);
            return closingDay;
        }

        public OpeningHoursDB UpdateOpeningHours(OpeningHoursDB hours)
        {
            if (hours == null)
            {
                throw new ArgumentNullException("hours");
            }
            if (hours.OpeningHoursId == Guid.Empty)
            {
                throw new ArgumentException("Identifier cannot be empty.");
            }
            if (hours.SunClosed)
            {
                hours.SunStart1 = null;
                hours.SunEnd1 = null;
                hours.SunStart2 = null;
                hours.SunEnd2 = null;
            }
            if (hours.MonClosed)
            {
                hours.MonStart1 = null;
                hours.MonEnd1 = null;
                hours.MonStart2 = null;
                hours.MonEnd2 = null;
            }
            if (hours.TueClosed)
            {
                hours.TueStart1 = null;
                hours.TueEnd1 = null;
                hours.TueStart2 = null;
                hours.TueEnd2 = null;
            }
            if (hours.WedClosed)
            {
                hours.WedStart1 = null;
                hours.WedEnd1 = null;
                hours.WedStart2 = null;
                hours.WedEnd2 = null;
            }
            if (hours.ThuClosed)
            {
                hours.ThuStart1 = null;
                hours.ThuEnd1 = null;
                hours.ThuStart2 = null;
                hours.ThuEnd2 = null;
            }
            if (hours.FriClosed)
            {
                hours.FriStart1 = null;
                hours.FriEnd1 = null;
                hours.FriStart2 = null;
                hours.FriEnd2 = null;
            }
            if (hours.SatClosed)
            {
                hours.SatStart1 = null;
                hours.SatEnd1 = null;
                hours.SatStart2 = null;
                hours.SatEnd2 = null;
            }
            SqlParameter[] commandParameters = new SqlParameter[] { 
                new SqlParameter("@OpeningHoursId", hours.OpeningHoursId), new SqlParameter("@FriClosed", hours.FriClosed), new SqlParameter("@FriEnd1", hours.FriEnd1 ?? DBNull.Value), new SqlParameter("@FriEnd2", hours.FriEnd2 ?? DBNull.Value), new SqlParameter("@FriStart1", hours.FriStart1 ?? DBNull.Value), new SqlParameter("@FriStart2", hours.FriStart2 ?? DBNull.Value), new SqlParameter("@MonClosed", hours.MonClosed), new SqlParameter("@MonEnd1", hours.MonEnd1 ?? DBNull.Value), new SqlParameter("@MonEnd2", hours.MonEnd2 ?? DBNull.Value), new SqlParameter("@MonStart1", hours.MonStart1 ?? DBNull.Value), new SqlParameter("@MonStart2", hours.MonStart2 ?? DBNull.Value), new SqlParameter("@SalonId", hours.SalonId), new SqlParameter("@SatClosed", hours.SatClosed), new SqlParameter("@SatEnd1", hours.SatEnd1 ?? DBNull.Value), new SqlParameter("@SatEnd2", hours.SatEnd2 ?? DBNull.Value), new SqlParameter("@SatStart1", hours.SatStart1 ?? DBNull.Value),
                new SqlParameter("@SatStart2", hours.SatStart2 ?? DBNull.Value), new SqlParameter("@ShowOnWeb", hours.ShowOnWeb), new SqlParameter("@ShowOnMobile", hours.ShowOnMobile), new SqlParameter("@ShowOnWidget", hours.ShowOnWidget), new SqlParameter("@SunClosed", hours.SunClosed), new SqlParameter("@SunEnd1", hours.SunEnd1 ?? DBNull.Value), new SqlParameter("@SunEnd2", hours.SunEnd2 ?? DBNull.Value), new SqlParameter("@SunStart1", hours.SunStart1 ?? DBNull.Value), new SqlParameter("@SunStart2", hours.SunStart2 ?? DBNull.Value), new SqlParameter("@ThuClosed", hours.ThuClosed), new SqlParameter("@ThuEnd1", hours.ThuEnd1 ?? DBNull.Value), new SqlParameter("@ThuEnd2", hours.ThuEnd2 ?? DBNull.Value), new SqlParameter("@ThuStart1", hours.ThuStart1 ?? DBNull.Value), new SqlParameter("@ThuStart2", hours.ThuStart2 ?? DBNull.Value), new SqlParameter("@TueClosed", hours.TueClosed), new SqlParameter("@TueEnd1", hours.TueEnd1 ?? DBNull.Value),
                new SqlParameter("@TueEnd2", hours.TueEnd2 ?? DBNull.Value), new SqlParameter("@TueStart1", hours.TueStart1 ?? DBNull.Value), new SqlParameter("@TueStart2", hours.TueStart2 ?? DBNull.Value), new SqlParameter("@WedClosed", hours.WedClosed), new SqlParameter("@WedEnd1", hours.WedEnd1 ?? DBNull.Value), new SqlParameter("@WedEnd2", hours.WedEnd2 ?? DBNull.Value), new SqlParameter("@WedStart1", hours.WedStart1 ?? DBNull.Value), new SqlParameter("@WedStart2", hours.WedStart2 ?? DBNull.Value)
            };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_OpeningHoursUpdate", commandParameters) <= 0)
            {
                return null;
            }
            string key = $"OPENING_HOURS-{"SALON-" + hours.SalonId}";
            this._cacheManager.Remove(key);
            hours = this.GetOpeningHoursById(hours.OpeningHoursId);
            return hours;
        }

        public SalonDB UpdateSalon(SalonDB salon)
        {
            if (salon == null)
            {
                throw new ArgumentNullException("salon");
            }
            if (salon.SalonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            if (salon.Name == null)
            {
                throw new ArgumentNullException("salon.Name");
            }
            if (salon.SEName == null)
            {
                throw new ArgumentNullException("salon.SEName");
            }
            if (salon.Latitude == null)
            {
                throw new ArgumentNullException("salon.Latitude");
            }
            if (salon.Longitude == null)
            {
                throw new ArgumentNullException("salon.Longitude");
            }
            if (salon.TotalTicketCount < 0)
            {
                throw new ArgumentOutOfRangeException("salon.TotalTicketCount", salon.TotalTicketCount, "total ticket count cannot be less than 0.");
            }
            if (salon.TotalRatingSum < 0.0)
            {
                throw new ArgumentOutOfRangeException("salon.TotalRatingSum", salon.TotalRatingSum, "total rating sum order cannot be less than 0.");
            }
            if (salon.TotalRatingVotes < 0.0)
            {
                throw new ArgumentOutOfRangeException("salon.TotalRatingVotes", salon.TotalRatingVotes, "total rating votes cannot be less than 0.");
            }
            if (salon.TotalReviews < 0)
            {
                throw new ArgumentOutOfRangeException("salon.TotalReviews", salon.TotalReviews, "total reviews cannot be less than 0.");
            }
            salon.Abbreviation = salon.Abbreviation ?? string.Empty;
            salon.AddressLine1 = salon.AddressLine1 ?? string.Empty;
            salon.AddressLine2 = salon.AddressLine2 ?? string.Empty;
            salon.AddressLine3 = salon.AddressLine3 ?? string.Empty;
            salon.AddressLine4 = salon.AddressLine4 ?? string.Empty;
            salon.AddressLine5 = salon.AddressLine5 ?? string.Empty;
            salon.AdminComment = salon.AdminComment ?? string.Empty;
            salon.CityTown = salon.CityTown ?? string.Empty;
            salon.Company = salon.Company ?? string.Empty;
            salon.Country = salon.Country ?? string.Empty;
            salon.County = salon.County ?? string.Empty;
            salon.Directions = salon.Directions ?? string.Empty;
            salon.Email = salon.Email ?? string.Empty;
            salon.FaxNumber = salon.FaxNumber ?? string.Empty;
            salon.FullDescription = salon.FullDescription ?? string.Empty;
            salon.Latitude = salon.Latitude ?? string.Empty;
            salon.Longitude = salon.Longitude ?? string.Empty;
            salon.MetaDescription = salon.MetaDescription ?? string.Empty;
            salon.MetaKeywords = salon.MetaKeywords ?? string.Empty;
            salon.MetaTitle = salon.MetaTitle ?? string.Empty;
            salon.Mobile = salon.Mobile ?? string.Empty;
            salon.Name = salon.Name ?? string.Empty;
            salon.PhoneNumber = salon.PhoneNumber ?? string.Empty;
            salon.RealexPayerRef = salon.RealexPayerRef ?? string.Empty;
            salon.SEName = salon.SEName ?? string.Empty;
            salon.ShortDescription = salon.ShortDescription ?? string.Empty;
            salon.StateProvince = salon.StateProvince ?? string.Empty;
            salon.VatNumber = salon.VatNumber ?? string.Empty;
            salon.VatNumberStatus = salon.VatNumberStatus ?? string.Empty;
            salon.Website = salon.Website ?? string.Empty;
            salon.ZipPostalCode = salon.ZipPostalCode ?? string.Empty;
            SqlParameter[] parameterArray2 = new SqlParameter[0x30];
            parameterArray2[0] = new SqlParameter("@SalonId", salon.SalonId);
            parameterArray2[1] = new SqlParameter("@Abbreviation", salon.Abbreviation);
            parameterArray2[2] = new SqlParameter("@Active", salon.Active);
            parameterArray2[3] = new SqlParameter("@AddressLine1", salon.AddressLine1);
            parameterArray2[4] = new SqlParameter("@AddressLine2", salon.AddressLine2);
            parameterArray2[5] = new SqlParameter("@AddressLine3", salon.AddressLine3);
            parameterArray2[6] = new SqlParameter("@AddressLine4", salon.AddressLine4);
            parameterArray2[7] = new SqlParameter("@AddressLine5", salon.AddressLine5);
            parameterArray2[8] = new SqlParameter("@AdminComment", salon.AdminComment);
            parameterArray2[9] = new SqlParameter("@BookOnWeb", salon.BookOnWeb);
            parameterArray2[10] = new SqlParameter("@BookOnWidget", salon.BookOnWidget);
            parameterArray2[11] = new SqlParameter("@BookOnMobile", salon.BookOnMobile);
            parameterArray2[12] = new SqlParameter("@CityTown", salon.CityTown);
            parameterArray2[13] = new SqlParameter("@Company", salon.Company);
            parameterArray2[14] = new SqlParameter("@Country", salon.Country);
            parameterArray2[15] = new SqlParameter("@County", salon.County);
            parameterArray2[0x10] = new SqlParameter("@CreatedOnUtc", salon.CreatedOnUtc);
            parameterArray2[0x11] = new SqlParameter("@Deleted", salon.Deleted);
            parameterArray2[0x12] = new SqlParameter("@Directions", salon.Directions);
            parameterArray2[0x13] = new SqlParameter("@Email", salon.Email);
            parameterArray2[20] = new SqlParameter("@FaxNumber", salon.FaxNumber);
            parameterArray2[0x15] = new SqlParameter("@FullDescription", salon.FullDescription);
            parameterArray2[0x16] = new SqlParameter("@Latitude", salon.Latitude);
            parameterArray2[0x17] = new SqlParameter("@Longitude", salon.Longitude);
            parameterArray2[0x18] = new SqlParameter("@MetaDescription", salon.MetaDescription);
            parameterArray2[0x19] = new SqlParameter("@MetaKeywords", salon.MetaKeywords);
            parameterArray2[0x1a] = new SqlParameter("@MetaTitle", salon.MetaTitle);
            parameterArray2[0x1b] = new SqlParameter("@Mobile", salon.Mobile);
            parameterArray2[0x1c] = new SqlParameter("@Name", salon.Name);
            parameterArray2[0x1d] = new SqlParameter("@PhoneNumber", salon.PhoneNumber);
            Guid? pictureId = salon.PictureId;
            parameterArray2[30] = new SqlParameter("@PictureId", pictureId.HasValue ? ((SqlGuid) pictureId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[0x1f] = new SqlParameter("@Published", salon.Published);
            parameterArray2[0x20] = new SqlParameter("@CurrencyCode", salon.CurrencyCode);
            parameterArray2[0x21] = new SqlParameter("@RealexPayerRef", salon.RealexPayerRef);
            parameterArray2[0x22] = new SqlParameter("@SEName", salon.SEName);
            parameterArray2[0x23] = new SqlParameter("@ShortDescription", salon.ShortDescription);
            parameterArray2[0x24] = new SqlParameter("@ShowOnWeb", salon.ShowOnWeb);
            parameterArray2[0x25] = new SqlParameter("@ShowOnWidget", salon.ShowOnWidget);
            parameterArray2[0x26] = new SqlParameter("@ShowOnMobile", salon.ShowOnMobile);
            parameterArray2[0x27] = new SqlParameter("@StateProvince", salon.StateProvince);
            parameterArray2[40] = new SqlParameter("@TotalTicketCount", salon.TotalTicketCount);
            parameterArray2[0x29] = new SqlParameter("@TotalRatingSum", salon.TotalRatingSum);
            parameterArray2[0x2a] = new SqlParameter("@TotalRatingVotes", salon.TotalRatingVotes);
            parameterArray2[0x2b] = new SqlParameter("@TotalReviews", salon.TotalReviews);
            parameterArray2[0x2c] = new SqlParameter("@VatNumber", salon.VatNumber);
            parameterArray2[0x2d] = new SqlParameter("@VatNumberStatus", salon.VatNumberStatus);
            parameterArray2[0x2e] = new SqlParameter("@Website", salon.Website);
            parameterArray2[0x2f] = new SqlParameter("@ZipPostalCode", salon.ZipPostalCode);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonUpdate", commandParameters) <= 0)
            {
                return null;
            }
            this._cacheManager.Remove($"SALON-{salon.SalonId}");
            this._cacheManager.Remove($"SALON-{salon.SEName}");
            salon = this.GetSalonById(salon.SalonId);
            return salon;
        }

        public SalonPaymentMethodDB UpdateSalonPaymentMethod(SalonPaymentMethodDB paymentMethod)
        {
            if (paymentMethod == null)
            {
                throw new ArgumentNullException("paymentMethod");
            }
            if (paymentMethod.SalonPaymentMethodId == Guid.Empty)
            {
                throw new ArgumentNullException("Salon payment method identifier cannot be empty.");
            }
            if (paymentMethod.RealexPayerRef == null)
            {
                throw new ArgumentException("Realex player reference cannot be null.");
            }
            if (paymentMethod.RealexPayerRef == string.Empty)
            {
                throw new ArgumentException("Realex player reference cannot be empty.");
            }
            if (paymentMethod.RealexCardRef == null)
            {
                throw new ArgumentException("Realex card reference cannot be null.");
            }
            if (paymentMethod.RealexCardRef == string.Empty)
            {
                throw new ArgumentException("Realex card reference cannot be empty.");
            }
            if (paymentMethod.SalonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            paymentMethod.Alias = paymentMethod.Alias ?? string.Empty;
            paymentMethod.CardCvv2 = paymentMethod.CardCvv2 ?? string.Empty;
            paymentMethod.CardExpirationMonth = paymentMethod.CardExpirationMonth ?? string.Empty;
            paymentMethod.CardExpirationYear = paymentMethod.CardExpirationYear ?? string.Empty;
            paymentMethod.CardName = paymentMethod.CardName ?? string.Empty;
            paymentMethod.CardNumber = paymentMethod.CardNumber ?? string.Empty;
            paymentMethod.CardType = paymentMethod.CardType ?? string.Empty;
            paymentMethod.CreatedBy = paymentMethod.CreatedBy ?? string.Empty;
            paymentMethod.MaskedCardNumber = paymentMethod.MaskedCardNumber ?? string.Empty;
            paymentMethod.RealexPayerRef = paymentMethod.RealexPayerRef ?? string.Empty;
            paymentMethod.RealexCardRef = paymentMethod.RealexCardRef ?? string.Empty;
            paymentMethod.UpdatedBy = paymentMethod.UpdatedBy ?? string.Empty;
            SqlParameter[] commandParameters = new SqlParameter[] { 
                new SqlParameter("@SalonPaymentMethodId", paymentMethod.SalonPaymentMethodId), new SqlParameter("@RealexCardRef", paymentMethod.RealexCardRef), new SqlParameter("@RealexPayerRef", paymentMethod.RealexPayerRef), new SqlParameter("@SalonId", paymentMethod.SalonId), new SqlParameter("@Alias", paymentMethod.Alias), new SqlParameter("@CardType", paymentMethod.CardType), new SqlParameter("@CardName", paymentMethod.CardName), new SqlParameter("@CardNumber", paymentMethod.CardNumber), new SqlParameter("@MaskedCardNumber", paymentMethod.MaskedCardNumber), new SqlParameter("@CardCvv2", paymentMethod.CardCvv2), new SqlParameter("@CardExpirationMonth", paymentMethod.CardExpirationMonth), new SqlParameter("@CardExpirationYear", paymentMethod.CardExpirationYear), new SqlParameter("@IsPrimary", paymentMethod.IsPrimary), new SqlParameter("@Active", paymentMethod.Active), new SqlParameter("@CreatedOn", paymentMethod.CreatedOn), new SqlParameter("@CreatedBy", paymentMethod.CreatedBy),
                new SqlParameter("@UpdatedOn", paymentMethod.UpdatedOn), new SqlParameter("@UpdatedBy", paymentMethod.UpdatedBy)
            };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonPaymentMethodUpdate", commandParameters) <= 0)
            {
                return null;
            }
            paymentMethod = this.GetSalonPaymentMethodById(paymentMethod.SalonPaymentMethodId);
            return paymentMethod;
        }

        public SalonRatingDB UpdateSalonRating(SalonRatingDB rating)
        {
            if (rating == null)
            {
                throw new ArgumentNullException("rating");
            }
            if (rating.SalonRatingId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("rating.SalonRatingId", "SalonRatingId cannot be empty.");
            }
            if (rating.SalonId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("rating.SalonId", "SalonId cannot be empty.");
            }
            if (rating.EmployeeOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.EmployeeOverallRating", "EmployeeOverallRating cannot be less than zero.");
            }
            if (rating.EmployeeOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.EmployeeOverallSum", "EmployeeOverallSum cannot be less than zero.");
            }
            if (rating.EmployeeOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.EmployeeOverallTotal", "EmployeeOverallTotal cannot be less than zero.");
            }
            if (rating.GoAgainOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.GoAgainOverallRating", "GoAgainOverallRating cannot be less than zero.");
            }
            if (rating.GoAgainOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.GoAgainOverallSum", "GoAgainOverallSum cannot be less than zero.");
            }
            if (rating.GoAgainOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.GoAgainOverallTotal", "GoAgainOverallTotal cannot be less than zero.");
            }
            if (rating.SalonOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SalonOverallRating", "SalonOverallRating cannot be less than zero.");
            }
            if (rating.SalonOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SalonOverallSum", "SalonOverallSum cannot be less than zero.");
            }
            if (rating.SalonOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SalonOverallTotal", "SalonOverallTotal cannot be less than zero.");
            }
            if (rating.SatisfactionOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SatisfactionOverallRating", "SatisfactionOverallRating cannot be less than zero.");
            }
            if (rating.SatisfactionOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SatisfactionOverallSum", "SatisfactionOverallSum cannot be less than zero.");
            }
            if (rating.SatisfactionOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.SatisfactionOverallTotal", "SatisfactionOverallTotal cannot be less than zero.");
            }
            if (rating.ServiceOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ServiceOverallRating", "ServiceOverallRating cannot be less than zero.");
            }
            if (rating.ServiceOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ServiceOverallSum", "ServiceOverallSum cannot be less than zero.");
            }
            if (rating.ServiceOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ServiceOverallTotal", "ServiceOverallTotal cannot be less than zero.");
            }
            if (rating.ValueOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ValueOverallRating", "ValueOverallRating cannot be less than zero.");
            }
            if (rating.ValueOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ValueOverallSum", "ValueOverallSum cannot be less than zero.");
            }
            if (rating.ValueOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.ValueOverallTotal", "ValueOverallTotal cannot be less than zero.");
            }
            if (rating.WeightedOverallRating < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.WeightedOverallRating", "WeightedOverallRating cannot be less than zero.");
            }
            if (rating.WeightedOverallSum < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.WeightedOverallSum", "WeightedOverallSum cannot be less than zero.");
            }
            if (rating.WeightedOverallTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("rating.WeightedOverallTotal", "WeightedOverallTotal cannot be less than zero.");
            }
            if (rating.ReviewCount < 0)
            {
                throw new ArgumentOutOfRangeException("rating.ReviewCount", "ReviewCount cannot be less than zero.");
            }
            if (!string.IsNullOrEmpty(rating.LastReviewerDate))
            {
                if (!Regex.IsMatch(rating.LastReviewerDate, @"(\d{4})-(\d{2})-(\d{2})"))
                {
                    throw new ArgumentException("LastReviewerDate is an invalid date.");
                }
                try
                {
                    DateTime.ParseExact(rating.LastReviewerDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("LastReviewerDate is an invalid date.");
                }
            }
            if (!string.IsNullOrEmpty(rating.LastReviewerTime))
            {
                if (!Regex.IsMatch(rating.LastReviewerTime, @"(\d{2}):(\d{2})"))
                {
                    throw new ArgumentException("LastReviewerTime is an invalid time.");
                }
                try
                {
                    DateTime.ParseExact(rating.LastReviewerTime, "HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("LastReviewerTime is an invalid time.");
                }
            }
            rating.LastReviewerDate = rating.LastReviewerDate ?? string.Empty;
            rating.LastReviewerTime = rating.LastReviewerTime ?? string.Empty;
            rating.LastReviewerDescription = rating.LastReviewerDescription ?? string.Empty;
            SqlParameter[] commandParameters = new SqlParameter[] { 
                new SqlParameter("@SalonRatingId", rating.SalonRatingId), new SqlParameter("@SalonId", rating.SalonId), new SqlParameter("@SalonOverallSum", rating.SalonOverallSum), new SqlParameter("@SalonOverallTotal", rating.SalonOverallTotal), new SqlParameter("@SalonOverallRating", rating.SalonOverallRating), new SqlParameter("@EmployeeOverallSum", rating.EmployeeOverallSum), new SqlParameter("@EmployeeOverallTotal", rating.EmployeeOverallTotal), new SqlParameter("@EmployeeOverallRating", rating.EmployeeOverallRating), new SqlParameter("@ServiceOverallSum", rating.ServiceOverallSum), new SqlParameter("@ServiceOverallTotal", rating.ServiceOverallTotal), new SqlParameter("@ServiceOverallRating", rating.ServiceOverallRating), new SqlParameter("@SatisfactionOverallSum", rating.SatisfactionOverallSum), new SqlParameter("@SatisfactionOverallTotal", rating.SatisfactionOverallTotal), new SqlParameter("@SatisfactionOverallRating", rating.SatisfactionOverallRating), new SqlParameter("@ValueOverallSum", rating.ValueOverallSum), new SqlParameter("@ValueOverallTotal", rating.ValueOverallTotal),
                new SqlParameter("@ValueOverallRating", rating.ValueOverallRating), new SqlParameter("@GoAgainOverallSum", rating.GoAgainOverallSum), new SqlParameter("@GoAgainOverallTotal", rating.GoAgainOverallTotal), new SqlParameter("@GoAgainOverallRating", rating.GoAgainOverallRating), new SqlParameter("@WeightedOverallSum", rating.WeightedOverallSum), new SqlParameter("@WeightedOverallTotal", rating.WeightedOverallTotal), new SqlParameter("@WeightedOverallRating", rating.WeightedOverallRating), new SqlParameter("@ReviewCount", rating.ReviewCount), new SqlParameter("@LastReviewerDate", rating.LastReviewerDate), new SqlParameter("@LastReviewerDescription", rating.LastReviewerDescription), new SqlParameter("@LastReviewerTime", rating.LastReviewerTime)
            };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonRatingUpdate", commandParameters) <= 0)
            {
                return null;
            }
            rating = this.GetSalonRatingById(rating.SalonRatingId);
            return rating;
        }

        public SalonReviewDB UpdateSalonReview(SalonReviewDB review)
        {
            if (review == null)
            {
                throw new ArgumentNullException("review");
            }
            if (review.SalonReviewId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.SalonReviewId", "SalonReviewId cannot be empty.");
            }
            if (review.AppointmentId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.AppointmentId", "AppointmentId cannot be empty.");
            }
            if (review.EmployeeId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.EmployeeId", "EmployeeId cannot be empty.");
            }
            if (review.EmployeeRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.EmployeeRating", "EmployeeRating cannot be less than zero.");
            }
            if (review.EmployeeRatingSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.EmployeeRatingSum", "EmployeeRatingSum cannot be less than zero.");
            }
            if (review.EmployeeRatingTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.EmployeeRatingTotal", "EmployeeRatingTotal cannot be less than zero.");
            }
            if (review.SalonId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.SalonId", "SalonId cannot be empty.");
            }
            if (review.SalonRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SalonRating", "SalonRating cannot be less than zero.");
            }
            if (review.SalonRatingSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SalonRatingSum", "SalonRatingSum cannot be less than zero.");
            }
            if (review.SalonRatingTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SalonRatingTotal", "SalonRatingTotal cannot be less than zero.");
            }
            if (review.SatisfactionRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SatisfactionRating", "SatisfactionRating cannot be less than zero.");
            }
            if (review.SatisfactionRatingSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SatisfactionRatingSum", "SatisfactionRatingSum cannot be less than zero.");
            }
            if (review.SatisfactionRatingTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.SatisfactionRatingTotal", "SatisfactionRatingTotal cannot be less than zero.");
            }
            if (review.ServiceId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.ServiceId", "ServiceId cannot be empty.");
            }
            if (review.ServiceRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ServiceRating", "ServiceRating cannot be less than zero.");
            }
            if (review.ServiceRatingSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ServiceRatingSum", "ServiceRatingSum cannot be less than zero.");
            }
            if (review.ServiceRatingTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ServiceRatingTotal", "ServiceRatingTotal cannot be less than zero.");
            }
            if (review.ValueRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ValueRating", "ValueRating cannot be less than zero.");
            }
            if (review.ValueRatingSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ValueRatingSum", "ValueRatingSum cannot be less than zero.");
            }
            if (review.ValueRatingTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.ValueRatingTotal", "ValueRatingTotal cannot be less than zero.");
            }
            if (review.WeightedRating < 0f)
            {
                throw new ArgumentOutOfRangeException("review.WeightedRating", "WeightedRating cannot be less than zero.");
            }
            if (review.WeightedSum < 0f)
            {
                throw new ArgumentOutOfRangeException("review.WeightedSum", "WeightedSum cannot be less than zero.");
            }
            if (review.WeightedTotal < 0f)
            {
                throw new ArgumentOutOfRangeException("review.WeightedTotal", "WeightedTotal cannot be less than zero.");
            }
            if (review.UserId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("review.UserId", "UserId cannot be empty.");
            }
            if (!string.IsNullOrEmpty(review.AppointmentDate))
            {
                if (!Regex.IsMatch(review.AppointmentDate, @"(\d{4})-(\d{2})-(\d{2})"))
                {
                    throw new ArgumentException("AppointmentDate is an invalid date.");
                }
                try
                {
                    DateTime.ParseExact(review.AppointmentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("AppointmentDate is an invalid date.");
                }
            }
            if (!string.IsNullOrEmpty(review.AppointmentTime))
            {
                if (!Regex.IsMatch(review.AppointmentTime, @"(\d{2}):(\d{2})"))
                {
                    throw new ArgumentException("AppointmentTime is an invalid time.");
                }
                try
                {
                    DateTime.ParseExact(review.AppointmentTime, "HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("AppointmentTime is an invalid time.");
                }
            }
            if (!string.IsNullOrEmpty(review.ReviewerDate))
            {
                if (!Regex.IsMatch(review.ReviewerDate, @"(\d{4})-(\d{2})-(\d{2})"))
                {
                    throw new ArgumentException("ReviewerDate is an invalid date.");
                }
                try
                {
                    DateTime.ParseExact(review.ReviewerDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("ReviewerDate is an invalid date.");
                }
            }
            if (!string.IsNullOrEmpty(review.ReviewerTime))
            {
                if (!Regex.IsMatch(review.ReviewerTime, @"(\d{2}):(\d{2})"))
                {
                    throw new ArgumentException("ReviewerTime is an invalid time.");
                }
                try
                {
                    DateTime.ParseExact(review.ReviewerTime, "HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("ReviewerTime is an invalid time.");
                }
            }
            if (!string.IsNullOrEmpty(review.ApproverDate))
            {
                if (!Regex.IsMatch(review.ApproverDate, @"(\d{4})-(\d{2})-(\d{2})"))
                {
                    throw new ArgumentException("ApproverDate is an invalid date.");
                }
                try
                {
                    DateTime.ParseExact(review.ApproverDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("ApproverDate is an invalid date.");
                }
            }
            if (!string.IsNullOrEmpty(review.ApproverTime))
            {
                if (!Regex.IsMatch(review.ApproverTime, @"(\d{2}):(\d{2})"))
                {
                    throw new ArgumentException("ApproverTime is an invalid time.");
                }
                try
                {
                    DateTime.ParseExact(review.ApproverTime, "HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new ArgumentException("ApproverTime is an invalid time.");
                }
            }
            review.ApproverDescription = review.ApproverDescription ?? string.Empty;
            review.Comments = review.Comments ?? string.Empty;
            review.CreatedBy = review.CreatedBy ?? string.Empty;
            review.EmployeeDescription = review.EmployeeDescription ?? string.Empty;
            review.ReviewerDescription = review.ReviewerDescription ?? string.Empty;
            review.SalonDescription = review.SalonDescription ?? string.Empty;
            review.ServiceDescription = review.ServiceDescription ?? string.Empty;
            review.UpdatedBy = review.UpdatedBy ?? string.Empty;
            review.UserDescription = review.UserDescription ?? string.Empty;
            review.ApproverDate = review.ApproverDate ?? string.Empty;
            review.ApproverTime = review.ApproverTime ?? string.Empty;
            review.AppointmentDate = review.AppointmentDate ?? string.Empty;
            review.AppointmentTime = review.AppointmentTime ?? string.Empty;
            review.ReviewerDate = review.ReviewerDate ?? string.Empty;
            review.ReviewerTime = review.ReviewerTime ?? string.Empty;
            SqlParameter[] parameterArray2 = new SqlParameter[0x2a];
            parameterArray2[0] = new SqlParameter("@SalonReviewId", review.SalonReviewId);
            parameterArray2[1] = new SqlParameter("@AppointmentDate", review.AppointmentDate);
            parameterArray2[2] = new SqlParameter("@AppointmentId", review.AppointmentId ?? SqlGuid.Null);
            parameterArray2[3] = new SqlParameter("@AppointmentTime", review.AppointmentTime);
            parameterArray2[4] = new SqlParameter("@ApproverDate", review.ApproverDate);
            parameterArray2[5] = new SqlParameter("@ApproverDescription", review.ApproverDescription);
            parameterArray2[6] = new SqlParameter("@ApproverTime", review.ApproverTime);
            parameterArray2[7] = new SqlParameter("@Comments", review.Comments);
            parameterArray2[8] = new SqlParameter("@CreatedBy", review.CreatedBy);
            parameterArray2[9] = new SqlParameter("@CreatedOn", review.CreatedOn);
            parameterArray2[10] = new SqlParameter("@EmployeeDescription", review.EmployeeDescription);
            Guid? employeeId = review.EmployeeId;
            parameterArray2[11] = new SqlParameter("@EmployeeId", employeeId.HasValue ? ((SqlGuid) employeeId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[12] = new SqlParameter("@EmployeeRating", review.EmployeeRating);
            parameterArray2[13] = new SqlParameter("@EmployeeRatingSum", review.EmployeeRatingSum);
            parameterArray2[14] = new SqlParameter("@EmployeeRatingTotal", review.EmployeeRatingTotal);
            parameterArray2[15] = new SqlParameter("@GoAgain", review.GoAgain);
            parameterArray2[0x10] = new SqlParameter("@ReviewerDate", review.ReviewerDate);
            parameterArray2[0x11] = new SqlParameter("@ReviewerDescription", review.ReviewerDescription);
            parameterArray2[0x12] = new SqlParameter("@ReviewerTime", review.ReviewerTime);
            parameterArray2[0x13] = new SqlParameter("@SalonDescription", review.SalonDescription);
            parameterArray2[20] = new SqlParameter("@SalonId", review.SalonId);
            parameterArray2[0x15] = new SqlParameter("@SalonRating", review.SalonRating);
            parameterArray2[0x16] = new SqlParameter("@SalonRatingSum", review.SalonRatingSum);
            parameterArray2[0x17] = new SqlParameter("@SalonRatingTotal", review.SalonRatingTotal);
            parameterArray2[0x18] = new SqlParameter("@SatisfactionRating", review.SatisfactionRating);
            parameterArray2[0x19] = new SqlParameter("@SatisfactionRatingSum", review.SatisfactionRatingSum);
            parameterArray2[0x1a] = new SqlParameter("@SatisfactionRatingTotal", review.SatisfactionRatingTotal);
            parameterArray2[0x1b] = new SqlParameter("@ServiceDescription", review.ServiceDescription);
            parameterArray2[0x1c] = new SqlParameter("@ServiceId", review.ServiceId ?? SqlGuid.Null);
            parameterArray2[0x1d] = new SqlParameter("@ServiceRating", review.ServiceRating);
            parameterArray2[30] = new SqlParameter("@ServiceRatingSum", review.ServiceRatingSum);
            parameterArray2[0x1f] = new SqlParameter("@ServiceRatingTotal", review.ServiceRatingTotal);
            parameterArray2[0x20] = new SqlParameter("@UpdatedBy", review.UpdatedBy);
            parameterArray2[0x21] = new SqlParameter("@UpdatedOn", review.UpdatedOn);
            parameterArray2[0x22] = new SqlParameter("@UserDescription", review.UserDescription);
            parameterArray2[0x23] = new SqlParameter("@WeightedSum", review.WeightedSum);
            parameterArray2[0x24] = new SqlParameter("@WeightedTotal", review.WeightedTotal);
            parameterArray2[0x25] = new SqlParameter("@WeightedRating", review.WeightedRating);
            parameterArray2[0x26] = new SqlParameter("@UserId", review.UserId ?? SqlGuid.Null);
            parameterArray2[0x27] = new SqlParameter("@ValueRating", review.ValueRating);
            parameterArray2[40] = new SqlParameter("@ValueRatingSum", review.ValueRatingSum);
            parameterArray2[0x29] = new SqlParameter("@ValueRatingTotal", review.ValueRatingTotal);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonReviewUpdate", commandParameters) <= 0)
            {
                return null;
            }
            review = this.GetSalonReviewById(review.SalonReviewId);
            return review;
        }
    }
}

