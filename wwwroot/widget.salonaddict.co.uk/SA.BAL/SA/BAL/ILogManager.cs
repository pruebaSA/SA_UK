namespace SA.BAL
{
    using System;
    using System.Collections.Generic;

    public interface ILogManager
    {
        void ClearLog();
        void DeleteLog(LogDB log);
        List<LogDB> GetAllErrors();
        LogDB GetErrorById(Guid logId);
        LogDB InsertError(LogDB log);
    }
}

