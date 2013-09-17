Imports Blackbaud.Web.Content
Imports Blackbaud.Web.Content.Core.Data
Imports Blackbaud.Web.Content.SPWrap

Public Class BBNCGivingHistoryOptionsManager
    Implements Core.IDataObjectManager(Of Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory)

    Private _contentID As Integer
    Public Property ContentID() As Integer
        Get
            Return _contentID
        End Get
        Set(ByVal value As Integer)
            _contentID = value
        End Set
    End Property

    Public Sub New(ByVal RecordID As Integer)
        Me.ContentID = RecordID
    End Sub

    Public Sub New()
    End Sub

    Public Function Save(ByVal target As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory) As Boolean Implements Core.IDataObjectManager(Of Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory).Save
        SaveColumns(target)
        SaveOptions(target)
        'sterling CR333921-012711
        'clear any cached gift data so new filters etc are applied
        Core.DataObject.FlushCache(GetType(Core.GivingHistoryFields).Name)
    End Function

    Private Sub SaveColumns(ByVal target As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory)
        Dim oCommand As SqlClient.SqlCommand = spAddUpdate_GivingHistoryUsedFields.CreateSQLCommand(Core.DataObject.AppConnString, Core.DataObject.ToXML(target.UsedFields), Me.ContentID)

        Try
            oCommand.Connection.Open()
            oCommand.ExecuteNonQuery()
        Finally
            oCommand.Connection.Close()
        End Try
    End Sub

    Private Sub SaveOptions(ByVal target As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory)
        Dim oCommand As SqlClient.SqlCommand = spAddUpdate_GivingHistory2.CreateSQLCommand(Core.DataObject.AppConnString, target.ID, Me.ContentID, HashTableToXML(target.GiftTypes), HashTableToXML(target.Campaigns), HashTableToXML(target.Funds), HashTableToXML(target.Appeals), target.QueryID, target.IncludeSoftCredits, target.PageSize, target.UseCustomFilter, target.UseCustomColumns, target.IncludePending, target.IncludeSummary, target.IncludeGiftTotal, target.IncludeGiftAidTotal, target.IncludePendingTotal, target.IncludePledgeTotal, target.IncludeBalanceTotal, target.IncludeTotalsCurrency, target.PledgePaymentPageID, target.RecurringGiftEditAllowAmtUpdates, target.RecurringGiftEditAllowFreqUpdates, target.RecurringGiftEditAllowPmntTypeUpdates, target.RecurringGiftEditAmtUpdateMinAmt, HashTableToXML(target.RecurringGiftEditFilterFunds), HashTableToXML(target.RecurringGiftEditFilterAppeals), target.RecurringGiftEditFilterQueryID, target.RecurringGiftEditFilterUseCustomFilter, target.RecurringGiftEditFreqUseGeneralRecurrence, target.RecGftEdtFreqIncludeRecurSchdStartDate, target.RecGftEdtFreqIncludeRecurSchdEndDate, Core.DataObject.ToXML(target.RecGiftEditRecurrenceSpecifics), target.RecGiftAdditionalDonationPageID, target.IncludeSoftCreditsTotal, target.IncludeHardCreditsTotal, target.RecurringGiftPaymentPageID, target.IncludeUnpaidEvents, target.UnpaidEventsPaymentPageID)

        Try
            oCommand.Connection.Open()
            oCommand.ExecuteNonQuery()

        Finally
            oCommand.Connection.Close()
        End Try
    End Sub

    Public Function Load(ByVal target As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory) As Boolean Implements Core.IDataObjectManager(Of Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory).Load
        LoadOptions(Me.ContentID, target)
        LoadColumnsByID(Me.ContentID, target)
    End Function

    Private Sub LoadColumnsByID(ByVal iRecordID As Integer, ByVal target As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory)
        Dim oDefaultColNames() As String
        Dim FundColumn As String

        If Core.PortalSettings.Current.Features.IsInfinity Then
            FundColumn = "Designation"
        Else
            FundColumn = "Fund Description"
        End If

        If RE7ServiceHelper.InstalledCountry = Core.RE7Service.bbInstalledCountries.CTY_UK Then
            oDefaultColNames = New String() {"Date", "Gift Type", FundColumn, "Amount", "Gift Aid Amount"}
        Else
            oDefaultColNames = New String() {"Date", "Gift Type", FundColumn, "Amount"}
        End If

        Dim oResultRow As SPWrap.spLoadRecord_GivingHistoryUsedFields.ResultRow()

        oResultRow = SPWrap.spLoadRecord_GivingHistoryUsedFields.ExecuteSP(Core.DataObject.AppConnString, iRecordID)

        If target.UseCustomColumns Then
            For Each rr In oResultRow
                If rr.DisplayOrder > 0 Then
                    Dim uField As New Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryColumn
                    uField.FieldLocalizationGuid = rr.FieldLocalizationGuid
                    uField.DefaultName = Core.StringLocalizer.BBString(rr.DefaultName)
                    uField.DisplayOrder = rr.DisplayOrder
                    uField.FieldID = rr.FieldID
                    uField.ColumnName = rr.ColumnName
                    uField.TableName = rr.TableName
                    uField.ColumnSortType = DirectCast(rr.SortType, GivingHistoryColumn.SortType)

                    If CanAddField(uField) Then
                        target.UsedFields.Columns.Add(uField)
                    End If
                Else
                    Dim aField As New Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryColumn
                    aField.ColumnName = rr.ColumnName
                    aField.TableName = rr.TableName
                    aField.FieldLocalizationGuid = rr.FieldLocalizationGuid
                    aField.FieldID = rr.ID
                    aField.DefaultName = Core.StringLocalizer.BBString(rr.DefaultName)
                    aField.ColumnSortType = DirectCast(rr.SortType, GivingHistoryColumn.SortType)

                    If CanAddField(aField) Then
                        target.AvailableFields.Columns.Add(aField)
                    End If
                End If
            Next
        Else
            'Load Default Columns: Date, Type, Fund Description, Amount
            For Each rr In oResultRow
                If oDefaultColNames.Contains(rr.DefaultName) Then
                    Dim uField As New Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryColumn
                    uField.FieldLocalizationGuid = rr.FieldLocalizationGuid
                    uField.DefaultName = Core.StringLocalizer.BBString(rr.DefaultName)
                    uField.FieldID = rr.ID
                    uField.ColumnName = rr.ColumnName
                    uField.TableName = rr.TableName
                    uField.DisplayOrder = Array.IndexOf(oDefaultColNames, rr.DefaultName) + 1
                    uField.ColumnSortType = DirectCast(rr.SortType, GivingHistoryColumn.SortType)

                    If CanAddField(uField) Then
                        target.UsedFields.Columns.Add(uField)
                    End If
                Else
                    Dim aField As New Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryColumn
                    aField.ColumnName = rr.ColumnName
                    aField.TableName = rr.TableName
                    aField.FieldLocalizationGuid = rr.FieldLocalizationGuid
                    aField.FieldID = rr.ID
                    aField.DefaultName = Core.StringLocalizer.BBString(rr.DefaultName)
                    aField.ColumnSortType = DirectCast(rr.SortType, GivingHistoryColumn.SortType)

                    If CanAddField(aField) Then
                        target.AvailableFields.Columns.Add(aField)
                    End If
                End If
            Next
        End If

        target.AvailableFields.Columns.Sort(Function(lhs, rhs) lhs.DefaultName.CompareTo(rhs.DefaultName))
        target.UsedFields.Columns.Sort(Function(lhs, rhs) lhs.DisplayOrder.CompareTo(rhs.DisplayOrder))
    End Sub

    Private Sub LoadOptions(ByVal iRecordID As Integer, ByVal target As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory)
        Dim oResultRow As SPWrap.spLoadRecord_GivingHistory2.ResultRow()

        oResultRow = SPWrap.spLoadRecord_GivingHistory2.ExecuteSP(Core.DataObject.AppConnString, iRecordID)

        'bug 84054 - ajm - general recurrence should be default
        If oResultRow.Length = 0 Then
            target.RecurringGiftEditFreqUseGeneralRecurrence = True 'default this to true
        Else
            For Each rr In oResultRow
                target.ID = rr.ID
                target.GiftTypes = HashTableFromXML(rr.GiftTypes)
                target.Campaigns = HashTableFromXML(rr.Campaigns)
                target.Funds = HashTableFromXML(rr.Funds)
                target.Appeals = HashTableFromXML(rr.Appeals)
                target.QueryID = rr.GiftQueryID
                target.IncludeSoftCredits = rr.IncludeSoftCredit
                target.IncludeSummary = rr.IncludeSummary
                target.IncludeGiftTotal = rr.IncludeGiftTotal
                target.IncludeGiftAidTotal = rr.IncludeGiftAidTotal
                target.IncludePendingTotal = rr.IncludePendingTotal
                target.IncludePledgeTotal = rr.IncludePledgeTotal
                If rr.ResultsPerPage > 0 Then
                    target.PageSize = rr.ResultsPerPage
                End If
                target.UseCustomFilter = rr.UseCustomFilter
                target.UseCustomColumns = rr.UseCustomColumn
                target.IncludePending = rr.IncludePending
                target.IncludeBalanceTotal = rr.IncludeBalanceTotal
                target.IncludeTotalsCurrency = rr.IncludeTotalsCurrency
                target.PledgePaymentPageID = rr.PledgePaymentPageID
                target.RecurringGiftEditAllowAmtUpdates = rr.RecGiftEditAllowAmtUpdate
                target.RecurringGiftEditAllowFreqUpdates = rr.RecGiftEditAllowFreqUpdates
                target.RecurringGiftEditAllowPmntTypeUpdates = rr.RecGiftEditAllowPmntTypeUpdate
                target.RecurringGiftEditAmtUpdateMinAmt = rr.RecGiftEditMinUpdateAmt
                target.RecurringGiftEditFilterAppeals = HashTableFromXML(rr.RecGiftEditFilterAppeals)
                target.RecurringGiftEditFilterFunds = HashTableFromXML(rr.RecGiftEditFilterFunds)
                target.RecurringGiftEditFilterQueryID = rr.RecGiftEditFilterQueryID
                target.RecurringGiftEditFilterUseCustomFilter = rr.RecGiftEditFilterUseCustomFilter
                target.RecurringGiftEditFreqUseGeneralRecurrence = rr.RecGiftEditFreqUseGeneralRecurrence
                target.RecGftEdtFreqIncludeRecurSchdStartDate = rr.RecGiftEditFreqAllowStartDateUpdates
                target.RecGftEdtFreqIncludeRecurSchdEndDate = rr.RecGiftEditFreqAllowEndDateUpdates
                target.RecGiftEditRecurrenceSpecifics = DirectCast(Core.DataObject.FromXML(Of String())(rr.RecGiftEditFreqSpecifics), String())
                target.RecGiftAdditionalDonationPageID = rr.RecGiftAdditionalDonationPageID
                target.IncludeSoftCreditsTotal = rr.IncludeSoftCreditsTotal
                target.IncludeHardCreditsTotal = rr.IncludeHardCreditsTotal
                target.RecurringGiftPaymentPageID = rr.RecurringGiftPaymentPageID
                target.IncludeUnpaidEvents = rr.IncludeUnpaidEvents
                target.UnpaidEventsPaymentPageID = rr.UnpaidEventsPaymentPageID
                target.PledgePaymentCurrencyID = GetPaymentCurrencyID(target.PledgePaymentPageID)
                target.RecurringGiftPaymentCurrencyID = GetPaymentCurrencyID(target.RecurringGiftPaymentPageID)
            Next
        End If
    End Sub

    Public Function BeforeContentDelete(ByVal ContentID As Integer) As Boolean Implements Core.IDataObjectManager(Of Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory).BeforeContentDelete
        SPWrap.spDelete_GivingHistory2.ExecuteNonQuery(Core.DataObject.AppConnString, ContentID, Core.BBWebPrincipal.Current.UserID)
    End Function

    Public Function CloneByID(ByVal CloneID As Integer, ByVal ClonedID As Integer, ByVal NewName As String, ByVal NewGuid As System.Guid) As Integer Implements Core.IDataObjectManager(Of Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory).CloneByID
        Dim oContent As New Core.Content(CloneID)
        Dim newID As Integer

        SPWrap.spClone_GivingHistory2.ExecuteNonQuery(Core.DataObject.AppConnString, newID, CloneID, ClonedID, Core.BBWebPrincipal.Current.UserID, NewGuid)
        oContent.CustomProperty("ClientGivingHistory2ID") = newID.ToString
        oContent.Save()
    End Function

    Private Function CanAddField(ByVal field As GivingHistoryColumn) As Boolean
        Dim canAdd As Boolean = True

        If Core.PortalSettings.Current.Features.IsRE7 Then
            If field.DefaultName = "Designation" OrElse _
             field.DefaultName = "Designation Lookup ID" OrElse _
             field.DefaultName = "Gift Lookup ID" OrElse _
             field.DefaultName = "Campaign Lookup ID" OrElse _
             field.DefaultName = "Site Hierarchy" OrElse _
             field.DefaultName = "Event Name" OrElse _
             field.DefaultName = "Event Details" OrElse _
             field.DefaultName = "Purpose" Then
                canAdd = False
            End If
        End If

        If Core.PortalSettings.Current.Features.IsInfinity Then
            If field.DefaultName = "Fund Description" OrElse _
                field.DefaultName = "Fund ID" OrElse _
                field.DefaultName = "Gift ID" OrElse _
                field.DefaultName = "Campaign ID" OrElse _
                field.DefaultName = "Installment Schedule" Then
                'ViM bug 68242 fix. Installment schedule is the same as frequency. We need additional fields like start date, 
                'end date & next transaction date for BBEC TM.
                canAdd = False
            End If
        End If

		If field.ColumnName = "GiftAidAmount" Then
            If Core.Data.RE7ServiceHelper.InstalledCountry <> Core.RE7Service.bbInstalledCountries.CTY_UK Then
                canAdd = False
            End If
		End If

		Return canAdd
    End Function

    Private Function GetPaymentCurrencyID(ByVal paymentPageID As Integer) As Guid?
        If paymentPageID = 0 Then
            Return Nothing
        Else
            Dim merchantAccountID As Guid = Core.MerchantAccount.GetMerchantAccount(paymentPageID)
            Dim MACurrencyISO As String = Core.MerchantAccount.GetCurrencyISO(merchantAccountID.ToString)
            Dim merchantAccountCurrencyID As String = Core.Internationalization.Currency.GetCurrencyFromISO(MACurrencyISO)

            If Len(merchantAccountCurrencyID) > 0 Then
                Return New Guid(merchantAccountCurrencyID)
            Else
                Return Nothing
            End If
        End If
    End Function
End Class
