Imports Blackbaud.Web.Content.Core
Imports Blackbaud.Web.Content.Core.Data
Imports System.Collections.Generic
Imports System.Linq
Imports Blackbaud.AppFx.ContentManagement.FrameworkClassLibrary.ShoppingCart
Imports Blackbaud.Web.Content.Core.Internationalization.Currency
Imports Blackbaud.Web.Content.Common
Imports Blackbaud.Web.Content
Imports Blackbaud.Web
Imports System.Data.SqlClient

Partial Public Class GivingHistory2Control
    Inherits ContentControl

#Region "Constants"
    Const ALL_FUNDS As String = "*"
    Const DUMMYROW As String = "DUMMYROW"
    Const SORT_COMMAND_ARG As String = "GivingHistorySort"
    Const VS_SORT_COLUMN As String = "GivingHistorySortColumn"
    Const VS_SORT_ASC As String = "GivingHistorySortAsc"
    Const VS_SELECTED_START_DATE As String = "GivingHistory2DatePickerStartDate"
    Const VS_SELECTED_END_DATE As String = "GivingHistory2DatePickerEndDate"
    Const VS_SELECTED_FUND As String = "GivingHistory2FundPickerOption"
    Const VS_PLEDGEPAYMENTPAGEINVALID As String = "GivingHistory2PledgePaymentPageInvalid"
    Const VS_FILTER_FUND As String = "GivingHistory2FilterByFund"
    Const VS_FILTER_DATE As String = "GivingHistory2FilterByDate"
    Const VS_SELECTED_FUND_INDEX As String = "GivingHistory2SelectedFundIndex"
    Const VS_SELECTED_DATE_INDEX As String = "GivingHistory2SelectedDateIndex"
    Const VS_ADDITIONALDONATIONPAGEINVALID As String = "GivingHistory2AdditionalDonationPageInvalid"
    Const VS_RECURRINGGIFTPAYMENTPAGEINVALID As String = "GivingHistory2RecurringGiftPaymentPageInvalid"
    Const VS_FILTER_GROUP As String = "GivingHistory2GroupFilter"
    Const VS_SELECTED_GROUP As String = "GivingHistory2SelectedGroup"
    Const VS_ADDITIONALDONATIONMERCHANTACCOUNTID As String = "AdditionalDonationMerchantAccountID"
    Const VS_PLEDGEPAYMENTMERCHANTACCOUNTID As String = "PledgePaymentMerchantAccountID"
    Const VS_RECURRINGGIFTPAYMENTMERCHANTACCOUNTID As String = "RecurringGiftPaymentMerchantAccountID"
    Const VS_EVENTPAYMENTMERCHANTACCOUNTID As String = "EventPaymentMerchantAccountID"
#End Region

#Region "Private Members"
    Private _ghDisplayContentObject As Core.Content
    Private _ghLanguageData As Core.LanguageMetaData
    Private _showMyData As Boolean = False
    Private _sortColumn As Nullable(Of Integer)
    Private _showActiveGiftsOnly As Boolean
    Private _sortAscending As Nullable(Of Boolean)
    Private _startDate As Date = Date.MinValue
    Private _endDate As Date = Date.MinValue
    Private _selectedFund As String
    Private m_bPledgePaymentPageInvalid As Boolean = True
    Private _givingHistory As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory
    Private _filterByDate As Boolean
    Private _filterByFund As Boolean
    Private _OverrideGiftTypeFilter As Generic.List(Of Enumerations.EInfinityGiftType)
    Private _countryDictionary As Generic.Dictionary(Of String, String)
    Private _callbackRecordID As String = String.Empty
    Private _callbackToken As String = String.Empty
    Private _selectedFundItemIndex As String
    Private _selectedDateRangeItemIndex As String
    Private _AdditionalDonationPageInvalid As Boolean = True
    Private _RecurringGiftPaymentPageInValid As Boolean = True
    Private _ddCount As Integer = 0
    Private _ddAmountCell As DataControlFieldCell
    Private _PledgePaymentMerchantAccountID As Guid
    Private _RecurringGiftPaymentMerchantAccountID As Guid
    Private _AdditionalDonationMerchantAccountID As Guid
    Private _EventPaymentMerchantAccountID As Guid
    Private _checkForCurrencySupport As Boolean

    Private ReadOnly Property ApplyFilter() As Boolean
        Get
            Return (FilterByDate OrElse FilterByFund OrElse FilterByGroup)
        End Get
    End Property

    Private Property AdditionalDonationMerchantAccountID() As Guid
        Get
            Dim vsValue = Me.ViewState(VS_ADDITIONALDONATIONMERCHANTACCOUNTID)
            If vsValue IsNot Nothing Then
                _AdditionalDonationMerchantAccountID = New Guid(vsValue.ToString)
            End If
            Return _AdditionalDonationMerchantAccountID
        End Get
        Set(ByVal value As Guid)
            _AdditionalDonationMerchantAccountID = value
            Me.ViewState(VS_ADDITIONALDONATIONMERCHANTACCOUNTID) = value
        End Set
    End Property

    Private Property PledgePaymentMerchantAccountID() As Guid
        Get
            Dim vsValue = Me.ViewState(VS_PLEDGEPAYMENTMERCHANTACCOUNTID)
            If vsValue IsNot Nothing Then
                _PledgePaymentMerchantAccountID = New Guid(vsValue.ToString)
            End If
            Return _PledgePaymentMerchantAccountID
        End Get
        Set(ByVal value As Guid)
            _PledgePaymentMerchantAccountID = value
            Me.ViewState(VS_PLEDGEPAYMENTMERCHANTACCOUNTID) = value
        End Set
    End Property

    Private Property RecurringGiftPaymentMerchantAccountID() As Guid
        Get
            Dim vsValue = Me.ViewState(VS_RECURRINGGIFTPAYMENTMERCHANTACCOUNTID)
            If vsValue IsNot Nothing Then
                _RecurringGiftPaymentMerchantAccountID = New Guid(vsValue.ToString)
            End If
            Return _RecurringGiftPaymentMerchantAccountID
        End Get
        Set(ByVal value As Guid)
            _RecurringGiftPaymentMerchantAccountID = value
            Me.ViewState(VS_RECURRINGGIFTPAYMENTMERCHANTACCOUNTID) = value
        End Set
    End Property

    Private Property EventPaymentMerchantAccountID() As Guid
        Get
            Dim vsValue = Me.ViewState(VS_EVENTPAYMENTMERCHANTACCOUNTID)
            If vsValue IsNot Nothing Then
                _EventPaymentMerchantAccountID = New Guid(vsValue.ToString)
            End If
            Return _EventPaymentMerchantAccountID
        End Get
        Set(ByVal value As Guid)
            _EventPaymentMerchantAccountID = value
            Me.ViewState(VS_EVENTPAYMENTMERCHANTACCOUNTID) = value
        End Set
    End Property

    Private Const FilterChangedKey As String = "TrxMgrFilterChanged"
    Private Property FilterChangedSinceCall() As Boolean
        Get
            Dim obj = BBSession.Item(FilterChangedKey + Me.UniqueID)

            Return If(obj Is Nothing, False, CType(obj, Boolean))
        End Get
        Set(ByVal value As Boolean)
            BBSession.Item(FilterChangedKey + Me.UniqueID) = value
        End Set
    End Property

    Private Enum PaymentType
        Pledge
        AdditionalDonation
        RecurringGift
    End Enum

    Private ReadOnly Property AllowCrossCurrencyPayment() As Boolean
        Get
            Return Settings.AllowCrossCurrencyPayment
        End Get
    End Property
#End Region

#Region "Properties"

    Public ReadOnly Property CallbackFromBBPay() As Boolean
        Get
            Return Not String.IsNullOrEmpty(CallbackRecordID) AndAlso Not String.IsNullOrEmpty(CallbackToken)
        End Get
    End Property

    Public Property CallbackRecordID() As String
        Get
            Return _callbackRecordID
        End Get
        Set(ByVal value As String)
            _callbackRecordID = value
        End Set
    End Property

    Public Property CallbackToken() As String
        Get
            Return _callbackToken
        End Get
        Set(ByVal value As String)
            _callbackToken = value
        End Set
    End Property

    Public Property ContentObjectFromDisplayControl() As Core.Content
        Get
            Return _ghDisplayContentObject
        End Get
        Set(ByVal value As Core.Content)
            _ghDisplayContentObject = value
        End Set
    End Property

    Public Property LanguageDataFromDisplayControl() As Core.LanguageMetaData
        Get
            Return _ghLanguageData
        End Get
        Set(ByVal value As Core.LanguageMetaData)
            _ghLanguageData = value
        End Set
    End Property

    Public Property ShowActiveGiftsOnly() As Boolean
        Get
            Return _showActiveGiftsOnly
        End Get
        Set(ByVal value As Boolean)
            _showActiveGiftsOnly = value
        End Set
    End Property

    Private Property SortColumn() As Nullable(Of Integer)
        Get
            If Not _sortColumn.HasValue Then
                Dim vsValue = Me.ViewState(VS_SORT_COLUMN & ViewStateKey)
                If vsValue IsNot Nothing Then
                    _sortColumn = CInt(vsValue)
                End If
            End If

            Return _sortColumn
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _sortColumn = value
            Me.ViewState(VS_SORT_COLUMN & ViewStateKey) = _sortColumn
        End Set
    End Property

    Private Property SortAscending() As Boolean
        Get
            If Not _sortAscending.HasValue Then
                Dim vsValue = Me.ViewState(VS_SORT_ASC & ViewStateKey)
                If vsValue IsNot Nothing Then
                    _sortAscending = CBool(vsValue)
                Else
                    _sortAscending = True
                End If
            End If
            Return _sortAscending.Value
        End Get
        Set(ByVal value As Boolean)
            _sortAscending = value
            Me.ViewState(VS_SORT_ASC & ViewStateKey) = value
        End Set
    End Property

    Public Property SelectedStartDate() As Date
        Get
            If _startDate = Date.MinValue Then
                Dim vsValue = Me.ViewState(VS_SELECTED_START_DATE & ViewStateKey)
                If vsValue IsNot Nothing Then
                    _startDate = DirectCast(vsValue, Date)
                Else
                    _startDate = Blackbaud.Web.Content.Common.App.GetUTCDate()
                End If
            End If
            Return _startDate
        End Get
        Set(ByVal value As Date)
            _startDate = value
            Me.ViewState(VS_SELECTED_START_DATE & ViewStateKey) = value
        End Set
    End Property

    Public Property SelectedEndDate() As Date
        Get
            If _endDate = Date.MinValue Then
                Dim vsValue = Me.ViewState(VS_SELECTED_END_DATE & ViewStateKey)
                If vsValue IsNot Nothing Then
                    _endDate = DirectCast(vsValue, Date)
                Else
                    _endDate = Blackbaud.Web.Content.Common.App.GetUTCDate()
                End If
            End If
            Return _endDate
        End Get
        Set(ByVal value As Date)
            _endDate = value
            Me.ViewState(VS_SELECTED_END_DATE & ViewStateKey) = value
        End Set
    End Property

    Public Property FilterByDate() As Boolean
        Get
            Dim vsValue = Me.ViewState(VS_FILTER_DATE & ViewStateKey)
            If vsValue IsNot Nothing Then
                _filterByDate = DirectCast(vsValue, Boolean)
            End If
            Return _filterByDate
        End Get
        Set(ByVal value As Boolean)
            _filterByDate = value
            Me.ViewState(VS_FILTER_DATE & ViewStateKey) = value
        End Set
    End Property

    Public Property FilterByFund() As Boolean
        Get
            Dim vsValue = Me.ViewState(VS_FILTER_FUND & ViewStateKey)
            If vsValue IsNot Nothing Then
                _filterByFund = DirectCast(vsValue, Boolean)
            End If
            Return _filterByFund
        End Get
        Set(ByVal value As Boolean)
            _filterByFund = value
            Me.ViewState(VS_FILTER_FUND & ViewStateKey) = value
        End Set
    End Property

    Private _filterGroup As Boolean
    Public Property FilterByGroup() As Boolean
        Get
            Dim vsValue = Me.ViewState(VS_FILTER_GROUP & ViewStateKey)
            If (vsValue IsNot Nothing) Then
                _filterGroup = DirectCast(vsValue, Boolean)
            End If
            Return _filterGroup
        End Get
        Set(ByVal value As Boolean)
            _filterGroup = value
            Me.ViewState(VS_FILTER_GROUP & ViewStateKey) = value
        End Set
    End Property

    Private _selectedGroup As Nullable(Of Integer)
    Public Property SelectedGroup() As Nullable(Of Integer)
        Get
            If _selectedGroup Is Nothing Then
                Dim vsValue = Me.ViewState(VS_SELECTED_GROUP & ViewStateKey)
                If vsValue IsNot Nothing Then
                    _selectedGroup = New Nullable(Of Integer)(DirectCast(vsValue, Integer))
                Else
                    _selectedGroup = New Nullable(Of Integer)()
                End If
            End If
            Return _selectedGroup
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _selectedGroup = value
            If value IsNot Nothing AndAlso value.HasValue Then Me.ViewState(VS_SELECTED_GROUP & ViewStateKey) = value.Value
        End Set
    End Property

    Public Property SelectedFund() As String
        Get
            If String.IsNullOrEmpty(_selectedFund) Then
                Dim vsValue = Me.ViewState(VS_SELECTED_FUND & ViewStateKey)
                If vsValue IsNot Nothing Then
                    _selectedFund = DirectCast(vsValue, String)
                Else
                    _selectedFund = ALL_FUNDS
                End If
            End If
            Return _selectedFund
        End Get
        Set(ByVal value As String)
            _selectedFund = value
            Me.ViewState(VS_SELECTED_FUND & ViewStateKey) = value
        End Set
    End Property

    'ajm - bug 60729
    'always invalid until proven otherwise
    Private Property PledgePaymentPageInValid() As Boolean
        Get
            Dim vsValue = Me.ViewState(VS_PLEDGEPAYMENTPAGEINVALID)
            If vsValue IsNot Nothing Then
                m_bPledgePaymentPageInvalid = CBool(vsValue)
            End If
            Return m_bPledgePaymentPageInvalid
        End Get
        Set(ByVal value As Boolean)
            m_bPledgePaymentPageInvalid = value
            Me.ViewState(VS_PLEDGEPAYMENTPAGEINVALID) = value
        End Set
    End Property

    'always invalid until proven otherwise
    Private Property AdditionalDonationPageInValid() As Boolean
        Get
            Dim vsValue = Me.ViewState(VS_ADDITIONALDONATIONPAGEINVALID)
            If vsValue IsNot Nothing Then
                _AdditionalDonationPageInvalid = CBool(vsValue)
            End If
            Return _AdditionalDonationPageInvalid
        End Get
        Set(ByVal value As Boolean)
            _AdditionalDonationPageInvalid = value
            Me.ViewState(VS_ADDITIONALDONATIONPAGEINVALID) = value
        End Set
    End Property

    Private Property RecurringGiftPaymentPageInValid() As Boolean
        Get
            Dim vsValue = Me.ViewState(VS_RECURRINGGIFTPAYMENTPAGEINVALID)
            If vsValue IsNot Nothing Then
                _RecurringGiftPaymentPageInValid = CBool(vsValue)
            End If
            Return _RecurringGiftPaymentPageInValid
        End Get
        Set(ByVal value As Boolean)
            _RecurringGiftPaymentPageInValid = value
            Me.ViewState(VS_RECURRINGGIFTPAYMENTPAGEINVALID) = value
        End Set
    End Property

    Public ReadOnly Property ViewStateKey() As String
        Get
            If Me.ShowActiveGiftsOnly Then
                Return "Active"
            Else
                Return "History"
            End If
        End Get
    End Property

    Public ReadOnly Property ShowFundPicker() As Boolean
        Get
            Return Me.History.UsedFields().Columns.Exists(Function(c As GivingHistoryColumn) c.TableName = "Gifts" AndAlso c.ColumnName = "FundDescription")
        End Get
    End Property

    'as of right now this property will only affect infinity
    Public Property OverrideGiftTypeFilter As List(Of Enumerations.EInfinityGiftType)
        Get
            If _OverrideGiftTypeFilter Is Nothing Then
                _OverrideGiftTypeFilter = New Generic.List(Of Enumerations.EInfinityGiftType)
            End If
            Return _OverrideGiftTypeFilter
        End Get
        Set(ByVal value As Generic.List(Of Enumerations.EInfinityGiftType))
            _OverrideGiftTypeFilter = value
        End Set
    End Property

    Public Property SelectedFundItemIndex() As String
        Get
            If String.IsNullOrEmpty(_selectedFundItemIndex) Then
                Dim vsValue = Me.ViewState(VS_SELECTED_FUND_INDEX & ViewStateKey)
                If vsValue IsNot Nothing Then
                    _selectedFundItemIndex = DirectCast(vsValue, String)
                End If
            End If
            Return _selectedFundItemIndex
        End Get
        Set(ByVal value As String)
            _selectedFundItemIndex = value
            Me.ViewState(VS_SELECTED_FUND_INDEX & ViewStateKey) = value
        End Set
    End Property

    Public Property SelectedDateRangeItemIndex() As String
        Get
            If String.IsNullOrEmpty(_selectedDateRangeItemIndex) Then
                Dim vsValue = Me.ViewState(VS_SELECTED_DATE_INDEX & ViewStateKey)
                If vsValue IsNot Nothing Then
                    _selectedDateRangeItemIndex = DirectCast(vsValue, String)
                End If
            End If
            Return _selectedDateRangeItemIndex
        End Get
        Set(ByVal value As String)
            _selectedDateRangeItemIndex = value
            Me.ViewState(VS_SELECTED_DATE_INDEX & ViewStateKey) = value
        End Set
    End Property

    Public ReadOnly Property History() As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory
        Get
            If BBWebPrincipal.Current.User Is Nothing Then
                _givingHistory = New Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory()
            Else

                If _givingHistory Is Nothing Then
                    Dim historyLoaders As New List(Of ILoader(Of GivingHistoryFields))

                    If Me.DesignDisplay Then
                        Dim dummyLoader As New Blackbaud.CustomFx.WrappedParts.WebParts.BBNCGivingHistoryDummyLoader()
                        historyLoaders.Add(dummyLoader)
                        _givingHistory = New Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory(historyLoaders)
                        dummyLoader.ParentGivingHistory = _givingHistory
                    Else
                        Dim bbncLoader As New BBNCGivingHistoryPendingLoader()
                        historyLoaders.Add(bbncLoader)

                        ' TODO: refactor this section
                        If PortalSettings.Current.Features.IsInfinity Then
                            Dim sessionManager As New WebSessionManager()
                            Dim infinityLoader As New Blackbaud.CustomFx.WrappedParts.WebParts.InfinityGivingHistoryLoader(sessionManager, Me.ContentObjectFromDisplayControl.ContentID.ToString)
                            historyLoaders.Add(infinityLoader)
                            'sterling loads all payments on unpaid Event Regs from BBNC
                            Dim regPaymentLoader As New Blackbaud.CustomFx.WrappedParts.WebParts.PaymentPendingInBBNCEventLoader
                            historyLoaders.Add(regPaymentLoader)

                            _givingHistory = New Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory(historyLoaders)

                            infinityLoader.ParentGivingHistory = _givingHistory
                            infinityLoader.OverrideGiftTypeFilter = Me.OverrideGiftTypeFilter
                        ElseIf PortalSettings.Current.Features.IsRE7 Then
                            Dim sessionManager As New WebSessionManager()
                            Dim re7loader As New Blackbaud.CustomFx.WrappedParts.WebParts.RE7GivingHistoryLoader(sessionManager, Me.ContentObjectFromDisplayControl.ContentID.ToString)

                            historyLoaders.Add(re7loader)

                            ' Had to put this before assigning the re7loader's ParentGivingHistory prop
                            _givingHistory = New Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory(historyLoaders)

                            re7loader.ParentGivingHistory = _givingHistory
                            re7loader.ConstituentID = BBWebPrincipal.User.ConstituentID
                        End If

                        If _givingHistory Is Nothing Then
                            _givingHistory = New Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory(historyLoaders)
                        End If

                        bbncLoader.ParentGivingHistory = _givingHistory

                        'ViM loader loads all unprocessed pledge payments from BBNC
                        Dim paymentLoader As New PaymentPendingInBBNCPledgeLoader(_givingHistory.IncludePending)
                        historyLoaders.Add(paymentLoader)
                    End If

                    Dim ghOptionsManager = New BBNCGivingHistoryOptionsManager(Me.ContentObjectFromDisplayControl.ContentID)
                    ghOptionsManager.Load(_givingHistory)

                    ' TODO: Refactor all language stuff into a language loader? Probably not since he's accessing his own property to set these.
                    _givingHistory.SoftCreditText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_SOFT, LanguageMetaData.Encoding.HtmlEncoded)
                    _givingHistory.PendingText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_PENDING, LanguageMetaData.Encoding.HtmlEncoded)
                    _givingHistory.AnonymousText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_ANONYMOUS, LanguageMetaData.Encoding.HtmlEncoded)
                    _givingHistory.AnonymousDonorNameText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_ANONYMOUS_DONOR_NAME, LanguageMetaData.Encoding.HtmlEncoded)
                End If
            End If
            Return _givingHistory
        End Get
    End Property

    Private ReadOnly Property Countries() As Generic.Dictionary(Of String, String)
        Get
            If _countryDictionary IsNot Nothing AndAlso _countryDictionary.Count > 0 Then
                Return _countryDictionary
            Else
                If _countryDictionary Is Nothing Then
                    _countryDictionary = New Generic.Dictionary(Of String, String)
                End If

                Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
                Dim countryFilterData As New DataLists.TopLevel.CountryListFilterData
                countryFilterData.INCLUDEINACTIVE = False
                Dim countryList() = DataLists.TopLevel.CountryList.GetRows(provider, countryFilterData)

                For i As Integer = 0 To countryList.Count - 1
                    _countryDictionary.Add(countryList(i).ID.ToString, countryList(i).Country)
                Next
            End If

            Return _countryDictionary
        End Get
    End Property
#End Region

#Region "Language"
    Public Sub ApplyHelpTextLanguage()
        Me.lblGridHelpText.Text = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.HELPTEXT_GRID, LanguageMetaData.Encoding.HtmlEncoded)
        Me.ExportTitleLabel.Text = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.EXPORT_TITLE, LanguageMetaData.Encoding.HtmlEncoded)
        Me.PdfLinkText.Text = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.EXPORT_PDF_TEXT, LanguageMetaData.Encoding.HtmlEncoded)
        Me.CsvLinkText.Text = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.EXPORT_CSV_TEXT, LanguageMetaData.Encoding.HtmlEncoded)
    End Sub
    Private Function ApplyLanguageToList(ByVal query As IEnumerable(Of GivingHistoryDataRow)) As IList(Of GivingHistoryDataRow)

        Dim l = query.ToList()

        For Each item In l
            If item.Gifts__GiftTypeLanguageGUID <> Guid.Empty Then
                item.Gifts__Type = LanguageDataFromDisplayControl.GetLanguageString(item.Gifts__GiftTypeLanguageGUID, LanguageMetaData.Encoding.Raw)
            End If

            If item.Gifts__IsSoftCredit Then
                item.Gifts__Type = String.Concat(item.Gifts__Type, " ", Me.History.SoftCreditText)
                If item.Gifts__Anonymous.Contains(StringLocalizer.BBString("Yes")) OrElse item.Gifts__Anonymous.Contains("Yes") Then
                    item.Gifts__DonorName = Me.History.AnonymousDonorNameText
                End If
            ElseIf item.Gifts__Anonymous.Contains(StringLocalizer.BBString("Yes")) OrElse item.Gifts__Anonymous.Contains("Yes") Then
                item.Gifts__DonorName = String.Concat(item.Gifts__DonorName, " ", Me.History.AnonymousText)
            End If
        Next

        Return l
    End Function
#End Region

#Region "Styled Update Panel for Sorting"

    Private Sub GivingHistoryContainer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles GivingHistoryContainer.Load
        Dim eventArgument = Request("__eventargument")
        If Not String.IsNullOrEmpty(eventArgument) AndAlso eventArgument.StartsWith(SORT_COMMAND_ARG & Me.ClientID) Then
            Dim sortColumn As Integer
            Dim argsSplit() As String
            'ajm - cant assume the client id will only have 1 '_'
            'instead get the last elemnt of the array after the split, the column number should always be last
            argsSplit = eventArgument.Split("_"c)
            Integer.TryParse(argsSplit(argsSplit.Count - 1), sortColumn)
            SetSortOrder(sortColumn)
        End If
    End Sub

#End Region

#Region "Paging"
    Private Sub pager1_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles pager1.Command
        Dim currentPageIndex As Int32 = CType(e.CommandArgument, Int32)
        SetCurrentPageIndex(currentPageIndex)
        BindData(ApplyFilter, False)
    End Sub

    Private Sub SetPager(ByVal rowcount As Integer)
        Me.pager1.PageSize = Me.History.PageSize
        Me.pager1.CurrentIndex = Me.grid.CurrentPageIndex + 1
        Me.pager1.ItemCount = rowcount
        Me.pager1.Visible = True
        SetPagerLanguage()
    End Sub

    Private Sub SetPagerLanguage()
        Me.pager1.PageClause = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAGER_PAGE_SENTENCE_PIECE, LanguageMetaData.Encoding.HtmlEncoded)
        Me.pager1.OfClause = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAGER_OF_SENTENCE_PIECE, LanguageMetaData.Encoding.HtmlEncoded)
        Me.pager1.FirstClause = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAGER_FIRST, LanguageMetaData.Encoding.HtmlEncoded)
        Me.pager1.PreviousClause = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAGER_PREV, LanguageMetaData.Encoding.HtmlEncoded)
        Me.pager1.NextClause = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAGER_NEXT, LanguageMetaData.Encoding.HtmlEncoded)
        Me.pager1.LastClause = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAGER_LAST, LanguageMetaData.Encoding.HtmlEncoded)
    End Sub

    ' Expects a "1-based" index because our pager control uses that base
    ' TODO: maybe put this (and the pager) in the repeater control
    ' Maybe change the pager to be zero-based?
    Private Sub SetCurrentPageIndex(ByVal i As Integer)
        pager1.CurrentIndex = i
        Me.grid.CurrentPageIndex = i - 1
    End Sub
#End Region

#Region "Giving History Summary"

    Private Function SummaryColumnCount() As Integer
        Dim count As Integer = 0

        If Me.History.IncludeTotalsCurrency Then
            count += 1
        End If

        If Me.History.IncludeGiftTotal Then
            count += 1
        End If

        If Me.History.IncludeGiftAidTotal Then
            count += 1
        End If

        If Me.History.IncludePledgeTotal Then
            count += 1
        End If

        If Me.History.IncludePendingTotal Then
            count += 1
        End If

        If Me.History.IncludeBalanceTotal Then
            count += 1
        End If

        If Me.History.IncludeSoftCreditsTotal Then
            count += 1
        End If

        If Me.History.IncludeHardCreditsTotal Then
            count += 1
        End If

        Return count
    End Function

    Private Sub BindGivingHistorySummary(ByVal dataSource As IEnumerable(Of GivingHistoryDataRow))
        If Not Me.History.IncludeSummary Then
            Me.summary1.Visible = False
            Me.lblSummaryHelpText.Visible = False
            Return
        ElseIf SummaryColumnCount() <= 0 Then
            Me.summary1.Visible = False
            Return
        End If

        Dim dataSucces As Boolean = False

        'ViM: This routine has been modified to support multicurrency. The Anonymous type's currency field datatype is String in the if block, and is eCurrencyType in the else block.
        'Shouldn't cause problems since the two blocks call into different functions to format totals.
        If PortalSettings.Current.Features.MulticurrencyConditionSettingExists() Then
            Dim summaryDSMultiCurrency = From ds In dataSource _
                Where Me.History.IncludePendingTotal OrElse Not ds.Pending _
                Group ds By ds.Gifts__CurrencyISO Into Group _
                Order By Group.Sum(Function(x As GivingHistoryDataRow) x.Gifts__AmountUnformatted) _
                Select New With _
                { _
                .Currency = Gifts__CurrencyISO, _
                .GiftTotalUnformatted = Group.Sum(AddressOf GiftAmount), _
                .GiftTotal = FormatCurrency(.GiftTotalUnformatted, .Currency), _
                .PledgeTotalUnformatted = Group.Sum(AddressOf PledgeAmount), _
                .PledgeTotal = FormatCurrency(.PledgeTotalUnformatted, .Currency), _
                .PledgeBalanceTotalUnformatted = Group.Sum(AddressOf PledgeBalanceAmount), _
                .PledgeBalanceTotal = FormatCurrency(.PledgeBalanceTotalUnformatted, .Currency), _
                .GiftAidTotalUnformatted = Group.Sum(AddressOf GiftAidAmount), _
                .GiftAidTotal = FormatCurrency(.GiftAidTotalUnformatted, .Currency), _
                .PendingTotalUnformatted = Group.Sum(AddressOf PendingAmount), _
                .PendingTotal = FormatCurrency(.PendingTotalUnformatted, .Currency), _
                .SoftCreditTotal = FormatCurrency(Group.Sum(Function(f As GivingHistoryDataRow) If(f.Gifts__IsSoftCredit, f.Gifts__AmountUnformatted, Nothing)), .Currency), _
                .HardCreditTotal = FormatCurrency(Group.Sum(Function(f As GivingHistoryDataRow) If(f.Gifts__IsSoftCredit OrElse f.IsPledge, Nothing, f.Gifts__AmountUnformatted)), .Currency) _
                }

            If summaryDSMultiCurrency.Count > 0 Then
                Me.summary1.DataSource = summaryDSMultiCurrency
                dataSucces = True
            End If
        Else
            Dim summaryDSLegacy = From ds In dataSource _
                Where Me.History.IncludePendingTotal OrElse Not ds.Pending _
                Group ds By ds.Gifts__BbncCurrencyType Into Group _
                Order By Group.Sum(Function(x As GivingHistoryDataRow) x.Gifts__AmountUnformatted) _
                Select New With _
                { _
                .Currency = DirectCast(Gifts__BbncCurrencyType, Core.Data.GiftInformation.eCurrencyType), _
                .GiftTotalUnformatted = Group.Sum(AddressOf GiftAmount), _
                .GiftTotal = FormatTotalsField(.GiftTotalUnformatted, .Currency), _
                .PledgeTotalUnformatted = Group.Sum(AddressOf PledgeAmount), _
                .PledgeTotal = FormatTotalsField(.PledgeTotalUnformatted, .Currency), _
                .PledgeBalanceTotalUnformatted = Group.Sum(AddressOf PledgeBalanceAmount), _
                .PledgeBalanceTotal = FormatTotalsField(.PledgeBalanceTotalUnformatted, .Currency), _
                .GiftAidTotalUnformatted = Group.Sum(AddressOf GiftAidAmount), _
                .GiftAidTotal = FormatTotalsField(.GiftAidTotalUnformatted, .Currency), _
                .PendingTotalUnformatted = Group.Sum(AddressOf PendingAmount), _
                .PendingTotal = FormatTotalsField(.PendingTotalUnformatted, .Currency), _
                .SoftCreditTotal = FormatTotalsField(Group.Sum(Function(f As GivingHistoryDataRow) If(f.Gifts__IsSoftCredit, f.Gifts__AmountUnformatted, Nothing)), .Currency), _
                .HardCreditTotal = FormatTotalsField(Group.Sum(Function(f As GivingHistoryDataRow) If(f.Gifts__IsSoftCredit OrElse f.IsPledge, Nothing, f.Gifts__AmountUnformatted)), .Currency) _
                }

            If summaryDSLegacy.Count > 0 Then
                Me.summary1.DataSource = summaryDSLegacy
                dataSucces = True
            End If
        End If

        SetSummaryColumns()

        If Not dataSucces Then
            Dim dummyList As New List(Of String)
            dummyList.Add(String.Empty)

            Me.summary1.DataSource = From d In dummyList Select _
            New With _
              { _
              .Currency = DUMMYROW, _
              .GiftTotal = DUMMYROW, _
              .PledgeTotal = DUMMYROW, _
              .PledgeBalanceTotal = DUMMYROW, _
              .GiftAidTotal = DUMMYROW, _
              .PendingTotal = DUMMYROW, _
              .SoftCreditTotal = DUMMYROW, _
              .HardCreditTotal = DUMMYROW _
              }
        End If

        Me.summary1.DataBind()

        'ViM moving this out of the prerender event handler.
        summary1.Attributes("cellspacing") = "0"
        summary1.HeaderRow.TableSection = TableRowSection.TableHeader
        summary1.Attributes("summary") = String.Empty
    End Sub

    Private Sub SetSummaryColumns()
        ' This will need to change when columns become re-orderable
        Me.summary1.Columns(0).Visible = Me.History.IncludeTotalsCurrency
        Me.summary1.Columns(1).Visible = Me.History.IncludeGiftTotal
        Me.summary1.Columns(2).Visible = ShowGiftAidSummary()
        Me.summary1.Columns(3).Visible = Me.History.IncludePledgeTotal
        Me.summary1.Columns(4).Visible = Me.History.IncludePendingTotal AndAlso Not Me.ShowActiveGiftsOnly
        Me.summary1.Columns(5).Visible = Me.History.IncludeBalanceTotal
        Me.summary1.Columns(6).Visible = Me.History.IncludeSoftCreditsTotal
        Me.summary1.Columns(7).Visible = Me.History.IncludeHardCreditsTotal

        If Me.History.IncludeTotalsCurrency Then
            Me.summary1.Columns(0).HeaderText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.SUMMARY_CURRENCY, LanguageMetaData.Encoding.Raw)
        End If

        If Me.History.IncludeGiftTotal Then
            Me.summary1.Columns(1).HeaderText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.SUMMARY_GIFT_TOTAL, LanguageMetaData.Encoding.Raw)
        End If

        If ShowGiftAidSummary() Then
            Me.summary1.Columns(2).HeaderText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.SUMMARY_GIFT_AID_TOTAL, LanguageMetaData.Encoding.Raw)
        End If

        If Me.History.IncludePledgeTotal Then
            Me.summary1.Columns(3).HeaderText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.SUMMARY_PLEDGE_TOTAL, LanguageMetaData.Encoding.Raw)
        End If

        If Me.History.IncludePendingTotal Then
            Me.summary1.Columns(4).HeaderText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.SUMMARY_PENDING_TOTAL, LanguageMetaData.Encoding.Raw)
        End If

        If Me.History.IncludeBalanceTotal Then
            Me.summary1.Columns(5).HeaderText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.SUMMARY_BALANCE_TOTAL, LanguageMetaData.Encoding.Raw)
        End If

        If Me.History.IncludeSoftCreditsTotal Then
            Me.summary1.Columns(6).HeaderText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.SUMMARY_SOFTCREDIT_TOTAL, LanguageMetaData.Encoding.Raw)
        End If

        If Me.History.IncludeHardCreditsTotal Then
            Me.summary1.Columns(7).HeaderText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.SUMMARY_HARDCREDIT_TOTAL, LanguageMetaData.Encoding.Raw)
        End If
    End Sub

    Private Function ShowGiftAidSummary() As Boolean
        Return (RE7ServiceHelper.InstalledCountry = RE7Service.bbInstalledCountries.CTY_UK AndAlso Me.History.IncludeGiftAidTotal)
    End Function

    Private Sub summary1_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles summary1.DataBound
        If summary1.Rows.Count = 1 Then
            Dim row = summary1.Rows(0)
            Dim bEmptyRow As Boolean = True
            For Each cell As TableCell In row.Cells
                If Not String.IsNullOrEmpty(cell.Text) AndAlso cell.Text <> DUMMYROW Then
                    bEmptyRow = False
                    Exit For
                End If
            Next

            If bEmptyRow Then
                row.Cells.Clear()

                Dim tc As New TableCell
                tc.ColumnSpan = summary1.Columns.Count + 1
                tc.Text = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_EMPTY_GRID_NOTIFICATION, LanguageMetaData.Encoding.HtmlEncoded)
                tc.CssClass = "TransactionManagerGridNotificationCell"
                row.Cells.Add(tc)
            End If
        End If
    End Sub
#End Region

#Region "Helper Routines"
    Private Function FormatTotalsField(ByVal amount As Decimal, ByVal currency As Core.Data.GiftInformation.eCurrencyType) As String
        Return BBFormatCurrency(amount, Core.Common.CurrencySymbol(currency))
    End Function

    Private Function GiftAmount(ByVal row As GivingHistoryDataRow) As Decimal
        If Not (row.IsPledge OrElse row.Pending) Then
            Return TrueDecValue(row.Gifts__AmountUnformatted)
        Else
            Return 0
        End If
    End Function

    Private Function IsPledgePayment(ByVal row As GivingHistoryDataRow) As Boolean
        Dim pledges = AssociatedPledges(row)

        Return (pledges.Count > 0)
    End Function

    Private Function AssociatedPledges(ByVal row As GivingHistoryDataRow) As IEnumerable(Of GivingHistoryFields.GiftsRow)
        Dim ghf = Me.History.GivingHistoryData

        Dim pledges = (From gift In ghf.Gifts _
          Join payment In ghf.Payments _
          On gift.GiftRecordID Equals payment.PaidToID _
          Where payment.GiftRecordID = row.Gifts__GiftRecordID _
          AndAlso Core.Data.GivingHistory.IsPledge(gift) _
          Order By CDate(gift._Date) Descending _
          Select gift)

        Return pledges
    End Function

    Private Function PledgeAmount(ByVal row As GivingHistoryDataRow) As Decimal
        If Not row.Pending AndAlso row.IsPledge Then
            Return TrueDecValue(row.Gifts__AmountUnformatted)
        Else
            Return 0
        End If
    End Function

    ' Need the balance amount because pledge amounts don't take write-offs into consideration.
    Private Function PledgeBalanceAmount(ByVal row As GivingHistoryDataRow) As Decimal
        If Not row.Pending AndAlso row.IsPledge Then
            Return TrueDecValue(row.Gifts__BalanceUnformatted)
        Else
            Return 0
        End If
    End Function

    Private Function GiftAidAmount(ByVal row As GivingHistoryDataRow) As Decimal
        If Not (row.Pending OrElse row.IsPledge) Then
            Return TrueDecValue(row.Gifts__GiftAidAmountUnformatted)
        Else
            Return 0
        End If
    End Function

    Private Function PendingAmount(ByVal row As GivingHistoryDataRow) As Decimal
        If row.Pending Then
            Return TrueDecValue(row.Gifts__AmountUnformatted)
        Else
            Return 0
        End If
    End Function

    Private Shared Function TrueDecValue(ByVal value As Decimal) As Decimal
        If value = Decimal.MinValue Then
            Return 0
        Else
            Return value
        End If
    End Function

    Private Function DisplayedCurrencies(ByVal dataSource As IEnumerable(Of GivingHistoryDataRow)) As IEnumerable(Of Blackbaud.Web.Content.Core.Data.GiftInformation.eCurrencyType)
        Return From ds In dataSource Select DirectCast(ds.Gifts__BbncCurrencyType, Blackbaud.Web.Content.Core.Data.GiftInformation.eCurrencyType) Distinct
    End Function

    Private Function DisplayedGiftTypes(ByVal dataSource As IEnumerable(Of GivingHistoryDataRow)) As IEnumerable(Of String)
        Return From ds In dataSource Order By ds.Gifts__Type Select ds.Gifts__Type Distinct
    End Function
#End Region

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If PortalSettings.Current.EventRegistrantID <> Guid.Empty Then
            PayUnpaidEventRegistration(PortalSettings.Current.EventRegistrantID)
        End If


    End Sub

    Public Sub BindData(ByVal filterchanged As Boolean, ByVal tabchanged As Boolean)
        If BBWebPrincipal.Current.User IsNot Nothing Then
            FilterChangedSinceCall = filterchanged
            PaymentPages_GetMerchantAccount()
            Dim ds = GetRepeaterDataSource(filterchanged, tabchanged)
            Me.BindRepeater(ds)
        End If
    End Sub

    Private Function GetRepeaterDataSource(ByVal filterchanged As Boolean, ByVal tabchanged As Boolean) As IList(Of GivingHistoryDataRow)

        If ViewState(VS_PLEDGEPAYMENTPAGEINVALID) Is Nothing Then
            IsPaymentPageValid(Me.History.PledgePaymentPageID, PaymentType.Pledge)
        End If

        If ViewState(VS_ADDITIONALDONATIONPAGEINVALID) Is Nothing Then
            IsPaymentPageValid(Me.History.RecGiftAdditionalDonationPageID, PaymentType.AdditionalDonation)
        End If

        If ViewState(VS_RECURRINGGIFTPAYMENTPAGEINVALID) Is Nothing Then
            IsPaymentPageValid(Me.History.RecurringGiftPaymentPageID, PaymentType.RecurringGift)
        End If

        Dim appliedQuery = Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory.getGivingHistoryDataSource(Me.History.GivingHistoryData, Me.History.IncludeUnpaidEvents)

        If filterchanged OrElse tabchanged Then
            If FilterByFund Then
                appliedQuery = Me.History.FilterByFund(Me.SelectedFund, appliedQuery)
            End If

            If FilterByDate Then
                appliedQuery = Me.History.FilterByDate(Me.SelectedStartDate, Me.SelectedEndDate, appliedQuery)
            End If

            If tabchanged Then
                SetCurrentPageIndex(1)
            End If
        End If

        If Me.ShowActiveGiftsOnly Then
            appliedQuery = appliedQuery.Where(Function(f) f.Gifts__IsActiveGift)
        End If

        Dim enumeratedQuery As IList(Of GivingHistoryDataRow)
        enumeratedQuery = Me.ApplyLanguageToList(appliedQuery)

        Me.SetDefaultSort()

        If Me.FilterByGroup Then
            If SelectedGroup.HasValue() Then
                If SelectedGroup.Value = -1 Then

                Else
                    Dim groupedQuery = History.Group(SelectedGroup.Value, enumeratedQuery)
                    'Rebuild this list by group

                    Dim ls As New List(Of GivingHistoryDataRow)
                    'TODO This could probably stand being optimized somehow
                    Dim grpCount As Integer = 0
                    For Each grp In groupedQuery
                        If Not Me.grid.GroupHeaders.ContainsKey(grpCount) Then
                            Me.grid.GroupHeaders.Add(grpCount, Server.HtmlEncode(If(grp.Key IsNot Nothing, grp.Key.ToString, " ")))

                            Dim subTotal As Decimal = 0

                            grpCount += grp.Count
                            For i As Integer = 0 To grp.Count - 1
                                subTotal += grp(i).Gifts__AmountUnformatted
                            Next
                            If grpCount = 0 Then
                                Me.grid.SubTotalHeaders.Add(grpCount, String.Format("<b>{0:C}</b>", subTotal))
                            Else
                                Me.grid.SubTotalHeaders.Add(grpCount - 1, String.Format("<b>{0:C}</b>", subTotal))
                            End If
                        End If

                        If SortColumn.HasValue Then
                            ls.AddRange(Me.History.Sort(SortColumn.Value, SortAscending, grp.ToList))
                        Else
                            ls.AddRange(grp.ToList)
                        End If
                    Next
                    enumeratedQuery = ls
                End If

            Else
                'TODO: Non-Column based Groups go here
            End If
        ElseIf SortColumn.HasValue Then
            enumeratedQuery = Me.History.Sort(SortColumn.Value, SortAscending, enumeratedQuery)
        End If

        Return enumeratedQuery
    End Function

    ''' <summary>
    ''' Professional services method written by Chris Whisenhunt.
    ''' This function replaces the columns values of recurring sponsorship gifts.
    ''' </summary>
    Private Sub SwitchDataValues(ByRef dataSource As IList(Of GivingHistoryDataRow))
        Dim newValues As New Dictionary(Of Guid, String)

        Dim dt As New DataTable()

        'get a datatable that has all of the ids and matching childrens
        Using con As New SqlConnection(Settings.ConnectionString)
            Using cmd As New SqlCommand("USR_USP_GETCHILDINFOFORCONSTITUENTID", con)
                cmd.CommandType = CommandType.StoredProcedure

                Dim constit = New Core.Data.ShelbyConstituent(BBWebPrincipal.Current.User)
                Dim constitID = Core.Data.ShelbyConstituent.GetConstituentsGuid(constit.RecordID).ToString

                cmd.Parameters.AddWithValue("@ID", constitID)

                Using dta As New SqlDataAdapter(cmd)
                    cmd.Connection.Open()
                    dta.Fill(dt)
                End Using
            End Using
        End Using

        'create our dictionary of ids and children
        For Each row As DataRow In dt.Rows
            Dim id As New Guid

            'build our designation string
            Dim designation = String.Format("{0} - {1}", row("NAME").ToString(), row("LOOKUPID").ToString())

            If (Guid.TryParse(row("ID").ToString(), id)) Then
                newValues.Add(id, designation)
            End If
        Next

        'loop through the curren datasource and replace the designation with our new text
        For Each item In dataSource
            Dim recordID As New Guid()

            If (Guid.TryParse(item.Gifts__GiftRecordID, recordID)) Then
                'get the current design and lookupid
                Dim desc = item.Gifts__FundraisingPurpose

                'if this is a child designation then it will be in the dictionary
                If (newValues.ContainsKey(recordID)) Then
                    desc = newValues(recordID)
                End If

                'set the new value
                item.Gifts__FundDescription = desc
            End If
        Next
    End Sub

    Private Sub BindRepeater(ByVal dataSource As IList(Of GivingHistoryDataRow))
        SwitchDataValues(dataSource)

        SetRepeaterColumns()
        grid.GridSubtotalText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.SUBTOTAL_GRID_TEXT, LanguageMetaData.Encoding.HtmlEncoded)
        If dataSource.Count = 0 Then
            Dim explanation As New List(Of String)
            explanation.Add(Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.ITEM_FLAG_EMPTY_GRID_NOTIFICATION, LanguageMetaData.Encoding.HtmlEncoded))
            Me.grid.DataSource = explanation
            Me.ExportContainer.Visible = False
        Else
            Me.grid.DataSource = dataSource
            '176005 - briansw - android devices cannot handle the PDF/CSV response and download the file
            Me.ExportContainer.Visible = Not Me.Request.UserAgent.Contains("Android")
        End If

        Me._ddCount = 0

        SetPager(dataSource.Count)
        BindGrid(dataSource.Count)
        BindGivingHistorySummary(dataSource)
    End Sub

    Private Sub BindGrid(ByVal rowcount As Integer)
        Me.grid.ColspanAll = (rowcount = 0)
        Me.grid.AllowPaging = (rowcount > Me.History.PageSize)
        Me.grid.PageSize = Me.History.PageSize
        Me.grid.Summary = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.ACCESSIBILITY_GRID_SUMMARY, LanguageMetaData.Encoding.HtmlEncoded)
        Me.grid.DataBind()
    End Sub

    Private Sub IsPaymentPageValid(ByVal pageid As Integer, ByVal type As PaymentType)

        Dim pageinvalid As Boolean = True
        Try
            If pageid > 0 Then
                Dim pageInfo As SPWrap.spPageInfo.ResultRow = SPWrap.spPageInfo.ExecuteSP(PortalSettings.ConnectionString, PortalSettings.Current.ClientSitesID, pageid, 0).FirstOrDefault
                If pageInfo IsNot Nothing Then pageinvalid = pageInfo.Deleted
            End If
        Catch ex As Exception
            'invalid
        End Try

        Select Case type
            Case PaymentType.Pledge
                Me.PledgePaymentPageInValid = pageinvalid
            Case PaymentType.AdditionalDonation
                Me.AdditionalDonationPageInValid = pageinvalid
            Case PaymentType.RecurringGift
                Me.RecurringGiftPaymentPageInValid = pageinvalid
        End Select
    End Sub

    Private Sub SetSortOrder(ByVal newSortColumn As Integer)
        If Me.SortColumn = newSortColumn Then
            Me.SortAscending = Not Me.SortAscending
        Else
            Me.SortAscending = True
        End If

        Me.SortColumn = newSortColumn
        SetCurrentPageIndex(1)
        BindData(ApplyFilter, False)
    End Sub

    Private Sub SetDefaultSort()
        If Not Me.SortColumn.HasValue Then
            Dim col As Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryColumn

            Dim primarySortColumn = GivingHistoryDataRow.GenerateDataField("Gifts", "Date")

            For i = 0 To Me.History.UsedFields.Columns.Count - 1
                col = Me.History.UsedFields.Columns(i)

                If primarySortColumn = GivingHistoryDataRow.GenerateDataField(col.TableName, col.ColumnName) Then
                    Me.SortColumn = i
                    Me.SortAscending = False
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub SetRepeaterColumns()
        Me.grid.Columns.Clear()

        Dim fields = Me.History.UsedFields()
        Dim col As BoundField

        For Each field As Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryColumn In fields.Columns
            col = New BoundField()
            col.DataField = GivingHistoryDataRow.GenerateDataField(field.TableName, field.ColumnName)
            col.HeaderText = Me.LanguageDataFromDisplayControl.GetLanguageString(field.FieldLocalizationGuid, LanguageMetaData.Encoding.HtmlEncoded)
            Me.grid.Columns.Add(col)
        Next
    End Sub

    Private Sub grid_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles grid.Init
        If Me.ShowActiveGiftsOnly Then
            grid.ViewStateUniqueId = "Active"
        Else
            grid.ViewStateUniqueId = "History"
        End If
    End Sub

    ' TODO: Convert this to add the row instead of remove (need change in repeater "grid")?
    ' TODO: Refactor the way we call queries. They should be passed in with the data item and called if they exist,
    '       they should not all be cased off here (if/elseif/etc).
    Private Sub grid_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles grid.ItemDataBound
        If (e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem) Then
            If TypeOf e.Item.DataItem Is GivingHistoryDataRow Then
                Dim ghdr = DirectCast(e.Item.DataItem, GivingHistoryDataRow)
                Dim mainRow = DirectCast(e.Item.FindControl("mainRow"), TableRow)
                Dim detailsRow = DirectCast(e.Item.FindControl("detailsRow"), TableRow)
                Dim detailsCell = DirectCast(e.Item.FindControl("detailsCell"), TableCell)
                Dim details As System.Text.StringBuilder
                Dim list As HtmlGenericControl = New HtmlGenericControl("ul")
                'Dim payPledge As System.Text.StringBuilder
                Dim customLink As String
                Dim ghf = Me.History.GivingHistoryData

                'building up menu for js
                'list.Attributes.Add("class", String.Concat("accDD", _ddCount))
                list.Attributes.Add("class", "TransactionManagerAccDD")
                list.Attributes.Add("id", String.Concat("TransactionManagerAccDD", _ddCount))
                '_ddCount += 1
                Dim headerItem As HtmlGenericControl = New HtmlGenericControl("li")
                list.Controls.Add(headerItem)

                Dim header As HtmlGenericControl = New HtmlGenericControl("h3")
                header.Attributes.Add("class", "")
                headerItem.Controls.Add(header)
                Dim title As Label = New Label
                title.Text = "+"
                header.Controls.Add(title)
                Dim section As HtmlGenericControl = New HtmlGenericControl("div")
                section.Attributes.Add("class", "acc-section")
                headerItem.Controls.Add(section)
                Dim sectionCon As HtmlGenericControl = New HtmlGenericControl("div")
                sectionCon.Attributes.Add("class", "acc-content")
                section.Controls.Add(sectionCon)

                'need to find the Amount cell for the dropdown
                For Each tc As TableCell In mainRow.Cells
                    If TypeOf tc Is DataControlFieldCell Then
                        Dim ddDCFC As DataControlFieldCell = DirectCast(tc, DataControlFieldCell)
                        If (DirectCast(ddDCFC.ContainingField, BoundField).DataField = GivingHistoryDataRow.GenerateDataField("Gifts", "Amount")) Then
                            _ddAmountCell = ddDCFC
                        End If
                    End If
                Next

                If e.Item.ItemType = ListItemType.Item Then
                    mainRow.CssClass = "TransactionManagerOddRow"
                    detailsRow.CssClass = "TransactionManagerDetailsOddRow"
                Else ' AlternatingItem
                    mainRow.CssClass = "TransactionManagerEvenRow"
                    detailsRow.CssClass = "TransactionManagerDetailsEvenRow"
                End If

                'bug 68685 - ajm - this span should exist on for all rows not just pledge rows. see bug details for notes from design.
                customLink = String.Format("<span class=""{0}""></span>", "TransactionManagerGridCellAmountLink")

                If (Not String.IsNullOrEmpty(ghdr.Gifts__Type) AndAlso ghdr.IsPledge) Then
                    'ViM Add link to donation page for pledge payments
                    'bug 55831 Hide link if pledge has a unprocessed, pending payment
                    If CanShowPledgePaymentLink(ghdr) Then

                        Dim paypledgeURL As String
                        With URLBuilder.GetSitePageLinkBuilder(Me.History.PledgePaymentPageID)
                            .SetQueryValue(PortalSettings.EQueryStringParams.PledgeId, ghdr.Gifts__GiftRecordID)
                            paypledgeURL = .ToString()
                        End With

                        customLink = String.Format("<span class=""{0}""><a href=""{1}"" border=""0"" class=""{2}"">{3}</a></span>", "TransactionManagerGridCellAmountLink", paypledgeURL, "PledgePaymentLink", "(Pay)")
                    End If
                End If

                If Me.ShowActiveGiftsOnly AndAlso Not String.IsNullOrEmpty(ghdr.Event_participations__EventDetails) AndAlso ghdr.Gifts__BalanceUnformatted > 0 Then

                    If CanShowEventPaymentLink(ghdr) Then
                        customLink = String.Empty
                        Dim payeventURL As String
                        With URLBuilder.GetSitePageLinkBuilder(PortalSettings.Current.PageId)
                            .SetQueryValue(PortalSettings.EQueryStringParams.EventRegistrantID, ghdr.Event_participations__EventRegistrantID.ToString)
                            payeventURL = .ToString()
                        End With

                        customLink = String.Format("<span class=""{0}""><a href=""{1}"" class=""{2}"">{3}</a></span>", "TransactionManagerGridCellAmountLink", payeventURL, "PledgePaymentLink", "(Pay)")
                    End If
                End If

                Dim linksDiv As New UI.HtmlControls.HtmlGenericControl("DIV")
                linksDiv.Attributes("class") = "TransactionManagerGridEditGiftWrapper"

                'Donation Form link for making additional donation for sponsorship recurring gifts
                If ghdr.Gifts__IsRecurringGift AndAlso (Not String.IsNullOrEmpty(ghdr.Gifts__Type) AndAlso ghdr.Gifts__Type.ToUpper.Trim.ToUpper = "SPONSORSHIP RECURRING GIFT") Then
                    If CanShowAdditionalDonationLink(ghdr) Then

                        Dim additionalDonationURL As String = String.Empty
                        With URLBuilder.GetSitePageLinkBuilder(Me.History.RecGiftAdditionalDonationPageID)
                            .SetQueryValue(PortalSettings.EQueryStringParams.DonationFundID, ghdr.Funds__FundID)
                            .SetQueryValue(PortalSettings.EQueryStringParams.DonationFundDesc, Web.Content.Common.Encryption.Encryptor.Encrypt(ghdr.Gifts__FundDescription))
                            .SetQueryValue(PortalSettings.EQueryStringParams.IsAdditonalDonation, True)
                            additionalDonationURL = .ToString()
                        End With

                        Dim addtionalDonationLink As String = String.Format("<span class=""{0}""><a href=""{1}"" class=""{2}"">{3}</a></span>", "TransactionManagerGridCellAmountLink", additionalDonationURL, "TransactionManagerGridEditGiftLink", "Make Additional Donation")

                        sectionCon.Controls.Add(New LiteralControl(addtionalDonationLink))
                        Dim htmlBreak As HtmlGenericControl = New HtmlGenericControl("br")
                        sectionCon.Controls.Add(htmlBreak)

                        If _ddAmountCell IsNot Nothing AndAlso Not _ddAmountCell.Controls.Contains(list) Then
                            _ddAmountCell.Controls.Add(list)
                            _ddCount += 1
                        End If
                    End If

                    'Paying recurring gifts online - for now only sponsorship recurring gifts
                    'ViM payment blocked for SoftCredits, MatchingGifts, recurring gifts with EFT turned on. TODO: Add other constraints
                    'ViM 9/16/2010 - When EFT is turned off (remove auto payments in EC), the recurring gift is not pulled into TM
                    If CanShowRecurringPaymentLink(ghdr) Then
                        Dim payrecgiftURL As String
                        With URLBuilder.GetSitePageLinkBuilder(Me.History.RecurringGiftPaymentPageID)
                            .SetQueryValue(PortalSettings.EQueryStringParams.RecurringGiftId, ghdr.Gifts__GiftRecordID)
                            payrecgiftURL = .ToString()
                        End With

                        customLink = String.Format("<span class=""{0}""><a href=""{1}"" class=""{2}"">{3}</a></span>", "TransactionManagerGridCellAmountLink", payrecgiftURL, "PledgePaymentLink", "(Pay)")
                    End If
                End If

                For Each tc As TableCell In mainRow.Cells
                    If TypeOf tc Is DataControlFieldCell Then
                        Dim dcfc As DataControlFieldCell = DirectCast(tc, DataControlFieldCell)
                        If (DirectCast(dcfc.ContainingField, BoundField).DataField = GivingHistoryDataRow.GenerateDataField("Gifts", "ReceiptNumber")) Then
                            Dim lbText As New Label
                            lbText.Text = tc.Text
                            tc.Controls.Add(lbText)

                            If Not String.IsNullOrEmpty(ghdr.Gifts__ReceiptKey) Then
                                Dim lnkPDF As New HyperLink
                                lnkPDF.NavigateUrl = URLBuilder.BuildEReceiptLink(ghdr.Gifts__ReceiptKey)
                                lnkPDF.CssClass = "TransactionManagerReceiptLink"

                                Dim pdfImage As New HtmlImage
                                pdfImage.Alt = "Receipt"
                                pdfImage.Src = Page.ResolveUrl("~/images/pdf.png")
                                pdfImage.Attributes("class") = "TransactionManagerReceiptImage"
                                lnkPDF.Controls.Add(pdfImage)
                                tc.Controls.Add(lnkPDF)
                            End If
                        End If

                        'ViM moved pay pledge link to main row TODO: remove dependency on Amount coiumn.
                        If (DirectCast(dcfc.ContainingField, BoundField).DataField = GivingHistoryDataRow.GenerateDataField("Gifts", "Amount")) Then
                            If Not String.IsNullOrEmpty(customLink) Then
                                Dim lbText As New Label
                                lbText.CssClass = "TransactionManagerGridCellAmount"
                                lbText.Text = tc.Text
                                tc.Controls.Add(lbText)
                                tc.Controls.Add(New LiteralControl(customLink))
                            End If
                        End If
                    End If
                Next

                Dim hasDetails As Boolean = False

                If (Not String.IsNullOrEmpty(ghdr.Gifts__Type) AndAlso ghdr.IsPledge) Then

                    'BUG 64156 - ajm - bad to assume the payment gift is going to be in the 
                    'results for the transaction manager. what if the transaction manager
                    'is set up to only show pledges, the payment will not be available.
                    'moved this payment data to be held in the payments table
                    '' TODO: There is no way to know for certain if 2 gifts on the same day
                    ''       can be determined to have been given in the correct order just by
                    ''       the date and the RE ID does not guarantee order
                    'Dim payments = (From gift In ghf.Gifts _
                    '               Join payment In ghf.Payments _
                    '               On gift.REGiftID Equals payment.REGiftID _
                    '               Where payment.PaidToID = ghdr.Gifts__REGiftID _
                    '               Order By CDate(gift._Date) Descending _
                    '               Select gift).ToList()


                    'ViM fix for bug 76477
                    'GiftRecordID is the backofficeID we should be using
                    Dim payments = ghf.Payments.Where(Function(f) f.PaidToID.ToString = ghdr.Gifts__GiftRecordID.ToString).OrderByDescending(Function(f) CDate(f.PaymentDate))

                    details = New System.Text.StringBuilder("<dl>")
                    details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PLEDGE_DETAIL_BALANCE, LanguageMetaData.Encoding.HtmlEncoded), ghdr.Gifts__Balance)


                    details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PLEDGE_DETAIL_TOTALPAID, LanguageMetaData.Encoding.HtmlEncoded), String.Format("{0:C}", (ghdr.Gifts__AmountUnformatted - ghdr.Gifts__BalanceUnformatted)))
                    If payments.Count > 0 Then
                        Dim mostRecentPayment = payments.Last()
                        details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PLEDGE_DETAIL_AMOUNT, LanguageMetaData.Encoding.HtmlEncoded), mostRecentPayment.PaymentAmount)
                        details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PLEDGE_DETAIL_DATE, LanguageMetaData.Encoding.HtmlEncoded), CDate(mostRecentPayment.PaymentDate).ToShortDateString)
                    End If

                    'WI 206814 RyanBeg: Removed unneccassry code to output the Next Installment Due Date. Changes made to GivingHistory.vb, Line 697, handles this.
                    If ghdr.Gifts__NumberOfInstallments <> "" Then
                        If CInt(ghdr.Gifts__NumberOfInstallments) > 0 Then
                            If ghdr.Gifts__NextInstallmentDates <> "" Then
                                Dim nextInstall As String = ""
                                If CDate(ghdr.Gifts__NextInstallmentDates) < Today Then
                                    nextInstall = "<span class=""TransactionManagerGridInstallmentOverdue"">" & ghdr.Gifts__NextInstallmentDates & "</span>"
                                Else
                                    nextInstall = ghdr.Gifts__NextInstallmentDates
                                End If
                                If nextInstall <> "" Then
                                    details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PLEDGE_NEXTINSTALLMENT, LanguageMetaData.Encoding.HtmlEncoded), nextInstall)
                                End If
                            End If

                            Dim tmpDate As String = ""
                            Try
                                tmpDate = CDate(ghdr.Gifts__LastInstallmentDate).ToShortDateString()
                            Catch
                            End Try

                            details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PLEDGE_LASTINSTALLMENT, LanguageMetaData.Encoding.HtmlEncoded), tmpDate)
                        End If
                    End If

                    If ghdr.Gifts__BalanceUnformatted > 0 AndAlso ghdr.Gifts__IsEFT Then
                        details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel""></dt><dd class=""TransactionManagerGridDetailValue"">{0}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PLEDGE_DETAIL_ISEFT, LanguageMetaData.Encoding.HtmlEncoded))
                    End If

                    details.Append("</dl>")
                    detailsCell.Controls.Add(New LiteralControl(details.ToString()))
                    hasDetails = True
                ElseIf (PortalSettings.Current.Features.IsInfinity AndAlso Not String.IsNullOrEmpty(ghdr.Gifts__Type) AndAlso ghdr.Gifts__IsRecurringGift) Then
                    details = New System.Text.StringBuilder("")
                    If Not String.IsNullOrEmpty(ghdr.Gifts__AutoPayCCPartialNumber) Then
                        details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_CC_NUMBER, LanguageMetaData.Encoding.HtmlEncoded), ghdr.Gifts__AutoPayCCPartialNumber)
                        details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_CC_EXPIRES, LanguageMetaData.Encoding.HtmlEncoded), ghdr.Gifts__AutoPayCCExpirationDate)
                        hasDetails = True
                    ElseIf Not String.IsNullOrEmpty(ghdr.Gifts__AutoPayDDAcctType) Then
                        details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_DD_ACCT_TYPE, LanguageMetaData.Encoding.HtmlEncoded), ghdr.Gifts__AutoPayDDAcctType)
                        details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_DD_ACCT_NUMBER, LanguageMetaData.Encoding.HtmlEncoded), ghdr.Gifts__AutoPayDDAcctPartialNumber)
                        details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_DD_ACCT_DESC, LanguageMetaData.Encoding.HtmlEncoded), System.Web.HttpUtility.HtmlEncode(ghdr.Gifts__AutoPayDDAcctDescription))
                        hasDetails = True
                    End If

                    If Not String.IsNullOrEmpty(ghdr.LastRecurringPayment_Amount) Then
                        details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_LAST_PAYMENT_AMT, LanguageMetaData.Encoding.HtmlEncoded), ghdr.LastRecurringPayment_Amount)
                        details.AppendFormat("<div class=""TransactionManagerGridDetailSet""><dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd></div>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.RECURRING_GIFT_DETAIL_LAST_PAYMENT_DATE, LanguageMetaData.Encoding.HtmlEncoded), ghdr.LastRecurringPayment_Date)
                        hasDetails = True
                    End If

                    If hasDetails Then
                        details.Insert(0, "<dl>")
                        details.Append("</dl></br>")
                        detailsCell.Controls.Add(New LiteralControl(details.ToString()))
                    End If

                    If ghdr.Gifts__IsEditableRecurringGift Then

                        If Me.History.RecurringGiftEditAllowAmtUpdates OrElse Me.History.RecurringGiftEditAllowFreqUpdates Then
                            Dim linkEditRecurringGift As Blackbaud.AppFx.CMS.FCL.GiftEditor.RecurringGiftEditDisplay = DirectCast(Page.LoadControl("~/admin/GivingHistory/RecurringGiftEditDisplay.ascx"), Blackbaud.AppFx.CMS.FCL.GiftEditor.RecurringGiftEditDisplay)
                            With linkEditRecurringGift
                                .ControlID = String.Concat(ghdr.Gifts__GiftRecordID.Replace("-"c, "_"c), "_link_editgift")
                                .RecurringGiftID = ghdr.Gifts__GiftRecordID
                                '.GiftAmount = Format(ghdr.Gifts__AmountUnformatted, "0.00")
                                .GiftAmount = FormatMoney(ghdr.Gifts__AmountUnformatted, GetCultureInfoFromCurrencyISO(ghdr.Gifts__CurrencyISO))
                                .GiftStartDate = ghdr.RecurringGiftStartDate
                                .GiftEndDate = ghdr.RecurringGiftEndDate
                                .GiftFrequency = ghdr.Gifts__InstallmentFrequency
                                .MinGiftAmount = Me.History.RecurringGiftEditAmtUpdateMinAmt
                                .AllowAmtUpdate = Me.History.RecurringGiftEditAllowAmtUpdates
                                .AllowFreqUpdate = Me.History.RecurringGiftEditAllowFreqUpdates
                                .SpecificFrequencyOptions = Me.History.RecurrenceFrequencyDataSource
                                .AllowFreqStartDateUpdate = Me.History.RecGftEdtFreqIncludeRecurSchdStartDate
                                .AllowFreqEndDateUpdate = Me.History.RecGftEdtFreqIncludeRecurSchdEndDate
                                .UseGeneralRecurrence = Me.History.RecurringGiftEditFreqUseGeneralRecurrence
                                .GiftNextTransactionDate = If(Not String.IsNullOrEmpty(ghdr.Gifts__RecurringGiftNextTransactionDate), CDate(ghdr.Gifts__RecurringGiftNextTransactionDate), CDate(ghdr.RecurringGiftStartDate))
                                .GiftFrequencyCode = ghdr.Gifts__FrequencyCode
                                'bug 88640 add currency symbol in front of amount field
                                .CurrencySymbol = Core.CurrencySymbol(Core.MerchantAccountBBPS.BLL.MATransactionInfo.InstalledCurrencyType())
                                .CurrencyISO = ghdr.Gifts__CurrencyISO

                                'CSS
                                .EditLinkCSS = "TransactionManagerGridEditGiftLink"
                                .ServerControlCSS = "TransactionManagerGridEditGiftDiv"
                                .ModalEditFormAmountDivCSS &= "TransactionManagerEditFormAmount"
                                .ModalEditFormBodyCSS &= "TransactionManagerEditFormBody"
                                .ModalEditFormButtonPaneCSS &= "TransactionManagerEditFormButtonPane"
                                .ModalEditFormCSS &= "TransactionManagerEditForm"
                                .ModalEditFormDatePickersContainerCSS &= "TransactionManagerEditFormDateContainer"
                                .ModalEditFormEndDateContainerCSS &= "TransactionManagerEditFormEndDate"
                                .ModalEditFormFieldCaptionCSS &= "TransactionManagerEditFormFieldCaption"
                                .ModalEditFormHeaderCSS &= "TransactionManagerEditFormHeader"
                                .ModalEditFormHeaderLabelCSS &= "TransactionManagerEditFormHeaderLabel"
                                .ModalEditFormHelpTextCSS &= "TransactionManagerEditFormHelpText"
                                .ModalEditFormRecurrenceScheduleContainerCSS &= "TransactionManagerRecurrenceSchedule"
                                .ModalEditFormRecurrenceScheduleRadioButtonListContainerCSS &= "TransactionManagerRecurrenceScheduleRadioButtonListContainer"
                                .ModalEditFormRecurrenceScheduleRadioGroupingCSS &= "TransactionManagerRecurrenceScheduleRadioGrouping"
                                .ModalEditFormRecurrenceScheduleRadioSelectedCSS &= "TransactionManagerRecurrenceScheduleRadioSelected"
                                .ModalEditFormRecurrenceScheduleSelectedAreaCSS &= "TransactionManagerRecurrenceScheduleSelectedArea"
                                .ModalEditFormRecurrenceScheduleSelectedAreaInnerCSS &= "TransactionManagerRecurrenceScheduleSelectedAreaInner"
                                .ModalEditFormStartDateContainerCSS &= "TransactionManagerEditFormStartDate"
                                .ModalEditFormSubmitButtonCSS &= "TransactionManagerEditFormSubmitButton"
                                .ModalEditFormTextboxCSS &= "TransactionManagerEditFormTextbox"
                                .ModalEditFormRecurrenceScheduleRadioContainerCSS &= "TransactionManagerRecurrenceScheduleRadioContainer"
                                .ModalEditFormRecurrenceScheduleRadioCSS &= "TransactionManagerRecurrenceScheduleRadioButtonSelected"
                                .ModalEditFormHelpDivCSS &= "TransactionManagerHelpTextDiv"

                                'language
                                .CancelButtonText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_CANCEL_BUTTON_TEXT, LanguageMetaData.Encoding.HtmlEncoded)
                                .EditButtonText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_EDIT_BUTTON_TEXT, LanguageMetaData.Encoding.HtmlEncoded)
                                .EndingDateText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_END_DATE_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .FrequencyText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_FREQUENCY_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .GiftAmountText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_GIFT_AMOUNT_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .SaveButtonText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_SAVE_BUTTON_TEXT, LanguageMetaData.Encoding.HtmlEncoded)
                                .SavingText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_SAVING_MESSAGE, LanguageMetaData.Encoding.HtmlEncoded)
                                .StartingDateText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_START_DATE_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .TitleText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_TITLE_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .UseCurrentScheduleText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_USE_SAME_FREQ_OPTION, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationAmountGreaterMinText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_VALIDATION_AMT_GREATER_MIN, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationAmountInvalidNumberText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_VALIDATION_AMT_NUMBER, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationInvalidEndDateText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.VALIDATION_DATE_INVALIDENDDATE, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationInvalidGeneralScheduleText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_VALIDATION_INVALID_SCHEDULE, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationInvalidStartDateText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.VALIDATION_DATE_INVALIDSTARTDATE, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationEndAfterStartDateText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.VALIDATION_DATE_INVALIDENDBEFORESTART, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationInvalidCustomFrequencyText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_VALIDATION_INVALID_CUSTOM_FREQUENCY, LanguageMetaData.Encoding.HtmlEncoded)
                                .HelpText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.HELPTEXT_DETAIL_EDITING, LanguageMetaData.Encoding.HtmlEncoded)
                            End With

                            AddHandler linkEditRecurringGift.EditRecurringGiftDetails_Save, AddressOf LinkEditRecurringGift_Save_Handler

                            sectionCon.Controls.Add(linkEditRecurringGift)
                            Dim htmlBreak As HtmlGenericControl = New HtmlGenericControl("br")
                            sectionCon.Controls.Add(htmlBreak)

                            If _ddAmountCell IsNot Nothing AndAlso Not _ddAmountCell.Controls.Contains(list) Then
                                _ddAmountCell.Controls.Add(list)
                                _ddCount += 1
                            End If
                        End If

                        If Me.History.RecurringGiftEditAllowPmntTypeUpdates Then
                            Dim linkEditPayment As Blackbaud.AppFx.CMS.FCL.GiftEditor.PaymentEditDisplay = DirectCast(Page.LoadControl("~/admin/GivingHistory/PaymentEditDisplay.ascx"), Blackbaud.AppFx.CMS.FCL.GiftEditor.PaymentEditDisplay)
                            linkEditPayment.ControlID = String.Concat(ghdr.Gifts__GiftRecordID.Replace("-"c, "_"c), "_link_editpayment")
                            linkEditPayment.RecurringGiftID = ghdr.Gifts__GiftRecordID
                            linkEditPayment.PaymentAccountID = ghdr.Gifts__AutoPayDDAcctID
                            linkEditPayment.CardToken = ghdr.Gifts__AutoPayCCToken
                            linkEditPayment.Countries = Countries
                            linkEditPayment.InstalledCountry = DirectCast(CInt(RE7ServiceHelper.InstalledCountry), AppFx.CMS.FCL.GiftEditor.EditDisplayHelper.eInstalledCountry)

                            Select Case ghdr.Gifts__PayMethod.ToUpper
                                Case "CREDIT CARD"
                                    linkEditPayment.PaymentMethod = AppFx.ContentManagement.FrameworkClassLibrary.ShoppingCart.BaseClassLibrary.Cart.EPaymentMethod.CreditCard
                                Case "DIRECT DEBIT"
                                    linkEditPayment.PaymentMethod = AppFx.ContentManagement.FrameworkClassLibrary.ShoppingCart.BaseClassLibrary.Cart.EPaymentMethod.DirectDebit
                            End Select

                            If ghf.Constituents_PaymentAccounts IsNot Nothing Then
                                For Each ar As GivingHistoryFields.Constituents_PaymentAccountsRow In ghf.Constituents_PaymentAccounts
                                    Select Case ar.PaymentType
                                        Case GivingHistoryCommon.ConstituentPaymentAccountTypes.DirectDebit : linkEditPayment.DirectDebitAccounts.Add(ar.ID, ar.AccountDescription)
                                        Case GivingHistoryCommon.ConstituentPaymentAccountTypes.CreditCard : linkEditPayment.CreditCardAccounts.Add(ar.ID, ar.AccountDescription)
                                    End Select
                                Next
                            End If

                            With linkEditPayment
                                'css
                                .EditLinkCSS = "TransactionManagerGridEditGiftLink"
                                .ServerControlCSS = "TransactionManagerGridEditGiftDiv"
                                .EditLinkCSS = "TransactionManagerGridEditGiftLink"
                                .ServerControlCSS = "TransactionManagerGridEditGiftDiv"
                                .ModalEditFormBodyCSS &= "TransactionManagerEditFormBody"
                                .ModalEditFormButtonPaneCSS &= "TransactionManagerEditFormButtonPane"
                                .ModalEditFormCSS &= "TransactionManagerEditForm"
                                .ModalEditFormFieldCaptionCSS &= "TransactionManagerEditFormFieldCaption"
                                .ModalEditFormHeaderCSS &= "TransactionManagerEditFormHeader"
                                .ModalEditFormHeaderLabelCSS &= "TransactionManagerEditFormHeaderLabel"
                                .ModalEditFormHelpTextCSS &= "TransactionManagerEditFormHelpText"
                                .ModalEditFormSubmitButtonCSS &= "TransactionManagerEditFormSubmitButton"
                                .ModalEditFormTextboxCSS &= "TransactionManagerEditFormTextbox"
                                .ModalEditFormDirectDebitFieldsCSS &= "TransactionManagerEditFormDirectDebitFields"
                                .ModalEditFormCreditCardLinkCSS &= "TransactionManagerEditFormCreditCardLink"
                                .ModalEditFormHelpDivCSS &= "TransactionManagerHelpTextDiv"

                                'language
                                .AccountHolderText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_ACCOUNT_HOLDER_LABEL, LanguageMetaData.Encoding.Raw)
                                .AccountNumberText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_ACCOUNT_NUMBER_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .AccountTypeText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_ACCOUNT_TYPE_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .BranchNameText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_BRANCH_NAME_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .CancelButtonText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_CANCEL_BUTTON_TEXT, LanguageMetaData.Encoding.HtmlEncoded)
                                .CountryText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_COUNTRY_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .CreditCardText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_CREDIT_CARD_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .CreditCardDropOptionText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_CREDIT_CARD_OPTION, LanguageMetaData.Encoding.Raw)
                                .DebitAccountText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_DEBIT_ACCOUNT_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .DirectDebitDropOptionText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_DIRECT_DEBIT_OPTION, LanguageMetaData.Encoding.Raw)
                                .EditButtonText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_EDIT_BUTTON_TEXT, LanguageMetaData.Encoding.HtmlEncoded)
                                .FinancialInstitutionText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_FINANCIAL_INST_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .PayWithNewCardText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_NEW_CREDIT_CARD_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .PayWithNewCardDropOptionText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_NEW_CREDIT_CARD_OPTION, LanguageMetaData.Encoding.Raw)
                                .PayWithNewDebitAccountDropOptionText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_NEW_DIRECT_DEBIT_OPTION, LanguageMetaData.Encoding.Raw)
                                .PaymentMethodText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_PAY_METHOD_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .RoutingNumberText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_ROUTING_NUMBER_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .SaveButtonText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_SAVE_BUTTON_TEXT, LanguageMetaData.Encoding.HtmlEncoded)
                                .SavingText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_SAVING_MESSAGE, LanguageMetaData.Encoding.HtmlEncoded)
                                .TitleText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_TITLE_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .UK_DebitDateText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_UK_DEBIT_DATE_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .UK_OriginatorIdText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_UK_ORIGINATOR_LABEL, LanguageMetaData.Encoding.Raw)
                                .UpdateCreditCardText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_UPDATE_CREDIT_CARD_LABEL, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationInvalidCardOptionText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_INVALID_CARD_OPTION, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationInvalidDebitOptionText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_INVALID_DEBIT_OPTION, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationRoutingNumberInvalidText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_INVALID_ROUTING_NUM, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationAccountHolderRequiredText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_ACCOUNT_HOLDER, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationAccountNumberRequiredText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_ACCOUNT_NUM, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationAccountTypeRequiredText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_ACCOUNT_TYPE, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationCountryRequiredText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_COUNTRY, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationFinancialInstitutionRequiredText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_FINANCIAL_INST, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationRoutingNumberRequiredText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_ROUTING_NUM, LanguageMetaData.Encoding.HtmlEncoded)
                                .UpdateCreditCardLinkText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_UPDATE_CREDIT_CARD_LINK, LanguageMetaData.Encoding.HtmlEncoded)
                                .PayWithNewCreditCardLinkText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_NEW_CREDIT_CARD_LINK, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationSortCodeRequiredText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_SORTCODE, LanguageMetaData.Encoding.HtmlEncoded)
                                .HelpText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.HELPTEXT_PAYMENT_EDITING, LanguageMetaData.Encoding.HtmlEncoded)
                                .ValidationPaymentMethodRequiredText = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_REQUIRED_PAYMETHOD, LanguageMetaData.Encoding.HtmlEncoded)
                            End With

                            If Me.CallbackFromBBPay AndAlso (String.Compare(Me.CallbackRecordID, linkEditPayment.RecurringGiftID) = 0) Then
                                linkEditPayment.CallbackToken = Me.CallbackToken
                            Else
                                linkEditPayment.CallbackToken = String.Empty
                            End If

                            AddHandler linkEditPayment.EditRecurringGiftPaymentDetails_Save, AddressOf LinkEditPayment_Save_Handler


                            sectionCon.Controls.Add(linkEditPayment)
                            Dim htmlBreak As HtmlGenericControl = New HtmlGenericControl("br")
                            sectionCon.Controls.Add(htmlBreak)



                            If _ddAmountCell IsNot Nothing AndAlso Not _ddAmountCell.Controls.Contains(list) Then
                                _ddAmountCell.Controls.Add(list)
                                _ddCount += 1
                            End If

                        End If

                        If linksDiv.Controls.Count > 0 Then
                            detailsCell.Controls.Add(linksDiv)
                            hasDetails = True
                        End If
                    End If
                Else
                    ' Should only return 0 or 1 gift records
                    Dim pledges = AssociatedPledges(ghdr)

                    If pledges.Count > 0 Then
                        Dim associatedPledge = pledges.First()

                        details = New System.Text.StringBuilder("<dl>")
                        details.AppendFormat("<dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PLEDGE_PAYMENT_DETAIL_PLEDGE_DATE, LanguageMetaData.Encoding.HtmlEncoded), CDate(associatedPledge._Date).ToShortDateString)
                        details.AppendFormat("<dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PLEDGE_PAYMENT_DETAIL_PLEDGE_AMOUNT, LanguageMetaData.Encoding.HtmlEncoded), associatedPledge.Amount)
                        details.AppendFormat("<dt class=""TransactionManagerGridDetailLabel"">{0}</dt><dd class=""TransactionManagerGridDetailValue"">{1}</dd>", Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PLEDGE_PAYMENT_DETAIL_PLEDGE_BALANCE, LanguageMetaData.Encoding.HtmlEncoded), associatedPledge.Balance)
                        details.Append("</dl>")

                        detailsCell.Controls.Add(New LiteralControl(details.ToString()))

                        hasDetails = True
                    Else
                        e.Item.Controls.Remove(detailsRow)
                    End If
                End If

                If hasDetails Then

                    If Not Me.DesignDisplay Then
                        'Lancemo-092010- Changed this attribute to each cell instead of the whole row.  
                        'The last cell needs not have the onclick to keep the row from expanding when clicking actionable links
                        'mainRow.Attributes("onclick") = String.Format("{0}.givingHistory2.togglePanel('{1}', '{2}');", Core.Common.ScriptNamespace, mainRow.ClientID, detailsRow.ClientID)
                        For Each tc As TableCell In mainRow.Cells
                            tc.Attributes("onclick") = String.Format("{0}.givingHistory2.togglePanel('{1}', '{2}');", Core.Common.ScriptNamespace, mainRow.ClientID, detailsRow.ClientID)


                        Next
                        If _ddAmountCell IsNot Nothing Then
                            _ddAmountCell.Attributes("onclick") = String.Empty
                            _ddAmountCell.CssClass += " DropDownCorrection"
                        End If
                    End If

                    Dim expandCollapseClass As String = " Collapsed"
                    Dim imageGif As String = "~/images/plus.gif"

                    'active tab gifts are expanded by default
                    If Me.ShowActiveGiftsOnly Then
                        expandCollapseClass = " Expanded"
                        imageGif = "~/images/minus.gif"
                    End If

                    mainRow.CssClass += " ExpandCollapse"
                    detailsRow.CssClass += expandCollapseClass

                    'Wrapping the toggle icon with a link to add it to the tabbing order (bug 71285)
                    Dim tmpLit As LiteralControl = New LiteralControl()
                    tmpLit.Text = String.Format("<a href=""#"" onclick=""return false;""><img src=""{0}"" alt=""Toggle revenue detail information"" border=0 /></a>", Me.Page.ResolveUrl(imageGif))
                    mainRow.Cells(0).Controls.Add(tmpLit)
                End If
            ElseIf TypeOf e.Item.DataItem Is String Then
                Dim row = DirectCast(e.Item.Controls(0), TableRow)
                row.Cells(0).Text = e.Item.DataItem.ToString()
            End If
        ElseIf e.Item.ItemType = ListItemType.Header Then

            Dim mainRow = DirectCast(e.Item.Controls(1), TableRow)

            For i = 1 To mainRow.Cells.Count - 1
                Dim columnIndex As Integer = i - 1
                Dim cell = mainRow.Cells(i)

                Dim sortCommand As String = Me.Page.ClientScript.GetPostBackEventReference(Me.GivingHistoryContainer, String.Concat(SORT_COMMAND_ARG & Me.ClientID, "_", columnIndex))
                cell.Attributes.Add("onclick", sortCommand)

                If SortColumn.HasValue AndAlso SortColumn.Value = columnIndex Then
                    If SortAscending Then
                        cell.CssClass = String.Concat(cell.CssClass, " TransactionManagerGridHeaderCellSortedAscending")
                    Else
                        cell.CssClass = String.Concat(cell.CssClass, " TransactionManagerGridHeaderCellSortedDescending")
                    End If
                End If
            Next
        End If
        ' _ddCount += 1
    End Sub

    Private Function CanShowRecurringPaymentLink(ByVal datarow As GivingHistoryDataRow) As Boolean
        Dim show As Boolean
        Dim currencyRulesSatisfied As Boolean

        If RecurringGiftPaymentMerchantAccountID <> Guid.Empty Then
            If PortalSettings.Current.Features.MulticurrencyConditionSettingExists() Then
                Dim supportsCurrency As Boolean = MerchantAccount.SupportsCurrency(merchantAccountID:=0, _
                                                                    merchantAccountGuid:=RecurringGiftPaymentMerchantAccountID.ToString, _
                                                                    currencyID:=String.Empty, _
                                                                    currencyISO:=datarow.Gifts__CurrencyISO)
                If supportsCurrency Then
                    currencyRulesSatisfied = True
                Else
                    If AllowCrossCurrencyPayment Then
                        Dim merchantAccountCurrencyIso As String = MerchantAccount.GetCurrencyISO(RecurringGiftPaymentMerchantAccountID.ToString)
                        Dim exchangeRate As Decimal = Internationalization.Currency.GetExchangeRate(merchantAccountCurrencyIso, datarow.Gifts__CurrencyISO)
                        Dim accountSystem_CurrencySet_Contains_PaymentCurrency As Boolean = datarow.Gifts__PaymentCurrencyAcceptable

                        If exchangeRate > 0 AndAlso accountSystem_CurrencySet_Contains_PaymentCurrency Then
                            currencyRulesSatisfied = True
                        End If
                    End If
                End If
            Else
                currencyRulesSatisfied = True
            End If
        End If

        show = (Not RecurringGiftPaymentPageInValid AndAlso _
                Not datarow.Gifts__IsSoftCredit AndAlso _
                (String.IsNullOrEmpty(datarow.RecurringGiftEndDate) OrElse _
                    (Not String.IsNullOrEmpty(datarow.RecurringGiftEndDate) AndAlso _
                    Date.Compare(Date.Now, CDate(datarow.RecurringGiftEndDate)) <= 0)) AndAlso _
                Not datarow.Gifts__IsEFT AndAlso _
                currencyRulesSatisfied)

        Return show
    End Function

    Private Function CanShowPledgePaymentLink(ByVal datarow As GivingHistoryDataRow) As Boolean
        Dim show As Boolean
        'PaymentsInBBNCPledges datatable contains IDs of pledges that have an unprocessed payment in BBNC.
        Dim pendingPledgePayment As Boolean = (From pledge In Me.History.GivingHistoryData.PaymentsInBBNCPledges Where pledge.RecordID = datarow.Gifts__GiftRecordID Select pledge).Count > 0
        Dim currencyRulesSatisfied As Boolean

        If PledgePaymentMerchantAccountID <> Guid.Empty Then
            If PortalSettings.Current.Features.MulticurrencyConditionSettingExists() Then
                Dim supportsCurrency As Boolean = MerchantAccount.SupportsCurrency(merchantAccountID:=0, _
                                                                                    merchantAccountGuid:=PledgePaymentMerchantAccountID.ToString, _
                                                                                    currencyID:=String.Empty, _
                                                                                    currencyISO:=datarow.Gifts__CurrencyISO)
                If supportsCurrency Then
                    currencyRulesSatisfied = True
                Else
                    If AllowCrossCurrencyPayment Then
                        Dim merchantAccountCurrencyIso As String = MerchantAccount.GetCurrencyISO(PledgePaymentMerchantAccountID.ToString)
                        Dim exchangeRate As Decimal = Internationalization.Currency.GetExchangeRate(merchantAccountCurrencyIso, datarow.Gifts__CurrencyISO)
                        Dim accountSystem_CurrencySet_Contains_PaymentCurrency As Boolean = datarow.Gifts__PaymentCurrencyAcceptable

                        If exchangeRate > 0 AndAlso accountSystem_CurrencySet_Contains_PaymentCurrency Then
                            currencyRulesSatisfied = True
                        End If
                    End If
                End If
            Else
                currencyRulesSatisfied = True
            End If
        End If

        show = (Not PledgePaymentPageInValid AndAlso _
                datarow.Gifts__BalanceUnformatted > 0.0 AndAlso _
                Not pendingPledgePayment AndAlso _
                Not datarow.Gifts__IsEFT AndAlso _
                Not datarow.Gifts__IsSoftCredit AndAlso _
                Not datarow.Gifts__Type = "MG Pledge" AndAlso _
                currencyRulesSatisfied)

        Return show
    End Function

    Private Function CanShowEventPaymentLink(ByVal datarow As GivingHistoryDataRow) As Boolean
        Dim show As Boolean
        'PaymentsInBBNCEvents datatable contains RegistrationIDs of Event Reg fees that have an unprocessed payment in BBNC.
        Dim currencyRulesSatisfied As Boolean
        Dim pendingEventRegPayment As Boolean = (From eventReg In Me.History.GivingHistoryData.PaymentsInBBNCEvents _
                                                 Where eventReg.RegistrationID = datarow.Event_participations__RegistrationID.ToString _
                                                 Select eventReg).Count > 0

        If EventPaymentMerchantAccountID <> Guid.Empty Then
            If PortalSettings.Current.Features.MulticurrencyConditionSettingExists() Then
                Dim supportsCurrency As Boolean = MerchantAccount.SupportsCurrency(merchantAccountID:=0, _
                                                                                    merchantAccountGuid:=EventPaymentMerchantAccountID.ToString, _
                                                                                    currencyID:=String.Empty, _
                                                                                    currencyISO:=datarow.Gifts__CurrencyISO)
                If supportsCurrency Then
                    currencyRulesSatisfied = True
                Else
                    If AllowCrossCurrencyPayment Then
                        Dim merchantAccountCurrencyIso As String = MerchantAccount.GetCurrencyISO(EventPaymentMerchantAccountID.ToString)
                        Dim exchangeRate As Decimal = Internationalization.Currency.GetExchangeRate(merchantAccountCurrencyIso, datarow.Gifts__CurrencyISO)

                        If exchangeRate > 0 Then
                            currencyRulesSatisfied = True
                        End If
                    End If
                End If
            Else
                currencyRulesSatisfied = True
            End If
        End If

        show = (Not pendingEventRegPayment AndAlso currencyRulesSatisfied)

        Return show
    End Function

    Private Function CanShowAdditionalDonationLink(ByVal datarow As GivingHistoryDataRow) As Boolean
        Dim show As Boolean
        Dim supportsCurrency As Boolean

        If AdditionalDonationMerchantAccountID <> Guid.Empty Then
            If PortalSettings.Current.Features.MulticurrencyConditionSettingExists() Then
                supportsCurrency = MerchantAccount.SupportsCurrency(merchantAccountID:=0, _
                                                                    merchantAccountGuid:=AdditionalDonationMerchantAccountID.ToString, _
                                                                    currencyID:=String.Empty, _
                                                                    currencyISO:=datarow.Gifts__CurrencyISO)
            Else
                supportsCurrency = True
            End If
        End If

        show = (Not AdditionalDonationPageInValid AndAlso supportsCurrency)

        Return show
    End Function

    Private Function LoadFinancialInstitution(ByVal e As Blackbaud.AppFx.CMS.FCL.GiftEditor.EditRecurringGiftPaymentEventArgs) As String
        Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
        Dim financialInstitutionFilterData = New SearchLists.FinancialInstitution.FinancialInstitutionSearchFilterData
        Dim financialInstitutionId As String = String.Empty

        financialInstitutionFilterData.ROUTINGNUMBER = e.RoutingNumber
        financialInstitutionFilterData.EXACTMATCHONLY = True

        Dim financialInstitutions = SearchLists.FinancialInstitution.FinancialInstitutionSearch.GetIDs(provider, financialInstitutionFilterData)

        If financialInstitutions.Count > 0 Then
            financialInstitutionId = financialInstitutions(0)
        Else
            Dim fieldInfo1 = Core.AddForms.FinancialInstitution.FinancialInstitutionAddForm.LoadData(provider)
            With fieldInfo1
                .FINANCIALINSTITUTION = e.FinancialInstitution
                .BRANCHNAME = e.BranchName
                .ROUTINGNUMBER = e.RoutingNumber
                .ISSPONSORINGINSTITUTION = False
                .COUNTRYID = New Guid(e.FinancialInstitutionCountryID)
            End With

            financialInstitutionId = fieldInfo1.Save(provider)
        End If

        Return financialInstitutionId
    End Function

    Private Function CheckNewAccountInfoWithCurrentAccounts(ByVal constitid As String, ByVal routingnumber As String, ByVal accountnumber As String) As Boolean
        Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
        Dim constitFinancialAccounts = DataLists.Constituent.ConstituentFinancialAccountsList.GetRows(provider, constitid)
        Dim currentAccount = constitFinancialAccounts.Where(Function(ca) (ca.Account_number = accountnumber And ca.Routing_number = routingnumber))
        Dim retval As Boolean = False

        'currentAccount is a check to see if the routing number and account number entered by the user matches an entry from the constituent's financial accounts list
        'If it does, then we don't want to create a duplicate entry in the database.
        If currentAccount.Count > 0 Then
            retval = True
        End If

        Return retval
    End Function

    Public Sub PayUnpaidEventRegistration(ByVal HostRegistrantID As Guid)
        ObsoleteAPIInstantiation()

        Dim shoppingCart = Core.CoreSalesOrderCart.GetCachedCart()
        If CanAddNewEventItemToCart(shoppingCart.ID, HostRegistrantID) Then
            Dim eventItem As New Core.Data.BBEC.ShoppingCart.EventItem
            Dim detailsDataSet = Core.Data.GivingHistory.getGivingHistoryDataSource(Me.History.GivingHistoryData, Me.History.IncludeUnpaidEvents)

            Dim registrationsAttributes = From x In detailsDataSet Where x.Event_participations__EventRegistrantID = HostRegistrantID Select x.Event_participations__EventDetails, x.Gifts__BalanceUnformatted, x.Gifts__Balance, x.Event_participations__EventName, x.Event_participations__EventRegistrantID, x.Gifts__CurrencyISO

            If registrationsAttributes IsNot Nothing AndAlso registrationsAttributes.Count > 0 Then
                Dim registrationAttributes = registrationsAttributes(0)

                'check for currency support here
                'and set the eventItem price and buildshelbyEventRgTran with the converted amount if needed.
                Dim eventRegistrationTransactionCurrencyISO As String = registrationAttributes.Gifts__CurrencyISO
                Dim ma As New MerchantAccount(EventPaymentMerchantAccountID)
                Dim merchantAccountCurrencyISO As String = ma.ISO4217
                Dim merchantAccountCurrencySymbol As String = ma.currencySymbol
                Dim eventRegistrationCurrencySupported As Boolean = (merchantAccountCurrencyISO = eventRegistrationTransactionCurrencyISO)

                With eventItem
                    If eventRegistrationCurrencySupported Then
                        .SetData(BuildShelbyEventRegTran(registrationAttributes.Gifts__BalanceUnformatted, HostRegistrantID, merchantAccountCurrencyISO, merchantAccountCurrencySymbol))
                        .Price = CDbl(registrationAttributes.Gifts__BalanceUnformatted)
                    Else
                        Dim fxBalance As Decimal = GivingHistoryCommon.GetAmountInPaymentCurrency(registrationAttributes.Gifts__BalanceUnformatted, eventRegistrationTransactionCurrencyISO, merchantAccountCurrencyISO)
                        .SetData(BuildShelbyEventRegTran(fxBalance, HostRegistrantID, merchantAccountCurrencyISO, merchantAccountCurrencySymbol))
                        .Price = CDbl(fxBalance)
                        .Options.HasConvertedCurrencyPricing = True
                    End If

                    .Quantity = 1
                    .Total = .Price

                    .Description = registrationAttributes.Event_participations__EventName
                    .Options.Quantity.Visible = False
                    .EventGroupID = Guid.NewGuid
                    .UnpaidExistingEvent = True

                    'Balance is formatted with the correct currency in Content\Core\Data\GivingHistory\InfinityGivingHistoryLoader.vb AddGift()
                    .Attributes = BuildUnpaidEventAttributes(registrationAttributes.Gifts__Balance, registrationAttributes.Event_participations__EventDetails, registrationAttributes.Event_participations__EventRegistrantID.ToString)
                End With

                shoppingCart.Add(eventItem)
                shoppingCart.Save()
            End If
        End If

        URLBuilder.CommonRedirect(URLBuilder.GetSitePageLinkBuilder(Me.History.UnpaidEventsPaymentPageID).ToString)
    End Sub

    Private Function CanAddNewEventItemToCart(ByVal SalesOrderID As Guid, ByVal HostRegistrantID As Guid) As Boolean
        'Check to see if there is an item in the cart already related to this registration
        Dim cartItemRows = DataLists.SalesOrder.SalesOrderList.GetRows(Core.AppFx.ServiceProvider.Current, SalesOrderID.ToString)
        'sterling wi 124822 added null checks
        If cartItemRows IsNot Nothing AndAlso cartItemRows.Count > 0 Then
            Dim cartItems = From x In cartItemRows Select x.ATTRIBUTES
            For Each item In cartItems
                Dim itemAttributes As Generic.List(Of BaseClassLibrary.Item.Attribute) = DirectCast(DataObject.FromXML(Of Generic.List(Of BaseClassLibrary.Item.Attribute))(item), Generic.List(Of BaseClassLibrary.Item.Attribute))
                If itemAttributes IsNot Nothing AndAlso itemAttributes.Count > 0 Then
                    Dim itemRegistrantID = (From x In itemAttributes Where x.Value = HostRegistrantID.ToString).SingleOrDefault
                    If itemRegistrantID IsNot Nothing Then
                        Return False
                    End If
                End If
            Next
        End If

        Return True
    End Function

    Private Function BuildUnpaidEventAttributes(ByVal Balance As String, ByVal EventDetails As String, ByVal HostRegistrantID As String) As Generic.List(Of BaseClassLibrary.Item.Attribute)
        Dim cartAttributes As New Generic.List(Of BaseClassLibrary.Item.Attribute)
        Dim eventDetailsAttr As New BaseClassLibrary.Item.Attribute
        With eventDetailsAttr
            .Name = Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.FIELD_BALANCE, LanguageMetaData.Encoding.HtmlEncoded)
            .Value = Balance
        End With
        cartAttributes.Add(eventDetailsAttr)

        eventDetailsAttr = New BaseClassLibrary.Item.Attribute
        With eventDetailsAttr
            .HideName = True
            .Name = "EventDetails"
            .Value = EventDetails
        End With
        cartAttributes.Add(eventDetailsAttr)

        eventDetailsAttr = New BaseClassLibrary.Item.Attribute
        With eventDetailsAttr
            .HideName = True
            .HideValue = True
            .Name = "HostRegistrantID"
            .Value = HostRegistrantID
        End With
        cartAttributes.Add(eventDetailsAttr)

        Return cartAttributes
    End Function

    Private Function BuildShelbyEventRegTran(ByVal total As Decimal, ByVal hostRegistrantID As Guid, ByVal giftCurrencyISO As String, ByVal giftCurrencySymbol As String) As ShelbyEventRegTran
        Dim tran As New ShelbyEventRegTran(CType(BuildTransactionOriginInformation(PortalSettings.Current.ActivePage, Me.ContentObject), OriginInformation))
        Dim registrationInfoList As New Generic.List(Of RegistrationInformation)
        Dim unpaidEvents = From x In Me.History.GivingHistoryData.Event_participations Where x.HostRegistrantID = hostRegistrantID.ToString

        If unpaidEvents IsNot Nothing AndAlso unpaidEvents.Count > 0 Then
            Dim unpaidEvent = unpaidEvents(0)
            Dim registrationInfo As New RegistrationInformation

            Dim shelbyEventIDs = DataLists.Event.ShelbyEventIDFromEventGUIDDataList.GetRows(Core.AppFx.ServiceProvider.Current, unpaidEvent.EventRecordID)

            If shelbyEventIDs IsNot Nothing AndAlso shelbyEventIDs.Count > 0 Then
                registrationInfo.Event = New EventInformation
                With registrationInfo.Event
                    .Name = unpaidEvent.EventName
                    .BackOfficeID = shelbyEventIDs(0).ID
                    tran.ShelbyEventID = shelbyEventIDs(0).ID
                End With
            End If

            Dim registrationTypes = From x In unpaidEvents Select x.TicketType
            Dim regFeeInformation As New Generic.List(Of RegistrationFeeInformation)

            For Each registrationType In registrationTypes.ToList.Distinct
                Dim registrationTicketType As String = registrationType
                Dim participants = From x In unpaidEvents Where x.TicketType = registrationTicketType
                Dim registrationTypeInformation = participants(0)

                Dim feeInformation As New RegistrationFeeInformation
                With feeInformation
                    .GiftAmount = System.Xml.XmlConvert.ToString(registrationTypeInformation.TicketPriceUnformatted)
                    .ShelbyDescription = registrationTicketType
                    .Unit = registrationTicketType
                    .RegistrationID = New Guid(registrationTypeInformation.RegistrationID)
                    Dim shelbyEventPriceIDs = DataLists.Event.ShelbyEventPriceIDFromGUIDDataList.GetRows(Core.AppFx.ServiceProvider.Current, registrationTypeInformation.EventPriceID)
                    If shelbyEventPriceIDs IsNot Nothing AndAlso shelbyEventPriceIDs.Count > 0 Then
                        .BackOfficeID = shelbyEventPriceIDs(0).ID
                    End If
                End With

                Dim regGuestInformation As New Generic.List(Of RegistrantInformation)
                Dim guestCount As Integer = 0
                For Each participant In participants
                    guestCount += 1

                    Dim guestInformation As New RegistrantInformation
                    With guestInformation
                        Dim recordsGUID As Guid = New Guid(participant.GuestConstituentID)
                        If recordsGUID <> Guid.Empty Then
                            .Name = participant.ParticipantName
                            .RecordsGUID = recordsGUID
                        Else
                            .IsAnonymous = True
                            ' Use the RecordsGUID for the registrantID instead of the constituentID
                            ' for the unknown guest so we have something to map the registrant to in the plugin
                            .RecordsGUID = New Guid(participant.GuestRegistrantID)
                        End If

                        If participant.GuestRegistrantID = participant.HostRegistrantID Then
                            .IsPayer = True
                        End If

                    End With
                    regGuestInformation.Add(guestInformation)

                    If guestCount >= CInt(registrationTypeInformation.RegistrationCount) Then
                        Dim oldFeeInformation As New RegistrationFeeInformation
                        feeInformation.Guests = regGuestInformation.ToArray
                        regFeeInformation.Add(feeInformation)
                        oldFeeInformation = feeInformation

                        feeInformation = New RegistrationFeeInformation
                        With feeInformation
                            .BackOfficeID = oldFeeInformation.BackOfficeID
                            .RegistrationID = oldFeeInformation.RegistrationID
                            .GiftAmount = oldFeeInformation.GiftAmount
                            .ShelbyDescription = oldFeeInformation.ShelbyDescription
                            .ShelbyEventPriceID = oldFeeInformation.ShelbyEventPriceID
                            .Unit = oldFeeInformation.Unit
                        End With

                        regGuestInformation = New Generic.List(Of RegistrantInformation)
                        guestCount = 0
                    End If
                Next
            Next

            registrationInfo.Fees = regFeeInformation.ToArray
            registrationInfoList.Add(registrationInfo)
        End If

        tran.EmailID = PortalSettings.EmailSourceID
        tran.EmailSubject = PortalSettings.EmailSubject
        tran.UnpaidEventRegistrationPayment = True
        tran.Donor = New DonorInformation()
        tran.Gift = New GiftInformation()
        tran.Gift.Amount = CSng(total)
        tran.Gift.CurrencySymbol = giftCurrencySymbol
        tran.Gift.ISO4217 = giftCurrencyISO
        tran.Registrations = registrationInfoList.ToArray
        tran.TransactionGUID = System.Guid.NewGuid
        tran.Status = ShelbyTransaction.ETransactionStatus.Pending

        Return tran
    End Function

    Public Sub LinkEditPayment_Save_Handler(ByVal sender As Object, ByVal e As Blackbaud.AppFx.CMS.FCL.GiftEditor.PaymentEventArgsBase)
        Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
        Dim constit As ShelbyConstituent = New Core.Data.ShelbyConstituent(BBWebPrincipal.Current.User)
        Dim constitID As String = Core.Data.ShelbyConstituent.GetConstituentsGuid(constit.RecordID).ToString
        Dim gp As Blackbaud.AppFx.CMS.FCL.GiftEditor.EditRecurringGiftPaymentEventArgs

        Select Case e.PaymentMethod
            Case 3 'Direct Debit TODO: type this value

                gp = DirectCast(e, Blackbaud.AppFx.CMS.FCL.GiftEditor.EditRecurringGiftPaymentEventArgs)

                If gp.NewDirectDebitAccount Then
                    Dim financialInstitutionId As String = String.Empty

                    'bug 88712 add validations to language tab
                    If CheckNewAccountInfoWithCurrentAccounts(constitID, gp.RoutingNumber, gp.AccountNumber) Then
                        Throw New ApplicationException(Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_DUPLICATE_ACCOUNT, LanguageMetaData.Encoding.HtmlEncoded)) '"Account already exists. Please select from payment account list or enter new account information")
                    End If

                    Try
                        financialInstitutionId = LoadFinancialInstitution(gp)
                    Catch ex As Exception
                        If ex.Message.ToLower.Contains("routing number") Then
                            Throw New ApplicationException(Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.PAYMENT_EDIT_VALIDATION_INVALID_ROUTING_NUM, LanguageMetaData.Encoding.HtmlEncoded))
                        Else
                            Throw New ApplicationException(ex.Message)
                        End If

                    End Try

                    'TODO: Need some sanity check here to ensure returned string is a valid guid.
                    If Not String.IsNullOrEmpty(financialInstitutionId) Then
                        Dim constituentAccountId As String = String.Empty
                        Dim fieldInfo2 = Core.AddForms.ConstituentFinancialAccount.ConstituentFinancialAccountAddForm.LoadData(provider, constitID)
                        With fieldInfo2
                            .ACCOUNTNAME = gp.AccountName
                            .ACCOUNTNUMBER = gp.AccountNumber
                            .ACCOUNTTYPE = CType(gp.AccountType, AddForms.ConstituentFinancialAccount.ConstituentFinancialAccountAddFormData.ACCOUNTTYPEVALUES)
                            .EFTSTATUSCODE = AddForms.ConstituentFinancialAccount.ConstituentFinancialAccountAddFormData.EFTSTATUSCODEVALUES.Prenotify
                            .FINANCIALINSTITUTIONID = New Guid(financialInstitutionId)
                        End With

                        Try
                            constituentAccountId = fieldInfo2.Save(provider)

                            'TODO: Need some sanity check here to ensure returned string is a valid guid.
                            If Not String.IsNullOrEmpty(constituentAccountId) Then

                                Dim fieldInfo3 = Core.EditForms.Revenue.RecurringGiftPaymentDetailsEditFormBBNC.LoadData(provider, e.RevenueID)
                                With fieldInfo3
                                    .ACCOUNTID = New Guid(constituentAccountId)
                                    .AUTOPAY = True
                                    .CONSTITUENTID = New Guid(constitID)
                                    .PAYMENTMETHODCODE = EditForms.Revenue.RecurringGiftPaymentDetailsEditFormBBNCData.PAYMENTMETHODCODEVALUES.Direct_Debit
                                    .REFERENCEDATE = CType(Date.Now, Blackbaud.AppFx.FuzzyDate)
                                    .CARDHOLDERNAME = Nothing
                                    .CREDITCARDNUMBER = Nothing
                                    .CREDITCARDTOKEN = Nothing
                                    .CREDITTYPECODEID = Nothing
                                    .DDISOURCECODEID = Nothing
                                    .DDISOURCEDATE = Nothing
                                    .EXPIRESON = Blackbaud.AppFx.FuzzyDate.Empty
                                    .PMADVANCENOTICESENTDATE = Nothing
                                    .PMINSTRUCTIONDATE_CANCEL = Nothing
                                    .PMINSTRUCTIONDATE_NEW = Nothing
                                    .PMINSTRUCTIONDATE_SETUP = Nothing
                                    .PMINSTRUCTIONTOSENDCODE = EditForms.Revenue.RecurringGiftPaymentDetailsEditFormBBNCData.PMINSTRUCTIONTOSENDCODEVALUES.New
                                    .REFERENCENUMBER = ""
                                    .SENDPMINSTRUCTION = Nothing
                                    .STANDINGORDERSETUP = Nothing
                                    .STANDINGORDERSETUPDATE = Nothing
                                End With


                                'ViM 3/22/2010:
                                'The code generated file (BBMetalWeb) for this spec has been manually changed to get around the inability to save a Null value 
                                'from a dataform field while using BBMetalWeb. BBMetalWeb by default generates code that calls SetValueIfNotNull for each of the 
                                'fields on a form before passing data to be written to the database. This issue is being investigated and should have a fix in the near future. 
                                Try
                                    fieldInfo3.Save(provider)
                                Catch ex As Exception
                                    Throw New ApplicationException(ex.Message)
                                End Try
                            End If
                        Catch ex As Exception
                            Throw New ApplicationException(ex.Message)
                        End Try
                    End If
                Else
                    'change to another direct debit account
                    Dim fieldInfo3 = Core.EditForms.Revenue.RecurringGiftPaymentDetailsEditFormBBNC.LoadData(provider, e.RevenueID)
                    With fieldInfo3
                        .ACCOUNTID = New Guid(gp.ConstituentAccountID)
                        .AUTOPAY = True
                        .CONSTITUENTID = New Guid(constitID)
                        .PAYMENTMETHODCODE = EditForms.Revenue.RecurringGiftPaymentDetailsEditFormBBNCData.PAYMENTMETHODCODEVALUES.Direct_Debit
                        .REFERENCEDATE = CType(Date.Now, Blackbaud.AppFx.FuzzyDate)
                        .CARDHOLDERNAME = Nothing
                        .CREDITCARDNUMBER = Nothing
                        .CREDITCARDTOKEN = Nothing
                        .CREDITTYPECODEID = Nothing
                        .DDISOURCECODEID = Nothing
                        .DDISOURCEDATE = Nothing
                        .EXPIRESON = Blackbaud.AppFx.FuzzyDate.Empty
                        .PMADVANCENOTICESENTDATE = Nothing
                        .PMINSTRUCTIONDATE_CANCEL = Nothing
                        .PMINSTRUCTIONDATE_NEW = Nothing
                        .PMINSTRUCTIONDATE_SETUP = Nothing
                        .PMINSTRUCTIONTOSENDCODE = EditForms.Revenue.RecurringGiftPaymentDetailsEditFormBBNCData.PMINSTRUCTIONTOSENDCODEVALUES.New
                        .REFERENCENUMBER = ""
                        .SENDPMINSTRUCTION = Nothing
                        .STANDINGORDERSETUP = Nothing
                        .STANDINGORDERSETUPDATE = Nothing
                    End With

                    'ViM 3/22/2010:
                    'The code generated file (BBMetalWeb) for this spec has been manually changed to get around the inability to save a Null value 
                    'from a dataform field while using BBMetalWeb. BBMetalWeb by default generates code that calls SetValueIfNotNull for each of the 
                    'fields on a form before passing data to be written to the database. This issue is being investigated and should have a fix in the near future. 
                    Try
                        fieldInfo3.Save(provider)
                    Catch ex As Exception
                        '"System cannot update your information at this time"               
                        Throw New ApplicationException(ex.Message)
                    End Try
                End If

            Case 2 'credit card
                Try
                    If e.BBPayApprovedTransaction Then
                        Dim fieldInfo3 = Core.EditForms.Revenue.RecurringGiftPaymentDetailsEditFormBBNC.LoadData(provider, e.RevenueID)
                        With fieldInfo3
                            .ACCOUNTID = Nothing
                            .AUTOPAY = True
                            .CONSTITUENTID = New Guid(constitID)
                            .PAYMENTMETHODCODE = EditForms.Revenue.RecurringGiftPaymentDetailsEditFormBBNCData.PAYMENTMETHODCODEVALUES.Credit_Card
                            .REFERENCEDATE = CType(Date.Now, Blackbaud.AppFx.FuzzyDate)
                            .CARDHOLDERNAME = e.CardHolderName
                            .CREDITCARDNUMBER = e.CreditCardNumber
                            .CREDITCARDTOKEN = New Guid(e.CreditCardToken)
                            .CREDITTYPECODEID = GetCreditTypeCode(e.CreditType)
                            If .CREDITTYPECODEID = Guid.Empty Then .CREDITTYPECODEID = Nothing
                            .DDISOURCECODEID = Nothing
                            .DDISOURCEDATE = Nothing
                            .EXPIRESON = CType(e.ExpiresOn, Blackbaud.AppFx.FuzzyDate)
                            .PMADVANCENOTICESENTDATE = Nothing
                            .PMINSTRUCTIONDATE_CANCEL = Nothing
                            .PMINSTRUCTIONDATE_NEW = Nothing
                            .PMINSTRUCTIONDATE_SETUP = Nothing
                            .PMINSTRUCTIONTOSENDCODE = EditForms.Revenue.RecurringGiftPaymentDetailsEditFormBBNCData.PMINSTRUCTIONTOSENDCODEVALUES.New
                            .REFERENCENUMBER = ""
                            .SENDPMINSTRUCTION = Nothing
                            .STANDINGORDERSETUP = Nothing
                            .STANDINGORDERSETUPDATE = Nothing
                        End With

                        fieldInfo3.Save(provider)
                    Else
                        Dim fieldInfo = Core.EditForms.RevenueSchedule.RevenueScheduleCreditCardEditDataForm.LoadData(provider, e.RevenueID)
                        With fieldInfo
                            .CREDITCARDTOKEN = New Guid(e.CreditCardToken)
                        End With

                        fieldInfo.Save(provider)
                    End If
                Catch ex As Exception
                    Throw New ApplicationException(ex.Message) 'TODO: '"System cannot update your information at this time"  
                End Try
        End Select
        CType(sender, BBNCExtensions.ServerControls.UserModalPart).RefreshPage(False)
    End Sub

    Private Function GetCreditTypeCode(ByVal IINCodeString As String) As Guid
        Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
        Dim codeTableFilterData As New DataLists.CodeTable.CodeTableEntryListFilterData
        codeTableFilterData.INCLUDEINACTIVE = False
        Dim codeTableList() = DataLists.CodeTable.CodeTableEntryList.GetRows(provider, "D428EFCD-A5C7-45FA-8F34-444FDE67B05C", codeTableFilterData)
        Dim retval As Guid = Guid.Empty

        For Each row As DataLists.CodeTable.CodeTableEntryListRow In codeTableList
            If row.Description.Trim.ToUpper.Equals(IINCodeString) OrElse row.Description.Trim.Replace(" ", "").ToUpper.Equals(IINCodeString) Then
                retval = row.ID
                Exit For
            End If
        Next

        Return retval
    End Function

    Public Sub LinkEditRecurringGift_Save_Handler(ByVal sender As Object, ByVal e As Blackbaud.AppFx.CMS.FCL.GiftEditor.EditRecurringGiftEventArgs)
        'since the edit control is supposed to stay neutral the load/save has to be done here
        'this will become problematic when we want to allow them to edit more info about the splits
        'i think the edit control should load and save itself so we dont have to do this...
        Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
        Dim fieldInfo = Core.EditForms.Revenue.RecurringGiftDetailsEditDataForm.LoadData(provider, e.RevenueID)

        fieldInfo.AMOUNT = e.Amount

        'fix up startdate and frequency
        Dim frequencyInt As Integer
        Dim nextTranDate As Date = e.StartDate

        If Not e.Frequency = Guid.Empty Then
            Dim freqDetails() As Core.DataLists.TopLevel.RecurrenceRecordsDatalistRow
            Dim filter As New Core.DataLists.TopLevel.RecurrenceRecordsDatalistFilterData
            filter.IDs = e.Frequency.ToString
            freqDetails = Core.DataLists.TopLevel.RecurrenceRecordsDatalist.GetRows(provider, filter)

            If freqDetails IsNot Nothing AndAlso freqDetails.Count > 0 Then
                With freqDetails(0)
                    AppFx.ContentManagement.UI.WebControls.RecurrenceHelper.Helper.CalculateGiftSchedule(nextTranDate, frequencyInt, CType(.RECURRENCETYPE, AppFx.ContentManagement.UI.WebControls.RecurrenceHelper.ERecurrenceType), CType(.DAYOFWEEK, AppFx.ContentManagement.UI.WebControls.RecurrenceHelper.EDaysOfWeek), CInt(.DAY), CType(.MONTH, AppFx.ContentManagement.UI.WebControls.RecurrenceHelper.EMonthOfYear), CInt(.INTERVAL))
                End With
            End If
        Else
            With e.GeneralFrequency
                AppFx.ContentManagement.UI.WebControls.RecurrenceHelper.Helper.CalculateGiftSchedule(nextTranDate, frequencyInt, .RecurrenceType, .DayOfWeek, .DayOfMonth, CType(.Month, AppFx.ContentManagement.UI.WebControls.RecurrenceHelper.EMonthOfYear), .Interval)
            End With
        End If

        fieldInfo.FREQUENCYCODE = CType(frequencyInt, EditForms.Revenue.RecurringGiftDetailsEditDataFormData.FREQUENCYCODEVALUES)
        fieldInfo.STARTDATE = nextTranDate

        'ViM 3/22/2010:
        'The code generated file (BBMetalWeb) for this spec has been manually changed to get around the inability to save a Null value 
        'from a dataform field while using BBMetalWeb. BBMetalWeb by default generates code that calls SetValueIfNotNull for each of the 
        'fields on a form before passing data to be written to the database. This issue is being investigated and should have a fix in the near future. 
        If Date.Compare(e.EndDate, Date.MinValue) = 0 Then
            fieldInfo.ENDDATE = Nothing
        Else
            fieldInfo.ENDDATE = e.EndDate
        End If

        'bug 88712 add validations to language tab
        If fieldInfo.ENDDATE IsNot Nothing AndAlso Date.Compare(fieldInfo.ENDDATE.Value, nextTranDate) < 0 Then
            Throw New ApplicationException(Me.LanguageDataFromDisplayControl.GetLanguageString(GivingHistoryLanguage.LanguageGuids.DETAILS_EDIT_VALIDATION_INVALID_NEXT_TRANSACTION, LanguageMetaData.Encoding.HtmlEncoded))
        End If

        'fix up splits, only thing we allow them to edit is the overall amount
        'so all we need to do is re-distribute
        Dim originalWeights(fieldInfo.SPLITS.Count - 1) As Decimal
        Dim resultingAmounts(fieldInfo.SPLITS.Count - 1) As Decimal
        For index As Integer = 0 To fieldInfo.SPLITS.Count - 1 Step 1
            originalWeights(index) = fieldInfo.SPLITS(index).AMOUNT.Value
        Next

        resultingAmounts = Blackbaud.Web.Content.Core.Data.BBEC.Revenue.RevenueHelper.DistributeWeighted(e.Amount, originalWeights, 2)

        For index As Integer = 0 To resultingAmounts.Count - 1 Step 1
            fieldInfo.SPLITS(index).AMOUNT = resultingAmounts(index)
        Next

        'fieldInfo.NEXTINSTALLMENTID 'this stays the same? i think? check this, first thing it does in sql is delete this row and all others matching the revID

        Try
            fieldInfo.Save(provider)
        Catch ex As Exception
            '"System cannot update your information at this time"               
            Throw New ApplicationException(ex.Message)
        End Try
        CType(sender, BBNCExtensions.ServerControls.UserModalPart).RefreshPage(False)
    End Sub

    Private Sub PaymentPages_GetMerchantAccount()
        If Me.History.PledgePaymentPageID > 0 Then
            PledgePaymentMerchantAccountID = MerchantAccount.GetMerchantAccount(Me.History.PledgePaymentPageID)
        End If

        If Me.History.RecurringGiftPaymentPageID > 0 Then
            RecurringGiftPaymentMerchantAccountID = MerchantAccount.GetMerchantAccount(Me.History.RecurringGiftPaymentPageID)
        End If

        If Me.History.RecGiftAdditionalDonationPageID > 0 Then
            AdditionalDonationMerchantAccountID = MerchantAccount.GetMerchantAccount(Me.History.RecGiftAdditionalDonationPageID)
        End If

        If Me.History.UnpaidEventsPaymentPageID > 0 Then
            EventPaymentMerchantAccountID = ShoppingCartPartData.GetShoppingCartMerchantAccountGuid(Me.History.UnpaidEventsPaymentPageID)
        End If
    End Sub

#Region "Exports"

    Private Sub PdfLinkButton_Click(ByVal snder As Object, ByVal e As System.EventArgs) Handles PdfLinkButton.Click
        If BBWebPrincipal.Current.User Is Nothing Then
            Return
        End If

        ''Create HTML for the pdf
        ''
        Dim flds = Me.History.UsedFields.Columns
        Dim lst As New List(Of String)
        Dim sbPdf As New System.Text.StringBuilder
        Dim stl As New Core.StyleSheet(PortalSettings.Current.ActivePage.StylesheetId)
        'We only want styles for the transaction manager.  Include defaults in case we decide to default the styling on the timestamp
        'We need to exclude CustomCSS and Common in case they've styled the <body> or <head> tags which would cause weirdness in the 
        'pdf output
        sbPdf.Append("<style type=""text/css"">" + stl.CSSForBrowser(Blackbaud.AppFx.Products.EContentTypes.GivingHistory2, ExcludeCommonStyles:=True, ExcludeDefaultStyles:=False, ExcludeCustomStyles:=True) + "</style>")
        sbPdf.Append("<div class=""ExportPdfContainer""><div class=""ExportPdfHeader""></div><div class=""ExportPdfDateContainer"">")
        sbPdf.Append(Server.HtmlEncode(Core.Common.DisplayDate(Date.UtcNow).ToString) + "</div>")
        sbPdf.Append("<table class=""ExportPdfTable""><tr class=""ExportPdfHeaderRow"">")
        'Headers
        Dim lngId As String
        For Each fld In flds
            lngId = Me.LanguageDataFromDisplayControl.GetLanguageString(fld.FieldLocalizationGuid, LanguageMetaData.Encoding.HtmlEncoded)
            sbPdf.Append("<th class=""ExportPdfHeaderCell""><b>" + lngId + "</b></th>")
            lst.Add(GivingHistoryDataRow.GenerateDataField(fld.TableName, fld.ColumnName))
        Next
        sbPdf.Append("</tr>")

        'Fields
        Dim prop As String
        Dim dynobj As Object
        For Each dta In Me.GetRepeaterDataSource(Me.FilterChangedSinceCall, False)
            sbPdf.Append("<tr class=""ExportPdfDataRow"">")
            For Each l In lst
                dynobj = CallByName(dta, l, CallType.Get)
                prop = If(dynobj IsNot Nothing, dynobj.ToString, "")
                sbPdf.Append("<td class=""ExportPdfTableCell"">" + prop + "</td>")
            Next
            sbPdf.Append("</tr>")
        Next
        sbPdf.Append("</table><div class=""ExportPdfFooter""></div></div>")


        ''Output the file bytes to the browser    
        ''        
        Dim pdfBts As Byte() = ddpdfHandler.Html2Pdf(sbPdf.ToString, Nothing)
        Response.Clear()
        Response.ContentType = "application/pdf"
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(BBWebPrincipal.Current.User.DisplayName) + "_Transaction_Export.pdf")
        Response.AppendHeader("Content-Length", pdfBts.Length.ToString)
        Response.BinaryWrite(pdfBts)
        Response.End()


    End Sub

    Private Sub CsvLinkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CsvLinkButton.Click
        If BBWebPrincipal.Current.User Is Nothing Then
            Return
        End If
        Dim sbCsv As New System.Text.StringBuilder
        Dim flds = Me.History.UsedFields.Columns
        Dim lst As New List(Of String)
        Dim lngId As String

        'Output field headers
        For Each fld In flds
            lngId = Me.LanguageDataFromDisplayControl.GetLanguageString(fld.FieldLocalizationGuid, LanguageMetaData.Encoding.HtmlEncoded)
            If lngId.Contains(",") Then lngId = """" + lngId + """"
            sbCsv.Append(lngId + ",")
            lst.Add(GivingHistoryDataRow.GenerateDataField(fld.TableName, fld.ColumnName))
        Next
        sbCsv.Remove(sbCsv.Length - 1, 1)
        sbCsv.Append(vbNewLine)

        Dim prop As String
        Dim dynobj As Object
        For Each dta In Me.GetRepeaterDataSource(Me.FilterChangedSinceCall, False)
            For Each l In lst
                dynobj = CallByName(dta, l, CallType.Get)
                prop = If(dynobj IsNot Nothing, dynobj.ToString, "")
                If prop.Contains(",") Then prop = """" + prop + """"
                sbCsv.Append(prop + ",")
            Next
            'Remove trailing , from the row,
            sbCsv.Remove(sbCsv.Length - 1, 1)
            sbCsv.Append(vbNewLine)
        Next

        'Get rid of the railing newline
        sbCsv.Remove(sbCsv.Length - 1, 1)
        'Output the Csv File
        Response.Clear()
        Response.ContentType = "text/csv"
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(BBWebPrincipal.Current.User.DisplayName) + "_Transaction_Export.csv")
        Response.AppendHeader("Content-Length", sbCsv.Length.ToString)
        'String Write by default converts the string to utf-8 which causes ms excel to add garbage when the '£' character is present
        Response.Charset = "Windows-1252"
        Response.BinaryWrite(System.Text.Encoding.Default.GetBytes(sbCsv.ToString))
        Response.End()
    End Sub

    Public Sub ObsoleteAPIInstantiation()
        If HttpContext.Current.Items("bbncapi") Is Nothing Then
            HttpContext.Current.Items("bbncapi") = New Blackbaud.Web.Content.Core.Extensions.API.NetCommunity(Me.Page)
        End If
    End Sub

#End Region

    Private Function CreateStartupScript() As String
        Dim clientIds = New System.Text.StringBuilder
        clientIds.Append("{")
        clientIds.Append(String.Format("wrapperObj:'{0}'", Me.ExportContainer.ClientID))
        clientIds.Append(String.Format(",exportButton:'{0}'", Me.ExportDropDownButton.ClientID))
        clientIds.Append(String.Format(",exportList:'{0}'", Me.ExportListPanel.ClientID))
        'String.Format doesn't like an extra } in the string
        clientIds.Append("}")
        Return String.Format("$(BLACKBAUD.netcommunity.GivingHistory2Control($,{0}));", clientIds)
    End Function

    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        ScriptManager.RegisterClientScriptInclude(GivingHistoryContainer, GivingHistoryContainer.GetType, "GivingHistory2Control", Page.ResolveUrl("~/Client/Scripts/GivingHIstory2Control.js"))
        ScriptManager.RegisterStartupScript(GivingHistoryContainer, GivingHistoryContainer.GetType, "GivingHistory2ControlStartup", Me.CreateStartupScript, True)
        Dim rowcount As Integer = 0
        Dim scriptString As String = String.Empty

        While rowcount < _ddCount
            'scriptString = String.Concat(scriptString, String.Format("var menu{0}=new menu.dd(""menu{0}"");menu.init(""menu{0}"",""menuhover"");", rowcount))
            scriptString = String.Concat(scriptString, String.Format("var TransactionManagerAccDD{0}= new TINY.accordion.slider(""TransactionManagerAccDD{0}""); TransactionManagerAccDD{0}.init(""TransactionManagerAccDD{0}"",""h3"",1,-1);", rowcount))
            rowcount += 1
        End While


        'scriptString = String.Concat(scriptString, "var accDD1= new TINY.accordion.slider(""accDD1""); accDD1.init(""accDD1"",""h3"",1,-1);")
        'scriptString = String.Concat(scriptString, "var accDD3= new TINY.accordion.slider(""accDD3""); accDD3.init(""accDD3"",""h3"",1,-1);")
        ScriptManager.RegisterStartupScript(GivingHistoryContainer, GivingHistoryContainer.GetType, "GivingHistory2DropdownInit", scriptString, True)
        ScriptManager.RegisterClientScriptInclude(GivingHistoryContainer, GivingHistoryContainer.GetType, "GivingHistory2Dropdown", Page.ResolveUrl("~/Client/Scripts/GivingHistory2Dropdown.js"))

    End Sub
End Class
