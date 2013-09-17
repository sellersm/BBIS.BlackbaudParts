Imports Blackbaud.Web.Content.SPWrap
Imports Blackbaud.Web.Content.Core.Data
Imports Blackbaud.Web.Content.Core

'This loader loads BBNC pending gifts only to grab the recordIDs of pledges that have payments made towards them but not processed 
'in the backoffice. If we need to filter the TM on such pledges, we can use the PaymentsInBBNCPledges datatable.

'TODO: If BBNCGivingHistory is already loaded, then fill PaymentsInBBNCPledges datatable from the Gifts datatable
Public Class PaymentPendingInBBNCPledgeLoader
    Implements ILoader(Of GivingHistoryFields)

    Dim _BBNCGivingHistoryLoaded As Boolean

    Public Sub New(ByVal BBNCGivingHistoryLoaded As Boolean)
        _BBNCGivingHistoryLoaded = BBNCGivingHistoryLoaded
    End Sub

    Public Function Load(ByVal fields As GivingHistoryFields) As GivingHistoryFields Implements ILoader(Of GivingHistoryFields).Load
        Dim pendingDonation As ShelbyDonationTran

        Dim donationResultRow = spTransactions_GetUnprocessedDonationTransactionForUser.ExecuteSP(DataObject.AppConnString, BBWebPrincipal.Current.UserID)

        For Each rr In donationResultRow
            pendingDonation = Transactions.DonationFromXML(rr.XMLObjectData)
            AddPaymentsInBBNCPledgesRow(fields, pendingDonation.Gift)
        Next

        Return fields
    End Function

    Private Sub AddPaymentsInBBNCPledgesRow(ByVal fields As GivingHistoryFields, ByVal gift As GiftInformation)
        Dim tableRow = fields.PaymentsInBBNCPledges.NewPaymentsInBBNCPledgesRow

        If gift.IsPaymentTowardsExistingPledge AndAlso gift.ExistingPledgeBackOfficeID.Length > 0 Then
            tableRow.RecordID = CStr(gift.ExistingPledgeBackOfficeID)
            fields.PaymentsInBBNCPledges.Rows.Add(tableRow)
        End If
    End Sub

End Class

'There is a need to produce result sets from GivingHistoryFields based on differences between datatables like Gifts, Funds etc.
'For e.g. We need pledges have don't have a pending payment in BBNC. A way to obtain this result set is to subtract the pledges 
'having pending payments from all pledges. See GivingHistoryDisplay2 ApplyFilters() for a sample.
Public Class GivingHistoryRecordComparer
    Implements Generic.IEqualityComparer(Of GivingHistoryDataRow)

    Public Function Equals1(ByVal x As GivingHistoryDataRow, ByVal y As GivingHistoryDataRow) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of GivingHistoryDataRow).Equals
        If x.Gifts__GiftRecordID = y.Gifts__GiftRecordID Then
            Return True
        End If
    End Function

    Public Function GetHashCode1(ByVal obj As GivingHistoryDataRow) As Integer Implements System.Collections.Generic.IEqualityComparer(Of GivingHistoryDataRow).GetHashCode

    End Function
End Class