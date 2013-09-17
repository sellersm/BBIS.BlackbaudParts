using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class CountryProfileDisplay : BBNCExtensions.Parts.CustomPartDisplayBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(Request.QueryString["id"])) this.loadCountry();
                }
                catch (Exception ex)
                {
                    this.lblError.Text = ex.Message + "<br /><br />" + ex.StackTrace;
                    this.lblError.Visible = true;
                }
            }
        }

        private void loadCountry()
        {
            CountryProfileOptions options = (CountryProfileOptions)this.Content.GetContent(typeof(CountryProfileOptions));
            if (options != null)
            {
                Guid id = new Guid(Request.QueryString["id"].ToString());

                this.lblInfo.Text = Utility.GetNoteTextFromConstituent(id, options.NoteType);
                this.imgPhoto.ImageUrl = "ImageHandler.ashx?context=constituent&id=" + id.ToString() + "&type=" + options.ImageDocType;// .Controls.Add(Utility.GetPhotoFromAttachements(provider, id, "Child Photo"));
            }
        }
    }
}