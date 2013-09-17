using System;

[Serializable()]
public class MyMergeData
{
    #region Fields
    private ChildMergeData[] _children;
    private string _donorFirstName;
    private string _donorMiddleName;
    private string _donorLastName;
    private string _donorAddressLines;
    private string _donorCity;
    private string _donorState;
    private string _donorCountry;
    private string _donorZipCode;
    private string _donorDayPhone;
    private string _donorEveningPhone;
    private string _donorEmail;
    private string _donorCurrentSponsor;    
    private string _donorHearAboutUs;
    private string _creditCardType;
    private string _creditCardNumber;
    private string _creditCardHolderName;
    private string _paymentRecurrence;
    private string _totalCost;
    #endregion

    #region Properties
    public ChildMergeData[] Children
    {
        get
        {
            return _children;
        }
        set
        {
            _children = value;
        }
    }

    public string DonorFirstName
    {
        get
        {
            return _donorFirstName;
        }
        set
        {
            _donorFirstName = value;
        }
    }

    public string DonorMiddleName
    {
        get
        {
            return _donorMiddleName;
        }
        set
        {
            _donorMiddleName = value;
        }
    }

    public string DonorLastName
    {
        get
        {
            return _donorLastName;
        }
        set
        {
            _donorLastName = value;
        }
    }
    
    public string DonorAddressLines
    {
        get
        {
            return _donorAddressLines;
        }
        set
        {
            _donorAddressLines = value;
        }
    }

    public string DonorCity
    {
        get
        {
            return _donorCity;
        }
        set
        {
            _donorCity = value;
        }
    }

    public string DonorState
    {
        get
        {
            return _donorState;
        }
        set
        {
            _donorState = value;
        }
    }

    public string DonorCountry
    {
        get
        {
            return _donorCountry;
        }
        set
        {
            _donorCountry = value;
        }
    }

    public string DonorZipCode
    {
        get
        {
            return _donorZipCode;
        }
        set
        {
            _donorZipCode = value;
        }
    }

    public string DonorDayPhone
    {
        get
        {
            return _donorDayPhone;
        }
        set
        {
            _donorDayPhone = value;
        }
    }

    public string DonorEveningPhone
    {
        get
        {
            return _donorEveningPhone;
        }
        set
        {
            _donorEveningPhone = value;
        }
    }

    public string DonorEmail
    {
        get
        {
            return _donorEmail;
        }
        set
        {
            _donorEmail = value;
        }
    }

    public string DonorCurrentSponsor
    {
        get
        {
            return _donorCurrentSponsor;
        }
        set
        {
            _donorCurrentSponsor = value;
        }
    }
    
    public string DonorHearAboutUs
    {
        get
        {
            return _donorHearAboutUs;
        }
        set
        {
            _donorHearAboutUs = value;
        }
    }

    public string CreditCardType
    {
        get
        {
            return _creditCardType;
        }
        set
        {
            _creditCardType = value;
        }
    }

    public string CreditCardNumber
    {
        get
        {
            return _creditCardNumber;
        }
        set
        {
            _creditCardNumber = value;
        }
    }

    public string CreditCardHolderName
    {
        get
        {
            return _creditCardHolderName;
        }
        set
        {
            _creditCardHolderName = value;
        }
    }

    public string PaymentRecurrence
    {
        get
        {
            return _paymentRecurrence;
        }
        set
        {
            _paymentRecurrence = value;
        }
    }

    public string TotalCost
    {
        get
        {
            return _totalCost;
        }
        set
        {
            _totalCost = value;
        }
    }
    #endregion    
}