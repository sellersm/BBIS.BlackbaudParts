<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GivingHistory2Control.ascx.vb"
	Inherits="Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistory2Control" %>
<%@ Register TagPrefix="gh" Namespace="Blackbaud.Web.Content.Portal.GivingHistory"
	Assembly="Blackbaud.Web.Content.Portal" %>
<%@ Register TagPrefix="core" Namespace="Blackbaud.Web.Content.Core.Controls" Assembly="Blackbaud.Web.Content.Core" %>
<%@ Register TagPrefix="comp" Namespace="Blackbaud.Web.Content.Portal.Components"
	Assembly="Blackbaud.Web.Content.Portal" %>
<%@ Register TagPrefix="uc" Namespace="Blackbaud.Web.Content.Portal.UpdateControls"
	Assembly="Blackbaud.Web.Content.Portal" %>
<div class="TransactionManagerInformationGrid">
	<uc:StyledUpdatePanel ID="GivingHistoryContainer" UpdateMode="Conditional" runat="server">
		<ContentTemplate>
			<div class="TransactionManagerHelpTextDiv">
				<core:BBLabel runat="server" ID="lblGridHelpText" CssClass="TransactionManagerHelpTextLabel"></core:BBLabel>
			</div>
			<div>
				<asp:panel id="ExportContainer" runat="server" visible="false" cssclass="TransactionManagerExportContainer">
					<core:BBLabel ID="ExportTitleLabel" runat="server" Text="Export" />
					<core:BBLinkButton runat="server" ID="ExportDropDownButton" Width="16px" OnClientClick="javascript: return false;"
						CssClass="TransactionManagerExportButton" />
					<asp:panel id="ExportListPanel" runat="server" cssclass="TransactionManagerExportOptionsContainer">
						<ul>
							<li>
								<core:BBLinkButton ID="PdfLinkButton" runat="server" CssClass="TransactionManagerExportLink">
              <img src="./images/transaction_manager_pdf.png" alt="pdf" width="16" height="16" />
              <asp:Literal ID="PdfLinkText" runat="server" Text="PDF" />
								</core:BBLinkButton>
							</li>
							<li>
								<core:BBLinkButton ID="CsvLinkButton" runat="server" CssClass="TransactionManagerExportLink">
              <img src="./images/transaction_manager_csv.png" alt="csv" width="16" height="16" />
              <asp:Literal ID="CsvLinkText" runat="server" Text="CSV" />
								</core:BBLinkButton>
							</li>
						</ul>
					</asp:panel>
				</asp:panel>
			</div>
			<gh:GivingHistoryGrid ID="grid" runat="server" AllowPaging="false" />
			<comp:Pager ID="pager1" runat="server" GenerateFirstLastSection="true" GenerateGoToSection="false"
				GenerateToolTips="false" Visible="false" />
			<div class="TransactionManagerSummaryContainer">
				<div class="TransactionManagerHelpTextDiv">
					<core:BBLabel runat="server" ID="lblSummaryHelpText" CssClass="TransactionManagerHelpTextLabel"></core:BBLabel>
				</div>                
				<asp:gridview id="summary1" cssclass="TransactionManagerSummaryTable" cellpadding="0"
					cellspacing="-1" gridlines="None" enableviewstate="false" autogeneratecolumns="false"
					useaccessibleheader="true" runat="server">
					<headerstyle cssclass="TransactionManagerSummaryHeaderRow" />
					<rowstyle cssclass="TransactionManagerSummaryDetailOddRow" />
					<alternatingrowstyle cssclass="TransactionManagerSummaryDetailEvenRow" />
					<columns>
            <asp:BoundField DataField="Currency" HeaderText="Currency" HeaderStyle-CssClass="TransactionManagerSummaryHeaderLabel"
              ItemStyle-CssClass="TransactionManagerSummaryDetailLabel" />
            <asp:BoundField DataField="GiftTotal" HeaderText="Gift Total" HeaderStyle-CssClass="TransactionManagerSummaryHeaderValue"
              ItemStyle-CssClass="TransactionManagerSummaryDetailValue" />
            <asp:BoundField DataField="GiftAidTotal" HeaderText="Gift Aid Total" HeaderStyle-CssClass="TransactionManagerSummaryHeaderValue"
              ItemStyle-CssClass="TransactionManagerSummaryDetailValue" />
            <asp:BoundField DataField="PledgeTotal" HeaderText="Pledge Total" HeaderStyle-CssClass="TransactionManagerSummaryHeaderValue"
              ItemStyle-CssClass="TransactionManagerSummaryDetailValue" />
            <asp:BoundField DataField="PendingTotal" HeaderText="Pending Total" HeaderStyle-CssClass="TransactionManagerSummaryHeaderValue"
              ItemStyle-CssClass="TransactionManagerSummaryDetailValue" />
            <asp:BoundField DataField="PledgeBalanceTotal" HeaderText="Remaining Pledge Balance"
              HeaderStyle-CssClass="TransactionManagerSummaryHeaderValue" ItemStyle-CssClass="TransactionManagerSummaryDetailValue" />
              <asp:BoundField DataField="SoftCreditTotal" HeaderText="Soft Credit Total"
               HeaderStyle-CssClass="TransactionManagerSummaryHeaderValue" ItemStyle-CssClass="TransactionManagerSummaryDetailValue" />
              <asp:BoundField DataField="HardCreditTotal" HeaderText="Donation Total"
               HeaderStyle-CssClass="TransactionManagerSummaryHeaderValue" ItemStyle-CssClass="TransactionManagerSummaryDetailValue" />
               
          </columns>
				</asp:gridview>
			</div>
		</ContentTemplate>
		<Triggers>
			<asp:postbacktrigger controlid="PdfLinkButton" />
			<asp:postbacktrigger controlid="CsvLinkButton" />
		</Triggers>
	</uc:StyledUpdatePanel>
</div>
