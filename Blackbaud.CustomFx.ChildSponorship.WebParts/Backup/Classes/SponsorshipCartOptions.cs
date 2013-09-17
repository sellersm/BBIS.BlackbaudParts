using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public class SponsorshipCartOptions
    {
        #region Fields
        private int _paymentFormPageID;
        #endregion

        #region Properties
        public int PaymentFormPageID
        {
            get { return _paymentFormPageID; }
            set { _paymentFormPageID = value; }
        }
        #endregion
    }
}