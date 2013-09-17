using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class MyFinancialCommitmentsEdit : BBNCExtensions.Parts.CustomPartEditorBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void OnLoadContent()
        {
            if (!IsPostBack)
            {
                MyFinancialCommitmentsOptions options = (MyFinancialCommitmentsOptions)this.Content.GetContent(typeof(MyFinancialCommitmentsOptions));
                if (options != null)
                {
                    this.chkDemo.Checked = options.DemoMode;
                    this.txtMessage.Text = options.ThankYouMessage;
                }
            }
        }

        public override bool OnSaveContent(bool bDialogIsClosing = true)
        {
            MyFinancialCommitmentsOptions options = new MyFinancialCommitmentsOptions();
            options.DemoMode = this.chkDemo.Checked;
            options.ThankYouMessage = this.txtMessage.Text;
            this.Content.SaveContent(options);
            return true;
        }
    }
}