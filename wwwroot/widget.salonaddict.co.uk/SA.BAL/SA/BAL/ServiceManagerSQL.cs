namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;

    public class ServiceManagerSQL : IServiceManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public ServiceManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        private CategoryDB CategoryMapping(SqlDataReader reader) => 
            new CategoryDB { 
                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                CategoryId = reader.GetGuid(reader.GetOrdinal("CategoryId")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder")),
                MetaDescription = reader.GetString(reader.GetOrdinal("MetaDescription")),
                MetaKeywords = reader.GetString(reader.GetOrdinal("MetaKeywords")),
                MetaTitle = reader.GetString(reader.GetOrdinal("MetaTitle")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                ParentCategoryId = reader.GetValue(reader.GetOrdinal("ParentCategoryId")) as Guid?,
                SEName = reader.GetString(reader.GetOrdinal("SEName")),
                ShowOnMobile = reader.GetBoolean(reader.GetOrdinal("ShowOnMobile")),
                ShowOnWeb = reader.GetBoolean(reader.GetOrdinal("ShowOnWeb")),
                ShowOnWidget = reader.GetBoolean(reader.GetOrdinal("ShowOnWidget"))
            };

        public List<CategoryDB> GetCategories()
        {
            string key = $"CATEGORY-{"ALL"}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<CategoryDB>) obj2;
            }
            List<CategoryDB> list = new List<CategoryDB>();
            SqlParameter[] parameterValues = new SqlParameter[0];
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_CategoryLoad", parameterValues))
            {
                while (reader.Read())
                {
                    CategoryDB item = this.CategoryMapping(reader);
                    list.Add(item);
                }
            }
            this._cacheManager.Add(key, list);
            return list;
        }

        public ServiceDB GetServiceById(Guid serviceId)
        {
            if (serviceId == Guid.Empty)
            {
                throw new ArgumentException("service identifier cannot be empty.");
            }
            ServiceDB edb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@ServiceId", serviceId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_ServiceLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    edb = this.ServiceMapping(reader);
                }
            }
            return edb;
        }

        public List<ServiceDB> GetServicesBySalonId(Guid salonId)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("salon identifier cannot be empty.");
            }
            string key = $"SERVICE-{"SALON-" + salonId}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<ServiceDB>) obj2;
            }
            List<ServiceDB> list = new List<ServiceDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_ServiceLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    ServiceDB item = this.ServiceMapping(reader);
                    list.Add(item);
                }
            }
            this._cacheManager.Add(key, list);
            return list;
        }

        public ServiceDB InsertService(ServiceDB service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (service.DisplayOrder < 0)
            {
                throw new ArgumentOutOfRangeException("service.DisplayOrder", service.DisplayOrder, "Display order cannot be less than 0.");
            }
            if (service.Duration < 0)
            {
                throw new ArgumentOutOfRangeException("service.DurationInMinutes", service.Duration, "Duration in minutes cannot be less than 0.");
            }
            if (service.CurrencyCode == null)
            {
                throw new ArgumentNullException("service.currencyCode");
            }
            if (service.OldPrice < 0M)
            {
                throw new ArgumentOutOfRangeException("service.OldPrice", service.OldPrice, "Old price cannot be less than 0.");
            }
            if (service.Price < 0M)
            {
                throw new ArgumentOutOfRangeException("service.Price", service.Price, "Price cannot be less than 0.");
            }
            service.Name = service.Name ?? string.Empty;
            service.FullDescription = service.FullDescription ?? string.Empty;
            service.MetaDescription = service.MetaDescription ?? string.Empty;
            service.MetaKeywords = service.MetaKeywords ?? string.Empty;
            service.MetaTitle = service.MetaTitle ?? string.Empty;
            service.SEName = service.SEName ?? string.Empty;
            service.ShortDescription = service.ShortDescription ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@ServiceId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[0x1c];
            parameterArray2[0] = parameter;
            parameterArray2[1] = new SqlParameter("@Active", service.Active);
            parameterArray2[2] = new SqlParameter("@BookOnWeb", service.BookOnWeb);
            parameterArray2[3] = new SqlParameter("@BookOnMobile", service.BookOnMobile);
            parameterArray2[4] = new SqlParameter("@BookOnWidget", service.BookOnWidget);
            Guid? nullable = service.CategoryId1;
            parameterArray2[5] = new SqlParameter("@CategoryId1", nullable.HasValue ? ((SqlGuid) nullable.GetValueOrDefault()) : SqlGuid.Null);
            Guid? nullable2 = service.CategoryId2;
            parameterArray2[6] = new SqlParameter("@CategoryId2", nullable2.HasValue ? ((SqlGuid) nullable2.GetValueOrDefault()) : SqlGuid.Null);
            Guid? nullable3 = service.CategoryId3;
            parameterArray2[7] = new SqlParameter("@CategoryId3", nullable3.HasValue ? ((SqlGuid) nullable3.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[8] = new SqlParameter("@CreatedOnUtc", service.CreatedOnUtc);
            parameterArray2[9] = new SqlParameter("@CurrencyCode", service.CurrencyCode);
            parameterArray2[10] = new SqlParameter("@Deleted", service.Deleted);
            parameterArray2[11] = new SqlParameter("@DisplayOrder", service.DisplayOrder);
            parameterArray2[12] = new SqlParameter("@Name", service.Name);
            parameterArray2[13] = new SqlParameter("@Duration", service.Duration);
            parameterArray2[14] = new SqlParameter("@FullDescription", service.FullDescription);
            parameterArray2[15] = new SqlParameter("@IsTaxExempt", service.IsTaxExempt);
            parameterArray2[0x10] = new SqlParameter("@MetaDescription", service.MetaDescription);
            parameterArray2[0x11] = new SqlParameter("@MetaKeywords", service.MetaKeywords);
            parameterArray2[0x12] = new SqlParameter("@MetaTitle", service.MetaTitle);
            parameterArray2[0x13] = new SqlParameter("@OldPrice", service.OldPrice);
            parameterArray2[20] = new SqlParameter("@Price", service.Price);
            Guid? pictureId = service.PictureId;
            parameterArray2[0x15] = new SqlParameter("@PictureId", pictureId.HasValue ? ((SqlGuid) pictureId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[0x16] = new SqlParameter("@SalonId", service.SalonId);
            parameterArray2[0x17] = new SqlParameter("@SEName", service.SEName);
            parameterArray2[0x18] = new SqlParameter("@ShortDescription", service.ShortDescription);
            parameterArray2[0x19] = new SqlParameter("@ShowOnWeb", service.ShowOnWeb);
            parameterArray2[0x1a] = new SqlParameter("@ShowOnMobile", service.ShowOnMobile);
            parameterArray2[0x1b] = new SqlParameter("@ShowOnWidget", service.ShowOnWidget);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ServiceInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid serviceId = new Guid(parameter.Value.ToString());
            service = this.GetServiceById(serviceId);
            string key = $"SERVICE-{"SALON-" + service.SalonId}";
            this._cacheManager.Remove(key);
            return service;
        }

        private ServiceDB ServiceMapping(SqlDataReader reader) => 
            new ServiceDB { 
                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                BookOnWeb = reader.GetBoolean(reader.GetOrdinal("BookOnWeb")),
                BookOnMobile = reader.GetBoolean(reader.GetOrdinal("BookOnMobile")),
                BookOnWidget = reader.GetBoolean(reader.GetOrdinal("BookOnWidget")),
                CategoryId1 = reader.GetValue(reader.GetOrdinal("CategoryId1")) as Guid?,
                CategoryId2 = reader.GetValue(reader.GetOrdinal("CategoryId2")) as Guid?,
                CategoryId3 = reader.GetValue(reader.GetOrdinal("CategoryId3")) as Guid?,
                CreatedOnUtc = reader.GetDateTime(reader.GetOrdinal("CreatedOnUtc")),
                CurrencyCode = reader.GetString(reader.GetOrdinal("CurrencyCode")),
                Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted")),
                DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                FullDescription = reader.GetString(reader.GetOrdinal("FullDescription")),
                IsTaxExempt = reader.GetBoolean(reader.GetOrdinal("IsTaxExempt")),
                MetaDescription = reader.GetString(reader.GetOrdinal("MetaDescription")),
                MetaKeywords = reader.GetString(reader.GetOrdinal("MetaKeywords")),
                MetaTitle = reader.GetString(reader.GetOrdinal("MetaTitle")),
                OldPrice = reader.GetDecimal(reader.GetOrdinal("OldPrice")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                SEName = reader.GetString(reader.GetOrdinal("SEName")),
                ServiceId = reader.GetGuid(reader.GetOrdinal("ServiceId")),
                ShortDescription = reader.GetString(reader.GetOrdinal("ShortDescription")),
                ShowOnWeb = reader.GetBoolean(reader.GetOrdinal("ShowOnWeb")),
                ShowOnMobile = reader.GetBoolean(reader.GetOrdinal("ShowOnMobile")),
                ShowOnWidget = reader.GetBoolean(reader.GetOrdinal("ShowOnWidget"))
            };

        public ServiceDB UpdateService(ServiceDB service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (service.ServiceId == Guid.Empty)
            {
                throw new ArgumentException("Service identifier cannot be empty.");
            }
            if (service.DisplayOrder < 0)
            {
                throw new ArgumentOutOfRangeException("service.DisplayOrder", service.DisplayOrder, "Display order cannot be less than 0.");
            }
            if (service.Duration < 0)
            {
                throw new ArgumentOutOfRangeException("service.DurationInMinutes", service.Duration, "Duration in minutes cannot be less than 0.");
            }
            if (service.CurrencyCode == null)
            {
                throw new ArgumentNullException("service.CurrencyCode");
            }
            if (service.OldPrice < 0M)
            {
                throw new ArgumentOutOfRangeException("service.OldPrice", service.OldPrice, "Old price cannot be less than 0.");
            }
            if (service.Price < 0M)
            {
                throw new ArgumentOutOfRangeException("service.Price", service.Price, "Price cannot be less than 0.");
            }
            service.Name = service.Name ?? string.Empty;
            service.FullDescription = service.FullDescription ?? string.Empty;
            service.MetaDescription = service.MetaDescription ?? string.Empty;
            service.MetaKeywords = service.MetaKeywords ?? string.Empty;
            service.MetaTitle = service.MetaTitle ?? string.Empty;
            service.SEName = service.SEName ?? string.Empty;
            service.ShortDescription = service.ShortDescription ?? string.Empty;
            SqlParameter[] parameterArray2 = new SqlParameter[0x1c];
            parameterArray2[0] = new SqlParameter("@ServiceId", service.ServiceId);
            parameterArray2[1] = new SqlParameter("@Active", service.Active);
            parameterArray2[2] = new SqlParameter("@BookOnWeb", service.BookOnWeb);
            parameterArray2[3] = new SqlParameter("@BookOnMobile", service.BookOnMobile);
            parameterArray2[4] = new SqlParameter("@BookOnWidget", service.BookOnWidget);
            Guid? nullable = service.CategoryId1;
            parameterArray2[5] = new SqlParameter("@CategoryId1", nullable.HasValue ? ((SqlGuid) nullable.GetValueOrDefault()) : SqlGuid.Null);
            Guid? nullable2 = service.CategoryId2;
            parameterArray2[6] = new SqlParameter("@CategoryId2", nullable2.HasValue ? ((SqlGuid) nullable2.GetValueOrDefault()) : SqlGuid.Null);
            Guid? nullable3 = service.CategoryId3;
            parameterArray2[7] = new SqlParameter("@CategoryId3", nullable3.HasValue ? ((SqlGuid) nullable3.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[8] = new SqlParameter("@CreatedOnUtc", service.CreatedOnUtc);
            parameterArray2[9] = new SqlParameter("@CurrencyCode", service.CurrencyCode);
            parameterArray2[10] = new SqlParameter("@Deleted", service.Deleted);
            parameterArray2[11] = new SqlParameter("@DisplayOrder", service.DisplayOrder);
            parameterArray2[12] = new SqlParameter("@Name", service.Name);
            parameterArray2[13] = new SqlParameter("@Duration", service.Duration);
            parameterArray2[14] = new SqlParameter("@FullDescription", service.FullDescription);
            parameterArray2[15] = new SqlParameter("@IsTaxExempt", service.IsTaxExempt);
            parameterArray2[0x10] = new SqlParameter("@MetaDescription", service.MetaDescription);
            parameterArray2[0x11] = new SqlParameter("@MetaKeywords", service.MetaKeywords);
            parameterArray2[0x12] = new SqlParameter("@MetaTitle", service.MetaTitle);
            parameterArray2[0x13] = new SqlParameter("@OldPrice", service.OldPrice);
            parameterArray2[20] = new SqlParameter("@Price", service.Price);
            Guid? pictureId = service.PictureId;
            parameterArray2[0x15] = new SqlParameter("@PictureId", pictureId.HasValue ? ((SqlGuid) pictureId.GetValueOrDefault()) : SqlGuid.Null);
            parameterArray2[0x16] = new SqlParameter("@SalonId", service.SalonId);
            parameterArray2[0x17] = new SqlParameter("@SEName", service.SEName);
            parameterArray2[0x18] = new SqlParameter("@ShortDescription", service.ShortDescription);
            parameterArray2[0x19] = new SqlParameter("@ShowOnWeb", service.ShowOnWeb);
            parameterArray2[0x1a] = new SqlParameter("@ShowOnMobile", service.ShowOnMobile);
            parameterArray2[0x1b] = new SqlParameter("@ShowOnWidget", service.ShowOnWidget);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_ServiceUpdate", commandParameters) <= 0)
            {
                return null;
            }
            service = this.GetServiceById(service.ServiceId);
            string key = $"SERVICE-{"SALON-" + service.SalonId}";
            this._cacheManager.Remove(key);
            return service;
        }
    }
}

