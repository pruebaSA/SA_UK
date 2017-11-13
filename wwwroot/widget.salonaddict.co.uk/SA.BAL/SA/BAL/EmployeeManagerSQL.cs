namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Xml.Linq;

    public class EmployeeManagerSQL : IEmployeeManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public EmployeeManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        public void DeleteEmployeeServiceMapping(Employee_Service_MappingDB mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }
            if (mapping.EmployeeId == Guid.Empty)
            {
                throw new ArgumentException("employee identifier cannot be empty.");
            }
            if (mapping.ServiceId == Guid.Empty)
            {
                throw new ArgumentException("service identifier cannot be empty.");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@MappingId", mapping.MappingId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_EmployeeServiceMappingDeleteById", commandParameters);
            EmployeeDB employeeById = IoC.Resolve<IEmployeeManager>().GetEmployeeById(mapping.EmployeeId);
            string key = $"EMPLOYEE-SERVICE-{"SALON-" + employeeById.SalonId}";
            this._cacheManager.Remove(key);
        }

        public void DeleteEmployeeServiceMappingByEmployee(EmployeeDB employee)
        {
            if (employee == null)
            {
                throw new ArgumentException("employee cannot be null.");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@EmployeeId", employee.EmployeeId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_EmployeeServiceMappingDeleteByEmployeeId", commandParameters);
            string key = $"EMPLOYEE-SERVICE-{"SALON-" + employee.SalonId}";
            this._cacheManager.Remove(key);
        }

        private EmployeeDB EmployeeMapping(SqlDataReader reader) => 
            new EmployeeDB { 
                BookOnWeb = reader.GetBoolean(reader.GetOrdinal("BookOnWeb")),
                BookOnMobile = reader.GetBoolean(reader.GetOrdinal("BookOnMobile")),
                BookOnWidget = reader.GetBoolean(reader.GetOrdinal("BookOnWidget")),
                CreatedOnUtc = reader.GetDateTime(reader.GetOrdinal("CreatedOnUtc")),
                DateOfBirth = reader.GetValue(reader.GetOrdinal("DOB")) as DateTime?,
                DateOfHire = reader.GetValue(reader.GetOrdinal("DOH")) as DateTime?,
                Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted")),
                DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder")),
                DisplayText = reader.GetString(reader.GetOrdinal("DisplayText")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                EmployeeId = reader.GetGuid(reader.GetOrdinal("EmployeeId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                MetaDescription = reader.GetString(reader.GetOrdinal("MetaDescription")),
                MetaKeywords = reader.GetString(reader.GetOrdinal("MetaKeywords")),
                MetaTitle = reader.GetString(reader.GetOrdinal("MetaTitle")),
                Mobile = reader.GetString(reader.GetOrdinal("Mobile")),
                MobilePIN = reader.GetString(reader.GetOrdinal("MobilePIN")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                PictureId = reader.GetValue(reader.GetOrdinal("PictureId")) as Guid?,
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                SEName = reader.GetString(reader.GetOrdinal("SEName")),
                Sex = reader.GetString(reader.GetOrdinal("Sex")),
                ShowOnWeb = reader.GetBoolean(reader.GetOrdinal("ShowOnWeb")),
                ShowOnMobile = reader.GetBoolean(reader.GetOrdinal("ShowOnMobile")),
                ShowOnWidget = reader.GetBoolean(reader.GetOrdinal("ShowOnWidget")),
                TotalRatingSum = reader.GetInt32(reader.GetOrdinal("TotalRatingSum")),
                TotalRatingVotes = reader.GetInt32(reader.GetOrdinal("TotalRatingVotes")),
                TotalReviews = reader.GetInt32(reader.GetOrdinal("TotalReviews")),
                VatNumber = reader.GetString(reader.GetOrdinal("VatNumber")),
                VatNumberStatus = reader.GetString(reader.GetOrdinal("VatNumberStatus"))
            };

        private Employee_Service_MappingDB EmployeeServiceMappingMapping(SqlDataReader reader) => 
            new Employee_Service_MappingDB { 
                EmployeeId = reader.GetGuid(reader.GetOrdinal("EmployeeId")),
                MappingId = reader.GetGuid(reader.GetOrdinal("MappingId")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                ServiceId = reader.GetGuid(reader.GetOrdinal("ServiceId"))
            };

        public EmployeeDB GetEmployeeById(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                throw new ArgumentException("employee identifier cannot be empty.");
            }
            EmployeeDB edb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@EmployeeId", employeeId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_EmployeeLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    edb = this.EmployeeMapping(reader);
                }
            }
            return edb;
        }

        public List<EmployeeDB> GetEmployeesBySalonId(Guid salonId)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            string key = $"EMPLOYEE-{"SALON-" + salonId}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<EmployeeDB>) obj2;
            }
            List<EmployeeDB> list = new List<EmployeeDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_EmployeeLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    EmployeeDB item = this.EmployeeMapping(reader);
                    list.Add(item);
                }
            }
            this._cacheManager.Add(key, list);
            return list;
        }

        public Employee_Service_MappingDB GetEmployeeServiceMapping(Guid employeeId, Guid serviceId)
        {
            if (employeeId == Guid.Empty)
            {
                throw new ArgumentException("employee identifier cannot be empty.");
            }
            if (serviceId == Guid.Empty)
            {
                throw new ArgumentException("service identifier cannot be empty.");
            }
            Employee_Service_MappingDB gdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@EmployeeId", employeeId), new SqlParameter("@ServiceId", serviceId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_EmployeeServiceMappingLoad", parameterValues))
            {
                if (reader.Read())
                {
                    gdb = this.EmployeeServiceMappingMapping(reader);
                }
            }
            return gdb;
        }

        public List<Employee_Service_MappingDB> GetEmployeeServiceMappingByEmployeeId(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                throw new ArgumentException("employee identifier cannot be empty.");
            }
            List<Employee_Service_MappingDB> list = new List<Employee_Service_MappingDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@EmployeeId", employeeId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_EmployeeServiceMappingLoadByEmployeeId", parameterValues))
            {
                while (reader.Read())
                {
                    Employee_Service_MappingDB item = this.EmployeeServiceMappingMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public Employee_Service_MappingDB GetEmployeeServiceMappingById(Guid mappingId)
        {
            if (mappingId == Guid.Empty)
            {
                throw new ArgumentException("employee service mapping identifier cannot be empty.");
            }
            Employee_Service_MappingDB gdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@MappingId", mappingId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_EmployeeServiceMappingLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    gdb = this.EmployeeServiceMappingMapping(reader);
                }
            }
            return gdb;
        }

        public List<Employee_Service_MappingDB> GetEmployeeServiceMappingBySalonId(Guid salonId)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            List<Employee_Service_MappingDB> list = new List<Employee_Service_MappingDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_EmployeeServiceMappingLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    Employee_Service_MappingDB item = this.EmployeeServiceMappingMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public List<Employee_Service_MappingDB> GetEmployeeServiceMappingByServiceId(Guid serviceId)
        {
            if (serviceId == Guid.Empty)
            {
                throw new ArgumentException("service identifier cannot be empty.");
            }
            List<Employee_Service_MappingDB> list = new List<Employee_Service_MappingDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@ServiceId", serviceId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_EmployeeServiceMappingLoadByServiceId", parameterValues))
            {
                while (reader.Read())
                {
                    Employee_Service_MappingDB item = this.EmployeeServiceMappingMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public EmployeeDB InsertEmployee(EmployeeDB employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }
            if (employee.SalonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            if (employee.SEName == null)
            {
                throw new ArgumentNullException("SEname cannot be null.");
            }
            if (employee.DisplayOrder < 0)
            {
                throw new ArgumentOutOfRangeException("employee.DisplayOrder", employee.DisplayOrder, "display order cannot be less than 0.");
            }
            if (employee.TotalRatingSum < 0)
            {
                throw new ArgumentOutOfRangeException("employee.TotalRatingSum", employee.TotalRatingSum, "total rating sum order cannot be less than 0.");
            }
            if (employee.TotalRatingVotes < 0)
            {
                throw new ArgumentOutOfRangeException("employee.TotalRatingVotes", employee.TotalRatingVotes, "total rating votes cannot be less than 0.");
            }
            if (employee.TotalReviews < 0)
            {
                throw new ArgumentOutOfRangeException("employee.TotalReviews", employee.TotalReviews, "total reviews cannot be less than 0.");
            }
            employee.DisplayText = employee.DisplayText ?? string.Empty;
            employee.Email = employee.Email ?? string.Empty;
            employee.FirstName = employee.FirstName ?? string.Empty;
            employee.LastName = employee.LastName ?? string.Empty;
            employee.MetaDescription = employee.MetaDescription ?? string.Empty;
            employee.MetaKeywords = employee.MetaKeywords ?? string.Empty;
            employee.MetaTitle = employee.MetaTitle ?? string.Empty;
            employee.Mobile = employee.Mobile ?? string.Empty;
            employee.MobilePIN = employee.MobilePIN ?? string.Empty;
            employee.PhoneNumber = employee.PhoneNumber ?? string.Empty;
            employee.SEName = employee.SEName ?? string.Empty;
            employee.Sex = employee.Sex ?? string.Empty;
            employee.VatNumber = employee.VatNumber ?? string.Empty;
            employee.VatNumberStatus = employee.VatNumberStatus ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@EmployeeId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[0x1f];
            parameterArray2[0] = parameter;
            parameterArray2[1] = new SqlParameter("@BookOnWeb", employee.BookOnWeb);
            parameterArray2[2] = new SqlParameter("@BookOnMobile", employee.BookOnMobile);
            parameterArray2[3] = new SqlParameter("@BookOnWidget", employee.BookOnWidget);
            parameterArray2[4] = new SqlParameter("@CreatedOnUtc", employee.CreatedOnUtc);
            DateTime? dateOfBirth = employee.DateOfBirth;
            parameterArray2[5] = new SqlParameter("@DOB", dateOfBirth.HasValue ? ((SqlDateTime) dateOfBirth.GetValueOrDefault()) : SqlDateTime.Null);
            DateTime? dateOfHire = employee.DateOfHire;
            parameterArray2[6] = new SqlParameter("@DOH", dateOfHire.HasValue ? ((SqlDateTime) dateOfHire.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[7] = new SqlParameter("@Deleted", employee.Deleted);
            parameterArray2[8] = new SqlParameter("@DisplayOrder", employee.DisplayOrder);
            parameterArray2[9] = new SqlParameter("@DisplayText", employee.DisplayText);
            parameterArray2[10] = new SqlParameter("@Email", employee.Email);
            parameterArray2[11] = new SqlParameter("@FirstName", employee.FirstName);
            parameterArray2[12] = new SqlParameter("@LastName", employee.LastName);
            parameterArray2[13] = new SqlParameter("@MetaDescription", employee.MetaDescription);
            parameterArray2[14] = new SqlParameter("@MetaKeywords", employee.MetaKeywords);
            parameterArray2[15] = new SqlParameter("@MetaTitle", employee.MetaTitle);
            parameterArray2[0x10] = new SqlParameter("@Mobile", employee.Mobile);
            parameterArray2[0x11] = new SqlParameter("@MobilePIN", employee.MobilePIN);
            parameterArray2[0x12] = new SqlParameter("@PhoneNumber", employee.PhoneNumber);
            Guid? pictureId = employee.PictureId;
            parameterArray2[0x13] = new SqlParameter("@PictureId", pictureId.HasValue ? ((SqlGuid) pictureId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[20] = new SqlParameter("@SalonId", employee.SalonId);
            parameterArray2[0x15] = new SqlParameter("@SEName", employee.SEName);
            parameterArray2[0x16] = new SqlParameter("@Sex", employee.Sex);
            parameterArray2[0x17] = new SqlParameter("@ShowOnWeb", employee.ShowOnWeb);
            parameterArray2[0x18] = new SqlParameter("@ShowOnMobile", employee.ShowOnMobile);
            parameterArray2[0x19] = new SqlParameter("@ShowOnWidget", employee.ShowOnWidget);
            parameterArray2[0x1a] = new SqlParameter("@TotalRatingSum", employee.TotalRatingSum);
            parameterArray2[0x1b] = new SqlParameter("@TotalRatingVotes", employee.TotalRatingVotes);
            parameterArray2[0x1c] = new SqlParameter("@TotalReviews", employee.TotalReviews);
            parameterArray2[0x1d] = new SqlParameter("@VatNumber", employee.VatNumber);
            parameterArray2[30] = new SqlParameter("@VatNumberStatus", employee.VatNumberStatus);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_EmployeeInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid employeeId = new Guid(parameter.Value.ToString());
            employee = this.GetEmployeeById(employeeId);
            string key = $"EMPLOYEE-{"SALON-" + employee.SalonId}";
            this._cacheManager.Remove(key);
            return employee;
        }

        public Employee_Service_MappingDB InsertEmployeeServiceMapping(Employee_Service_MappingDB mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }
            if (mapping.EmployeeId == Guid.Empty)
            {
                throw new ArgumentException("employee identifier cannot be empty.");
            }
            if (mapping.ServiceId == Guid.Empty)
            {
                throw new ArgumentException("service identifier cannot be empty.");
            }
            if (mapping.Price < 0M)
            {
                throw new ArgumentOutOfRangeException("mapping.Price", mapping.Price, "Price cannot be less than zero.");
            }
            if (mapping.Duration < 0)
            {
                throw new ArgumentOutOfRangeException("mapping.DurationInMinutes", mapping.Duration, "Duration in minutes cannot be less than zero.");
            }
            SqlParameter parameter = new SqlParameter("@MappingId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { parameter, new SqlParameter("@EmployeeId", mapping.EmployeeId), new SqlParameter("@ServiceId", mapping.ServiceId), new SqlParameter("@Price", mapping.Price), new SqlParameter("@Duration", mapping.Duration) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_EmployeeServiceMappingInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid mappingId = new Guid(parameter.Value.ToString());
            EmployeeDB employeeById = IoC.Resolve<IEmployeeManager>().GetEmployeeById(mapping.EmployeeId);
            string key = $"EMPLOYEE-SERVICE-{"SALON-" + employeeById.SalonId}";
            this._cacheManager.Remove(key);
            mapping = this.GetEmployeeServiceMappingById(mappingId);
            return mapping;
        }

        public bool InsertEmployeeServiceMappingMultiple(List<Employee_Service_MappingDB> mappings)
        {
            if (mappings == null)
            {
                throw new ArgumentNullException("mappings");
            }
            if (mappings.Count == 0)
            {
                throw new ArgumentException("Mappings cannot be empty.");
            }
            XDocument document = new XDocument(new object[] { new XElement("Mappings") });
            foreach (Employee_Service_MappingDB gdb in mappings)
            {
                document.Root.Add(new XElement("Mapping", new object[] { new XElement("EmployeeId", gdb.EmployeeId), new XElement("ServiceId", gdb.ServiceId), new XElement("Price", gdb.Price), new XElement("Duration", gdb.Duration) }));
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("Mappings_XML", new SqlXml(document.CreateReader())) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_EmployeeServiceMappingInsertMultiple", commandParameters) <= 0)
            {
                return false;
            }
            return true;
        }

        public EmployeeDB UpdateEmployee(EmployeeDB employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }
            if (employee.EmployeeId == Guid.Empty)
            {
                throw new ArgumentException("Employee identifier cannot be empty.");
            }
            if (employee.SalonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            if (employee.SEName == null)
            {
                throw new ArgumentNullException("SEname cannot be null.");
            }
            if (employee.DisplayOrder < 0)
            {
                throw new ArgumentOutOfRangeException("employee.DisplayOrder", employee.DisplayOrder, "display order cannot be less than 0.");
            }
            if (employee.TotalRatingSum < 0)
            {
                throw new ArgumentOutOfRangeException("employee.TotalRatingSum", employee.TotalRatingSum, "total rating sum order cannot be less than 0.");
            }
            if (employee.TotalRatingVotes < 0)
            {
                throw new ArgumentOutOfRangeException("employee.TotalRatingVotes", employee.TotalRatingVotes, "total rating votes cannot be less than 0.");
            }
            if (employee.TotalReviews < 0)
            {
                throw new ArgumentOutOfRangeException("employee.TotalReviews", employee.TotalReviews, "total reviews cannot be less than 0.");
            }
            employee.DisplayText = employee.DisplayText ?? string.Empty;
            employee.Email = employee.Email ?? string.Empty;
            employee.FirstName = employee.FirstName ?? string.Empty;
            employee.LastName = employee.LastName ?? string.Empty;
            employee.MetaDescription = employee.MetaDescription ?? string.Empty;
            employee.MetaKeywords = employee.MetaKeywords ?? string.Empty;
            employee.MetaTitle = employee.MetaTitle ?? string.Empty;
            employee.Mobile = employee.Mobile ?? string.Empty;
            employee.MobilePIN = employee.MobilePIN ?? string.Empty;
            employee.PhoneNumber = employee.PhoneNumber ?? string.Empty;
            employee.SEName = employee.SEName ?? string.Empty;
            employee.Sex = employee.Sex ?? string.Empty;
            employee.VatNumber = employee.VatNumber ?? string.Empty;
            employee.VatNumberStatus = employee.VatNumberStatus ?? string.Empty;
            SqlParameter[] parameterArray2 = new SqlParameter[0x1f];
            parameterArray2[0] = new SqlParameter("@EmployeeId", employee.EmployeeId);
            parameterArray2[1] = new SqlParameter("@BookOnWeb", employee.BookOnWeb);
            parameterArray2[2] = new SqlParameter("@BookOnMobile", employee.BookOnMobile);
            parameterArray2[3] = new SqlParameter("@BookOnWidget", employee.BookOnWidget);
            parameterArray2[4] = new SqlParameter("@CreatedOnUtc", employee.CreatedOnUtc);
            DateTime? dateOfBirth = employee.DateOfBirth;
            parameterArray2[5] = new SqlParameter("@DOB", dateOfBirth.HasValue ? ((SqlDateTime) dateOfBirth.GetValueOrDefault()) : SqlDateTime.Null);
            DateTime? dateOfHire = employee.DateOfHire;
            parameterArray2[6] = new SqlParameter("@DOH", dateOfHire.HasValue ? ((SqlDateTime) dateOfHire.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[7] = new SqlParameter("@Deleted", employee.Deleted);
            parameterArray2[8] = new SqlParameter("@DisplayOrder", employee.DisplayOrder);
            parameterArray2[9] = new SqlParameter("@DisplayText", employee.DisplayText);
            parameterArray2[10] = new SqlParameter("@Email", employee.Email);
            parameterArray2[11] = new SqlParameter("@FirstName", employee.FirstName);
            parameterArray2[12] = new SqlParameter("@LastName", employee.LastName);
            parameterArray2[13] = new SqlParameter("@MetaDescription", employee.MetaDescription);
            parameterArray2[14] = new SqlParameter("@MetaKeywords", employee.MetaKeywords);
            parameterArray2[15] = new SqlParameter("@MetaTitle", employee.MetaTitle);
            parameterArray2[0x10] = new SqlParameter("@Mobile", employee.Mobile);
            parameterArray2[0x11] = new SqlParameter("@MobilePIN", employee.MobilePIN);
            parameterArray2[0x12] = new SqlParameter("@PhoneNumber", employee.PhoneNumber);
            Guid? pictureId = employee.PictureId;
            parameterArray2[0x13] = new SqlParameter("@PictureId", pictureId.HasValue ? ((SqlGuid) pictureId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[20] = new SqlParameter("@SalonId", employee.SalonId);
            parameterArray2[0x15] = new SqlParameter("@SEName", employee.SEName);
            parameterArray2[0x16] = new SqlParameter("@Sex", employee.Sex);
            parameterArray2[0x17] = new SqlParameter("@ShowOnWeb", employee.ShowOnWeb);
            parameterArray2[0x18] = new SqlParameter("@ShowOnMobile", employee.ShowOnMobile);
            parameterArray2[0x19] = new SqlParameter("@ShowOnWidget", employee.ShowOnWidget);
            parameterArray2[0x1a] = new SqlParameter("@TotalRatingSum", employee.TotalRatingSum);
            parameterArray2[0x1b] = new SqlParameter("@TotalRatingVotes", employee.TotalRatingVotes);
            parameterArray2[0x1c] = new SqlParameter("@TotalReviews", employee.TotalReviews);
            parameterArray2[0x1d] = new SqlParameter("@VatNumber", employee.VatNumber);
            parameterArray2[30] = new SqlParameter("@VatNumberStatus", employee.VatNumberStatus);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_EmployeeUpdate", commandParameters) <= 0)
            {
                return null;
            }
            employee = this.GetEmployeeById(employee.EmployeeId);
            string key = $"EMPLOYEE-{"SALON-" + employee.SalonId}";
            this._cacheManager.Remove(key);
            return employee;
        }
    }
}

