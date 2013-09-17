using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class MergeFieldsData : Blackbaud.Web.Content.Core.Data.IDataProvider2
{
    private MyMergeData[] _data;

    public MergeFieldsData(MyMergeData[] data)
    {
        _data = data;
    }
        
    public int GetEFieldType
    {
        get
        {
            return MergeFieldsProvider.FIELD_TYPE;
        }
        set
        {

        }
    }
    
    public string GetFieldById(int fieldId, int attributeId)
    {
        return GetFieldById(fieldId, attributeId, 0);
    }

    public string GetFieldById(int fieldId, int attributeId, int rowNumber)
    {
        return GetFieldById(fieldId, attributeId, rowNumber.ToString()); 
    }

    public string GetFieldById(int fieldId, int attributeId, string rowNumber)
    {
        try
        {
            if(rowNumber.Trim().Length.Equals(0))
            {
                rowNumber = "0";
            }

            int[] rowNum = Array.ConvertAll(rowNumber.Split(new char[]{'_'}), new Converter<string,int>(Convert.ToInt32));

            int max = rowNum.Length - 1;

            switch ((MergeFieldsProvider.eMyFields)fieldId)
            {
                case MergeFieldsProvider.eMyFields.DONORFIRSTNAME:
                    return this._data[rowNum[0]].DonorFirstName;                    
                case MergeFieldsProvider.eMyFields.DONORMIDDLENAME:
                    return this._data[rowNum[0]].DonorMiddleName;                    
                case MergeFieldsProvider.eMyFields.DONORLASTNAME:
                    return this._data[rowNum[0]].DonorLastName;                    
                case MergeFieldsProvider.eMyFields.DONORADDRESSLINES:
                    return this._data[rowNum[0]].DonorAddressLines;                    
                case MergeFieldsProvider.eMyFields.DONORCITY:
                    return this._data[rowNum[0]].DonorCity;                    
                case MergeFieldsProvider.eMyFields.DONORSTATE:
                    return this._data[rowNum[0]].DonorState;                    
                case MergeFieldsProvider.eMyFields.DONORZIP:
                    return this._data[rowNum[0]].DonorZipCode;                    
                case MergeFieldsProvider.eMyFields.DONORCOUNTRY:
                    return this._data[rowNum[0]].DonorCountry;                    
                case MergeFieldsProvider.eMyFields.DONORDAYPHONE:
                    return this._data[rowNum[0]].DonorDayPhone;                    
                case MergeFieldsProvider.eMyFields.DONOREVENINGPHONE:
                    return this._data[rowNum[0]].DonorEveningPhone;                    
                case MergeFieldsProvider.eMyFields.DONOREMAIL:
                    return this._data[rowNum[0]].DonorEmail;                    
                case MergeFieldsProvider.eMyFields.DONORCURRENTSPONSOR:
                    return this._data[rowNum[0]].DonorCurrentSponsor;               
                case MergeFieldsProvider.eMyFields.DONORHEARABOUTUS:
                    return this._data[rowNum[0]].DonorHearAboutUs;                    
                case MergeFieldsProvider.eMyFields.CREDITCARDTYPE:
                    return this._data[rowNum[0]].CreditCardType;                    
                case MergeFieldsProvider.eMyFields.CREDITCARDNUMBER:
                    return this._data[rowNum[0]].CreditCardNumber;                    
                case MergeFieldsProvider.eMyFields.CREDITCARDHOLDERNAME:
                    return this._data[rowNum[0]].CreditCardHolderName;                    
                case MergeFieldsProvider.eMyFields.PAYMENTRECURRENCE:
                    return this._data[rowNum[0]].PaymentRecurrence;                    
                case MergeFieldsProvider.eMyFields.TOTALCOST:
                    return this._data[rowNum[0]].TotalCost;                    
                case MergeFieldsProvider.eMyFields.CHILDNAME:
                    return this._data[rowNum[0]].Children[rowNum[max]].Name;                    
                case MergeFieldsProvider.eMyFields.CHILDLOCATION:
                    return this._data[rowNum[0]].Children[rowNum[max]].Location;                    
                case MergeFieldsProvider.eMyFields.CHILDDOB:
                    return this._data[rowNum[0]].Children[rowNum[max]].DOB;                    
                case MergeFieldsProvider.eMyFields.CHILDGENDER:
                    return this._data[rowNum[0]].Children[rowNum[max]].Gender;                                    
                default:
                    break;
            }            
        }
        catch { }

        return string.Empty;
    }

    #region Ignore
    public System.Data.DataTable GetChildRecords(int fieldCategory, string fieldIds)
    {
        throw new NotImplementedException();
    }

    public string GetFieldNameById(int fieldId, int attributeId, bool fullyQualified = false)
    {
        throw new NotImplementedException();
    }

    public int GetMultiRowRecordCount(int fieldCategory)
    {
        throw new NotImplementedException();
    }

    public bool IsFieldPrivate(int fieldId, int attributeId)
    {
        throw new NotImplementedException();
    }

    public int NewRecord(int fieldCategory)
    {
        throw new NotImplementedException();
    }

    public void SetFieldById(int fieldId, int attributeId, int rowNumber, string value)
    {
        throw new NotImplementedException();
    }

    public void SetFieldById(int fieldId, int attributeId, string[] value)
    {
        throw new NotImplementedException();
    }

    public void SetFieldById(int fieldId, int attributeId, string value)
    {
        throw new NotImplementedException();
    }
    #endregion
}