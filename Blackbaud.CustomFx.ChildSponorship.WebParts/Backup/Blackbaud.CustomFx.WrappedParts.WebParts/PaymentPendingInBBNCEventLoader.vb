Imports Blackbaud.Web.Content.SPWrap
Imports Blackbaud.Web.Content.Core.Data
Imports Blackbaud.Web.Content.Core

'This loader loads BBNC pending gifts only to grab the registration IDs of event regs that have payments made towards them but not processed 
'in the backoffice. If we need to filter the TM on such registrations, we can use the PaymentsInBBNCEvents datatable.
Public Class PaymentPendingInBBNCEventLoader
    Implements ILoader(Of GivingHistoryFields)

    Public Function Load(ByVal fields As GivingHistoryFields) As GivingHistoryFields Implements ILoader(Of GivingHistoryFields).Load
        Dim pendingEventReg As ShelbyEventRegTran

        Dim eventRegResultRow = spTransactions_GetUnprocessedEventTransactionForUser.ExecuteSP(DataObject.AppConnString, BBWebPrincipal.Current.UserID)

        For Each rr In eventRegResultRow
            pendingEventReg = Transactions.EventRegFromXML(rr.XMLObjectData)
            AddPaymentsInBBNCEventsRow(fields, pendingEventReg)
        Next

        Return fields
    End Function

    Private Sub AddPaymentsInBBNCEventsRow(ByVal fields As GivingHistoryFields, ByVal eventReg As ShelbyEventRegTran)
        Dim tableRow = fields.PaymentsInBBNCEvents.NewPaymentsInBBNCEventsRow

        If eventReg.UnpaidEventRegistrationPayment Then
            tableRow.RegistrationID = eventReg.Registrations(0).Fees(0).RegistrationID.ToString
            fields.PaymentsInBBNCEvents.Rows.Add(tableRow)
        End If
    End Sub

End Class

