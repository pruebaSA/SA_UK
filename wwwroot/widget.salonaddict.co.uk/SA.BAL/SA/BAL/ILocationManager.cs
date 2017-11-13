namespace SA.BAL
{
    using System;
    using System.Collections.Generic;

    public interface ILocationManager
    {
        void DeleteLocationSalonMapping(LocationSalonMappingDB mapping);
        List<LocationDB> GetAreasPostalCodes(Guid regionId);
        List<LocationDB> GetAreasPostalCodesByCountry(Guid countyId);
        List<LocationDB> GetCountiesByCountry(string country);
        List<LocationDB> GetCountiesCityTownsByCountry(string country);
        LocationSalonMappingDB GetLocationSalonMappingById(Guid mappingId);
        List<LocationSalonSummaryDB> GetLocationSalonSummary(Guid salonId);
        bool InsertLocationSalonMapping(LocationSalonMappingDB mapping);
    }
}

