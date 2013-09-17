Public Class GivingHistoryDataRow

	' {0} = Table name
	' {1} = Column name
	' Property name syntax for DataBinding
	Private Const FIELD_FORMAT As String = "{0}__{1}"

    Sub New()
        ' TODO: Complete member initialization 
    End Sub

	Public Shared Function GenerateDataField(ByVal tableName As String, ByVal columnName As String) As String
		Return String.Format(FIELD_FORMAT, tableName, columnName)
	End Function

	' Yes, this gets any property...but it's intended for fields and it's not API code.
	' So rather than ensure that it's one of the field types, let's just not misuse it.
	' Fields are Properties whose format match GenerateDataField()
	Public ReadOnly Property SortDataField(ByVal displayedFieldName As String) As Object
		Get
			Dim sortedFieldName As String

			Select Case displayedFieldName
				Case GenerateDataField("Gifts", "Amount")
					sortedFieldName = GenerateDataField("Gifts", "AmountUnformatted")
				Case GenerateDataField("Gifts", "Balance")
					sortedFieldName = GenerateDataField("Gifts", "BalanceUnformatted")
				Case GenerateDataField("Gifts", "GiftAidAmount")
                    sortedFieldName = GenerateDataField("Gifts", "GiftAidAmountUnformatted")
                Case GenerateDataField("Gifts", "NumberOfInstallments")
                    sortedFieldName = GenerateDataField("Gifts", "NumberOfInstallmentsUnformatted")
                Case Else
                    sortedFieldName = displayedFieldName
            End Select

			Return Me.GetType().GetProperty(sortedFieldName).GetValue(Me, Nothing)
		End Get
	End Property

	Public ReadOnly Property IsPledge() As Boolean
		Get
			Return Not String.IsNullOrEmpty(Me.Gifts__Balance)
		End Get
	End Property

	Private _campaigns_CampaignDescription As String
	Public Property Campaigns__CampaignDescription() As String
		Get
			Return _campaigns_CampaignDescription
		End Get
		Set(ByVal value As String)
			_campaigns_CampaignDescription = value
		End Set
	End Property

	Private _campaigns_CampaignID As String
	Public Property Campaigns__CampaignID() As String
		Get
			Return _campaigns_CampaignID
		End Get
		Set(ByVal value As String)
			_campaigns_CampaignID = value
		End Set
	End Property

	Private _gifts__Amount As String
	Public Property Gifts__Amount() As String
		Get
			Return _gifts__Amount
		End Get
		Set(ByVal value As String)
			_gifts__Amount = value
		End Set
	End Property

	Private _gifts__AmountUnformatted As Decimal
	Public Property Gifts__AmountUnformatted() As Decimal
		Get
			Return _gifts__AmountUnformatted
		End Get
		Set(ByVal value As Decimal)
			_gifts__AmountUnformatted = value
		End Set
	End Property

	Private _gifts__Anonymous As String
	Public Property Gifts__Anonymous() As String
		Get
			Return _gifts__Anonymous
		End Get
		Set(ByVal value As String)
			_gifts__Anonymous = value
		End Set
	End Property

	Private _appeals_AppealDescription As String
	Public Property Appeals__AppealDescription() As String
		Get
			Return _appeals_AppealDescription
		End Get
		Set(ByVal value As String)
			_appeals_AppealDescription = value
		End Set
	End Property

	Private _gifts__DonorName As String
	Public Property Gifts__DonorName() As String
		Get
			Return _gifts__DonorName
		End Get
		Set(ByVal value As String)
			_gifts__DonorName = value
		End Set
	End Property

	Private _gifts__Subtype As String
	Public Property Gifts__Subtype() As String
		Get
			Return _gifts__Subtype
		End Get
		Set(ByVal value As String)
			_gifts__Subtype = value
		End Set
	End Property

	Private _gifts__Balance As String
	Public Property Gifts__Balance() As String
		Get
			Return _gifts__Balance
		End Get
		Set(ByVal value As String)
			_gifts__Balance = value
		End Set
	End Property

	Private _gifts__BalanceUnformatted As Decimal
	Public Property Gifts__BalanceUnformatted() As Decimal
		Get
			Return _gifts__BalanceUnformatted
		End Get
		Set(ByVal value As Decimal)
			_gifts__BalanceUnformatted = value
		End Set
	End Property

	Private _gifts__BbncCurrencyType As Int32
	Public Property Gifts__BbncCurrencyType() As Int32
		Get
			Return _gifts__BbncCurrencyType
		End Get
		Set(ByVal value As Int32)
			_gifts__BbncCurrencyType = value
		End Set
	End Property

	Private _gifts__IsSoftCredit As Boolean
	Public Property Gifts__IsSoftCredit() As Boolean
		Get
			Return _gifts__IsSoftCredit
		End Get
		Set(ByVal value As Boolean)
			_gifts__IsSoftCredit = value
		End Set
	End Property

	Private _gifts__Campaign As String
	Public Property Gifts__Campaign() As String
		Get
			Return _gifts__Campaign
		End Get
		Set(ByVal value As String)
			_gifts__Campaign = value
		End Set
	End Property

	Private _gifts_Date As String
	Public Property Gifts__Date() As String
		Get
			Return _gifts_Date
		End Get
		Set(ByVal value As String)
			_gifts_Date = value
		End Set
	End Property

	Private _gifts__Fund As String
	Public Property Gifts__Fund() As String
		Get
			Return _gifts__Fund
		End Get
		Set(ByVal value As String)
			_gifts__Fund = value
		End Set
	End Property

	Private _gifts__FundDescription As String
	Public Property Gifts__FundDescription() As String
		Get
			Return _gifts__FundDescription
		End Get
		Set(ByVal value As String)
			_gifts__FundDescription = value
		End Set
	End Property

	Private _gifts__GiftAidAmount As String
	Public Property Gifts__GiftAidAmount() As String
		Get
			Return _gifts__GiftAidAmount
		End Get
		Set(ByVal value As String)
			_gifts__GiftAidAmount = value
		End Set
	End Property

	Private _gifts__GiftAidAmountUnformatted As Decimal
	Public Property Gifts__GiftAidAmountUnformatted() As Decimal
		Get
			Return _gifts__GiftAidAmountUnformatted
		End Get
		Set(ByVal value As Decimal)
			_gifts__GiftAidAmountUnformatted = value
		End Set
	End Property

	Private _gifts__InstallmentFrequency As String
	Public Property Gifts__InstallmentFrequency() As String
		Get
			Return _gifts__InstallmentFrequency
		End Get
		Set(ByVal value As String)
			_gifts__InstallmentFrequency = value
		End Set
	End Property


	Private _gifts__InstallmentSchedule As String
	Public Property Gifts__InstallmentSchedule() As String
		Get
			Return _gifts__InstallmentSchedule
		End Get
		Set(ByVal value As String)
			_gifts__InstallmentSchedule = value
		End Set
	End Property


	Private _gifts__NumberOfInstallments As String
	Public Property Gifts__NumberOfInstallments() As String
		Get
			Return _gifts__NumberOfInstallments
		End Get
		Set(ByVal value As String)
			_gifts__NumberOfInstallments = value
		End Set
	End Property

    Private _gifts__NumberOfInstallmentsUnformatted As Integer
    Public Property Gifts__NumberOfInstallmentsUnformatted() As Integer
        Get
            Return _gifts__NumberOfInstallmentsUnformatted
        End Get
        Set(ByVal value As Integer)
            _gifts__NumberOfInstallmentsUnformatted = value
        End Set
    End Property

	Private _gifts__PayMethod As String
	Public Property Gifts__PayMethod() As String
		Get
			Return _gifts__PayMethod
		End Get
		Set(ByVal value As String)
			_gifts__PayMethod = value
		End Set
	End Property


	Private _gifts__Pending As String
	Public Property Gifts__Pending() As String
		Get
			Return _gifts__Pending
		End Get
		Set(ByVal value As String)
			_gifts__Pending = value
		End Set
	End Property


	Private _gifts__ReceiptDate As String
	Public Property Gifts__ReceiptDate() As String
		Get
			Return _gifts__ReceiptDate
		End Get
		Set(ByVal value As String)
			_gifts__ReceiptDate = value
		End Set
	End Property

	Private _gifts__ReceiptKey As String
	Public Property Gifts__ReceiptKey() As String
		Get
			Return _gifts__ReceiptKey
		End Get
		Set(ByVal value As String)
			_gifts__ReceiptKey = value
		End Set
	End Property


	Private _gifts__ReceiptNumber As String
	Public Property Gifts__ReceiptNumber() As String
		Get
			Return _gifts__ReceiptNumber
		End Get
		Set(ByVal value As String)
			_gifts__ReceiptNumber = value
		End Set
    End Property

    Private _gifts__ReceiptAmount As String
    Public Property Gifts__ReceiptAmount() As String
        Get
            Return _gifts__ReceiptAmount
        End Get
        Set(value As String)
            _gifts__ReceiptAmount = value
        End Set
    End Property


	Private _gifts__REGiftID As String
	Public Property Gifts__REGiftID() As String
		Get
			Return _gifts__REGiftID
		End Get
		Set(ByVal value As String)
			_gifts__REGiftID = value
		End Set
	End Property

	Private _gifts__GiftRecordID As String
	Public Property Gifts__GiftRecordID() As String
		Get
			Return _gifts__GiftRecordID
		End Get
		Set(ByVal value As String)
			_gifts__GiftRecordID = value
		End Set
	End Property

	Private _gifts__SiteHierarchy As String
	Public Property Gifts__SiteHierarchy() As String
		Get
			Return _gifts__SiteHierarchy
		End Get
		Set(ByVal value As String)
			_gifts__SiteHierarchy = value
		End Set
	End Property

	Private _gifts__Type As String
	Public Property Gifts__Type() As String
		Get
			Return _gifts__Type
		End Get
		Set(ByVal value As String)
			_gifts__Type = value
		End Set
	End Property

	Private _gifts__GiftTypeLanguageGUID As Guid
	Public Property Gifts__GiftTypeLanguageGUID() As Guid
		Get
			Return _gifts__GiftTypeLanguageGUID
		End Get
		Set(ByVal value As Guid)
			_gifts__GiftTypeLanguageGUID = value
		End Set
	End Property


	Private _ID As Guid
	Public Property ID() As Guid
		Get
			Return _ID
		End Get
		Set(ByVal value As Guid)
			_ID = value
		End Set
	End Property


	Private _pending As Boolean
	Public Property Pending() As Boolean
		Get
			Return _pending
		End Get
		Set(ByVal value As Boolean)
			_pending = value
		End Set
	End Property


	Private _transactionType As String
	Public Property TransactionType() As String
		Get
			Return _transactionType
		End Get
		Set(ByVal value As String)
			_transactionType = value
		End Set
	End Property


	Private _gifts__IsEFT As Boolean
	Public Property Gifts__IsEFT() As Boolean
		Get
			Return _gifts__IsEFT
		End Get
		Set(ByVal value As Boolean)
			_gifts__IsEFT = value
		End Set
	End Property

	Private _gifts__OriginalPledgeID As String
	Public Property Gifts__OriginalPledgeID() As String
		Get
			Return _gifts__OriginalPledgeID
		End Get
		Set(ByVal value As String)
			_gifts__OriginalPledgeID = value
		End Set
	End Property

	Private _gifts__IsActiveGift As Boolean
	Public Property Gifts__IsActiveGift() As Boolean
		Get
			Return _gifts__IsActiveGift
		End Get
		Set(ByVal value As Boolean)
			_gifts__IsActiveGift = value
		End Set
	End Property

	Private _gifts__IsRecurringGift As Boolean
	Public Property Gifts__IsRecurringGift() As Boolean
		Get
			Return _gifts__IsRecurringGift
		End Get
		Set(ByVal value As Boolean)
			_gifts__IsRecurringGift = value
		End Set
	End Property

	Private _gifts__AutoPayCCPartialNumber As String
	Public Property Gifts__AutoPayCCPartialNumber() As String
		Get
			Return _gifts__AutoPayCCPartialNumber
		End Get
		Set(ByVal value As String)
			_gifts__AutoPayCCPartialNumber = value
		End Set
	End Property

	Private _gifts__AutoPayCCExpirationDate As String
	Public Property Gifts__AutoPayCCExpirationDate() As String
		Get
			Return _gifts__AutoPayCCExpirationDate
		End Get
		Set(ByVal value As String)
			_gifts__AutoPayCCExpirationDate = value
		End Set
	End Property

	Private _gifts__AutoPayDDAcctDescription As String
	Public Property Gifts__AutoPayDDAcctDescription() As String
		Get
			Return _gifts__AutoPayDDAcctDescription
		End Get
		Set(ByVal value As String)
			_gifts__AutoPayDDAcctDescription = value
		End Set
	End Property

	Private _gifts__AutoPayDDAcctType As String
	Public Property Gifts__AutoPayDDAcctType() As String
		Get
			Return _gifts__AutoPayDDAcctType
		End Get
		Set(ByVal value As String)
			_gifts__AutoPayDDAcctType = value
		End Set
	End Property

	Private _LastRecurringPayment_Amount As String
	Public Property LastRecurringPayment_Amount() As String
		Get
			Return _LastRecurringPayment_Amount
		End Get
		Set(ByVal value As String)
			_LastRecurringPayment_Amount = value
		End Set
	End Property

	Private _LastRecurringPayment_Date As String
	Public Property LastRecurringPayment_Date() As String
		Get
			Return _LastRecurringPayment_Date
		End Get
		Set(ByVal value As String)
			_LastRecurringPayment_Date = value
		End Set
	End Property

	Private _gifts_AutoPayDDAcctPartialNumber As String
	Public Property Gifts__AutoPayDDAcctPartialNumber() As String
		Get
			Return _gifts_AutoPayDDAcctPartialNumber
		End Get
		Set(ByVal value As String)
			_gifts_AutoPayDDAcctPartialNumber = value
		End Set
	End Property

	Private _gifts_AutoPayDDAcctID As String
	Public Property Gifts__AutoPayDDAcctID() As String
		Get
			Return _gifts_AutoPayDDAcctID
		End Get
		Set(ByVal value As String)
			_gifts_AutoPayDDAcctID = value
		End Set
	End Property

	Private _gifts_AutoPayCCToken As String
	Public Property Gifts__AutoPayCCToken() As String
		Get
			Return _gifts_AutoPayCCToken
		End Get
		Set(ByVal value As String)
			_gifts_AutoPayCCToken = value
		End Set
	End Property

	Private _RecurringGiftStartDate As String
	Public Property RecurringGiftStartDate() As String
		Get
			Return _RecurringGiftStartDate
		End Get
		Set(ByVal value As String)
			_RecurringGiftStartDate = value
		End Set
	End Property

	Private _RecurringGiftEndDate As String
	Public Property RecurringGiftEndDate() As String
		Get
			Return _RecurringGiftEndDate
		End Get
		Set(ByVal value As String)
			_RecurringGiftEndDate = value
		End Set
	End Property

	Private _gifts__IsEditableRecurringGift As Boolean
	Public Property Gifts__IsEditableRecurringGift() As Boolean
		Get
			Return _gifts__IsEditableRecurringGift
		End Get
		Set(ByVal value As Boolean)
			_gifts__IsEditableRecurringGift = value
		End Set
	End Property

	Private _gifts__RecurringGiftNextTransactionDate As String
	Public Property Gifts__RecurringGiftNextTransactionDate() As String
		Get
			Return _gifts__RecurringGiftNextTransactionDate
		End Get
		Set(ByVal value As String)
			_gifts__RecurringGiftNextTransactionDate = value
		End Set
	End Property

	Private _gifts__FrequencyCode As Integer
	Public Property Gifts__FrequencyCode() As Integer
		Get
			Return _gifts__FrequencyCode
		End Get
		Set(ByVal value As Integer)
			_gifts__FrequencyCode = value
		End Set
	End Property

	Private _gifts_FundraisingPurpose As String
    Public Property Gifts__FundraisingPurpose() As String
        Get
            Return _gifts_FundraisingPurpose
        End Get
        Set(ByVal value As String)
            _gifts_FundraisingPurpose = value
        End Set
    End Property

	Private _funds_FundID As String
	Public Property Funds__FundID() As String
		Get
			Return _funds_FundID
		End Get
		Set(ByVal value As String)
			_funds_FundID = value
		End Set
	End Property

	Private _Event_participations_EventName As String
	Public Property Event_participations__EventName() As String
		Get
			If Not String.IsNullOrEmpty(_Event_participations_EventName) Then
				Return _Event_participations_EventName
			Else
				Return String.Empty
			End If
		End Get
		Set(ByVal value As String)
			_Event_participations_EventName = value
		End Set
	End Property

	Private _Event_participations_EventDetails As String
	Public Property Event_participations__EventDetails() As String
		Get
			Return _Event_participations_EventDetails
		End Get
		Set(ByVal value As String)
			_Event_participations_EventDetails = value
		End Set
    End Property

    Private _Event_participations_EventRegistrantID As Guid
    Public Property Event_participations__EventRegistrantID() As Guid
        Get
            Return _Event_participations_EventRegistrantID
        End Get
        Set(ByVal value As Guid)
            _Event_participations_EventRegistrantID = value
        End Set
    End Property

	Private _Event_participations__RegistrationID As Guid
	Public Property Event_participations__RegistrationID() As Guid
		Get
			Return _Event_participations__RegistrationID
		End Get
		Set(ByVal value As Guid)
			_Event_participations__RegistrationID = value
		End Set
    End Property

    Private _gifts__CurrencyISO As String
    Public Property Gifts__CurrencyISO() As String
        Get
            Return _gifts__CurrencyISO
        End Get
        Set(ByVal value As String)
            _gifts__CurrencyISO = value
        End Set
    End Property

    Private _gifts__PaymentCurrencyAcceptable As Boolean
    Public Property Gifts__PaymentCurrencyAcceptable() As Boolean
        Get
            Return _gifts__PaymentCurrencyAcceptable
        End Get
        Set(ByVal value As Boolean)
            _gifts__PaymentCurrencyAcceptable = value
        End Set
    End Property

    Private _gifts__NextInstallmentDates As String
    Public Property Gifts__NextInstallmentDates() As String
        Get
            Return _gifts__NextInstallmentDates
        End Get
        Set(ByVal value As String)
            _gifts__NextInstallmentDates = value
        End Set
    End Property

    Private _gifts__LastInstallmentDate As String
    Public Property Gifts__LastInstallmentDate() As String
        Get
            Return _gifts__LastInstallmentDate
        End Get
        Set(ByVal value As String)
            _gifts__LastInstallmentDate = value
        End Set
    End Property
End Class
