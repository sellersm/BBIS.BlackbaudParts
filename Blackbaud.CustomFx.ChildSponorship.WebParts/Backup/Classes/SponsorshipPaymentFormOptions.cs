using System;

public class SponsorshipPaymentFormOptions
{
    #region Fields
    private int _thankYouPageID;
    private bool _demoMode;
    private int _merchantAccountID;
    private ConfirmationEmailOptions _emailOptions;
    private string _batchNumberPrefix;
    #endregion

    #region Properties
    public int ThankYouPageID
    {
        get { return this._thankYouPageID; }
        set { this._thankYouPageID = value; }
    }
        
    public bool DemoMode
    {
        get { return this._demoMode; }
        set { this._demoMode = value; }
    }
        
    public int MerchantAccountID
    {
        get { return this._merchantAccountID; }
        set { this._merchantAccountID = value; }
    }

    public ConfirmationEmailOptions EmailOptions
    {
        get { return _emailOptions; }
        set { _emailOptions = value; }
    }

    public string BatchNumberPrefix
    {
        get { return _batchNumberPrefix; }
        set { _batchNumberPrefix = value; }
    }
    #endregion
}