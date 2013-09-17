using System;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public class SponsorshipSearchOptions
    {
        #region Fields
        private string _thumbnailNoteType;
        private int _resultsPerPage;
        private int _moreInfoPageID;
        private int _sponsorPageID;
        #endregion

        #region Properties
        public string ThumbnailNoteType
        {
            get { return _thumbnailNoteType; }
            set { _thumbnailNoteType = value; }
        }
        
        public int ResultsPerPage
        {
            get { return _resultsPerPage; }
            set { _resultsPerPage = value; }
        }
        
        public int MoreInfoPageID
        {
            get { return _moreInfoPageID; }
            set { _moreInfoPageID = value; }
        }

        public int SponsorPageID
        {
            get { return _sponsorPageID; }
            set { _sponsorPageID = value; }
        }
        #endregion
    }
}