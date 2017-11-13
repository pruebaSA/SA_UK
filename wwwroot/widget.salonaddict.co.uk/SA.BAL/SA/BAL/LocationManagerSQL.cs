namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class LocationManagerSQL : ILocationManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public LocationManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        public void DeleteLocationSalonMapping(LocationSalonMappingDB mapping)
        {
            if (mapping.MappingId == Guid.Empty)
            {
                throw new ArgumentException("Mapping identifier cannot be empty.");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@MappingId", mapping.MappingId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_LocationSalonMappingDeleteById", commandParameters);
        }

        public List<LocationDB> GetAreasPostalCodes(Guid regionId)
        {
            if (regionId == Guid.Empty)
            {
                throw new ArgumentException("Region identifier cannot be empty.");
            }
            string key = $"loc.ap.{regionId.ToString()}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<LocationDB>) obj2;
            }
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@CountyOrCityTownId", regionId) };
            List<LocationDB> list = new List<LocationDB>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_LocationLoadAreasPostalCodes", parameterValues))
            {
                while (reader.Read())
                {
                    LocationDB locationFromReader = this.GetLocationFromReader(reader);
                    list.Add(locationFromReader);
                }
            }
            this._cacheManager.Add(key, list);
            return list;
        }

        public List<LocationDB> GetAreasPostalCodesByCountry(Guid countyId)
        {
            if (countyId == Guid.Empty)
            {
                throw new ArgumentException("County identifier cannot be empty.");
            }
            string key = $"loc.cnty.{countyId.ToString()}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<LocationDB>) obj2;
            }
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@CountyId", countyId) };
            List<LocationDB> list = new List<LocationDB>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_LocationLoadAreasPostalCodesByCounty", parameterValues))
            {
                while (reader.Read())
                {
                    LocationDB locationFromReader = this.GetLocationFromReader(reader);
                    list.Add(locationFromReader);
                }
            }
            this._cacheManager.Add(key, list);
            return list;
        }

        public List<LocationDB> GetCountiesByCountry(string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                throw new ArgumentException("Country cannot be null or empty.");
            }
            string key = $"loc.cntry.{country}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<LocationDB>) obj2;
            }
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@CountrySEName", country) };
            List<LocationDB> list = new List<LocationDB>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_LocationLoadCountiesByCountry", parameterValues))
            {
                while (reader.Read())
                {
                    LocationDB locationFromReader = this.GetLocationFromReader(reader);
                    list.Add(locationFromReader);
                }
            }
            this._cacheManager.Add(key, list);
            return list;
        }

        public List<LocationDB> GetCountiesCityTownsByCountry(string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                throw new ArgumentException("Country cannot be null or empty.");
            }
            string key = $"loc.cntyct.{country}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (List<LocationDB>) obj2;
            }
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@CountrySEName", country) };
            List<LocationDB> list = new List<LocationDB>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_LocationLoadCountiesCityTownsByCountry", parameterValues))
            {
                while (reader.Read())
                {
                    LocationDB locationFromReader = this.GetLocationFromReader(reader);
                    list.Add(locationFromReader);
                }
            }
            this._cacheManager.Add(key, list);
            return list;
        }

        private LocationDB GetLocationFromReader(SqlDataReader reader) => 
            new LocationDB { 
                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                CityTown = reader.GetValue(reader.GetOrdinal("CityTown")) as Guid?,
                Country = reader.GetValue(reader.GetOrdinal("Country")) as Guid?,
                County = reader.GetValue(reader.GetOrdinal("County")) as Guid?,
                Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Latitude = reader.GetString(reader.GetOrdinal("Latitude")),
                LocationId = reader.GetGuid(reader.GetOrdinal("LocationId")),
                LocationType = reader.GetInt32(reader.GetOrdinal("LocationType")),
                Longitude = reader.GetString(reader.GetOrdinal("Longitude")),
                MetaDescription = reader.GetString(reader.GetOrdinal("MetaDescription")),
                MetaKeywords = reader.GetString(reader.GetOrdinal("MetaKeywords")),
                MetaTitle = reader.GetString(reader.GetOrdinal("MetaTitle")),
                SEName = reader.GetString(reader.GetOrdinal("SEName")),
                ShowOnWeb = reader.GetBoolean(reader.GetOrdinal("ShowOnWeb")),
                ShowOnMobile = reader.GetBoolean(reader.GetOrdinal("ShowOnMobile")),
                ShowOnWidget = reader.GetBoolean(reader.GetOrdinal("ShowOnWidget")),
                StateProvince = reader.GetValue(reader.GetOrdinal("StateProvince")) as Guid?
            };

        public LocationSalonMappingDB GetLocationSalonMappingById(Guid mappingId)
        {
            if (mappingId == Guid.Empty)
            {
                throw new ArgumentException("Mapping identifier cannot be empty.");
            }
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@LocationSalonMappingId", mappingId) };
            LocationSalonMappingDB locationSalonMappingFromReader = null;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_LocationSalonMappingLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    locationSalonMappingFromReader = this.GetLocationSalonMappingFromReader(reader);
                }
            }
            return locationSalonMappingFromReader;
        }

        private LocationSalonMappingDB GetLocationSalonMappingFromReader(SqlDataReader reader) => 
            new LocationSalonMappingDB { 
                LocationId = reader.GetGuid(reader.GetOrdinal("LocationId")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                MappingId = reader.GetGuid(reader.GetOrdinal("MappingId"))
            };

        public List<LocationSalonSummaryDB> GetLocationSalonSummary(Guid salonId)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            List<LocationSalonSummaryDB> list = new List<LocationSalonSummaryDB>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_LocationSalonSummaryLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    LocationSalonSummaryDB locationSalonSummaryFromReader = this.GetLocationSalonSummaryFromReader(reader);
                    list.Add(locationSalonSummaryFromReader);
                }
            }
            return list;
        }

        private LocationSalonSummaryDB GetLocationSalonSummaryFromReader(SqlDataReader reader) => 
            new LocationSalonSummaryDB { 
                LocationId = reader.GetGuid(reader.GetOrdinal("LocationId")),
                MappingId = reader.GetGuid(reader.GetOrdinal("MappingId")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            };

        public bool InsertLocationSalonMapping(LocationSalonMappingDB mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentException("Mapping cannot be null.");
            }
            if (mapping.LocationId == Guid.Empty)
            {
                throw new ArgumentException("Location identifier cannot be empty.");
            }
            if (mapping.SalonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            SqlParameter parameter = new SqlParameter("@MappingId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { parameter, new SqlParameter("@LocationId", mapping.LocationId), new SqlParameter("@SalonId", mapping.SalonId) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_LocationSalonMappingInsert", commandParameters) <= 0)
            {
                return false;
            }
            new Guid(parameter.Value.ToString());
            return true;
        }
    }
}

