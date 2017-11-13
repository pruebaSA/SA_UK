namespace SA.BAL
{
    using System;
    using System.Collections.Generic;

    public interface IServiceManager
    {
        List<CategoryDB> GetCategories();
        ServiceDB GetServiceById(Guid serviceId);
        List<ServiceDB> GetServicesBySalonId(Guid salonId);
        ServiceDB InsertService(ServiceDB service);
        ServiceDB UpdateService(ServiceDB service);
    }
}

