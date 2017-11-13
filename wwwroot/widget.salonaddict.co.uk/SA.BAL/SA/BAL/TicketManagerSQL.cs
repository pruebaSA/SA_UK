namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;

    public class TicketManagerSQL : ITicketManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public TicketManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        public void DeleteTicketAlert(TicketAlertDB alert)
        {
            if (alert == null)
            {
                throw new ArgumentNullException("alert");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@AlertId", alert.AlertId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TicketAlertDeleteById", commandParameters);
        }

        public List<OpenTicketDB> GetOpenTicketsBySalonId(Guid salonId)
        {
            List<OpenTicketDB> list = new List<OpenTicketDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_ReportSalonOpenTickets", parameterValues))
            {
                while (reader.Read())
                {
                    OpenTicketDB item = this.OpenTicketMappingDB(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public TicketAlertDB GetTicketAlertById(Guid alertId)
        {
            TicketAlertDB tdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@AlertId", alertId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_TicketAlertLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    tdb = this.TicketAlertMappingDB(reader);
                }
            }
            return tdb;
        }

        public List<TicketAlertDB> GetTicketAlertsBySalonId(Guid salonId)
        {
            List<TicketAlertDB> list = new List<TicketAlertDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_TicketAlertLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    TicketAlertDB item = this.TicketAlertMappingDB(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public TicketSummaryDB GetTicketById(Guid ticketId)
        {
            TicketSummaryDB ydb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@TicketId", ticketId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_TicketSummaryLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    ydb = this.TicketSummaryMappingDB(reader);
                }
            }
            return ydb;
        }

        public TicketRowDB GetTicketRowById(Guid ticketRowId)
        {
            TicketRowDB wdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@TicketRowId", ticketRowId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_TicketRowLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    wdb = this.TicketRowMappingDB(reader);
                }
            }
            return wdb;
        }

        public List<TicketRowDB> GetTicketRowsByTicketId(Guid ticketId)
        {
            List<TicketRowDB> list = new List<TicketRowDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@TicketId", ticketId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_TicketRowLoadByTicketId", parameterValues))
            {
                while (reader.Read())
                {
                    TicketRowDB item = this.TicketRowMappingDB(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public TicketSummaryDB InsertTicket(TicketSummaryDB ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("ticket");
            }
            ticket.AdminComment = ticket.AdminComment ?? string.Empty;
            ticket.AuthorizationTransactionCode = ticket.AuthorizationTransactionCode ?? string.Empty;
            ticket.AuthorizationTransactionId = ticket.AuthorizationTransactionId ?? string.Empty;
            ticket.AuthorizationTransactionResult = ticket.AuthorizationTransactionResult ?? string.Empty;
            ticket.BillingDisplayText = ticket.BillingDisplayText ?? string.Empty;
            ticket.BillingEmail = ticket.BillingEmail ?? string.Empty;
            ticket.BillingFirstName = ticket.BillingFirstName ?? string.Empty;
            ticket.BillingLastName = ticket.BillingLastName ?? string.Empty;
            ticket.BillingMobile = ticket.BillingMobile ?? string.Empty;
            ticket.BillingPhone = ticket.BillingPhone ?? string.Empty;
            ticket.CaptureTransactionId = ticket.CaptureTransactionId ?? string.Empty;
            ticket.CaptureTransactionResult = ticket.CaptureTransactionResult ?? string.Empty;
            ticket.CardName = ticket.CardName ?? string.Empty;
            ticket.CardNumberMasked = ticket.CardNumberMasked ?? string.Empty;
            ticket.CardType = ticket.CardType ?? string.Empty;
            ticket.CurrencyCode = ticket.CurrencyCode ?? string.Empty;
            ticket.CustomerDisplayText = ticket.CustomerDisplayText ?? string.Empty;
            ticket.CustomerEmail = ticket.CustomerEmail ?? string.Empty;
            ticket.CustomerFirstName = ticket.CustomerFirstName ?? string.Empty;
            ticket.CustomerIPAddress = ticket.CustomerIPAddress ?? string.Empty;
            ticket.CustomerLastName = ticket.CustomerLastName ?? string.Empty;
            ticket.CustomerMobile = ticket.CustomerMobile ?? string.Empty;
            ticket.CustomerPhone = ticket.CustomerPhone ?? string.Empty;
            ticket.CustomerSpecialRequest = ticket.CustomerSpecialRequest ?? string.Empty;
            ticket.OpenUserDisplayText = ticket.OpenUserDisplayText ?? string.Empty;
            ticket.Language = ticket.Language ?? string.Empty;
            ticket.SalonDisplayText = ticket.SalonDisplayText ?? string.Empty;
            ticket.TicketNumber = ticket.TicketNumber ?? string.Empty;
            ticket.VoidUserDisplayText = ticket.VoidUserDisplayText ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@TicketId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[0x3b];
            parameterArray2[0] = parameter;
            parameterArray2[1] = new SqlParameter("@AdminComment", ticket.AdminComment);
            parameterArray2[2] = new SqlParameter("@AuthorizationTransactionCode", ticket.AuthorizationTransactionCode);
            parameterArray2[3] = new SqlParameter("@AuthorizationTransactionId", ticket.AuthorizationTransactionId);
            parameterArray2[4] = new SqlParameter("@AuthorizationTransactionResult", ticket.AuthorizationTransactionResult);
            parameterArray2[5] = new SqlParameter("@BillingDisplayText", ticket.BillingDisplayText);
            parameterArray2[6] = new SqlParameter("@BillingEmail", ticket.BillingEmail);
            parameterArray2[7] = new SqlParameter("@BillingFirstName", ticket.BillingFirstName);
            parameterArray2[8] = new SqlParameter("@BillingLastName", ticket.BillingLastName);
            parameterArray2[9] = new SqlParameter("@BillingMobile", ticket.BillingMobile);
            parameterArray2[10] = new SqlParameter("@BillingPhone", ticket.BillingPhone);
            parameterArray2[11] = new SqlParameter("@BookedOnMobile", ticket.BookedOnMobile);
            parameterArray2[12] = new SqlParameter("@BookedOnWeb", ticket.BookedOnWeb);
            parameterArray2[13] = new SqlParameter("@BookedOnWidget", ticket.BookedOnWidget);
            DateTime? cancelledOnUtc = ticket.CancelledOnUtc;
            parameterArray2[14] = new SqlParameter("@CancelledOnUtc", cancelledOnUtc.HasValue ? ((SqlDateTime) cancelledOnUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[15] = new SqlParameter("@CaptureTransactionId", ticket.CaptureTransactionId);
            parameterArray2[0x10] = new SqlParameter("@CaptureTransactionResult", ticket.CaptureTransactionResult);
            int? cardExpirationMonth = ticket.CardExpirationMonth;
            parameterArray2[0x11] = new SqlParameter("@CardExpirationMonth", cardExpirationMonth.HasValue ? ((SqlInt32) cardExpirationMonth.GetValueOrDefault()) : SqlInt32.Null);
            int? cardExpirationYear = ticket.CardExpirationYear;
            parameterArray2[0x12] = new SqlParameter("@CardExpirationYear", cardExpirationYear.HasValue ? ((SqlInt32) cardExpirationYear.GetValueOrDefault()) : SqlInt32.Null);
            parameterArray2[0x13] = new SqlParameter("@CardName", ticket.CardName);
            parameterArray2[20] = new SqlParameter("@CardNumberMasked", ticket.CardNumberMasked);
            parameterArray2[0x15] = new SqlParameter("@CardType", ticket.CardType);
            DateTime? closedOnUtc = ticket.ClosedOnUtc;
            parameterArray2[0x16] = new SqlParameter("@ClosedOnUtc", closedOnUtc.HasValue ? ((SqlDateTime) closedOnUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x17] = new SqlParameter("@Confirmed", ticket.Confirmed);
            DateTime? openedOnUtc = ticket.OpenedOnUtc;
            parameterArray2[0x18] = new SqlParameter("@OpenedOnUtc", openedOnUtc.HasValue ? ((SqlDateTime) openedOnUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x19] = new SqlParameter("@OpenUserDisplayText", ticket.OpenUserDisplayText);
            Guid? openUserId = ticket.OpenUserId;
            parameterArray2[0x1a] = new SqlParameter("@OpenUserId", openUserId.HasValue ? ((SqlGuid) openUserId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[0x1b] = new SqlParameter("@CreatedOnUtc", ticket.CreatedOnUtc);
            parameterArray2[0x1c] = new SqlParameter("@CurrencyCode", ticket.CurrencyCode);
            parameterArray2[0x1d] = new SqlParameter("@CurrencyRate", ticket.CurrencyRate);
            parameterArray2[30] = new SqlParameter("@CustomerDisplayText", ticket.CustomerDisplayText);
            parameterArray2[0x1f] = new SqlParameter("@CustomerEmail", ticket.CustomerEmail);
            parameterArray2[0x20] = new SqlParameter("@CustomerFirstName", ticket.CustomerFirstName);
            parameterArray2[0x21] = new SqlParameter("@CustomerIPAddress", ticket.CustomerIPAddress);
            parameterArray2[0x22] = new SqlParameter("@CustomerLastName", ticket.CustomerLastName);
            parameterArray2[0x23] = new SqlParameter("@CustomerMobile", ticket.CustomerMobile);
            parameterArray2[0x24] = new SqlParameter("@CustomerPhone", ticket.CustomerPhone);
            parameterArray2[0x25] = new SqlParameter("@CustomerReminder", ticket.CustomerReminder);
            parameterArray2[0x26] = new SqlParameter("@CustomerSpecialRequest", ticket.CustomerSpecialRequest);
            parameterArray2[0x27] = new SqlParameter("@Deleted", ticket.Deleted);
            parameterArray2[40] = new SqlParameter("@DepositRate", ticket.DepositRate);
            parameterArray2[0x29] = new SqlParameter("@DepositRequired", ticket.DepositRequired);
            parameterArray2[0x2a] = new SqlParameter("@Language", ticket.Language);
            parameterArray2[0x2b] = new SqlParameter("@PaidAmount", ticket.PaidAmount);
            parameterArray2[0x2c] = new SqlParameter("@PaidAmountInCustomerCurrency", ticket.PaidAmountInCustomerCurrency);
            DateTime? paidDateUtc = ticket.PaidDateUtc;
            parameterArray2[0x2d] = new SqlParameter("@PaidDateUtc", paidDateUtc.HasValue ? ((SqlDateTime) paidDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x2e] = new SqlParameter("@RepeatCustomer", ticket.RepeatCustomer);
            parameterArray2[0x2f] = new SqlParameter("@SalonDisplayText", ticket.SalonDisplayText);
            parameterArray2[0x30] = new SqlParameter("@SalonId", ticket.SalonId);
            parameterArray2[0x31] = new SqlParameter("@Subtotal", ticket.Subtotal);
            parameterArray2[50] = new SqlParameter("@TaxRate", ticket.TaxRate);
            parameterArray2[0x33] = new SqlParameter("@TicketNumber", ticket.TicketNumber);
            int? ticketStatusType = ticket.TicketStatusType;
            parameterArray2[0x34] = new SqlParameter("@TicketStatusType", ticketStatusType.HasValue ? ((SqlInt32) ticketStatusType.GetValueOrDefault()) : SqlInt32.Null);
            parameterArray2[0x35] = new SqlParameter("@Total", ticket.Total);
            parameterArray2[0x36] = new SqlParameter("@TotalTax", ticket.TotalTax);
            Guid? userId = ticket.UserId;
            parameterArray2[0x37] = new SqlParameter("@UserId", userId.HasValue ? ((SqlGuid) userId.GetValueOrDefault()) : SqlGuid.Null);
            DateTime? voidedOnUtc = ticket.VoidedOnUtc;
            parameterArray2[0x38] = new SqlParameter("@VoidedOnUtc", voidedOnUtc.HasValue ? ((SqlDateTime) voidedOnUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x39] = new SqlParameter("@VoidUserDisplayText", ticket.VoidUserDisplayText);
            Guid? voidUserId = ticket.VoidUserId;
            parameterArray2[0x3a] = new SqlParameter("@VoidUserId", voidUserId.HasValue ? ((SqlGuid) voidUserId.GetValueOrDefault()) : SqlGuid.Null);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TicketSummaryInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid ticketId = new Guid(parameter.Value.ToString());
            ticket = this.GetTicketById(ticketId);
            return ticket;
        }

        public TicketAlertDB InsertTicketAlert(TicketAlertDB alert)
        {
            if (alert == null)
            {
                throw new ArgumentNullException("alert");
            }
            alert.Email = alert.Email ?? string.Empty;
            alert.DisplayText = alert.DisplayText ?? string.Empty;
            alert.FirstName = alert.FirstName ?? string.Empty;
            alert.LastName = alert.LastName ?? string.Empty;
            alert.Mobile = alert.Mobile ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@AlertId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { parameter, new SqlParameter("@Active", alert.Active), new SqlParameter("@ByEmail", alert.ByEmail), new SqlParameter("@BySMS", alert.BySMS), new SqlParameter("@DisplayText", alert.DisplayText), new SqlParameter("@Email", alert.Email), new SqlParameter("@FirstName", alert.FirstName), new SqlParameter("@LastName", alert.LastName), new SqlParameter("@Mobile", alert.Mobile), new SqlParameter("@SalonId", alert.SalonId) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TicketAlertInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid alertId = new Guid(parameter.Value.ToString());
            alert = this.GetTicketAlertById(alertId);
            return alert;
        }

        public TicketRowDB InsertTicketRow(TicketRowDB ticketRow)
        {
            if (ticketRow == null)
            {
                throw new ArgumentNullException("ticketRow");
            }
            ticketRow.EmployeeDisplayText = ticketRow.EmployeeDisplayText ?? string.Empty;
            ticketRow.ServiceDisplayText = ticketRow.ServiceDisplayText ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@TicketRowId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[0x12];
            parameterArray2[0] = parameter;
            parameterArray2[1] = new SqlParameter("@EmployeeDisplayText", ticketRow.EmployeeDisplayText);
            Guid? employeeId = ticketRow.EmployeeId;
            parameterArray2[2] = new SqlParameter("@EmployeeId", employeeId.HasValue ? ((SqlGuid) employeeId.GetValueOrDefault()) : SqlGuid.Null);
            DateTime? endDate = ticketRow.EndDate;
            parameterArray2[3] = new SqlParameter("@EndDate", endDate.HasValue ? ((SqlDateTime) endDate.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[4] = new SqlParameter("@EndTime", ticketRow.EndTime ?? DBNull.Value);
            parameterArray2[5] = new SqlParameter("@MultiDay", ticketRow.MultiDay);
            parameterArray2[6] = new SqlParameter("@Price", ticketRow.Price);
            parameterArray2[7] = new SqlParameter("@RowOrder", ticketRow.RowOrder);
            parameterArray2[8] = new SqlParameter("@RowTotal", ticketRow.RowTotal);
            parameterArray2[9] = new SqlParameter("@ServiceDisplayText", ticketRow.ServiceDisplayText);
            Guid? serviceId = ticketRow.ServiceId;
            parameterArray2[10] = new SqlParameter("@ServiceId", serviceId.HasValue ? ((SqlGuid) serviceId.GetValueOrDefault()) : SqlGuid.Null);
            DateTime? startDate = ticketRow.StartDate;
            parameterArray2[11] = new SqlParameter("@StartDate", startDate.HasValue ? ((SqlDateTime) startDate.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[12] = new SqlParameter("@StartTime", ticketRow.StartTime ?? DBNull.Value);
            parameterArray2[13] = new SqlParameter("@Taxable", ticketRow.Taxable);
            parameterArray2[14] = new SqlParameter("@TaxRate", ticketRow.TaxRate);
            parameterArray2[15] = new SqlParameter("@TicketId", ticketRow.TicketId);
            parameterArray2[0x10] = new SqlParameter("@TicketRowType", ticketRow.TicketRowType);
            parameterArray2[0x11] = new SqlParameter("@TotalTax", ticketRow.TotalTax);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TicketRowInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid ticketRowId = new Guid(parameter.Value.ToString());
            ticketRow = this.GetTicketRowById(ticketRowId);
            return ticketRow;
        }

        private OpenTicketDB OpenTicketMappingDB(SqlDataReader reader) => 
            new OpenTicketDB { 
                CustomerDisplayText = reader.GetString(reader.GetOrdinal("CustomerDisplayText")),
                OpenedOnUtc = reader.GetValue(reader.GetOrdinal("OpenedOnUtc")) as DateTime?,
                OpenUserDisplayText = reader.GetString(reader.GetOrdinal("OpenUserDisplayText")),
                OpenUserId = reader.GetValue(reader.GetOrdinal("OpenUserId")) as Guid?,
                RowTotal = reader.GetDecimal(reader.GetOrdinal("RowTotal")),
                ServiceDisplayText = reader.GetString(reader.GetOrdinal("ServiceDisplayText")),
                StartDate = reader.GetValue(reader.GetOrdinal("StartDate")) as DateTime?,
                StartTime = reader.GetValue(reader.GetOrdinal("StartTime")) as TimeSpan?,
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                TicketId = reader.GetGuid(reader.GetOrdinal("TicketId"))
            };

        private TicketAlertDB TicketAlertMappingDB(SqlDataReader reader) => 
            new TicketAlertDB { 
                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                AlertId = reader.GetGuid(reader.GetOrdinal("AlertId")),
                ByEmail = reader.GetBoolean(reader.GetOrdinal("ByEmail")),
                BySMS = reader.GetBoolean(reader.GetOrdinal("BySMS")),
                DisplayText = reader.GetString(reader.GetOrdinal("DisplayText")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Mobile = reader.GetString(reader.GetOrdinal("Mobile")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId"))
            };

        private TicketRowDB TicketRowMappingDB(SqlDataReader reader) => 
            new TicketRowDB { 
                EmployeeDisplayText = reader.GetString(reader.GetOrdinal("EmployeeDisplayText")),
                EmployeeId = reader.GetValue(reader.GetOrdinal("EmployeeId")) as Guid?,
                EndDate = reader.GetValue(reader.GetOrdinal("EndDate")) as DateTime?,
                EndTime = reader.GetValue(reader.GetOrdinal("EndTime")) as TimeSpan?,
                MultiDay = reader.GetBoolean(reader.GetOrdinal("MultiDay")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                RowOrder = reader.GetInt32(reader.GetOrdinal("RowOrder")),
                RowTotal = reader.GetDecimal(reader.GetOrdinal("RowTotal")),
                ServiceDisplayText = reader.GetString(reader.GetOrdinal("ServiceDisplayText")),
                ServiceId = reader.GetValue(reader.GetOrdinal("ServiceId")) as Guid?,
                StartDate = reader.GetValue(reader.GetOrdinal("StartDate")) as DateTime?,
                StartTime = reader.GetValue(reader.GetOrdinal("StartTime")) as TimeSpan?,
                Taxable = reader.GetBoolean(reader.GetOrdinal("Taxable")),
                TaxRate = reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                TicketId = reader.GetGuid(reader.GetOrdinal("TicketId")),
                TicketRowId = reader.GetGuid(reader.GetOrdinal("TicketRowId")),
                TicketRowType = reader.GetInt32(reader.GetOrdinal("TicketRowType")),
                TotalTax = reader.GetDecimal(reader.GetOrdinal("TotalTax"))
            };

        private TicketSummaryDB TicketSummaryMappingDB(SqlDataReader reader) => 
            new TicketSummaryDB { 
                AdminComment = reader.GetString(reader.GetOrdinal("AdminComment")),
                AuthorizationTransactionCode = reader.GetString(reader.GetOrdinal("AuthorizationTransactionCode")),
                AuthorizationTransactionId = reader.GetString(reader.GetOrdinal("AuthorizationTransactionId")),
                AuthorizationTransactionResult = reader.GetString(reader.GetOrdinal("AuthorizationTransactionResult")),
                BillingDisplayText = reader.GetString(reader.GetOrdinal("BillingDisplayText")),
                BillingEmail = reader.GetString(reader.GetOrdinal("BillingEmail")),
                BillingFirstName = reader.GetString(reader.GetOrdinal("BillingFirstName")),
                BillingLastName = reader.GetString(reader.GetOrdinal("BillingLastName")),
                BillingMobile = reader.GetString(reader.GetOrdinal("BillingMobile")),
                BillingPhone = reader.GetString(reader.GetOrdinal("BillingPhone")),
                BookedOnMobile = reader.GetBoolean(reader.GetOrdinal("BookedOnMobile")),
                BookedOnWeb = reader.GetBoolean(reader.GetOrdinal("BookedOnWeb")),
                BookedOnWidget = reader.GetBoolean(reader.GetOrdinal("BookedOnWidget")),
                CancelledOnUtc = reader.GetValue(reader.GetOrdinal("CancelledOnUtc")) as DateTime?,
                CaptureTransactionId = reader.GetString(reader.GetOrdinal("CaptureTransactionId")),
                CaptureTransactionResult = reader.GetString(reader.GetOrdinal("CaptureTransactionResult")),
                CardExpirationMonth = reader.GetValue(reader.GetOrdinal("CardExpirationMonth")) as int?,
                CardExpirationYear = reader.GetValue(reader.GetOrdinal("CardExpirationYear")) as int?,
                CardName = reader.GetString(reader.GetOrdinal("CardName")),
                CardNumberMasked = reader.GetString(reader.GetOrdinal("CardNumberMasked")),
                CardType = reader.GetString(reader.GetOrdinal("CardType")),
                ClosedOnUtc = reader.GetValue(reader.GetOrdinal("ClosedOnUtc")) as DateTime?,
                Confirmed = reader.GetBoolean(reader.GetOrdinal("Confirmed")),
                CreatedOnUtc = reader.GetDateTime(reader.GetOrdinal("CreatedOnUtc")),
                CurrencyCode = reader.GetString(reader.GetOrdinal("CurrencyCode")),
                CurrencyRate = reader.GetDecimal(reader.GetOrdinal("CurrencyRate")),
                CustomerDisplayText = reader.GetString(reader.GetOrdinal("CustomerDisplayText")),
                CustomerEmail = reader.GetString(reader.GetOrdinal("CustomerEmail")),
                CustomerFirstName = reader.GetString(reader.GetOrdinal("CustomerFirstName")),
                CustomerIPAddress = reader.GetString(reader.GetOrdinal("CustomerIPAddress")),
                CustomerLastName = reader.GetString(reader.GetOrdinal("CustomerLastName")),
                CustomerMobile = reader.GetString(reader.GetOrdinal("CustomerMobile")),
                CustomerPhone = reader.GetString(reader.GetOrdinal("CustomerPhone")),
                CustomerReminder = reader.GetValue(reader.GetOrdinal("CustomerReminder")) as DateTime?,
                CustomerSpecialRequest = reader.GetString(reader.GetOrdinal("CustomerSpecialRequest")),
                Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted")),
                DepositRate = reader.GetDecimal(reader.GetOrdinal("DepositRate")),
                DepositRequired = reader.GetBoolean(reader.GetOrdinal("DepositRequired")),
                Language = reader.GetString(reader.GetOrdinal("Language")),
                OpenedOnUtc = reader.GetValue(reader.GetOrdinal("OpenedOnUtc")) as DateTime?,
                OpenUserDisplayText = reader.GetString(reader.GetOrdinal("OpenUserDisplayText")),
                OpenUserId = reader.GetValue(reader.GetOrdinal("OpenUserId")) as Guid?,
                PaidAmount = reader.GetDecimal(reader.GetOrdinal("PaidAmount")),
                PaidAmountInCustomerCurrency = reader.GetDecimal(reader.GetOrdinal("PaidAmountInCustomerCurrency")),
                PaidDateUtc = reader.GetValue(reader.GetOrdinal("PaidDateUtc")) as DateTime?,
                RepeatCustomer = reader.GetValue(reader.GetOrdinal("RepeatCustomer")) as bool?,
                SalonDisplayText = reader.GetString(reader.GetOrdinal("SalonDisplayText")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                TaxRate = reader.GetDecimal(reader.GetOrdinal("TaxRate")),
                TicketId = reader.GetGuid(reader.GetOrdinal("TicketId")),
                TicketNumber = reader.GetString(reader.GetOrdinal("TicketNumber")),
                TicketStatusType = reader.GetValue(reader.GetOrdinal("TicketStatusType")) as int?,
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                TotalTax = reader.GetDecimal(reader.GetOrdinal("TotalTax")),
                UserId = reader.GetValue(reader.GetOrdinal("UserId")) as Guid?,
                VoidedOnUtc = reader.GetValue(reader.GetOrdinal("VoidedOnUtc")) as DateTime?,
                VoidUserDisplayText = reader.GetString(reader.GetOrdinal("VoidUserDisplayText")),
                VoidUserId = reader.GetValue(reader.GetOrdinal("VoidUserId")) as Guid?
            };

        public TicketSummaryDB UpdateTicket(TicketSummaryDB ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("ticket");
            }
            ticket.AdminComment = ticket.AdminComment ?? string.Empty;
            ticket.AuthorizationTransactionCode = ticket.AuthorizationTransactionCode ?? string.Empty;
            ticket.AuthorizationTransactionId = ticket.AuthorizationTransactionId ?? string.Empty;
            ticket.AuthorizationTransactionResult = ticket.AuthorizationTransactionResult ?? string.Empty;
            ticket.BillingDisplayText = ticket.BillingDisplayText ?? string.Empty;
            ticket.BillingEmail = ticket.BillingEmail ?? string.Empty;
            ticket.BillingFirstName = ticket.BillingFirstName ?? string.Empty;
            ticket.BillingLastName = ticket.BillingLastName ?? string.Empty;
            ticket.BillingMobile = ticket.BillingMobile ?? string.Empty;
            ticket.BillingPhone = ticket.BillingPhone ?? string.Empty;
            ticket.CaptureTransactionId = ticket.CaptureTransactionId ?? string.Empty;
            ticket.CaptureTransactionResult = ticket.CaptureTransactionResult ?? string.Empty;
            ticket.CardName = ticket.CardName ?? string.Empty;
            ticket.CardNumberMasked = ticket.CardNumberMasked ?? string.Empty;
            ticket.CardType = ticket.CardType ?? string.Empty;
            ticket.CurrencyCode = ticket.CurrencyCode ?? string.Empty;
            ticket.CustomerDisplayText = ticket.CustomerDisplayText ?? string.Empty;
            ticket.CustomerEmail = ticket.CustomerEmail ?? string.Empty;
            ticket.CustomerFirstName = ticket.CustomerFirstName ?? string.Empty;
            ticket.CustomerIPAddress = ticket.CustomerIPAddress ?? string.Empty;
            ticket.CustomerLastName = ticket.CustomerLastName ?? string.Empty;
            ticket.CustomerMobile = ticket.CustomerMobile ?? string.Empty;
            ticket.CustomerPhone = ticket.CustomerPhone ?? string.Empty;
            ticket.CustomerSpecialRequest = ticket.CustomerSpecialRequest ?? string.Empty;
            ticket.Language = ticket.Language ?? string.Empty;
            ticket.OpenUserDisplayText = ticket.OpenUserDisplayText ?? string.Empty;
            ticket.SalonDisplayText = ticket.SalonDisplayText ?? string.Empty;
            ticket.TicketNumber = ticket.TicketNumber ?? string.Empty;
            ticket.VoidUserDisplayText = ticket.VoidUserDisplayText ?? string.Empty;
            SqlParameter[] parameterArray2 = new SqlParameter[0x3b];
            parameterArray2[0] = new SqlParameter("@TicketId", ticket.TicketId);
            parameterArray2[1] = new SqlParameter("@AdminComment", ticket.AdminComment);
            parameterArray2[2] = new SqlParameter("@AuthorizationTransactionCode", ticket.AuthorizationTransactionCode);
            parameterArray2[3] = new SqlParameter("@AuthorizationTransactionId", ticket.AuthorizationTransactionId);
            parameterArray2[4] = new SqlParameter("@AuthorizationTransactionResult", ticket.AuthorizationTransactionResult);
            parameterArray2[5] = new SqlParameter("@BillingDisplayText", ticket.BillingDisplayText);
            parameterArray2[6] = new SqlParameter("@BillingEmail", ticket.BillingEmail);
            parameterArray2[7] = new SqlParameter("@BillingFirstName", ticket.BillingFirstName);
            parameterArray2[8] = new SqlParameter("@BillingLastName", ticket.BillingLastName);
            parameterArray2[9] = new SqlParameter("@BillingMobile", ticket.BillingMobile);
            parameterArray2[10] = new SqlParameter("@BillingPhone", ticket.BillingPhone);
            parameterArray2[11] = new SqlParameter("@BookedOnMobile", ticket.BookedOnMobile);
            parameterArray2[12] = new SqlParameter("@BookedOnWeb", ticket.BookedOnWeb);
            parameterArray2[13] = new SqlParameter("@BookedOnWidget", ticket.BookedOnWidget);
            DateTime? cancelledOnUtc = ticket.CancelledOnUtc;
            parameterArray2[14] = new SqlParameter("@CancelledOnUtc", cancelledOnUtc.HasValue ? ((SqlDateTime) cancelledOnUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[15] = new SqlParameter("@CaptureTransactionId", ticket.CaptureTransactionId);
            parameterArray2[0x10] = new SqlParameter("@CaptureTransactionResult", ticket.CaptureTransactionResult);
            int? cardExpirationMonth = ticket.CardExpirationMonth;
            parameterArray2[0x11] = new SqlParameter("@CardExpirationMonth", cardExpirationMonth.HasValue ? ((SqlInt32) cardExpirationMonth.GetValueOrDefault()) : SqlInt32.Null);
            int? cardExpirationYear = ticket.CardExpirationYear;
            parameterArray2[0x12] = new SqlParameter("@CardExpirationYear", cardExpirationYear.HasValue ? ((SqlInt32) cardExpirationYear.GetValueOrDefault()) : SqlInt32.Null);
            parameterArray2[0x13] = new SqlParameter("@CardName", ticket.CardName);
            parameterArray2[20] = new SqlParameter("@CardNumberMasked", ticket.CardNumberMasked);
            parameterArray2[0x15] = new SqlParameter("@CardType", ticket.CardType);
            DateTime? closedOnUtc = ticket.ClosedOnUtc;
            parameterArray2[0x16] = new SqlParameter("@ClosedOnUtc", closedOnUtc.HasValue ? ((SqlDateTime) closedOnUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x17] = new SqlParameter("@Confirmed", ticket.Confirmed);
            DateTime? openedOnUtc = ticket.OpenedOnUtc;
            parameterArray2[0x18] = new SqlParameter("@OpenedOnUtc", openedOnUtc.HasValue ? ((SqlDateTime) openedOnUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x19] = new SqlParameter("@OpenUserDisplayText", ticket.OpenUserDisplayText);
            Guid? openUserId = ticket.OpenUserId;
            parameterArray2[0x1a] = new SqlParameter("@OpenUserId", openUserId.HasValue ? ((SqlGuid) openUserId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[0x1b] = new SqlParameter("@CreatedOnUtc", ticket.CreatedOnUtc);
            parameterArray2[0x1c] = new SqlParameter("@CurrencyCode", ticket.CurrencyCode);
            parameterArray2[0x1d] = new SqlParameter("@CurrencyRate", ticket.CurrencyRate);
            parameterArray2[30] = new SqlParameter("@CustomerDisplayText", ticket.CustomerDisplayText);
            parameterArray2[0x1f] = new SqlParameter("@CustomerEmail", ticket.CustomerEmail);
            parameterArray2[0x20] = new SqlParameter("@CustomerFirstName", ticket.CustomerFirstName);
            parameterArray2[0x21] = new SqlParameter("@CustomerIPAddress", ticket.CustomerIPAddress);
            parameterArray2[0x22] = new SqlParameter("@CustomerLastName", ticket.CustomerLastName);
            parameterArray2[0x23] = new SqlParameter("@CustomerMobile", ticket.CustomerMobile);
            parameterArray2[0x24] = new SqlParameter("@CustomerPhone", ticket.CustomerPhone);
            parameterArray2[0x25] = new SqlParameter("@CustomerReminder", ticket.CustomerReminder);
            parameterArray2[0x26] = new SqlParameter("@CustomerSpecialRequest", ticket.CustomerSpecialRequest);
            parameterArray2[0x27] = new SqlParameter("@Deleted", ticket.Deleted);
            parameterArray2[40] = new SqlParameter("@DepositRate", ticket.DepositRate);
            parameterArray2[0x29] = new SqlParameter("@DepositRequired", ticket.DepositRequired);
            parameterArray2[0x2a] = new SqlParameter("@Language", ticket.Language);
            parameterArray2[0x2b] = new SqlParameter("@PaidAmount", ticket.PaidAmount);
            parameterArray2[0x2c] = new SqlParameter("@PaidAmountInCustomerCurrency", ticket.PaidAmountInCustomerCurrency);
            DateTime? paidDateUtc = ticket.PaidDateUtc;
            parameterArray2[0x2d] = new SqlParameter("@PaidDateUtc", paidDateUtc.HasValue ? ((SqlDateTime) paidDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x2e] = new SqlParameter("@RepeatCustomer", ticket.RepeatCustomer);
            parameterArray2[0x2f] = new SqlParameter("@SalonDisplayText", ticket.SalonDisplayText);
            parameterArray2[0x30] = new SqlParameter("@SalonId", ticket.SalonId);
            parameterArray2[0x31] = new SqlParameter("@Subtotal", ticket.Subtotal);
            parameterArray2[50] = new SqlParameter("@TaxRate", ticket.TaxRate);
            parameterArray2[0x33] = new SqlParameter("@TicketNumber", ticket.TicketNumber);
            int? ticketStatusType = ticket.TicketStatusType;
            parameterArray2[0x34] = new SqlParameter("@TicketStatusType", ticketStatusType.HasValue ? ((SqlInt32) ticketStatusType.GetValueOrDefault()) : SqlInt32.Null);
            parameterArray2[0x35] = new SqlParameter("@Total", ticket.Total);
            parameterArray2[0x36] = new SqlParameter("@TotalTax", ticket.TotalTax);
            Guid? userId = ticket.UserId;
            parameterArray2[0x37] = new SqlParameter("@UserId", userId.HasValue ? ((SqlGuid) userId.GetValueOrDefault()) : SqlGuid.Null);
            DateTime? voidedOnUtc = ticket.VoidedOnUtc;
            parameterArray2[0x38] = new SqlParameter("@VoidedOnUtc", voidedOnUtc.HasValue ? ((SqlDateTime) voidedOnUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x39] = new SqlParameter("@VoidUserDisplayText", ticket.VoidUserDisplayText);
            Guid? voidUserId = ticket.VoidUserId;
            parameterArray2[0x3a] = new SqlParameter("@VoidUserId", voidUserId.HasValue ? ((SqlGuid) voidUserId.GetValueOrDefault()) : SqlGuid.Null);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TicketSummaryUpdate", commandParameters) <= 0)
            {
                return null;
            }
            ticket = this.GetTicketById(ticket.TicketId);
            return ticket;
        }

        public TicketAlertDB UpdateTicketAlert(TicketAlertDB alert)
        {
            if (alert == null)
            {
                throw new ArgumentNullException("alert");
            }
            alert.Email = alert.Email ?? string.Empty;
            alert.DisplayText = alert.DisplayText ?? string.Empty;
            alert.FirstName = alert.FirstName ?? string.Empty;
            alert.LastName = alert.LastName ?? string.Empty;
            alert.Mobile = alert.Mobile ?? string.Empty;
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@AlertId", alert.AlertId), new SqlParameter("@Active", alert.Active), new SqlParameter("@ByEmail", alert.ByEmail), new SqlParameter("@BySMS", alert.BySMS), new SqlParameter("@DisplayText", alert.DisplayText), new SqlParameter("@Email", alert.Email), new SqlParameter("@FirstName", alert.FirstName), new SqlParameter("@LastName", alert.LastName), new SqlParameter("@Mobile", alert.Mobile), new SqlParameter("@SalonId", alert.SalonId) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TicketAlertUpdate", commandParameters) <= 0)
            {
                return null;
            }
            alert = this.GetTicketAlertById(alert.AlertId);
            return alert;
        }
    }
}

