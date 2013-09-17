using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Blackbaud.Web.Content.Core;
using Blackbaud.Web.Content.Common;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class SponsorshipPaymentFormEdit : BBNCExtensions.Parts.CustomPartEditorBase
    {
        protected Blackbaud.Web.Content.Portal.HtmlEditorControl2 htmledConfirmationEmail;
        protected Blackbaud.Web.Content.Portal.PageLink plinkThankYouPage;

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
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                //clear the ddl and use the oob api call
                ddlMerchantAccounts.Items.Clear();
                BBNCExtensions.API.NetCommunity.Current().Utility.MerchantAccount.LoadListWithMerchantAcccounts(ddlMerchantAccounts, false, 0);
                LoadOptions();
            }
        }

        public void LoadOptions()
        {
            if (!IsPostBack)
            {
                if (MyContent != null)
                {
                    plinkThankYouPage.PageID = MyContent.ThankYouPageID;
                    this.chkDemo.Checked = MyContent.DemoMode;
                    ddlMerchantAccounts.SelectedValue = MyContent.MerchantAccountID.ToString();                    
                    txtPrefix.Text = MyContent.BatchNumberPrefix;
                    SetEmailOptions(MyContent.EmailOptions);
                }
            }            
        }

        protected void htmled2ConfirmationEmail_Init(object sender, System.EventArgs e) 
        {
            htmledConfirmationEmail.FieldListProvider = new Blackbaud.Web.Content.Core.Data.IFieldListProvider[] { new MergeFieldsProvider() };
        }        

        public override bool OnSaveContent(bool bDialogIsClosing = true)
        {
            MyContent.ThankYouPageID = plinkThankYouPage.PageID;
            MyContent.DemoMode = this.chkDemo.Checked;
            MyContent.MerchantAccountID = Convert.ToInt16(ddlMerchantAccounts.SelectedValue);
            MyContent.BatchNumberPrefix = txtPrefix.Text;
            MyContent.EmailOptions = GetEmailOptions();            

            this.Content.SaveContent(MyContent);
            return true;
        }

        private void SetEmailOptions(ConfirmationEmailOptions emailOptions)
        {   
            if(emailOptions != null)
            {
                txtFromAddress.Text = emailOptions.FromAddress;
                txtFromName.Text = emailOptions.FromName;
                txtReplyAddress.Text = emailOptions.ReplyAddress;
                txtSubject.Text = emailOptions.Subject;
                htmledConfirmationEmail.StorageHTML = emailOptions.HTML;
            }
        }
    
        private ConfirmationEmailOptions GetEmailOptions()
        {
            ConfirmationEmailOptions result = new ConfirmationEmailOptions();

            result.HTML = htmledConfirmationEmail.StorageHTML;
            result.FromName = txtFromName.Text;
            result.FromAddress = txtFromAddress.Text;
            result.ReplyAddress = txtReplyAddress.Text;
            result.Subject = txtSubject.Text;

            bool emailTemplateAlreadyExists = false;

            if(MyContent.EmailOptions != null)
            {
                emailTemplateAlreadyExists = MyContent.EmailOptions.TemplateID > 0;
            }

            EmailTemplate emailTemplate = null;

            if(emailTemplateAlreadyExists)
            {
                emailTemplate = new EmailTemplate(MyContent.EmailOptions.TemplateID);
            }
            else
            {
                emailTemplate = new EmailTemplate();
            }

            emailTemplate.FromDisplayName = result.FromName;
            emailTemplate.FromAddress = result.FromAddress;
            emailTemplate.ContentHTML = result.HTML;
            emailTemplate.Subject = result.Subject;
            emailTemplate.Name = "Confirmation message for part " + this.Content.ContentID.ToString() + " - " + Guid.NewGuid().ToString();
            emailTemplate.ReplyAddress = result.ReplyAddress;
            emailTemplate.Description = "Created from code on a Sponsorship Payment Form part.";
            emailTemplate.DataSourceID = 1;
            emailTemplate.Save();

            result.TemplateID = emailTemplate.ID;

            return result;
        }
    }
}