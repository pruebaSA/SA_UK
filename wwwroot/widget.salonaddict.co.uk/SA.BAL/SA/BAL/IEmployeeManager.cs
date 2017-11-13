namespace SA.BAL
{
    using System;
    using System.Collections.Generic;

    public interface IEmployeeManager
    {
        void DeleteEmployeeServiceMapping(Employee_Service_MappingDB mapping);
        void DeleteEmployeeServiceMappingByEmployee(EmployeeDB employee);
        EmployeeDB GetEmployeeById(Guid employeeId);
        List<EmployeeDB> GetEmployeesBySalonId(Guid salonId);
        Employee_Service_MappingDB GetEmployeeServiceMapping(Guid employeeId, Guid serviceId);
        List<Employee_Service_MappingDB> GetEmployeeServiceMappingByEmployeeId(Guid employeeId);
        Employee_Service_MappingDB GetEmployeeServiceMappingById(Guid mappingId);
        List<Employee_Service_MappingDB> GetEmployeeServiceMappingBySalonId(Guid salonId);
        List<Employee_Service_MappingDB> GetEmployeeServiceMappingByServiceId(Guid serviceId);
        EmployeeDB InsertEmployee(EmployeeDB employee);
        Employee_Service_MappingDB InsertEmployeeServiceMapping(Employee_Service_MappingDB mapping);
        bool InsertEmployeeServiceMappingMultiple(List<Employee_Service_MappingDB> mappings);
        EmployeeDB UpdateEmployee(EmployeeDB employee);
    }
}

