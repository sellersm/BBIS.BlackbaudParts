using System;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public class ProjectProfileOptions
    {
        #region Fields
        private string _noteDocType;
        private string _imageDocType;
        #endregion

        #region Properties
        public string NoteDocType
        {
            get { return _noteDocType; }
            set { _noteDocType = value; }
        }

        public string ImageDocType
        {
            get { return _imageDocType; }
            set { _imageDocType = value; }
        }
        #endregion
    }
}