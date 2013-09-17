using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

using Blackbaud.AppFx.WebAPI;
using Blackbaud.AppFx.WebAPI.ServiceProxy;
using Blackbaud.AppFx.Constituent.Catalog.WebApiClient.DataLists.Constituent;
using Blackbaud.AppFx.Constituent.Catalog.WebApiClient.ViewForms.ConstituentNote;
using Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.DataLists.SponsorshipOpportunity;
using Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.ViewForms.SponsorshipOpportunityNote;
using System.Text;

public static class Utility
{
    public static int GetBbncDesignationId(string guid)
    {
        int results = 0;
        SqlConnection con = new SqlConnection(Blackbaud.Web.Content.Core.Settings.ConnectionString);
        string sql = @"
                SELECT 
	                m.ID 
                FROM 
	                dbo.BBNCDESIGNATIONIDMAP m WITH(NOLOCK)
	                INNER JOIN dbo.REVENUESPLIT r WITH(NOLOCK) ON m.DESIGNATIONID = r.DESIGNATIONID
                WHERE
	                r.REVENUEID = @REVENUEID";

        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@REVENUEID", guid);

        con.Open();
        object returnValue = cmd.ExecuteScalar();
        if (returnValue != DBNull.Value) results = Convert.ToInt32(returnValue);
        con.Close();
        return results;
    }

    public static int GetBbncDesignationIdFromSponsorshipOpportunity(string guid)
    {
        int results = 0;

        SqlConnection con = new SqlConnection(Blackbaud.Web.Content.Core.Settings.ConnectionString);
        string sql = @"
                SELECT
                    m.ID
                FROM 
                    dbo.SPONSORSHIPOPPORTUNITY so WITH(NOLOCK)
                    INNER JOIN dbo.SPONSORSHIPLOCATION sloc WITH(NOLOCK) ON sloc.ID = so.SPONSORSHIPLOCATIONID
                    INNER JOIN dbo.BBNCDESIGNATIONIDMAP m WITH(NOLOCK) ON sloc.DESIGNATIONID = m.DESIGNATIONID
                WHERE
	                so.ID = @ID";

        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@ID", guid);

        con.Open();
        object returnValue = cmd.ExecuteScalar();
        if (returnValue != DBNull.Value) results = Convert.ToInt32(returnValue);
        con.Close();
        return results;
    }

    public static string GetNoteTextFromSponsorship(AppFxWebServiceProvider provider, Guid constituentId, string type)
    {
        string results = "";

        SqlConnection con = new SqlConnection(Blackbaud.Web.Content.Core.Settings.ConnectionString);
        string sql = @"
                SELECT 
                    HTMLNOTE 
                FROM 
                    dbo.SPONSORSHIPOPPORTUNITY so
                    INNER JOIN dbo.SPONSORSHIPOPPORTUNITYCHILD so_child on so_child.ID = so.ID
                    INNER JOIN SPONSORSHIPOPPORTUNITYNOTE n ON n.SPONSORSHIPOPPORTUNITYID = so.ID 
                    INNER JOIN SPROPPNOTETYPECODE t ON n.SPROPPNOTETYPECODEID = t.ID
                WHERE 
                    so_child.CONSTITUENTID = @ID
                    AND t.DESCRIPTION = @TYPE";

        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("ID", constituentId.ToString());
        cmd.Parameters.AddWithValue("TYPE", type);

        con.Open();
        results = (string)cmd.ExecuteScalar();
        con.Close();


        //DataListLoadRequest request = SponsorshipOpportunityDocumentationDataList.CreateRequest(provider);
        //request.DataListID = new Guid("1eb02c18-8111-4129-b718-e35ef62e1cfa");
        //request.ContextRecordID = constituentId.ToString();
        //SponsorshipOpportunityDocumentationDataListRow[] rows = SponsorshipOpportunityDocumentationDataList.GetRows(provider, request);
        //foreach (SponsorshipOpportunityDocumentationDataListRow row in rows)
        //{
        //    if (row.TYPE == type && row.DOCUMENTATIONTYPE == "Note")
        //    {
        //        DataFormLoadRequest noteRequest = SponsorshipOpportunityNoteViewForm.CreateRequest(provider);
        //        noteRequest.RecordID = row.ID.ToString();
        //        SponsorshipOpportunityNoteViewFormData note = SponsorshipOpportunityNoteViewForm.LoadData(provider, noteRequest);
        //        results = note.TEXTNOTE;
        //        break;
        //    }
        //}
        return results;
    }

    public static string GetNoteTextFromConstituent(Guid constituentId, string type)
    {
        string results = "";

        SqlConnection con = new SqlConnection(Blackbaud.Web.Content.Core.Settings.ConnectionString);
        string sql = @"
                SELECT 
	                HTMLNOTE 
                FROM 
	                dbo.CONSTITUENTNOTE c
	                INNER JOIN CONSTITUENTNOTETYPECODE t ON c.CONSTITUENTNOTETYPECODEID = t.ID 
                WHERE 
	                CONSTITUENTID = @ID
	                AND t.DESCRIPTION = @TYPE";

        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("ID", constituentId.ToString());
        cmd.Parameters.AddWithValue("TYPE", type);

        con.Open();
        results = (string)cmd.ExecuteScalar();
        con.Close();


        //DataListLoadRequest request = ConstituentDocumentationList.CreateRequest(provider);
        ////request.DataListID = new Guid("d100a5c3-fbc1-4d0f-b7da-eda314f28fbe");
        //request.ContextRecordID = constituentId.ToString();
        //request.DataListName = "Constituent Documentation List";
        //ConstituentDocumentationListRow[] rows = ConstituentDocumentationList.GetRows(provider, request);
        //foreach (ConstituentDocumentationListRow row in rows)
        //{
        //    if (row.TYPE == type && row.DOCUMENTATIONTYPE == "Note")
        //    {
        //        //DataFormLoadRequest noteRequest = ConstituentNoteViewForm.CreateRequest(provider);
        //        //noteRequest.RecordID = row.ID.ToString();
        //        //ConstituentNoteViewFormData note = ConstituentNoteViewForm.LoadData(provider, noteRequest);
        //        //results = note.TEXTNOTE;
        //        //break;
        //    }
        //}

        //DataFormLoadRequest noteRequest = ConstituentNoteViewForm.CreateRequest(provider);
        //noteRequest.RecordID = "5440382E-C11C-4D10-BC14-997AC545F368";
        //ConstituentNoteViewFormData note = ConstituentNoteViewForm.LoadData(provider, noteRequest);
        //results = note.TEXTNOTE;

        return results;
    }

    public static decimal GetSponsorshipAmount()
    {
        decimal results = 0;

        //DataFormLoadRequest request = SponsorshipProgramInformationViewForm.CreateRequest(this.API.AppFxWebServiceProvider);
        //request.FormID = new Guid("302dd985-5b5b-4527-9f31-435bfd71c623");
        //request.RecordID = "32FA809A-5EF1-4A17-862C-7DFE0AB49F19";
        //SponsorshipProgramInformationViewFormData data = SponsorshipProgramInformationViewForm.LoadData(this.API.AppFxWebServiceProvider, request);
        //if (data.COST.HasValue) results = data.COST.Value;

        SqlConnection con = new SqlConnection(Blackbaud.Web.Content.Core.Settings.ConnectionString);
        string sql = @"SELECT AMOUNT FROM SPONSORSHIPPROGRAM WHERE ID = @ID";

        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("ID", "32FA809A-5EF1-4A17-862C-7DFE0AB49F19");

        con.Open();
        results = (decimal)cmd.ExecuteScalar();
        con.Close();

        return results;
    }

    //public static System.Drawing.Image GetPhotoFromAttachements(AppFxWebServiceProvider provider, Guid constituentId, string type)
    //{
    //    System.Drawing.Image results = null;
    //    DataListLoadRequest request = SponsorshipOpportunityDocumentationDataList.CreateRequest(provider);
    //    request.DataListID = new Guid("1eb02c18-8111-4129-b718-e35ef62e1cfa");
    //    request.ContextRecordID = constituentId.ToString();
    //    SponsorshipOpportunityDocumentationDataListRow[] rows = SponsorshipOpportunityDocumentationDataList.GetRows(provider, request);

    //    foreach (SponsorshipOpportunityDocumentationDataListRow row in rows)
    //    {
    //        if (row.TYPE == type && row.DOCUMENTATIONTYPE == "Attachment")
    //        {
    //            DataFormLoadRequest noteRequest = SponsorshipOpportunityAttachmentViewForm.CreateRequest(provider);
    //            noteRequest.RecordID = row.ID.ToString();
    //            SponsorshipOpportunityAttachmentViewFormData photo = SponsorshipOpportunityAttachmentViewForm.LoadData(provider, noteRequest);
    //            System.IO.MemoryStream stream = new System.IO.MemoryStream(photo.FILE);
    //            results = System.Drawing.Image.FromStream(stream);
    //            break;
    //        }
    //    }
    //    return results;
    //}

    //public static void BindDrowpDownList(string sql, DropDownList cmb)
    //{
    //    SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);
    //    SqlCommand cmd = new SqlCommand(sql, connection);
    //    connection.Open();
    //    cmb.DataSource = cmd.ExecuteReader();
    //    cmb.DataBind();
    //    connection.Close();
    //}

    //public static void GetSummary(AppFxWebServiceProvider provider, Guid constituentId)
    //{
    //    DataListLoadRequest request = SponsorshipOpportunityDocumentationDataList.CreateRequest(provider);
    //    request.DataListID = new Guid("1eb02c18-8111-4129-b718-e35ef62e1cfa");
    //    request.ContextRecordID = constituentId.ToString();
    //    SponsorshipOpportunityDocumentationDataListRow[] rows = SponsorshipOpportunityDocumentationDataList.GetRows(provider, request);            

    //    SponsorshipOpportunityDocumentationDataList list;

    //    //SponsorshipDetailViewFormData results = new SponsorshipDetailViewFormData();
    //    //DataFormLoadRequest request = SponsorshipDetailViewForm.CreateRequest(provider);
    //    //request.RecordID = constituentId.ToString();
    //    //results = SponsorshipDetailViewForm.LoadData(provider, request);

    //    ////SponsorshipDetailViewFormData

    //    //request = ConstituentNoteViewForm.CreateRequest(provider);
    //    //request.ContextRecordID = constituentId.ToString();            
    //    //ConstituentNoteViewFormData note = Blackbaud.AppFx.Constituent.Catalog.WebApiClient.ViewForms.ConstituentNote.ConstituentNoteViewForm.LoadData(provider, request);            

    //    //return results;
    //}

    public static Guid GetCrmCC(string name)
    {
        Guid results = Guid.Empty;
        switch (name.ToUpper())
        {
            case "VISA":
                results = new Guid("C6FA2233-5A84-474C-9685-9C97C3B86DD8");
                break;
            case "MASTERCARD":
                results = new Guid("807B065D-B093-4AA1-8DBC-720B26FF4ADD");
                break;
            case "AMERICANEXPRESS":
                results = new Guid("E418F944-D7B5-458F-A1A0-0AA932D756B4");
                break;
            case "DISCOVER":
                results = new Guid("84D8AF0D-501B-40B9-B500-94A490B9C41E");
                break;
        }

        return results;
    }

    private delegate double RoundingFunction(double value);

    private enum RoundingDirection { Up, Down }

    public static double RoundUp(double value, int precision)
    {
        return Round(value, precision, RoundingDirection.Up);
    }

    public static double RoundDown(double value, int precision)
    {
        return Round(value, precision, RoundingDirection.Down);
    }

    private static double Round(double value, int precision, RoundingDirection roundingDirection)
    {
        RoundingFunction roundingFunction;

        if (roundingDirection == RoundingDirection.Up)
        {
            roundingFunction = Math.Ceiling;
        }
        else
        {
            roundingFunction = Math.Floor;
        }

        value *= Math.Pow(10, precision);
        value = roundingFunction(value);
        return value * Math.Pow(10, -1 * precision);
    }

    public static Guid GetBbbid(int id, BBNCExtensions.API.Transactions.MerchantAccount[] accounts)
    {
        foreach (BBNCExtensions.API.Transactions.MerchantAccount account in accounts)
        {
            if (account.Id == id) return account.BBPSID;
        }

        return Guid.Empty;
    }

    public static string GetBBISPageUrl(int pageID)
    {
        return Blackbaud.Web.Content.Core.URLBuilder.GetSitePageLinkBuilder(pageID).ToString();
    }

    public static void RedirectToBBISPage(int pageID)
    {
        RedirectToBBISPage(pageID, null);
    }

    /// <summary>
    /// This method is untested.
    /// </summary>
    /// <param name="pageID"></param>
    /// <param name="queryStrings"></param>
    public static void RedirectToBBISPage(int pageID, Dictionary<string, string> queryStrings)
    {
        try
        {
            string url = GetBBISPageUrl(pageID);

            StringBuilder strings = null;

            if (queryStrings != null)
            {
                strings = new StringBuilder();

                foreach (var entry in queryStrings)
                {
                    strings.Append(string.Format("{0}={1}&", entry.Key, entry.Value));
                }

                if (url.Contains("?"))
                {
                    string.Concat("&", url);
                }
                else
                {
                    string.Concat("?", url);
                }

                string.Concat(url, strings.ToString());
            }

            HttpContext.Current.Response.Redirect((string.IsNullOrWhiteSpace(url) ? "#" : url));
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }    
}
