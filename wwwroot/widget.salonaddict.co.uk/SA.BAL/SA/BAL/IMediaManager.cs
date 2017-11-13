namespace SA.BAL
{
    using System;

    public interface IMediaManager
    {
        void DeletePicture(PictureDB picture);
        PictureDB GetPictureById(Guid pictureId);
        PictureDB InsertPicture(PictureDB picture);
    }
}

