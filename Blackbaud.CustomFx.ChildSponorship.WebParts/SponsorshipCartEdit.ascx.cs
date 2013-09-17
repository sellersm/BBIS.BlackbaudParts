using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class SponsorshipCartEdit : BBNCExtensions.Parts.CustomPartEditorBase
    {
        protected Blackbaud.Web.Content.Portal.PageLink plinkPaymentPage;

        private SponsorshipCartOptions _myContent;
        private SponsorshipCartOptions MyContent
        {
            get
            {
                if (_myContent == null)
                {
                    _myContent = (SponsorshipCartOptions)this.Content.GetContent(typeof(SponsorshipCartOptions));

                    if (_myContent == null)
                    {
                        _myContent = new SponsorshipCartOptions();
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
            if (!IsPostBack)
            {
                plinkPaymentPage.PageID = MyContent.PaymentFormPageID;
                MyContent.PaymentFormPageID = plinkPaymentPage.PageID;
            }
        }

        public override bool OnSaveContent(bool bDialogIsClosing = true)
        {
            MyContent.PaymentFormPageID = plinkPaymentPage.PageID;
            this.Content.SaveContent(MyContent);
            return true;
        }
    }
}