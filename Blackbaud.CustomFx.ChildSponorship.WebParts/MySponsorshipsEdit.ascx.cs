using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class MySponsorshipsEdit : BBNCExtensions.Parts.CustomPartEditorBase
    {
        protected Blackbaud.Web.Content.Portal.PageLink plinkMoreInfoPage;
        protected Blackbaud.Web.Content.Portal.PageLink plinkEmailPage;

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
            if(!IsPostBack)
            {
                ddlMerchantAccounts.Items.Clear();
                BBNCExtensions.API.NetCommunity.Current().Utility.MerchantAccount.LoadListWithMerchantAcccounts(ddlMerchantAccounts, false);

                ddlMerchantAccounts.SelectedValue = MyContent.MerchantAccountID.ToString();   
            }
        }

        public override void OnLoadContent()
        {
            if (!IsPostBack)
            {
                txtDocType.Text = MyContent.ThumbnailNoteType;
                plinkMoreInfoPage.PageID = MyContent.MoreInfoPageID;
                plinkEmailPage.PageID = MyContent.EmailPageID;
                chkDemo.Checked = MyContent.DemoMode;
                txtMessage.Text = MyContent.ThankYouMessage;             
            }
        }

        public override bool OnSaveContent(bool bDialogIsClosing = true)
        {
            MyContent.ThumbnailNoteType = this.txtDocType.Text;
            MyContent.MoreInfoPageID = this.plinkMoreInfoPage.PageID;
            MyContent.EmailPageID = this.plinkEmailPage.PageID;
            MyContent.DemoMode = this.chkDemo.Checked;
            MyContent.ThankYouMessage = this.txtMessage.Text;
            MyContent.MerchantAccountID = Convert.ToInt16(ddlMerchantAccounts.SelectedValue);

            this.Content.SaveContent(MyContent);
            return true;
        }
    }
}