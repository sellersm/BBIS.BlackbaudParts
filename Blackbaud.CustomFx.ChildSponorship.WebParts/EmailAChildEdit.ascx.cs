using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Blackbaud.Web.Content.Core;

namespace Blackbaud.CustomFx.ChildSponorship.WebParts
{
    public partial class EmailAChildEdit : BBNCExtensions.Parts.CustomPartEditorBase
    {
        protected Blackbaud.Web.Content.Portal.HtmlEditorControl2 ucHtmlControl;
        protected Blackbaud.Web.Content.Portal.PageLink plinkErrorPage;
        protected Blackbaud.Web.Content.Portal.PageLink plinkSuccessPage;
        protected Blackbaud.Web.Content.Portal.PageLink plinkMySponsorshipsPage;

        private EmailAChildOptions _myContent;
        private EmailAChildOptions MyContent
        {
            get
            {
                if (_myContent == null)
                {
                    _myContent = (EmailAChildOptions)this.Content.GetContent(typeof(EmailAChildOptions));

                    if (_myContent == null)
                    {
                        _myContent = new EmailAChildOptions();
                    }
                }

                return _myContent;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void OnLoadContent()
        {
            if(!IsPostBack)
            {
                txtFromAddress.Text = MyContent.FromAddress;
                txtFromName.Text = MyContent.FromName;
                txtToAddress.Text = MyContent.ToAddress;  
                txtSubject.Text = MyContent.Subject;
                plinkErrorPage.PageID = MyContent.ErrorPageID;
                plinkSuccessPage.PageID = MyContent.SuccessPageID;              
                plinkMySponsorshipsPage.PageID = MyContent.MySponsorshipsPageID;
                ucHtmlControl.StorageHTML = MyContent.LinkHtml;              
            }
        }

        public override bool OnSaveContent(bool bDialogIsClosing = true)
        {
            try
            {
                if(plinkErrorPage.PageID < 1)
                {
                    throw new Exception("Error page is required.");
                }
                
                if(plinkSuccessPage.PageID < 1)
                {
                    throw new Exception("Success page is required.");
                }

                if (plinkMySponsorshipsPage.PageID < 1)
                {
                    throw new Exception("My sponsorships page is required.");
                }

                MyContent.FromAddress = txtFromAddress.Text;
                MyContent.ToAddress = txtToAddress.Text;
                MyContent.FromName = txtFromName.Text;
                MyContent.LinkHtml = ucHtmlControl.StorageHTML;
                MyContent.Subject = txtSubject.Text;
                MyContent.ErrorPageID = plinkErrorPage.PageID;
                MyContent.SuccessPageID = plinkSuccessPage.PageID;
                MyContent.MySponsorshipsPageID = plinkMySponsorshipsPage.PageID;

                if (!String.IsNullOrWhiteSpace(MyContent.LinkHtml))
                {
                    bool emailTemplateAlreadyExists = false;

                    if (MyContent != null)
                    {
                        emailTemplateAlreadyExists = MyContent.TemplateID > 0;
                    }

                    EmailTemplate emailTemplate = null;

                    if (emailTemplateAlreadyExists)
                    {
                        emailTemplate = new EmailTemplate(MyContent.TemplateID);
                    }
                    else
                    {
                        emailTemplate = new EmailTemplate();
                    }

                    emailTemplate.FromDisplayName = MyContent.FromName;
                    emailTemplate.FromAddress = MyContent.FromAddress;
                    emailTemplate.Name = "Email a child email setup for part " + this.Content.ContentID.ToString() + " - " + Guid.NewGuid().ToString();
                    emailTemplate.ReplyAddress = MyContent.ReplyAddress;
                    emailTemplate.Description = "Created from code on an Email a child form part.";
                    emailTemplate.DataSourceID = 1;
                    emailTemplate.Save();

                    MyContent.TemplateID = emailTemplate.ID;

                    Content.SaveContent(MyContent);
                }
                else
                {
                    throw new Exception("A link to the privacy policy and email preferences page must not be blank.");
                }

                return true;
            }
            catch(Exception ex)
            {
                lblError.Visible = true;
                lblError.Text = ex.Message;

                return false;
            }
        }
    }
}