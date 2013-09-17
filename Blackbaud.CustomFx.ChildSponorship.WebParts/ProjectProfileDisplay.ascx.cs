using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class ProjectProfileDisplay : BBNCExtensions.Parts.CustomPartDisplayBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
            if (!String.IsNullOrWhiteSpace(Request.QueryString["id"])) this.loadProject();
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message + "<br /><br />" + ex.StackTrace;
                this.lblError.Visible = true;
            }

        }

        private void loadProject()
        {
            ProjectProfileOptions options = (ProjectProfileOptions)this.Content.GetContent(typeof(ProjectProfileOptions));
            if (options != null && Request.QueryString["id"] != null)
            {
                Guid id = new Guid(Request.QueryString["id"].ToString());
                this.lblInfo.Text = Utility.GetNoteTextFromConstituent(id, options.NoteDocType);
                this.imgPhoto.ImageUrl = "ImageHandler.ashx?context=constituent&id=" + id.ToString() + "&type=" + options.ImageDocType;// .Controls.Add(Utility.GetPhotoFromAttachements(provider, id, "Child Photo"));
            }
        }
    }
}