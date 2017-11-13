namespace SA.BAL
{
    using System;
    using System.Collections.Generic;

    public interface IMessageManager
    {
        void DeleteQueuedMessage(QueuedMessageDB value);
        QueuedMessageDB GetQueuedMessageById(Guid queuedMessageId);
        QueuedMessageDB InsertQueuedMessage(QueuedMessageDB value);
        List<QueuedMessageDB> SearchQueuedMessages(string sender, string recipient, DateTime startDate, DateTime endDate, int queuedEmailCount, bool notSentItemsOnly, int maxSendTries);
        QueuedMessageDB UpdateQueuedMessage(QueuedMessageDB value);
    }
}

