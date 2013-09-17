using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class CountryProfileEdit : BBNCExtensions.Parts.CustomPartEditorBase
    {
        private CountryProfileOptions _myContent;
        private CountryProfileOptions MyContent
        {
            get
            {
                if (_myContent == null)
                {
                    _myContent = (CountryProfileOptions)this.Content.GetContent(typeof(CountryProfileOptions));

                    if (_myContent == null)
                    {
                        _myContent = new CountryProfileOptions();
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
                this.txtNoteType.Text = MyContent.NoteType;
                this.txtImage.Text = MyContent.ImageDocType;                
            }
        }

        public override bool OnSaveContent(bool bDialogIsClosing = true)
        {
            MyContent.NoteType = this.txtNoteType.Text;
            MyContent.ImageDocType = this.txtImage.Text;
            this.Content.SaveContent(MyContent);

            return true;
        }
    }
}