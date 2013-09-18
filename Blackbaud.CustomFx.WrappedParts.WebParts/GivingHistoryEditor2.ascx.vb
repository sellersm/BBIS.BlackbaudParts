Imports Blackbaud.Web.Content.Core.Data
Imports System.Collections.Generic
Imports Blackbaud.AppFx.Products
Imports Blackbaud.Web.Content.Core
Imports Blackbaud.Web.Content
Imports System.Drawing
Imports Blackbaud.Web.Content.Portal

Partial Public Class GivingHistoryEditor2
    Inherits ContentEditControl

    Public Const OBJECT_DELIMITER As Char = ":"c
    Public Const ATTRIBUTE_DELIMITER As Char = "|"c

    Private Const SCRIPT_KEY As String = "script_miscfunctions"
    Private Const SCRIPT_FILE As String = "Client/Scripts/MiscFunctions.js"
    Private Const CP_GIVINGHISTORY As String = "ClientGivingHistory2ID"
    Private Const RECURRENCE_GRID_KEY As String = "Gh2RecurrenceGrid"
    Private Const RECURRENCE_SPECIFIC_KEY As String = "Gh2RecurrenceSpecifics"
    Private m_oGivingHistory As Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory
    Private backOfficeSystemName, backOfficeCFAText, backOfficeCampaignText, backOfficeFundText, softCreditText, acknowledgeText, giftTotalsHelpText, queryTypeTextLbl, queryTypeTextRdo As String

    Private Function InstanceID() As String
        Return String.Concat(Me.ID, BBSession.SessionID)
    End Function

    Private ReadOnly Property GridSessionKey(ByVal sKey As String) As String
        Get
            Return String.Concat(sKey, Me.InstanceID.GetHashCode.ToString)
        End Get
    End Property

    Private _hideGiftFilter As Boolean
    Public Property HideGiftFilter() As Boolean
        Get
            Return _hideGiftFilter
        End Get
        Set(ByVal value As Boolean)
            _hideGiftFilter = value
        End Set
    End Property

    Private _hidePledge As Boolean
    Public Property HidePledge() As Boolean
        Get
            Return _hidePledge
        End Get
        Set(ByVal value As Boolean)
            _hidePledge = value
        End Set
    End Property

    Private _showAdditionalDonation As Boolean
    Public Property ShowAdditionalDonation() As Boolean
        Get
            Return _showAdditionalDonation
        End Get
        Set(ByVal value As Boolean)
            _showAdditionalDonation = value
        End Set
    End Property

    Private _recurrenceGridDataSource As Generic.List(Of DataLists.TopLevel.RecurrenceRecordsDatalistRow)
    Public ReadOnly Property RecurrenceGridDataSource() As Generic.List(Of DataLists.TopLevel.RecurrenceRecordsDatalistRow)
        Get
            If _recurrenceGridDataSource Is Nothing Then
                Dim sKey As String = GridSessionKey(RECURRENCE_GRID_KEY)
                If BBSession.Item(sKey) Is Nothing Then
                    _recurrenceGridDataSource = New Generic.List(Of DataLists.TopLevel.RecurrenceRecordsDatalistRow)()
                    'get from DB
                    Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
                    Dim filterData = New DataLists.TopLevel.RecurrenceRecordsDatalistFilterData
                    filterData.IDs = ""
                    Dim recurrenceRecords() As DataLists.TopLevel.RecurrenceRecordsDatalistRow
                    If m_oGivingHistory IsNot Nothing AndAlso m_oGivingHistory.RecGiftEditRecurrenceSpecifics IsNot Nothing AndAlso m_oGivingHistory.RecGiftEditRecurrenceSpecifics.Count > 0 Then
                        Dim ids As String = String.Join(",", m_oGivingHistory.RecGiftEditRecurrenceSpecifics)
                        If Not String.IsNullOrEmpty(ids) Then
                            filterData.IDs = ids
                            recurrenceRecords = DataLists.TopLevel.RecurrenceRecordsDatalist.GetRows(provider, filterData)
                            _recurrenceGridDataSource.AddRange(recurrenceRecords)

                            Dim sKeySpecifics As String = GridSessionKey(RECURRENCE_SPECIFIC_KEY)
                            If BBSession.Item(sKey) Is Nothing Then
                                BBSession.Add(sKeySpecifics, m_oGivingHistory.RecGiftEditRecurrenceSpecifics)
                            Else
                                BBSession.Item(sKeySpecifics) = m_oGivingHistory.RecGiftEditRecurrenceSpecifics
                            End If

                        End If
                    End If

                    BBSession.Add(sKey, _recurrenceGridDataSource)
                Else
                    _recurrenceGridDataSource = DirectCast(BBSession.Item(sKey), Generic.List(Of DataLists.TopLevel.RecurrenceRecordsDatalistRow))
                End If
            End If

            Return _recurrenceGridDataSource
        End Get
    End Property

    Public Overrides Sub InitDiscoveryInfo()
        'MyBase.InitDiscoveryInfo()

        Me.mDiscoveryInfo = New DiscoverableBase(TypeName(Me), "Giving History 2.0", "", "")
        Dim oToolbarGroup As New ToolbarGroup()
        oToolbarGroup.AddItem(New AdminHelpVerb(EContextHelpIDs.ContextHelpID_GivingHistory2))
        Me.mDiscoveryInfo.AddItem(oToolbarGroup)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitJavascript()

        If PortalSettings.Current.Features.IsRE7 Then
            RecurringGiftPaymentPanel.Visible = False   'bugs 137056,137412
            divRecurringGiftUpdates.Visible = False
            divUnpaidEvents.Visible = False
            UnpaidEventPanel.Visible = False
        End If

        If Me.HideGiftFilter Then
            divGiftsFiltering.Visible = False
            divPendingTotal.Visible = False
        End If
        If Me.HidePledge Then PledgePanel.Visible = False

        ' TODO: shouldn't we do something with IsPostBack? ...

        'If Not Page.IsPostBack Then
        Dim cacheManager As New WebSessionManager()
        Dim re7Loader As New RE7GivingHistoryLoader(cacheManager, Me.ContentObject.ContentID.ToString)
        Dim bbncLoader As New BBNCGivingHistoryPendingLoader()
        Dim optionsManager = New BBNCGivingHistoryOptionsManager(Me.ContentId)
        Dim historyLoaders As New Generic.List(Of ILoader(Of GivingHistoryFields))
        historyLoaders.Add(re7Loader)
        historyLoaders.Add(bbncLoader)
        m_oGivingHistory = New Blackbaud.CustomFx.WrappedParts.WebParts.Data.GivingHistory(historyLoaders) ' TODO: access the property...not the private var
        m_oGivingHistory.OptionsManager = optionsManager

        'CR318371-042109 - ajm - localize text
        lblCustomColumnsHelp.Text = StringLocalizer.BBString(lblCustomColumnsHelp.Text)
        lblCustomPanelHelp.Text = StringLocalizer.BBString(lblCustomPanelHelp.Text)
        lblDefaultPanelHelp.Text = StringLocalizer.BBString(lblDefaultPanelHelp.Text)

        If Not Page.IsPostBack Then
            optionsManager.Load(m_oGivingHistory)
            re7Loader.ParentGivingHistory = m_oGivingHistory
            bbncLoader.ParentGivingHistory = m_oGivingHistory
            ClearSession() 'clear specific recurring grid
            DataBind()
        End If

        BackOfficeCaptioning()
    End Sub


#Region "Language"
    Protected Overrides Sub InitializeLanguageData()
        With LanguageData
            Dim group As String = ""


            ' Columns
            group = "Transaction Manager - Columns"
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_DATE, Nothing, "Date", "Date", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_AMOUNT, Nothing, "Amount", "Amount", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_TYPE, Nothing, "Gift type", "Gift type", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_BALANCE, Nothing, "Balance", "Balance", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_CAMPAIGN_DESCRIPTION, Nothing, "Campaign description", "Campaign description", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_INSTALLMENT_FREQUENCY, Nothing, "Installment frequency", "Installment frequency", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_INSTALLMENT_SCHEDULE, Nothing, "Installment schedule", "Installment schedule", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_NUMBER_OF_INSTALLMENTS, Nothing, "Number of installments", "Number of installments", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_PAY_METHOD, Nothing, "Pay method", "Pay method", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_PENDING, Nothing, "Pending", "Pending", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_RECEIPT_AMOUNT, Nothing, "Receipt amount", "Receipt amount", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_RECEIPT_DATE, Nothing, "Receipt date", "Receipt date", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_RECEIPT_NUMBER, Nothing, "Receipt number", "Receipt number", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_DONORNAME_PUBLICNAME, Nothing, "Donor name", "Donor name", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            If Core.Data.RE7ServiceHelper.InstalledCountry = RE7Service.bbInstalledCountries.CTY_UK Then
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_AID_AMOUNT, Nothing, "Gift aid amount", "Gift aid amount", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            End If
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_ANONYMOUS, Nothing, "Anonymous", "Anonymous", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_SUBTYPE, Nothing, "Gift subtype", "Gift subtype", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_APPEAL_DESCRIPTION, Nothing, "Appeal description", "Appeal", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            If PortalSettings.Current.Features.IsInfinity Then
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_DESIGNATION_LOOKUPID, Nothing, "Designation lookup ID", "Designation lookup ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_DESIGNATION_PUBLICNAME, Nothing, "Designation public name", "Designation", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_LOOKUPD, Nothing, "Gift lookup ID", "Gift lookup ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_CAMPAIGN_LOOKUPID, Nothing, "Campaign lookup ID", "Campaign lookup ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_SITE_HEIRARCHY, Nothing, "Site hierarchy", "Site hierarchy", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_EVENT_NAME, Nothing, "Event name", "Event name", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_EVENT_DETAILS, Nothing, "Event details", "Event details", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_FUNDRAISING_PURPOSE, Nothing, "Fundraising purpose", "Purpose", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            Else
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_FUND_ID, Nothing, "Fund ID", "Fund ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_FUND_DESCRIPTION, Nothing, "Fund description", "Fund description", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_ID, Nothing, "Gift ID", "Gift ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
                .RegisterLanguageField(New FieldInfo(GivingHistoryLanguage.LanguageGuids.FIELD_CAMPAIGN_ID, Nothing, "Campaign ID", "Campaign ID", group, FieldInfo.ELanguageStringType.Heading, LocalizeName:=True, LocalizeDefault:=True))
            End If


        End With

        MyBase.InitializeLanguageData()
    End Sub


    Private Class LanguageGuids
        'tabs
        Public Const FULL_HISTORY_TAB As String = "048e8456-6eed-4e8f-b891-b1c79a4b8bc4"
        Public Const ACTIVE_GIFTS_TAB As String = "0f03bd22-22ae-4cc4-be41-2cec7e0a7afc"

        'tab content help
        Public Const HELPTEXT_ACTIVE_GRID As String = "3e46accb-f17e-42c4-93c2-4602524da101"
        Public Const HELPTEXT_FULL_HISTORY_GRID As String = "fdb7e8a7-a15f-47d7-8c58-8b66619b0745"
    End Class
#End Region

    Private Sub BackOfficeCaptioning()
        If PortalSettings.Current.Features.IsInfinity Then
            backOfficeSystemName = "Blackbaud Enterprise"
            backOfficeCFAText = "designations and appeals"
            backOfficeFundText = "Designations"
            softCreditText = "revenue recognition"
            acknowledgeText = "acknowledges"
            giftTotalsHelpText = " The date range and designation filters also apply to this section."
            queryTypeTextLbl = "Revenue Query"
            queryTypeTextRdo = "Revenue query"
        Else
            backOfficeSystemName = "Raiser's Edge"
            backOfficeCFAText = "campaigns, funds and appeals"
            backOfficeCampaignText = "Campaigns"
            backOfficeFundText = "Funds"
            softCreditText = "soft credits"
            acknowledgeText = "acknowledge"
            giftTotalsHelpText = " The date range and fund filters also apply to this section."
            queryTypeTextLbl = "Gift Query"
            queryTypeTextRdo = "Gift query"
        End If

        If Not Page.IsPostBack Then
            lblQueryBackOfficeName.Text = backOfficeSystemName
            lblRecGftUpdBOText.Text = backOfficeSystemName
            lblBackOfficeCFA.Text = backOfficeCFAText
            lblRecGftUpdBOCFA.Text = backOfficeCFAText
            lblBackOfficeName.Text = backOfficeSystemName
            lblBackOfficeCFA2.Text = backOfficeCFAText
            lblRecGftUpdBOCFA2.Text = backOfficeCFAText
            lblBackOfficeCampaign.Text = backOfficeCampaignText
            lblBackOfficeFund.Text = backOfficeFundText
            lblRecGftUpdBOFundsText.Text = backOfficeFundText
            lblFundColumn.Text = If(PortalSettings.Current.Features.IsInfinity, "Designation", "Fund Description")
            SoftCreditDisplayCheckBox.Text = String.Format("Include {0}", softCreditText)
            lblSoftCreditHelp.Text = String.Format("If you include {0}, the total gift amount may exceed the amount given by the user because {1} {2} multiple donors for a single gift. For information about {3}, ", softCreditText, softCreditText, acknowledgeText, softCreditText)
            lblGiftTotalsHelp.Text = giftTotalsHelpText
            lblBOQueryType.Text = queryTypeTextLbl & ":"
            GiftsREQueryRadioButton.Text = queryTypeTextRdo
            chkIncludeSoftCreditTotal.Text = "Include " + softCreditText + " total"
            SoftCreditTotalLiteral.Text = "This option adds a column to display the total " + softCreditText + " amount."
            HardCreditTotalLiteral.Text = " This option adds a column to display the total donation amount. This total excludes pending gifts, pledges, and " + softCreditText + "."
        End If
    End Sub

    Public Overrides Sub DataBind()
        With m_oGivingHistory

            If .PageSize > 0 Then
                ResultsPerPageDropDownList.SelectedValue = .PageSize.ToString
            End If

            GiftsCustomRadioButton.Checked = .UseCustomFilter
            If .QueryID > 0 Then
                GiftsREQueryRadioButton.Checked = True
                GiftQueryLink.QueryId = .QueryID
            End If

            SoftCreditDisplayCheckBox.Checked = .IncludeSoftCredits
            If Not SoftCreditDisplayCheckBox.Checked Then
                SoftCreditsPanel.Style.Add("display", "none")
            End If
            chkIncludeSoftCreditTotal.Checked = .IncludeSoftCreditsTotal
            chkIncludeHardCreditTotal.Checked = .IncludeHardCreditsTotal

            PendingTransactionDisplayCheckBox.Checked = .IncludePending
            cbUnpaidEvents.Checked = .IncludeUnpaidEvents
            If Not cbUnpaidEvents.Checked Then
                UnpaidEventPanel.Style.Add("display", "none")
            End If
            ColumnCustomRadioButton.Checked = .UseCustomColumns
            LoadLabelWithGiftTypes(Me.lblGiftTypes, .GiftTypes)

            'ViM BBEC Giving History filtering based on Purposes is not a requirement at this point.
            If .ShowCampaigns Then
                LoadLabel(Me.lblCampaigns, .Campaigns, RecordSearchType.Campaign, Core.Data.RE7ServiceHelper.BOSystemText(Core.BackOfficeSystemEnums.eBOTextIndex.eCampaignText, True, "Campaigns"), Me.HiddenSelectedCampaignIDs, False)
            Else
                tr_CampaignFilter.Visible = False
            End If

            LoadLabel(Me.lblFunds, .Funds, RecordSearchType.Fund, Core.Data.RE7ServiceHelper.BOSystemText(Core.BackOfficeSystemEnums.eBOTextIndex.eFilterFund, True, "Funds"), Me.HiddenSelectedFundIDs, False)
            LoadLabel(Me.lblAppeals, .Appeals, RecordSearchType.Appeal, "Appeals", Me.HiddenSelectedAppealIDs, False)

            If (Me.HidePledge) Then
                PledgeBalanceTotalPanel.Visible = False
                PledgeTotalCheckBoxPanel.Visible = False
            Else
                Me.chkIncludePledgeTotal.Checked = .IncludePledgeTotal
                Me.chkIncludeBalanceTotal.Checked = .IncludeBalanceTotal

                'ViM Page link control to pick donation page for taking pledge payments
                If .PledgePaymentPageID > 0 Then
                    Try
                        Me.PledgePaymentPageLink.PageID = .PledgePaymentPageID
                    Catch ex As Exception
                    End Try
                End If
            End If

            Me.chkIncludeSummary.Checked = .IncludeSummary
            Me.chkIncludeGiftTotal.Checked = .IncludeGiftTotal
            Me.chkIncludeGiftAidTotal.Checked = .IncludeGiftAidTotal
            Me.chkIncludePendingTotal.Checked = .IncludePendingTotal
            Me.chkIncludeTotalsCurrency.Checked = .IncludeTotalsCurrency


            'recurring gift updates
            'filter options
            If .RecurringGiftEditFilterUseCustomFilter Then
                rdoRecGftUpdFilterCustom.Checked = True
            ElseIf .RecurringGiftEditFilterQueryID > 0 Then
                rdoRecGftUpdFilterQuery.Checked = True
                RecGftUpdFilterQueryLnk.QueryId = .RecurringGiftEditFilterQueryID
            Else
                rdoRecGftUpdFilterAll.Checked = True
            End If

            LoadLabel(Me.lblRecGftUpdFundsChosen, .RecurringGiftEditFilterFunds, RecordSearchType.Fund, Core.Data.RE7ServiceHelper.BOSystemText(Core.BackOfficeSystemEnums.eBOTextIndex.eFilterFund, True, "Funds"), Me.HiddenSelectedRecGiftEditFilterFundIDs, True)
            LoadLabel(Me.lblRecGftUpdAppealsChosen, .RecurringGiftEditFilterAppeals, RecordSearchType.Appeal, "Appeals", Me.HiddenSelectedRecGiftEditFilterAppealIDs, True)

            chkAllowRecurringAmountUpdates.Checked = .RecurringGiftEditAllowAmtUpdates
            chkAllowFrequencyUpdates.Checked = .RecurringGiftEditAllowFreqUpdates
            chkAllowPaymentTypeUpdates.Checked = .RecurringGiftEditAllowPmntTypeUpdates

            txtAllowRecAmtUpdatesMinAmt.Text = .RecurringGiftEditAmtUpdateMinAmt.ToString

            'recurrence control for updating recurring gifts frequency options
            If .RecurringGiftEditFreqUseGeneralRecurrence Then
                Me.generalRecurrenceRadio.Checked = True
            Else
                Me.specificRecurrenceRadio.Checked = True
            End If

            RecurringGiftScheduleStartDateCheckBox.Checked = .RecGftEdtFreqIncludeRecurSchdStartDate
            RecurringGiftScheduleEndDateCheckBox.Checked = .RecGftEdtFreqIncludeRecurSchdEndDate

            Me.RecurrenceDataGrid.DataSource = RecurrenceGridDataSource
            Me.RecurrenceDataGrid.DataBind()
            Me.pnlAdditionalDonation.Visible = ShowAdditionalDonation

            If ShowAdditionalDonation AndAlso .RecGiftAdditionalDonationPageID > 0 Then
                Try
                    Me.RecGiftAdditionalDonationPageLink.PageID = .RecGiftAdditionalDonationPageID
                Catch ex As Exception
                End Try
            End If

            If .RecurringGiftPaymentPageID > 0 Then
                Try
                    Me.RecGiftPaymentPageLink.PageID = .RecurringGiftPaymentPageID
                Catch ex As Exception

                End Try
            End If

            If .UnpaidEventsPaymentPageID > 0 Then
                Try
                    Me.UnpaidEventPageLink.PageID = .UnpaidEventsPaymentPageID
                Catch ex As Exception
                End Try
            End If

        End With

        If RE7ServiceHelper.InstalledCountry <> RE7Service.bbInstalledCountries.CTY_UK Then
            Me.chkIncludeGiftAidTotal.Visible = False
            Me.SpecificColumnsUK.Visible = False
        End If

        LoadColumns()
    End Sub

    Public Overrides Sub RaisePostBackEvent(ByVal eventArgument As String)
        MyBase.RaisePostBackEvent(eventArgument)
        If eventArgument.ToUpper.Equals(BBDirectPostBackVerb.BBSaveVerb.SaveID) Then
            If Not DirectCast(Page, IBBWebPageWithValidation).ValidateGroup(Me.ValidationGroup) Then
                Return
            End If
            ModalDialogRedirect.AddDialogCloseScript(Me, eventArgument)
        End If
    End Sub

    Private Sub InitJavascript()
        GiftsDefaultRadioButton.Attributes("onClick") = "ChangePanel(2,'GiftsDefault');"
        GiftsCustomRadioButton.Attributes("onClick") = "ChangePanel(2,'GiftsCustom');"
        GiftsREQueryRadioButton.Attributes("onClick") = "ChangePanel(2,'GiftsREQuery');"

        ColumnDefaulRadioButton.Attributes("onClick") = "ChangePanel(3,'ColumnDefault');"
        ColumnCustomRadioButton.Attributes("onClick") = "RenderColumns();ChangePanel(3,'ColumnCustom');"

        rdoRecGftUpdFilterAll.Attributes("onClick") = "ChangePanel(4,'RecGftUpdFilterDefault');"
        rdoRecGftUpdFilterCustom.Attributes("onClick") = "ChangePanel(4,'RecGftUpdFilterCustom');"
        rdoRecGftUpdFilterQuery.Attributes("onClick") = "ChangePanel(4,'RecGftUpdFilterQuery');"

        specificRecurrenceRadio.Attributes("onClick") = "ChangePanel(5,'specificRecurrence');"
        generalRecurrenceRadio.Attributes("onClick") = "ChangePanel(5,'generalRecurrence');"
    End Sub


    Private Sub LoadColumns()

        Dim availableFieldIDs As New List(Of String)
        Dim availableFieldNames As New List(Of String)

        For Each availableField In m_oGivingHistory.AvailableFields.Columns
            availableFieldIDs.Add(availableField.FieldID.ToString())
            availableFieldNames.Add(GetLanguageFieldName(availableField.FieldLocalizationGuid))
        Next



        If availableFieldIDs.Count > 0 Then
            Dim availList = String.Concat(String.Join(",", availableFieldIDs.ToArray()), ATTRIBUTE_DELIMITER, String.Join(",", availableFieldNames.ToArray()))
            Me.HiddenAvailableColumnData.Value = FormHelper.DoScriptVariableSafeInject(availList)
        End If

        If m_oGivingHistory.UsedFields.Columns.Count > 0 Then
            Me.HiddenUsedColumnData.Value = FormHelper.DoScriptVariableSafeInject(FormHelper.JsonFromObject(m_oGivingHistory.UsedFields))
            Me.HiddenUsedColumnString.Value = Me.HiddenUsedColumnData.Value
        End If
    End Sub


    Private Function GetLanguageFieldName(ByVal ID As System.Guid) As String
        Dim retValue As String = ""
        For Each item As System.Collections.Generic.KeyValuePair(Of System.Guid, Web.Content.Core.FieldInfo) In LanguageData.Data
            Dim info As Web.Content.Core.FieldInfo = item.Value
            If ID = info.Id Then
                retValue = LanguageData.GetLanguageString(info.Id, LanguageMetaData.Encoding.Raw)
                Exit For
            End If
        Next
        Return retValue
    End Function

    Private Function SessionKey(ByVal searchType As RecordSearchType, ByVal isRecGiftFilter As Boolean) As String
        If isRecGiftFilter Then
            Return String.Concat("GivingHistoryRecGiftFilter:", searchType.ToString, ":", Me.ContentId)
        Else
            Return String.Concat("GivingHistoryFilter:", searchType.ToString, ":", Me.ContentId)
        End If
    End Function

    Private Function CFASelection(ByVal searchType As RecordSearchType) As RE7Service.Selection
        Dim oReturn As RE7Service.Selection

        Select Case searchType
            Case RecordSearchType.Appeal
                Dim selection As RE7Service.AppealSelection = New RE7Service.AppealSelection
                selection.Description = True
                selection.System_ID = True
                oReturn = selection

            Case RecordSearchType.Campaign
                Dim selection As RE7Service.CampaignSelection = New RE7Service.CampaignSelection
                selection.Description = True
                selection.System_ID = True
                oReturn = selection

            Case RecordSearchType.Fund
                Dim selection As RE7Service.FundSelection = New RE7Service.FundSelection
                selection.Description = True
                selection.System_ID = True
                oReturn = selection
        End Select

        Return oReturn
    End Function

    Private Function GetCFANames(ByVal oList As Hashtable, ByVal searchType As RecordSearchType) As Generic.List(Of BBCFAType)
        Dim Identifiers() As String
        Dim oCFAs As New List(Of BBCFAType)
        Dim iNumToFetch As Integer
        Dim cfaDF As RE7Service.GetCFADataArguments

        For Each sKey As String In oList.Keys
            ReDim Preserve Identifiers(iNumToFetch)
            Identifiers(iNumToFetch) = sKey
            iNumToFetch += 1
        Next

        If iNumToFetch > 0 Then
            cfaDF = New RE7Service.GetCFADataArguments
            cfaDF.IdType = RE7Service.CFAIdType.SystemRecordId
            cfaDF.Selection = CFASelection(searchType)
            cfaDF.Context = RE7Service.eCFAContext.Filter
            cfaDF.Identifiers = Identifiers

            Select Case searchType
                Case RecordSearchType.Appeal
                    Dim a() As RE7Service.AppealData = Core.Data.AppealSearch.FetchData(cfaDF)
                    If a IsNot Nothing Then
                        For Each oData As RE7Service.AppealData In a
                            '5/30/2007 sterling 276148-053007
                            'added check for deleted record
                            If oData IsNot Nothing Then
                                oCFAs.Add(New BBCFAType(CInt(oData.System_ID), oData.Description, searchType))
                            Else
                                oCFAs.Add(New BBCFAType(-1, "Appeal has been deleted", searchType))
                            End If
                        Next
                    End If
                Case RecordSearchType.Campaign
                    Dim a() As RE7Service.CampaignData = Core.Data.CampaignSearch.FetchData(cfaDF)
                    If a IsNot Nothing Then
                        For Each oData As RE7Service.CampaignData In a
                            '5/30/2007 sterling 276148-053007
                            'added check for deleted record
                            If oData IsNot Nothing Then
                                oCFAs.Add(New BBCFAType(CInt(oData.System_ID), oData.Description, searchType))
                            Else
                                oCFAs.Add(New BBCFAType(-1, String.Concat(Core.Data.RE7ServiceHelper.BOSystemText(Core.BackOfficeSystemEnums.eBOTextIndex.eCampaignText, False, "Campaign"), " has been deleted"), searchType))
                            End If
                        Next
                    End If
                Case RecordSearchType.Fund
                    If PortalSettings.Current.Features.IsInfinity Then
                        'infinity designations being revamped
                        'Dim designationsFilter As New DataLists.TopLevel.DesignationsByMapIDsDataListFilterData
                        'designationsFilter.MAPIDSLIST = String.Join(",", Identifiers)
                        'Dim designations() As DataLists.TopLevel.DesignationsByMapIDsDataListRow = DataLists.TopLevel.DesignationsByMapIDsDataList.GetRows(provider, designationsFilter)

                        'ViM transaction mgr saves guids for designation ids in it's content object. so retooling to use guids instead of map ids
                        Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
                        Dim designationsFilter As New DataLists.TopLevel.DesignationsByGuidsDataListFilterData
                        designationsFilter.GUIDSLIST = String.Join(",", Identifiers)
                        Dim designations() As DataLists.TopLevel.DesignationsByGuidsDataListRow = DataLists.TopLevel.DesignationsByGuidsDataList.GetRows(provider, designationsFilter)

                        For Each desObj As DataLists.TopLevel.DesignationsByGuidsDataListRow In designations
                            oCFAs.Add(New BBCFAType(desObj.MapID, HttpUtility.HtmlEncode(desObj.VANITYNAME), RecordSearchType.Fund, desObj.ID.ToString))
                        Next
                        'if a fund has been deleted then it wont be returned by the above call and will therefore be removed
                        'from the list of filters automatically, ie no server error like 276148-053007
                    Else
                        Dim a() As RE7Service.FundData = Core.Data.FundSearch.FetchData(cfaDF)
                        If a IsNot Nothing Then
                            For Each oData As RE7Service.FundData In a
                                '5/30/2007 sterling 276148-053007
                                'added check for deleted record
                                If oData IsNot Nothing Then
                                    oCFAs.Add(New BBCFAType(CInt(oData.System_ID), oData.Description, searchType))
                                Else
                                    oCFAs.Add(New BBCFAType(-1, String.Concat(Core.Data.RE7ServiceHelper.BOSystemText(Core.BackOfficeSystemEnums.eBOTextIndex.eFilterFund, False, "Fund"), " has been deleted"), searchType))
                                End If
                            Next
                        End If
                    End If

            End Select
        End If

        Return oCFAs

    End Function


    Private Sub LoadLabel(ByVal oLabel As Label, ByVal oList As Hashtable, ByVal searchType As RecordSearchType, ByVal cfaText As String, ByVal hiddenCtl As HtmlControls.HtmlInputHidden, ByVal isRecGiftFilter As Boolean)
        Dim oCFA = GetCFANames(oList, searchType)
        Dim ghf As Blackbaud.Web.Content.Core.Data.GivingHistoryFilter = New Blackbaud.Web.Content.Core.Data.GivingHistoryFilter

        If oCFA.Count > 0 Then

            ghf.All = False
            ghf.BBCFATypes = oCFA

            oLabel.Text = String.Join(",", oCFA.Select(Function(cfa) cfa.Name).ToArray())

            '5/30/2007 sterling 276148-053007
            'change text color to red if one of the selected items has been deleted from backoffice system
            If oLabel.Text.Contains("has been deleted") Then
                oLabel.ForeColor = Color.Red
            End If
        Else
            oLabel.Text = String.Concat("All ", cfaText)
            ghf.All = True
        End If

        'If BBSession.Item(SessionKey(searchType)) IsNot Nothing Then
        '	BBSession.Add(SessionKey(searchType), ghf)
        'Else
        '	BBSession.Item(SessionKey(searchType)) = ghf
        'End If

        If PortalSettings.Current.Features.IsInfinity AndAlso searchType = RecordSearchType.Fund Then
            If BBSession.Item(SessionKey(searchType, isRecGiftFilter) & "List") IsNot Nothing Then
                BBSession.Add(SessionKey(searchType, isRecGiftFilter) & "List", ghf.BBCFATypes)
            Else
                BBSession.Item(SessionKey(searchType, isRecGiftFilter) & "List") = ghf.BBCFATypes
            End If

            If BBSession.Item(SessionKey(searchType, isRecGiftFilter)) IsNot Nothing Then
                BBSession.Add(SessionKey(searchType, isRecGiftFilter), ghf)
            Else
                BBSession.Item(SessionKey(searchType, isRecGiftFilter)) = ghf
            End If

            'sessionKey::multiOption::allOption
            hiddenCtl.Value = FormHelper.DoScriptVariableSafeInject(SessionKey(searchType, isRecGiftFilter) & "List::1::1")
        Else
            If BBSession.Item(SessionKey(searchType, isRecGiftFilter)) IsNot Nothing Then
                BBSession.Add(SessionKey(searchType, isRecGiftFilter), ghf)
            Else
                BBSession.Item(SessionKey(searchType, isRecGiftFilter)) = ghf
            End If

            hiddenCtl.Value = FormHelper.DoScriptVariableSafeInject(SessionKey(searchType, isRecGiftFilter))
        End If



    End Sub


    Private Sub LoadLabelWithGiftTypes(ByVal oLabel As Label, ByVal oList As Hashtable)
        Dim oSBID As New System.Text.StringBuilder
        'Dim sType As String = Nothing

        If oList.Count > 0 Then
            Dim oSB As New System.Text.StringBuilder
            oSBID.Append("0^")

            For Each e As RE7Service.GiftType In Core.Data.RE7ServiceHelper.BackOfficeAppInfo.GivingHistoryTypes
                If oList.ContainsKey(CStr(CInt(e.eType))) Then
                    If oSB.Length > 0 Then
                        oSB.Append(", ")
                    End If
                    oSB.Append(StringLocalizer.BBString(e.Text))
                    If oSBID.Length > 2 Then
                        oSBID.Append(OBJECT_DELIMITER)
                    End If
                    oSBID.Append(String.Concat(CStr(CInt(e.eType)))) ', AttributeDelimiter, sType))
                End If
            Next
            oLabel.Text = oSB.ToString
        Else
            oLabel.Text = "All Types"
            oSBID.Append("-1^")
        End If

        Me.HiddenSelectedGiftTypeIDs.Value = FormHelper.DoScriptVariableSafeInject(oSBID.ToString)
    End Sub

    Private Sub GetSelectedValues(ByVal oHash As Hashtable, ByVal sSessionKey As String)

        Dim GHF As Blackbaud.Web.Content.Core.Data.GivingHistoryFilter
        Dim cfaType As BBCFAType

        If BBSession.Item(sSessionKey) IsNot Nothing Then
            GHF = DirectCast(BBSession.Item(sSessionKey), Blackbaud.Web.Content.Core.Data.GivingHistoryFilter)
        Else
            GHF = New Blackbaud.Web.Content.Core.Data.GivingHistoryFilter
        End If

        With oHash
            .Clear()
            If Not GHF.All Then
                For i As Integer = 0 To GHF.BBCFATypes.Count - 1
                    cfaType = DirectCast(GHF.BBCFATypes.Item(i), BBCFAType)
                    .Add(cfaType.ID.ToString, cfaType.ID.ToString)
                Next
            End If
        End With
    End Sub
    'comment
    Private Sub GetSelectedDesignationValues(ByVal oHash As Hashtable, ByVal sSessionKey As String)

        Dim GHF As Blackbaud.Web.Content.Core.Data.GivingHistoryFilter
        Dim cfaType As BBCFAType

        If BBSession.Item(sSessionKey) IsNot Nothing Then
            GHF = DirectCast(BBSession.Item(sSessionKey), Blackbaud.Web.Content.Core.Data.GivingHistoryFilter)
        Else
            GHF = New Blackbaud.Web.Content.Core.Data.GivingHistoryFilter
        End If

        With oHash
            .Clear()
            If Not GHF.All Then
                For i As Integer = 0 To GHF.BBCFATypes.Count - 1
                    cfaType = DirectCast(GHF.BBCFATypes.Item(i), BBCFAType)
                    'ViM store the Guids as Values and the MapIds as Keys because on
                    .Add(cfaType.ID.ToString, cfaType.GuidID.ToString)
                Next
            End If
        End With
    End Sub

    Private Sub SyncInfinityDesignationListWithGHFilterSessionObject(ByVal isRecGiftFilter As Boolean)
        Dim list As New Generic.List(Of BBCFAType)
        If BBSession.Item(SessionKey(RecordSearchType.Fund, isRecGiftFilter) & "List") IsNot Nothing Then
            list.AddRange(DirectCast(BBSession.Item(SessionKey(RecordSearchType.Fund, isRecGiftFilter) & "List"), Generic.List(Of BBCFAType)))
        End If

        Dim GHF As Blackbaud.Web.Content.Core.Data.GivingHistoryFilter
        If BBSession.Item(SessionKey(RecordSearchType.Fund, isRecGiftFilter)) IsNot Nothing Then
            GHF = DirectCast(BBSession.Item(SessionKey(RecordSearchType.Fund, isRecGiftFilter)), Blackbaud.Web.Content.Core.Data.GivingHistoryFilter)
        Else
            GHF = New Blackbaud.Web.Content.Core.Data.GivingHistoryFilter
        End If

        If list.Count = 0 Then GHF.All = True Else GHF.All = False
        GHF.BBCFATypes = list

        If BBSession.Item(SessionKey(RecordSearchType.Fund, isRecGiftFilter)) IsNot Nothing Then
            BBSession.Item(SessionKey(RecordSearchType.Fund, isRecGiftFilter)) = GHF
        Else
            BBSession.Add(SessionKey(RecordSearchType.Fund, isRecGiftFilter), GHF)
        End If
    End Sub

    Private Sub GetSelectedGiftTypes(ByVal oHash As Hashtable, ByVal value As String)
        Dim aObjects() As String
        Dim aPairs() As String
        Dim aValues() As String

        With oHash
            .Clear()
            If Len(value) > 0 Then
                aObjects = value.Split("^"c)
                If Not CBool(aObjects(0)) Then
                    'Not All selected
                    aPairs = aObjects(1).Split(OBJECT_DELIMITER)
                    For Each sItem As String In aPairs
                        If (Len(sItem) > 0) Then
                            aValues = sItem.Split(ATTRIBUTE_DELIMITER)
                            .Add(aValues(0), CInt(aValues(0)))
                        End If
                    Next sItem
                End If
            End If
        End With
    End Sub

    Private Sub GetSelectedColumns()
        Dim oCol As GivingHistoryColumn
        Dim GiftColumns As GivingHistoryColumns

        GiftColumns = FormHelper.ObjectFromJson(Of GivingHistoryColumns)(Me.HiddenUsedColumnData.Value)

        If Me.ColumnCustomRadioButton.Checked Then
            ' Order is important because we re-order the list in the editor's JavaScript
            ' but we don't change the DisplayOrder attribute...
            ' the answer to that, I do not know...
            For i = 0 To GiftColumns.Columns.Count - 1
                oCol = GiftColumns.Columns(i)
                oCol.DisplayOrder = i + 1
                m_oGivingHistory.UsedFields.Columns.Add(oCol)
            Next
            If m_oGivingHistory.UsedFields.Columns.Count = 0 Then
                Throw New ApplicationException("Please select columns for the custom column configuration.")
            End If
        End If

    End Sub

    Public Sub SaveToObjects()
        With m_oGivingHistory
            .SiteContentID = Me.ContentId
            .IncludeSoftCredits = Me.SoftCreditDisplayCheckBox.Checked
            If .IncludeSoftCredits Then
                .IncludeSoftCreditsTotal = chkIncludeSoftCreditTotal.Checked
                .IncludeHardCreditsTotal = chkIncludeHardCreditTotal.Checked
            End If
            '.SoftCreditText = Me.SoftCreditTextBox.Text
            .IncludePending = Me.PendingTransactionDisplayCheckBox.Checked
            .IncludeUnpaidEvents = cbUnpaidEvents.Checked
            '.PendingText = Me.PendingTextBox.Text
            .PageSize = CInt(ResultsPerPageDropDownList.SelectedValue)

            .UseCustomFilter = GiftsCustomRadioButton.Checked
            .UseCustomColumns = ColumnCustomRadioButton.Checked

            GetSelectedColumns()
            GetSelectedGiftTypes(.GiftTypes, Me.HiddenSelectedGiftTypeIDs.Value)
            GetSelectedValues(.Campaigns, Me.HiddenSelectedCampaignIDs.Value)
            If PortalSettings.Current.Features.IsInfinity Then
                SyncInfinityDesignationListWithGHFilterSessionObject(False)
                GetSelectedDesignationValues(.Funds, SessionKey(RecordSearchType.Fund, False))
            Else
                GetSelectedValues(.Funds, SessionKey(RecordSearchType.Fund, False)) 'Me.HiddenSelectedFundIDs.Value)
            End If

            GetSelectedValues(.Appeals, Me.HiddenSelectedAppealIDs.Value)

            If GiftsREQueryRadioButton.Checked Then
                .QueryID = GiftQueryLink.QueryId
                If .QueryID = 0 Then
                    Throw New ApplicationException(String.Format("Please select a {0} {1}.", backOfficeSystemName, queryTypeTextLbl))
                End If
            Else
                .QueryID = 0
            End If

            .IncludeSummary = Me.chkIncludeSummary.Checked
            .IncludeGiftTotal = Me.chkIncludeGiftTotal.Checked
            .IncludeGiftAidTotal = Me.chkIncludeGiftAidTotal.Checked
            If Not Me.HidePledge Then
                .IncludePledgeTotal = Me.chkIncludePledgeTotal.Checked
                .IncludePendingTotal = Me.chkIncludePendingTotal.Checked
                .IncludeBalanceTotal = Me.chkIncludeBalanceTotal.Checked
            Else
                .IncludePledgeTotal = False
                .IncludePendingTotal = False
                .IncludeBalanceTotal = False
            End If

            .IncludeTotalsCurrency = Me.chkIncludeTotalsCurrency.Checked

            If Me.PledgePaymentPageLink.PageID > 0 Then
                .PledgePaymentPageID = CInt(Me.PledgePaymentPageLink.PageID)
            Else
                .PledgePaymentPageID = 0
            End If

            'recurring gift updates
            .RecurringGiftEditAllowAmtUpdates = Me.chkAllowRecurringAmountUpdates.Checked
            .RecurringGiftEditAllowFreqUpdates = Me.chkAllowFrequencyUpdates.Checked
            .RecurringGiftEditAllowPmntTypeUpdates = Me.chkAllowPaymentTypeUpdates.Checked

            'save filtering options
            .RecurringGiftEditFilterUseCustomFilter = Me.rdoRecGftUpdFilterCustom.Checked
            If Me.rdoRecGftUpdFilterQuery.Checked Then
                .RecurringGiftEditFilterQueryID = RecGftUpdFilterQueryLnk.QueryId
                If .RecurringGiftEditFilterQueryID = 0 Then
                    Throw New ApplicationException(String.Format("Please select a {0} {1} for recurring gift updates.", backOfficeSystemName, queryTypeTextLbl))
                End If
            Else
                .RecurringGiftEditFilterQueryID = 0
            End If

            If PortalSettings.Current.Features.IsInfinity Then
                SyncInfinityDesignationListWithGHFilterSessionObject(True)
                GetSelectedDesignationValues(.RecurringGiftEditFilterFunds, SessionKey(RecordSearchType.Fund, True))
            Else
                GetSelectedValues(.RecurringGiftEditFilterFunds, SessionKey(RecordSearchType.Fund, True))
            End If

            GetSelectedValues(.RecurringGiftEditFilterAppeals, Me.HiddenSelectedRecGiftEditFilterAppealIDs.Value)

            'save amount options
            If Me.chkAllowRecurringAmountUpdates.Checked Then
                Dim minAmt As Decimal = 0

                If String.IsNullOrEmpty(txtAllowRecAmtUpdatesMinAmt.Text) Then
                    .RecurringGiftEditAmtUpdateMinAmt = minAmt
                Else
                    If Decimal.TryParse(txtAllowRecAmtUpdatesMinAmt.Text, minAmt) Then
                        If minAmt >= 0 Then
                            .RecurringGiftEditAmtUpdateMinAmt = minAmt
                        Else
                            Throw New ApplicationException("Please enter a valid minimum gift amount for recurring gift amount updates.")
                        End If
                    Else
                        Throw New ApplicationException("Please enter a valid minimum gift amount for recurring gift amount updates.")
                    End If
                End If
            End If

            'recurring gift frequency options
            .RecurringGiftEditFreqUseGeneralRecurrence = generalRecurrenceRadio.Checked
            .RecGftEdtFreqIncludeRecurSchdStartDate = chkAllowFrequencyUpdates.Checked AndAlso RecurringGiftScheduleStartDateCheckBox.Checked
            .RecGftEdtFreqIncludeRecurSchdEndDate = chkAllowFrequencyUpdates.Checked AndAlso RecurringGiftScheduleEndDateCheckBox.Checked

            Dim recSpecificsOldValues() As String = DirectCast(BBSession.Item(GridSessionKey(RECURRENCE_SPECIFIC_KEY)), String())

            Try
                'delete all old rows from table
                If recSpecificsOldValues IsNot Nothing AndAlso recSpecificsOldValues.Count > 0 Then
                    For i As Integer = 0 To recSpecificsOldValues.Count - 1 Step 1
                        RecordOperations.DeleteRecurrence.ExecuteOperation(Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current, recSpecificsOldValues(i))
                    Next
                End If
            Catch ex As Exception
            End Try

            Try
                If RecurrenceGridDataSource.Count > 0 Then
                    ReDim .RecGiftEditRecurrenceSpecifics(RecurrenceGridDataSource.Count - 1)
                    Dim index As Integer = 0
                    For Each recRecord As DataLists.TopLevel.RecurrenceRecordsDatalistRow In RecurrenceGridDataSource
                        Dim recurID As System.Guid = Guid.Empty
                        Dim recur As New AddForms.Recurrence.RecurrenceAddDataFormData()

                        With recur
                            .RECURRENCETYPE = CByte(recRecord.RECURRENCETYPE)
                            .STARTDATE = New Date(1753, 1, 1)
                            .ENDDATE = Nothing
                            .INTERVAL = CShort(recRecord.INTERVAL)
                            .DAY = CByte(recRecord.DAY)
                            .DAYOFWEEK = CByte(recRecord.DAYOFWEEK)
                            .WEEK = CByte(recRecord.WEEK)
                            .MONTH = CByte(recRecord.MONTH)
                            recurID = New Guid(.Save(Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current))
                        End With

                        .RecGiftEditRecurrenceSpecifics(index) = recurID.ToString
                        index += 1
                    Next
                Else
                    ReDim .RecGiftEditRecurrenceSpecifics(0)
                    Array.Resize(Of String)(.RecGiftEditRecurrenceSpecifics, 0)
                End If

            Catch ex As Exception
                Throw New ApplicationException("Please enter a valid specific recurrence setting.")
            End Try

            If .RecurringGiftEditAllowFreqUpdates AndAlso Not .RecurringGiftEditFreqUseGeneralRecurrence AndAlso .RecGiftEditRecurrenceSpecifics.Count = 0 Then
                Throw New ApplicationException("Please select at least one recurring gift frequency.")
            End If

            If ShowAdditionalDonation AndAlso Me.RecGiftAdditionalDonationPageLink.PageID > 0 Then
                .RecGiftAdditionalDonationPageID = CInt(Me.RecGiftAdditionalDonationPageLink.PageID)
            Else
                .RecGiftAdditionalDonationPageID = 0
            End If

            If Me.RecGiftPaymentPageLink.PageID > 0 Then
                .RecurringGiftPaymentPageID = CInt(Me.RecGiftPaymentPageLink.PageID)
            Else
                .RecurringGiftPaymentPageID = 0
            End If

            If Me.UnpaidEventPageLink.PageID > 0 Then
                .UnpaidEventsPaymentPageID = CInt(Me.UnpaidEventPageLink.PageID)
            Else
                .UnpaidEventsPaymentPageID = 0
            End If

            ClearSession()
        End With
    End Sub

    Private Sub ClearSession()
        'clear session
        Dim sKey As String = GridSessionKey(RECURRENCE_GRID_KEY)
        If Not BBSession.Item(sKey) Is Nothing Then BBSession.Remove(sKey)
        sKey = GridSessionKey(RECURRENCE_SPECIFIC_KEY)
        If Not BBSession.Item(sKey) Is Nothing Then BBSession.Remove(sKey)
    End Sub

    Public Overrides Sub ServerValidate(ByVal ValidationInfo As Core.ContentControl.ServerValidationInfo)

        If Me.Page.IsValid Then
            Try
                Dim oldID As Integer = m_oGivingHistory.ID
                SaveToObjects()

                'Dim optionsSaver As New BBNCGivingHistoryOptionsManager(Me.ContentId)
                'optionsSaver.Save(m_oGivingHistory)
                m_oGivingHistory.Save()

                If oldID <= 0 Then
                    Me.ContentObject.CustomProperty(CP_GIVINGHISTORY) = CStr(m_oGivingHistory.ID)

                    If Not Me.IsChildEditor Then
                        DirectCast(Me.ContentObject, DataObject).Save()
                    End If
                End If

            Catch ex As ApplicationException
                ValidationInfo.AddInfo(Nothing, ex.Message)
            End Try

        End If
    End Sub

    Private Function ClientVariables() As String
        With New System.Text.StringBuilder
            .Append("var SELECTED_LISTS='';")
            .Append(String.Concat("var HIDDEN_AVAILABLE_COLUMN_DATA='", HiddenAvailableColumnData.ClientID, "';"))
            .Append(String.Concat("var HIDDEN_USED_COLUMN_DATA='", HiddenUsedColumnData.ClientID, "';"))
            .Append(String.Concat("var HIDDEN_USED_COLUMN_STRING='", HiddenUsedColumnString.ClientID, "';"))

            Dim usedFields As Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryColumns = m_oGivingHistory.UsedFields
            For Each field As Blackbaud.CustomFx.WrappedParts.WebParts.GivingHistoryColumn In usedFields.Columns
                field.DefaultName = GetLanguageFieldName(field.FieldLocalizationGuid)
            Next


            .Append(String.Concat("var USED_COLS=", FormHelper.JsonFromObject(usedFields), ";"))
            .Append(String.Concat("var SECTION_ELEMENTTOFOCUS='", divChooseColumnsSection.ClientID, "';"))
            .Append(String.Concat("var COLS_ELEMENTTOFOCUS='", ColumnCustomRadioButton.ClientID, "';"))
            .Append(String.Concat("var GIFT_TYPE_LABEL='", lblGiftTypes.ClientID, "';"))
            .Append(String.Concat("var HIDDEN_GIFTTYPEIDS_ID='", HiddenSelectedGiftTypeIDs.ClientID, "';"))
            .Append(String.Concat("var CAMPAIGN_LABEL='", lblCampaigns.ClientID, "';"))
            .Append(String.Concat("var HIDDEN_CAMPAIGNIDS_ID='", HiddenSelectedCampaignIDs.ClientID, "';"))
            .Append(String.Concat("var FUND_LABEL='", lblFunds.ClientID, "';"))
            .Append(String.Concat("var HIDDEN_FUNDIDS_ID='", HiddenSelectedFundIDs.ClientID, "';"))
            .Append(String.Concat("var APPEAL_LABEL='", lblAppeals.ClientID, "';"))
            .Append(String.Concat("var HIDDEN_APPEALIDS_ID='", HiddenSelectedAppealIDs.ClientID, "';"))
            .Append(String.Concat("var REC_GIFT_FUND_LABEL='", lblRecGftUpdFundsChosen.ClientID, "';"))
            .Append(String.Concat("var REC_GIFT_HIDDEN_FUNDIDS_ID='", HiddenSelectedRecGiftEditFilterFundIDs.ClientID, "';"))
            .Append(String.Concat("var REC_GIFT_APPEAL_LABEL='", lblRecGftUpdAppealsChosen.ClientID, "';"))
            .Append(String.Concat("var REC_GIFT_HIDDEN_APPEALIDS_ID='", HiddenSelectedRecGiftEditFilterAppealIDs.ClientID, "';"))
            .Append(String.Concat("var SOFT_CREDIT_HELP_LINK='", AdminHelpVerb.FullURL(EContextHelpIDs.ContextHelpID_GivingHistory2_SoftCredits), "';"))
            If PortalSettings.Current.Features.IsInfinity Then
                .Append(String.Concat("var FUND_PICKER_CONTROL='~/admin/Common/DesignationSelect.ascx';"))
            ElseIf PortalSettings.Current.Features.IsRE7 Then
                .Append(String.Concat("var FUND_PICKER_CONTROL='~/admin/givinghistory/Funds.ascx';"))
            End If
            .Append("var SOFT_CREDIT_PANEL_ID='" + Me.SoftCreditsPanel.ClientID + "';")
            .Append("var UNPAID_EVENT_PANEL_ID='" + Me.UnpaidEventPanel.ClientID + "';")
            Return .ToString
        End With
    End Function

    Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
        MyBase.OnPreRender(e)

        With CType(Page, IBBWebPage).Head
            .AddScriptLink(SCRIPT_KEY, Page.ResolveUrl(SCRIPT_FILE))
            .AddScript("GivingHistoryScript", ClientVariables())
            .AddScriptLink("GivingHistory", Page.ResolveUrl("Client/Scripts/GivingHistoryEditor.js"))
            .AddScriptLink("GivingHistoryFilter", Page.ResolveUrl("Client/Scripts/GivingHistory.js"))
        End With

        Dim loadPanelScript As New System.Text.StringBuilder

        If m_oGivingHistory.UseCustomFilter Then
            loadPanelScript.Append("ChangePanel(2, 'GiftsCustom');")
        ElseIf m_oGivingHistory.QueryID > 0 OrElse Me.GiftsREQueryRadioButton.Checked Then
            loadPanelScript.Append("ChangePanel(2, 'GiftsREQuery');")
        End If

        If PortalSettings.Current.Features.IsInfinity AndAlso m_oGivingHistory.RecurringGiftEditFilterUseCustomFilter Then
            loadPanelScript.Append("ChangePanel(4,'RecGftUpdFilterCustom');")
        ElseIf PortalSettings.Current.Features.IsInfinity AndAlso m_oGivingHistory.RecurringGiftEditFilterQueryID > 0 OrElse Me.rdoRecGftUpdFilterQuery.Checked Then
            loadPanelScript.Append("ChangePanel(4,'RecGftUpdFilterQuery');")
        End If

        If PortalSettings.Current.Features.IsInfinity AndAlso m_oGivingHistory.RecurringGiftEditFreqUseGeneralRecurrence Then
            loadPanelScript.Append("ChangePanel(5,'generalRecurrence');")
        End If


        If m_oGivingHistory.UseCustomColumns Then
            loadPanelScript.Append("RenderColumns();ChangePanel(3, 'ColumnCustom');")
        End If

        If chkIncludePendingTotal.Visible Then
            PendingTransactionDisplayCheckBox.Attributes.Add("onclick", String.Format("DisableIncludePendingTotals(!this.checked, '{0}');", chkIncludePendingTotal.ClientID))

            If Not PendingTransactionDisplayCheckBox.Checked Then
                ScriptManager.RegisterStartupScript(Me.Page, Me.GetType, "InitializePendingTotalsCheckbox", String.Format("DisableIncludePendingTotals(true, '{0}');", chkIncludePendingTotal.ClientID), True)
            End If
        End If
        ScriptManager.RegisterStartupScript(Me, Me.GetType, "LoadPanels", loadPanelScript.ToString, True)


        'If Me.generalRecurrenceRadio.Checked Then
        '    Me.specificRecurrencePanel.Style.Add("display", "none")
        '    Me.generalRecurrencePanel.Style.Remove("display")
        'Else
        '    Me.generalRecurrencePanel.Style.Add("display", "none")
        '    Me.specificRecurrencePanel.Style.Remove("display")
        'End If
        SetupCustomFieldLanguage()
    End Sub

    Private Sub SetupCustomFieldLanguage()
        lblDate.Text = LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.FIELD_DATE, LanguageMetaData.Encoding.Raw)
        lblGiftType.Text = LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.FIELD_GIFT_TYPE, LanguageMetaData.Encoding.Raw)
        If PortalSettings.Current.Features.IsInfinity Then
            lblFundColumn.Text = LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.FIELD_DESIGNATION_PUBLICNAME, LanguageMetaData.Encoding.Raw)
        Else
            lblFundColumn.Text = LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.FIELD_FUND_DESCRIPTION, LanguageMetaData.Encoding.Raw)
        End If
        lblAmount.Text = LanguageData.GetLanguageString(GivingHistoryLanguage.LanguageGuids.FIELD_AMOUNT, LanguageMetaData.Encoding.Raw)
    End Sub

    Private Sub AddRecurrenceButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddRecurrenceButton.Click

        Dim rowToAdd As New DataLists.TopLevel.RecurrenceRecordsDatalistRow With _
         {.RECURRENCETYPE = CByte(RecurringGiftSchedule.RecurrenceType), _
         .STARTDATE = New Date(1753, 1, 1), _
         .ENDDATE = Nothing, _
         .INTERVAL = CShort(RecurringGiftSchedule.Interval), _
         .DAY = CByte(RecurringGiftSchedule.DayOfMonth), _
         .DAYOFWEEK = CByte(RecurringGiftSchedule.DayOfWeekMask), _
         .WEEK = CByte(RecurringGiftSchedule.WeekOfMonth), _
         .MONTH = CByte(RecurringGiftSchedule.MonthOfYear), _
         .ID = Guid.NewGuid()}

        If RecurringGiftSchedule.IsValid Then
            If Not RecurrenceDataGrid_ContainsItem(rowToAdd) Then
                lblRecGiftFrequencyGridError.Text = ""

                RecurrenceGridDataSource.Add(rowToAdd)

                Me.RecurrenceDataGrid.DataSource = RecurrenceGridDataSource
                Me.RecurrenceDataGrid.DataBind()
            Else
                lblRecGiftFrequencyGridError.Text = "Duplicate frequency"
            End If
        Else
            lblRecGiftFrequencyGridError.Text = "Invalid frequency"
        End If

        BringControlInView(Me.chkAllowFrequencyUpdates)

    End Sub

    Private Sub RecurrenceDataGrid_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles RecurrenceDataGrid.ItemDataBound
        Dim data As DataLists.TopLevel.RecurrenceRecordsDatalistRow = DirectCast(DirectCast(e.Item, DataGridItem).DataItem, DataLists.TopLevel.RecurrenceRecordsDatalistRow)
        Dim frequencyLabel As Label = _
        DirectCast(e.Item.FindControl("lblFrequency"), Label)

        If frequencyLabel IsNot Nothing Then frequencyLabel.Text = Core.GivingHistoryCommon.BuildFrequencySchedule(CType(data.RECURRENCETYPE, Core.RecurrenceHelper.ERecurrenceType), CType(data.DAYOFWEEK, Core.RecurrenceHelper.EDaysOfWeek), CInt(data.DAY), CType(data.MONTH, Core.RecurrenceHelper.EMonthOfYear), data.INTERVAL)
    End Sub

    Private Sub RecurrenceDataGrid_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles RecurrenceDataGrid.ItemCommand

        If e.CommandArgument.Equals("Remove") Then

            If e.Item.ItemIndex >= 0 Then
                RecurrenceGridDataSource.RemoveAt(e.Item.ItemIndex)

                Me.RecurrenceDataGrid.DataSource = RecurrenceGridDataSource
                Me.RecurrenceDataGrid.DataBind()
            End If

        End If

        BringControlInView(Me.chkAllowFrequencyUpdates)

    End Sub

    Private Function RecurrenceDataGrid_ContainsItem(ByVal itemToAdd As DataLists.TopLevel.RecurrenceRecordsDatalistRow) As Boolean

        For Each item As DataLists.TopLevel.RecurrenceRecordsDatalistRow In Me.RecurrenceGridDataSource
            If item.DAY = itemToAdd.DAY _
            AndAlso item.DAYOFWEEK = itemToAdd.DAYOFWEEK _
            AndAlso item.INTERVAL = itemToAdd.INTERVAL _
            AndAlso item.MONTH = itemToAdd.MONTH _
            AndAlso item.RECURRENCETYPE = itemToAdd.RECURRENCETYPE _
            AndAlso item.WEEK = itemToAdd.WEEK Then
                Return True
            End If
        Next

        Return False

    End Function
End Class
