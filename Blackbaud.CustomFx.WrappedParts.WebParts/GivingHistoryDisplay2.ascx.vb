Imports Blackbaud.Web.Content.Core
Imports Blackbaud.Web.Content.Core.Data
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports Blackbaud.AppFx.CMS.FCL
Imports System.Linq
Imports Blackbaud.Web.Content.Portal
Imports Blackbaud.Web

Partial Public Class GivingHistoryDisplay2
    Inherits ContentControl

#Region "Constants"
    Const ALL_FUNDS As String = "*"
    Const VS_SELECTED_DATE_OPTION As String = "GivingHistory2DatePickerOption"
#End Region

#Region "Protected Members"
    Protected Enum DateRange
        All
        Past1Month
        Past6Months
        Past1Year
        Specific
    End Enum
#End Region

#Region "Private Members"
    Private _dateRange As Nullable(Of DateRange)
    Private _startDate As Date = Date.MinValue
    Private _endDate As Date = Date.MinValue
    Private _selectedFund As String
    Private _filterChanged As Boolean = False
    Private _tabChanged As Boolean = False
    Private _OverrideGiftTypeFilter As Generic.List(Of Web.Content.Common.Enumerations.EInfinityGiftType)
    Private _modalPostBack As Boolean = False
    Private _callbackFromBBPay As Boolean

    Private ReadOnly Property GivingHistory() As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory
        Get
            Dim _returnvalue As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory
            Select Case mTabControl.ActiveViewIndex
                Case 0 : _returnvalue = givingHistoryActiveOnly.History
                Case 1 : _returnvalue = givingHistoryAll.History
            End Select
            Return _returnvalue
        End Get
    End Property

    Private ReadOnly Property ShowFundPicker() As Boolean
        Get
            Dim _returnvalue As Boolean
            Select Case mTabControl.ActiveViewIndex
                Case 0 : _returnvalue = givingHistoryActiveOnly.ShowFundPicker
                Case 1 : _returnvalue = givingHistoryAll.ShowFundPicker
            End Select
            Return _returnvalue
        End Get
    End Property

    Private Property SelectedDateRange() As DateRange
        Get
            If Not _dateRange.HasValue Then
                Dim vsValue = Me.ViewState(VS_SELECTED_DATE_OPTION & ViewStateKey)
                If vsValue IsNot Nothing Then
                    _dateRange = DirectCast(vsValue, DateRange)
                Else
                    _dateRange = DateRange.All
                End If
            End If
            Return _dateRange.Value
        End Get
        Set(ByVal value As DateRange)
            _dateRange = value
            Me.ViewState(VS_SELECTED_DATE_OPTION & ViewStateKey) = value
        End Set
    End Property

    Private Property SelectedStartDate() As Date
        Get
            Return _startDate
        End Get
        Set(ByVal value As Date)
            _startDate = value
        End Set
    End Property

    Private Property SelectedEndDate() As Date
        Get
            Return _endDate
        End Get
        Set(ByVal value As Date)
            _endDate = value
        End Set
    End Property

    Private Property SelectedFund() As String
        Get
            Return _selectedFund
        End Get
        Set(ByVal value As String)
            _selectedFund = value
        End Set
    End Property

    Private ReadOnly Property ViewStateKey() As String
        Get
            Dim _returnvalue As String
            Select Case mTabControl.ActiveViewIndex
                Case 0 : _returnvalue = givingHistoryActiveOnly.ViewStateKey
                Case 1 : _returnvalue = givingHistoryAll.ViewStateKey
            End Select
            Return _returnvalue
        End Get
    End Property

    Private ReadOnly Property ModalPostBack() As Boolean
        Get
            If Page.IsPostBack And Not Page.IsAsync Then
                Dim eventArgument = Request("__eventargument")
                If Not String.IsNullOrEmpty(eventArgument) Then
                    If eventArgument.Contains("UMP_EVENT_SAVE") OrElse eventArgument.Contains("UMP_EVENT_NOSAVE") Then
                        _modalPostBack = True
                    End If
                End If
            End If

            Return _modalPostBack
        End Get
    End Property

    Private ReadOnly Property CallBackFromBBPay() As Boolean
        Get
            Return (Not String.IsNullOrEmpty(Request.QueryString("t"))) AndAlso IsGuid(Request.QueryString("t"))
        End Get
    End Property

    Private ReadOnly Property CallBackToken() As String
        Get
            If CallBackFromBBPay Then Return Request.QueryString("t")
        End Get
    End Property

    Private ReadOnly Property CallBackRecordID() As String
        Get
            If CallBackFromBBPay AndAlso Not String.IsNullOrEmpty(Request.QueryString("r")) AndAlso IsGuid(Request.QueryString("r")) Then
                Return Request.QueryString("r")
            End If
        End Get
    End Property
#End Region

#Region "Language"
    Protected Overrides Sub InitializeLanguageData()
        With LanguageData
            Dim group As String = ""

            group = "Transaction Manager - Tabs"
            .RegisterLanguageField(New FieldInfo(LanguageGuids.ACTIVE_GIFTS_TAB, Nothing, "Active gifts", "Active", group, FieldInfo.ELanguageStringType.Link, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(LanguageGuids.FULL_HISTORY_TAB, Nothing, "All gifts", "History", group, FieldInfo.ELanguageStringType.Link, LocalizeName:=True, LocalizeDefault:=True))

            group = "Transaction Manager - All Filters"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.ALL_FILTERS_APPLY_BUTTON, Nothing, "Apply filter button", "Apply", group, FieldInfo.ELanguageStringType.Button, LocalizeName:=True, LocalizeDefault:=True))

            ' Date Filter
            group = "Transaction Manager - Date Filter"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_LABEL, Nothing, "Date filter label", "Date range:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_ALL, Nothing, "Date filter (All dates)", "All dates", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_1_MONTH, Nothing, "Date filter (From 1 month before)", "One month prior to current date", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_6_MONTHS, Nothing, "Date filter (From 6 months before)", "Six months prior to current date", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_1_YEAR, Nothing, "Date filter (From 1 year before)", "One year prior to current date", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_SPECIFIC, Nothing, "Date filter (Specific range)", "Specific range", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))

            ' Fund Filter
            If PortalSettings.Current.Features.IsInfinity Then
                group = "Transaction Manager - Designation Filter"
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FUND_FILTER_LABEL, Nothing, "Designation filter label", "Designations:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FUND_FILTER_ALL, Nothing, "Designation filter (All designations)", "All designations", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
            Else
                group = "Transaction Manager - Fund Filter"
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FUND_FILTER_LABEL, Nothing, "Fund filter label", "Fund:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FUND_FILTER_ALL, Nothing, "Fund filter (All funds)", "All funds", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
            End If

            'Group Filter
            group = "Transaction Manager - Group Filter"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.GROUP_FILTER_LABEL, Nothing, "Group filter label", "Group by:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))

            ' Accessibility
            group = "Transaction Manager - Accessibility"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.ACCESSIBILITY_GRID_SUMMARY, Nothing, "Grid summary", "This table displays giving history", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))

            'Export
            group = "Transaction Manager - Export"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.EXPORT_TITLE, Nothing, "Export title", "Export", group, FieldInfo.ELanguageStringType.Label, True, True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.EXPORT_CSV_TEXT, Nothing, "Csv link text", "CSV", group, FieldInfo.ELanguageStringType.Link, True, True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.EXPORT_PDF_TEXT, Nothing, "Pdf link text", "PDF", group, FieldInfo.ELanguageStringType.Link, True, True))

            ' Columns
            group = "Transaction Manager - Columns"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_DATE, Nothing, "Date", "Date", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_AMOUNT, Nothing, "Amount", "Amount", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_TYPE, Nothing, "Gift type", "Gift type", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_BALANCE, Nothing, "Balance", "Balance", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_CAMPAIGN_DESCRIPTION, Nothing, "Campaign description", "Campaign description", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_INSTALLMENT_FREQUENCY, Nothing, "Installment frequency", "Installment frequency", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_INSTALLMENT_SCHEDULE, Nothing, "Installment schedule", "Installment schedule", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_NUMBER_OF_INSTALLMENTS, Nothing, "Number of installments", "Number of installments", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_PAY_METHOD, Nothing, "Pay method", "Pay method", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_PENDING, Nothing, "Pending", "Pending", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_RECEIPT_AMOUNT, Nothing, "Receipt amount", "Receipt amount", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_RECEIPT_DATE, Nothing, "Receipt date", "Receipt date", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_RECEIPT_NUMBER, Nothing, "Receipt number", "Receipt number", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_DONORNAME_PUBLICNAME, Nothing, "Donor name", "Donor name", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            If RE7ServiceHelper.InstalledCountry = RE7Service.bbInstalledCountries.CTY_UK Then
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_AID_AMOUNT, Nothing, "Gift aid amount", "Gift aid amount", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            End If
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_ANONYMOUS, Nothing, "Anonymous", "Anonymous", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_SUBTYPE, Nothing, "Gift subtype", "Gift subtype", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_APPEAL_DESCRIPTION, Nothing, "Appeal description", "Appeal", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            If PortalSettings.Current.Features.IsInfinity Then
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_DESIGNATION_LOOKUPID, Nothing, "Designation lookup ID", "Designation lookup ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_DESIGNATION_PUBLICNAME, Nothing, "Designation public name", "Designation", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_LOOKUPD, Nothing, "Gift lookup ID", "Gift lookup ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_CAMPAIGN_LOOKUPID, Nothing, "Campaign lookup ID", "Campaign lookup ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_SITE_HEIRARCHY, Nothing, "Site hierarchy", "Site hierarchy", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_EVENT_NAME, Nothing, "Event name", "Event name", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_EVENT_DETAILS, Nothing, "Event details", "Event details", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_FUNDRAISING_PURPOSE, Nothing, "Fundraising purpose", "Purpose", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            Else
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_FUND_ID, Nothing, "Fund ID", "Fund ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_FUND_DESCRIPTION, Nothing, "Fund description", "Fund description", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_ID, Nothing, "Gift ID", "Gift ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_CAMPAIGN_ID, Nothing, "Campaign ID", "Campaign ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            End If

            ' Item Flags
            group = "Transaction Manager - Item Flags"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_PENDING, Nothing, "Pending gifts", "(Pending)", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            If PortalSettings.Current.Features.IsInfinity Then
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_SOFT, Nothing, "Revenue recognitions", "(Revenue recognition)", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            Else
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_SOFT, Nothing, "Soft credit gifts", "(Soft credit)", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            End If
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_EMPTY_GRID_NOTIFICATION, Nothing, "Empty grid notification", "No data found matching the filter criteria", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_ANONYMOUS, Nothing, "Anonymous gifts", "(Anonymous)", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_ANONYMOUS_DONOR_NAME, Nothing, "Anonymous donor name", "Anonymous donor", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))

            ' Pledge Details
            group = "Transaction Manager - Pledge Details"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_DETAIL_BALANCE, Nothing, "Balance remaining", "Balance remaining:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_DETAIL_TOTALPAID, Nothing, "Total paid", "Total paid:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_DETAIL_AMOUNT, Nothing, "Last installment amount", "Last installment amount:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_DETAIL_DATE, Nothing, "Last payment date", "Last payment date:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_DETAIL_ISEFT, Nothing, "Pledge is automatically paid", "This gift is set up for automatic payments.", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_NEXTINSTALLMENT, Nothing, "Next installment date", "Next installment due:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_LASTINSTALLMENT, Nothing, "Paid in full date", "Paid in full date:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))

            ' Pledge Payment Details
            group = "Transaction Manager - Pledge Payment Details"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_PAYMENT_DETAIL_PLEDGE_DATE, Nothing, "Pledge date", "Pledge date:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_PAYMENT_DETAIL_PLEDGE_AMOUNT, Nothing, "Pledge amount", "Pledge amount:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_PAYMENT_DETAIL_PLEDGE_BALANCE, Nothing, "Pledge balance", "Pledge balance:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))

            'Pledge Payment Link
            group = "Transaction Manager - Pledge Payment Link"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PLEDGE_PAYMENT_LINK, Nothing, "Pledge payment link label", "(Pay)", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))

            'recurring gift details
            If PortalSettings.Current.Features.IsInfinity Then
                group = "Transaction Manager - Recurring Gift Details"
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_CC_EXPIRES, Nothing, "Auto payment CC expires date", "Card expiration date:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_CC_NUMBER, Nothing, "Auto payment CC last 4 digits", "Card number ending in:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_DD_ACCT_DESC, Nothing, "Auto payment debit account", "Financial institution:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_DD_ACCT_NUMBER, Nothing, "Auto payment debit account number last 4 digits", "Account number ending in:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_DD_ACCT_TYPE, Nothing, "Auto payment debit account type", "Account type:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_LAST_PAYMENT_AMT, Nothing, "Last payment amount", "Last payment amount:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_LAST_PAYMENT_DATE, Nothing, "Last payment date", "Last payment date:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            End If

            ' Pager
            group = "Transaction Manager - Pager"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAGER_PAGE_SENTENCE_PIECE, Nothing, """Page"" sentence piece", "Page", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAGER_OF_SENTENCE_PIECE, Nothing, """of"" sentence piece", "of", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAGER_FIRST, Nothing, "First page", "First", group, FieldInfo.ELanguageStringType.Link, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAGER_PREV, Nothing, "Previous page", "Prev", group, FieldInfo.ELanguageStringType.Link, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAGER_NEXT, Nothing, "Next page", "Next", group, FieldInfo.ELanguageStringType.Link, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAGER_LAST, Nothing, "Last page", "Last", group, FieldInfo.ELanguageStringType.Link, LocalizeName:=True, LocalizeDefault:=True))

            ' Summary
            group = "Transaction Manager - Summary"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.SUMMARY_CURRENCY, Nothing, "Currency", "Currency", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.SUMMARY_GIFT_TOTAL, Nothing, "Gift total", "Gift total", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            If RE7ServiceHelper.InstalledCountry = RE7Service.bbInstalledCountries.CTY_UK Then
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.SUMMARY_GIFT_AID_TOTAL, Nothing, "Gift aid total", "Gift aid total", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            End If
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.SUMMARY_PLEDGE_TOTAL, Nothing, "Pledge total", "Pledge total", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.SUMMARY_PENDING_TOTAL, Nothing, "Pending total", "Pending total", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.SUMMARY_BALANCE_TOTAL, Nothing, "Remaining pledge total", "Remaining pledge total", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
            If (PortalSettings.Current.Features.IsInfinity()) Then
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.SUMMARY_SOFTCREDIT_TOTAL, Nothing, "Recognition credit total", "Recognition credit total", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            Else
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.SUMMARY_SOFTCREDIT_TOTAL, Nothing, "Soft credit total", "Soft credit total", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            End If
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.SUMMARY_HARDCREDIT_TOTAL, Nothing, "Donation total", "Donation total", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))

            ' Validation Messages
            group = "Transaction Manager - Validation Messages"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.VALIDATION_DATE_INVALIDSTARTDATE, Nothing, "Invalid start date", "Start date must be a valid date that does not occur in the past", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.VALIDATION_DATE_INVALIDENDDATE, Nothing, "Invalid end date", "Invalid end date", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.VALIDATION_DATE_INVALIDENDBEFORESTART, Nothing, "End date before start date", "The end cannot be before the start date", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))

            If PortalSettings.Current.Features.IsInfinity Then
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_VALIDATION_AMT_GREATER_MIN, Nothing, "Amount less than minimum", "Gift amount must be greater than or equal to ", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_VALIDATION_AMT_NUMBER, Nothing, "Amount invalid number", "Gift amount must be a valid number", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_VALIDATION_INVALID_SCHEDULE, Nothing, "General schedule invalid", "Selected recurrence schedule is invalid or missing information", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_VALIDATION_INVALID_CUSTOM_FREQUENCY, Nothing, "Custom schedule invalid", "Frequency must be selected", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_INVALID_CARD_OPTION, Nothing, "Credit card required", "Credit card is required", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_INVALID_DEBIT_OPTION, Nothing, "Direct debit required", "Payment account is required", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_INVALID_ROUTING_NUM, Nothing, "Routing number invalid", "Routing number is invalid", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_ACCOUNT_HOLDER, Nothing, "Account holder required", "Account holder's name is required", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_ACCOUNT_NUM, Nothing, "Account number required", "Account number is required", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_ACCOUNT_TYPE, Nothing, "Account type required", "Account type is required", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_COUNTRY, Nothing, "Country required", "Country is required", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_FINANCIAL_INST, Nothing, "Financial institution required", "Financial institution is required", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_ROUTING_NUM, Nothing, "Routing number required", "Routing number is required", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_PAYMETHOD, Nothing, "Payment method required", "Payment method is required", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                'bug 88712 add validations to language tab
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_DUPLICATE_ACCOUNT, Nothing, "Duplicate debit account", "This account already exists, please select from payment account list or enter new account information", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_VALIDATION_INVALID_NEXT_TRANSACTION, Nothing, "Invalid next transaction date", "At least one payment toward this recurring gift must occur between the start and end dates", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))

                If RE7ServiceHelper.InstalledCountry = RE7Service.bbInstalledCountries.CTY_UK Then
                    .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_SORTCODE, Nothing, "Sort code required", "Sort code is required", group, FieldInfo.ELanguageStringType.Message))
                End If
            End If



			' Gift Types
			' CSM - temporarily commented out.
			group = "Transaction Manager - Gift Types"
			.RegisterCodeTableLanguageFields(EStaticCodeTables.GiftTypesRemoveUnused, Nothing, group, FieldInfo.ELanguageStringType.Message)

            ' Help Text
            group = "Transaction Manager - Help Text"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.HELPTEXT_FILTER, lblFilterHelpText, "Filter help text", "", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.HELPTEXT_GRID, Nothing, "Grid help text", "", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.HELPTEXT_SUMMARY, Nothing, "Summary help text", "", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(LanguageGuids.HELPTEXT_ACTIVE_GRID, ActiveHistoryHelpTextLabel, "Active gifts grid help text", "", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(LanguageGuids.HELPTEXT_FULL_HISTORY_GRID, FullHistoryHelpTextLabel, "Full history grid help text", "", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.HELPTEXT_PAYMENT_EDITING, Nothing, "Edit payment method help text", "", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.HELPTEXT_DETAIL_EDITING, Nothing, "Edit gift details help text", "", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            If PortalSettings.Current.Features.IsRE7 Then
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.HELPTEXT_TRUNCATE_RECORDS, Nothing, "Too many records help text", _
                                                     "Note: Your giving history includes more than 100 transactions, but the website only displays the 100 most recent. Totals on the website do not include older transactions. For older transactions or a complete giving history, please contact us.", _
                                                     group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
            End If

            If PortalSettings.Current.Features.IsInfinity Then
                'Edit Modules
                'Details
                group = "Transaction Manager - Edit Gift Details"
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_FREQUENCY_LABEL, Nothing, "Frequency", "Frequency:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_GIFT_AMOUNT_LABEL, Nothing, "Gift amount", "Gift amount:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_START_DATE_LABEL, Nothing, "Starting date", "Starting:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_END_DATE_LABEL, Nothing, "Ending date", "Ending:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_TITLE_LABEL, Nothing, "Window title", "Edit gift details", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_USE_SAME_FREQ_OPTION, Nothing, "Use current frequency option", "Use current frequency", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_SAVE_BUTTON_TEXT, Nothing, "Save button text", "Save", group, FieldInfo.ELanguageStringType.Button, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_CANCEL_BUTTON_TEXT, Nothing, "Cancel button text", "Cancel", group, FieldInfo.ELanguageStringType.Button, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_EDIT_BUTTON_TEXT, Nothing, "Edit link text", "Edit gift details", group, FieldInfo.ELanguageStringType.Link, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_SAVING_MESSAGE, Nothing, "Saving message", "Saving...", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))


                'Payment
                group = "Transaction Manager - Edit Payment Method"
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_ACCOUNT_HOLDER_LABEL, Nothing, "Account holder's name", "Account holder's name:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_ACCOUNT_NUMBER_LABEL, Nothing, "Account number", "Account number:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_ACCOUNT_TYPE_LABEL, Nothing, "Account type", "Account type:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_BRANCH_NAME_LABEL, Nothing, "Branch name", "Branch name:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_CANCEL_BUTTON_TEXT, Nothing, "Cancel button text", "Cancel", group, FieldInfo.ELanguageStringType.Button, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_COUNTRY_LABEL, Nothing, "Country", "Country:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_CREDIT_CARD_LABEL, Nothing, "Credit card drop down label", "Available credit cards:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_CREDIT_CARD_OPTION, Nothing, "Credit card option", "Credit card", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_DEBIT_ACCOUNT_LABEL, Nothing, "Payment account drop down label", "Payment accounts:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_DIRECT_DEBIT_OPTION, Nothing, "Direct debit drop down option", "Direct debit", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_NEW_CREDIT_CARD_LABEL, Nothing, "Add new credit card message", "If you wish to add a new credit card, click the following link: ", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_NEW_CREDIT_CARD_OPTION, Nothing, "Add new credit card drop down option", "Pay with new credit card", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_NEW_DIRECT_DEBIT_OPTION, Nothing, "Add new debit account drop down option", "Pay with new direct debit account", group, FieldInfo.ELanguageStringType.DropDownList, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_PAY_METHOD_LABEL, Nothing, "Payment method drop down label", "Payment method:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_SAVE_BUTTON_TEXT, Nothing, "Save button text", "Save", group, FieldInfo.ELanguageStringType.Button, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_TITLE_LABEL, Nothing, "Window title", "Edit payment method", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                If RE7ServiceHelper.InstalledCountry = RE7Service.bbInstalledCountries.CTY_UK Then
                    .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_UK_DEBIT_DATE_LABEL, Nothing, "Date", "Date:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                    .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_UK_ORIGINATOR_LABEL, Nothing, "Service User's ID Number", "Service user's ID number:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                    .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_FINANCIAL_INST_LABEL, Nothing, "Financial institution", "Financial institution:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                    .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_ROUTING_NUMBER_LABEL, Nothing, "Sort code", "Sort code:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                ElseIf RE7ServiceHelper.InstalledCountry = RE7Service.bbInstalledCountries.CTY_NEWZEALAND OrElse RE7ServiceHelper.InstalledCountry = RE7Service.bbInstalledCountries.CTY_AUSTRALIA Then
                    .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_FINANCIAL_INST_LABEL, Nothing, "Financial institution", "Financial institution:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                    .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_ROUTING_NUMBER_LABEL, Nothing, "BSB number", "BSB number:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                Else
                    .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_FINANCIAL_INST_LABEL, Nothing, "Financial institution", "Financial institution:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                    .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_ROUTING_NUMBER_LABEL, Nothing, "Routing number", "Routing number:", group, FieldInfo.ELanguageStringType.Label, LocalizeName:=True, LocalizeDefault:=True))
                End If

                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_UPDATE_CREDIT_CARD_LABEL, Nothing, "Update credit card message", "If you wish to update your credit card information, click the following link: ", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_EDIT_BUTTON_TEXT, Nothing, "Edit link text", "Edit payment method", group, FieldInfo.ELanguageStringType.Link, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_SAVING_MESSAGE, Nothing, "Saving message", "Saving...", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_NEW_CREDIT_CARD_LINK, Nothing, "Add new credit card link", "Add new card", group, FieldInfo.ELanguageStringType.Link, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_UPDATE_CREDIT_CARD_LINK, Nothing, "Update credit card link", "Update card", group, FieldInfo.ELanguageStringType.Link, LocalizeName:=True, LocalizeDefault:=True))
            End If
            group = "Transaction Manager - Subtotal"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.SUBTOTAL_GRID_TEXT, Nothing, "Subtotal grid text", "Total: ", group, FieldInfo.ELanguageStringType.Message, LocalizeName:=True, LocalizeDefault:=True))

            .RegisterChildControl(givingHistoryActiveOnly)
            .RegisterChildControl(givingHistoryAll)
        End With

        MyBase.InitializeLanguageData()
    End Sub

    Private Sub SetUpLanguageStrings()
        lnkHistoryTab.Text = LanguageData.GetLanguageString(LanguageGuids.FULL_HISTORY_TAB, LanguageMetaData.Encoding.HtmlEncoded)
        lnkActiveTab.Text = LanguageData.GetLanguageString(LanguageGuids.ACTIVE_GIFTS_TAB, LanguageMetaData.Encoding.HtmlEncoded)

        Me.btnFilter.Text = LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.ALL_FILTERS_APPLY_BUTTON, LanguageMetaData.Encoding.Raw)
        Me.DatePickerLabel.Text = LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_LABEL, LanguageMetaData.Encoding.Raw)
        Me.FundPickerLabel.Text = LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.FUND_FILTER_LABEL, LanguageMetaData.Encoding.Raw)
        Me.GroupPickerLabel.Text = LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.GROUP_FILTER_LABEL, LanguageMetaData.Encoding.Raw)
        givingHistoryAll.ApplyHelpTextLanguage()
        givingHistoryActiveOnly.ApplyHelpTextLanguage()
    End Sub

    Private Class LanguageGuids
        'tabs
        Public Const FULL_HISTORY_TAB As String = "048e8456-6eed-4e8f-b891-b1c79a4b8bc4"
        Public Const ACTIVE_GIFTS_TAB As String = "0f03bd22-22ae-4cc4-be41-2cec7e0a7afc"

        'tab content help
        Public Const HELPTEXT_ACTIVE_GRID As String = "3e46accb-f17e-42c4-93c2-4602524da101"
        Public Const HELPTEXT_FULL_HISTORY_GRID As String = "fdb7e8a7-a15f-47d7-8c58-8b66619b0745"
    End Class
#End Region

#Region "Public Members"
    'as of right now this property will only affect infinity
    Public Property OverrideGiftTypeFilter() As Generic.List(Of Web.Content.Common.Enumerations.EInfinityGiftType)
        Get
            If _OverrideGiftTypeFilter Is Nothing Then
                _OverrideGiftTypeFilter = New Generic.List(Of Web.Content.Common.Enumerations.EInfinityGiftType)
            End If
            Return _OverrideGiftTypeFilter
        End Get
        Set(ByVal value As Generic.List(Of Web.Content.Common.Enumerations.EInfinityGiftType))
            _OverrideGiftTypeFilter = value
        End Set
    End Property
#End Region

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        givingHistoryActiveOnly.ContentObjectFromDisplayControl = Me.ContentObject
        givingHistoryAll.ContentObjectFromDisplayControl = Me.ContentObject

        givingHistoryActiveOnly.LanguageDataFromDisplayControl = Me.LanguageData
        givingHistoryAll.LanguageDataFromDisplayControl = Me.LanguageData

        givingHistoryActiveOnly.OverrideGiftTypeFilter = Me.OverrideGiftTypeFilter
        givingHistoryAll.OverrideGiftTypeFilter = Me.OverrideGiftTypeFilter

        SetUpLanguageStrings()
    End Sub

    Private Function IsGuid(ByVal s As String) As Boolean
        Dim regGuid As New Regex(Web.Content.Common.RegularExpressions.RegEx_GUID, RegexOptions.Compiled)
        Return regGuid.IsMatch(s)
    End Function

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If BBWebPrincipal.Current.User Is Nothing Then
            Me.Visible = False
        Else
            If Not DesignDisplay AndAlso Not Page.IsPostBack Then
                DataBind()
            End If

            If ModalPostBack Then
                BindViewControl()
            End If
        End If

        'on initial page load set current tab
        If Not Page.IsPostBack Then
            SetActiveTabSelected()
        End If

        'ViM fix for 60848. Don't cache this page as it contains user specific giving history
        Me.SetPageNoCache()
    End Sub

    Public Sub SetActiveTabSelected()
        tabActiveGiftsDiv.Attributes("class") &= " TransactionManagerCurrentTab"
        lnkActiveTab.CssClass &= " TransactionManagerCurrentTabLink"
    End Sub

    Public Function TabSelected() As Boolean
        Return tabActiveGiftsDiv.Attributes("class").IndexOf("TransactionManagerCurrentTab") > -1 Or _
                tabHistoryGiftsDiv.Attributes("class").IndexOf("TransactionManagerCurrentTab") > -1
    End Function

    Public Overrides Sub DataBind()
        MyBase.DataBind()

        Me.BindDatePickers()
        Me.BindFilters()
        Me.BindViewControl()

        'JDL - WI150756 - must cap the number of gifts returned from RE7 due to possible server timeout
        If PortalSettings.Current.Features.IsRE7 AndAlso Me.GivingHistory.TruncateNumberOfRecords Then
            lblTruncatedRecordsHelpText.Visible = True
            Me.lblTruncatedRecordsHelpText.Text = Me.LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.HELPTEXT_TRUNCATE_RECORDS, LanguageMetaData.Encoding.Raw)
        End If
    End Sub

    'ViM This sub contains a good bit of duplicate code. This is undesirable and needs to cleaned up.
    Public Sub BindViewControl()
        Select Case mTabControl.ActiveViewIndex
            Case 0
                With givingHistoryActiveOnly
                    .CallbackRecordID = CallBackRecordID
                    .CallbackToken = CallBackToken
                    .BindData(_filterChanged, _tabChanged)
                End With
            Case 1
                With givingHistoryAll
                    .CallbackRecordID = CallBackRecordID
                    .CallbackToken = CallBackToken
                    .BindData(_filterChanged, _tabChanged)
                End With
        End Select
    End Sub

    Private Sub BindFilters()
        Me.BindDateRangeDropdown()
        Me.BindFundPicker()
        Me.BindGroupPicker()
    End Sub

    'ViM This sub contains a good bit of duplicate code. This is undesirable and needs to cleaned up.
    Public Sub Tab_Changed(ByVal sender As Object, ByVal e As EventArgs) Handles lnkActiveTab.Click, lnkHistoryTab.Click
        Dim lnkSender As LinkButton = DirectCast(sender, LinkButton)
        Dim lnkSenderValue As Integer = -1
        Integer.TryParse(lnkSender.Attributes.Item("value"), lnkSenderValue)
        If lnkSenderValue > -1 Then
            mTabControl.ActiveViewIndex = lnkSenderValue

            Select Case lnkSenderValue
                Case 0
                    tabHistoryGiftsDiv.Attributes("class") = tabHistoryGiftsDiv.Attributes("class").Replace("TransactionManagerCurrentTab", "")
                    lnkHistoryTab.CssClass = lnkHistoryTab.CssClass.Replace("TransactionManagerCurrentTabLink", "")
                    If Not tabActiveGiftsDiv.Attributes("class").Contains("TransactionManagerCurrentTab") Then
                        tabActiveGiftsDiv.Attributes("class") &= " TransactionManagerCurrentTab"
                        lnkActiveTab.CssClass &= " TransactionManagerCurrentTabLink"
                    End If
                Case 1
                    tabActiveGiftsDiv.Attributes("class") = tabActiveGiftsDiv.Attributes("class").Replace("TransactionManagerCurrentTab", "")
                    lnkActiveTab.CssClass = lnkActiveTab.CssClass.Replace("TransactionManagerCurrentTabLink", "")
                    If Not tabHistoryGiftsDiv.Attributes("class").Contains("TransactionManagerCurrentTab") Then
                        tabHistoryGiftsDiv.Attributes("class") &= " TransactionManagerCurrentTab"
                        lnkHistoryTab.CssClass &= " TransactionManagerCurrentTabLink"
                    End If
            End Select
        End If

        _tabChanged = True

        BindViewControl()
        SetFilters(True, lnkSenderValue)
    End Sub

    'ViM This sub contains a good bit of duplicate code. This is undesirable and needs to cleaned up.
    Private Sub SetFilters(ByVal tabchanged As Boolean, Optional ByVal tabid As Integer = -1)
        If tabchanged Then
            Select Case tabid
                Case 0
                    If Me.ShowFundPicker Then Integer.TryParse(givingHistoryActiveOnly.SelectedFundItemIndex, fundPicker.SelectedIndex)
                    Integer.TryParse(givingHistoryActiveOnly.SelectedDateRangeItemIndex, datePicker.SelectedIndex)

                    If DirectCast(datePicker.SelectedIndex, DateRange) = DateRange.Specific Then
                        dpStartDate.Text = givingHistoryActiveOnly.SelectedStartDate.ToShortDateString
                        dpStartDate.Visible = Not String.IsNullOrEmpty(dpStartDate.Text)

                        dpEndDate.Text = givingHistoryActiveOnly.SelectedEndDate.ToShortDateString
                        dpEndDate.Visible = Not String.IsNullOrEmpty(dpEndDate.Text)

                        divSpecificDateRange.Attributes("class") = "TransactionManagerSpecificDateDiv TransactionManagerSpecificDateDivSelected"
                    Else
                        divSpecificDateRange.Attributes("class") = "TransactionManagerSpecificDateDiv TransactionManagerSpecificDateDivNotSelected"
                    End If
                Case 1
                    If Me.ShowFundPicker Then Integer.TryParse(givingHistoryAll.SelectedFundItemIndex, fundPicker.SelectedIndex)
                    Integer.TryParse(givingHistoryAll.SelectedDateRangeItemIndex, datePicker.SelectedIndex)

                    If DirectCast(datePicker.SelectedIndex, DateRange) = DateRange.Specific Then
                        dpStartDate.Text = givingHistoryAll.SelectedStartDate.ToShortDateString
                        dpStartDate.Visible = Not String.IsNullOrEmpty(dpStartDate.Text)

                        dpEndDate.Text = givingHistoryAll.SelectedEndDate.ToShortDateString
                        dpEndDate.Visible = Not String.IsNullOrEmpty(dpEndDate.Text)

                        divSpecificDateRange.Attributes("class") = "TransactionManagerSpecificDateDiv TransactionManagerSpecificDateDivSelected"
                    Else
                        divSpecificDateRange.Attributes("class") = "TransactionManagerSpecificDateDiv TransactionManagerSpecificDateDivNotSelected"
                    End If
            End Select
        Else
            If DirectCast(datePicker.SelectedIndex, DateRange) = DateRange.Specific Then
                divSpecificDateRange.Attributes("class") = "TransactionManagerSpecificDateDiv TransactionManagerSpecificDateDivSelected"
            Else
                divSpecificDateRange.Attributes("class") = "TransactionManagerSpecificDateDiv TransactionManagerSpecificDateDivNotSelected"
            End If
        End If
    End Sub

    Private Sub BindDateRangeDropdown()
        Me.datePicker.Items.FindByValue(DateRange.All.ToString("d")).Text = Me.LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_ALL, LanguageMetaData.Encoding.Raw)
        Me.datePicker.Items.FindByValue(DateRange.Past1Month.ToString("d")).Text = Me.LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_1_MONTH, LanguageMetaData.Encoding.Raw)
        Me.datePicker.Items.FindByValue(DateRange.Past6Months.ToString("d")).Text = Me.LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_6_MONTHS, LanguageMetaData.Encoding.Raw)
        Me.datePicker.Items.FindByValue(DateRange.Past1Year.ToString("d")).Text = Me.LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_1_YEAR, LanguageMetaData.Encoding.Raw)
        Me.datePicker.Items.FindByValue(DateRange.Specific.ToString("d")).Text = Me.LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DATE_FILTER_SPECIFIC, LanguageMetaData.Encoding.Raw)
    End Sub

    Private Sub BindDatePickers()
        dpStartDate.Text = DisplayDate(Blackbaud.Web.Content.Common.App.GetUTCDate()).ToShortDateString
        dpEndDate.Text = DisplayDate(Blackbaud.Web.Content.Common.App.GetUTCDate()).ToShortDateString
    End Sub

    Private Sub BindFundPicker()
        ' TODO: Make sure the field that we use to populate the fund gets updated here
        If Me.ShowFundPicker Then
            Me.fundPicker.Items.Clear()
            Dim allFunds = Me.LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.FUND_FILTER_ALL, LanguageMetaData.Encoding.Raw)
            Me.fundPicker.Items.Add(New ListItem(allFunds, ALL_FUNDS))

            Dim results = From fund In GivingHistory.GivingHistoryData.Funds Where (Not String.IsNullOrEmpty(fund.Description)) Order By fund.Description Ascending Select New With {Key fund.Description} Distinct.ToList

            For Each fund In results
                Dim fundName As String = ""
                If fund.Description.Length > 25 Then
                    fundName = fund.Description.Substring(0, 25) & "..."
                Else
                    fundName = fund.Description
                End If
                Me.fundPicker.Items.Add(New ListItem(fundName, fund.Description))
            Next

            Me.fundPicker.DataBind()
        Else
            Me.fundPickerPanel.Visible = False
        End If
    End Sub

    Private Sub BindGroupPicker()
        With Me.GivingHistory.UsedFields()
            'Columns are public variables not properties, can't do regular
            'databinding
            groupPicker.Items.Clear()
            groupPicker.Items.Add(New ListItem("", "-1"))
            Dim idx = 0
            For Each clm In .Columns
                If clm.ColumnSortType <> Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryColumn.SortType.Currency Then
                    groupPicker.Items.Add(New ListItem(Me.LanguageData.GetLanguageString(clm.FieldLocalizationGuid, LanguageMetaData.Encoding.HtmlEncoded), idx.ToString))
                End If
                idx += 1
            Next
        End With
    End Sub


    Private Sub btnFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFilter.Click


        If Me.ValidateDateRange Then


            'AndrewMc 7/13/2010 Refactored this out into a single common switch rather than
            'switching on each case

            Dim givingHistoryCntrl As GivingHistory2Control
            Select Case mTabControl.ActiveViewIndex
                Case 0
                    givingHistoryCntrl = givingHistoryActiveOnly
                Case 1
                    givingHistoryCntrl = givingHistoryAll
            End Select


            'Date Range Filter
            Me.SelectedDateRange = DirectCast(datePicker.SelectedIndex, DateRange)
            Me.SelectedFund = fundPicker.SelectedValue

            If Me.SelectedDateRange = DateRange.Specific Then
                Me.SelectedStartDate = CDate(dpStartDate.Text)
                Me.SelectedEndDate = CDate(dpEndDate.Text)
            End If

            If Me.SelectedDateRange <> DateRange.All Then
                Dim currentDate = Blackbaud.Web.Content.Common.App.GetUTCDate()
                Dim startDate As System.DateTime = currentDate
                Dim endDate As System.DateTime = currentDate

                Select Case Me.SelectedDateRange
                    Case DateRange.Past1Month
                        startDate = currentDate.AddMonths(-1)
                    Case DateRange.Past6Months
                        startDate = currentDate.AddMonths(-6)
                    Case DateRange.Past1Year
                        startDate = currentDate.AddYears(-1)
                    Case DateRange.Specific
                        startDate = Me.SelectedStartDate
                        endDate = Me.SelectedEndDate
                    Case Else
                        Throw New ArgumentOutOfRangeException("Unrecognized DateRange specified.")
                End Select

                With givingHistoryCntrl
                    .SelectedStartDate = startDate
                    .SelectedEndDate = endDate
                    .SelectedDateRangeItemIndex = datePicker.SelectedIndex.ToString
                    .FilterByDate = True
                End With
            Else
                With givingHistoryCntrl
                    .SelectedDateRangeItemIndex = datePicker.SelectedIndex.ToString
                    .FilterByDate = False
                End With
            End If

            'Fund Filter
            If Me.SelectedFund <> ALL_FUNDS Then
                With givingHistoryCntrl
                    .SelectedFund = Me.SelectedFund
                    .SelectedFundItemIndex = fundPicker.SelectedIndex.ToString
                    .FilterByFund = True
                End With
            Else
                With givingHistoryCntrl
                    .SelectedFundItemIndex = fundPicker.SelectedIndex.ToString
                    .FilterByFund = False
                End With
            End If

            'Group Filter
            With givingHistoryCntrl
                .SelectedGroup = New Nullable(Of Integer)(Integer.Parse(groupPicker.SelectedValue))
                .FilterByGroup = True
            End With

            _filterChanged = True
        Else
            _filterChanged = False
        End If

        BindViewControl()
        SetFilters(False)
    End Sub

    Private Function ValidateDateRange() As Boolean

        Dim bValid As Boolean = True
        If DirectCast([Enum].Parse(GetType(DateRange), Me.datePicker.SelectedValue), DateRange) = DateRange.Specific Then

            Dim startDate As Date = Date.MinValue
            If IsDate(dpStartDate.Text) Then
                startDate = CDate(dpStartDate.Text)
            Else
                bValid = False
                ValidationSummary1.AddMessage(Me.LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.VALIDATION_DATE_INVALIDSTARTDATE, LanguageMetaData.Encoding.Raw))
            End If

            Dim endDate As Date
            If IsDate(dpEndDate.Text) Then
                endDate = CDate(dpEndDate.Text)
            Else
                bValid = False
                ValidationSummary1.AddMessage(Me.LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.VALIDATION_DATE_INVALIDENDDATE, LanguageMetaData.Encoding.Raw))
            End If

            If startDate <> Date.MinValue AndAlso endDate <> Date.MinValue Then
                If startDate > endDate Then
                    bValid = False
                    ValidationSummary1.AddMessage(Me.LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.VALIDATION_DATE_INVALIDENDBEFORESTART, LanguageMetaData.Encoding.Raw))
                End If
            End If
        End If

        Return bValid
    End Function

    Private Sub RenderJavaScript()
        ScriptManager.RegisterClientScriptInclude(Me, Me.GetType, "GivingHistory2", Page.ResolveUrl("~/client/scripts/GivingHistory2.js"))
        ScriptManager.RegisterClientScriptInclude(Me, Me.GetType, "RecurringGiftEditorNamespace", Page.ResolveUrl("~/Client/Scripts/GiftEditorNamespace.js"))
        datePicker.Attributes.Add("onchange", String.Format(Common.ScriptNamespace & ".givingHistory2.SelectSpecificRange('{0}', '{1}', '{2}');", datePicker.ClientID, CInt(DateRange.Specific), divSpecificDateRange.ClientID))
    End Sub

    Private Sub RenderStyleLinks()
        DirectCast(Me.Page, BBPage).Head.AddStyleLink("GivingHistory2Css", Page.ResolveUrl("~/admin/GivingHistory/GivingHistory2.css"))

        If HttpContext.Current.Request.Browser.IsBrowser("ie") Then
            DirectCast(Me.Page, BBPage).Head.AddStyleLink("GivingHistory2IECss", Page.ResolveUrl("~/admin/GivingHistory/GivingHistory2IE.css"))
        Else
            DirectCast(Me.Page, BBPage).Head.AddStyleLink("GivingHistory2OtherCss", Page.ResolveUrl("~/admin/GivingHistory/GivingHistory2Other.css"))
        End If
    End Sub

    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        RenderJavaScript()
        RenderStyleLinks()
    End Sub
End Class
