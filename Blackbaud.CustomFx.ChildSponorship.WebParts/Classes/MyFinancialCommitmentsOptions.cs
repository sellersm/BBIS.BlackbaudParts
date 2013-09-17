using System;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public class MyFinancialCommitmentsOptions
    {
        #region Fields
        private bool _demoMode;
        private string _thankYouMessage;
        private int _merchantAccountID;
        #endregion

        #region Properties
        public bool DemoMode
        {
            get { return _demoMode; }
            set { _demoMode = value; }
        }

        public string ThankYouMessage
        {
            get { return _thankYouMessage; }
            set { _thankYouMessage = value; }
        }
        public int MerchantAccountID
        {
            get { return this._merchantAccountID; }
            set { this._merchantAccountID = value; }
        }
        #endregion
    }
}