Imports Blackbaud.Web.Content.Core.Internationalization.Currency
Imports Blackbaud.Web.Content.Core

Public Module GivingHistoryCommon

    Public Enum ConstituentPaymentAccountTypes
        DirectDebit = 0
        CreditCard = 1
    End Enum

    Public Enum ERecurrenceOptions
        General = 0
        Specific = 1
    End Enum

    'ViM With the Transaction Manager now supporting BBEC gifts, we want to be able to store * work with guids (designation ids in this case)
    'and move away from the mapids as we are also now in one single database. These 2 functions HashTableToXML(), HashTableFromXML have been modified 
    'to work with strings (guids as strings) and with integers (in the case of RE records ids)
    Public Function HashTableToXML(ByVal oTable As Hashtable, Optional ByVal bUTF8 As Boolean = False) As String

        Dim sXML As String
        Try
            Dim oSerializer As New System.Xml.Serialization.XmlSerializer(GetType(String()))
            Dim oStream As New IO.MemoryStream
            Dim oWriter As IO.StreamWriter
            Dim aValues() As String

            'Since a hashtable is not serializable we must first copy the values to an array
            ReDim aValues(oTable.Count - 1)
            oTable.Values.CopyTo(aValues, 0)
            If bUTF8 Then
                oWriter = New IO.StreamWriter(oStream, System.Text.UTF8Encoding.UTF8)
            Else
                oWriter = New IO.StreamWriter(oStream)
            End If

            'Serialize the array
            oSerializer.Serialize(oWriter, aValues)
            oStream.Position = 0

            Dim oReader As New IO.StreamReader(oStream)
            sXML = oReader.ReadToEnd()
        Catch ex As Exception
            Dim oSerializer As New System.Xml.Serialization.XmlSerializer(GetType(Integer()))
            Dim oStream As New IO.MemoryStream
            Dim oWriter As IO.StreamWriter
            Dim aValues() As Integer

            'Since a hashtable is not serializable we must first copy the values to an array
            ReDim aValues(oTable.Count - 1)
            oTable.Values.CopyTo(aValues, 0)
            If bUTF8 Then
                oWriter = New IO.StreamWriter(oStream, System.Text.UTF8Encoding.UTF8)
            Else
                oWriter = New IO.StreamWriter(oStream)
            End If

            'Serialize the array
            oSerializer.Serialize(oWriter, aValues)
            oStream.Position = 0

            Dim oReader As New IO.StreamReader(oStream)
            sXML = oReader.ReadToEnd()
        End Try

        Return sXML
    End Function

    Public Function HashTableFromXML(ByVal XML As String, Optional ByVal bUTF8 As Boolean = False) As Hashtable
        Try
            Dim aValues() As String
            'Since a hashtable is not serializable we must deserialize to an array
            Dim oSerializer As New System.Xml.Serialization.XmlSerializer(GetType(String()))
            aValues = DirectCast(oSerializer.Deserialize(New IO.StringReader(XML)), String())

            'Copy the array contents to our hashtable
            Dim oTable As New Hashtable
            For Each iValue As String In aValues
                oTable.Add(CStr(iValue), iValue)
            Next iValue

            Return oTable
        Catch
            Dim aValues() As Integer
            'Since a hashtable is not serializable we must deserialize to an array
            Dim oSerializer As New System.Xml.Serialization.XmlSerializer(GetType(Integer()))
            aValues = DirectCast(oSerializer.Deserialize(New IO.StringReader(XML)), Integer())

            'Copy the array contents to our hashtable
            Dim oTable As New Hashtable
            For Each iValue As Integer In aValues
                oTable.Add(CStr(iValue), iValue)
            Next iValue

            Return oTable
        End Try
    End Function

    Public Function BuildFrequencySchedule(ByVal RecurrenceType As RecurrenceHelper.ERecurrenceType, ByVal DayOfWeek As RecurrenceHelper.EDaysOfWeek, ByVal DayOfMonth As Integer, ByVal MonthOfYear As RecurrenceHelper.EMonthOfYear, ByVal Interval As Integer) As String
        Dim _schedule As New System.Text.StringBuilder

        Select Case RecurrenceType
            Case RecurrenceHelper.ERecurrenceType.MonthlyFixedDay
                _schedule.Append("Day " + DayOfMonth.ToString + " of every " + Interval.ToString + " months")
            Case RecurrenceHelper.ERecurrenceType.Weekly
                _schedule.Append("Recur every " + Interval.ToString + " week(s) on " + DayOfWeek.ToString)
            Case RecurrenceHelper.ERecurrenceType.YearlyFixedDate
                _schedule.Append("Every " + MonthOfYear.ToString + " " + DayOfMonth.ToString)
        End Select

        Return _schedule.ToString
    End Function

    Public Function GetAmountInPaymentCurrency(ByVal amount As Decimal, ByVal transactionISO As String, ByVal paymentCurrencyISO As String) As Decimal
        Dim retval As Decimal
        Dim provider = Blackbaud.Web.Content.Core.AppFx.ServiceProvider.Current
        Dim transactionCurrency As Blackbaud.Web.Content.Core.Internationalization.Currency = GetCurrency(transactionISO)
        Dim paymentCurrency As Blackbaud.Web.Content.Core.Internationalization.Currency = GetCurrency(paymentCurrencyISO)

        If transactionCurrency Is Nothing OrElse paymentCurrency Is Nothing Then
            retval = amount
        Else
            Dim exchangeRateFilterData As New DataLists.TopLevel.GetCurrencyExchangeRateDataListFilterData

            Dim paymentCurrencyID As String = GetCurrencyFromISO(paymentCurrency.ISO4217)
            If Not String.IsNullOrEmpty(paymentCurrencyID) Then
                exchangeRateFilterData.FROMCURRENCYID = New Guid(paymentCurrencyID)
            End If

            Dim transactionCurrencyID As String = GetCurrencyFromISO(transactionCurrency.ISO4217)
            If Not String.IsNullOrEmpty(transactionCurrencyID) Then
                exchangeRateFilterData.TOCURRENCYID = New Guid(transactionCurrencyID)
            End If

            Dim exchangeRate As DataLists.TopLevel.GetCurrencyExchangeRateDataListRow
            exchangeRate = DataLists.TopLevel.GetCurrencyExchangeRateDataList.GetRows(provider, exchangeRateFilterData).FirstOrDefault

            If exchangeRate.RATE > 0.0 Then
                retval = Blackbaud.AppFx.CurrencyMath.GetConvertUpperLimit(amount, transactionCurrency.Format.DecimalDigits, transactionCurrency.Format.RoundingType, paymentCurrency.Format.DecimalDigits, exchangeRate.RATE)
            End If
        End If

        Return retval
    End Function
End Module

Namespace GivingHistoryLanguage
    Public Class LanguageGuids
        Public Const ALL_FILTERS_APPLY_BUTTON As String = "258F6207-F30A-4f94-A205-2AD727B55FF5"
        Public Const DATE_FILTER_LABEL As String = "12e14ce5-b107-4044-afdf-059b2d3dacb6"
        Public Const DATE_FILTER_ALL As String = "036f9d40-5561-40bc-9e54-c3b2dc3b5668"
        Public Const DATE_FILTER_1_MONTH As String = "62ed1422-e5b6-490a-acbd-50032db48ea0"
        Public Const DATE_FILTER_6_MONTHS As String = "88612053-c174-4f35-a6b1-4df7898a4f36"
        Public Const DATE_FILTER_1_YEAR As String = "f705bdb1-a94d-45ee-b02d-72e84671fb37"
        Public Const DATE_FILTER_SPECIFIC As String = "76E283F3-96DD-462a-8232-337B0448763C"
        Public Const FUND_FILTER_LABEL As String = "cc9847e1-14ec-4011-a10e-a11b202a55c0"
        Public Const FUND_FILTER_ALL As String = "9a7823d6-eeb9-461f-a9ad-ead7efe444c9"
        Public Const GROUP_FILTER_LABEL As String = "2CBD78DF-B561-4EE8-83BC-FE3A820FF187"
        Public Const ACCESSIBILITY_GRID_SUMMARY As String = "747d08dc-9328-4694-8071-04fda6f5f8ca"
        ' The Field_{0} match GUIDs in the database where the columns are stored
        Public Const FIELD_DATE As String = "54b35cb5-950c-46f8-a727-468f191c2260"
        Public Const FIELD_AMOUNT As String = "e3bfea5a-3e39-435b-9d3e-3da661d98ed8"
        Public Const FIELD_GIFT_TYPE As String = "cdeb7148-620d-4a05-83c7-56d46ed07dbe"
        Public Const FIELD_FUND_DESCRIPTION As String = "1a568757-1f37-456a-a0fd-4ab2b2fdecd2"
        Public Const FIELD_BALANCE As String = "c83d92b6-0f4f-4a24-9fb0-53d63c1246a1"
        Public Const FIELD_CAMPAIGN_DESCRIPTION As String = "8D345F1C-9A0F-41e9-94E3-BB275B0DBC60"
        Public Const FIELD_CAMPAIGN_ID As String = "554418a3-4a31-4cdc-be83-ff2d37d24621"
        Public Const FIELD_FUND_ID As String = "f4e95ee6-0a0f-41e3-bc40-84ec7f57a06c"
        Public Const FIELD_GIFT_ID As String = "a81e70b9-29f9-4046-93cb-7ef43cc3881c"
        Public Const FIELD_INSTALLMENT_FREQUENCY As String = "1183a992-2463-4083-87db-f127ff0a4408"
        Public Const FIELD_INSTALLMENT_SCHEDULE As String = "0068658b-5439-4faa-8205-fecc1973e7b7"
        Public Const FIELD_NUMBER_OF_INSTALLMENTS As String = "1555c9a8-e264-4b67-9233-410c06d9532c"
        Public Const FIELD_PAY_METHOD As String = "cf19be99-abdd-4a1f-8857-38bbdf4ede39"
        Public Const FIELD_PENDING As String = "69f54217-2ee3-48b4-925f-644d5f05dc88"
        Public Const FIELD_RECEIPT_AMOUNT As String = "b0809524-b7bb-481b-a8ce-5091437965a8"
        Public Const FIELD_RECEIPT_DATE As String = "edb27131-932a-4e2d-8925-e1bcd182a76d"
        Public Const FIELD_RECEIPT_NUMBER As String = "648439e9-4acb-4d56-bc5c-1bad146672bb"
        Public Const FIELD_GIFT_AID_AMOUNT As String = "D14547E7-7F91-4cbd-961B-83920E3A3B15"
        Public Const FIELD_ANONYMOUS As String = "2132E0B9-F2B7-4ecd-B0B3-9CD1A5FDEF44"
        Public Const FIELD_GIFT_SUBTYPE As String = "05446546-1C8F-4435-8FCB-AF08EE078DDB"
        Public Const ITEM_FLAG_PENDING As String = "33308f45-77da-458a-bb18-8c116303673b"
        Public Const ITEM_FLAG_SOFT As String = "e3006dd3-3277-4649-9316-dbc4704257db"
        Public Const ITEM_FLAG_ANONYMOUS As String = "712BD207-FBA0-47D0-BB4A-98CA4555F595"
        Public Const ITEM_FLAG_ANONYMOUS_DONOR_NAME As String = "5099257E-FAD8-4EF0-B3AD-FE1C2610A6BB"
        Public Const ITEM_FLAG_EMPTY_GRID_NOTIFICATION As String = "03923D27-C6D9-4c9f-9C63-DB68B194D3B0"
        Public Const PLEDGE_DETAIL_BALANCE As String = "07acb321-f11c-4c4a-b855-6260d0b3f415"
        Public Const PLEDGE_DETAIL_TOTALPAID As String = "719C5107-AE70-4658-97B6-5FD0EF31C6E7"
        Public Const PLEDGE_DETAIL_AMOUNT As String = "b4917df8-c1c2-4b09-860c-ae012108258c"
        Public Const PLEDGE_DETAIL_DATE As String = "4e6ebb95-9ffd-4570-807d-16efb6b4d428"
        Public Const PLEDGE_NEXTINSTALLMENT As String = "1AAB7DC8-D09E-40F5-96AE-8DE3EEB3678F"
        Public Const PLEDGE_LASTINSTALLMENT As String = "633BF7AF-1850-46EA-9D4B-84811CA4A5CF"
        Public Const PLEDGE_PAYMENT_DETAIL_PLEDGE_DATE As String = "42f2dbae-19e2-47d9-ac4a-a3ee26c868e6"
        Public Const PLEDGE_PAYMENT_DETAIL_PLEDGE_AMOUNT As String = "ec3e4286-b70a-403c-ad99-b74df8a9d820"
        Public Const PLEDGE_PAYMENT_DETAIL_PLEDGE_BALANCE As String = "313cd9f9-45b3-4ca4-8106-26ad2e15687c"
        Public Const PLEDGE_PAYMENT_LINK As String = "B7B8E218-9456-4151-B37F-3DA0F39FED08"
        Public Const PAGER_PAGE_SENTENCE_PIECE As String = "f531d479-376f-4c18-bbb8-06428488e5fb"
        Public Const PAGER_OF_SENTENCE_PIECE As String = "06470634-f0a1-4f34-bece-66970ffa0728"
        Public Const PAGER_FIRST As String = "8843aab8-340d-4e94-9c77-62e677d3a0df"
        Public Const PAGER_PREV As String = "b8e35fed-10e8-4680-90bb-97843845d604"
        Public Const PAGER_NEXT As String = "ad2a3679-e7cb-46cb-a626-459ff4aab355"
        Public Const PAGER_LAST As String = "a65c6f70-cf0a-4efe-a735-1164f063a4ec"
        Public Const SUMMARY_CURRENCY As String = "B6F11A82-A0BC-464c-BCF3-8584E494E329"
        Public Const SUMMARY_GIFT_TOTAL As String = "3CC48677-5EA3-4746-BC4D-449768888C57"
        Public Const SUMMARY_GIFT_AID_TOTAL As String = "98A68E25-70C5-493a-A726-CD934C47C65E"
        Public Const SUMMARY_PLEDGE_TOTAL As String = "6D661C86-3FC1-4891-A3F4-092D8D9427E2"
        Public Const SUMMARY_PENDING_TOTAL As String = "A471378B-FF83-4583-B0A0-B068FAFB33E3"
        Public Const SUMMARY_BALANCE_TOTAL As String = "94BDBA75-AD4E-4a21-ACA0-F172EB6CC5F1"
        Public Const SUMMARY_SOFTCREDIT_TOTAL As String = "BCCA8A4B-71A1-4B7D-913A-FEB6C3ABA7A3"
        Public Const SUMMARY_HARDCREDIT_TOTAL As String = "E7A7C257-535E-4451-928F-904185A253CC"
        Public Const VALIDATION_DATE_INVALIDSTARTDATE As String = "16547D46-5450-40fa-9F79-EDD180B45119"
        Public Const VALIDATION_DATE_INVALIDENDDATE As String = "9305755D-C1A4-49a2-8CB0-58BCA431D5DB"
        Public Const VALIDATION_DATE_INVALIDENDBEFORESTART As String = "A793E71B-CBCC-498a-B5B7-9A8095B10D4C"
        Public Const HELPTEXT_FILTER As String = "A42E082A-612B-4ac4-8066-B92B012F254F"
        Public Const HELPTEXT_GRID As String = "75B8D76F-73A7-4c6a-B7BC-F4F826F6AD61"
        Public Const HELPTEXT_SUMMARY As String = "34509190-317E-4d90-A9E8-272DBCB06A71"
        Public Const HELPTEXT_PAYABLE_FILTER As String = "856C2C31-5697-41e8-B13A-A9DD9BEB6F88"
        Public Const FIELD_APPEAL_DESCRIPTION As String = "99E9449E-2C09-4D83-B090-ED325E548285"
        Public Const FIELD_DONORNAME_PUBLICNAME As String = "A8A362A1-92F3-480D-84AC-9F1B2674DFF4"
        'Export
        Public Const EXPORT_TITLE As String = "1C55DB1B-EFA9-45AD-A2FB-7F8D8345F4C4"
        Public Const EXPORT_PDF_TEXT As String = "3CE2AFF6-E255-4A12-967C-F0FD478FA345"
        Public Const EXPORT_CSV_TEXT As String = "6E8A9CDD-6F1E-449E-A9C8-EF5FA62D96C0"

        'BBEC specific columns
        Public Const FIELD_DESIGNATION_PUBLICNAME As String = "18C85DF2-4261-4905-B2CB-FE3672544B44"
        Public Const FIELD_DESIGNATION_LOOKUPID As String = "A07C36D5-0082-44db-A961-435F9BBFB58B"
        Public Const FIELD_GIFT_LOOKUPD As String = "C4D9A70E-2C78-42c8-989E-662053D07BD0"
        Public Const FIELD_CAMPAIGN_LOOKUPID As String = "530C9883-AB91-47a3-8743-082AFA4E238C"
        Public Const FIELD_SITE_HEIRARCHY As String = "DB1A19DE-2259-4005-919E-BA77595BE9F2"
        Public Const FIELD_EVENT_NAME As String = "D5A4D2EE-52DD-4f32-B88E-3683F7BBC451"
        Public Const FIELD_EVENT_DETAILS As String = "7098CD49-F37E-4a3c-810D-8476F8886FBC"
        Public Const FIELD_FUNDRAISING_PURPOSE As String = "B63CE00B-64BB-4A1D-9A50-0523073BF273"
        'recurring gift details
        Public Const RECURRING_GIFT_DETAIL_CC_NUMBER As String = "cccac681-fb16-4dbf-9092-a94289416028"
        Public Const RECURRING_GIFT_DETAIL_CC_EXPIRES As String = "558cb9ed-8590-426f-a76b-4df28612b585"
        Public Const RECURRING_GIFT_DETAIL_DD_ACCT_NUMBER As String = "1EC8B5B4-64D2-4ee8-A83F-7ACFFA686AA9"
        Public Const RECURRING_GIFT_DETAIL_DD_ACCT_TYPE As String = "c245a8fb-1a7e-4dee-940b-1f7485d0de6b"
        Public Const RECURRING_GIFT_DETAIL_DD_ACCT_DESC As String = "9242de89-a8e4-42ef-8b70-70c9f6a5e1e5"
        Public Const RECURRING_GIFT_DETAIL_LAST_PAYMENT_AMT As String = "1633a078-3d9a-4ec8-b58b-263a3263fbdc"
        Public Const RECURRING_GIFT_DETAIL_LAST_PAYMENT_DATE As String = "d0a025b1-21c7-430d-8188-cb5c0b7b2f66"
        Public Const RECURRING_GIFT_DETAIL_NO_PAYMENTS As String = "39026b33-71ab-49fa-88d7-55d9eae5b43f"
        Public Const PLEDGE_DETAIL_ISEFT As String = "ebf8363e-1463-4859-85fc-4eb5eb76fa1e"

        'recurring gift editing 
        'details
        Public Const DETAILS_EDIT_TITLE_LABEL As String = "ed50af9a-017a-4377-b530-581fc28fca92"
        Public Const DETAILS_EDIT_GIFT_AMOUNT_LABEL As String = "897df093-0f78-4573-928c-144942d1a4bb"
        Public Const DETAILS_EDIT_FREQUENCY_LABEL As String = "6aa1f9f2-834b-4417-9dbb-113dbe7943d3"
        Public Const DETAILS_EDIT_START_DATE_LABEL As String = "81fdd214-2269-40cb-8e15-13ea1f5de957"
        Public Const DETAILS_EDIT_END_DATE_LABEL As String = "a63232a7-2292-4f69-9e84-275d53b91e86"
        Public Const DETAILS_EDIT_SAVE_BUTTON_TEXT As String = "18883e12-8fc1-441d-a971-80fd59ed94f2"
        Public Const DETAILS_EDIT_CANCEL_BUTTON_TEXT As String = "fcbf3758-aceb-4e11-8513-7fcfb9e0c16f"
        Public Const DETAILS_EDIT_USE_SAME_FREQ_OPTION As String = "2b056eaf-9178-48f5-8ff2-d735ebfd4e30"
        Public Const DETAILS_EDIT_EDIT_BUTTON_TEXT As String = "33db5d64-cd01-49fe-9371-a98a535c5d9e"
        Public Const DETAILS_EDIT_SAVING_MESSAGE As String = "9993ca4d-96b4-4fc5-88b8-2da6246c7b3c"
        Public Const DETAILS_EDIT_VALIDATION_AMT_GREATER_MIN As String = "6c4e6385-c3fd-49b8-9b27-cf143add7680"
        Public Const DETAILS_EDIT_VALIDATION_AMT_NUMBER As String = "54a6a95e-464e-4db1-9e7f-b4f87fca77de"
        Public Const DETAILS_EDIT_VALIDATION_INVALID_SCHEDULE As String = "230b0ad5-0203-4baf-928c-60eefc2fc036"
        Public Const DETAILS_EDIT_VALIDATION_INVALID_CUSTOM_FREQUENCY As String = "47E56E99-84EF-4ab6-B63B-B0791A971013"
        Public Const HELPTEXT_DETAIL_EDITING As String = "add0d0e4-ec2f-4786-91e1-c1a38b523fec"
        Public Const DETAILS_EDIT_VALIDATION_INVALID_NEXT_TRANSACTION As String = "705c0ca4-4d4e-40ce-865f-6762b5643c79"
        Public Const HELPTEXT_TRUNCATE_RECORDS As String = "713D20B3-4A97-4E18-BD73-B07DCFEE2D18"

        'payment
        Public Const PAYMENT_EDIT_TITLE_LABEL As String = "fbe86639-e9ef-4570-8bee-fd506963ea34"
        Public Const PAYMENT_EDIT_PAY_METHOD_LABEL As String = "4918f29e-290e-4eda-bc3a-d60b8966adb2"
        Public Const PAYMENT_EDIT_DEBIT_ACCOUNT_LABEL As String = "a30871a6-cfdf-47e1-b76e-18877159031b"
        Public Const PAYMENT_EDIT_CREDIT_CARD_LABEL As String = "8cda36c3-98f1-40fb-bde2-a5f88719426e"
        Public Const PAYMENT_EDIT_CREDIT_CARD_OPTION As String = "cfddaad8-6d7b-4ce5-badb-88de656976d0"
        Public Const PAYMENT_EDIT_DIRECT_DEBIT_OPTION As String = "21ee5fdd-9316-444b-98a4-a410927d677d"
        Public Const PAYMENT_EDIT_NEW_CREDIT_CARD_OPTION As String = "c4dc1c18-a324-4c5c-957b-0b9644f853ba"
        Public Const PAYMENT_EDIT_NEW_DIRECT_DEBIT_OPTION As String = "a80cfb6e-936c-417b-8aaf-f8af0a2543fa"
        Public Const PAYMENT_EDIT_FINANCIAL_INST_LABEL As String = "eb827e9b-6229-4e5e-b7f8-f12efe03d0b0"
        Public Const PAYMENT_EDIT_BRANCH_NAME_LABEL As String = "ef3087af-ef0a-43da-87b2-4126b4f86eab"
        Public Const PAYMENT_EDIT_COUNTRY_LABEL As String = "ac159e1a-cac1-40f8-a3ff-1aafe0ae63e3"
        Public Const PAYMENT_EDIT_ROUTING_NUMBER_LABEL As String = "66056b86-5efa-425c-b6ee-556878bc6150"
        Public Const PAYMENT_EDIT_ACCOUNT_NUMBER_LABEL As String = "12c5ce8e-51dc-490e-b447-f400bdd9415d"
        Public Const PAYMENT_EDIT_ACCOUNT_TYPE_LABEL As String = "e98318f4-9d71-4ba3-8369-08381d23f034"
        Public Const PAYMENT_EDIT_ACCOUNT_HOLDER_LABEL As String = "8189619c-144f-49aa-b708-d8d960b265e4"
        Public Const PAYMENT_EDIT_UPDATE_CREDIT_CARD_LABEL As String = "914f37c6-6d14-4d5a-a62c-9dc70fe0ed7e"
        Public Const PAYMENT_EDIT_NEW_CREDIT_CARD_LABEL As String = "524ff20b-dcc4-4c1c-bb10-09f066c349b5"
        Public Const PAYMENT_EDIT_UK_ORIGINATOR_LABEL As String = "2d5aef01-27a8-4354-a9a2-24c9c2de8703"
        Public Const PAYMENT_EDIT_UK_DEBIT_DATE_LABEL As String = "0f2a7c8e-f2c3-4b04-a129-77460ca763fe"
        Public Const PAYMENT_EDIT_SAVE_BUTTON_TEXT As String = "10caeb06-05ed-49a6-914a-c285d1f65f51"
        Public Const PAYMENT_EDIT_CANCEL_BUTTON_TEXT As String = "979015c3-9cae-4fe9-a20c-f7b3d75415ab"
        Public Const PAYMENT_EDIT_EDIT_BUTTON_TEXT As String = "2f099753-ba20-47fb-a1b4-30834bcb64d5"
        Public Const PAYMENT_EDIT_SAVING_MESSAGE As String = "599c603b-cc01-4cdb-9230-609c14170d90"
        Public Const PAYMENT_EDIT_VALIDATION_INVALID_CARD_OPTION As String = "5c790831-83d0-4bda-8d80-7c6257f4e810"
        Public Const PAYMENT_EDIT_VALIDATION_INVALID_DEBIT_OPTION As String = "71d54ab6-9075-4a9d-a4c4-37e0b036f93c"
        Public Const PAYMENT_EDIT_VALIDATION_REQUIRED_ACCOUNT_TYPE As String = "e66200c1-8fbf-4009-a3ed-91f813de5162"
        Public Const PAYMENT_EDIT_VALIDATION_REQUIRED_FINANCIAL_INST As String = "c5546818-218e-459f-bb14-bde8fbbdfc69"
        Public Const PAYMENT_EDIT_VALIDATION_REQUIRED_ROUTING_NUM As String = "db535e74-e879-455c-bc24-d6f6428cce76"
        Public Const PAYMENT_EDIT_VALIDATION_INVALID_ROUTING_NUM As String = "2ddda1ee-e367-4e3c-b27b-91cf59375224"
        Public Const PAYMENT_EDIT_VALIDATION_REQUIRED_ACCOUNT_NUM As String = "25ce885a-3ffd-4538-b652-91aed7f4e9ad"
        Public Const PAYMENT_EDIT_VALIDATION_REQUIRED_ACCOUNT_HOLDER As String = "5ce93cc8-ca2d-4808-8fa0-608829ef6ff3"
        Public Const PAYMENT_EDIT_VALIDATION_REQUIRED_COUNTRY As String = "31ded8de-a40c-41aa-b75c-3f9f502813d9"
        Public Const PAYMENT_EDIT_UPDATE_CREDIT_CARD_LINK As String = "9e0f306e-69e1-4aa1-9b9a-3e49d8b098d9"
        Public Const PAYMENT_EDIT_NEW_CREDIT_CARD_LINK As String = "e947c7f1-5ad8-4a52-a041-3d79460f9ac3"
        Public Const PAYMENT_EDIT_VALIDATION_REQUIRED_SORTCODE As String = "6107C47C-E7A0-4faf-A48D-9AE13D0ABAE0"
        Public Const HELPTEXT_PAYMENT_EDITING As String = "35ac5ee0-a09d-481d-9396-7e9376b26b9a"
        Public Const PAYMENT_EDIT_VALIDATION_REQUIRED_PAYMETHOD As String = "DB588FAF-2C73-41fa-9914-C056501F52E3"
        Public Const PAYMENT_EDIT_VALIDATION_DUPLICATE_ACCOUNT As String = "11178848-4517-412e-8955-fdbd29c134ef"

        'subtotal
        Public Const SUBTOTAL_GRID_TEXT As String = "51BD11DD-8552-4C57-8877-9E6EC91841F1"

    End Class
End Namespace
