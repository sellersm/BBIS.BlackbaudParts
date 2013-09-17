using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class ChildProfileEdit : BBNCExtensions.Parts.CustomPartEditorBase
    {
        protected Blackbaud.Web.Content.Portal.PageLink plinkCountryPage;
        protected Blackbaud.Web.Content.Portal.PageLink plinkProjectPage;
        protected Blackbaud.Web.Content.Portal.PageLink plinkSponsorPage;

        private ChildProfileOptions _myContent;
        private ChildProfileOptions MyContent
        {
            get
            {
                if (_myContent == null)
                {
                    _myContent = (ChildProfileOptions)this.Content.GetContent(typeof(ChildProfileOptions));

                    if (_myContent == null)
                    {
                        _myContent = new ChildProfileOptions();
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
                txtDocType.Text = MyContent.FullPhotoType;
                plinkCountryPage.PageID = MyContent.CountryPageID;
                plinkProjectPage.PageID = MyContent.ProjectPageID;
                plinkSponsorPage.PageID = MyContent.SponsorPageID;
                txtBio.Text = MyContent.ChildBioDocType;
            }
        }

        public override bool OnSaveContent(bool bDialogIsClosing)
        {
            MyContent.FullPhotoType = txtDocType.Text;
            MyContent.CountryPageID = plinkCountryPage.PageID;
            MyContent.ProjectPageID = plinkProjectPage.PageID;
            MyContent.SponsorPageID = plinkSponsorPage.PageID;
            MyContent.ChildBioDocType = txtBio.Text;

            this.Content.SaveContent(MyContent);

            return true;
        }
    }
}