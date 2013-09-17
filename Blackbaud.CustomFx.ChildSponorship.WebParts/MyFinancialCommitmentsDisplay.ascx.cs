using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using Blackbaud.AppFx.WebAPI;
using Blackbaud.AppFx.WebAPI.ServiceProxy;
using Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.DataLists.Constituent;
using Blackbaud.AppFx.Fundraising.Catalog.WebApiClient.DataLists.Constituent;
using Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.ViewForms.Sponsorship;
using Blackbaud.AppFx.Fundraising.Catalog.V1_1.WebApiClient.ViewForms.Revenue;
using Blackbaud.AppFx.Fundraising.Catalog.WebApiClient.DataLists.Revenue;
using Blackbaud.AppFx.Fundraising.Catalog.WebApiClient.AddForms.Revenue;
using Blackbaud.AppFx.Fundraising.Catalog.WebApiClient.ViewForms.Revenue;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class MyFinancialCommitmentsDisplay : BBNCExtensions.Parts.CustomPartDisplayBase
    {
        private MyFinancialCommitmentsOptions _options;
        private MyFinancialCommitmentsOptions options
        {
            get
            {
                if (this._options == null)
                {
                    this._options = (MyFinancialCommitmentsOptions)this.Content.GetContent(typeof(MyFinancialCommitmentsOptions));
                }

                return this._options;
            }
        }

        public string GetIsEnabled(string autoPay)
        {
            return (autoPay == "Yes") ? "false" : "true";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.bindCommitments();                
            }
        }       
        

        private void bindCommitments()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Commitment");
            dt.Columns.Add("Amount");
            dt.Columns.Add("AutoPay");
            dt.Columns.Add("Frequency");

            DataListLoadRequest request = SponsorshipDataList.CreateRequest(this.API.AppFxWebServiceProvider);
            request.DataListID = new Guid("b87f9b24-91e9-40c2-b54a-f6fbcc09f6f0");
            if (Request.QueryString["Id"] != null)
            {
                request.ContextRecordID = Convert.ToString(Request.QueryString["Id"]);
            }
            else
            {
                request.ContextRecordID = this.API.Users.CurrentUser.BackOfficeGuid.ToString();
            }

            SponsorshipDataListRow[] rows = SponsorshipDataList.GetRows(this.API.AppFxWebServiceProvider, request);

            foreach (SponsorshipDataListRow row in rows)
            {
                DataFormLoadRequest sponsorshipRequest = SponsorshipPaymentViewForm.CreateRequest(this.API.AppFxWebServiceProvider);
                sponsorshipRequest.FormID = new Guid("aeed02c3-ddba-41bc-8e7c-341c77068875");
                sponsorshipRequest.ContextRecordID = row.SPONSORSHIPID.ToString();
                sponsorshipRequest.RecordID = row.SPONSORSHIPID.ToString();
                SponsorshipPaymentViewFormData data = SponsorshipPaymentViewForm.LoadData(this.API.AppFxWebServiceProvider, sponsorshipRequest);

                DataFormLoadRequest paymentRequest = RevenuePaymentDetailsByTransactionViewForm.CreateRequest(this.API.AppFxWebServiceProvider);
                paymentRequest.FormID = new Guid("bafca6f5-4cdb-4173-aece-113713d38bfb");
                paymentRequest.ContextRecordID = row.REVENUEID.ToString();
                paymentRequest.RecordID = row.REVENUEID.ToString();
                RevenuePaymentDetailsByTransactionViewFormData paymentData = RevenuePaymentDetailsByTransactionViewForm.LoadData(this.API.AppFxWebServiceProvider, paymentRequest);

                DataRow dr = dt.NewRow();

                dr["Commitment"] = row.NAME + " (" + row.COMMITMENTID + ")";
                if (data.BASEAMOUNT.HasValue) dr["Amount"] = data.BASEAMOUNT.Value.ToString("c");
                dr["Frequency"] = data.FREQUENCY;
                dr["AutoPay"] = paymentData.AUTOPAY.Value ? "Yes" : "No";
                dr["Id"] = row.REVENUEID.ToString();

                dt.Rows.Add(dr);
            }

            DataListLoadRequest giftRequest = ConstituentExpandedRevenueHistoryList.CreateRequest(this.API.AppFxWebServiceProvider);
            giftRequest.DataListID = new Guid("4c2acb2c-a819-4092-a599-e43102824b1e");
            giftRequest.ContextRecordID = this.API.Users.CurrentUser.BackOfficeGuid.ToString();
            ConstituentExpandedRevenueHistoryListFilterData filter = new ConstituentExpandedRevenueHistoryListFilterData();
            filter.TRANSACTIONTYPEOPTIONCODE_IDVALUE = AppFx.Fundraising.Catalog.WebApiClient.DataLists.Constituent.ConstituentExpandedRevenueHistoryListEnums.TRANSACTIONTYPEOPTIONCODE.Recurring_Gifts;

            ConstituentExpandedRevenueHistoryListRow[] giftRows = ConstituentExpandedRevenueHistoryList.GetRows(this.API.AppFxWebServiceProvider, giftRequest);

            foreach (ConstituentExpandedRevenueHistoryListRow row in giftRows)
            {
                if (row.TRANSACTIONTYPECODE != "Recurring gift") continue;
                if (row.REVENUETYPE == "Sponsorship") continue;

                DataRow dr = dt.NewRow();

                DataFormLoadRequest sponsorshipRequest = RevenueTransactionProfileViewForm.CreateRequest(this.API.AppFxWebServiceProvider);
                sponsorshipRequest.FormID = new Guid("74f1a311-2ed9-4c0b-b666-9bf318748d9b");
                sponsorshipRequest.ContextRecordID = row.REVENUEID.ToString();
                sponsorshipRequest.RecordID = row.REVENUEID.ToString();
                RevenueTransactionProfileViewFormData data = RevenueTransactionProfileViewForm.LoadData(this.API.AppFxWebServiceProvider, sponsorshipRequest);

                DataFormLoadRequest paymentRequest = RevenuePaymentDetailsByTransactionViewForm.CreateRequest(this.API.AppFxWebServiceProvider);
                paymentRequest.FormID = new Guid("bafca6f5-4cdb-4173-aece-113713d38bfb");
                paymentRequest.ContextRecordID = row.REVENUEID.ToString();
                paymentRequest.RecordID = row.REVENUEID.ToString();
                RevenuePaymentDetailsByTransactionViewFormData paymentData = RevenuePaymentDetailsByTransactionViewForm.LoadData(this.API.AppFxWebServiceProvider, paymentRequest);

                dr["Commitment"] = row.DETAIL;
                dr["Amount"] = row.AMOUNT.ToString("c");
                dr["Frequency"] = data.FREQUENCY;
                dr["AutoPay"] = paymentData.AUTOPAY.Value ? "Yes" : "No";
                dr["Id"] = row.REVENUEID.ToString();

                dt.Rows.Add(dr);
            }

            this.gvCommitments.DataSource = dt;
            this.gvCommitments.DataBind();
        }

        private void bindPayments(string id)
        {
            this.mvMain.SetActiveView(this.viewPayments);

            DataListLoadRequest request = RecurringGiftInstallmentHistoryDataList.CreateRequest(this.API.AppFxWebServiceProvider);
            request.DataListID = new Guid("4d326172-d5d5-4966-a91c-0c0d2b02b155");
            request.ContextRecordID = id;            
            RecurringGiftInstallmentHistoryDataListRow[] rows = RecurringGiftInstallmentHistoryDataList.GetRows(this.API.AppFxWebServiceProvider, request);

            DataTable dt = new DataTable();
            dt.Columns.Add("ACTIVITYTYPE");
            dt.Columns.Add("AMOUNT");
            dt.Columns.Add("BALANCE");
            dt.Columns.Add("DATE");

            foreach (RecurringGiftInstallmentHistoryDataListRow row in rows)
            {
                if (row.ACTIVITYTYPE == "Installment") continue;
                
                DataRow dr = dt.NewRow();

                dr["ACTIVITYTYPE"] = row.ACTIVITYTYPE;
                dr["AMOUNT"] = row.AMOUNT.ToString("c");
                dr["BALANCE"] = row.BALANCE;
                dr["DATE"] = row.DATE.Value.ToShortDateString();

                dt.Rows.Add(dr);
            }

            this.gvPayments.DataSource = dt;
            this.gvPayments.DataBind();
        }

        protected void gvCommitments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewPayments")
            {
                this.bindPayments(e.CommandArgument.ToString());
            }
            else if (e.CommandName == "MakePayment")
            {
                ViewState["GiftId"] = e.CommandArgument.ToString();                
                this.populateYears();
                this.mvMain.SetActiveView(this.viewPay);        
                this.txtAmount.Text = Utility.GetSponsorshipAmount().ToString("c").Replace("$", "").Replace(",", "");
            }
        }

        protected void lnkBack_Click(object sender, EventArgs e)
        {
            this.mvMain.SetActiveView(this.viewCommitments);            
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblError.Visible = false;
                DateTime cardDate = new DateTime(Convert.ToInt32(this.cmbCcExpYear.SelectedValue), Convert.ToInt32(this.cmbCcExpMonth.SelectedValue), 1);
                if (cardDate < DateTime.Now)
                {
                    this.lblError.Visible = true;
                    this.lblError.Text = "Credit Card is expired.";
                }
                else
                {
                    if (this.processPayment())
                    {
                        this.submitPayment();
                        this.resetForm();
                        this.lblMessage.Text = this.options.ThankYouMessage;

                        this.mvMain.SetActiveView(this.viewThankYou);
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message + "<br /><br />" + ex.StackTrace;
                this.lblError.Visible = true;
            }

        }

        private void submitPayment()
        {
            DataFormLoadRequest request = PaymentAddForm.CreateLoadRequest(this.API.AppFxWebServiceProvider);
            request.FormID = new Guid("3e5b7b99-fb01-49d4-9020-c953006b7d0f");

            PaymentAddFormData data = PaymentAddForm.LoadData(this.API.AppFxWebServiceProvider, request);
            data.CONSTITUENTID = this.API.Users.CurrentUser.BackOfficeGuid;
            data.DATE = DateTime.Now;
            data.AMOUNT = Convert.ToDecimal(this.txtAmount.Text);
            data.PAYMENTMETHODCODE_IDVALUE = AppFx.Fundraising.Catalog.WebApiClient.AddForms.Revenue.PaymentAddFormEnums.PAYMENTMETHODCODE.Credit_Card;
            data.APPLICATIONCODE_IDVALUE = AppFx.Fundraising.Catalog.WebApiClient.AddForms.Revenue.PaymentAddFormEnums.APPLICATIONCODE.Recurring_Gift;
            data.REVENUESTREAMS = new PaymentAddFormData.REVENUESTREAMS_DATAITEM[1];
            data.REVENUESTREAMS[0] = new PaymentAddFormData.REVENUESTREAMS_DATAITEM();
            data.REVENUESTREAMS[0].APPLICATIONCODE_IDVALUE = AppFx.Fundraising.Catalog.WebApiClient.AddForms.Revenue.PaymentAddFormEnums.REVENUESTREAMS_APPLICATIONCODE.Recurring_Gift;
            data.REVENUESTREAMS[0].APPLIED = Convert.ToDecimal(this.txtAmount.Text);
            //data.REVENUESTREAMS[0].ID = new Guid("CB4BE127-694E-4645-828C-F099E87C7E91");
            data.REVENUESTREAMS[0].APPLICATIONID = new Guid(Convert.ToString(ViewState["GiftId"]));
            data.CREDITCARDNUMBER = this.txtCcNumber.Text;
            data.CARDHOLDERNAME = this.txtCcName.Text;
            data.EXPIRESON = new AppFx.FuzzyDate(Convert.ToInt32(this.cmbCcExpYear.SelectedValue), Convert.ToInt32(this.cmbCcExpMonth.SelectedValue));
            data.CREDITTYPECODEID = Utility.GetCrmCC(this.cmbCcType.SelectedValue);
            data.RECEIPTAMOUNT = Convert.ToDecimal(this.txtAmount.Text);
            data.SOURCECODE = "BBIS";            

            data.Save(this.API.AppFxWebServiceProvider);
        }

        private bool processPayment()
        {
            MyFinancialCommitmentsOptions options = (MyFinancialCommitmentsOptions)this.Content.GetContent(typeof(MyFinancialCommitmentsOptions));
                        
            BBPSPaymentInfo payment = new BBPSPaymentInfo();
            payment.DemoMode = options.DemoMode;
            payment.MerchantAcctID = options.MerchantAccountID;
            payment.Bbpid = Utility.GetBbbid(options.MerchantAccountID, this.API.Transactions.MerchantAccounts);
            payment.SkipCardValidation = options.DemoMode;  
            
            int designationId = Utility.GetBbncDesignationId(Convert.ToString(ViewState["GiftId"]));            
            payment.AddDesignationInfo(Convert.ToDecimal(this.txtAmount.Text), "BBIS Child Sponsorship Transaction", designationId);
            payment.AppealID = 1;
            payment.Comments = "";
             
            payment.PaymentMethod = BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod.CreditCard;
            payment.CreditCardCSC = this.txtCcSecurityCode.Text;
            payment.CreditCardExpirationMonth = Convert.ToInt32(this.cmbCcExpMonth.SelectedValue);
            payment.CreditCardExpirationYear = Convert.ToInt32(this.cmbCcExpYear.SelectedValue);
            payment.CreditCardHolderName = this.txtCcName.Text;
            payment.CreditCardNumber = this.txtCcNumber.Text; //VIOLATION of PCI Compliance - as a developer we can by no means ever write code that consumes someones credit card number
            payment.CreditCardType = (BBNCExtensions.Interfaces.Services.CreditCardType)Enum.Parse(typeof(BBNCExtensions.Interfaces.Services.CreditCardType), this.cmbCcType.SelectedValue);

            payment.DonorStreetAddress = this.txtBillingAddress.Text;
            payment.DonorCity = this.txtBillingCity.Text;
            payment.DonorStateProvince = this.cmbBillingCountry.SelectedValue == "US" ? this.cmbBillingState.SelectedValue : this.txtBillingRegion.Text;
            payment.DonorZIP = this.txtBillingZip.Text;                  
            
            BBNCExtensions.API.Transactions.Donations.RecordDonationReply reply = this.API.Transactions.RecordDonation(payment.GeneratePaymentArgs());

            this.lblSummary.Text = "Payment of " + Convert.ToDecimal(this.txtAmount.Text).ToString("c") + " accepted.";
            
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

        private void resetForm()
        {
            this.txtAmount.Text = "";
            this.txtBillingAddress.Text = "";
            this.txtBillingAddress2.Text = "";
            this.txtBillingCity.Text = "";
            this.cmbBillingState.ClearSelection();
            this.txtBillingRegion.Text = "";
            this.cmbBillingCountry.SelectedValue = "US";
            this.txtCcName.Text = "";
            this.txtCcNumber.Text = "";
            this.txtBillingZip.Text = "";
            this.txtCcSecurityCode.Text = "";
            this.cmbCcExpMonth.ClearSelection();
            this.cmbCcExpYear.ClearSelection();
            this.cmbCcType.ClearSelection();
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

        protected void lnkPayBack_Click(object sender, EventArgs e)
        {
            this.mvMain.SetActiveView(this.viewCommitments);
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

        protected void lnkThanksBack_Click(object sender, EventArgs e)
        {
            this.mvMain.SetActiveView(this.viewCommitments);
        }
    }
}
