using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Blackbaud.CustomFx.ChildSponorship.WebParts.Classes;
using Blackbaud.Web.Content.Core;

namespace Blackbaud.CustomFx.ChildSponorship.WebParts
{
    public partial class EmailAChildDisplay : BBNCExtensions.Parts.CustomPartDisplayBase
    {
        private string _childid;
        private string _childname;
        private string _sponsorid;
        private string _sponsorname;

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
            if(!IsPostBack)
            {
                if(Request.QueryString["args"] != null)
                {
                    var args = new EncryptedQueryString(Request.QueryString["args"]);
                    _childid = args["CHILDID"];
                    _childname = args["CHILDNAME"];

                    lblChildID.Text = string.Format("Child ID: {0}", _childid);
                    lblChildName.Text = string.Format("Child Name: {0}", _childname);                
                }
                else
                { 
                    btnSend.Enabled = false;
                }
            }            
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Utility.GetBBISPageUrl(MyContent.MySponsorshipsPageID), true);
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            
            try
            {
                var args = new EncryptedQueryString(Request.QueryString["args"]);
                _childid = args["CHILDID"];
                _childname = args["CHILDNAME"];
                _sponsorid = args["SPONSORID"];
                _sponsorname = args["SPONSORNAME"];

                string customMessage = txtMessage.Text.Replace("\r\n", "<br />");

                StringBuilder message = new StringBuilder();
                message.AppendLine("<table>");                
                message.AppendLine(string.Format("<tr><td>Sponsor ID:</td><td>{0}</td></tr>", _sponsorid));
                message.AppendLine(string.Format("<tr><td>Sponsor Name:</td><td>{0}</td></tr>", _sponsorname));
                message.AppendLine("<tr><td colspan='2'><br /></td></tr>");
                message.AppendLine(string.Format("<tr><td>Child ID:</td><td>{0}</td></tr>", _childid));
                message.AppendLine(string.Format("<tr><td>Child Name:</td><td>{0}</td></tr>", _childname));
                message.AppendLine("<tr><td colspan='2'><br /></td></tr>");
                message.AppendLine("<tr><td colspan='2'><br /></td></tr>");
                message.AppendLine(string.Format("<tr><td colspan='2'>{0}</td></tr>", customMessage));
                message.AppendLine("<tr><td colspan='2'><br /></td></tr>");
                message.AppendLine("<tr><td colspan='2'><br /></td></tr>");
                message.AppendLine(string.Format("<tr><td colspan='2'>{0}</td></tr>", MyContent.LinkHtml));
                message.AppendLine("<tr><td colspan='2'><br /></td></tr>");
                message.AppendLine("<tr><td colspan='2'><br /></td></tr>");
                message.AppendLine("</table>");

                var template = new EmailTemplate(MyContent.TemplateID);
                     
                var em = new EMail(template);
                em.Save();                    
                em.FromAddress = MyContent.FromAddress;
                em.FromDisplayName = MyContent.FromName;
                em.Subject = MyContent.Subject;
                em.ContentHTML = message.ToString();
                    
                em.Send(MyContent.ToAddress, MyContent.ToAddress, API.Users.CurrentUser.RaisersEdgeID, API.Users.CurrentUser.UserID, null, this.Page);
            }
            catch (ArgumentNullException ane)
            {
                errorMessage = ane.Message;
            }
            catch(Exception ex)
            {
                errorMessage = "An error has occurred, please try again.";                
                Blackbaud.Web.Content.Core.Common.LogErrorToDB(ex, false);
            }
                        
            if(!string.IsNullOrWhiteSpace(errorMessage))
            {
                Utility.RedirectToBBISPage(MyContent.ErrorPageID);                
            }
            else
            {
                Utility.RedirectToBBISPage(MyContent.SuccessPageID);
            }
        }

        public override void RegisterLanguageFields()
        {
            RegisterLanguageField("D2CF2C11-602D-4C02-B678-D2C56F68E4AD", lblMessageHeader, "Message header", "Please enter the letter contents here:", "General", false);

            base.RegisterLanguageFields();
        } 
    }
}