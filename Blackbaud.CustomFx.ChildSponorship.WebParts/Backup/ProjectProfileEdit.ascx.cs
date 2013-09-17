using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class ProjectProfileEdit : BBNCExtensions.Parts.CustomPartEditorBase
    {
        private ProjectProfileOptions _myContent;
        private ProjectProfileOptions MyContent
        {
            get
            {
                if (_myContent == null)
                {
                    _myContent = (ProjectProfileOptions)this.Content.GetContent(typeof(ProjectProfileOptions));

                    if (_myContent == null)
                    {
                        _myContent = new ProjectProfileOptions();
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
                this.txtType.Text = MyContent.NoteDocType;
                this.txtPhoto.Text = MyContent.ImageDocType;
            }
        }

        public override bool OnSaveContent(bool bDialogIsClosing = true)
        {
            MyContent.NoteDocType = this.txtType.Text;
            MyContent.ImageDocType = this.txtPhoto.Text;

            this.Content.SaveContent(MyContent);

            return true;
        }
    }
}