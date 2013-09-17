Imports Blackbaud.Web.Content.Core.RE7Service
Imports Blackbaud.Web.Content.Core.Data
Imports System.Web.Services.Protocols
Imports Blackbaud.Web.Content.Core
Imports Blackbaud.Web.Content
Imports System.Xml

Public Class RE7GivingHistoryLoader
    Implements ILoader(Of GivingHistoryFields)

    Private Const CACHE_KEY_BASE As String = "history"

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

    Private moConstituentData As RE7Service.ConstitData
    Private ReadOnly Property ConstituentData() As RE7Service.ConstitData
        Get
            Return moConstituentData
        End Get
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

    Private miPage As Integer
    Public Property GiftPageNumber() As Integer
        Get
            Return (miPage)
        End Get
        Set(ByVal value As Integer)
            miPage = value
        End Set
    End Property

    Private miConstitID As Integer
    Public Property ConstituentID() As Integer
        Get
            Return miConstitID
        End Get
        Set(ByVal value As Integer)
            miConstitID = value
        End Set
    End Property

    'Private Function GetConstituentHistory() As RE7Service.ConstitData
    '    Dim args As New RE7Service.GetConstitDataArgs
    '    With args
    '        .Selection = BuildSelection(New Object)
    '        .IDType = RE7Service.ConstitIDType.SystemRecordID
    '        .Identifier = CStr(ConstituentID)
    '    End With

    '    Try
    '        Return Core.Data.RE7ServiceHelper.GetSvcProxy(True).GetConstitData(args).Data
    '    Catch ex As Exception
    '        Return Nothing

    '    End Try

    'End Function

    Private Function AddMembershipAndEventSelection() As RE7Service.ConstitSelection

        Dim selection As New RE7Service.ConstitSelection
        With selection
            .Membership = New RE7Service.MembershipSelection
            With .Membership
                .Membership_ID = True
                .System_ID = True
                .Primary = True
                .Standing = True
                .Current_Dues_Amount = True
                .Program = True
                .Category = True
                .Subcategory = True
                .Date_Joined = True
                .Total_Years = True
                .Last_Renewed_Date = True
                .Consecutive_Years = True
                .History = New RE7Service.MemTranSelection
                With .History
                    .DateAdded = True
                    .DateChanged = True
                    .TransDate = True
                    .Program = True
                    .Category_description = True
                    .Dues = True
                    .Expires_on = True
                    .System_ID = True
                    .Linked_Gifts = New RE7Service.MemGiftSelection
                    With .Linked_Gifts
                        .Gift_Record = New RE7Service.GiftSelection
                        .Gift_Record.System_ID = True
                    End With
                End With

                .Current = New RE7Service.MemTranSelection
                .Current.Send_Benefits_To = True
                .Current.Benefits = New RE7Service.BenefitSelection
                With .Current.Benefits
                    .Count = True
                    .Unit_Cost = True
                    .Total_Benefit_Value = True
                    .Sent = True
                    .Comments = True
                    .Benefit = True
                End With

            End With

            .Event_participations = New RE7Service.ConstitPartSelection
            With .Event_participations
                .Event = New RE7Service.EventSelection
                With .Event
                    .System_ID = True
                    .Event_ID = True
                    .Name = True
                    .Start_Date = True
                End With
                .Registration_Fees = New RE7Service.PartFeeSelection
                With .Registration_Fees
                    .Gift_Amount = True
                    .Receipt_Amount = True
                    .System_ID = True
                End With
                .Registration_fees_gifts = New RE7Service.ParticipantFeeSelection
                .Registration_fees_gifts.Fees_gifts = New RE7Service.GiftSelection
                With .Registration_fees_gifts.Fees_gifts
                    .System_ID = True
                End With
            End With
        End With

        Return selection
    End Function


    Private Function BuildSelection(ByVal selectionArgs As Object) As RE7Service.ConstitSelection

        Dim selection As New RE7Service.ConstitSelection

        With selection

            .Gifts = BuildGiftSelection()

            ''
            '' Leaving Memberships and Events commented out for future extension of Giving History
            ''
            '.Membership = New RE7Service.MembershipSelection
            'With .Membership
            '    .Membership_ID = True
            '    .System_ID = True
            '    .Primary = True
            '    .Standing = True
            '    .Current_Dues_Amount = True
            '    .Program = True
            '    .Category = True
            '    .Subcategory = True
            '    .Date_Joined = True
            '    .Total_Years = True
            '    .Last_Renewed_Date = True
            '    .Consecutive_Years = True
            '    .History = New RE7Service.MemTranSelection
            '    With .History
            '        .DateAdded = True
            '        .DateChanged = True
            '        .TransDate = True
            '        .Program = True
            '        .Category_description = True
            '        .Dues = True
            '        .Expires_on = True
            '        .System_ID = True
            '        .Linked_Gifts = New RE7Service.MemGiftSelection
            '        With .Linked_Gifts
            '            .Gift_Record = New RE7Service.GiftSelection
            '            .Gift_Record.System_ID = True
            '        End With
            '    End With

            '    .Current = New RE7Service.MemTranSelection
            '    .Current.Send_Benefits_To = True
            '    .Current.Benefits = New RE7Service.BenefitSelection
            '    With .Current.Benefits
            '        .Count = True
            '        .Unit_Cost = True
            '        .Total_Benefit_Value = True
            '        .Sent = True
            '        .Comments = True
            '        .Benefit = True
            '    End With

            'End With

            '.Event_participations = New RE7Service.ConstitPartSelection
            'With .Event_participations
            '    .Event = New RE7Service.EventSelection
            '    With .Event
            '        .System_ID = True
            '        .Event_ID = True
            '        .Name = True
            '        .Start_Date = True
            '    End With
            '    .Registration_Fees = New RE7Service.PartFeeSelection
            '    With .Registration_Fees
            '        .Gift_Amount = True
            '        .Receipt_Amount = True
            '        .System_ID = True
            '    End With
            '    .Registration_fees_gifts = New RE7Service.ParticipantFeeSelection
            '    .Registration_fees_gifts.Fees_gifts = New RE7Service.GiftSelection
            '    With .Registration_fees_gifts.Fees_gifts
            '        .System_ID = True
            '    End With


            'End With

        End With

        Return selection
    End Function

    Public Function BuildGiftSelection() As RE7Service.GiftSelection

        Dim selection As New RE7Service.GiftSelection

        With selection
            .Date = True
            .Amount = True
            .Fund = True
            .Pay_method = True
            .Receipt_date = True
            .Receipt_number = True
            .Receipt_amount = True
            .Installment_schedule = True
            .Installment_Frequency = True
            .Balance = True
            .Number_of_Installments = True
            .Type = True
            .System_ID = True
            .Gift_ID = True
            .Gift_subtype = True
            .Anonymous = True
            .EFT = True
            .Next_transaction_date = True
            .Gift_status = True
            .Gift_status_date = True

            If Core.Data.RE7ServiceHelper.InstalledCountry = RE7Service.bbInstalledCountries.CTY_UK Then
                .Tax_claim_amount = True
                .Tax_claim_eligible = True
            End If

            .Tributes = New RE7Service.TributeSelection
            With .Tributes
                .Name = True
                .Tribute = True
                .Tribute_Description = True
                .Tribute_Type = True
                .Acknowledgement = True
            End With

            .Funds = New RE7Service.GiftFundSelection
            With .Funds
                .Description = True
                '.Fund = New RE7Service.FundSelection
                '.Fund.Default_Appeal = True
                '.Fund.Default_Campaign_ID = True
            End With

            .Appeals = New RE7Service.GiftAppealSelection
            With .Appeals
                .Appeal_ID = True
                .System_ID = True
                .Description = True
                .Appeal = New RE7Service.AppealSelection
                .Appeal.Packages = New RE7Service.AppealPackageSelection
                With .Appeal.Packages
                    .Package = True
                    .System_ID = True
                End With
            End With


            .Installments = New RE7Service.InstallmentSelection
            With .Installments
                .Amount = True
                .Balance = True
                .Date = True
                .Payments = New RE7Service.PaymentSelection
                With .Payments
                    .Payment_Gift = New RE7Service.GiftSelection
                    .Payment_Gift.Type = True
                    .Payment_Gift.Gift_ID = True
                    .Payment_Gift.System_ID = True
                    .Payment_Gift.Amount = True
                    .Payment_Gift.Date = True
                End With
            End With

            .Reminders_Sent = New RE7Service.GiftRemindersSelection
            With .Reminders_Sent
                .Date_Due = True
                .Sent_On = True
                .Sent_For_Description = True
                .Balance = True
                .Amount = True
            End With

            .Campaigns = New RE7Service.GiftCampaignSelection
            With .Campaigns
                .Campaign_ID = True
                .Description = True
            End With

        End With

        Return selection

    End Function

    ' TODO: make sure this still works for supervisor, it looks like it will (even after the return type changes)
    Public Function Load(ByVal fields As GivingHistoryFields) As GivingHistoryFields Implements ILoader(Of GivingHistoryFields).Load

        If ConstituentID < 1 Then Return fields

        Dim RE7Fields As GivingHistoryFields = GetCachedHistoryData()

        If RE7Fields Is Nothing Then

            moConstituentData = New RE7Service.ConstitData

            Try
                ConstituentData.Gifts = GetPagedGiftData(0, DoSearch(Me.ParentGivingHistory.QueryID))
            Catch ex As Exception
                ' CR319135-050609 JDL 5/7/09
                '  Since this is on the client side, we shouldn't be displaying any error messages
                '  while we're trying to retrieve gift data. In this case, no gifts will load, similar
                '  to how we handle a query or filter that returns no results.
                ConstituentData.Gifts = Nothing
            End Try

            Dim myRecordID As Integer = ConstituentID
            RE7Fields = New GivingHistoryFields

            If ConstituentData.Gifts IsNot Nothing Then

                AddGiftRows(ConstituentData.Gifts, RE7Fields, myRecordID)

                ''
                '' Leaving Events and Memberships commented out for future extension of the Giving History
                ''
                'If ConstituentData.Event_participations IsNot Nothing Then
                '    For Each evnt In ConstituentData.Event_participations
                '        Dim eventRow = fields.Event_participations.NewEvent_participationsRow
                '        eventRow.Constituent_RecordID = myRecordID
                '        eventRow.Event_ID = evnt.Event.Event_ID
                '        eventRow.Event_RecordID = evnt.Event.System_ID
                '        eventRow.Name = evnt.Event.Name
                '        eventRow.Start_Date = evnt.Event.Start_Date

                '        For Each regFee In evnt.Registration_Fees
                '            Dim regFeeRow = fields.Registration_Fees.NewRegistration_FeesRow
                '            regFeeRow.Event_RecordID = eventRow.Event_RecordID
                '            regFeeRow.Gift_Amount = regFee.Gift_Amount
                '            regFeeRow.Receipt_Amount = regFee.Receipt_Amount
                '            fields.Registration_Fees.Rows.Add(regFeeRow)
                '        Next

                '        For Each regFeeGift In evnt.Registration_fees_gifts
                '            For Each regFeeGift2 In regFeeGift.Fees_gifts
                '                Dim eventLnkGiftRow = fields.Event_Linked_Gifts.NewEvent_Linked_GiftsRow
                '                eventLnkGiftRow.Event_RecordID = eventRow.Event_RecordID

                '                Dim linkedGiftID As String = regFeeGift2.System_ID
                '                Dim linkedID = From myGift In fields.Gifts _
                '                                Where myGift.Gift_RecordID = linkedGiftID.ToString _
                '                                Select myGift.ID
                '                eventLnkGiftRow.Gifts_ID = linkedID.Single

                '                fields.Event_Linked_Gifts.Rows.Add(eventLnkGiftRow)
                '            Next
                '        Next
                '        fields.Event_participations.Rows.Add(eventRow)
                '    Next
                'End If

                'If ConstituentData.Membership IsNot Nothing Then
                '    For Each mem In ConstituentData.Membership
                '        Dim memRow = fields.Membership.NewMembershipRow
                '        memRow.Constituent_RecordID = myRecordID
                '        memRow.REMembership_ID = mem.Membership_ID
                '        memRow.Membership_RecordID = mem.System_ID
                '        memRow.Primary = mem.Primary
                '        memRow.Standing = mem.Standing
                '        memRow.Current_Dues_Amount = mem.Current_Dues_Amount
                '        memRow.Program = mem.Program
                '        memRow.Category = mem.Category
                '        memRow.Subcategory = mem.Subcategory
                '        memRow.Date_Joined = mem.Date_Joined
                '        memRow.Total_Years = mem.Total_Years
                '        memRow.Last_Renewed_Date = mem.Last_Renewed_Date
                '        memRow.Consecutive_Years = mem.Consecutive_Years
                '        For Each ben In mem.Current.Benefits
                '            Dim benRow = fields.Benefits.NewBenefitsRow
                '            benRow.Membership_ID = memRow.ID
                '            benRow.Count = ben.Count
                '            benRow.Unit_Cost = ben.Unit_Cost
                '            benRow.Total_Benefit_Value = ben.Total_Benefit_Value
                '            benRow.Sent = ben.Sent
                '            benRow.Comments = ben.Comments
                '            benRow.Benefit = ben.Benefit
                '            benRow.Send_Benefits_To = mem.Current.Send_Benefits_To
                '            fields.Benefits.Rows.Add(benRow)
                '        Next
                '        For Each hist In mem.History
                '            Dim histRow = fields.History.NewHistoryRow
                '            histRow.Membership_ID = memRow.ID
                '            histRow.History_RecordID = hist.System_ID
                '            histRow.DateAdded = hist.DateAdded
                '            histRow.DateChanged = hist.DateChanged
                '            histRow.TransDate = hist.TransDate
                '            histRow.Program = hist.Program
                '            histRow.Category_description = hist.Category_description
                '            histRow.Dues = hist.Dues
                '            histRow.Expires_on = hist.Expires_on
                '            For Each memLnkGift In hist.Linked_Gifts
                '                Dim memLnkGiftRow = fields.Mem_Linked_Gifts.NewMem_Linked_GiftsRow
                '                memLnkGiftRow.History_RecordID = histRow.History_RecordID

                '                Dim linkedGiftID As String = memLnkGift.Gift_Record.System_ID
                '                Dim linkedID = From myGift In fields.Gifts _
                '                                Where myGift.Gift_RecordID = linkedGiftID.ToString _
                '                                Select myGift.ID


                '                memLnkGiftRow.Gifts_ID = linkedID.Single

                '                fields.Mem_Linked_Gifts.Rows.Add(memLnkGiftRow)
                '            Next
                '            fields.History.Rows.Add(histRow)
                '        Next
                '        fields.Membership.Rows.Add(memRow)

                '    Next
                'End If
            End If

            CacheHistoryData(RE7Fields)

        Else
            'JDL - WI150756 - must cap the number of gifts returned from RE7 due to possible server timeout
            If RE7Fields.Gifts IsNot Nothing AndAlso RE7Fields.Gifts.Rows.Count >= 100 Then
                Me.ParentGivingHistory.TruncateNumberOfRecords = True
            End If

        End If

        fields.Merge(RE7Fields)

        Return fields

    End Function

    Private Sub AddGiftRows(ByVal oGiftData As GiftData(), ByVal RE7Fields As GivingHistoryFields, ByVal myRecordID As Integer)
        ' All gifts in RE are the same currency per Tom O
        Dim installedCurrencyType = Core.MerchantAccountBBPS.BLL.MATransactionInfo.InstalledCurrencyType()

        Dim sIDs = Me.BuildDonationTransactionIDs(oGiftData)
        Dim receiptKeys As SPWrap.spLoadEReceiptKeysByDonationTransactionIDs.ResultRow()
        If Not String.IsNullOrEmpty(sIDs) Then
            receiptKeys = SPWrap.spLoadEReceiptKeysByDonationTransactionIDs.ExecuteSP(PortalSettings.ConnectionString, sIDs)
        Else
            receiptKeys = New SPWrap.spLoadEReceiptKeysByDonationTransactionIDs.ResultRow() {}
        End If

        For Each gift In oGiftData

            Dim giftRow = RE7Fields.Gifts.NewGiftsRow
            giftRow.ID = System.Guid.NewGuid()
            giftRow.ConstituentRecordID = myRecordID
            giftRow.GiftRecordID = gift.System_ID
            giftRow.REGiftID = gift.Gift_ID
            giftRow._Date = gift.Date
            giftRow.Amount = gift.Amount
            giftRow.AmountUnformatted = gift.AmountUnformatted
            giftRow.Fund = gift.Fund
            giftRow.PayMethod = gift.Pay_method
            giftRow.ReceiptDate = gift.Receipt_date
            giftRow.ReceiptNumber = gift.Receipt_number
            giftRow.ReceiptAmount = gift.Receipt_amount
            giftRow.InstallmentSchedule = gift.Installment_schedule
            giftRow.InstallmentFrequency = gift.Installment_Frequency
            giftRow.Balance = If(gift.Type IsNot Nothing AndAlso gift.Type.ToLower().Contains("pledge"), gift.Balance, String.Empty) ' Only pledges have a balance
            giftRow.BalanceUnformatted = gift.BalanceUnformatted
            giftRow.NumberOfInstallments = If(gift.Type IsNot Nothing AndAlso gift.Type.ToLower().Contains("pledge"), gift.Number_of_Installments, String.Empty) ' Only pledges have installments
            Integer.TryParse(gift.Number_of_Installments, giftRow.NumberOfInstallmentsUnformatted)
            giftRow.BbncCurrencyType = installedCurrencyType
            giftRow.SiteHierarchy = ""
            giftRow.FundraisingPurpose = ""
            If gift.EFT.Length > 0 Then
                If gift.EFT.ToUpper.CompareTo("YES") = 0 Then
                    giftRow.IsEFT = True
                Else
                    giftRow.IsEFT = False
                End If
            End If

            'OriginalPledgeID is used to keep track of pending pledge payment (gifts). We want to hide the pay pledge
            'link if a payment is pending. As of now, OriginalPledgeID is useless for processed or backoffice gifts.
            giftRow.OriginalPledgeID = String.Empty

            Dim transactionID As Integer
            Integer.TryParse(gift.BBNCTransactions_ID, transactionID)
            If transactionID > 0 Then
                giftRow.ReceiptKey = (From row In receiptKeys Where row.DonationTransactionsID = transactionID Select row.LinkKey).FirstOrDefault
            End If

            If Core.Data.RE7ServiceHelper.InstalledCountry = RE7Service.bbInstalledCountries.CTY_UK Then
                If gift.Tax_claim_eligible IsNot Nothing AndAlso gift.Tax_claim_eligible.ToLower <> "0" Then
                    'not sure why this is being bbformated, string comes in alread formated
                    'however,
                    'this was not causing a problem until 6.15, somehow the 
                    'currency used by RE and the currency formatting done by vb no longer matched
                    'this caused a server error, strange work around was to change the 
                    'browser language to match the RE culture then no server error

                    'giftRow.GiftAidAmount = BBFormatCurrency(gift.Tax_claim_amount)
                    giftRow.GiftAidAmount = gift.Tax_claim_amount
                Else
                    giftRow.GiftAidAmount = BBFormatCurrency("0")
                End If
            End If

            giftRow.GiftAidAmountUnformatted = gift.Tax_claim_amountUnformatted
            giftRow.Type = gift.Type
            giftRow.IsSoftCredit = gift.IsSoftCredit
            giftRow.DonorName = If(String.IsNullOrEmpty(gift.Constituent_Name), BBWebPrincipal.Current.User.DisplayName, gift.Constituent_Name)
            giftRow.GiftTypeLanguageGUID = GetGiftTypeLanguageGUID(gift.Type)
            giftRow.Anonymous = gift.Anonymous
            giftRow.Subtype = gift.Gift_subtype

            giftRow.Pending = False
            For Each fund In gift.Funds
                giftRow.FundDescription = String.Concat(giftRow.FundDescription, "; ", fund.Description)
            Next
            If giftRow.FundDescription IsNot Nothing Then
                giftRow.FundDescription = giftRow.FundDescription.Remove(0, 2)
            End If

            If gift.Tributes IsNot Nothing Then
                For Each trib In gift.Tributes
                    Dim tribRow = RE7Fields.Tributes.NewTributesRow
                    tribRow.GiftsID = giftRow.ID
                    tribRow.Name = trib.Name
                    tribRow.Tribute = trib.Tribute
                    tribRow.TributeDescription = trib.Tribute_Description
                    tribRow.TributeType = trib.Tribute_Type
                    tribRow.Acknowlegement = trib.Acknowledgement
                    RE7Fields.Tributes.Rows.Add(tribRow)
                Next
            End If

            If gift.Appeals IsNot Nothing Then
                For Each app In gift.Appeals
                    Dim appRow = RE7Fields.Appeals.NewAppealsRow
                    appRow.Gifts_ID = giftRow.ID
                    appRow.AppealRecordID = app.System_ID
                    appRow.Appeal_ID = app.Appeal_ID
                    appRow.Appeal_Description = app.Description
                    If app.Appeal IsNot Nothing AndAlso app.Appeal.Packages IsNot Nothing Then
                        For Each pack In app.Appeal.Packages
                            Dim packRow = RE7Fields.Packages.NewPackagesRow
                            packRow.AppealRecordID = appRow.Appeal_ID
                            packRow.Package = pack.Package
                            packRow.SystemID = pack.System_ID
                            RE7Fields.Packages.Rows.Add(packRow)
                        Next
                    End If
                    RE7Fields.Appeals.Rows.Add(appRow)
                Next
            End If

            'Dim lastInstallmentDate As DateTime = DateTime.MinValue

            If gift.Installments IsNot Nothing Then
                For instCount As Integer = 0 To gift.Installments.Count - 1
                    Dim instRow = RE7Fields.Installments.NewInstallmentsRow
                    instRow.GiftsID = giftRow.ID
                    instRow.InstallmentNumber = CStr(instCount + 1) 'Does this work or is there a better way to get this?
                    instRow.Amount = gift.Installments(instCount).Amount
                    instRow.Balance = gift.Installments(instCount).Balance
                    instRow.InstallmentDate = gift.Installments(instCount).Date
                    If gift.Installments(instCount).Payments IsNot Nothing Then
                        For Each pay In gift.Installments(instCount).Payments
                            Dim payRow = RE7Fields.Payments.NewPaymentsRow
                            payRow.PaidToID = giftRow.GiftRecordID
                            payRow.PaidToRecordID = instRow.GiftsID
                            payRow.InstallmentNumber = instRow.InstallmentNumber
                            payRow.PaymentType = pay.Payment_Gift.Type
                            payRow.REGiftID = pay.Payment_Gift.Gift_ID
                            payRow.GiftRecordID = pay.Payment_Gift.System_ID
                            payRow.PaymentAmount = pay.Payment_Gift.Amount
                            payRow.PaymentDate = pay.Payment_Gift.Date
                            RE7Fields.Payments.Rows.Add(payRow)
                        Next
                    End If

                    'If DateTime.Compare(CDate(gift.Installments(instCount).Date), lastInstallmentDate) > 0 Then
                    '    lastInstallmentDate = CDate(gift.Installments(instCount).Date)
                    'End If

                    RE7Fields.Installments.Rows.Add(instRow)
                Next
            End If

            If gift.Reminders_Sent IsNot Nothing Then
                For Each reminder In gift.Reminders_Sent
                    Dim remRow = RE7Fields.Reminders_Sent.NewReminders_SentRow
                    remRow.GiftsID = giftRow.ID
                    remRow.DateDue = reminder.Date_Due
                    remRow.SentOn = reminder.Sent_On
                    remRow.SentForDescription = reminder.Sent_For_Description
                    remRow.Balance = reminder.Balance
                    remRow.Amount = reminder.Amount
                    RE7Fields.Reminders_Sent.Rows.Add(remRow)
                Next
            End If

            If gift.Funds IsNot Nothing Then
                For Each fund In gift.Funds
                    Dim fundRow = RE7Fields.Funds.NewFundsRow
                    fundRow.GiftsID = giftRow.ID
                    fundRow.Description = fund.Description
                    RE7Fields.Funds.Rows.Add(fundRow)
                Next
            End If

            If gift.Campaigns IsNot Nothing Then
                For Each campaign In gift.Campaigns
                    Dim campaignRow = RE7Fields.Campaigns.NewCampaignsRow
                    campaignRow.GiftsID = giftRow.ID
                    campaignRow.CampaignID = campaign.Campaign_ID ' TODO: decide if we want this or the sys id
                    campaignRow.CampaignDescription = campaign.Description
                    RE7Fields.Campaigns.Rows.Add(campaignRow)
                Next
            End If

            Dim balance As Decimal = 0
            'ViM fix for bug 90010 - BalanaceUnformated is a calculated value on the RE7Service side. The expession below ensures a safe way to set balance.
            If Not Decimal.TryParse(gift.Balance, balance) Then balance = gift.BalanceUnformatted

            'added gift_status check for bug 179089 ChY 2/9/2012, 2/17/2012:
            giftRow.IsActiveGift = (gift.Gift_status.ToLower = "active") AndAlso _
                ((giftRow.IsEFT AndAlso If(Not String.IsNullOrEmpty(gift.Next_transaction_date), Date.Compare(CDate(gift.Next_transaction_date), Date.UtcNow) >= 0, False)) _
                OrElse ((Not String.IsNullOrEmpty(gift.Type)) AndAlso (gift.Type.ToLower().Contains("pledge") AndAlso balance > 0)))

            giftRow.AutoPayCCToken = Guid.Empty
            giftRow.AutoPayDDAcctID = Guid.Empty

            'used for infinity
            giftRow.RecurringGiftStartDate = String.Empty
            giftRow.RecurringGiftEndDate = String.Empty
            giftRow.RecurringGiftNextTransactionDate = String.Empty
            giftRow.FrequencyCode = 0

            RE7Fields.Gifts.Rows.Add(giftRow)
        Next
    End Sub

    Private Shared Function GetGiftTypeLanguageGUID(ByVal sGiftType As String) As Guid

        Dim giftType = (From item In RE7ServiceHelper.BackOfficeAppInfo.GiftTypes Where item.Text = sGiftType Select item).SingleOrDefault

        If giftType IsNot Nothing AndAlso giftType.eType > 0 Then
            Return LanguageGUIDHelper.LanguageGuidForField(EStaticCodeTables.GiftTypesRemoveUnused, giftType.eType)
        End If

        Return Guid.Empty
    End Function

    Private Sub CacheHistoryData(ByVal moGivingHistoryData As GivingHistoryFields)
        ' bug 120853 (CR330319-062810) ChY 10/15/2010: speed up page refresh 
        ' tomke 7/14/11 CR337262-062111 ensure cached object is specific to this part
        'DataObject.AddObjectToCache(DataObject.ToXML(moGivingHistoryData), GetType(GivingHistoryFields).Name & "_" & ConstituentID)
        DataObject.AddToCache(DataObject.ToXML(moGivingHistoryData), GetType(GivingHistoryFields).Name & "_" & Me.ParentGivingHistory.ID.ToString & "_" & ConstituentID)

    End Sub

    Private ReadOnly Property GetCachedHistoryData() As GivingHistoryFields
        Get
            ' bug 120853 (CR330319-062810) ChY 10/15/2010: speed up page refresh 
            ' tomke 7/14/11 CR337262-062111 ensure cached object is specific to this part
            'Dim data As Object = DataObject.GetObjectFromCache(GetType(GivingHistoryFields).Name & "_" & ConstituentID)
            Dim data As Object = DataObject.GetObjectFromCache(GetType(GivingHistoryFields).Name & "_" & Me.ParentGivingHistory.ID.ToString & "_" & ConstituentID)

            If data IsNot Nothing Then
                Return DataObject.FromXML(Of GivingHistoryFields)(CStr(data))
            Else
                Return Nothing
            End If
        End Get
    End Property

    Private Function DoSearch(ByVal queryID As Integer) As Integer
        Dim proxy As MasterService

        Try
            proxy = RE7ServiceHelper.GetSvcProxy()
            proxy.Timeout = System.Threading.Timeout.Infinite

            With Me.SearchState
                If Len(.PageKey) > 0 Then proxy.RemovePagedIdSet(GetRemovePagedIdSetArguments(.PageKey))

                .PrepareFreshSearch()

                .PageKey = System.Guid.NewGuid().ToString()

                Dim args As New CreatePagedGiftIDSetArgs
                With args
                    .MaxRecords = Me.SearchState.MaxToFetch
                    .Key = Me.SearchState.PageKey
                    .CacheExpirationMinutes = Me.SearchState.CachedExpirationMinutes

                    .Criteria = New GiftSearchCriteria
                    .Criteria.SelectFromQueryID = queryID
                End With

                AddSearchCriteria(args)

                Dim reply As CreatePagedGiftIDSetReply = proxy.CreatePagedGiftIDSet(args)

                .TotalFetched = reply.NumberOfRecords
                .MoreRecordsExist = reply.MoreRecords
                .CurrentPage = 0
                .CurrentRecord = 0

                Return .TotalFetched
            End With

        Catch sex As SoapException
            Throw New System.ApplicationException("Error fetching records (DoSearch()): " & RE7ServiceHelper.GetExceptionMessage(sex, proxy))

        Catch ex As Exception
            Throw New System.ApplicationException("Error fetching records (DoSearch()): " & ex.Message)

        Finally
            proxy.Dispose()
        End Try

    End Function

    Private Function GetPagedGiftData(ByVal pageNumber As Integer, ByRef numberFetched As Integer, Optional ByVal recursive As Boolean = False) As GiftData()

        'JDL - WI150756 - must cap the number of gifts returned from RE7 due to possible server timeout
        If numberFetched <= 100 Then
            Me.SearchState.RecordsPerPage = numberFetched
        Else
            Me.SearchState.RecordsPerPage = 100
            Me.ParentGivingHistory.TruncateNumberOfRecords = True
        End If
        numberFetched = 0
        Dim start As Integer
        With Me.SearchState

            pageNumber = Math.Max(0, Math.Min(pageNumber, .NumPagesFetched))
            start = pageNumber * .RecordsPerPage
        End With

        Dim selection As GiftSelection = BuildGiftSelection()

        Dim proxy As MasterService = Nothing

        Try
            Dim oSearchArgs As RE7Service.CreatePagedGiftIDSetArgs = New RE7Service.CreatePagedGiftIDSetArgs()
            oSearchArgs.Criteria = New RE7Service.GiftSearchCriteria()

            AddSearchCriteria(oSearchArgs)

            Dim args As New GetPagedGiftDataArgs

            If Me.SearchState IsNot Nothing Then
                args.StartingIndex = start
                args.NumberOfRecords = Me.SearchState.RecordsPerPage
                args.PagedIDSetKey = Me.SearchState.PageKey
                Select Case Me.SearchState.OrderBy
                    Case RecordSearchOrderBy.OrderByID
                        args.IDOrder = GetPagedGiftDataOrder.IDSet
                    Case RecordSearchOrderBy.OrderByKeyName
                        args.IDOrder = GetPagedGiftDataOrder.GiftDate
                End Select
                args.Selection = selection
                args.Criteria = oSearchArgs.Criteria
            Else
                AddMembershipAndEventSelection()
            End If

            proxy = Core.Data.RE7ServiceHelper.GetSvcProxy()
            Dim reply As GetPagedGiftDataReply = proxy.GetPagedGiftData(args)

            numberFetched = reply.Data.Length

            Dim data(reply.Data.Length - 1) As GiftData
            For i As Integer = 0 To reply.Data.Length - 1
                data(i) = CType(reply.Data(i), GiftData)
            Next i
            Return data

        Catch sex As System.Web.Services.Protocols.SoapException

            Dim header As ResponseErrorHeader = RE7ServiceHelper.GetResponseErrorHeader(proxy)

            If Not header Is Nothing Then
                Select Case header.MessageExceptionCode
                    Case MessageExceptionCode.None
                        Throw sex

                    Case MessageExceptionCode.PagedIDSetNotFound
                        ' cache expired - search again
                        ' check for recursion 
                        If Not recursive Then
                            If DoSearch(0) > 0 Then 'TODO: Add check here
                                Me.SearchState.CurrentPage = pageNumber
                                Return GetPagedGiftData(pageNumber, numberFetched, True)
                            End If
                        Else
                            Throw New System.ApplicationException("Error fetching records - Invalid Page Cache Key (GetPagedGiftData()): " & RE7ServiceHelper.GetExceptionMessage(sex, proxy))
                        End If

                    Case MessageExceptionCode.StartingIndexTooHigh
                        ' No results found from specified filtering criteria/query, return nothing
                        Return Nothing
                End Select
            Else
                Throw New System.ApplicationException("Error fetching records (GetPagedGiftData()): " & RE7ServiceHelper.GetExceptionMessage(sex, proxy))
            End If

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Throw
        Finally
            If proxy IsNot Nothing Then proxy.Dispose()
        End Try

    End Function

    Private Const VIEWSTATEKEY_RECORDSEARCHSTATE As String = "GiftRecordSearchState"
    Private _searchState As RecordSearchState
    Private ReadOnly Property SearchState() As RecordSearchState
        Get
            If _searchState Is Nothing Then
                _searchState = New RecordSearchState
                _searchState.OrderBy = RecordSearchOrderBy.OrderByKeyName
            End If

            Return _searchState
        End Get
    End Property

    Private Function GetRemovePagedIdSetArguments(ByVal key As String) As RemovePagedIdSetArgs

        Dim args As New RemovePagedIdSetArgs
        args.Key = key
        Return args

    End Function

    Private Sub AddSearchCriteria(ByVal oSearchArgs As RE7Service.CreatePagedGiftIDSetArgs)


        oSearchArgs.Criteria.ConstituentRecordID = ConstituentID
        oSearchArgs.Criteria.ShowSoftCredits = Me.ParentGivingHistory.IncludeSoftCredits

        If Me.ParentGivingHistory.UseCustomFilter Then
            With Me.ParentGivingHistory
                If .GiftTypes.Count > 0 Then
                    Dim iTypes As RE7Service.EGiftTypes
                    For Each iType As RE7Service.EGiftTypes In .GiftTypes.Values
                        iTypes = (iTypes Or iType)
                    Next iType
                    oSearchArgs.Criteria.GiftTypes = iTypes
                End If

                If .Campaigns.Count > 0 Then
                    ReDim oSearchArgs.Criteria.CampaignIDs(.Campaigns.Count - 1)
                    .Campaigns.Values.CopyTo(oSearchArgs.Criteria.CampaignIDs, 0)
                End If

                If .Funds.Count > 0 Then
                    ReDim oSearchArgs.Criteria.FundIDs(.Funds.Count - 1)
                    .Funds.Values.CopyTo(oSearchArgs.Criteria.FundIDs, 0)
                End If

                If .Appeals.Count > 0 Then
                    ReDim oSearchArgs.Criteria.AppealIDs(.Appeals.Count - 1)
                    .Appeals.Values.CopyTo(oSearchArgs.Criteria.AppealIDs, 0)
                End If
            End With
        End If
    End Sub

    Private Function BuildDonationTransactionIDs(ByVal gifts() As RE7Service.GiftData) As String
        Dim doc As New XmlDocument
        Dim root As XmlElement = doc.CreateElement("DonationTransactions")
        doc.AppendChild(root)

        Dim bValidGiftFound As Boolean = False
        For Each gift In gifts
            If Not String.IsNullOrEmpty(gift.BBNCTransactions_ID) Then
                Dim el = doc.CreateElement("DonationTransaction")

                Dim donationTransactionID = doc.CreateAttribute("ID")
                donationTransactionID.Value = gift.BBNCTransactions_ID
                el.Attributes.Append(donationTransactionID)

                root.AppendChild(el)

                bValidGiftFound = True
            End If
        Next

        Dim output As String = String.Empty
        If bValidGiftFound Then
            output = doc.OuterXml
        End If
        Return output
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

    Public Sub New(ByVal cacheManager As ICacheManager, ByVal instanceID As String)
        Me.CacheManager = cacheManager
        Me.InstanceID = instanceID
    End Sub
End Class
