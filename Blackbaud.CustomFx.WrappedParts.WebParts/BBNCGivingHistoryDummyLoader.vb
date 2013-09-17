Imports Blackbaud.Web.Content.Core.Data
Imports Blackbaud.Web.Content.Core
Imports Blackbaud.Web.Content

Public Class BBNCGivingHistoryDummyLoader
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

    ' TODO: Make currency symbol localized
    Public Function Load(ByVal target As GivingHistoryFields) As GivingHistoryFields Implements ILoader(Of GivingHistoryFields).Load
        Dim gr As GivingHistoryFields.GiftsRow

        gr = FillGiftsRow(target.Gifts.NewGiftsRow(), 35, Decimal.MinValue, "Campaign1", "5/5/2001", "New Building Fund", "New Building Fund Description", "Monthly", "Schedule", "1", "Cash", True, "1/1/2001", "32893", "56000", String.Empty)
        target.Gifts.Rows.Add(gr)

        gr = FillGiftsRow(target.Gifts.NewGiftsRow(), 50, 50, "Campaign2", "6/6/2001", "Children In Need", "Children In Need Description", "Weekly", "Schedule", "7", "Cash", True, "1/1/2001", "32894", "56001", String.Empty)
        target.Gifts.Rows.Add(gr)

        gr = FillGiftsRow(target.Gifts.NewGiftsRow(), 15, Decimal.MinValue, "Campaign3", "7/7/2001", "Food Drive", "Food Drive Description", "Once", "Schedule", "3", "Cash", True, "1/1/2001", "32895", "56002", String.Empty)
        target.Gifts.Rows.Add(gr)

        Return target
    End Function

    Private Function FillGiftsRow(ByVal row As GivingHistoryFields.GiftsRow, ByVal Amount As Decimal, ByVal Balance As Decimal, ByVal Campaign As String, ByVal [Date] As String, ByVal Fund As String, ByVal FundDescription As String, ByVal Installment_Frequency As String, ByVal Installment_schedule As String, ByVal Number_of_Installments As String, ByVal Pay_method As String, ByVal Pending As Boolean, ByVal Receipt_Date As String, ByVal Receipt_number As String, ByVal REGift_RecordID As String, ByVal Type As String) As GivingHistoryFields.GiftsRow

        Dim giftAidAmount = Core.Data.DonationTransaction.GiftAidAmount(Convert.ToDateTime([Date]), Amount)
        Dim currency = Core.MerchantAccountBBPS.BLL.MATransactionInfo.InstalledCurrencyType()

        row.ID = System.Guid.NewGuid()
        row.Amount = If(Amount = Decimal.MinValue, String.Empty, BBFormatCurrency(Amount, currency))
        row.Anonymous = "No"
        row.AmountUnformatted = Amount
        row.Balance = If(Balance = Decimal.MinValue, String.Empty, BBFormatCurrency(Balance, currency))
        row.BalanceUnformatted = Balance
        row.BbncCurrencyType = currency
        'row.Campaign = Campaign
        row._Date = [Date]
        row.Fund = Fund
        row.FundDescription = FundDescription
        row.GiftAidAmount = If(giftAidAmount = Decimal.MinValue, String.Empty, BBFormatCurrency(giftAidAmount, currency))
        row.GiftAidAmountUnformatted = giftAidAmount
        row.InstallmentFrequency = Installment_Frequency
        row.InstallmentSchedule = Installment_schedule
        row.NumberOfInstallments = Number_of_Installments
        row.NumberOfInstallmentsUnformatted = CInt(Number_of_Installments)
        row.PayMethod = Pay_method
        row.Pending = Pending
        row.ReceiptDate = Receipt_Date
        row.ReceiptNumber = Receipt_number
        row.GiftRecordID = REGift_RecordID
        row.Type = Me.ParentGivingHistory.PendingTextFor(Type)
        row.GiftTypeLanguageGUID = Guid.Empty
        row.IsSoftCredit = False
        row.OriginalPledgeID = String.Empty

        Return row
    End Function
End Class
