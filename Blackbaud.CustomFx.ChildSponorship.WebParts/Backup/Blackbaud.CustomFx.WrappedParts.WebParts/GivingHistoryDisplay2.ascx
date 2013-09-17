<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GivingHistoryDisplay2.ascx.vb"
  Inherits="Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryDisplay2" %>
<%@ Register TagPrefix="gh2" TagName="ghControl" Src="GivingHistory2Control.ascx" %>
<%@ Register TagPrefix="core" Namespace="Blackbaud.Web.Content.Core.Controls" Assembly="Blackbaud.Web.Content.Core" %>
<core:BBLabel ID="pageError" Visible="false" Encode="false" runat="server"></core:BBLabel>
<asp:PlaceHolder ID="ValidPage" Visible="true" runat="server">
  <div class="TransactionManagerWrapper">
    <div class="TransactionManagerTabsContainer">
      <div id="tabActiveGiftsDiv" runat="server" class="TransactionManagerTab">
        <asp:LinkButton runat="server" ID="lnkActiveTab" value="0" CssClass="TransactionManagerTabLink"></asp:LinkButton>
      </div>
      <div id="tabHistoryGiftsDiv" runat="server" class="TransactionManagerTab">
        <asp:LinkButton runat="server" ID="lnkHistoryTab" value="1" CssClass="TransactionManagerTabLink"></asp:LinkButton>
      </div>
    </div>
    <div class="ui-tabs-content TransactionManagerTabContent">
      <div class="TransactionManagerContentWrapper">
        <div class="TransactionManagerHelpTextDiv">
          <core:BBLabel runat="server" ID="lblFilterHelpText" CssClass="TransactionManagerHelpTextLabel"></core:BBLabel>
        </div>
        <div class="TransactionManagerFilterContainer">
          <div class="TransactionManagerFilterItem">
            <core:BBValidationSummary HeaderText="The following error(s) must be corrected before continuing:"
              ID="ValidationSummary1" runat="server" CssClass="BBFormValidatorSummary"></core:BBValidationSummary>
              <div class="TransactionManagerHelpTextDiv"> 		
		            <core:BBLabel runat="server" ID="lblTruncatedRecordsHelpText" CssClass="TransactionManagerHelpTextLabel" Visible="false"></core:BBLabel>    
              </div>
            <div class="TransactionManagerFieldContainer">
              <core:BBLabel ID="DatePickerLabel" runat="server" AssociatedControlID="datePicker"
                CssClass="BBFieldCaption TransactionManagerDatePickerFieldCaption" />
              <asp:DropDownList ID="datePicker" CssClass="BBFormSelectList" runat="server">
                <asp:ListItem Value="<%$ Code: DateRange.All %>" />
                <asp:ListItem Value="<%$ Code: DateRange.Past1Month %>" />
                <asp:ListItem Value="<%$ Code: DateRange.Past6Months %>" />
                <asp:ListItem Value="<%$ Code: DateRange.Past1Year %>" />
                <asp:ListItem Value="<%$ Code: DateRange.Specific %>" />
              </asp:DropDownList>
            </div>
            <div runat="server" id="divSpecificDateRange" class="TransactionManagerSpecificDateDiv TransactionManagerSpecificDateDivNotSelected">
              <div class="TransactionManagerFieldContainer">
                <core:DatePicker runat="server" ID="dpStartDate" ImageURL="~/images/calendar.gif"
                  IncludeTime="false" />
              </div>
              to
              <div class="TransactionManagerFieldContainer">
                <core:DatePicker runat="server" ID="dpEndDate" ImageURL="~/images/calendar.gif" IncludeTime="false" />
              </div>
            </div>
          </div>
          <div id="fundPickerPanel" class="TransactionManagerFilterItem" runat="server">
            <div class="TransactionManagerFieldContainer">
              <core:BBLabel ID="FundPickerLabel" runat="server" AssociatedControlID="fundPicker"
                CssClass="BBFieldCaption TransactionManagerFundPickerFieldCaption" />
              <asp:DropDownList ID="fundPicker" CssClass="BBFormSelectList" AppendDataBoundItems="true"
                runat="server" />
            </div>
          </div>
          <div class="TransactionManagerFilterItem">
            <div class="TransactionManagerFieldContainer">
              <core:BBLabel ID="GroupPickerLabel" runat="server" AssociatedControlID="groupPicker"
                CssClass="BBFieldCaption TransactionManagerGroupPickerCaption" Text=""/>
                <asp:DropDownList ID="groupPicker" CssClass="BBFormSelectList" runat="server" />
            </div>
          </div>
          <div id="divFilterButton" class="TransactionManagerFilterActions">
            <asp:Button runat="server" ID="btnFilter" Text="Apply" />
          </div>
        </div>
        <asp:MultiView ID="mTabControl" runat="server" ActiveViewIndex="0">
          <asp:View ID="VwActiveHistoryTab" runat="server">
            <div class="TransactionManagerTabContentActiveGifts">
              <div class="TransactionManagerHelpTextDiv">
                <core:BBLabel runat="server" ID="ActiveHistoryHelpTextLabel" CssClass="TransactionManagerHelpTextLabel"></core:BBLabel>
              </div>
              <gh2:ghControl ID="givingHistoryActiveOnly" runat="server" ShowActiveGiftsOnly="true" />
            </div>
          </asp:View>
          <asp:View ID="VwHistoryTab" runat="server">
            <div class="TransactionManagerTabContentFullHistoryGifts">
              <div class="TransactionManagerHelpTextDiv">
                <core:BBLabel runat="server" ID="FullHistoryHelpTextLabel" CssClass="TransactionManagerHelpTextLabel"></core:BBLabel>
              </div>
              <gh2:ghControl ID="givingHistoryAll" runat="server" ShowActiveGiftsOnly="false" />
            </div>
          </asp:View>
        </asp:MultiView>
      </div>
    </div>
  </div>
</asp:PlaceHolder>
