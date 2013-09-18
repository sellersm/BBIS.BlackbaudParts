using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using Blackbaud.AppFx.WebAPI;
using Blackbaud.AppFx.WebAPI.ServiceProxy;
using Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.DataLists.Constituent;
using Blackbaud.AppFx.Fundraising.Catalog.WebApiClient.DataLists.Constituent;
using Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.ViewForms.Sponsorship;
using Blackbaud.AppFx.Fundraising.Catalog.V1_1.WebApiClient.ViewForms.Revenue;
using Blackbaud.AppFx.Fundraising.Catalog.WebApiClient.DataLists.Revenue;
using Blackbaud.AppFx.Fundraising.Catalog.WebApiClient.AddForms.Revenue;
using Blackbaud.AppFx.Fundraising.Catalog.WebApiClient.ViewForms.Revenue;
using Blackbaud.CustomFx.ChildSponorship.WebParts.Classes;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class MySponsorshipsDisplay : BBNCExtensions.Parts.CustomPartDisplayBase
    {
        private MySponsorshipsOptions _myContent;
        private MySponsorshipsOptions MyContent
        {
            get
            {
                if (_myContent == null)
                {
                    _myContent = (MySponsorshipsOptions)this.Content.GetContent(typeof(MySponsorshipsOptions));

                    if (_myContent == null)
                    {
                        _myContent = new MySponsorshipsOptions();
                    }
                }

                return _myContent;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (this.API.Users.CurrentUser.BackOfficeGuid != Guid.Empty)
                //{
                    this.bindSponsorships();
                //}
            }
        }

        private void bindSponsorships()
        {
            DataTable dt = new DataTable();


            string sql = @"
                SELECT
                    so.ID,
                    so.LOOKUPID AS 'Child No',
                    sc.FIRSTNAME + ' ' + sc.LASTNAME AS 'Child Name',
                    sc.BIRTHDATE AS 'Birthdate',
                    sc.AGE AS 'Age',
                    r.ID AS 'RevenueId',
                    c.NAME as 'SPONSORNAME',
                    c.LOOKUPID as 'SPONSORID'
                FROM
                    SPONSORSHIP s
                    INNER JOIN SPONSORSHIPOPPORTUNITY so ON s.SPONSORSHIPOPPORTUNITYID = so.ID
                    INNER JOIN SPONSORSHIPOPPORTUNITYCHILD sc ON so.ID = sc.ID
                    INNER JOIN REVENUESPLIT rs ON s.REVENUESPLITID = rs.ID
                    INNER JOIN REVENUE r ON rs.REVENUEID = r.ID
                    left join CONSTITUENT C on s.CONSTITUENTID = c.ID
                WHERE
	                s.CONSTITUENTID = @Id
	                AND s.STATUS = 'Active'";

            using(SqlConnection con = new SqlConnection(Blackbaud.Web.Content.Core.Settings.ConnectionString))
            {
                using(SqlDataAdapter cmd = new SqlDataAdapter(sql, con))
                {
                    cmd.SelectCommand.CommandType = CommandType.Text;
                    if (this.API.Users.CurrentUser.BackOfficeGuid != Guid.Empty)
                    {
                        cmd.SelectCommand.Parameters.AddWithValue("@Id", this.API.Users.CurrentUser.BackOfficeGuid.ToString());
                    }
                    else
                    {
                        cmd.SelectCommand.Parameters.AddWithValue("@Id", "83D3F0A9-9645-4DCA-87A5-CC16AB1D5B4F");
                    }

                    con.Open();
                    cmd.Fill(dt);
                }
            }

            //dt.Columns.Add("Child No");
            //dt.Columns.Add("Child Name");
            //dt.Columns.Add("Age");
            //dt.Columns.Add("Birthdate");
            
            //DataListLoadRequest request = Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.DataLists.Constituent.SponsorshipDataList.CreateRequest(this.API.AppFxWebServiceProvider);
            //request.DataListID = new Guid("b87f9b24-91e9-40c2-b54a-f6fbcc09f6f0");
            //request.ContextRecordID = "7599754F-48F1-4CF0-95C5-4650B78366A3";
            //SponsorshipDataListRow[] rows = SponsorshipDataList.GetRows(this.API.AppFxWebServiceProvider, request);

            //foreach (SponsorshipDataListRow row in rows)
            //{
            //    DataFormLoadRequest opportunityRequest = SponsorshipOpportunityDetailViewForm.CreateRequest(this.API.AppFxWebServiceProvider);
            //    opportunityRequest.FormID = new Guid("20a1c198-6745-49f0-984d-3751558113f2");
            //    opportunityRequest.RecordID = row.SPONSORSHIPOPPORTUNITYID.ToString();
            //    //opportunityRequest.ContextRecordID = row.SPONSORSHIPOPPORTUNITYID.ToString();
            //    SponsorshipOpportunityDetailViewFormData opportunity = SponsorshipOpportunityDetailViewForm.LoadData(this.API.AppFxWebServiceProvider, opportunityRequest);
                
            //    DataFormLoadRequest nameRequest = SponsorshipOpportunityName.CreateRequest(this.API.AppFxWebServiceProvider);
            //    nameRequest.FormID = new Guid("0fec4381-a657-4627-a65b-8afab11af9c8");
            //    nameRequest.RecordID = row.SPONSORSHIPOPPORTUNITYID.ToString();
            //    SponsorshipOpportunityNameData nameData = SponsorshipOpportunityName.LoadData(this.API.AppFxWebServiceProvider, nameRequest);

            //    DataRow dr = dt.NewRow();
            //    dr["Child No"] = opportunity.LOOKUPID;
            //    dr["Child Name"] = nameData.SPONSORSHIPOPPORTUNITYNAME;
            //    dr["Age"] = opportunity.AGE;
            //    dr["Birthdate"] = opportunity.BIRTHDATE.Month + "/" + opportunity.BIRTHDATE.Day + "/" + opportunity.BIRTHDATE.Year;
            //    dt.Rows.Add(dr);
            //}

            this.gvSponsorships.DataSource = dt;
            this.gvSponsorships.DataBind();
        }

        protected void gvSponsorships_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {                
                DataRowView row = (DataRowView)e.Row.DataItem;                    

                string url = Utility.GetBBISPageUrl(MyContent.MoreInfoPageID);

                string moreInfoUrl = url + "{0}id=" + row["ID"];
                moreInfoUrl = moreInfoUrl.IndexOf("?") > -1 ? String.Format(moreInfoUrl, "&") : String.Format(moreInfoUrl, "?");
                moreInfoUrl = moreInfoUrl + "&sponsored=y";

                ((LinkButton)e.Row.FindControl("lnkNo")).PostBackUrl = moreInfoUrl;
                ((LinkButton)e.Row.FindControl("lnkName")).PostBackUrl = moreInfoUrl;
                ((ImageButton)e.Row.FindControl("imgThumbnail")).ImageUrl = "ImageHandler.ashx?context=sponsorship&type=" + MyContent.ThumbnailNoteType + "&id=" + row["Id"];
                ((ImageButton)e.Row.FindControl("imgThumbnail")).PostBackUrl = moreInfoUrl;    
                
                if(e.Row.FindControl("lnkEmail") != null)
                {
                    var lnkEmail = (HyperLink)e.Row.FindControl("lnkEmail");

                    EncryptedQueryString args = new EncryptedQueryString();
                    args["CHILDID"] = row["Child No"].ToString();
                    args["CHILDNAME"] = row["Child Name"].ToString();
                    args["SPONSORID"] = row["SPONSORID"].ToString(); //need to get this
                    args["SPONSORNAME"] = row["SPONSORNAME"].ToString(); //need to get this

                    string emailUrl = string.Concat(Utility.GetBBISPageUrl(MyContent.EmailPageID), string.Format("?args={0}", args.ToString()));

                    lnkEmail.NavigateUrl = emailUrl;
                }
            }
        }

        protected void gvSponsorships_RowCommand(object sender, GridViewCommandEventArgs e)
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

        private void submitPayment()
        {
            DataFormLoadRequest request = PaymentAddForm.CreateLoadRequest(this.API.AppFxWebServiceProvider);
            request.FormID = new Guid("3e5b7b99-fb01-49d4-9020-c953006b7d0f");
            
            List<string> giftsToProcess = new List<string>();
            if (ViewState["selectedSponsorships"] != null)
            {
                giftsToProcess = ViewState["selectedSponsorships"] as List<string>;
            }
            else
            {
                giftsToProcess.Add(ViewState["GiftId"].ToString());
            }

            decimal enteredAmount = Convert.ToDecimal(this.txtAmount.Text);
            decimal amount = enteredAmount / giftsToProcess.Count;
            decimal runningTotal = 0;
            amount = Math.Round(amount, 2);

            for (int i=0; i < giftsToProcess.Count; i++)
            {
                // if this is the last gift to process, check to see if we need to adjust for rounding
                if (i + 1 == giftsToProcess.Count)
                {
                    if (runningTotal + amount != enteredAmount) amount = enteredAmount - runningTotal;
                }

                PaymentAddFormData data = PaymentAddForm.LoadData(this.API.AppFxWebServiceProvider, request);
                data.CONSTITUENTID = this.API.Users.CurrentUser.BackOfficeGuid;
                data.DATE = DateTime.Now;
                data.AMOUNT = amount;
                data.PAYMENTMETHODCODE_IDVALUE = AppFx.Fundraising.Catalog.WebApiClient.AddForms.Revenue.PaymentAddFormEnums.PAYMENTMETHODCODE.Credit_Card;
                data.APPLICATIONCODE_IDVALUE = AppFx.Fundraising.Catalog.WebApiClient.AddForms.Revenue.PaymentAddFormEnums.APPLICATIONCODE.Recurring_Gift;
                data.REVENUESTREAMS = new PaymentAddFormData.REVENUESTREAMS_DATAITEM[1];
                data.REVENUESTREAMS[0] = new PaymentAddFormData.REVENUESTREAMS_DATAITEM();
                data.REVENUESTREAMS[0].APPLICATIONCODE_IDVALUE = AppFx.Fundraising.Catalog.WebApiClient.AddForms.Revenue.PaymentAddFormEnums.REVENUESTREAMS_APPLICATIONCODE.Recurring_Gift;
                data.REVENUESTREAMS[0].APPLIED = amount;
                data.REVENUESTREAMS[0].APPLICATIONID = new Guid(giftsToProcess[i]);
                data.CREDITCARDNUMBER = this.txtCcNumber.Text;
                data.CARDHOLDERNAME = this.txtCcName.Text;
                data.EXPIRESON = new AppFx.FuzzyDate(Convert.ToInt32(this.cmbCcExpYear.SelectedValue), Convert.ToInt32(this.cmbCcExpMonth.SelectedValue));
                data.CREDITTYPECODEID = Utility.GetCrmCC(this.cmbCcType.SelectedValue);
                data.RECEIPTAMOUNT = amount;                
                data.SOURCECODE = "BBIS";
                
                data.Save(this.API.AppFxWebServiceProvider);
                
                runningTotal += amount;
            }

            ViewState["selectedSponsorships"] = null;
            ViewState["GiftId"] = null;
        }

        private bool processPayment()
        {
            var opts = MyContent;

            BBPSPaymentInfo payment = new BBPSPaymentInfo();
            payment.SkipCreateGiftTransaction = true;
            payment.DemoMode = opts.DemoMode;
            payment.MerchantAcctID = opts.MerchantAccountID;
            payment.Bbpid = Utility.GetBbbid(opts.MerchantAccountID, this.API.Transactions.MerchantAccounts);
            payment.SkipCardValidation = MyContent.DemoMode;            
            payment.AppealID = 1;
            payment.Comments = "";
            
            List<string> giftsToProcess = new List<string>();
            if (ViewState["selectedSponsorships"] != null)
            {
                giftsToProcess = ViewState["selectedSponsorships"] as List<string>;
            }
            else
            {
                giftsToProcess.Add(ViewState["GiftId"].ToString());
            }

            decimal enteredAmount = Convert.ToDecimal(this.txtAmount.Text);
            decimal amount = enteredAmount / (giftsToProcess.Count.Equals(0) ? 1 : giftsToProcess.Count);
            decimal runningTotal = 0;
            amount = Math.Round(amount, 2);

            for (int i = 0; i < giftsToProcess.Count; i++)
            {
                if (i + 1 == giftsToProcess.Count)
                {
                    if (runningTotal + amount != enteredAmount) amount = enteredAmount - runningTotal;
                }

                int designationId = Utility.GetBbncDesignationId(giftsToProcess[i]);                
                payment.AddDesignationInfo(amount, "BBIS Child Sponsorship Transaction", designationId);
            }

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
                payment.DonorStreetAddress = this.txtBillingAddress.Text;
                payment.DonorCity = this.txtBillingCity.Text;
                payment.DonorStateProvince = this.cmbBillingCountry.SelectedValue == "US" ? this.cmbBillingState.SelectedValue : this.txtBillingRegion.Text;
                payment.DonorZIP = this.txtBillingZip.Text;                
            }

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

        protected void lnkBack_Click(object sender, EventArgs e)
        {
            this.mvMain.SetActiveView(this.viewSponsorships);
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
                        this.lblMessage.Text = MyContent.ThankYouMessage;
                        this.mvMain.SetActiveView(this.viewThankYou);
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Visible = true;
                this.lblError.Text = ex.Message + "<br /><br />" + ex.StackTrace;
            } 
        }

        protected void lnkPayBack_Click(object sender, EventArgs e)
        {
            this.mvMain.SetActiveView(this.viewSponsorships);
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

            foreach (GridViewRow row in this.gvSponsorships.Rows)
            {
                ((CheckBox)row.FindControl("chkPayment")).Checked = false;
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
                lblProvinceRegionRequired1.Visible = false;
            }
            else
            {
                this.txtBillingRegion.Enabled = true;
                this.cmbBillingState.Enabled = false;
                this.reqBillingState.Enabled = false;
                this.reqBillingRegion.Enabled = true;
                lblProvinceRegionRequired1.Visible = true;
            }
        }

        protected void lnkMakePayment_Click(object sender, EventArgs e)
        {
            this.mvMain.SetActiveView(this.viewPay);
            this.populateYears();
            List<string> selectedSponsorships = new List<string>();
            decimal totalPayments = 0;
            int count = 0;
            foreach (GridViewRow row in this.gvSponsorships.Rows)
            {
                bool isChecked = ((CheckBox)row.FindControl("chkPayment")).Checked;
                if (isChecked)
                {
                    string id = this.gvSponsorships.DataKeys[row.RowIndex]["RevenueId"].ToString();
                    selectedSponsorships.Add(id);
                    count++;
                }
            }

            totalPayments = count * Utility.GetSponsorshipAmount();
            this.txtAmount.Text = totalPayments.ToString("c").Replace("$","").Replace(",","");

            this.lblSummary.Visible = true;
            this.lblSummary.Text = "<br /><br /><b>Total payments for selected sponsorships is: " + totalPayments.ToString("c") + "<b><br />";
            ViewState["selectedSponsorships"] = selectedSponsorships;
        }

        protected void lnkThanksBack_Click(object sender, EventArgs e)
        {
            this.mvMain.SetActiveView(this.viewSponsorships);
        }
    }
}