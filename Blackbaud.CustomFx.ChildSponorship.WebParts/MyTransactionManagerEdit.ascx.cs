using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Blackbaud.CustomFx.ChildSponorship.WebParts
{
    public partial class MyTransactionManagerEdit : BBNCExtensions.Parts.CustomPartEditorBase
    {
        private MyTransactionManagerOptions _myContent;
        private MyTransactionManagerOptions MyContent
        {
            get
            {
                if (_myContent == null)
                {
                    _myContent = (MyTransactionManagerOptions)this.Content.GetContent(typeof(MyTransactionManagerOptions));

                    if (_myContent == null)
                    {
                        _myContent = new MyTransactionManagerOptions();
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
                chkShowActive.Checked = MyContent.ShowActive;
                chkShowHistory.Checked = MyContent.ShowHistory;
            }
        }

        public override bool OnSaveContent(bool bDialogIsClosing = true)
        {
            MyContent.ShowActive = chkShowActive.Checked;
            MyContent.ShowHistory = chkShowHistory.Checked;

            this.Content.SaveContent(MyContent);

            return true;
        }
    }
}