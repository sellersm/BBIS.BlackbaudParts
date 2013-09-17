<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GivingHistoryEditor2.ascx.vb"
    Inherits="Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryEditor2" %>
<%@ Register TagPrefix="uc1" TagName="querylink" Src="~/admin/common/QueryLink.ascx" %>
<%@ Register TagPrefix="uc2" TagName="columnpicker" Src="~/admin/GivingHistory/GivingHistoryColumnPicker.ascx" %>
<%@ Register TagPrefix="uc2" TagName="pagelink" Src="~/admin/PageLink.ascx" %>
<%@ Register Assembly="Blackbaud.AppFx.ContentManagement.UI.WebControls" Namespace="Blackbaud.AppFx.ContentManagement.UI.WebControls"
    TagPrefix="wc" %>
<style type="text/css">
    .SoftCreditHelpLink
    {
        cursor: pointer;
        text-decoration: underline;
        color: Blue;
    }
    .ColumnContainer
    {
        border: 1px solid #CCCCCC;
        padding: 0px 5px 5px 5px;
        overflow: auto;
        margin-top: 15px;
        min-height: 23px;
    }
    .ColumnItem
    {
        border: 1px solid #AAAAAA;
        background-color: #EEF6FF;
        float: left;
        margin-top: 5px;
        margin-right: 3px;
        padding: 3px;
        padding-right: 15px;
        white-space: nowrap;
    }
    .NewColumnLink
    {
        border: 1px solid #AAAAAA;
        background-color: #EEF6FF;
        color: #333333;
        padding: 3px;
        text-decoration: none;
        cursor: pointer;
        margin-top: 8px;
        float: right;
    }
    .NewColumnLink:hover
    {
        cursor: pointer;
        background-color: #DCEBFE;
    }
    .DisabledLink
    {
        cursor: default;
        color: #999999;
    }
    #ColumnCustomContainer .ColumnItem
    {
        cursor: default;
        position: relative;
        background-image:url(<%= page.resolveUrl("~/images/splitbuttondown.gif")%>);
        background-repeat:no-repeat;
        background-position:center right;
        padding:3px 20px 3px 3px;
    }
    #ColumnCustomContainer .ColumnItem:hover
    {
        cursor: pointer;
        background-color: #DCEBFE;
    }
    .CustomColumnCurtain
    {
        background-color: #DCEBFE;
        border: 1px solid #BBBBBB;
        padding: 4px;
        position: absolute;
        margin-top: 21px;
    }
    .CustomColumnCurtain p
    {
        margin: 0px;
        padding: 3px;
    }
    .CurtainMoveDisabled
    {
        color: #999999;
    }
    .CurtainClickable:hover
    {
        cursor: pointer;
    }
    .MsgText
    {
        display: block;
        padding: 4px;
    }
    .GiftQueryLinkCell table tr td span
    {
        width: 98% !important;
    }
</style>
<div style="margin-bottom: 15px; margin-top: 15px; border: 1px solid white;">
    <input id="HiddenAvailableColumnData" type="hidden" name="HiddenAvailableColumnData"
        runat="server" />
    <input id="HiddenUsedColumnData" type="hidden" name="HiddenUsedColumnData" runat="server" />
    <input id="HiddenUsedColumnString" type="hidden" name="HiddenUsedColumnData" runat="server" />
    <input id="HiddenSelectedGiftTypeIDs" type="hidden" name="HiddenSelectedGiftTypeIDs"
        runat="server" />
    <input id="HiddenSelectedCampaignIDs" type="hidden" name="HiddenSelectedCampaignIDs"
        runat="server" />
    <input id="HiddenSelectedFundIDs" type="hidden" name="HiddenSelectedFundIDs" runat="server" />
    <input id="HiddenSelectedAppealIDs" type="hidden" runat="server" />
    <input id="HiddenSelectedRecGiftEditFilterFundIDs" type="hidden" name="HiddenSelectedRecGiftEditFilterFundIDs"
        runat="server" />
    <input id="HiddenSelectedRecGiftEditFilterAppealIDs" type="hidden" runat="server" />
    <div class="StepGrouping">
        <h1 class="StepGroupingHeading">
            Configure results
        </h1>
        <div class="StepGroupingBody">
            <div class="HelpText">
                Select the maximum number of gift records to appear in the Transaction Manager.
                You can display up to 100 records per page.
            </div>
            <div class="SelectOption">
                <div class="SelectOptionField">
                    <div class="FieldHeading">
                        <asp:Label runat="server" ID="ResultsPerPageLabel" Text="Results per page:" AssociatedControlID="ResultsPerPageDropDownList"
                            CssClass="FieldLabel"></asp:Label>
                        <span class="clear"></span>
                    </div>
                    <%-- END FieldHeading --%>
                    <div class="FieldContent">
                        <asp:DropDownList runat="server" ID="ResultsPerPageDropDownList" CssClass="FieldSelect FieldInput">
                            <asp:ListItem Text="10" Value="10">
                            </asp:ListItem>
                            <asp:ListItem Text="25" Value="25" Selected="True">
                            </asp:ListItem>
                            <asp:ListItem Text="50" Value="50">
                            </asp:ListItem>
                            <asp:ListItem Text="100" Value="100">
                            </asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <%-- END FieldContent --%>
                    <div class="clear">
                    </div>
                </div>
                <%-- END SelectOptionField --%>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
    <div id="divGiftsFiltering" runat="server">
        <div class="StepGrouping">
            <h1 class="StepGroupingHeading">
                Choose the gifts to display
            </h1>
            <div class="StepGroupingBody">
                <div class="HelpText">
                    You can include all gifts in a constituent's Transaction Manager or filter gifts
                    to meet your criteria. To filter gifts, enter the criteria here or use a&nbsp;<asp:Label
                        ID="lblBackOfficeName" runat="server"></asp:Label>&nbsp;query.
                </div>
                <div class="RadioContainer">
                    <div class="RadioGrouping">
                        <p id="GiftsDefaultItem" class="RadioSelected">
                            <asp:RadioButton runat="server" ID="GiftsDefaultRadioButton" GroupName="GiftGroup"
                                Text="All gifts" Checked="true"></asp:RadioButton>
                        </p>
                        <p id="GiftsCustomItem">
                            <asp:RadioButton runat="server" ID="GiftsCustomRadioButton" GroupName="GiftGroup"
                                Text="Custom filtering criteria"></asp:RadioButton>
                        </p>
                        <p id="GiftsREQueryItem">
                            <asp:RadioButton runat="server" ID="GiftsREQueryRadioButton" GroupName="GiftGroup"
                                Text="Gift query"></asp:RadioButton>
                        </p>
                    </div>
                    <div class="SelectedArea">
                        <div class="SelectedAreaInner">
                            <div id="GiftsDefaultPanel">
                                <p class="SelectedDescription">
                                    This option displays all gift types,&nbsp;<asp:Label ID="lblBackOfficeCFA" runat="server"></asp:Label>&nbsp;for
                                    all gift dates.</p>
                                <p class="SelectedDescription">
                                    To filter the gifts, select <b>Custom filtering criteria</b>.</p>
                            </div>
                            <div id="GiftsCustomPanel" style="display: none;">
                                <p class="SelectedDescription">
                                    This option filters gifts by gift type,&nbsp;<asp:Label ID="lblBackOfficeCFA2" runat="server"></asp:Label>.</p>
                                <p class="SelectedDescription">
                                    To display all gifts, select <b>All gifts</b>.</p>
                                <table style="font-size: x-small; margin-top: 5px;" cellspacing="3" cellpadding="0"
                                    border="0" width="95%">
                                    <tr valign="top">
                                        <td class="wsNowrap taRight" width="95px">
                                            Gift Types:
                                        </td>
                                        <td class="PickerDescription">
                                            <asp:Label ID="lblGiftTypes" runat="server" CssClass="MsgText">All Types</asp:Label>
                                        </td>
                                        <td class="PickerSearch" onclick="EditTypes();">
                                            <button id="btnModifyGiftTypes" type="button" class="PickerSearchInput">
                                                Change</button>
                                        </td>
                                    </tr>
                                    <tr valign="top" runat="server" id="tr_CampaignFilter">
                                        <td class="wsNowrap taRight" width="95px">
                                            <asp:Label ID="lblBackOfficeCampaign" runat="server"></asp:Label>:
                                        </td>
                                        <td class="PickerDescription">
                                            <asp:Label ID="lblCampaigns" runat="server" CssClass="MsgText" Style="">All Campaigns</asp:Label>
                                        </td>
                                        <td class="PickerSearch" onclick="EditCampaigns();">
                                            <button id="btnModifyCampaigns" type="button" class="PickerSearchInput">
                                                Change</button>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td class="wsNowrap taRight" width="95px">
                                            <asp:Label ID="lblBackOfficeFund" runat="server"></asp:Label>:
                                        </td>
                                        <td class="PickerDescription">
                                            <asp:Label ID="lblFunds" runat="server" CssClass="MsgText" Style="">All Funds</asp:Label>
                                        </td>
                                        <td class="PickerSearch" onclick="EditFunds();">
                                            <button id="btnModifyFunds" type="button" class="PickerSearchInput">
                                                Change</button>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td class="wsNowrap taRight" width="95px">
                                            Appeals:
                                        </td>
                                        <td class="PickerDescription">
                                            <asp:Label ID="lblAppeals" runat="server" CssClass="MsgText" Style="">All Appeals</asp:Label>
                                        </td>
                                        <td class="PickerSearch" onclick="EditAppeals();">
                                            <button id="btnModifyAppeals" type="button" class="PickerSearchInput">
                                                Change</button>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="GiftsREQueryPanel" style="display: none;">
                                <p class="SelectedDescription">
                                    This option uses a query from
                                    <asp:Label ID="lblQueryBackOfficeName" runat="server" Text="The Raiser’s Edge" />
                                    to filter gifts.</p>
                                <p class="SelectedDescription">
                                    To display all gifts, select <b>All gifts</b>.</p>
                                <table style="margin-top: 5px; width: 98%">
                                    <tr>
                                        <td style="width: 100px;">
                                            <asp:Label ID="lblBOQueryType" runat="server" Text="Gift Query:"></asp:Label>
                                        </td>
                                        <td class="GiftQueryLinkCell">
                                            <uc1:querylink ID="GiftQueryLink" AllRecordsText="None" CanDelete="false" runat="server"
                                                BBSystem="RE" REQueryType="Gift" />
                                        </td>
                                        <td>
                                            <span class="RequiredFieldMarker">*</span>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="VerticalOption">
                    <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="CheckboxOptionField">
                                <asp:CheckBox ID="SoftCreditDisplayCheckBox" onclick="enableDisableSoftCreditTotals(this);"
                                    runat="server"></asp:CheckBox>
                            </td>
                            <td class="CheckboxOptionHelpArrow">
                                <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                            </td>
                            <td class="CheckboxOptionHelp">
                                <span class="CheckboxOptionHelpText">
                                    <asp:Label ID="lblSoftCreditHelp" runat="server"></asp:Label>
                                    <a id="A1" onclick="window.open(SOFT_CREDIT_HELP_LINK);" class="SoftCreditHelpLink">
                                        click here</a>. </span>
                            </td>
                        </tr>
                    </table>
                    <%-- END CheckboxOption --%>
                </div>
                <div class="VerticalOption">
                    <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="CheckboxOptionField">
                                <asp:CheckBox ID="PendingTransactionDisplayCheckBox" runat="server" Checked="true"
                                    Text="Include pending online transactions"></asp:CheckBox>
                            </td>
                            <td class="CheckboxOptionHelpArrow">
                                <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                            </td>
                            <td class="CheckboxOptionHelp">
                                <span class="CheckboxOptionHelpText">If you include pending transactions, the Transaction
                                    Manager displays gifts you receive on your website that have not yet been processed.
                                    These online transactions appear regardless of any filters you apply to the Transaction
                                    Manager data. </span>
                            </td>
                        </tr>
                    </table>
                    <%-- END CheckboxOption --%>
                </div>
                <div id="divUnpaidEvents" runat="server" class="VerticalOption">
                    <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="CheckboxOptionField">
                                <asp:CheckBox ID="cbUnpaidEvents" onclick="enableDisableUnpaidEvents(this);" runat="server"
                                    Text="Include unpaid event registrations" />
                            </td>
                            <td class="CheckboxOptionHelpArrow">
                                <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                            </td>
                            <td class="CheckboxOptionHelp">
                                <span class="CheckboxOptionHelpText">If you include unpaid event registrations, the
                                    Transaction Manager displays information about events for which the fees have not
                                    been paid and allows the user to pay the associated fees.</span>
                            </td>
                        </tr>
                    </table>
                    <%-- END CheckboxOption --%>
                </div>
                <div class="clear">
                </div>
            </div>
        </div>
    </div>
    <div class="StepGrouping" runat="server" ID="divChooseColumnsSection">
        <h1 class="StepGroupingHeading">
            Choose the columns to display
        </h1>
        <div class="StepGroupingBody">
            <div class="HelpText">
                <%--CR318371-042109 ajm - added text to label so we can localize it --%>
                <asp:Label ID="lblCustomColumnsHelp" runat="server" Text="You can customize the appearance and content of the Transaction Manager. You can add and remove columns, and you can rearrange the order. To rename the column headings, use the Language tab.">
                </asp:Label>
            </div>
            <div class="RadioContainer">
                <div class="RadioGroupingNarrow">
                    <p id="ColumnDefaultItem" class="RadioSelected">
                        <asp:RadioButton runat="server" ID="ColumnDefaulRadioButton" GroupName="ColumnGroup"
                            Text="Default" Checked="true"></asp:RadioButton>
                    </p>
                    <p id="ColumnCustomItem">
                        <asp:RadioButton runat="server" ID="ColumnCustomRadioButton" GroupName="ColumnGroup"
                            Text="Custom"></asp:RadioButton>
                    </p>
                </div>
                <div class="SelectedAreaNarrow">
                    <div class="SelectedAreaInner">
                        <div id="ColumnDefaultPanel">
                            <p class="SelectedDescription">
                                This option displays the default Transaction Manager configuration.</p>
                            <%--CR318371-042109 ajm - added text to label so we can localize it --%>
                            <p class="SelectedDescription">
                                <asp:Label ID="lblDefaultPanelHelp" runat="Server" Text="To customize the columns, select" />
                                <b>Custom</b>.</p>
                            <div class="ColumnContainer">
                                <div class="ColumnItem"><asp:Label ID="lblDate" runat="server" Text="Date"></asp:Label></div>
                                <div class="ColumnItem"><asp:Label ID="lblGiftType" runat="server" Text="Gift Type"></asp:Label></div>
                                <div class="ColumnItem"><asp:Label ID="lblFundColumn" runat="server" Text="Designation"></asp:Label></div>
                                <div class="ColumnItem"><asp:Label ID="lblAmount" runat="server" Text="Amount"></asp:Label></div>
                                <asp:Literal ID="SpecificColumnsUK" runat="server">
									<div class="ColumnItem">Gift Aid Amount</div>
                                </asp:Literal>
                            </div>
                        </div>
                        <div id="ColumnCustomPanel" style="display: none;">
                            <%--CR318371-042109 ajm - added text to label so we can localize it --%>
                            <p class="SelectedDescription">
                                <asp:Label ID="lblCustomPanelHelp" runat="server" Text="This option allows you to customize the appearance of the Transaction Manager page. You can remove columns, add columns to display additional information, and reposition columns.">
                                </asp:Label>
                            </p>
                            <div id="ColumnCustomContainer" class="ColumnContainer" onmouseover="ToggleCurtainIn()"
                                onmouseout="ToggleCurtainOut()">
                            </div>
                            <a class="NewColumnLink" id="addColumn">+ Add Column</a>
                            <div class="clear">
                            </div>
                            <div id="ColumnCustomCurtain" class="CustomColumnCurtain" style="display: none;"
                                onmouseover="ToggleCurtainIn()" onmouseout="ToggleCurtainOut()">
                                <!--<p id="CurtainColumnName">Column Name</p>-->
                                <p id="CurtainMoveLeft" class="CurtainClickable">
                                    Move Left</p>
                                <p id="CurtainMoveRight" class="CurtainClickable">
                                    Move Right</p>
                                <!--<p id="CurtainChangeLabel" class="CurtainClickable">Change Label</p>-->
                                <p id="CurtainRemove" class="CurtainClickable">
                                    Remove</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
    <div class="StepGrouping">
        <h1 class="StepGroupingHeading">
            Choose the summary information to display
        </h1>
        <div class="StepGroupingBody">
            <div class="HelpText">
                You can add a summary section that displays gift totals. Select the columns of data
                to include.
            </div>
            <div class="VerticalOption">
                <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="CheckboxOptionField">
                            <asp:CheckBox ID="chkIncludeSummary" runat="server" Checked="true" Text="Include gift totals section" />
                        </td>
                        <td class="CheckboxOptionHelpArrow">
                            <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                        </td>
                        <td class="CheckboxOptionHelp">
                            <span class="CheckboxOptionHelpText">This option adds a summary section for gift totals.<asp:Label
                                ID="lblGiftTotalsHelp" runat="server"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </table>
                <%-- END CheckboxOption --%>
                <div id="summarySubOptions" runat="server" class="SubOptionContainer">
                    <div class="VerticalOption">
                        <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                            <tr>
                                <td class="CheckboxOptionField">
                                    <asp:CheckBox ID="chkIncludeTotalsCurrency" runat="server" Checked="true" Text="Include currency types" />
                                </td>
                                <td class="CheckboxOptionHelpArrow">
                                    <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                </td>
                                <td class="CheckboxOptionHelp">
                                    <span class="CheckboxOptionHelpText">This option adds a column that displays all currencies
                                        the constituent used.</span>
                                </td>
                            </tr>
                        </table>
                        <%-- END CheckboxOption --%>
                    </div>
                    <%-- END VerticalOption --%>
                    <div class="VerticalOption">
                        <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                            <tr>
                                <td class="CheckboxOptionField">
                                    <asp:CheckBox ID="chkIncludeGiftTotal" runat="server" Checked="true" Text="Include gift total" />
                                </td>
                                <td class="CheckboxOptionHelpArrow">
                                    <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                </td>
                                <td class="CheckboxOptionHelp">
                                    <span class="CheckboxOptionHelpText">This option adds a column that displays the total
                                        gift amount. This excludes all pending gifts, pending pledged gifts, and non-pending
                                        pledged gifts.</span>
                                </td>
                            </tr>
                        </table>
                        <%-- END CheckboxOption --%>
                    </div>
                    <%-- END VerticalOption --%>
                    <asp:Panel ID="SoftCreditsPanel" runat="server">
                        <div class="VerticalOption">
                            <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td class="CheckboxOptionField">
                                        <asp:CheckBox ID="chkIncludeSoftCreditTotal" runat="server" Checked="true" Text="Include Soft Credit total" />
                                    </td>
                                    <td class="CheckboxOptionHelpArrow">
                                        <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                    </td>
                                    <td class="CheckboxOptionHelp">
                                        <span class="CheckboxOptionHelpText">
                                            <asp:Literal ID="SoftCreditTotalLiteral" runat="server">This option adds a column that displays the total
                    soft credit amount.</asp:Literal></span>
                                    </td>
                                </tr>
                            </table>
                            <%-- END CheckboxOption --%>
                        </div>
                        <%-- END VerticalOption --%>
                        <div class="VerticalOption">
                            <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td class="CheckboxOptionField">
                                        <asp:CheckBox ID="chkIncludeHardCreditTotal" runat="server" Checked="true" Text="Include donation amount total" />
                                    </td>
                                    <td class="CheckboxOptionHelpArrow">
                                        <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                    </td>
                                    <td class="CheckboxOptionHelp">
                                        <span class="CheckboxOptionHelpText">
                                            <asp:Literal ID="HardCreditTotalLiteral" runat="server">
                  This option adds a column that displays the total donation amount. This excludes all pending gifts, 
                  pending pledge gifts, non-pending pledged gifts, and recognition credits.
                                            </asp:Literal>
                                        </span>
                                    </td>
                                </tr>
                            </table>
                            <%-- END CheckboxOption --%>
                        </div>
                    </asp:Panel>
                    <%-- END VerticalOption --%>
                    <div class="VerticalOption">
                        <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                            <tr>
                                <td class="CheckboxOptionField">
                                    <asp:CheckBox ID="chkIncludeGiftAidTotal" runat="server" Checked="true" Text="Include Gift Aid total" />
                                </td>
                                <td class="CheckboxOptionHelpArrow">
                                    <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                </td>
                                <td class="CheckboxOptionHelp">
                                    <span class="CheckboxOptionHelpText">This option adds a column that displays the total
                                        Gift Aid amount claimed.</span>
                                </td>
                            </tr>
                        </table>
                        <%-- END CheckboxOption --%>
                    </div>
                    <%-- END VerticalOption --%>
                    <div class="VerticalOption" runat="server" id="PledgeTotalCheckBoxPanel">
                        <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                            <tr>
                                <td class="CheckboxOptionField">
                                    <asp:CheckBox ID="chkIncludePledgeTotal" runat="server" Checked="true" Text="Include pledged gifts total" />
                                </td>
                                <td class="CheckboxOptionHelpArrow">
                                    <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                </td>
                                <td class="CheckboxOptionHelp">
                                    <span class="CheckboxOptionHelpText">This option adds a column that displays the total
                                        pledged gift amount.</span>
                                </td>
                            </tr>
                        </table>
                        <%-- END CheckboxOption --%>
                    </div>
                    <%-- END VerticalOption --%>
                    <div runat="server" id="divPendingTotal" class="VerticalOption">
                        <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                            <tr>
                                <td class="CheckboxOptionField">
                                    <asp:CheckBox ID="chkIncludePendingTotal" runat="server" Checked="true" Text="Include pending gifts total" />
                                </td>
                                <td class="CheckboxOptionHelpArrow">
                                    <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                </td>
                                <td class="CheckboxOptionHelp">
                                    <span class="CheckboxOptionHelpText">This option adds a column that displays the total
                                        pending gift amount. You cannot select this option if you did not select include
                                        pending online transactions.</span>
                                </td>
                            </tr>
                        </table>
                        <%-- END CheckboxOption --%>
                    </div>
                    <%-- END VerticalOption --%>
                    <div class="VerticalOption" runat="server" id="PledgeBalanceTotalPanel">
                        <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                            <tr>
                                <td class="CheckboxOptionField">
                                    <asp:CheckBox ID="chkIncludeBalanceTotal" runat="server" Checked="true" Text="Include remaining pledge balance total" />
                                </td>
                                <td class="CheckboxOptionHelpArrow">
                                    <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                </td>
                                <td class="CheckboxOptionHelp">
                                    <span class="CheckboxOptionHelpText">This option adds a column that displays the total
                                        remaining pledge amount.</span>
                                </td>
                            </tr>
                        </table>
                        <%-- END CheckboxOption --%>
                    </div>
                    <%-- END VerticalOption --%>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
    <asp:Panel ID="PledgePanel" runat="server" CssClass="StepGrouping">
        <h1 class="StepGroupingHeading">
            Configure pledge payment
        </h1>
        <div class="StepGroupingBody">
            <div class="HelpText">
                Select a page that contains a Donation Form for your online donors to use to pay
                outstanding pledges. The selected page must allow either credit card or direct debit
                transactions.
            </div>
            <%--<div class="SelectOption">
	            <div class="SelectOptionField">
                    <div class="FieldHeading">
						<asp:Label runat="server" ID="Label1" Text="Donation Form for pledge payment:" AssociatedControlID="ddlPledgePaymentPage" CssClass="FieldLabel"></asp:Label>
						<span class="clear"></span>
                    </div><%-- END FieldHeading
                    
                    <div class="FieldContent">
						<asp:DropDownList runat="server" ID="ddlPledgePaymentPage" CssClass="FieldSelect FieldInput">
						</asp:DropDownList>                    
					</div><%-- END FieldContent
					<div class="clear"></div>
				</div><%-- END SelectOptionField
			</div>
			--%>
            <div class="VerticalOption">
                <div class="SelectOption">
                    <div class="SelectOptionField">
                        <div class="FieldContent">
                            <asp:Label ID="lblPledgePaymentPage" Text="" runat="server" CssClass="FieldLabel"
                                AssociatedControlID="PledgePaymentPageLink"></asp:Label>
                            <uc2:pagelink ID="PledgePaymentPageLink" ContentTypeFilter="45" CanDelete="true"
                                Required="false" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
            <%-- END VerticalOption --%>
            <div class="clear">
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="UnpaidEventPanel" runat="server" CssClass="StepGrouping">
        <h1 class="StepGroupingHeading">
            Configure event fee payment
        </h1>
        <div class="StepGroupingBody">
            <div class="HelpText">
                Select a page that contains a Payment 2.0 part for your online donors to use to pay
                event fees. The selected page must allow either credit card or direct debit transactions.
            </div>
            <div class="VerticalOption">
                <div class="SelectOption">
                    <div class="SelectOptionField">
                        <div class="FieldContent">
                            <asp:Label ID="lblUnpaidEventPage" Text="" runat="server" CssClass="FieldLabel" AssociatedControlID="UnpaidEventPageLink">
                            </asp:Label>
                            <uc2:pagelink ID="UnpaidEventPageLink" ContentTypeFilter="9102" CanDelete="true" Required="false"
                                runat="server" />
                        </div>
                    </div>
                </div>
            </div>
            <%-- END VerticalOption --%>
            <div class="clear">
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlAdditionalDonation" runat="server" CssClass="StepGrouping">
        <h1 class="StepGroupingHeading">
            Additional donation for recurring gifts
        </h1>
        <div class="StepGroupingBody">
            <div class="HelpText">
                Select a page that contains a Donation Form for your online donors to use to make
                an additional donation to existing recurring gifts.
            </div>
            <div class="VerticalOption">
                <div class="SelectOption">
                    <div class="SelectOptionField">
                        <div class="FieldContent">
                            <asp:Label ID="lblRecGiftAdditionalDonationPageLink" Text="" runat="server" CssClass="FieldLabel"
                                AssociatedControlID="RecGiftAdditionalDonationPageLink"></asp:Label>
                            <uc2:pagelink ID="RecGiftAdditionalDonationPageLink" ContentTypeFilter="45" runat="server"
                                CanDelete="true" Required="false" />
                        </div>
                    </div>
                </div>
            </div>
            <%-- END VerticalOption --%>
            <div class="clear">
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="RecurringGiftPaymentPanel" runat="server" CssClass="StepGrouping">
        <h1 class="StepGroupingHeading">
            Configure recurring gift payment
        </h1>
        <div class="StepGroupingBody">
            <div class="HelpText">
                Select the page that contains a Donation Form for your online donors to use to
                pay recurring gifts associated with a sponsorship.
            </div>
            <div class="VerticalOption">
                <div class="SelectOption">
                    <div class="SelectOptionField">
                        <div class="FieldContent">
                            <asp:Label ID="lblRecGiftPaymentPageLink" Text="" runat="server" CssClass="FieldLabel"
                                AssociatedControlID="RecGiftPaymentPageLink"></asp:Label>
                            <uc2:pagelink ID="RecGiftPaymentPageLink" ContentTypeFilter="45" CanDelete="true" runat="server" Required="false" />
                        </div>
                    </div>
                </div>
            </div>
            <%-- END VerticalOption --%>
            <div class="clear">
            </div>
        </div>
    </asp:Panel>
    <div id="divRecurringGiftUpdates" runat="server" class="StepGrouping">
        <h1 class="StepGroupingHeading">
            Recurring gift updates
        </h1>
        <div class="StepGroupingBody">
            <div class="HelpText">
                Select which recurring gifts in this Transaction Manager can be updated.
            </div>
            <%-- EDITABLE GIFTS FILTER OPTIONS --%>
            <div class="RadioContainer">
                <div class="RadioGrouping">
                    <p id="RecGftUpdFilterDefaultItem" class="RadioSelected">
                        <asp:RadioButton runat="server" ID="rdoRecGftUpdFilterAll" GroupName="RecGftUpdGroup"
                            Text="All gifts"></asp:RadioButton>
                    </p>
                    <p id="RecGftUpdFilterCustomItem">
                        <asp:RadioButton runat="server" ID="rdoRecGftUpdFilterCustom" GroupName="RecGftUpdGroup"
                            Text="Custom filtering criteria"></asp:RadioButton>
                    </p>
                    <p id="RecGftUpdFilterQueryItem">
                        <asp:RadioButton runat="server" ID="rdoRecGftUpdFilterQuery" GroupName="RecGftUpdGroup"
                            Text="Revenue query"></asp:RadioButton>
                    </p>
                </div>
                <div class="SelectedArea">
                    <div class="SelectedAreaInner">
                        <div id="RecGftUpdFilterDefaultPanel">
                            <p class="SelectedDescription">
                                This option displays all&nbsp;<asp:Label ID="lblRecGftUpdBOCFA" runat="server"></asp:Label>&nbsp;for
                                all gift dates.</p>
                            <p class="SelectedDescription">
                                To filter the gifts, select <b>Custom filtering criteria</b>.</p>
                        </div>
                        <div id="RecGftUpdFilterCustomPanel" style="display: none;">
                            <p class="SelectedDescription">
                                This option filters gifts by&nbsp;<asp:Label ID="lblRecGftUpdBOCFA2" runat="server"></asp:Label>.</p>
                            <p class="SelectedDescription">
                                To display all gifts, select <b>All gifts</b>.</p>
                            <table style="font-size: x-small; margin-top: 5px;" cellspacing="3" cellpadding="0"
                                border="0" width="95%">
                                <%-- <tr valign="top" runat="server" id="tr1">
                                    <td class="wsNowrap taRight" width="95px">
                                        <asp:Label ID="lblRecGftUpdBOCampaignsText" runat="server"></asp:Label>:
                                    </td>
                                    <td class="PickerDescription">
                                        <asp:Label ID="lblRecGftUpdCampaignsChosen" runat="server" CssClass="MsgText" Style="">All Campaigns</asp:Label>
                                    </td>
                                    <td class="PickerSearch" onclick="RecGftUpdEditCampaigns();">
                                        <button id="btnRecGftUpdModifyCampaigns" type="button" class="PickerSearchInput">
                                            Change</button>
                                    </td>
                                </tr>--%>
                                <tr valign="top">
                                    <td class="wsNowrap taRight" width="95px">
                                        <asp:Label ID="lblRecGftUpdBOFundsText" runat="server"></asp:Label>:
                                    </td>
                                    <td class="PickerDescription">
                                        <asp:Label ID="lblRecGftUpdFundsChosen" runat="server" CssClass="MsgText" Style="">All Funds</asp:Label>
                                    </td>
                                    <td class="PickerSearch" onclick="RecGftUpdEditFunds();">
                                        <button id="btnRecGftUpdModifyFunds" type="button" class="PickerSearchInput">
                                            Change</button>
                                    </td>
                                </tr>
                                <tr valign="top">
                                    <td class="wsNowrap taRight" width="95px">
                                        Appeals:
                                    </td>
                                    <td class="PickerDescription">
                                        <asp:Label ID="lblRecGftUpdAppealsChosen" runat="server" CssClass="MsgText" Style="">All Appeals</asp:Label>
                                    </td>
                                    <td class="PickerSearch" onclick="RecGftUpdEditAppeals();">
                                        <button id="btnRecGftUpdModifyAppeals" type="button" class="PickerSearchInput">
                                            Change</button>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="RecGftUpdFilterQueryPanel" style="display: none;">
                            <p class="SelectedDescription">
                                This option uses a query from
                                <asp:Label ID="lblRecGftUpdBOText" runat="server" Text="The Raiser’s Edge" />
                                to filter gifts.</p>
                            <p class="SelectedDescription">
                                To display all gifts, select <b>All gifts</b>.</p>
                            <table style="margin-top: 5px; width: 98%">
                                <tr>
                                    <td style="width: 100px;">
                                        Revenue Query:
                                    </td>
                                    <td class="GiftQueryLinkCell">
                                        <uc1:querylink ID="RecGftUpdFilterQueryLnk" AllRecordsText="None" CanDelete="false"
                                            runat="server" BBSystem="RE" REQueryType="Gift" />
                                    </td>
                                    <td>
                                        <span class="RequiredFieldMarker">*</span>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
            <%-- AMOUNT UPDATES --%>
            <div class="VerticalOption">
                <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="CheckboxOptionField">
                            <asp:CheckBox ID="chkAllowRecurringAmountUpdates" runat="server" Checked="true" Text="Allow amount updates" />
                        </td>
                        <td class="CheckboxOptionHelpArrow">
                            <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                        </td>
                        <td class="CheckboxOptionHelp">
                            <span class="CheckboxOptionHelpText">This option allows a donor to make updates to the
                                amount of existing recurring gifts.<asp:Label ID="lblAllowRecurringAmountUpdatesHelp"
                                    runat="server"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </table>
                <%-- END CheckboxOption --%>
                <div id="divSubOptionAllowRecAmtUpdates" runat="server" class="SubOptionContainer">
                    <div class="VerticalOption">
                        <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                            <tr>
                                <td class="CheckboxOptionField" style="padding-left: 5px;">
                                    <div class="FieldHeading">
                                        <asp:Label runat="server" ID="lblAllowRecAmtUpdatesMinAmt" Text="Minimum Gift Amount:"
                                            AssociatedControlID="txtAllowRecAmtUpdatesMinAmt" CssClass="FieldLabel"></asp:Label>
                                        <span class="clear"></span>
                                    </div>
                                    <%-- END FieldHeading --%>
                                    <div class="FieldContent">
                                        <asp:TextBox ID="txtAllowRecAmtUpdatesMinAmt" runat="server" MaxLength="6">
                                        </asp:TextBox>
                                    </div>
                                    <div class="clear">
                                    </div>
                                </td>
                                <td class="CheckboxOptionHelpArrow">
                                    <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                </td>
                                <td class="CheckboxOptionHelp">
                                    <span class="CheckboxOptionHelpText">Specify the minimum amount that a donor must give.
                                        Enter 0 or leave blank for no minimum amount.</span>
                                </td>
                            </tr>
                        </table>
                        <%-- END CheckboxOption --%>
                    </div>
                    <%-- END VerticalOption --%>
                </div>
            </div>
            <%-- END VerticalOption --%>
            <div class="clear">
            </div>
            <%-- FREQUENCY UPDATES --%>
            <div class="VerticalOption">
                <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="CheckboxOptionField">
                            <asp:CheckBox ID="chkAllowFrequencyUpdates" runat="server" Checked="true" Text="Allow frequency updates" />
                        </td>
                        <td class="CheckboxOptionHelpArrow">
                            <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                        </td>
                        <td class="CheckboxOptionHelp">
                            <span class="CheckboxOptionHelpText">This option allows a donor to make updates to the
                                frequency of recurring gifts.<asp:Label ID="lblAllowFrequencyUpdatesHelp" runat="server"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </table>
                <%-- END CheckboxOption --%>
                <div id="divSubOptionAllowFrequencyUpdates" runat="server" class="SubOptionContainer">
                    <div class="VerticalOption">
                        <div class="HelpText">
                            Set the method to determine options for recurring gifts.
                        </div>
                        <div id="radioContainerDiv" class="RadioContainer">
                            <div id="radioGroupRecurrenceDiv" class="RadioGroupingNarrow">
                                <p id="generalRecurrenceItem">
                                    <asp:RadioButton runat="server" ID="generalRecurrenceRadio" GroupName="GroupRecurrence"
                                        Text="General" />
                                </p>
                                <p id="specificRecurrenceItem" class="RadioSelected">
                                    <asp:RadioButton runat="server" ID="specificRecurrenceRadio" GroupName="GroupRecurrence"
                                        Text="Specific" />
                                </p>
                            </div>
                            <div class="SelectedAreaNarrow">
                                <div class="SelectedAreaInner">
                                    <div id="generalRecurrencePanel" style="display: none;">
                                        <span class="DonationSetupInstructionCell">The donor will provide the giving frequency
                                            when editing the gift.</span></div>
                                    <div id="specificRecurrencePanel">
                                        <div class="HelpText">
                                            First select the frequency settings, then click the <strong>Add Frequency</strong>
                                            button to add the option to the edit form.
                                        </div>
                                        <div style="padding-bottom: 5px;">
                                            <asp:Label ID="lblRecGiftFrequencyGridError" runat="server" Style="color: Red;"></asp:Label>
                                        </div>
                                        <div class="GridContainer">
                                            <asp:DataGrid ID="RecurrenceDataGrid" runat="server" CssClass="DataGrid" AutoGenerateColumns="False"
                                                BorderWidth="0" BorderStyle="None" CellPadding="0" CellSpacing="0" border="0"
                                                GridLines="None">
                                                <ItemStyle CssClass="DataGridItem"></ItemStyle>
                                                <AlternatingItemStyle CssClass="DataGridItemAlternating" />
                                                <HeaderStyle CssClass="DataGridHeader"></HeaderStyle>
                                                <Columns>
                                                    <asp:TemplateColumn Visible="False">
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblData"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Frequency" HeaderStyle-Width="100%">
                                                        <ItemStyle Wrap="False" CssClass="DataGridItemCell DataGridItemCellLeft"></ItemStyle>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblFrequency" runat="server">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn>
                                                        <ItemStyle HorizontalAlign="Center" CssClass="DataGridItemCell DataGridItemCellRight">
                                                        </ItemStyle>
                                                        <ItemTemplate>
                                                            <asp:Button ID="removeFrequency" runat="server" CssClass="CommandButton" Text="Remove"
                                                                ToolTip="Remove" CausesValidation="False" CommandArgument="Remove"></asp:Button>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                            <div class="ListAddition">
                                                <div class="ListAdditionContent">
                                                    <wc:RecurringGiftSchedule ID="RecurringGiftSchedule" runat="server" />
                                                </div>
                                                <div class="ListAdditionButton">
                                                    <asp:Button ID="AddRecurrenceButton" runat="server" CssClass="CommandButton" Text="Add Frequency"
                                                        Width="125px" CausesValidation="False"></asp:Button>
                                                </div>
                                            </div>
                                            <%-- END ListAddition --%>
                                        </div>
                                        <%-- END GridContainer --%>
                                    </div>
                                    <%-- END id="divRecurSpecific" --%>
                                </div>
                                <%-- END SelectedAreaInner --%>
                            </div>
                            <%-- END SelectedAreaNarrow --%>
                            <div class="clear">
                            </div>
                        </div>
                        <%-- END RadioContainer --%>
                        <div class="VerticalOption">
                            <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td class="CheckboxOptionField">
                                        <asp:CheckBox ID="RecurringGiftScheduleStartDateCheckBox" runat="server" Text="Allow start date changes">
                                        </asp:CheckBox>
                                    </td>
                                    <td class="CheckboxOptionHelpArrow">
                                        <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                    </td>
                                    <td class="CheckboxOptionHelp">
                                        <span class="CheckboxOptionHelpText">This option allows a donor to update the start
                                            date of existing recurring gifts that have not commenced.<asp:Label ID="Label1" runat="server"></asp:Label>
                                        </span>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="VerticalOption">
                            <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td class="CheckboxOptionField">
                                        <asp:CheckBox ID="RecurringGiftScheduleEndDateCheckBox" runat="server" Text="Allow ending date changes">
                                        </asp:CheckBox>
                                    </td>
                                    <td class="CheckboxOptionHelpArrow">
                                        <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                                    </td>
                                    <td class="CheckboxOptionHelp">
                                        <span class="CheckboxOptionHelpText">This option allows a donor to enter the ending
                                            date of existing recurring gifts.<asp:Label ID="Label2" runat="server"></asp:Label>
                                        </span>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <%-- END VerticalOption --%>
                </div>
            </div>
            <%-- END VerticalOption --%>
            <div class="clear">
            </div>
            <%-- PAYMENT TYPE UPDATES --%>
            <div class="VerticalOption">
                <table class="CheckboxOption" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="CheckboxOptionField">
                            <asp:CheckBox ID="chkAllowPaymentTypeUpdates" runat="server" Checked="true" Text="Allow payment updates" />
                        </td>
                        <td class="CheckboxOptionHelpArrow">
                            <img src="<%=page.resolveUrl("~/images/FieldHelpArrow.gif") %>" alt="" />
                        </td>
                        <td class="CheckboxOptionHelp">
                            <span class="CheckboxOptionHelpText">This option allows a donor to make updates to the
                                credit card and direct debit information for existing recurring gifts.<asp:Label
                                    ID="lblAllowPaymentTypeUpdatesHelp" runat="server"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </table>
                <%-- END CheckboxOption --%>
            </div>
            <%-- END VerticalOption --%>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
