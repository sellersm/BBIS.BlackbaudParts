Imports System.Linq
Imports System.Collections.Generic
Imports Blackbaud.Web.Content
Imports Blackbaud.Web.Content.Core

Namespace Data

    Public Class GivingHistory
        Inherits Core.DataObject
        Implements IContentProvider


#Region "Public Properties"
        Private _id As Integer
        Public Property ID() As Integer
            Get
                Return _id
            End Get
            Set(ByVal value As Integer)
                _id = value
            End Set
        End Property

        Private _includeSoftCredits As Boolean
        Public Property IncludeSoftCredits() As Boolean
            Get
                Return _includeSoftCredits
            End Get
            Set(ByVal value As Boolean)
                _includeSoftCredits = value
            End Set
        End Property

        Private _softCreditText As String
        Public Property SoftCreditText() As String
            Get
                Return _softCreditText
            End Get
            Set(ByVal Value As String)
                _softCreditText = Value
            End Set
        End Property

        Private _queryID As Integer
        Public Property QueryID() As Integer
            Get
                Return _queryID
            End Get
            Set(ByVal value As Integer)
                _queryID = value
            End Set
        End Property

        Private _siteContentID As Integer
        Public Property SiteContentID() As Integer
            Get
                Return _siteContentID
            End Get
            Set(ByVal value As Integer)
                _siteContentID = value
            End Set
        End Property

        Private m_oGiftTypes As New Hashtable
        Public Property GiftTypes() As Hashtable
            Get
                Return m_oGiftTypes
            End Get
            Set(ByVal value As Hashtable)
                m_oGiftTypes = value
            End Set
        End Property

        Private m_oCampaigns As New Hashtable
        Public Property Campaigns() As Hashtable
            Get
                Return m_oCampaigns
            End Get
            Set(ByVal value As Hashtable)
                m_oCampaigns = value
            End Set
        End Property

        Private m_oFunds As New Hashtable
        Public Property Funds() As Hashtable
            Get
                Return m_oFunds
            End Get
            Set(ByVal value As Hashtable)
                m_oFunds = value
            End Set
        End Property

        Private m_oAppeals As New Hashtable
        Public Property Appeals() As Hashtable
            Get
                Return m_oAppeals
            End Get
            Set(ByVal value As Hashtable)
                m_oAppeals = value
            End Set
        End Property

        Private _pageSize As Integer = 25
        Public Property PageSize() As Integer
            Get
                Return _pageSize
            End Get
            Set(ByVal value As Integer)
                _pageSize = value
            End Set
        End Property

        Private _useCustomFilter As Boolean
        Public Property UseCustomFilter() As Boolean
            Get
                Return _useCustomFilter
            End Get
            Set(ByVal value As Boolean)
                _useCustomFilter = value
            End Set
        End Property

        Private _useCustomColumns As Boolean
        Public Property UseCustomColumns() As Boolean
            Get
                Return _useCustomColumns
            End Get
            Set(ByVal value As Boolean)
                _useCustomColumns = value
            End Set
        End Property

        Private _includePending As Boolean
        Public Property IncludePending() As Boolean
            Get
                Return _includePending
            End Get
            Set(ByVal value As Boolean)
                _includePending = value
            End Set
        End Property

        Private _pendingText As String
        Public Property PendingText() As String
            Get
                Return _pendingText
            End Get
            Set(ByVal Value As String)
                _pendingText = Value
            End Set
        End Property

        Private _anonymousText As String
        Public Property AnonymousText() As String
            Get
                Return _anonymousText
            End Get
            Set(ByVal value As String)
                _anonymousText = value
            End Set
        End Property

        Private _anonymousDonorNameText As String
        Public Property AnonymousDonorNameText() As String
            Get
                Return _anonymousDonorNameText
            End Get
            Set(ByVal value As String)
                _anonymousDonorNameText = value
            End Set
        End Property

        Public ReadOnly Property ShowCampaigns() As Boolean
            Get
                ' TODO: Move this out of this class and into the options loader
                'Return Core.Data.RE7ServiceHelper.BOSystemOption(Core.BackOfficeSystemEnums.eBOSystemOptions.eShowGivingHistoryCampaignFilter)
                'Show for RE7, hide for BBEC
                Return PortalSettings.Current.Features.IsRE7
            End Get
        End Property

        Private _loaders As Generic.IEnumerable(Of ILoader(Of GivingHistoryFields))
        Private Property Loaders() As Generic.IEnumerable(Of ILoader(Of GivingHistoryFields))
            Get
                Return _loaders
            End Get
            Set(ByVal value As Generic.IEnumerable(Of ILoader(Of GivingHistoryFields)))
                _loaders = value
            End Set
        End Property

        Private _optionsManager As IDataObjectManager(Of GivingHistory)
        Public Property OptionsManager() As IDataObjectManager(Of GivingHistory)
            Get
                Return _optionsManager
            End Get
            Set(ByVal value As IDataObjectManager(Of GivingHistory))
                _optionsManager = value
            End Set
        End Property

        Private m_lAvailableFields As New GivingHistoryColumns
        Public Property AvailableFields() As GivingHistoryColumns
            Get
                If m_lAvailableFields.Columns Is Nothing Then
                    m_lAvailableFields.Columns = New Generic.List(Of GivingHistoryColumn)
                End If
                Return m_lAvailableFields
            End Get
            Set(ByVal value As GivingHistoryColumns)
                m_lAvailableFields = value
            End Set
        End Property

        Private m_lUsedFields As New GivingHistoryColumns
        Public Property UsedFields() As GivingHistoryColumns
            Get
                If m_lUsedFields.Columns Is Nothing Then
                    m_lUsedFields.Columns = New Generic.List(Of GivingHistoryColumn)
                End If
                Return m_lUsedFields
            End Get
            Set(ByVal value As GivingHistoryColumns)
                m_lUsedFields = value
            End Set
        End Property

        Private _includeBalanceTotal As Boolean
        Public Property IncludeBalanceTotal() As Boolean
            Get
                Return _includeBalanceTotal
            End Get
            Set(ByVal value As Boolean)
                _includeBalanceTotal = value
            End Set
        End Property

        Private _includePledgeTotal As Boolean
        Public Property IncludePledgeTotal() As Boolean
            Get
                Return _includePledgeTotal
            End Get
            Set(ByVal value As Boolean)
                _includePledgeTotal = value
            End Set
        End Property

        Private _includePendingTotal As Boolean
        Public Property IncludePendingTotal() As Boolean
            Get
                Return _includePendingTotal
            End Get
            Set(ByVal value As Boolean)
                _includePendingTotal = value
            End Set
        End Property

        Private _includeGiftTotal As Boolean
        Public Property IncludeGiftTotal() As Boolean
            Get
                Return _includeGiftTotal
            End Get
            Set(ByVal value As Boolean)
                _includeGiftTotal = value
            End Set
        End Property

        Private _includeSummary As Boolean
        Public Property IncludeSummary() As Boolean
            Get
                Return _includeSummary
            End Get
            Set(ByVal value As Boolean)
                _includeSummary = value
            End Set
        End Property

        Private _includeGiftAidTotal As Boolean
        Public Property IncludeGiftAidTotal() As Boolean
            Get
                Return _includeGiftAidTotal
            End Get
            Set(ByVal value As Boolean)
                _includeGiftAidTotal = value
            End Set
        End Property

        Private _includeTotalsCurrency As Boolean
        Public Property IncludeTotalsCurrency() As Boolean
            Get
                Return _includeTotalsCurrency
            End Get
            Set(ByVal value As Boolean)
                _includeTotalsCurrency = value
            End Set
        End Property

        Private _includeSoftCreditsTotal As Boolean
        Public Property IncludeSoftCreditsTotal() As Boolean
            Get
                Return _includeSoftCreditsTotal
            End Get
            Set(ByVal value As Boolean)
                _includeSoftCreditsTotal = value
            End Set
        End Property

        Private _includeHardCreditsTotal As Boolean
        Public Property IncludeHardCreditsTotal() As Boolean
            Get
                Return _includeHardCreditsTotal
            End Get
            Set(ByVal value As Boolean)
                _includeHardCreditsTotal = value
            End Set
        End Property

        Private _pledgePaymentPageID As Integer
        Public Property PledgePaymentPageID() As Integer
            Get
                Return _pledgePaymentPageID
            End Get
            Set(ByVal value As Integer)
                _pledgePaymentPageID = value
            End Set
        End Property

        'recurring gift update options
        Private _recurringGiftEditAllowAmtUpdates As Boolean
        Public Property RecurringGiftEditAllowAmtUpdates() As Boolean
            Get
                Return _recurringGiftEditAllowAmtUpdates
            End Get
            Set(ByVal value As Boolean)
                _recurringGiftEditAllowAmtUpdates = value
            End Set
        End Property

        Private _recurringGiftEditAllowFreqUpdates As Boolean
        Public Property RecurringGiftEditAllowFreqUpdates() As Boolean
            Get
                Return _recurringGiftEditAllowFreqUpdates
            End Get
            Set(ByVal value As Boolean)
                _recurringGiftEditAllowFreqUpdates = value
            End Set
        End Property

        Private _recurringGiftEditAllowPmntTypeUpdates As Boolean
        Public Property RecurringGiftEditAllowPmntTypeUpdates() As Boolean
            Get
                Return _recurringGiftEditAllowPmntTypeUpdates
            End Get
            Set(ByVal value As Boolean)
                _recurringGiftEditAllowPmntTypeUpdates = value
            End Set
        End Property

        Private _recurringGiftEditAmtUpdateMinAmt As Decimal
        Public Property RecurringGiftEditAmtUpdateMinAmt() As Decimal
            Get
                Return Math.Round(_recurringGiftEditAmtUpdateMinAmt, 2)
            End Get
            Set(ByVal value As Decimal)
                _recurringGiftEditAmtUpdateMinAmt = Math.Round(value, 2)
            End Set
        End Property

        Private _recurringGiftEditFilterFunds As New Hashtable
        Public Property RecurringGiftEditFilterFunds() As Hashtable
            Get
                Return _recurringGiftEditFilterFunds
            End Get
            Set(ByVal value As Hashtable)
                _recurringGiftEditFilterFunds = value
            End Set
        End Property

        Private _recurringGiftEditFilterAppeals As New Hashtable
        Public Property RecurringGiftEditFilterAppeals() As Hashtable
            Get
                Return _recurringGiftEditFilterAppeals
            End Get
            Set(ByVal value As Hashtable)
                _recurringGiftEditFilterAppeals = value
            End Set
        End Property

        Private _recurringGiftEditFilterQueryID As Integer
        Public Property RecurringGiftEditFilterQueryID() As Integer
            Get
                Return _recurringGiftEditFilterQueryID
            End Get
            Set(ByVal value As Integer)
                _recurringGiftEditFilterQueryID = value
            End Set
        End Property

        Private _recurringGiftEditFilterUseCustomFilter As Boolean
        Public Property RecurringGiftEditFilterUseCustomFilter() As Boolean
            Get
                Return _recurringGiftEditFilterUseCustomFilter
            End Get
            Set(ByVal value As Boolean)
                _recurringGiftEditFilterUseCustomFilter = value
            End Set
        End Property

        Private _recurringGiftEditFreqUseGeneralRecurrence As Boolean
        Public Property RecurringGiftEditFreqUseGeneralRecurrence() As Boolean
            Get
                Return _recurringGiftEditFreqUseGeneralRecurrence
            End Get
            Set(ByVal value As Boolean)
                _recurringGiftEditFreqUseGeneralRecurrence = value
            End Set
        End Property

        Private _recGftEdtFreqIncludeRecurSchdEndDate As Boolean
        Public Property RecGftEdtFreqIncludeRecurSchdEndDate() As Boolean
            Get
                Return _recGftEdtFreqIncludeRecurSchdEndDate
            End Get
            Set(ByVal value As Boolean)
                _recGftEdtFreqIncludeRecurSchdEndDate = value
            End Set
        End Property

        Private _recGftEdtFreqIncludeRecurSchdStartDate As Boolean
        Public Property RecGftEdtFreqIncludeRecurSchdStartDate() As Boolean
            Get
                Return _recGftEdtFreqIncludeRecurSchdStartDate
            End Get
            Set(ByVal value As Boolean)
                _recGftEdtFreqIncludeRecurSchdStartDate = value
            End Set
        End Property

        Private _recGiftEditRecurrenceSpecifics() As String
        Public Property RecGiftEditRecurrenceSpecifics() As String()
            Get
                Return _recGiftEditRecurrenceSpecifics
            End Get
            Set(ByVal value As String())
                _recGiftEditRecurrenceSpecifics = value
            End Set
        End Property

        Private _recGiftAdditionalDonationPageID As Integer
        Public Property RecGiftAdditionalDonationPageID() As Integer
            Get
                _recGiftAdditionalDonationPageID = _recGiftAdditionalDonationPageID
                Return _recGiftAdditionalDonationPageID
            End Get
            Set(ByVal value As Integer)
                _recGiftAdditionalDonationPageID = value
            End Set
        End Property

        Private _recurringGiftPaymentPageID As Integer
        Public Property RecurringGiftPaymentPageID() As Integer
            Get
                Return _recurringGiftPaymentPageID
            End Get
            Set(ByVal value As Integer)
                _recurringGiftPaymentPageID = value
            End Set
        End Property

        Private _includeUnpaidEvents As Boolean
        Public Property IncludeUnpaidEvents() As Boolean
            Get
                Return _includeUnpaidEvents
            End Get
            Set(ByVal value As Boolean)
                _includeUnpaidEvents = value
            End Set
        End Property

        Private _unpaidEventsPaymentPageID As Integer
        Public Property UnpaidEventsPaymentPageID() As Integer
            Get
                Return _unpaidEventsPaymentPageID
            End Get
            Set(ByVal value As Integer)
                _unpaidEventsPaymentPageID = value
            End Set
        End Property

        Private _pledgePaymentCurrencyID As Guid?
        Public Property PledgePaymentCurrencyID() As Guid?
            Get
                Return _pledgePaymentCurrencyID
            End Get
            Set(ByVal value As Guid?)
                _pledgePaymentCurrencyID = value
            End Set
        End Property

        Private _recurringGiftPaymentCurrencyID As Guid?
        Public Property RecurringGiftPaymentCurrencyID() As Guid?
            Get
                Return _recurringGiftPaymentCurrencyID
            End Get
            Set(ByVal value As Guid?)
                _recurringGiftPaymentCurrencyID = value
            End Set
        End Property

        Private _truncateNumberOfRecords As Boolean
        Public Property TruncateNumberOfRecords() As Boolean
            Get
                Return _truncateNumberOfRecords
            End Get
            Set(ByVal value As Boolean)
                _truncateNumberOfRecords = value
            End Set
        End Property
#End Region

        ' We made this a function because the load on demand behavior felt more like a function
        ' This could store the variable privately and expose the fields if needed
        ' TODO: are we sure we don't want this to be a property?
        ' ...it seems like we could provide a "reload" function or something
        ' and to make this be a normal property instead of calling it every time or expecting
        ' the classes that use us to store it locally. For instance, the GH control stores this
        ' object as a public member...and every time he wants the history he has to load?
        ' Or else he has to keep a 2nd property that mirrors this function but just doesn't load?
        ' That just doesn't seem right.
        ' I've made it a property...let's review it.
        Private _givingHistoryData As GivingHistoryFields
        Public Property GivingHistoryData() As GivingHistoryFields
            Get
                If _givingHistoryData Is Nothing Then
                    _givingHistoryData = New GivingHistoryFields()

                    For Each loader In Me.Loaders
                        ' TODO: handle the exceptions thrown here
                        loader.Load(_givingHistoryData)
                    Next
                End If

                Return _givingHistoryData
            End Get
            Set(ByVal value As GivingHistoryFields)
                _givingHistoryData = value
            End Set
        End Property

        Private _recurrenceFrequencyDataSource As Generic.Dictionary(Of String, String)
        Public ReadOnly Property RecurrenceFrequencyDataSource() As Generic.Dictionary(Of String, String)
            Get
                If _recurrenceFrequencyDataSource Is Nothing Then
                    _recurrenceFrequencyDataSource = New Generic.Dictionary(Of String, String)()
                    If Me.RecurringGiftEditAllowFreqUpdates AndAlso Not Me.RecurringGiftEditFreqUseGeneralRecurrence Then
                        'get from DB
                        Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
                        Dim filterData = New DataLists.TopLevel.RecurrenceRecordsDatalistFilterData
                        filterData.IDs = ""
                        Dim recurrenceRecords() As DataLists.TopLevel.RecurrenceRecordsDatalistRow
                        If Me.RecGiftEditRecurrenceSpecifics IsNot Nothing AndAlso Me.RecGiftEditRecurrenceSpecifics.Count > 0 Then
                            Dim ids As String = String.Join(",", Me.RecGiftEditRecurrenceSpecifics)
                            If Not String.IsNullOrEmpty(ids) Then
                                filterData.IDs = ids
                                recurrenceRecords = DataLists.TopLevel.RecurrenceRecordsDatalist.GetRows(provider, filterData)

                                For Each recRecordRow As DataLists.TopLevel.RecurrenceRecordsDatalistRow In recurrenceRecords
                                    _recurrenceFrequencyDataSource.Add(recRecordRow.ID.ToString, GivingHistoryCommon.BuildFrequencySchedule(CType(recRecordRow.RECURRENCETYPE, Core.RecurrenceHelper.ERecurrenceType), CType(recRecordRow.DAYOFWEEK, Core.RecurrenceHelper.EDaysOfWeek), CInt(recRecordRow.DAY), CType(recRecordRow.MONTH, Core.RecurrenceHelper.EMonthOfYear), recRecordRow.INTERVAL))
                                Next
                            End If
                        End If
                    End If
                End If

                Return _recurrenceFrequencyDataSource
            End Get
        End Property

        ' This constructor uses Dependency Injection for determining which type of loader to use.
        ' The reason this happens is to decouple the class from the implementation of its datasource.
        ' There should not be any overloads within this class that provide an implementation of the
        ' required loader.
        Public Sub New(ByVal loaders As Generic.IEnumerable(Of ILoader(Of GivingHistoryFields)))
            Me.Loaders = loaders
        End Sub

        Public Sub New()
            ' Currently, we still need to have a default BBNC call in here due to the way cloning parts works.
            ' TODO: See if this is easily removed with an IoC container
            Me.OptionsManager = New BBNCGivingHistoryOptionsManager
        End Sub

        Public Shared Function IsPledge(ByVal gift As GivingHistoryFields.GiftsRow) As Boolean
            Return Not String.IsNullOrEmpty(gift.Balance)
        End Function

        Public Function PendingTextFor(ByVal type As String) As String
            Return Trim(String.Concat(type, " ", Me.PendingText))
        End Function

        Public Function CloneContentByID(ByVal CloneID As Integer, ByVal ClonedID As Integer, ByVal NewName As String, ByVal NewGuid As System.Guid) As Integer Implements IContentProvider.CloneContentByID

            OptionsManager.CloneByID(CloneID, ClonedID, NewName, NewGuid)

        End Function

        Public Overrides Sub BeforeContentDelete(ByVal ContentID As Integer)

            OptionsManager.BeforeContentDelete(ContentID)

        End Sub

        Public Overrides Function Save() As Integer

            OptionsManager.Save(Me)

        End Function

        Public Shared Function getGivingHistoryDataSource(ByVal ghf As GivingHistoryFields, ByVal bIncludeUnpaidEvents As Boolean) As IEnumerable(Of GivingHistoryDataRow)
            'WI 206814 - RyanBeg: Gifts__NextInstallmentDates will now return the next correct installment date.
            Dim ds = From gifts In ghf.Gifts _
            Group Join evt In ghf.Event_Linked_Gifts _
            On gifts.ID Equals evt.GiftsID _
            Into events = Group _
            From e2 In events.DefaultIfEmpty() _
            Group Join install In ghf.Installments _
            On gifts.ID Equals install.GiftsID _
            Into installments = Group
            From e In events.DefaultIfEmpty() _
            Group Join mem In ghf.Mem_Linked_Gifts _
            On gifts.ID Equals mem.GiftsID _
            Into memberships = Group _
            From m In memberships.DefaultIfEmpty() _
            Group Join lrp In ghf.LastRecurringPayment _
            On gifts.ID Equals lrp.GiftsID _
            Into lastRecPmnt = Group _
            From lastRecurringPmnt In lastRecPmnt.DefaultIfEmpty() _
            Order By CDate(gifts._Date) Descending _
            Select New GivingHistoryDataRow With { _
             .Appeals__AppealDescription = (From appls In ghf.Appeals Where appls.Gifts_ID = gifts.ID Select appls.Appeal_Description).FirstOrDefault, _
             .Gifts__Amount = gifts.Amount, _
             .Gifts__AmountUnformatted = gifts.AmountUnformatted, _
             .Gifts__Anonymous = gifts.Anonymous, _
             .Gifts__Balance = gifts.Balance, _
             .Gifts__BalanceUnformatted = gifts.BalanceUnformatted, _
             .Gifts__BbncCurrencyType = gifts.BbncCurrencyType, _
             .Campaigns__CampaignDescription = String.Join(";", (From campaigns In ghf.Campaigns Where campaigns.GiftsID = gifts.ID Select campaigns.CampaignDescription).ToArray()), _
             .Campaigns__CampaignID = String.Join(";", (From campaigns In ghf.Campaigns Where campaigns.GiftsID = gifts.ID Select campaigns.CampaignID).ToArray()), _
             .Gifts__Date = CDate(gifts._Date).ToShortDateString, _
             .Gifts__DonorName = gifts.DonorName, _
             .Gifts__Fund = gifts.Fund, _
             .Gifts__FundDescription = gifts.FundDescription, _
             .Gifts__GiftAidAmount = gifts.GiftAidAmount, _
             .Gifts__GiftAidAmountUnformatted = gifts.GiftAidAmountUnformatted, _
             .Gifts__GiftRecordID = gifts.GiftRecordID, _
             .Gifts__GiftTypeLanguageGUID = gifts.GiftTypeLanguageGUID, _
             .Gifts__InstallmentFrequency = gifts.InstallmentFrequency, _
             .Gifts__InstallmentSchedule = gifts.InstallmentSchedule, _
             .Gifts__IsSoftCredit = gifts.IsSoftCredit, _
             .Gifts__NumberOfInstallments = gifts.NumberOfInstallments, _
             .Gifts__NumberOfInstallmentsUnformatted = gifts.NumberOfInstallmentsUnformatted, _
             .Gifts__PayMethod = gifts.PayMethod, _
             .Gifts__Pending = StringLocalizer.BBString(If(gifts.Pending, "Yes", "No")), _
             .Gifts__ReceiptDate = gifts.ReceiptDate, _
             .Gifts__ReceiptKey = gifts.ReceiptKey, _
             .Gifts__ReceiptNumber = gifts.ReceiptNumber, _
             .Gifts__ReceiptAmount = gifts.ReceiptAmount, _
             .Gifts__REGiftID = gifts.REGiftID, _
             .Gifts__Subtype = gifts.Subtype, _
             .Gifts__Type = gifts.Type, _
             .ID = gifts.ID, _
             .Pending = gifts.Pending, _
             .TransactionType = If(e Is Nothing, If(m Is Nothing, If(Core.Data.GivingHistory.IsPledge(gifts), "Pledge", "Gift"), "Membership"), "Event"), _
             .Gifts__IsEFT = gifts.IsEFT, _
             .Gifts__IsActiveGift = gifts.IsActiveGift, _
             .Gifts__IsRecurringGift = gifts.IsRecurringGift, _
             .Gifts__AutoPayCCExpirationDate = gifts.AutoPayCCExpirationDate, _
             .Gifts__AutoPayCCPartialNumber = gifts.AutoPayCCPartialNumber, _
             .Gifts__AutoPayDDAcctDescription = gifts.AutoPayDDAcctDescription, _
             .Gifts__AutoPayDDAcctType = gifts.AutoPayDDAcctType, _
             .LastRecurringPayment_Amount = If(Not lastRecurringPmnt Is Nothing, lastRecurringPmnt.Amount, ""), _
             .LastRecurringPayment_Date = If(Not lastRecurringPmnt Is Nothing, lastRecurringPmnt.DatePaid, ""), _
             .Gifts__AutoPayDDAcctID = If(Not gifts.AutoPayDDAcctID = Guid.Empty, gifts.AutoPayDDAcctID.ToString, ""), _
             .Gifts__AutoPayDDAcctPartialNumber = gifts.AutoPayDDAcctPartialNumber, _
             .Gifts__AutoPayCCToken = If(Not gifts.AutoPayCCToken = Guid.Empty, gifts.AutoPayCCToken.ToString, ""), _
             .RecurringGiftEndDate = gifts.RecurringGiftEndDate, _
             .RecurringGiftStartDate = gifts.RecurringGiftStartDate, _
             .Gifts__IsEditableRecurringGift = gifts.IsEditableRecurringGift, _
             .Gifts__RecurringGiftNextTransactionDate = gifts.RecurringGiftNextTransactionDate, _
             .Gifts__FrequencyCode = gifts.FrequencyCode, _
             .Funds__FundID = (From funds In ghf.Funds Where funds.GiftsID = gifts.ID Select funds.FundID).FirstOrDefault, _
             .Gifts__SiteHierarchy = gifts.SiteHierarchy, _
             .Event_participations__EventName = String.Join(", ", (From eventParticipants In ghf.Event_participations Where eventParticipants.GiftRecordID = gifts.GiftRecordID Select eventParticipants.EventName).Distinct.ToArray), _
             .Event_participations__EventDetails = BuildEventDetailsByGift(ghf, gifts.BbncCurrencyType, gifts.GiftRecordID, .Event_participations__EventRegistrantID), _
             .Gifts__FundraisingPurpose = gifts.FundraisingPurpose, _
             .Gifts__CurrencyISO = (From currency In ghf.GiftCurrency Where currency.GiftsID = gifts.ID Select currency.ISO4217).FirstOrDefault, _
             .Gifts__PaymentCurrencyAcceptable = (From giftconditionsettings In ghf.GiftConditionSettings Where giftconditionsettings.GiftsID = gifts.ID Select giftconditionsettings.PaymentCurrencyAcceptable).FirstOrDefault, _
             .Gifts__NextInstallmentDates = String.Join(";", (From install In installments Where Decimal.Parse(install.Balance, Globalization.NumberStyles.Currency) <> 0 Select install.InstallmentDate).FirstOrDefault()), _
            .Gifts__LastInstallmentDate = (From installs In ghf.Installments Where gifts.ID = installs.GiftsID Select installs.InstallmentDate).LastOrDefault _
            }

            ' TODO: Derive a field indicating if the gift is a payment to a pledge
            ' TODO: I believe the .TransactionType property will fail if gifts.Type is null...we should ensure it's not nullable (unit test on dataset?)

            ' TODO:
            ' Types that might need more info:
            ' pledges, pledge payments

            ' TODO: Does Balance apply to anything besides pledges?
            '       Need to remove the $0 from all balances that wouldn't have a value
            If bIncludeUnpaidEvents Then
                Dim totalPrice As Decimal
                Dim totalBalance As Decimal
                ds = ds.Union( _
                From participation In ghf.Event_participations _
                Where participation.GiftRecordID = Guid.Empty.ToString _
                Group participation By participation.HostRegistrantID Into Group _
                Select New GivingHistoryDataRow With { _
                .Appeals__AppealDescription = String.Empty, _
                .Gifts__Anonymous = "False", _
                .Gifts__BbncCurrencyType = Core.MerchantAccountBBPS.BLL.MATransactionInfo.InstalledCurrencyType(), _
                .Campaigns__CampaignDescription = String.Empty, _
                .Campaigns__CampaignID = String.Empty, _
                .Gifts__Date = String.Empty, _
                .Gifts__DonorName = String.Empty, _
                .Gifts__Fund = String.Empty, _
                .Gifts__FundDescription = String.Empty, _
                .Gifts__GiftAidAmount = String.Empty, _
                .Gifts__GiftAidAmountUnformatted = 0, _
                .Gifts__GiftRecordID = String.Empty, _
                .Gifts__GiftTypeLanguageGUID = Guid.Empty, _
                .Gifts__InstallmentFrequency = String.Empty, _
                .Gifts__InstallmentSchedule = String.Empty, _
                .Gifts__IsSoftCredit = False, _
                .Gifts__NumberOfInstallments = String.Empty, _
                .Gifts__NumberOfInstallmentsUnformatted = 0, _
                .Gifts__PayMethod = String.Empty, _
                .Gifts__Pending = "Yes", _
                .Gifts__ReceiptDate = String.Empty, _
                .Gifts__ReceiptKey = String.Empty, _
                .Gifts__ReceiptNumber = String.Empty, _
                .Gifts__ReceiptAmount = String.Empty, _
                .Gifts__REGiftID = String.Empty, _
                .Gifts__Subtype = String.Empty, _
                .Gifts__Type = String.Empty, _
                .ID = Guid.Empty, _
                .Pending = True, _
                .TransactionType = "Event", _
                .Gifts__IsEFT = False, _
                .Gifts__IsActiveGift = True, _
                .Gifts__IsRecurringGift = False, _
                .Gifts__AutoPayCCExpirationDate = String.Empty, _
                .Gifts__AutoPayCCPartialNumber = String.Empty, _
                .Gifts__AutoPayDDAcctDescription = String.Empty, _
                .Gifts__AutoPayDDAcctType = String.Empty, _
                .LastRecurringPayment_Amount = String.Empty, _
                .LastRecurringPayment_Date = String.Empty, _
                .Gifts__AutoPayDDAcctID = String.Empty, _
                .Gifts__AutoPayDDAcctPartialNumber = String.Empty, _
                .Gifts__AutoPayCCToken = String.Empty, _
                .RecurringGiftEndDate = String.Empty, _
                .RecurringGiftStartDate = String.Empty, _
                .Gifts__IsEditableRecurringGift = False, _
                .Gifts__RecurringGiftNextTransactionDate = String.Empty, _
                .Gifts__FrequencyCode = 0, _
                .Funds__FundID = String.Empty, _
                .Gifts__SiteHierarchy = String.Empty, _
                .Event_participations__EventName = String.Join(", ", (From eventParticipants In ghf.Event_participations Where eventParticipants.HostRegistrantID = HostRegistrantID Select eventParticipants.EventName).Distinct.ToArray), _
                .Event_participations__EventDetails = BuildEventDetailsByGuest(ghf, Core.MerchantAccountBBPS.BLL.MATransactionInfo.InstalledCurrencyType(), HostRegistrantID), _
                .Gifts__CurrencyISO = (From ep In ghf.Event_participations Join ec In ghf.EventCurrency On ep.EventRecordID Equals ec.EventRecordID _
                                                      Where ep.HostRegistrantID = HostRegistrantID Select ec.ISO4217).FirstOrDefault, _
                .Event_participations__EventRegistrantID = New Guid(HostRegistrantID), _
                .Event_participations__RegistrationID = New Guid((From eventParticipants In ghf.Event_participations Where eventParticipants.HostRegistrantID = HostRegistrantID Select eventParticipants.RegistrationID).Distinct.ToArray(0)), _
                .Gifts__Balance = BuildAndFormatEventBalance(HostRegistrantID, totalBalance, totalPrice, _
                                (From ep In ghf.Event_participations Join ec In ghf.EventCurrency On ep.EventRecordID Equals ec.EventRecordID _
                                 Where ep.HostRegistrantID = HostRegistrantID Select ec.ISO4217).FirstOrDefault), _
                .Gifts__BalanceUnformatted = totalBalance, _
                .Gifts__Amount = FormatEventGitAmount(totalPrice, _
                                (From ep In ghf.Event_participations Join ec In ghf.EventCurrency On ep.EventRecordID Equals ec.EventRecordID _
                                 Where ep.HostRegistrantID = HostRegistrantID Select ec.ISO4217).FirstOrDefault), _
                .Gifts__AmountUnformatted = totalPrice, _
                .Gifts__NextInstallmentDates = String.Empty, _
                .Gifts__LastInstallmentDate = String.Empty} _
                )
            End If

            Return ds
        End Function

        Private Shared Function FormatEventGitAmount(ByVal amount As Decimal, ByVal currencyISO As String) As String
            If PortalSettings.Current.Features.MulticurrencyConditionSettingExists() Then
                Return Core.Internationalization.Currency.FormatCurrency(amount, currencyISO)
            Else
                Return BBFormatCurrency(amount.ToString, Core.MerchantAccountBBPS.BLL.MATransactionInfo.InstalledCurrencyType())
            End If
        End Function

        Private Shared Function BuildAndFormatEventBalance(ByVal hostID As String, ByRef balance As Decimal, ByRef totalPrice As Decimal, ByVal currencyISO As String) As String
            Dim balanceUnformatted As Decimal = BuildEventBalance(hostID, balance, totalPrice)

            If PortalSettings.Current.Features.MulticurrencyConditionSettingExists() Then
                Return Core.Internationalization.Currency.FormatCurrency(balanceUnformatted, currencyISO)
            Else
                Return BBFormatCurrency(balanceUnformatted, Core.MerchantAccountBBPS.BLL.MATransactionInfo.InstalledCurrencyType())
            End If
        End Function

        Private Shared Function BuildEventBalance(ByVal hostID As String, ByRef balance As Decimal, ByRef totalPrice As Decimal) As Decimal

            Dim balanceList = DataLists.Event.EventRegistrationBalanceByRegistrantDataList.GetRows(Core.AppFx.ServiceProvider.Current, hostID)
            If balanceList IsNot Nothing AndAlso balanceList.Count > 0 Then
                balance = balanceList(0).FEEBALANCE
                totalPrice = balanceList(0).TOTALFEES
            End If

            Return balance
        End Function

        Private Shared Function BuildEventDetailsByGift(ByVal ghf As GivingHistoryFields, ByVal eCurrencyType As Integer, ByVal giftID As String, ByRef registrantID As Guid) As String
            Dim participants = (From eventParticipants In ghf.Event_participations Where eventParticipants.GiftRecordID = giftID Select eventParticipants.ParticipantName, eventParticipants.TicketType, eventParticipants.TicketPrice, eventParticipants.HostRegistrantID).Distinct

            Dim participantDetails As String = String.Empty

            If participants.Count > 0 Then
                For Each participant In participants
                    Dim ticketPrice As String = participant.TicketPrice
                    If Not PortalSettings.Current.Features.MulticurrencyConditionSettingExists() Then
                        ticketPrice = BBFormatCurrency(participant.TicketPrice, DirectCast(eCurrencyType, Core.Data.GiftInformation.eCurrencyType))
                    End If
                    participantDetails = String.Concat(participantDetails, participant.ParticipantName, "(", participant.TicketType, " - ", ticketPrice, "), ")
                Next
                registrantID = New Guid(participants(0).HostRegistrantID)
                participantDetails = participantDetails.Substring(0, participantDetails.Length - 2)
            End If

            Return participantDetails
        End Function

        Private Shared Function BuildEventDetailsByGuest(ByVal ghf As GivingHistoryFields, ByVal eCurrencyType As Integer, ByVal hostID As String) As String
            Dim participants = (From eventParticipants In ghf.Event_participations Where eventParticipants.HostRegistrantID = hostID Select eventParticipants.ParticipantName, eventParticipants.TicketType, eventParticipants.TicketPrice).Distinct

            Dim participantDetails As String = String.Empty

            If participants.Count > 0 Then
                For Each participant In participants
                    Dim ticketPrice As String = participant.TicketPrice
                    If Not PortalSettings.Current.Features.MulticurrencyConditionSettingExists() Then
                        ticketPrice = BBFormatCurrency(participant.TicketPrice, DirectCast(eCurrencyType, Core.Data.GiftInformation.eCurrencyType))
                    End If
                    participantDetails = String.Concat(participantDetails, participant.ParticipantName, "(", participant.TicketType, " - ", ticketPrice, "), ")
                Next

                participantDetails = participantDetails.Substring(0, participantDetails.Length - 2)
            End If

            Return participantDetails
        End Function

        'We need to implement IEqualityComparer to make the groupby happy with our 
        'This private class lets the grouping leverage the comparison code already in
        'place for sorting.
        Private Class GivingHistoryEqualityCompare
            Implements System.Collections.Generic.IEqualityComparer(Of Object)

            Public Comparer As GivingHistoryDataFieldComparer
            'Special Case dates into year
            Public GroupByYear As Boolean

            Public Function Equals1(ByVal x As Object, ByVal y As Object) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of Object).Equals
                If GroupByYear Then
                    Return CType(x, Integer) = CType(y, Integer)
                Else
                    Return If(Me.Comparer(x, y) = 0, True, False)
                End If
            End Function

            Public Function GetHashCode1(ByVal obj As Object) As Integer Implements System.Collections.Generic.IEqualityComparer(Of Object).GetHashCode
                Return obj.GetHashCode
            End Function
        End Class

        Public Function Group(ByVal groupColumn As Integer, ByVal query As IEnumerable(Of GivingHistoryDataRow)) As System.Collections.Generic.IEnumerable(Of System.Linq.IGrouping(Of Object, GivingHistoryDataRow))
            'No one should group on an invalid colunn number but just in case
            If groupColumn < 0 OrElse groupColumn >= Me.UsedFields.Columns.Count Then
                Throw New ArgumentException("Grouping Column Invalid.")
            End If

            Dim l = query.ToList
            Dim col = Me.UsedFields.Columns(groupColumn)
            Dim fieldName = GivingHistoryDataRow.GenerateDataField(col.TableName, col.ColumnName)

            Dim fieldComparer As GivingHistoryDataFieldComparer

            Select Case col.ColumnSortType
                Case GivingHistoryColumn.SortType.DateTime
                    fieldComparer = AddressOf GivingHistoryDataFieldDateComparer
                Case GivingHistoryColumn.SortType.Numeric
                    fieldComparer = AddressOf GivingHistoryDataFieldNumericComparer
                Case GivingHistoryColumn.SortType.String
                    fieldComparer = AddressOf GivingHistoryDataFieldStringComparer
                Case GivingHistoryColumn.SortType.Currency
                    fieldComparer = AddressOf GivingHistoryDataFieldCurrencyComparer
            End Select


            Dim grps As System.Collections.Generic.IEnumerable(Of System.Linq.IGrouping(Of Object, GivingHistoryDataRow))

            If col.ColumnSortType <> GivingHistoryColumn.SortType.DateTime Then
                'We use the SortDataField here to get the correct key object field from the row
                'See commends above on the GivingHistoryEqualityCompare object.  We need a wrapper
                'so that groupby gets an equality compare and the types all check out
                'but we want to leverage the generic comparators already in place for the sorting
                grps = l.GroupBy(Function(fr) fr.SortDataField(fieldName), New GivingHistoryEqualityCompare With {.Comparer = fieldComparer})
            Else
                'Grouping dates is a special case.  We want to group by year only
                grps = l.GroupBy(Function(fr) If(Not String.IsNullOrEmpty(DirectCast(fr.SortDataField(fieldName), String)), Date.Parse(DirectCast(fr.SortDataField(fieldName), String)).Year, Date.Today.Year), New GivingHistoryEqualityCompare With {.GroupByYear = True})
            End If

            Return grps

        End Function

        Public Function Sort(ByVal sortcolumn As Nullable(Of Integer), ByVal sortacsending As Boolean, ByVal query As IEnumerable(Of GivingHistoryDataRow)) As IList(Of GivingHistoryDataRow)
            Dim l = query.ToList()

            If sortcolumn.HasValue AndAlso sortcolumn.Value < Me.UsedFields.Columns.Count Then
                Dim col = Me.UsedFields.Columns(sortcolumn.Value)
                Dim fieldName = GivingHistoryDataRow.GenerateDataField(col.TableName, col.ColumnName)

                Dim fieldComparer As GivingHistoryDataFieldComparer

                Select Case col.ColumnSortType
                    Case GivingHistoryColumn.SortType.DateTime
                        fieldComparer = AddressOf GivingHistoryDataFieldDateComparer
                    Case GivingHistoryColumn.SortType.Numeric
                        fieldComparer = AddressOf GivingHistoryDataFieldNumericComparer
                    Case GivingHistoryColumn.SortType.String
                        fieldComparer = AddressOf GivingHistoryDataFieldStringComparer
                    Case GivingHistoryColumn.SortType.Currency
                        fieldComparer = AddressOf GivingHistoryDataFieldCurrencyComparer
                    Case Else
                        Throw New ArgumentException("Unsupported ColumnSortType.")
                End Select

                l.Sort(Function(lhs, rhs) GivingHistoryDataRowComparer(fieldName, lhs, rhs, sortacsending, fieldComparer))
            End If

            Return l
        End Function

        Public Function FilterByFund(ByVal selectedfund As String, ByVal query As IEnumerable(Of GivingHistoryDataRow)) As IEnumerable(Of GivingHistoryDataRow)
            query = From fund In Me.GivingHistoryData.Funds Join ghdr In query On fund.GiftsID Equals ghdr.ID Where fund.Description.Contains(Trim(selectedfund)) Select ghdr
            Return query
        End Function

        Public Function FilterByDate(ByVal startdate As DateTime, ByVal enddate As DateTime, ByVal query As IEnumerable(Of GivingHistoryDataRow)) As IEnumerable(Of GivingHistoryDataRow)
            query = From ghdr In query Where If(Not String.IsNullOrEmpty(ghdr.Gifts__Date), CDate(ghdr.Gifts__Date), Date.Today) >= startdate AndAlso If(Not String.IsNullOrEmpty(ghdr.Gifts__Date), CDate(ghdr.Gifts__Date), Date.Today) <= enddate Select ghdr
            Return query
        End Function

#Region "GivingHistory Data Comparers"

        Private Delegate Function GivingHistoryDataFieldComparer(ByVal lhs As Object, ByVal rhs As Object) As Integer

        Private Shared Function IsEmptyField(ByVal fieldValue As Object) As Boolean
            If TypeOf fieldValue Is Decimal Then
                ' Minimum decimals are our convention for an "Empty" decimal field
                Return Decimal.Equals(Decimal.MinValue, DirectCast(fieldValue, Decimal))
            Else
                Return String.IsNullOrEmpty(Convert.ToString(fieldValue))
            End If
        End Function

        Private Shared Function GivingHistoryDataRowComparer(ByVal fieldName As String, ByVal lhs As GivingHistoryDataRow, ByVal rhs As GivingHistoryDataRow, ByVal ascending As Boolean, ByVal dataFieldComparer As GivingHistoryDataFieldComparer) As Integer
            Dim lhsVal = lhs.SortDataField(fieldName)
            Dim rhsVal = rhs.SortDataField(fieldName)

            Dim isLHSEmpty As Boolean = IsEmptyField(lhsVal)
            Dim isRHSEmpty As Boolean = IsEmptyField(rhsVal)

            ' Empty fields go to the end no matter what when sorting
            If isLHSEmpty AndAlso isRHSEmpty Then
                Return 0
            ElseIf isLHSEmpty Then
                Return 1
            ElseIf isRHSEmpty Then
                Return -1
            End If

            If ascending Then
                Return dataFieldComparer(lhsVal, rhsVal)
            Else
                Return dataFieldComparer(rhsVal, lhsVal)
            End If
        End Function

        Private Shared Function GivingHistoryDataFieldDateComparer(ByVal lhs As Object, ByVal rhs As Object) As Integer
            Return Date.Compare(Convert.ToDateTime(lhs), Convert.ToDateTime(rhs))
        End Function

        Private Shared Function GivingHistoryDataFieldNumericComparer(ByVal lhs As Object, ByVal rhs As Object) As Integer
            'sterling WI 121335
            'Convert.ToDecimal throws exceptions on empty string
            'changed to Decimal.TryParse
            Dim leftHandNumber, rightHandNumber As Decimal

            Decimal.TryParse(CStr(lhs), leftHandNumber)
            Decimal.TryParse(CStr(rhs), rightHandNumber)

            Return Decimal.Compare(leftHandNumber, rightHandNumber)
        End Function

        Private Shared Function GivingHistoryDataFieldStringComparer(ByVal lhs As Object, ByVal rhs As Object) As Integer
            Return String.Compare(Trim(Convert.ToString(lhs)), Trim(Convert.ToString(rhs)))
        End Function

        Private Shared Function GivingHistoryDataFieldCurrencyComparer(ByVal lhs As Object, ByVal rhs As Object) As Integer
            Return Decimal.Compare(SafeCDecl(lhs), SafeCDecl(rhs))
        End Function
#End Region

    End Class

End Namespace
