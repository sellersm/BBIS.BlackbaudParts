using System;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public class CountryProfileOptions
    {
        #region Fields
        private string _noteType;
        private string _imageDocType;
        #endregion

        #region Properties
        public string NoteType
        {
            get { return _noteType; }
            set { _noteType = value; }
        }

        public string ImageDocType
        {
            get { return _imageDocType; }
            set { _imageDocType = value; }
        }
        #endregion 
    }
}