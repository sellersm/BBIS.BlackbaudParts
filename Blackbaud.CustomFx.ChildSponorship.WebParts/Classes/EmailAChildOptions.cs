using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blackbaud.CustomFx.ChildSponorship.WebParts
{
    public class EmailAChildOptions : ConfirmationEmailOptions
    {
        #region Fields
        private string _toAddress;
        private string _linkHtml;
        private string _subject;
        private int _successPageID;
        private int _errorPageID;
        private int _mySponsorshipsPageID;
        #endregion

        #region Properties
        public string ToAddress
        {
            get { return _toAddress; }
            set { _toAddress = value; }
        }
        
        public string LinkHtml
        {
            get { return _linkHtml; }
            set { _linkHtml = value; }
        }

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        public int SuccessPageID
        {
            get { return _successPageID; }
            set { _successPageID = value; }
        }

        public int ErrorPageID
        {
            get { return _errorPageID; }
            set { _errorPageID = value; }
        }

        public int MySponsorshipsPageID
        {
            get { return _mySponsorshipsPageID; }
            set { _mySponsorshipsPageID = value; }
        }
        #endregion
    }
}