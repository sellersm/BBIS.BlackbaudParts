using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Blackbaud.Web.Content.Portal;
using System.Web.UI.HtmlControls;
using Blackbaud.Web.Content.Portal.GivingHistory;
using Blackbaud.Web.Content.Core;
using System.Data.SqlClient;
using System.Data;

namespace Blackbaud.CustomFx.ChildSponorship.WebParts
{
    public partial class MyTransactionManagerDisplay : BBNCExtensions.Parts.CustomPartDisplayBase
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
            try
            {
                MultiView mTabControl = (MultiView)FindRecursiveControl(this.Page, "mTabControl");

                if (mTabControl != null)
                {
                    //GivingHistory2Control givingHistoryAll = (GivingHistory2Control)FindRecursiveControl(mTabControl, "givingHistoryAll");

                    if (!MyContent.ShowBoth && !MyContent.ShowNone)
                    {
                        if (!MyContent.ShowActive)
                        {
                            mTabControl.ActiveViewIndex = 1;

                            HtmlControl tabActiveGiftsDiv = (HtmlControl)FindRecursiveControl(this.Page, "tabActiveGiftsDiv");
                            tabActiveGiftsDiv.Style.Add("display", "none");

                            HtmlControl tabHistoryGiftsDiv = (HtmlControl)FindRecursiveControl(this.Page, "tabHistoryGiftsDiv");
                            tabHistoryGiftsDiv.Attributes["class"] += " TransactionManagerCurrentTab";
                        }

                        if (!MyContent.ShowHistory)
                        {
                            LinkButton lnkHistoryTab = (LinkButton)FindRecursiveControl(this.Page, MyContent.HistoryLinkName);
                            lnkHistoryTab.Visible = false;
                        }
                    }

                    //if(MyContent.ShowChildInsteadOfDesignation)
                    //{
                        //DataTable dt = new DataTable();

                        //using(SqlConnection con = new SqlConnection(Blackbaud.Web.Content.Core.Settings.ConnectionString))
                        //{
                        //    using(SqlCommand cmd = new SqlCommand("USR_USP_GETCHILDINFOFORCONSTITUENTID", con))
                        //    {                                
                        //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        //        cmd.Parameters.AddWithValue("@ID", this.API.Users.CurrentUser.BackOfficeGuid);

                        //        using(SqlDataAdapter dta = new SqlDataAdapter(cmd))
                        //        {
                        //            cmd.Connection.Open();
                        //            dta.Fill(dt);
                        //        }
                        //    }
                        //}

                        //Dictionary<Guid, string> children = new Dictionary<Guid,string>();

                        //foreach (DataRow row in dt.Rows)
                        //{
                        //    Guid id = Guid.Empty;
                        //    string designation = string.Format("{0} - {1}", row["NAME"].ToString(), row["LOOKUPID"].ToString());

                        //    Guid.TryParse(row["ID"].ToString(), out id);

                        //    children.Add(id, designation);
                        //}

                        //GivingHistoryGrid grid = (GivingHistoryGrid)FindRecursiveControl(this.Page, "grid");
                        //IList<GivingHistoryDataRow> oldDS = (IList<GivingHistoryDataRow>)grid.DataSource;
                        //IList<GivingHistoryDataRow> newDS = new List<GivingHistoryDataRow>();
                    
                        //if (oldDS != null)
                        //{
                        //    foreach (var row in oldDS)
                        //    {
                        //        Guid recordID = new Guid();

                        //        if(Guid.TryParse(row.Gifts__GiftRecordID, out recordID))
                        //        {
                        //            string desc = row.Gifts__FundraisingPurpose;

                        //            if(children.ContainsKey(recordID))
                        //            {
                        //                desc = children[recordID];
                        //            }

                        //            row.Gifts__FundDescription = desc;
                        //        }

                        //        newDS.Add(row);
                        //    }
                        //}

                        //grid.DataSource = newDS;
                        //grid.DataBind();
                    //}
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private Control FindRecursiveControl(Control root, string ID)
        {
            if (root.ID == ID)
                return root;

            foreach (Control Ctl in root.Controls)
            {
                Control FoundCtl = FindRecursiveControl(Ctl, ID);

                if (FoundCtl != null)
                    return FoundCtl;
            }

            return null;
        }
    }
}