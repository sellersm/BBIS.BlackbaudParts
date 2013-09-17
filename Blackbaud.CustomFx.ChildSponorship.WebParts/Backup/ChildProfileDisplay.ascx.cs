using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

using Blackbaud.AppFx.WebAPI;
using Blackbaud.AppFx.WebAPI.ServiceProxy;
using Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.ViewForms.SponsorshipOpportunity;
using Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.ViewForms.SponsorshipProgram;
namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    public partial class ChildProfileDisplay : BBNCExtensions.Parts.CustomPartDisplayBase
    {
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
            if (!IsPostBack)
            {
                //get all of the querystrings being passed in
                Guid id = new Guid();

                if (Request.QueryString["ID"] != null)
                {
                    if (Guid.TryParse(Request.QueryString["ID"], out id))
                    {
                        bool isSponsored = false;

                        if (Request.QueryString["SPONSORED"] != null)
                        {
                            bool.TryParse(Request.QueryString["SPONSORED"], out isSponsored);
                        }

                        this.SetSponsorshipState(isSponsored);
                    }
                }

                if (id != Guid.Empty || id != null)
                {
                    try
                    {
                        //behave differently if they are already sponsored
                        this.LoadChildInfo(id);
                    }
                    catch (Exception ex)
                    {
                        this.lblError.Text = ex.Message + "<br /><br />" + ex.StackTrace;
                        this.lblError.Visible = true;
                    }
                }
            }
        }

        private void SetSponsorshipState(Boolean isSponsored)
        {
            this.trAmount.Visible = !isSponsored;
            this.trSponsor.Visible = !isSponsored;
        }

        private void LoadChildInfo(Guid id)
        {
            SqlConnection con = new SqlConnection(Blackbaud.Web.Content.Core.Settings.ConnectionString);
            string sql = "SELECT * FROM USR_V_QUERY_SPONSORSHIPOPPORTUNITY WHERE ID = @id";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.CommandType = CommandType.Text;

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                this.lblName.Text = reader["FULLNAME"].ToString();
                this.lblCountry.Text = reader["COUNTRYNAME"].ToString();
                this.lblchildAge.Text = reader["AGE"].ToString();
                this.lblchildGender.Text = reader["GENDER"].ToString();
                this.lblchildBirthdate.Text = reader["BIRTHDATE"].ToString();
                this.lblchildProject.Text = reader["PROJECTNAME"].ToString();
                this.lblchildNumber.Text = reader["CHILDNO"].ToString();
                this.lblchildSponsorAmount.Text = Utility.GetSponsorshipAmount().ToString("c");

                string countryUrl = Utility.GetBBISPageUrl(MyContent.CountryPageID) + "{0}id=" + reader["COUNTRYID"];
                string projectUrl = Utility.GetBBISPageUrl(MyContent.ProjectPageID) + "{0}id=" + reader["PROJECTID"];
                string sponsorUrl = Utility.GetBBISPageUrl(MyContent.SponsorPageID) + "{0}id=" + reader["ID"];

                countryUrl = countryUrl.IndexOf("?") > -1 ? String.Format(countryUrl, "&") : String.Format(countryUrl, "?");
                projectUrl = projectUrl.IndexOf("?") > -1 ? String.Format(projectUrl, "&") : String.Format(projectUrl, "?");
                sponsorUrl = sponsorUrl.IndexOf("?") > -1 ? String.Format(sponsorUrl, "&") : String.Format(sponsorUrl, "?");

                this.lnkCountry.PostBackUrl = countryUrl;
                this.lnkProject.PostBackUrl = projectUrl;
                this.lnkSponsor.PostBackUrl = sponsorUrl;

                this.lblchildBio.Text = Utility.GetNoteTextFromSponsorship(this.API.AppFxWebServiceProvider, new Guid(reader["ID"].ToString()), MyContent.ChildBioDocType);
                this.imgPhoto.ImageUrl = "ImageHandler.ashx?context=sponsorship&type=" + MyContent.FullPhotoType + "&id=" + reader["ID"];
            }
            con.Close();
        }

    }
}