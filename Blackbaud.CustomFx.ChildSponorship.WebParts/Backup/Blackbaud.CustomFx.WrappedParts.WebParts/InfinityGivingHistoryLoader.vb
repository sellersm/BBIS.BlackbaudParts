Imports Blackbaud.AppFx.ContentManagement.MetalWeb
Imports Blackbaud.Web.Content.Core.DataLists
Imports Blackbaud.Web.Content.Core.Data
Imports System.Xml.Linq
Imports Blackbaud.Web.Content.Core
Imports Blackbaud.Web.Content

Public Class InfinityGivingHistoryLoader
    Implements ILoader(Of GivingHistoryFields)

    Private Const IS_PENDING_GIFT_LOADER As Boolean = False
    Private Const CACHE_KEY_BASE As String = "infinityHistory"

    Public Enum InfinityTransactionTypeCode
        None = 0
        Pledge = 1
        RecurringGift = 2
    End Enum

    Private ReadOnly Property SessionKey(ByVal sKey As String) As String
        Get
            Return String.Concat(sKey, Me.InstanceID.GetHashCode.ToString)
        End Get
    End Property

    Private _instanceID As String
    Private Property InstanceID() As String
        Get
            Return String.Concat(_instanceID, BBSession.SessionID)
        End Get
        Set(ByVal value As String)
            _instanceID = value
        End Set
    End Property

    Private _fields As GivingHistoryFields
    Private Property Fields() As GivingHistoryFields
        Get
            Return _fields
        End Get
        Set(ByVal value As GivingHistoryFields)
            _fields = value
        End Set
    End Property

    Private _currencyType As Core.Data.GiftInformation.eCurrencyType
    Private Property CurrencyType() As Core.Data.GiftInformation.eCurrencyType
        Get
            Return _currencyType
        End Get
        Set(ByVal value As Core.Data.GiftInformation.eCurrencyType)
            _currencyType = value
        End Set
    End Property

    Private Function FormatCurrency(ByVal amount As Decimal) As String
        Return If(amount = Decimal.MinValue, String.Empty, BBFormatCurrency(amount, CurrencySymbol(Me.CurrencyType)))
    End Function

    Private Function FormatCurrency(ByVal amount As Decimal, ByVal transactionISO As String) As String
        If PortalSettings.Current.Features.MulticurrencyConditionSettingExists() Then
            If Len(Trim(transactionISO)) <> 3 Then
                Throw New ArgumentException("Invalid ISO 4217 code")
            End If
            Return If(amount = Decimal.MinValue, String.Empty, Core.Internationalization.Currency.FormatCurrency(amount, transactionISO))
        Else
            Return FormatCurrency(amount)
        End If
    End Function

    Private provider As Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider
    Private constit As ShelbyConstituent
    Private constitID As String

    ' TODO: Make it respect the query settings from the admin side
    '       Do not follow the same model as the RE7GivingHistoryLoader where it need a reference to
    '       the ParentGivingHistory.
    Public Function Load(ByVal target As GivingHistoryFields) As GivingHistoryFields Implements ILoader(Of GivingHistoryFields).Load
        If target Is Nothing Then
            Throw New ArgumentNullException("target")
        End If

        If BBWebPrincipal.Current.User.ConstituentID < 1 Then Return Fields

        'ajm - set these in here after we know we have a valid constit
        provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
        constit = New Core.Data.ShelbyConstituent(BBWebPrincipal.Current.User)
        constitID = Core.Data.ShelbyConstituent.GetConstituentsGuid(constit.RecordID).ToString

        Me.Fields = CachedHistoryData()

        If Me.Fields Is Nothing Then

            Me.Fields = New GivingHistoryFields()

            ' All gifts in RE are the same currency per Tom O
            ' TODO: This isn't RE, so double check
            Me.CurrencyType = Core.MerchantAccountBBPS.BLL.MATransactionInfo.InstalledCurrencyType()

            Dim gifts = GetGifts()
            Dim giftsRow As GivingHistoryFields.GiftsRow
            Dim pledges As New Generic.List(Of GivingHistoryFields.GiftsRow)
            Dim editableRecurringGifts = GetEditableRecurringGifts()

            For Each g In gifts
                giftsRow = AddGift(g)

                If g.TRANSACTIONTYPECODE = InfinityTransactionTypeCode.Pledge Then
                    pledges.Add(giftsRow)
                End If

                If editableRecurringGifts IsNot Nothing AndAlso editableRecurringGifts.Count > 0 _
                AndAlso editableRecurringGifts.Contains(g.ID.ToString) AndAlso g.AUTOPAY AndAlso If(Not String.IsNullOrEmpty(g.RECURRINGGIFTENDDATE), Date.Compare(CDate(g.RECURRINGGIFTENDDATE), Date.UtcNow) >= 0, True) Then
                    giftsRow.IsEditableRecurringGift = True
                End If

                'AddCampaigns(g, giftsRow.ID)
            Next

            Dim allInstallments = GetInstallments()

            For Each g As GivingHistoryFields.GiftsRow In pledges
                AddInstallments(g.GiftRecordID, g.ID, allInstallments)
                Dim appl = Me.Fields.Appeals.NewAppealsRow()
            Next

            'ViM If you allow users to edit their recurring gifts, you need to get their payment accounts as well
            'for now this is credit cards and dd accounts
            Dim constituentAccounts = GetConstituentPaymentAccounts()

            For Each item In constituentAccounts
                If item.ID <> Guid.Empty Then
                    Dim accountDesc As String = If(item.ACCOUNTDESCRIPTION Is Nothing, String.Empty, item.ACCOUNTDESCRIPTION)
                    AddPaymentAccount(item.ID.ToString, accountDesc, constitID, item.PAYMENTACCOUNTTYPE)
                End If
            Next

            If Not OverrideGiftTypeFilter.Contains(Blackbaud.Web.Content.Common.Enumerations.EInfinityGiftType.SponsorshipRecurringGift) AndAlso _
                Not OverrideGiftTypeFilter.Contains(Blackbaud.Web.Content.Common.Enumerations.EInfinityGiftType.SponsorshipRecurringGiftPayment) Then

                Dim events = GetEventData()

                For Each item In events
                    AddEvent(item)
                Next

            End If

            CachedHistoryData = Me.Fields
        End If

        target.Merge(Me.Fields)

        Return target

    End Function

    Private Property CachedHistoryData() As GivingHistoryFields
        Get
            Dim data As Object = Me.CacheManager.Item(Me.CacheManager.GetKey(SessionKey(CACHE_KEY_BASE)))

            If data IsNot Nothing Then
                Return DataObject.FromXML(Of GivingHistoryFields)(CStr(data))
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As GivingHistoryFields)
            Me.CacheManager.Item(Me.CacheManager.GetKey(CACHE_KEY_BASE)) = DataObject.ToXML(value)
        End Set
    End Property

    Private Sub AddEvent(ByVal regRow As Blackbaud.Web.Content.Core.DataLists.Constituent.ConstituentEventRegistrationsRow)
        Dim participationRow = Me.Fields.Event_participations.NewEvent_participationsRow

        participationRow.GiftRecordID = regRow.REVENUEID.ToString
        participationRow.EventRecordID = regRow.EVENTRECORDID.ToString
        participationRow.EventName = regRow.EVENTNAME
        participationRow.TicketType = regRow.TICKETTYPE
        participationRow.ParticipantName = regRow.PARTICIPANTNAME
        participationRow.HostRegistrantID = regRow.HOSTREGISTRANTID.ToString
        participationRow.GuestConstituentID = regRow.CONSTITUENTID.ToString
        participationRow.EventPriceID = regRow.EVENTPRICEID.ToString
        participationRow.GuestRegistrantID = regRow.GUESTREGISTRANTID.ToString
        participationRow.RegistrationID = regRow.REGISTRATIONID.ToString
        participationRow.RegistrationCount = regRow.REGISTRATIONCOUNT.ToString

        'ViM: In a multicurrency BBEC, revenue currency type cannot be based on legacy InstalledCountry setting.
        'We pull the ISO4217 for each revenue transaction fetched by the transaction manager.
        'Currency formatting for display is based on the culture associated with the ISO4127 (similar to how BBEC does currency formatting)
        Dim cur = Me.Fields.EventCurrency.NewEventCurrencyRow
        cur.EventRecordID = participationRow.EventRecordID
        cur.ISO4217 = If(PortalSettings.Current.Features.MulticurrencyConditionSettingExists(), regRow.ISOCURRENCYCODE, CurrencySymbol(Me.CurrencyType, True))
        cur.Event_participationsRow = participationRow
        Me.Fields.EventCurrency.AddEventCurrencyRow(cur)

        participationRow.TicketPrice = FormatCurrency(regRow.TICKETPRICE, cur.ISO4217)
        participationRow.TicketPriceUnformatted = regRow.TICKETPRICE

        Me.Fields.Event_participations.AddEvent_participationsRow(participationRow)
    End Sub

    Private Sub AddPaymentAccount(ByVal accountId As String, ByVal accountDescription As String, ByVal constituentId As String, ByVal paymentType As Integer)
        Dim paymentAccountRow As GivingHistoryFields.Constituents_PaymentAccountsRow

        paymentAccountRow = Me.Fields.Constituents_PaymentAccounts.NewConstituents_PaymentAccountsRow()
        paymentAccountRow.ID = accountId
        'bug 87527 -  localize checking
        paymentAccountRow.AccountDescription = If(accountDescription.Contains("Checking"), accountDescription.Replace("Checking", Char.ToUpper(StringLocalizer.BBString("checking~").Chars(0)) & StringLocalizer.BBString("checking~").Substring(1)), accountDescription)
        paymentAccountRow.ConstituentRecordID = constituentId
        paymentAccountRow.PaymentType = paymentType

        Me.Fields.Constituents_PaymentAccounts.AddConstituents_PaymentAccountsRow(paymentAccountRow)
    End Sub

    Private Sub AddInstallments(ByVal giftRecordId As String, ByVal giftsId As Guid, ByRef allInstallments As Blackbaud.Web.Content.Core.DataLists.Constituent.InstallmentsByConstituentRow())
        Dim installments = allInstallments.Where(Function(f) f.REVENUEID.ToString = giftRecordId)
        Dim installmentRow As GivingHistoryFields.InstallmentsRow


        Dim currencyISO As String = Me.Fields.GiftCurrency.Single(Function(f) f.GiftsID = giftsId).ISO4217


        For Each installment In installments
            If installment.RECORDTYPE <> 0 Then
                Continue For
            End If

            installmentRow = Me.Fields.Installments.NewInstallmentsRow()
            installmentRow.GiftsID = giftsId
            installmentRow.Amount = FormatCurrency(installment.AMOUNT, currencyISO)
            installmentRow.Balance = FormatCurrency(installment.BALANCE, currencyISO)
            installmentRow.InstallmentNumber = installment.SEQUENCE.ToString
            installmentRow.InstallmentDate = installment.DATE.ToString()
   Me.Fields.Installments.AddInstallmentsRow(installmentRow)
        Next

        Dim paymentRow As GivingHistoryFields.PaymentsRow

        For Each payment In installments
            If payment.RECORDTYPE <> 1 Then
                Continue For
            End If

            paymentRow = Me.Fields.Payments.NewPaymentsRow()
            paymentRow.PaidToRecordID = giftsId
            paymentRow.GiftRecordID = payment.TRANSACTIONID.ToString
            paymentRow.PaidToID = payment.REVENUEID.ToString
            paymentRow.REGiftID = payment.TRANSACTIONLOOKUPID
            paymentRow.PaymentType = payment.PAYMETHOD
            paymentRow.InstallmentNumber = payment.SEQUENCE.ToString
            paymentRow.PaymentAmount = FormatCurrency(payment.AMOUNT, currencyISO)
            paymentRow.PaymentDate = payment.DATE.Value.ToShortDateString
            Me.Fields.Payments.AddPaymentsRow(paymentRow)
        Next
    End Sub

    Private Function GetGifts() As Blackbaud.Web.Content.Core.DataLists.Constituent.TransactionManagerRecordsRow()
        Dim filterData = New Blackbaud.Web.Content.Core.DataLists.Constituent.TransactionManagerRecordsFilterData
        filterData.INCLUDESOFTCREDIT = ParentGivingHistory.IncludeSoftCredits
        filterData.QUERYID = ParentGivingHistory.QueryID
        filterData.APPEALSFILTER = GetAppealsString()
        filterData.DESIGNATIONSFILTER = GetDesignationsString()
        filterData.TYPESFILTER = GetGiftTypesString()
        filterData.PLEDGEPAYMENTCURRENCYID = ParentGivingHistory.PledgePaymentCurrencyID
        filterData.RECURRINGGIFTPAYMENTCURRENCYID = ParentGivingHistory.RecurringGiftPaymentCurrencyID

        Return Blackbaud.Web.Content.Core.DataLists.Constituent.TransactionManagerRecords.GetRows(provider, constitID, filterData)
    End Function

    Private Function GetInstallments() As Blackbaud.Web.Content.Core.DataLists.Constituent.InstallmentsByConstituentRow()
        Return Blackbaud.Web.Content.Core.DataLists.Constituent.InstallmentsByConstituent.GetRows(provider, constitID)
    End Function

    Private Function GetConstituentPaymentAccounts() As Blackbaud.Web.Content.Core.DataLists.Constituent.PaymentAccountsByConstituentRow()
        Return Blackbaud.Web.Content.Core.DataLists.Constituent.PaymentAccountsByConstituent.GetRows(provider, constitID)
    End Function

    Private Function GetEventData() As Blackbaud.Web.Content.Core.DataLists.Constituent.ConstituentEventRegistrationsRow()
        Dim filterData As New Blackbaud.Web.Content.Core.DataLists.Constituent.ConstituentEventRegistrationsFilterData
        filterData.INCLUDEUNPAIDEVENTS = ParentGivingHistory.IncludeUnpaidEvents

        Return Blackbaud.Web.Content.Core.DataLists.Constituent.ConstituentEventRegistrations.GetRows(provider, constitID, filterData)
    End Function

    Private Function GetEditableRecurringGifts() As Generic.List(Of String)
        Dim filterData = New Blackbaud.Web.Content.Core.DataLists.Constituent.RevenueRecordsByConstituentMatchingFiltersFilterData

        filterData.QUERYID = ParentGivingHistory.RecurringGiftEditFilterQueryID
        filterData.APPEALSFILTER = GetAppealsString(True)
        filterData.DESIGNATIONSFILTER = GetDesignationsString(True)
        'ajm - sponsorship recurring gifts are editable too
        Dim editableTypes(1) As String
        editableTypes(0) = CInt(Web.Content.Common.Enumerations.EInfinityGiftType.RecurringGift).ToString
        editableTypes(1) = CInt(Web.Content.Common.Enumerations.EInfinityGiftType.SponsorshipRecurringGift).ToString
        filterData.TYPESFILTER = String.Join(",", editableTypes) '4 = recurring gift, SponsorshipRecurringGift = 1024

        Dim results = Blackbaud.Web.Content.Core.DataLists.Constituent.RevenueRecordsByConstituentMatchingFilters.GetRows(provider, constitID, filterData)
        If results IsNot Nothing AndAlso results.Length > 0 Then
            Return results.Select(Of String)(Function(f) (f.ID.ToString)).ToList
        End If
    End Function

    Private Function AddGift(ByVal gift As Blackbaud.Web.Content.Core.DataLists.Constituent.TransactionManagerRecordsRow) As GivingHistoryFields.GiftsRow

        Dim row = Me.Fields.Gifts.NewGiftsRow()
        row.ID = System.Guid.NewGuid()
        Dim apls = Me.Fields.Appeals.NewAppealsRow()
        apls.Gifts_ID = row.ID
        apls.GiftsRow = row
        apls.Appeal_Description = gift.APPEAL_DESCRIPTION
        Me.Fields.Appeals.AddAppealsRow(apls)

        ' TODO: Make this field be a boolean...
        If gift.GIVENANONYMOUSLY Then
            row.Anonymous = StringLocalizer.BBString("Yes")
        Else
            row.Anonymous = StringLocalizer.BBString("No")
        End If

        row.IsSoftCredit = CBool(gift.RECOGNITIONCREDIT)
        row.DonorName = gift.DONORNAME
        row.BbncCurrencyType = Me.CurrencyType
        row.GiftRecordID = gift.ID.ToString()
        row.FundraisingPurpose = gift.FUNDRAISINGPURPOSE
        row.GiftTypeLanguageGUID = Guid.Empty ' TODO: implement this
        row._Date = gift.DATE.ToString
        row.PayMethod = If(gift.TRANSACTIONTYPECODE = InfinityTransactionTypeCode.Pledge, "Pledge", gift.PAYMENTMETHOD)
        row.Pending = IS_PENDING_GIFT_LOADER
        row.REGiftID = System.Web.HttpUtility.HtmlEncode(gift.LOOKUPID)
        row.Type = gift.GIFTTYPE 'gift.TRANSACTIONTYPE
        'row.FundDescription = gift.DESIGNATIONS
        'row.Fund = gift.DESIGNATIONLOOKUPIDS
        row.ReceiptDate = gift.RECEIPTDATE.ToString
        row.ReceiptNumber = If(gift.RECEIPTNUMBER <= 0, String.Empty, gift.RECEIPTNUMBER.ToString)
        row.InstallmentFrequency = gift.FREQUENCY
        row.InstallmentSchedule = gift.FREQUENCY
        row.NumberOfInstallments = If(gift.TRANSACTIONTYPECODE = InfinityTransactionTypeCode.Pledge, gift.NUMBEROFINSTALLMENTS.ToString, String.Empty)
        row.NumberOfInstallmentsUnformatted = gift.NUMBEROFINSTALLMENTS
        row.Subtype = gift.PLEDGESUBTYPE
        'OriginalPledgeID is used to keep track of pending pledge payment (gifts). We want to hide the pay pledge
        'link if a payment is pending. As of now, OriginalPledgeID is useless for processed or backoffice gifts.
        row.OriginalPledgeID = String.Empty
        row.IsEFT = gift.AUTOPAY
        'ViM AUTOPAY no longer a condition to flag a gift as active. Bug fix for 124941.
        'TODO: Non sponsorship recurring gifts that don't have credit/debit information cannot be active gifts
        row.IsActiveGift = ((gift.TRANSACTIONTYPECODE = InfinityTransactionTypeCode.RecurringGift AndAlso If(Not String.IsNullOrEmpty(gift.RECURRINGGIFTENDDATE), Date.Compare(CDate(gift.RECURRINGGIFTENDDATE), Date.UtcNow) >= 0, True)) OrElse (gift.TRANSACTIONTYPECODE = InfinityTransactionTypeCode.Pledge AndAlso gift.BALANCE > 0))
        row.IsRecurringGift = (gift.TRANSACTIONTYPECODE = InfinityTransactionTypeCode.RecurringGift)
        row.AutoPayCCExpirationDate = gift.AUTOPAYCCEXPIRE
        row.AutoPayCCPartialNumber = gift.AUTOPAYCCNUMBER
        'bug 87527 -  localize checking
        row.AutoPayDDAcctType = If(gift.AUTOPAYDDACCTTYPE = "Checking", Char.ToUpper(StringLocalizer.BBString("checking~").Chars(0)) & StringLocalizer.BBString("checking~").Substring(1), gift.AUTOPAYDDACCTTYPE)
        row.AutoPayDDAcctDescription = gift.AUTOPAYDDINSTDESC
        row.AutoPayCCToken = gift.AUTOPAYCCTOKEN
        row.AutoPayDDAcctID = gift.AUTOPAYDDACCTID
        row.AutoPayDDAcctPartialNumber = gift.AUTOPAYDDACCTNUMBER
        row.RecurringGiftNextTransactionDate = gift.NEXTTRANSACTIONDATE
        row.FrequencyCode = gift.FREQUENCYCODE
        row.SiteHierarchy = gift.SITEHIERARCHY
        row.RecurringGiftStartDate = gift.RECURRINGGIFTSTARTDATE
        row.RecurringGiftEndDate = gift.RECURRINGGIFTENDDATE

        If Not String.IsNullOrEmpty(gift.DESIGNATIONS) Then
            Dim tr As System.IO.TextReader = New System.IO.StringReader(gift.DESIGNATIONS)
            Dim doc As XDocument = XDocument.Load(tr)

            For Each d As XElement In doc.Elements.FirstOrDefault.Descendants
                Dim fundRow = Me.Fields.Funds.NewFundsRow
                fundRow.GiftsID = row.ID
                fundRow.Description = System.Web.HttpUtility.HtmlEncode(d.@VANITYNAME)
                fundRow.FundID = d.@ID
                Me.Fields.Funds.Rows.Add(fundRow)

                If row.Fund Is Nothing Then row.Fund = ""
                If row.FundDescription Is Nothing Then row.FundDescription = ""

                row.FundDescription = String.Concat(row.FundDescription, System.Web.HttpUtility.HtmlEncode(d.@VANITYNAME), "; ") 'designation.vanityname
                row.Fund = String.Concat(row.Fund, System.Web.HttpUtility.HtmlEncode(d.@USERID), "; ") 'designation.userid
            Next

            If Not String.IsNullOrEmpty(row.FundDescription) Then row.FundDescription = row.FundDescription.TrimEnd(New Char() {";"c, " "c})
            If Not String.IsNullOrEmpty(row.Fund) Then row.Fund = row.Fund.TrimEnd(New Char() {";"c, " "c})
        End If

        '!!^@#*&$^!!
        'bug 68315 - need a more unique seperator than just ':' or ';'
        'overkill? maybe.
        Dim descIdSeparators(1) As String
        descIdSeparators(0) = "!!^@#*&$^!!"

        'bug 68620 html encode values and make separator more unique
        Dim itemSeparators(1) As String
        itemSeparators(0) = ";*;"

        If Not String.IsNullOrEmpty(gift.CAMPAIGNS) Then
            For Each campaign In gift.CAMPAIGNS.Split(itemSeparators, StringSplitOptions.None)
                campaign = campaign.TrimEnd(New Char() {";"c, "*"c})
                Dim nameIdPair() As String = campaign.Split(descIdSeparators, StringSplitOptions.None)
                If nameIdPair.Length = 2 Then
                    Dim campaignRow = Me.Fields.Campaigns.NewCampaignsRow
                    campaignRow.GiftsID = row.ID
                    campaignRow.CampaignID = System.Web.HttpUtility.HtmlEncode(nameIdPair(1)) 'campaign.UserId
                    campaignRow.CampaignDescription = System.Web.HttpUtility.HtmlEncode(nameIdPair(0)) 'campaign.Name
                    Me.Fields.Campaigns.Rows.Add(campaignRow)
                End If
            Next
        End If

        'ViM: In a multicurrency BBEC, revenue currency type cannot be based on legacy InstalledCountry setting.
        'We pull the ISO4217 for each revenue transaction fetched by the transaction manager.
        'Currency formatting for display is based on the culture associated with the ISO4127 (similar to how BBEC does currency formatting)
        Dim cur = Me.Fields.GiftCurrency.NewGiftCurrencyRow
        cur.GiftsID = row.ID
        cur.ISO4217 = If(PortalSettings.Current.Features.MulticurrencyConditionSettingExists(), gift.ISO4217, CurrencySymbol(Me.CurrencyType, True))
        cur.GiftsRow = row
        Me.Fields.GiftCurrency.AddGiftCurrencyRow(cur)

        'moved all assignments dealing with currency down here, after the ISO4217 is set
        If row.IsSoftCredit Then
            row.Amount = FormatCurrency(gift.RECOGNITIONAMOUNT, cur.ISO4217)
            row.AmountUnformatted = gift.RECOGNITIONAMOUNT
        Else
            row.Amount = FormatCurrency(gift.AMOUNT, cur.ISO4217)
            row.AmountUnformatted = gift.AMOUNT
        End If
        row.ReceiptAmount = FormatCurrency(gift.RECEIPTAMOUNT, cur.ISO4217)

        row.Balance = If(gift.TRANSACTIONTYPECODE = InfinityTransactionTypeCode.Pledge, FormatCurrency(gift.BALANCE, cur.ISO4217), Nothing)
        row.BalanceUnformatted = gift.BALANCE

        row.GiftAidAmount = FormatCurrency(gift.GIFTAIDAMOUNT, cur.ISO4217)
        row.GiftAidAmountUnformatted = gift.GIFTAIDAMOUNT

        If gift.LASTRECURRINGPAYMENTAMOUNT > 0 AndAlso Not String.IsNullOrEmpty(gift.LASTRECURRINGPAYMENTDATE) Then
            Dim lastRecurringPaymentRow = Me.Fields.LastRecurringPayment.NewLastRecurringPaymentRow
            lastRecurringPaymentRow.GiftsID = row.ID
            lastRecurringPaymentRow.Amount = FormatCurrency(gift.LASTRECURRINGPAYMENTAMOUNT, cur.ISO4217)
            lastRecurringPaymentRow.DatePaid = gift.LASTRECURRINGPAYMENTDATE
            Me.Fields.LastRecurringPayment.Rows.Add(lastRecurringPaymentRow)
        End If

        Dim settings = Me.Fields.GiftConditionSettings.NewGiftConditionSettingsRow
        settings.GiftsID = row.ID

        If (gift.TRANSACTIONTYPECODE = InfinityTransactionTypeCode.Pledge) Then
            settings.PaymentCurrencyAcceptable = gift.PLEDGEPAYMENTCURRENCYACCEPTABLE
        ElseIf (gift.TRANSACTIONTYPECODE = InfinityTransactionTypeCode.RecurringGift) Then
            settings.PaymentCurrencyAcceptable = gift.RECURRINGGIFTPAYMENTCURRENCYACCEPTABLE
        Else
            settings.PaymentCurrencyAcceptable = False
        End If

        settings.GiftsRow = row
        Me.Fields.GiftConditionSettings.AddGiftConditionSettingsRow(settings)

        Me.Fields.Gifts.AddGiftsRow(row)

        Return row
    End Function

    Private Function GetGiftTypesString() As String
        Dim giftTypes As New System.Text.StringBuilder()

        If Me.OverrideGiftTypeFilter.Count > 0 Then
            For Each giftTypeID As Web.Content.Common.Enumerations.EInfinityGiftType In Me.OverrideGiftTypeFilter
                If Not String.IsNullOrEmpty(giftTypes.ToString) Then giftTypes.Append(",")
                giftTypes.Append(CInt(giftTypeID).ToString)
            Next
        ElseIf Me.ParentGivingHistory.UseCustomFilter Then
            For Each giftTypeID As Integer In Me.ParentGivingHistory.GiftTypes.Values
                If Not String.IsNullOrEmpty(giftTypes.ToString) Then giftTypes.Append(",")
                giftTypes.Append(giftTypeID)
            Next
        End If

        Return giftTypes.ToString
    End Function

    Private Function GetAppealsString(Optional ByVal isForRecGiftFilter As Boolean = False) As String
        Dim appeals As New System.Text.StringBuilder()

        If isForRecGiftFilter Then
            If Me.ParentGivingHistory.RecurringGiftEditFilterUseCustomFilter Then
                For Each appealID As Integer In Me.ParentGivingHistory.RecurringGiftEditFilterAppeals.Values
                    If Not String.IsNullOrEmpty(appeals.ToString) Then appeals.Append(",")
                    appeals.Append(appealID)
                Next
            End If
        Else
            If Me.ParentGivingHistory.UseCustomFilter Then
                For Each appealID As Integer In Me.ParentGivingHistory.Appeals.Values
                    If Not String.IsNullOrEmpty(appeals.ToString) Then appeals.Append(",")
                    appeals.Append(appealID)
                Next
            End If
        End If

        Return appeals.ToString
    End Function

    Private Function GetDesignationsString(Optional ByVal isForRecGiftFilter As Boolean = False) As String
        Dim designations As New System.Text.StringBuilder()
        Dim bGetChildren As Boolean = False

        If isForRecGiftFilter Then
            If Me.ParentGivingHistory.RecurringGiftEditFilterUseCustomFilter Then
                For Each designationID As String In Me.ParentGivingHistory.RecurringGiftEditFilterFunds.Keys 'actually designations
                    If Not String.IsNullOrEmpty(designations.ToString) Then designations.Append(",")
                    designations.Append(designationID)
                Next

                bGetChildren = True
            End If

        Else
            If Me.ParentGivingHistory.UseCustomFilter Then
                For Each designationID As String In Me.ParentGivingHistory.Funds.Keys 'actually designations
                    If Not String.IsNullOrEmpty(designations.ToString) Then designations.Append(",")
                    designations.Append(designationID)
                Next

                bGetChildren = True
            End If
        End If

        If bGetChildren AndAlso Not String.IsNullOrEmpty(designations.ToString) Then
            Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
            Dim filterData = New Blackbaud.Web.Content.Core.DataLists.TopLevel.DesignationChildrenDataListFilterData
            Dim rows As Blackbaud.Web.Content.Core.DataLists.TopLevel.DesignationChildrenDataListRow()

            filterData.PARENTIDSLIST = designations.ToString
            rows = Core.DataLists.TopLevel.DesignationChildrenDataList.GetRows(provider, filterData)

            If rows IsNot Nothing Then
                designations = New System.Text.StringBuilder
                For Each row As Blackbaud.Web.Content.Core.DataLists.TopLevel.DesignationChildrenDataListRow In rows
                    If Not String.IsNullOrEmpty(designations.ToString) Then designations.Append(",")
                    designations.Append(row.DESIGNATIONID)
                Next
            End If
        End If

        Return designations.ToString
    End Function

    Private _cacheManager As ICacheManager
    Private Property CacheManager() As ICacheManager
        Get
            Return _cacheManager
        End Get
        Set(ByVal value As ICacheManager)
            _cacheManager = value
        End Set
    End Property

    Private moGivingHistory As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory
    Public Property ParentGivingHistory() As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory
        Get
            Return moGivingHistory
        End Get
        Set(ByVal value As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory)
            moGivingHistory = value
        End Set
    End Property

    'as of right now this property will only affect infinity
    Private _OverrideGiftTypeFilter As Generic.List(Of Web.Content.Common.Enumerations.EInfinityGiftType)
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

    Public Sub New(ByVal cacheManager As ICacheManager, ByVal instanceID As String)
        Me.CacheManager = cacheManager
        Me.InstanceID = instanceID
    End Sub
End Class
