namespace SA.BAL
{
    using System;
    using System.Collections.Generic;

    public interface ITicketManager
    {
        void DeleteTicketAlert(TicketAlertDB alert);
        List<OpenTicketDB> GetOpenTicketsBySalonId(Guid salonId);
        TicketAlertDB GetTicketAlertById(Guid alertId);
        List<TicketAlertDB> GetTicketAlertsBySalonId(Guid salonId);
        TicketSummaryDB GetTicketById(Guid ticketId);
        TicketRowDB GetTicketRowById(Guid ticketRowId);
        List<TicketRowDB> GetTicketRowsByTicketId(Guid ticketId);
        TicketSummaryDB InsertTicket(TicketSummaryDB ticket);
        TicketAlertDB InsertTicketAlert(TicketAlertDB alert);
        TicketRowDB InsertTicketRow(TicketRowDB ticketRow);
        TicketSummaryDB UpdateTicket(TicketSummaryDB ticket);
        TicketAlertDB UpdateTicketAlert(TicketAlertDB alert);
    }
}

