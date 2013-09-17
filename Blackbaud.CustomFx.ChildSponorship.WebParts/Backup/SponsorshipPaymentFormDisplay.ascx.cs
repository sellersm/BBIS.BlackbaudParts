#region Usings
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Blackbaud.AppFx.WebAPI.ServiceProxy;
using Const = Blackbaud.AppFx.Constituent.Catalog.WebApiClient;
using Blackbaud.AppFx.Fundraising.Catalog.WebApiClient.AddForms.Revenue;
using Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship;
using MoM = Blackbaud.CustomFx.MissionOfMercy.Catalog;
using Blackbaud.Web.Content.Core;
using Blackbaud.AppFx.XmlTypes.DataForms;
using System.Data.SqlClient;
#endregion

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class SponsorshipPaymentFormDisplay : BBNCExtensions.Parts.CustomPartDisplayBase
    {
        private const string c_Referrer = "REFERRER";
        private const bool c_DEBUG = true; //if this is set to true and its my machine then it puts default data in for me 

        private SponsorshipPaymentFormOptions _myContent;
        private SponsorshipPaymentFormOptions MyContent
        {
            get
            {
                if (_myContent == null)
                {
                    _myContent = (SponsorshipPaymentFormOptions)this.Content.GetContent(typeof(SponsorshipPaymentFormOptions));

                    if (_myContent == null)
                    {
                        _myContent = new SponsorshipPaymentFormOptions();
                    }
                }

                return _myContent;
            }
        }

        private string BatchNumber
        {
            get
            {                
                string result = DateTime.Now.ToString("yyyyMMdd");

                if(!string.IsNullOrWhiteSpace(MyContent.BatchNumberPrefix))
                {
                    result = string.Format("{0}-{1}", MyContent.BatchNumberPrefix, result); 
                }

                return result;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session[c_Referrer] == null)
                {
                    Session[c_Referrer] = (Request.UrlReferrer == null ? null : Request.UrlReferrer.ToString());
                }
                lblReferrer.Text = (Session[c_Referrer] == null ? String.Empty : Session[c_Referrer].ToString());

                if (this.total == 0)
                {
                    //    Response.Redirect("/");
                }
                try
                {
                    this.populateYears();
                    this.populateHearAbout();
                    this.showTotal();

#if DEBUG
                    if (System.Environment.MachineName.Equals("CHS6CHRISWHI02"))
                    {
                        LoadDefaultPaymentInfo();
                    }
#endif
                }
                catch
                {
                    //ignore
                }
            }
        }

        private void LoadDefaultPaymentInfo()
        {
            txtFirstName.Text = "Chris";
            txtLastName.Text = "Whisenhunt";
            txtAddress.Text = "1333 Laurel Creek Road";
            txtCity.Text = "Fayetteville";
            txtZip.Text = "25840";
            txtDayPhone.Text = "(843) 991-5686";
            txtEmail.Text = "chris.whisenhunt@blackbaud.com";
            txtCcName.Text = "Chris Whisenhunt";
            txtCcNumber.Text = "4242424242424242";
            txtCcSecurityCode.Text = "123";
            cmbState.SelectedValue = "WV";
            cmbHearAbout.SelectedValue = "Internet";
            cmbCcType.SelectedValue = "Visa";
            cmbCcExpMonth.SelectedValue = "12";
            cmbCcExpYear.SelectedValue = "2016";
            radCcRecurrence.SelectedIndex = 0;

            //txtFirstName.Text = "Roxie";
            //txtLastName.Text = "BigDog";
            //txtAddress.Text = "503 Maclean Drive";
            //txtCity.Text = "Fayetteville";
            //txtZip.Text = "25840";
            //txtDayPhone.Text = "(336) 446-6614";
            //txtEmail.Text = "chris.whisenhunt@blackbaud.com";
            //txtCcName.Text = "ROxie BigDog";
            //txtCcNumber.Text = "4242424242424242";
            //txtCcSecurityCode.Text = "123";
            //cmbState.SelectedValue = "WV";
            //cmbHearAbout.SelectedValue = "Internet";
            //cmbCcType.SelectedValue = "Visa";
            //cmbCcExpMonth.SelectedValue = "12";
            //cmbCcExpYear.SelectedValue = "2016";
            //radCcRecurrence.SelectedIndex = 0;
        }

        private DataTable CartData
        {
            get
            {
                if (!c_DEBUG)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Id", typeof(Guid));
                    dt.Columns.Add("Gender", typeof(string));
                    dt.Columns.Add("Number", typeof(string));
                    dt.Columns.Add("Name", typeof(string));
                    dt.Columns.Add("Months", typeof(int));
                    dt.Columns.Add("Amount", typeof(decimal));
                    dt.Columns.Add("CountryName", typeof(string));
                    dt.Columns.Add("BirthDate", typeof(string));

                    DataRow dr = dt.NewRow();
                    dr["Id"] = new Guid("532CB04A-CDDC-4C5E-80B7-6B40B60B1D2F");
                    dr["Gender"] = "Male";
                    dr["Number"] = "C310290";
                    dr["Name"] = "Rhett";
                    dr["Months"] = "2";
                    dr["Amount"] = 34.00;
                    dr["CountryName"] = "Ethiopia";
                    dr["BirthDate"] = "10/04/1999";
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["Id"] = new Guid("48744258-51AE-4F72-A68D-EAD2A740F3B9");
                    dr["Gender"] = "Female";
                    dr["Number"] = "C211376";
                    dr["Name"] = "Roxanne";
                    dr["Months"] = "4";
                    dr["Amount"] = 34.00;
                    dr["CountryName"] = "Uganda";
                    dr["BirthDate"] = "11/06/1999";
                    dt.Rows.Add(dr);

                    return dt;
                }
                else
                {
                    return (DataTable)Session["CartData"];
                }
            }
            set { Session["CartData"] = value; }
        }

        private decimal total
        {
            get
            {
                decimal results = 0;
                if (this.CartData != null)
                {
                    foreach (DataRow dr in this.CartData.Rows)
                    {
                        results += Convert.ToInt32(dr["Months"]) * Convert.ToDecimal(dr["Amount"]);
                    }
                }
                return results;
            }
        }

        private void showTotal()
        {
            this.lblTotal.Text = "Total sponsorship amount is " + this.total.ToString("c");
        }

        private Guid createConsitutent(Guid stateID)
        {
            string addDataForm = "1f9671b3-6740-447c-ad15-ef2718c0e43a";

            DataFormLoadRequest request = Const.AddForms.Constituent.IndividualSpouseBusinessAddForm.CreateLoadRequest(this.API.AppFxWebServiceProvider);
            request.FormID = new Guid(addDataForm);
            Const.AddForms.Constituent.IndividualSpouseBusinessAddFormData data = Const.AddForms.Constituent.IndividualSpouseBusinessAddForm.LoadData(this.API.AppFxWebServiceProvider, request);

            data.FIRSTNAME = this.txtFirstName.Text;
            data.MIDDLENAME = this.txtMiddle.Text;
            data.LASTNAME = this.txtLastName.Text;
            data.ADDRESS_ADDRESSBLOCK = this.txtAddress.Text + " " + this.txtAddress2.Text;
            data.ADDRESS_CITY = this.txtCity.Text;
            data.ADDRESS_POSTCODE = this.txtZip.Text;
            data.ADDRESS_STATEID = stateID;
            data.PHONE_NUMBER = this.txtDayPhone.Text;
            data.EMAILADDRESS_EMAILADDRESS = this.txtEmail.Text;
            
            data.Save(this.API.AppFxWebServiceProvider);

            return new Guid(data.RecordID);
        }

        public void addGift(Guid constituentId)
        {
            DataFormLoadRequest request = RecurringGiftAddForm.CreateLoadRequest(this.API.AppFxWebServiceProvider);
            request.FormID = new Guid("47a3c222-5e99-44a2-a5ce-5989d18f5a13");
            RecurringGiftAddFormData data = RecurringGiftAddForm.LoadData(this.API.AppFxWebServiceProvider, request);

            data.AMOUNT = this.total;
            data.CONSTITUENTID = constituentId;
            data.CARDHOLDERNAME = this.txtCcName.Text;
            data.CREDITCARDNUMBER = this.txtCcNumber.Text;
            data.CREDITTYPECODEID = Utility.GetCrmCC(this.cmbCcType.SelectedValue);
            data.EXPIRESON = new Blackbaud.AppFx.FuzzyDate(Convert.ToInt32(this.cmbCcExpYear.SelectedValue), Convert.ToInt32(this.cmbCcExpMonth.SelectedValue));
            data.DATE = DateTime.Now;
            data.STARTDATE = DateTime.Now;
            data.SPLITS = new RecurringGiftAddFormData.SPLITS_DATAITEM[1];
            data.SPLITS[0] = new RecurringGiftAddFormData.SPLITS_DATAITEM();
            data.SPLITS[0].AMOUNT = this.total;
            data.SPLITS[0].DESIGNATIONID = new Guid("1B2EB1A9-2FC3-4AD1-A28C-2920799C3FDF");

            data.Save(this.API.AppFxWebServiceProvider);
        }

        private bool processPayment()
        {
            BBPSPaymentInfo payment = new BBPSPaymentInfo();

            //chriswh 6/20/2013
            //bug fix to stop the creation of the donation batch
            //since it is already being written directly to the system
            payment.SkipCreateGiftTransaction = true;

            payment.DemoMode = MyContent.DemoMode;
            payment.MerchantAcctID = MyContent.MerchantAccountID;
            payment.Bbpid = Utility.GetBbbid(MyContent.MerchantAccountID, this.API.Transactions.MerchantAccounts);
            payment.SkipCardValidation = MyContent.DemoMode;

            foreach (DataRow dr in this.CartData.Rows)
            {
                int designationId = Utility.GetBbncDesignationIdFromSponsorshipOpportunity(dr["Id"].ToString());
                decimal amount = Convert.ToInt32(dr["Months"]) * Convert.ToDecimal(dr["Amount"]);

                payment.AddDesignationInfo(amount, "BBIS Child Sponsorship Transaction", designationId);
            }

            payment.AppealID = 1;
            payment.Comments = "";

            if (this.radPayment.SelectedValue == "Check")
            {
                payment.PaymentMethod = BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod.Check;
            }
            else
            {
                payment.PaymentMethod = BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod.CreditCard;

                payment.CreditCardCSC = this.txtCcSecurityCode.Text;
                payment.CreditCardExpirationMonth = Convert.ToInt32(this.cmbCcExpMonth.SelectedValue);
                payment.CreditCardExpirationYear = Convert.ToInt32(this.cmbCcExpYear.SelectedValue);
                payment.CreditCardHolderName = this.txtCcName.Text;
                payment.CreditCardNumber = this.txtCcNumber.Text; //VIOLATION of PCI Compliance - as a developer we can by no means ever write code that consumes someones credit card number
                payment.CreditCardType = (BBNCExtensions.Interfaces.Services.CreditCardType)Enum.Parse(typeof(BBNCExtensions.Interfaces.Services.CreditCardType), this.cmbCcType.SelectedValue);

                if (this.radBilling.SelectedValue == "Yes")
                {
                    payment.DonorStreetAddress = this.txtAddress.Text;
                    payment.DonorCity = this.txtCity.Text;
                    payment.DonorStateProvince = this.cmbCountry.SelectedValue == "US" ? this.cmbState.SelectedValue : this.txtRegion.Text;
                    payment.DonorZIP = this.txtZip.Text;
                }
                else
                {
                    payment.DonorStreetAddress = this.txtBillingAddress.Text;
                    payment.DonorCity = this.txtBillingCity.Text;
                    payment.DonorStateProvince = this.cmbCountry.SelectedValue == "US" ? this.cmbBillingState.SelectedValue : this.txtBillingRegion.Text;
                    payment.DonorZIP = this.txtBillingZip.Text;
                }
            }

            payment.EmailAddress = this.txtEmail.Text;

            BBNCExtensions.API.Transactions.Donations.RecordDonationReply reply = this.API.Transactions.RecordDonation(payment.GeneratePaymentArgs());

            if (!payment.InterpretPaymentReply(reply).Success)
            {
                this.lblError.Visible = true;
                this.lblError.Text = payment.InterpretPaymentReply(reply).Message;
                return false;
            }
            else
            {              
                  
                return true;
            }
        }

        private void populateYears()
        {
            this.cmbCcExpYear.Items.Clear();
            this.cmbCcExpYear.Items.Add(new ListItem("-- Year --", ""));

            for (int i = 0; i < 15; i++)
            {
                int year = DateTime.Now.Year + i;
                this.cmbCcExpYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
        }

        private void populateHearAbout()
        {
            DataListLoadRequest request = MoM.DataLists.WebsiteSponsorshipCheckoutSourceDataList.CreateRequest(this.API.AppFxWebServiceProvider);
            request.DataListID = new Guid("37fe2ce9-1889-49b9-86fe-75cbd3af0af1");
            request.ContextRecordID = this.API.Users.CurrentUser.BackOfficeGuid.ToString();
            MoM.DataLists.WebsiteSponsorshipCheckoutSourceDataListRow[] rows = MoM.DataLists.WebsiteSponsorshipCheckoutSourceDataList.GetRows(this.API.AppFxWebServiceProvider, request);

            this.cmbHearAbout.Items.Clear();
            this.cmbHearAbout.Items.Add(new ListItem("-- Select --", ""));
            foreach (MoM.DataLists.WebsiteSponsorshipCheckoutSourceDataListRow row in rows)
            {
                ListItem item = new ListItem(row.SOURCEDESC, row.SOURCEDESC);
                item.Attributes.Add("data", row.ADDITIONALINFORMATIONCAPTION);
                this.cmbHearAbout.Items.Add(new ListItem(row.SOURCEDESC, row.SOURCEDESC));
            }
        }

        #region "UI behavioral methods"
        protected void cmbCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbCountry.SelectedValue == "US")
            {
                this.txtRegion.Enabled = false;
                this.cmbState.Enabled = true;
                this.reqState.Enabled = true;
                this.reqRegion.Enabled = false;
            }
            else
            {
                this.txtRegion.Enabled = true;
                this.cmbState.Enabled = false;
                this.reqState.Enabled = false;
                this.reqRegion.Enabled = true;
            }
        }

        protected void cmbBillingCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBillingCountry.SelectedValue == "US")
            {
                this.txtBillingRegion.Enabled = false;
                this.cmbBillingState.Enabled = true;
                this.reqBillingState.Enabled = true;
                this.reqBillingRegion.Enabled = false;
            }
            else
            {
                this.txtBillingRegion.Enabled = true;
                this.cmbBillingState.Enabled = false;
                this.reqBillingState.Enabled = false;
                this.reqBillingRegion.Enabled = true;
            }
        }

        protected void radBilling_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radBilling.SelectedValue == "Yes")
            {
                this.pnlBillingAddress.Visible = false;
            }
            else
            {
                this.pnlBillingAddress.Visible = true;
            }
        }

        protected void radIsSponsor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.radIsSponsor.SelectedValue == "Yes")
            {
                this.trSponsorId.Visible = true;
                this.trHearAboutSelection.Visible = false;
                this.trHearAbout.Visible = false;
            }
            else
            {
                this.trSponsorId.Visible = false;
                this.trHearAboutSelection.Visible = true;
                this.showHideHearAbout();
            }
        }

        protected void cmbHearAbout_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.showHideHearAbout();
        }

        protected void radPayment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radPayment.SelectedValue == "Check")
            {
                this.pnlCheck.Visible = true;
                this.pnlCC.Visible = false;
            }
            else
            {
                this.pnlCheck.Visible = false;
                this.pnlCC.Visible = true;
            }
        }

        private void showHideHearAbout()
        {
            DataListLoadRequest request = MoM.DataLists.WebsiteSponsorshipCheckoutSourceDataList.CreateRequest(this.API.AppFxWebServiceProvider);
            request.DataListID = new Guid("37fe2ce9-1889-49b9-86fe-75cbd3af0af1");
            request.ContextRecordID = this.API.Users.CurrentUser.BackOfficeGuid.ToString();
            MoM.DataLists.WebsiteSponsorshipCheckoutSourceDataListRow[] rows = MoM.DataLists.WebsiteSponsorshipCheckoutSourceDataList.GetRows(this.API.AppFxWebServiceProvider, request);

            foreach (MoM.DataLists.WebsiteSponsorshipCheckoutSourceDataListRow row in rows)
            {
                if (row.SOURCEDESC == this.cmbHearAbout.SelectedValue)
                {
                    this.lblHearAbout.Text = row.ADDITIONALINFORMATIONCAPTION;
                    this.trHearAbout.Visible = row.HASADDITIONALINFORMATION; // !String.IsNullOrWhiteSpace(row.ADDITIONALINFORMATIONCAPTION);
                    break;
                }
            }
        }
        #endregion

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (validCard())
                {
                    Guid constituentID = this.findConstituent();
                    //this.addGift(constituentID);

                    bool redirectToThankYou = false;
                                        
                    if (this.radPayment.SelectedValue == "Check")
                    {
                        this.createSponsorship(constituentID);
                        redirectToThankYou = true;
                    }
                    else
                    {
                        if (this.processPayment())
                        {
                            this.createSponsorship(constituentID);
                            redirectToThankYou = true;
                        }
                    }
                    
                    SendConfirmationEmail(GetMergeData());

                    CartData = null;

                    if (redirectToThankYou)
                    {
                        Utility.RedirectToBBISPage(MyContent.ThankYouPageID);
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Visible = true;
                this.lblError.Text = ex.Message + "<br /><br />" + ex.StackTrace;
            }
        }

        private bool validCard()
        {
            if (this.radPayment.SelectedValue == "Check") return true;

            DateTime cardDate = new DateTime(Convert.ToInt32(this.cmbCcExpYear.SelectedValue), Convert.ToInt32(this.cmbCcExpMonth.SelectedValue), 1);
            if (cardDate < DateTime.Now)
            {
                this.lblError.Visible = true;
                this.lblError.Text = "Credit Card is expired.";
                return false;
            }
            else
            {
                return true;
            }
        }

        private Guid findConstituent()
        {
            Guid id = Guid.Empty;

            SearchListLoadRequest request = Const.SearchLists.Constituent.ConstituentSearch.CreateRequest(this.API.AppFxWebServiceProvider);
            request.SearchListID = new Guid("23c5c603-d7d8-4106-aecc-65392b563887");

            Const.SearchLists.Constituent.ConstituentSearchFilterData data = new Const.SearchLists.Constituent.ConstituentSearchFilterData();
            data.CONSTITUENTTYPE = 1;
            
            Guid stateID = GetStateID(cmbState.Text);

            if (this.radIsSponsor.SelectedValue == "")
            {
                data.LOOKUPID = this.txtSponsorId.Text;
            }
            else
            {
                data.FIRSTNAME = this.txtFirstName.Text;
                data.KEYNAME = this.txtLastName.Text;
                data.CITY = this.txtCity.Text;
                data.POSTCODE = this.txtZip.Text;
                data.ADDRESSBLOCK = this.txtAddress.Text;                
            }

            string[] ids = Const.SearchLists.Constituent.ConstituentSearch.GetIDs(this.API.AppFxWebServiceProvider, data);

            if (ids.Length > 0)
            {
                Guid.TryParse(ids[0], out id);
            }
            else
            {
                id = this.createConsitutent(stateID);
            }

            return id;
        }

        private Guid GetStateID(string stateAbbreviation)
        {
            Guid stateID = Guid.Empty;

            var states = Blackbaud.AppFx.WebAPI.SimpleDataLists.LoadSimpleDataList(BBNCExtensions.API.NetCommunity.Current().AppFxWebServiceProvider, new Guid("7FA91401-596C-4F7C-936D-6E41683121D7"));

            foreach(var s in states)
            {
                if(s.Label.Equals(stateAbbreviation))
                {
                    stateID = new Guid(s.Value);
                }
            }

            return stateID;
        }

        private void createSponsorship(Guid constituentId)
        {
            foreach (DataRow dr in this.CartData.Rows)
            {
                DataFormLoadRequest request = SponsorshipAddForm.CreateLoadRequest(this.API.AppFxWebServiceProvider);
                request.FormID = new Guid("8a73db30-db6b-4f03-869e-3a649887fba7");
                SponsorshipAddFormData data = SponsorshipAddForm.LoadData(this.API.AppFxWebServiceProvider, request);

                data.REVENUECONSTITUENTID = constituentId;
                data.SPONSORSHIPCONSTITUENTID = constituentId;
                data.SPONSORSHIPOPPORTUNITYIDCHILD = new Guid(dr["Id"].ToString());

                if (this.radPayment.SelectedValue == "CC")
                {
                    data.PAYMENTMETHODCODE_IDVALUE = Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.PAYMENTMETHODCODE.Credit_Card;
                    data.CARDHOLDERNAME = this.txtCcName.Text;
                    data.CREDITCARDNUMBER = this.txtCcNumber.Text;
                    data.CREDITTYPECODEID = Utility.GetCrmCC(this.cmbCcType.SelectedValue);
                    data.FREQUENCYCODE_IDVALUE = this.getFrequency();
                    data.EXPIRESON = new Blackbaud.AppFx.FuzzyDate(Convert.ToInt32(this.cmbCcExpYear.SelectedValue), Convert.ToInt32(this.cmbCcExpMonth.SelectedValue));
                    data.AUTOPAY = true;
                }
                else
                {
                    data.AUTOPAY = false;
                }

                data.AMOUNT = Convert.ToDecimal(dr["Amount"]);

                data.SPONSORSHIPPROGRAMID = new Guid("32FA809A-5EF1-4A17-862C-7DFE0AB49F19");
                data.STARTDATE = DateTime.Now;
                data.REVENUESCHEDULESTARTDATE = DateTime.Now;
                data.GENDERCODE_IDVALUE = this.getGender(dr["Gender"].ToString());
                data.ISHIVPOSITIVECODE_IDVALUE = Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.ISHIVPOSITIVECODE.No;
                data.HASCONDITIONCODE_IDVALUE = Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.HASCONDITIONCODE.No;
                data.ISORPHANEDCODE_IDVALUE = Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.ISORPHANEDCODE.No;
                data.GIFTRECIPIENT = false;
                data.BATCHNUMBER = BatchNumber;

                data.Save(this.API.AppFxWebServiceProvider);

                Guid sponsorshipID = new Guid();

                if(Guid.TryParse(data.RecordID, out sponsorshipID))
                {
                    UpdateBatchNumber(sponsorshipID);
                }
            }
        }

        private void UpdateBatchNumber(Guid sponsorshipID)
        {
            using(SqlConnection con = new SqlConnection(Settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USR_USP_UPDATEBATCHNUMBERBYSPONSORSHIPID", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", sponsorshipID);
                    cmd.Parameters.AddWithValue("@BATCHNUMBER", BatchNumber);

                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #region Miscellaneous
        private Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.FREQUENCYCODE getFrequency()
        {
            switch (this.radCcRecurrence.SelectedValue)
            {
                case "3":
                    return Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.FREQUENCYCODE.Monthly;
                case "2":
                    return Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.FREQUENCYCODE.Quarterly;
                case "0":
                    return Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.FREQUENCYCODE.Annually;
            }

            return Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.FREQUENCYCODE.Monthly;
        }

        private Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.GENDERCODE getGender(string gender)
        {
            switch (gender)
            {
                case "boy":
                    return Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.GENDERCODE.Male;
                case "girl":
                    return Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.GENDERCODE.Female;
            }

            return Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.AddForms.Sponsorship.SponsorshipAddFormEnums.GENDERCODE.Male;
        }
        #endregion

        #region "Confirmation email and merge field methods"
        private List<MyMergeData> GetMergeData()
        {
            List<MyMergeData> mergeData = new List<MyMergeData>();

            MyMergeData myData = new MyMergeData();

            myData.DonorFirstName = txtFirstName.Text;
            myData.CreditCardHolderName = txtCcName.Text;

            string ccNumber = txtCcNumber.Text;

            myData.CreditCardNumber = ccNumber.Substring(ccNumber.Length - 4);
            myData.CreditCardType = cmbCcType.SelectedValue;
            myData.DonorAddressLines = string.Concat(txtAddress.Text, Environment.NewLine, txtAddress2.Text);
            myData.DonorCity = txtCity.Text;
            myData.DonorCountry = cmbCountry.SelectedValue;
            myData.DonorCurrentSponsor = radIsSponsor.SelectedValue;
            myData.DonorDayPhone = txtDayPhone.Text;
            myData.DonorEmail = txtEmail.Text;
            myData.DonorEveningPhone = txtNightPhone.Text;

            string hearAbout = string.Concat(cmbHearAbout.SelectedValue, " - ", txtHearAboutResponse.Text).Trim();
            myData.DonorHearAboutUs = (hearAbout.Substring(hearAbout.Length - 1).Equals("-") ? hearAbout.Substring(0, hearAbout.Length - 1) : hearAbout);

            myData.DonorLastName = txtLastName.Text;
            myData.DonorMiddleName = txtMiddle.Text;
            myData.DonorState = cmbState.Text;
            myData.DonorZipCode = txtZip.Text;
            myData.PaymentRecurrence = radCcRecurrence.SelectedValue;
            myData.TotalCost = this.total.ToString("c");

            List<ChildMergeData> children = null;

            if (CartData != null)
            {
                children = new List<ChildMergeData>();

                foreach (DataRow item in CartData.Rows)
                {
                    ChildMergeData kid = new ChildMergeData();
                    kid.Name = item["Name"].ToString();
                    kid.Gender = item["Gender"].ToString();
                    kid.DOB = item["BirthDate"].ToString();
                    kid.Location = item["CountryName"].ToString();
                    children.Add(kid);
                }

                myData.Children = children.ToArray();
            }

            mergeData.Add(myData);

            return mergeData;
        }

        private void SendConfirmationEmail(List<MyMergeData> mergeData)
        {
            var emailOptions = MyContent.EmailOptions;
            var template = new EmailTemplate(emailOptions.TemplateID);
            var email = new EMail(template);
            email.Save();

            var name = string.Empty;
            var emailAddress = string.Empty;

            var myData = mergeData[0];

            if (myData != null)
            {
                name = string.Concat(myData.DonorFirstName, " ", myData.DonorLastName).Trim();
                emailAddress = myData.DonorEmail;

                if (!string.IsNullOrWhiteSpace(emailAddress))
                {
                    MergeFieldsData[] provider = { new MergeFieldsData(mergeData.ToArray()) };
                    var html = emailOptions.HTML;

                    email.ContentHTML = Expand(emailOptions.HTML, mergeData.ToArray());
                    email.FromAddress = emailOptions.FromAddress;
                    email.FromDisplayName = emailOptions.FromName;
                    email.Subject = emailOptions.Subject;
                    email.ReplyAddress = emailOptions.ReplyAddress;

                    try
                    {
                        email.Send(emailAddress, name, API.Users.CurrentUser.RaisersEdgeID, API.Users.CurrentUser.UserID, provider, this.Page);
                    }
                    catch (Exception ex)
                    {
                        Common.LogErrorToDB(ex, false);
                    }
                }
            }
        }

        private string Expand(string html, IEnumerable<object> data)
        {
            if (html.IndexOf(HTMLHelper.MERGE_FIELD_ISLOOP_ATTR) < 0)
            {
                return html;
            }

            var sb = new System.Text.StringBuilder();
            var tw = new System.IO.StringWriter(sb);
            var hw = new HtmlTextWriter(tw);

            IEnumerable<string> sections = null;

            if (data is MyMergeData[])
            {
                sections = Blackbaud.Web.Content.Core.MergeFieldHelper.GetSections(ref html, (int)MergeFieldsProvider.eMyFields.BEGINSPONSORSHIPLOOP, (int)MergeFieldsProvider.eMyFields.ENDSPONSORSHIPLOOP, true);
            }
            else if (data is ChildMergeData[])
            {
                sections = Blackbaud.Web.Content.Core.MergeFieldHelper.GetSections(ref html, (int)MergeFieldsProvider.eMyFields.BEGINCHILDLOOP, (int)MergeFieldsProvider.eMyFields.ENDCHILDLOOP, true);
            }

            if (sections != null)
            {
                foreach (var sectionHTML in sections)
                {
                    if (!string.IsNullOrWhiteSpace(sectionHTML))
                    {
                        int itemRowNumber = 0;

                        foreach (var dataItem in data)
                        {
                            string workString = sectionHTML;

                            if (dataItem is MyMergeData)
                            {
                                workString = Expand(workString, ((MyMergeData)dataItem).Children);
                            }

                            Control htmlControl = Page.ParseControl(string.Concat(HTMLHelper.CORE_CONTROLS_REGISTER_DIRECTIVE, workString));

                            AssignRowNumbers(htmlControl.Controls, itemRowNumber);
                            htmlControl.RenderControl(hw);

                            itemRowNumber += 1;
                        }

                        html = html.Replace(sectionHTML, sb.ToString());
                    }
                }
            }

            return html;
        }

        private static void AssignRowNumbers(ControlCollection ctls, int itemRowNumber)
        {
            string MERGE_FIELD_ID_ATTR = "fieldid";
            string MERGE_FIELD_ROWNUMBER_ATTR = "rownumber";
            string MERGE_FIELD_ISLOOP_ATTR = "isloop";

            foreach (Control ctl in ctls)
            {
                System.Web.UI.HtmlControls.HtmlImage img = ctl as System.Web.UI.HtmlControls.HtmlImage;

                if (img != null)
                {
                    System.Web.UI.AttributeCollection attrs = img.Attributes;
                    int fieldID = DataObject.safeDBFieldIntValue(attrs[MERGE_FIELD_ID_ATTR]);
                    bool isLoop = DataObject.safeDBFieldBooleanValue(attrs[MERGE_FIELD_ISLOOP_ATTR]);

                    if (fieldID > 0)
                    {
                        if (!isLoop)
                        {
                            string rowNumAttr = attrs[MERGE_FIELD_ROWNUMBER_ATTR];

                            if (rowNumAttr == null)
                            {
                                rowNumAttr = "";
                            }

                            if (rowNumAttr.Length > 0)
                            {
                                rowNumAttr = string.Concat(itemRowNumber, "_", rowNumAttr);
                            }
                            else
                            {
                                rowNumAttr = itemRowNumber.ToString();
                            }

                            attrs[MERGE_FIELD_ROWNUMBER_ATTR] = rowNumAttr;
                        }

                        attrs["runat"] = "server";
                    }
                }

                if (ctl.Controls.Count > 0)
                {
                    AssignRowNumbers(ctl.Controls, itemRowNumber);
                }
            }
        }
        #endregion
        
        #region "Language Fields"
        public override void RegisterLanguageFields()
        {
            string LANGGUID_lblHowDidYouHearAboutUs = "18c9e932-fe88-421b-b4a5-447896209302";

            this.RegisterLanguageField(LANGGUID_lblHowDidYouHearAboutUs, lblHowDidYouHearAboutUs, "Hear about us", "How did you hear about us?", "Payment screen options", true);
        }
        #endregion
    }
}