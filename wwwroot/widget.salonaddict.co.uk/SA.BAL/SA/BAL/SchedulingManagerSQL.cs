namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;

    public class SchedulingManagerSQL : ISchedulingManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public SchedulingManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        public void DeleteSchedule(ScheduleDB schedule)
        {
            if (schedule == null)
            {
                throw new ArgumentNullException("schedule");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@ScheduleId", schedule.ScheduleId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SchedulingDeleteById", commandParameters);
            string key = $"SCHEDULING-{"SALON-" + schedule.SalonId}";
            this._cacheManager.Remove(key);
        }

        public void DeleteTimeBlock(TimeBlockDB block)
        {
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@BlockId", block.BlockId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TimeBlockDeleteById", commandParameters);
            string key = $"TIMEBLOCK-{"SALON-" + block.SalonId}";
            this._cacheManager.Remove(key);
        }

        public void DeleteTimeBlocksByEmployee(EmployeeDB employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@EmployeeId", employee.EmployeeId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TimeBlockDeleteByEmployeeId", commandParameters);
            string key = $"TIMEBLOCK-{"SALON-" + employee.SalonId}";
            this._cacheManager.Remove(key);
        }

        public void DeleteTimeBlocksByTicket(TicketSummaryDB ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("ticket");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@TicketId", ticket.TicketId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TimeBlockDeleteByTicketId", commandParameters);
            string key = $"TIMEBLOCK-{"SALON-" + ticket.SalonId}";
            this._cacheManager.Remove(key);
        }

        public ScheduleDB GetScheduleById(Guid scheduleId)
        {
            ScheduleDB edb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@ScheduleId", scheduleId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SchedulingLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    edb = this.ScheduleMappingDB(reader);
                }
            }
            return edb;
        }

        public List<ScheduleDB> GetSchedulingBySalonId(Guid salonId)
        {
            string key = $"SCHEDULING-{"SALON-" + salonId}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<ScheduleDB>) obj2;
            }
            List<ScheduleDB> list = new List<ScheduleDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SchedulingLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    ScheduleDB item = this.ScheduleMappingDB(reader);
                    list.Add(item);
                }
            }
            this._cacheManager.Add(key, list);
            return list;
        }

        public TimeBlockDB GetTimeBlockById(Guid blockId)
        {
            TimeBlockDB kdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@BlockId", blockId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_TimeBlockLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    kdb = this.TimeBlockMappingDB(reader);
                }
            }
            return kdb;
        }

        public List<TimeBlockDB> GetTimeBlocksBySalonId(Guid salonId)
        {
            string key = $"TIMEBLOCK-{"SALON-" + salonId}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<TimeBlockDB>) obj2;
            }
            List<TimeBlockDB> list = new List<TimeBlockDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_TimeBlockLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    TimeBlockDB item = this.TimeBlockMappingDB(reader);
                    list.Add(item);
                }
            }
            this._cacheManager.Add(key, list);
            return list;
        }

        public ScheduleDB InsertSchedule(ScheduleDB schedule)
        {
            if (schedule == null)
            {
                throw new ArgumentNullException("schedule");
            }
            SqlParameter parameter = new SqlParameter("@ScheduleId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[20];
            parameterArray2[0] = parameter;
            DateTime? date = schedule.Date;
            parameterArray2[1] = new SqlParameter("@Date", date.HasValue ? ((SqlDateTime) date.GetValueOrDefault()) : SqlDateTime.Null);
            Guid? employeeId = schedule.EmployeeId;
            parameterArray2[2] = new SqlParameter("@EmployeeId", employeeId.HasValue ? ((SqlGuid) employeeId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[3] = new SqlParameter("@End1", schedule.End1 ?? DBNull.Value);
            parameterArray2[4] = new SqlParameter("@End2", schedule.End2 ?? DBNull.Value);
            parameterArray2[5] = new SqlParameter("@End3", schedule.End3 ?? DBNull.Value);
            parameterArray2[6] = new SqlParameter("@End4", schedule.End4 ?? DBNull.Value);
            parameterArray2[7] = new SqlParameter("@End5", schedule.End5 ?? DBNull.Value);
            parameterArray2[8] = new SqlParameter("@End6", schedule.End6 ?? DBNull.Value);
            parameterArray2[9] = new SqlParameter("@SalonId", schedule.SalonId);
            parameterArray2[10] = new SqlParameter("@ScheduleType", schedule.ScheduleType);
            int? slots = schedule.Slots;
            parameterArray2[11] = new SqlParameter("@Slots", slots.HasValue ? ((SqlInt32) slots.GetValueOrDefault()) : SqlInt32.Null);
            parameterArray2[12] = new SqlParameter("@Start1", schedule.Start1 ?? DBNull.Value);
            parameterArray2[13] = new SqlParameter("@Start2", schedule.Start2 ?? DBNull.Value);
            parameterArray2[14] = new SqlParameter("@Start3", schedule.Start3 ?? DBNull.Value);
            parameterArray2[15] = new SqlParameter("@Start4", schedule.Start4 ?? DBNull.Value);
            parameterArray2[0x10] = new SqlParameter("@Start5", schedule.Start5 ?? DBNull.Value);
            parameterArray2[0x11] = new SqlParameter("@Start6", schedule.Start6 ?? DBNull.Value);
            parameterArray2[0x12] = new SqlParameter("@Time", schedule.Time ?? DBNull.Value);
            int? weekDay = schedule.WeekDay;
            parameterArray2[0x13] = new SqlParameter("@WeekDay", weekDay.HasValue ? ((SqlInt32) weekDay.GetValueOrDefault()) : SqlInt32.Null);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SchedulingInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid scheduleId = new Guid(parameter.Value.ToString());
            string key = $"SCHEDULING-{"SALON-" + schedule.SalonId}";
            this._cacheManager.Remove(key);
            schedule = this.GetScheduleById(scheduleId);
            return schedule;
        }

        public TimeBlockDB InsertTimeBlock(TimeBlockDB block)
        {
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            if (block.BlockTypeId < 1)
            {
                throw new ArgumentOutOfRangeException("BlockTypeId");
            }
            block.EmployeeDisplayText = block.EmployeeDisplayText ?? string.Empty;
            block.ServiceDisplayText = block.ServiceDisplayText ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@BlockId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[0x11];
            parameterArray2[0] = parameter;
            parameterArray2[1] = new SqlParameter("@BlockTypeId", block.BlockTypeId);
            parameterArray2[2] = new SqlParameter("@CycleLength", block.CycleLength);
            parameterArray2[3] = new SqlParameter("@CyclePeriodType", block.CyclePeriodType);
            parameterArray2[4] = new SqlParameter("@EmployeeDisplayText", block.EmployeeDisplayText);
            Guid? employeeId = block.EmployeeId;
            parameterArray2[5] = new SqlParameter("@EmployeeId", employeeId.HasValue ? ((SqlGuid) employeeId.GetValueOrDefault()) : SqlGuid.Null);
            DateTime? endDateUtc = block.EndDateUtc;
            parameterArray2[6] = new SqlParameter("@EndDateUtc", endDateUtc.HasValue ? ((SqlDateTime) endDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[7] = new SqlParameter("@EndTime", block.EndTime);
            parameterArray2[8] = new SqlParameter("@SalonId", block.SalonId);
            parameterArray2[9] = new SqlParameter("@ServiceDisplayText", block.ServiceDisplayText);
            Guid? serviceId = block.ServiceId;
            parameterArray2[10] = new SqlParameter("@ServiceId", serviceId.HasValue ? ((SqlGuid) serviceId.GetValueOrDefault()) : SqlGuid.Null);
            int? slots = block.Slots;
            parameterArray2[11] = new SqlParameter("@Slots", slots.HasValue ? ((SqlInt32) slots.GetValueOrDefault()) : SqlInt32.Null);
            parameterArray2[12] = new SqlParameter("@StartDateUtc", block.StartDateUtc);
            parameterArray2[13] = new SqlParameter("@StartTime", block.StartTime);
            Guid? ticketId = block.TicketId;
            parameterArray2[14] = new SqlParameter("@TicketId", ticketId.HasValue ? ((SqlGuid) ticketId.GetValueOrDefault()) : SqlGuid.Null);
            int? totalCycles = block.TotalCycles;
            parameterArray2[15] = new SqlParameter("@TotalCycles", totalCycles.HasValue ? ((SqlInt32) totalCycles.GetValueOrDefault()) : SqlInt32.Null);
            int? weekDay = block.WeekDay;
            parameterArray2[0x10] = new SqlParameter("@WeekDay", weekDay.HasValue ? ((SqlInt32) weekDay.GetValueOrDefault()) : SqlInt32.Null);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TimeBlockInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid blockId = new Guid(parameter.Value.ToString());
            string key = $"TIMEBLOCK-{"SALON-" + block.SalonId}";
            this._cacheManager.Remove(key);
            block = this.GetTimeBlockById(blockId);
            return block;
        }

        private ScheduleDB ScheduleMappingDB(SqlDataReader reader) => 
            new ScheduleDB { 
                Date = reader.GetValue(reader.GetOrdinal("Date")) as DateTime?,
                EmployeeId = reader.GetValue(reader.GetOrdinal("EmployeeId")) as Guid?,
                End1 = reader.GetValue(reader.GetOrdinal("End1")) as TimeSpan?,
                End2 = reader.GetValue(reader.GetOrdinal("End2")) as TimeSpan?,
                End3 = reader.GetValue(reader.GetOrdinal("End3")) as TimeSpan?,
                End4 = reader.GetValue(reader.GetOrdinal("End4")) as TimeSpan?,
                End5 = reader.GetValue(reader.GetOrdinal("End5")) as TimeSpan?,
                End6 = reader.GetValue(reader.GetOrdinal("End6")) as TimeSpan?,
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                ScheduleId = reader.GetGuid(reader.GetOrdinal("ScheduleId")),
                ScheduleType = reader.GetInt32(reader.GetOrdinal("ScheduleType")),
                Slots = reader.GetValue(reader.GetOrdinal("Slots")) as int?,
                Start1 = reader.GetValue(reader.GetOrdinal("Start1")) as TimeSpan?,
                Start2 = reader.GetValue(reader.GetOrdinal("Start2")) as TimeSpan?,
                Start3 = reader.GetValue(reader.GetOrdinal("Start3")) as TimeSpan?,
                Start4 = reader.GetValue(reader.GetOrdinal("Start4")) as TimeSpan?,
                Start5 = reader.GetValue(reader.GetOrdinal("Start5")) as TimeSpan?,
                Start6 = reader.GetValue(reader.GetOrdinal("Start6")) as TimeSpan?,
                Time = reader.GetValue(reader.GetOrdinal("Time")) as TimeSpan?,
                WeekDay = reader.GetValue(reader.GetOrdinal("WeekDay")) as int?
            };

        private TimeBlockDB TimeBlockMappingDB(SqlDataReader reader) => 
            new TimeBlockDB { 
                BlockId = reader.GetGuid(reader.GetOrdinal("BlockId")),
                BlockTypeId = reader.GetInt32(reader.GetOrdinal("BlockTypeId")),
                CycleLength = reader.GetInt32(reader.GetOrdinal("CycleLength")),
                CyclePeriodType = reader.GetInt32(reader.GetOrdinal("CyclePeriodType")),
                EmployeeDisplayText = reader.GetString(reader.GetOrdinal("EmployeeDisplayText")),
                EmployeeId = reader.GetValue(reader.GetOrdinal("EmployeeId")) as Guid?,
                EndDateUtc = reader.GetValue(reader.GetOrdinal("EndDateUtc")) as DateTime?,
                EndTime = reader.GetTimeSpan(reader.GetOrdinal("EndTime")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                ServiceDisplayText = reader.GetString(reader.GetOrdinal("ServiceDisplayText")),
                ServiceId = reader.GetValue(reader.GetOrdinal("ServiceId")) as Guid?,
                Slots = reader.GetValue(reader.GetOrdinal("Slots")) as int?,
                StartDateUtc = reader.GetDateTime(reader.GetOrdinal("StartDateUtc")),
                StartTime = reader.GetTimeSpan(reader.GetOrdinal("StartTime")),
                TicketId = reader.GetValue(reader.GetOrdinal("TicketId")) as Guid?,
                TotalCycles = reader.GetValue(reader.GetOrdinal("TotalCycles")) as int?,
                WeekDay = reader.GetValue(reader.GetOrdinal("WeekDay")) as int?
            };

        public ScheduleDB UpdateSchedule(ScheduleDB schedule)
        {
            if (schedule == null)
            {
                throw new ArgumentNullException("schedule");
            }
            SqlParameter[] parameterArray2 = new SqlParameter[20];
            parameterArray2[0] = new SqlParameter("@ScheduleId", schedule.ScheduleId);
            DateTime? date = schedule.Date;
            parameterArray2[1] = new SqlParameter("@Date", date.HasValue ? ((SqlDateTime) date.GetValueOrDefault()) : SqlDateTime.Null);
            Guid? employeeId = schedule.EmployeeId;
            parameterArray2[2] = new SqlParameter("@EmployeeId", employeeId.HasValue ? ((SqlGuid) employeeId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[3] = new SqlParameter("@End1", schedule.End1 ?? DBNull.Value);
            parameterArray2[4] = new SqlParameter("@End2", schedule.End2 ?? DBNull.Value);
            parameterArray2[5] = new SqlParameter("@End3", schedule.End3 ?? DBNull.Value);
            parameterArray2[6] = new SqlParameter("@End4", schedule.End4 ?? DBNull.Value);
            parameterArray2[7] = new SqlParameter("@End5", schedule.End5 ?? DBNull.Value);
            parameterArray2[8] = new SqlParameter("@End6", schedule.End6 ?? DBNull.Value);
            parameterArray2[9] = new SqlParameter("@SalonId", schedule.SalonId);
            parameterArray2[10] = new SqlParameter("@ScheduleType", schedule.ScheduleType);
            int? slots = schedule.Slots;
            parameterArray2[11] = new SqlParameter("@Slots", slots.HasValue ? ((SqlInt32) slots.GetValueOrDefault()) : SqlInt32.Null);
            parameterArray2[12] = new SqlParameter("@Start1", schedule.Start1 ?? DBNull.Value);
            parameterArray2[13] = new SqlParameter("@Start2", schedule.Start2 ?? DBNull.Value);
            parameterArray2[14] = new SqlParameter("@Start3", schedule.Start3 ?? DBNull.Value);
            parameterArray2[15] = new SqlParameter("@Start4", schedule.Start4 ?? DBNull.Value);
            parameterArray2[0x10] = new SqlParameter("@Start5", schedule.Start5 ?? DBNull.Value);
            parameterArray2[0x11] = new SqlParameter("@Start6", schedule.Start6 ?? DBNull.Value);
            parameterArray2[0x12] = new SqlParameter("@Time", schedule.Time ?? DBNull.Value);
            int? weekDay = schedule.WeekDay;
            parameterArray2[0x13] = new SqlParameter("@WeekDay", weekDay.HasValue ? ((SqlInt32) weekDay.GetValueOrDefault()) : SqlInt32.Null);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SchedulingUpdate", commandParameters) <= 0)
            {
                return null;
            }
            string key = $"SCHEDULING-{"SALON-" + schedule.SalonId}";
            this._cacheManager.Remove(key);
            schedule = this.GetScheduleById(schedule.ScheduleId);
            return schedule;
        }

        public TimeBlockDB UpdateTimeBlock(TimeBlockDB block)
        {
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }
            if (block.BlockTypeId < 1)
            {
                throw new ArgumentOutOfRangeException("BlockTypeId");
            }
            block.EmployeeDisplayText = block.EmployeeDisplayText ?? string.Empty;
            block.ServiceDisplayText = block.ServiceDisplayText ?? string.Empty;
            SqlParameter[] parameterArray2 = new SqlParameter[0x11];
            parameterArray2[0] = new SqlParameter("@BlockId", block.BlockId);
            parameterArray2[1] = new SqlParameter("@BlockTypeId", block.BlockTypeId);
            parameterArray2[2] = new SqlParameter("@CycleLength", block.CycleLength);
            parameterArray2[3] = new SqlParameter("@CyclePeriodType", block.CyclePeriodType);
            parameterArray2[4] = new SqlParameter("@EmployeeDisplayText", block.EmployeeDisplayText);
            Guid? employeeId = block.EmployeeId;
            parameterArray2[5] = new SqlParameter("@EmployeeId", employeeId.HasValue ? ((SqlGuid) employeeId.GetValueOrDefault()) : SqlGuid.Null);
            DateTime? endDateUtc = block.EndDateUtc;
            parameterArray2[6] = new SqlParameter("@EndDateUtc", endDateUtc.HasValue ? ((SqlDateTime) endDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[7] = new SqlParameter("@EndTime", block.EndTime);
            parameterArray2[8] = new SqlParameter("@SalonId", block.SalonId);
            parameterArray2[9] = new SqlParameter("@ServiceDisplayText", block.ServiceDisplayText);
            Guid? serviceId = block.ServiceId;
            parameterArray2[10] = new SqlParameter("@ServiceId", serviceId.HasValue ? ((SqlGuid) serviceId.GetValueOrDefault()) : SqlGuid.Null);
            int? slots = block.Slots;
            parameterArray2[11] = new SqlParameter("@Slots", slots.HasValue ? ((SqlInt32) slots.GetValueOrDefault()) : SqlInt32.Null);
            parameterArray2[12] = new SqlParameter("@StartDateUtc", block.StartDateUtc);
            parameterArray2[13] = new SqlParameter("@StartTime", block.StartTime);
            Guid? ticketId = block.TicketId;
            parameterArray2[14] = new SqlParameter("@TicketId", ticketId.HasValue ? ((SqlGuid) ticketId.GetValueOrDefault()) : SqlGuid.Null);
            int? totalCycles = block.TotalCycles;
            parameterArray2[15] = new SqlParameter("@TotalCycles", totalCycles.HasValue ? ((SqlInt32) totalCycles.GetValueOrDefault()) : SqlInt32.Null);
            int? weekDay = block.WeekDay;
            parameterArray2[0x10] = new SqlParameter("@WeekDay", weekDay.HasValue ? ((SqlInt32) weekDay.GetValueOrDefault()) : SqlInt32.Null);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_TimeBlockUpdate", commandParameters) <= 0)
            {
                return null;
            }
            string key = $"TIMEBLOCK-{"SALON-" + block.SalonId}";
            this._cacheManager.Remove(key);
            block = this.GetTimeBlockById(block.BlockId);
            return block;
        }
    }
}

