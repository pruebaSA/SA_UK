namespace SA.BAL
{
    using System;
    using System.Collections.Generic;

    public interface ISchedulingManager
    {
        void DeleteSchedule(ScheduleDB schedule);
        void DeleteTimeBlock(TimeBlockDB block);
        void DeleteTimeBlocksByEmployee(EmployeeDB employee);
        void DeleteTimeBlocksByTicket(TicketSummaryDB ticket);
        ScheduleDB GetScheduleById(Guid scheduleId);
        List<ScheduleDB> GetSchedulingBySalonId(Guid salonId);
        TimeBlockDB GetTimeBlockById(Guid blockId);
        List<TimeBlockDB> GetTimeBlocksBySalonId(Guid salonId);
        ScheduleDB InsertSchedule(ScheduleDB schedule);
        TimeBlockDB InsertTimeBlock(TimeBlockDB block);
        ScheduleDB UpdateSchedule(ScheduleDB schedule);
        TimeBlockDB UpdateTimeBlock(TimeBlockDB block);
    }
}

