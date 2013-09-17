using System.Data;

public partial class MergeFieldsProvider : Blackbaud.Web.Content.Core.Data.BaseFieldProvider
{
    public const int FIELD_TYPE = 06222013;

    protected override System.Data.DataSet GetFieldData()
    {
        if(m_fieldData == null)
        {
            m_fieldData = new DataSet();
            CreateFieldsTable(m_fieldData);

            DataTable table = m_fieldData.Tables["Fields"];

            AddField(table, FIELD_TYPE, (int)eMyFields.GENERALCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.BEGINSPONSORSHIPLOOP, (int)eMyFields.GENERALCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORCATEGORY, (int)eMyFields.GENERALCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORFIRSTNAME, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORMIDDLENAME, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORLASTNAME, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORADDRESSLINES, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORCITY, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORSTATE, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORZIP, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORCOUNTRY, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORDAYPHONE, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONOREVENINGPHONE, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONOREMAIL, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORCURRENTSPONSOR, (int)eMyFields.DONORCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.DONORHEARABOUTUS, (int)eMyFields.DONORCATEGORY);

            AddField(table, FIELD_TYPE, (int)eMyFields.PAYMENTCATEGORY, (int)eMyFields.GENERALCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.CREDITCARDTYPE, (int)eMyFields.PAYMENTCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.CREDITCARDNUMBER, (int)eMyFields.PAYMENTCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.CREDITCARDHOLDERNAME, (int)eMyFields.PAYMENTCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.TOTALCOST, (int)eMyFields.PAYMENTCATEGORY);

            AddField(table, FIELD_TYPE, (int)eMyFields.CHILDCATEGORY, (int)eMyFields.GENERALCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.BEGINCHILDLOOP, (int)eMyFields.CHILDCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.CHILDNAME, (int)eMyFields.CHILDCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.CHILDLOCATION, (int)eMyFields.CHILDCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.CHILDDOB, (int)eMyFields.CHILDCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.CHILDGENDER, (int)eMyFields.CHILDCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.ENDCHILDLOOP, (int)eMyFields.CHILDCATEGORY);
            AddField(table, FIELD_TYPE, (int)eMyFields.ENDSPONSORSHIPLOOP, (int)eMyFields.GENERALCATEGORY);

            m_fieldData.Relations.Add("FieldsTree", m_fieldData.Tables["Fields"].Columns["FieldId"], m_fieldData.Tables["Fields"].Columns["ParentFieldID"]);
        }

        return m_fieldData;
    }

    public override string GetFieldName(int fieldId, int attributeId, bool fullyQualified = false)
    {
        string category = GetCategoryForFieldName(fieldId, attributeId, fullyQualified);
        
        switch ((eMyFields)fieldId)
        {
            case eMyFields.DONORFIRSTNAME:
                return "First name";
            case eMyFields.DONORMIDDLENAME:
                return "Middle name";
            case eMyFields.DONORLASTNAME:
                return "Last name";
            case eMyFields.DONORADDRESSLINES:
                return "Address";
            case eMyFields.DONORCITY:
                return "City";
            case eMyFields.DONORSTATE:
                return "State";
            case eMyFields.DONORZIP:
                return "Zip";
            case eMyFields.DONORCOUNTRY:
                return "Country";
            case eMyFields.DONORDAYPHONE:
                return "Day phone";
            case eMyFields.DONOREVENINGPHONE:
                return "Evening phone";
            case eMyFields.DONOREMAIL:
                return "Email";
            case eMyFields.DONORCURRENTSPONSOR:
                return "Is current sponsor";
            case eMyFields.DONORHEARABOUTUS:
                return "How did you hear about us";
            case eMyFields.CREDITCARDTYPE:
                return "Credit card type";
            case eMyFields.CREDITCARDNUMBER:
                return "Credit card number";
            case eMyFields.CREDITCARDHOLDERNAME:
                return "Credit card holder name";
            case eMyFields.PAYMENTRECURRENCE:
                return "Payment recurrence";
            case eMyFields.TOTALCOST:
                return "Total cost";
            case eMyFields.BEGINCHILDLOOP:
                return "Begin child loop";
            case eMyFields.CHILDNAME:
                return "Name";
            case eMyFields.CHILDLOCATION:
                return "Location";
            case eMyFields.CHILDDOB:
                return "DOB";
            case eMyFields.CHILDGENDER:
                return "Gender";
            case eMyFields.ENDCHILDLOOP:
                return "End child loop";
            case eMyFields.DONORCATEGORY:
                return "Donor";
            case eMyFields.PAYMENTCATEGORY:
                return "Payment";
            case eMyFields.CHILDCATEGORY:
                return "Child";
            case eMyFields.BEGINSPONSORSHIPLOOP:
                return "Begin sponsorship loop";
            case eMyFields.ENDSPONSORSHIPLOOP:
                return "End sponsorship loop";
            case eMyFields.GENERALCATEGORY:
                return "General";
            default:
                break;
        }

        return "???";
    }
}