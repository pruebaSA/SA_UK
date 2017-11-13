namespace SA.BAL
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class MediaManagerSQL : IMediaManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public MediaManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        public void DeletePicture(PictureDB picture)
        {
            if (picture == null)
            {
                throw new ArgumentNullException("picture");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@PictureId", picture.PictureId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_PictureDeleteById", commandParameters);
        }

        public PictureDB GetPictureById(Guid pictureId)
        {
            PictureDB edb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@PictureId", pictureId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_PictureLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    edb = this.PictureMapping(reader);
                }
            }
            return edb;
        }

        public PictureDB InsertPicture(PictureDB picture)
        {
            if (picture == null)
            {
                throw new ArgumentNullException("picture");
            }
            picture.MimeType = picture.MimeType ?? string.Empty;
            picture.Name = picture.Name ?? string.Empty;
            picture.SEName = picture.SEName ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@PictureId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { parameter, new SqlParameter("@Deleted", picture.Deleted), new SqlParameter("@Height", picture.Height), new SqlParameter("@MimeType", picture.MimeType), new SqlParameter("@Name", picture.Name), new SqlParameter("@PictureBinary", picture.PictureBinary), new SqlParameter("@SEName", picture.SEName), new SqlParameter("@Width", picture.Width) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_PictureInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid pictureId = new Guid(parameter.Value.ToString());
            picture = this.GetPictureById(pictureId);
            return picture;
        }

        private PictureDB PictureMapping(SqlDataReader reader) => 
            new PictureDB { 
                Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted")),
                Height = reader.GetInt32(reader.GetOrdinal("Height")),
                MimeType = reader.GetString(reader.GetOrdinal("MimeType")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                PictureBinary = (byte[]) reader.GetValue(reader.GetOrdinal("PictureBinary")),
                PictureId = reader.GetGuid(reader.GetOrdinal("PictureId")),
                SEName = reader.GetString(reader.GetOrdinal("SEName")),
                Width = reader.GetInt32(reader.GetOrdinal("Width"))
            };
    }
}

