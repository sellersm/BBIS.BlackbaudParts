using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

using Blackbaud.AppFx.WebAPI;
using Blackbaud.AppFx.WebAPI.ServiceProxy;
using Blackbaud.AppFx.Sponsorship.Catalog.WebApiClient.DataLists.Sponsorship;

namespace Blackbaud.CustomFx.ChildSponsorship.WebParts
{
    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageHandler : BBNCExtensions.CustomHandlers.CustomHandlerBase
    {
        private string sponsorshipQuery = @"
                SELECT 
	                [FILE]
                FROM 
	                dbo.SPONSORSHIPOPPORTUNITYATTACHMENT a
	                INNER JOIN SPROPPATTACHMENTTYPECODE t ON t.ID = a.SPROPPATTACHMENTTYPECODEID
                WHERE 
	                SPONSORSHIPOPPORTUNITYID = @ID
	                AND t.DESCRIPTION = @TYPE";

        private string constituentQuery = @"
                SELECT 
	                [FILE]
                FROM 
	                dbo.CONSTITUENTATTACHMENT a
	                INNER JOIN CONSTITUENTATTACHMENTTYPECODE t ON t.ID = a.CONSTITUENTATTACHMENTTYPECODEID
                WHERE 
	                CONSTITUENTID = @ID
	                AND t.DESCRIPTION = @TYPE";

        public override void ProcessRequest(HttpContext context)
        {
            string recordId = context.Request.QueryString["id"].ToString();
            string type = context.Request.QueryString["type"].ToString();
            string contextType = context.Request.QueryString["context"].ToString();

            SqlConnection con = new SqlConnection(Blackbaud.Web.Content.Core.Settings.ConnectionString);

            string sql = "";

            if (contextType == "sponsorship")
            {
                sql = this.sponsorshipQuery;
            }
            else if (contextType == "constituent")
            {
                sql = this.constituentQuery;
            }

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("ID", recordId);
            cmd.Parameters.AddWithValue("TYPE", type);

            con.Open();
            byte[] pict = (byte[])cmd.ExecuteScalar();
            con.Close();

            context.Response.ContentType = "image/bmp";
            context.Response.OutputStream.Write(pict, 0, pict.Length);

            //DataListLoadRequest request = SponsorshipOpportunityDocumentationDataList.CreateRequest(this.API.AppFxWebServiceProvider);
            //request.DataListID = new Guid("1eb02c18-8111-4129-b718-e35ef62e1cfa");
            //request.ContextRecordID = constituentId.ToString();
            //SponsorshipOpportunityDocumentationDataListRow[] rows = SponsorshipOpportunityDocumentationDataList.GetRows(provider, request);

            //foreach (SponsorshipOpportunityDocumentationDataListRow row in rows)
            //{
            //    if (row.TYPE == type && row.DOCUMENTATIONTYPE == "Attachment")
            //    {
            //        DataFormLoadRequest noteRequest = SponsorshipOpportunityAttachmentViewForm.CreateRequest(provider);
            //        noteRequest.RecordID = row.ID.ToString();
            //        SponsorshipOpportunityAttachmentViewFormData photo = SponsorshipOpportunityAttachmentViewForm.LoadData(provider, noteRequest);
            //        System.IO.MemoryStream stream = new System.IO.MemoryStream(photo.FILE);
            //        break;
            //    }
            //}
        }

        public override bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}