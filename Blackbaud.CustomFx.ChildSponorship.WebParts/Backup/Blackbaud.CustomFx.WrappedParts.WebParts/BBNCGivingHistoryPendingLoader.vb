Imports Blackbaud.Web.Content.SPWrap
Imports Blackbaud.Web.Content.Core.Data
Imports Blackbaud.Web.Content.Core

Public Class BBNCGivingHistoryPendingLoader
  Implements ILoader(Of GivingHistoryFields)

  Private moGivingHistory As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory
  Public Property ParentGivingHistory() As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory
    Get
      Return moGivingHistory
    End Get
    Set(ByVal value As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory)
      moGivingHistory = value
    End Set
  End Property

  Public Sub New()

  End Sub

  Public Function Load(ByVal fields As GivingHistoryFields) As GivingHistoryFields Implements ILoader(Of GivingHistoryFields).Load
    If Not Me.ParentGivingHistory.IncludePending Then Return fields

    Dim pendingDonation As ShelbyDonationTran
    Dim donationResultRow = spTransactions_GetUnprocessedDonationTransactionForUser.ExecuteSP(DataObject.AppConnString, BBWebPrincipal.Current.UserID)

    For Each rr In donationResultRow

      pendingDonation = Transactions.DonationFromXML(rr.XMLObjectData)

      AddPendingGiftRow(fields, pendingDonation.Gift, DonationTransaction.GiftAidAmount(pendingDonation.Gift))

    Next

    Dim pendingMembership As ShelbyMembershipTran
    Dim bbecPendingMembership As BBEC.Membership.ShelbyMembershipTransaction
    Dim memberResultRow As spTransactions_GetUnprocessedMembershipTransactionForUser.ResultRow()
    memberResultRow = spTransactions_GetUnprocessedMembershipTransactionForUser.ExecuteSP(DataObject.AppConnString, BBWebPrincipal.Current.UserID)


    For Each rr In memberResultRow
      'bug 86189 - ajm - looks like this was NEVER set up for BBEC memberships! 
      'pendingMembership = Transactions.MembershipTranFromXML(rr.XMLObjectData)
      'AddPendingGiftRow(fields, pendingMembership.Gift, MembershipTransaction.GiftAidAmount(pendingMembership))

      If PortalSettings.Current.Features.IsInfinity Then
        bbecPendingMembership = Transactions.BBECMembershipTranFromXML(rr.XMLObjectData)
        AddPendingGiftRow(fields, bbecPendingMembership.Payment) 'bbec membership doesnt allow overpayment right now, and the gift aid is only calculated on the overpayment, see MembershipTransaction.GiftAidAmount
      Else
        pendingMembership = Transactions.MembershipTranFromXML(rr.XMLObjectData)
        AddPendingGiftRow(fields, pendingMembership.Gift, MembershipTransaction.GiftAidAmount(pendingMembership))
      End If

      'Dim memRow = fields.Membership.NewMembershipRow
      'memRow.Constituent_RecordID = BBWebPrincipal.Current.User.ConstituentID
      'memRow.Membership_RecordID = pendingMembership.MembershipInfo.ExistingMembershipID.ToString
      'memRow.Current_Dues_Amount = BBFormatCurrency(pendingMembership.MembershipInfo.DuesAmount, CurrencySymbol(DirectCast(pendingMembership.Gift.CurrencyType, GiftInformation.eCurrencyType)))
      'memRow.Category = pendingMembership.MembershipInfo.MembershipCategory.Name
      'memRow.Subcategory = pendingMembership.MembershipInfo.SubCategory
      'For Each ben As ShelbyMembershipBenefit In pendingMembership.MembershipInfo.Benefits
      '    Dim benRow = fields.Benefits.NewBenefitsRow
      '    benRow.Membership_ID = memRow.ID
      '    benRow.Count = ben.Count.ToString
      '    benRow.Unit_Cost = BBFormatCurrency(ben.UnitCost, CurrencySymbol(DirectCast(pendingMembership.Gift.CurrencyType, GiftInformation.eCurrencyType)))
      '    benRow.Total_Benefit_Value = BBFormatCurrency(ben.Value, CurrencySymbol(DirectCast(pendingMembership.Gift.CurrencyType, GiftInformation.eCurrencyType)))
      '    benRow.Sent = ben.Sent
      '    benRow.Comments = ben.Comments
      '    benRow.Benefit = ben.Name
      '    fields.Benefits.Rows.Add(benRow)
      'Next
      'fields.Membership.Rows.Add(memRow)
    Next

    Dim pendingEvent As ShelbyEventRegTran
    Dim eventResultRow As spTransactions_GetUnprocessedEventTransactionForUser.ResultRow()
    eventResultRow = spTransactions_GetUnprocessedEventTransactionForUser.ExecuteSP(DataObject.AppConnString, BBWebPrincipal.Current.UserID)

    For Each rr In eventResultRow
      pendingEvent = Transactions.EventRegFromXML(rr.XMLObjectData)

      AddPendingGiftRow(fields, pendingEvent.Gift)

      'For Each evnt As RegistrationInformation In pendingEvent.Registrations
      '    Dim eventRow = fields.Event_participations.NewEvent_participationsRow
      '    eventRow.Constituent_RecordID = BBWebPrincipal.Current.User.ConstituentID
      '    eventRow.Event_RecordID = evnt.Event.BackOfficeID.ToString
      '    eventRow.Name = evnt.Event.Name
      '    For Each regFee As RegistrationFeeInformation In evnt.Fees
      '        Dim regFeeRow = fields.Registration_Fees.NewRegistration_FeesRow
      '        regFeeRow.Event_RecordID = eventRow.Event_RecordID
      '        regFeeRow.Gift_Amount = BBFormatCurrency(regFee.GiftAmount, CurrencySymbol(DirectCast(pendingEvent.Gift.CurrencyType, GiftInformation.eCurrencyType)))
      '        fields.Registration_Fees.Rows.Add(regFeeRow)
      '    Next
      '    fields.Event_participations.Rows.Add(eventRow)
      'Next

    Next

    Return fields
  End Function

  Private _fundSelection As RE7Service.FundSelection
  Private ReadOnly Property FundSelection() As RE7Service.FundSelection
    Get
      If _fundSelection Is Nothing Then
        _fundSelection = New RE7Service.FundSelection()

        _fundSelection.System_ID = True
        _fundSelection.Description = True
      End If

      Return _fundSelection
    End Get
  End Property

  Private Function FetchData(ByVal IDs() As Integer) As RE7Service.FundData()

    Dim a As New RE7Service.GetCFADataArguments
    a.IdType = RE7Service.CFAIdType.SystemRecordId
    a.Selection = Me.FundSelection
    a.Identifiers = Array.ConvertAll(IDs, Function(x) x.ToString)
    'a.Context = eCFAContext.FundRaiser

    Try
      Return FundSearch.FetchData(a)
    Catch
      'catching all exceptions now
      'Catch exSoap As System.Web.Services.Protocols.SoapException
      'sterling 1/4/2007 causing issues with FRSync 
      'commenting out per Byron
      'Me.BackOfficeID = 0
      Throw
    End Try

  End Function

  Private Sub AddPendingGiftRow(ByVal fields As GivingHistoryFields, ByVal gift As GiftInformation, Optional ByVal giftAidAmount As Decimal = 0)

    If IncludeGift(gift) Then

      Dim giftRow = fields.Gifts.NewGiftsRow
      giftRow.ID = System.Guid.NewGuid()
      giftRow.ConstituentRecordID = BBWebPrincipal.Current.User.ConstituentID
      giftRow.GiftRecordID = CStr(gift.BackOfficeID)  ' Will be 0 as this is a pending transaction
      giftRow._Date = gift.GiftDate.ToString() ' TODO: Make the dataset store a date instead of a string, let it be nullable
            giftRow.Amount = BBFormatCurrency(gift.Amount, CurrencySymbol(gift.CurrencyType))
            giftRow.AmountUnformatted = Convert.ToDecimal(gift.Amount)
            giftRow.IsSoftCredit = False
            giftRow.DonorName = BBWebPrincipal.Current.User.DisplayName
            If gift.Anonymous Then
                giftRow.Anonymous = StringLocalizer.BBString("Yes")
            Else
                giftRow.Anonymous = StringLocalizer.BBString("No")
            End If

            If gift.Designations IsNot Nothing Then
                Dim fundDescriptions As New Generic.List(Of String)
                Dim fundDescription As String

                Dim designationIDs = gift.Designations.Where(Function(x) x.BackOfficeID > 0).Select(Function(x) x.BackOfficeID).Distinct.ToArray()
                Dim DesignationDescriptions As Blackbaud.Web.Content.Core.RE7Service.FundData()
                If Not PortalSettings.Current.Features.IsInfinity() Then DesignationDescriptions = FetchData(designationIDs)
                Dim designation As GiftInformation.DesignationInformation

                For Each des In gift.Designations
                    If des.BackOfficeID > 0 AndAlso Not PortalSettings.Current.Features.IsInfinity() Then
                        Try
                            ' Makes LINQ happy
                            designation = des
                            fundDescription = DesignationDescriptions.Where(Function(x) CInt(x.System_ID) = designation.BackOfficeID).Single.Description
                        Catch
                            fundDescription = des.Description
                        End Try
                    Else
                        fundDescription = des.Description
                    End If



                    fundDescriptions.Add(fundDescription)
                    Dim fundRow = fields.Funds.NewFundsRow
                    fundRow.GiftsID = giftRow.ID
                    fundRow.Description = fundDescription
                    fields.Funds.Rows.Add(fundRow)
                Next

                giftRow.FundDescription = String.Join(";", fundDescriptions.ToArray)
            End If
            giftRow.ReceiptKey = gift.EReceiptKey
            If gift.eReceiptNumber > 0 Then
                giftRow.ReceiptNumber = CStr(gift.eReceiptNumber)
            End If

            giftRow.Balance = String.Empty ' We don't count pending as a pledge because unprocessed transactions are not official
            giftRow.BalanceUnformatted = Decimal.MinValue ' We don't count pending as a pledge because unprocessed transactions are not official
            giftRow.PayMethod = GetPaymentMethodFriendlyName(gift.PaymentMethod)
            If gift.Recurrence IsNot Nothing AndAlso gift.Recurrence.Frequency > 0 Then
                giftRow.InstallmentFrequency = gift.Recurrence.Frequency.ToString
                giftRow.InstallmentSchedule = gift.Recurrence.RecurrenceLocalizedFrequencyLabel
            End If

            giftRow.Type = Me.ParentGivingHistory.PendingTextFor(gift.Type)
            giftRow.GiftTypeLanguageGUID = Guid.Empty

            If gift.GiftAid AndAlso giftAidAmount > 0 Then
                giftRow.GiftAidAmount = BBFormatCurrency(giftAidAmount, CurrencySymbol(gift.CurrencyType))
                giftRow.GiftAidAmountUnformatted = giftAidAmount
            ElseIf gift.GiftAid Then
                giftRow.GiftAidAmount = BBFormatCurrency("0")
            End If

      If String.IsNullOrEmpty(giftRow.GiftAidAmount) Then
        giftRow.GiftAidAmountUnformatted = Decimal.MinValue
      Else
        giftRow.GiftAidAmountUnformatted = giftAidAmount
      End If

      giftRow.BbncCurrencyType = gift.CurrencyType

      giftRow.Pending = True
      fields.Gifts.Rows.Add(giftRow)

      If gift.Tribute IsNot Nothing Then
        Dim tribRow = fields.Tributes.NewTributesRow
        tribRow.GiftsID = giftRow.ID
        tribRow.Name = gift.Tribute.Name
        tribRow.TributeDescription = gift.Tribute.Description
        tribRow.TributeType = gift.Tribute.TributeType

        fields.Tributes.Rows.Add(tribRow)
      End If

      'ViM OriginalPledgeID is used to keep track of pending pledge payment (gifts). We want to hide the pay pledge
      'link if a payment is pending.
      'Bug 61029
      If gift.ExistingPledgeBackOfficeID IsNot Nothing Then
        giftRow.OriginalPledgeID = CStr(gift.ExistingPledgeBackOfficeID)
      Else
        giftRow.OriginalPledgeID = String.Empty
      End If

      giftRow.AutoPayCCToken = Guid.Empty
      giftRow.AutoPayDDAcctID = Guid.Empty

      'ViM TODO: Do we need these fields for pending gifts?
      giftRow.RecurringGiftStartDate = String.Empty
      giftRow.RecurringGiftEndDate = String.Empty
      giftRow.RecurringGiftNextTransactionDate = String.Empty
      giftRow.FrequencyCode = 0

      ' Can't do installments/payments yet, as you cannot pay to a pledge yet.
    End If
  End Sub

  'TODO: this should probably be language tabed, but then so should all the data coming back from RE so they match
  Private Function GetPaymentMethodFriendlyName(ByVal method As GiftInformation.EPaymentMethod) As String
    Select Case method
      Case GiftInformation.EPaymentMethod.CreditCard
        Return "Credit Card"
      Case GiftInformation.EPaymentMethod.DirectDebit
        Return "Direct Debit"
      Case Else
        Return method.ToString
    End Select

  End Function

  Private Function IncludeGift(ByVal gift As GiftInformation) As Boolean

    'MSB 4-2-09 CR317351-040209
    If gift.Corporate Then
      Return False
    End If

    Return True
  End Function
End Class
