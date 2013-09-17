Option Infer On
Option Strict Off

Imports bbAppFxWebAPI = Blackbaud.AppFx.WebAPI



Namespace DataLists



        '<@ENUMS@>

            ''' <summary>
    ''' Provides WebApi access to the "Website Sponsorship Checkout Source Data List" catalog feature.  Used to populate the source drop down on the sponsorship checkout page
    ''' </summary>
<System.CodeDom.Compiler.GeneratedCodeAttribute("BBMetalWeb", "2011.8.2.0")> _
        Public NotInheritable Class [WebsiteSponsorshipCheckoutSourceMainDataList]

            Private Sub New()
                'this is a static class (only shared methods) that should never be instantiated.
            End Sub

            Private Shared ReadOnly _specId As Guid = New Guid("41541a9f-c07d-4f9a-bd96-e01fb2a48cb7")
            ''' <summary>
            ''' The DataList ID value for the "Website Sponsorship Checkout Source Data List" datalist
            ''' </summary>
            Public Shared ReadOnly Property SpecId() As Guid
                Get
                    Return _specId
                End Get
            End Property

            Private Shared ReadOnly _rowFactoryDelegate As Blackbaud.AppFx.WebAPI.DataListRowFactoryDelegate(Of WebsiteSponsorshipCheckoutSourceMainDataListRow) = AddressOf CreateListRow

            Private Shared Function CreateListRow(ByVal rowValues As String()) As [WebsiteSponsorshipCheckoutSourceMainDataListRow]
                Return New [WebsiteSponsorshipCheckoutSourceMainDataListRow](rowValues)
            End Function

            Public Shared Function CreateRequest(ByVal provider As bbAppFxWebAPI.AppFxWebServiceProvider) As bbAppFxWebAPI.ServiceProxy.DataListLoadRequest
                Return Blackbaud.AppFx.WebAPI.DataListServices.CreateDataListLoadRequest(provider, [WebsiteSponsorshipCheckoutSourceMainDataList].SpecId)
            End Function

            Public Shared Function GetRows(ByVal provider As bbAppFxWebAPI.AppFxWebServiceProvider, ByVal recordID As String) As WebsiteSponsorshipCheckoutSourceMainDataListRow()

                                If String.IsNullOrEmpty(recordID) Then Throw New ArgumentException("recordID is required for this datalist.", "recordID")

                Dim request = CreateRequest(provider)

                request.ContextRecordID = recordID



                Return GetRows(provider, request)

            End Function

            Public Shared Function GetRows(ByVal provider As bbAppFxWebAPI.AppFxWebServiceProvider, ByVal request As bbAppFxWebAPI.ServiceProxy.DataListLoadRequest) As WebsiteSponsorshipCheckoutSourceMainDataListRow()
                Return bbAppFxWebAPI.DataListServices.GetListRows(Of [WebsiteSponsorshipCheckoutSourceMainDataListRow])(provider, _rowFactoryDelegate, request)
            End Function

        End Class

#Region "Row Data Class"

        <System.CodeDom.Compiler.GeneratedCodeAttribute("BBMetalWeb", "2011.8.2.0")> _
        <System.Serializable()> _
        Public NotInheritable Class [WebsiteSponsorshipCheckoutSourceMainDataListRow]


Private [_SORTORDER] As Byte
Public Property [SORTORDER] As Byte
    Get
        Return Me.[_SORTORDER]
    End Get
    Set(value As Byte)
        Me.[_SORTORDER] = value
    End Set
End Property

Private [_APPEALID] As System.Guid
Public Property [APPEALID] As System.Guid
    Get
        Return Me.[_APPEALID]
    End Get
    Set(value As System.Guid)
        Me.[_APPEALID] = value
    End Set
End Property

Private [_SOLICITORID] As System.Guid
Public Property [SOLICITORID] As System.Guid
    Get
        Return Me.[_SOLICITORID]
    End Get
    Set(value As System.Guid)
        Me.[_SOLICITORID] = value
    End Set
End Property

Private [_SOURCEDESC] As String
Public Property [SOURCEDESC] As String
    Get
        Return Me.[_SOURCEDESC]
    End Get
    Set(value As String)
        Me.[_SOURCEDESC] = value
    End Set
End Property

Private [_HASADDITIONALINFORMATION] As Boolean
Public Property [HASADDITIONALINFORMATION] As Boolean
    Get
        Return Me.[_HASADDITIONALINFORMATION]
    End Get
    Set(value As Boolean)
        Me.[_HASADDITIONALINFORMATION] = value
    End Set
End Property

Private [_ADDITIONALINFORMATIONCAPTION] As String
Public Property [ADDITIONALINFORMATIONCAPTION] As String
    Get
        Return Me.[_ADDITIONALINFORMATIONCAPTION]
    End Get
    Set(value As String)
        Me.[_ADDITIONALINFORMATIONCAPTION] = value
    End Set
End Property

Private [_ADDITIONALINFORMATIONDATALISTID] As System.Guid
Public Property [ADDITIONALINFORMATIONDATALISTID] As System.Guid
    Get
        Return Me.[_ADDITIONALINFORMATIONDATALISTID]
    End Get
    Set(value As System.Guid)
        Me.[_ADDITIONALINFORMATIONDATALISTID] = value
    End Set
End Property



            Public Sub New()
                MyBase.New()
            End Sub

            Friend Sub New(ByVal dataListRowValues() As String)

                Blackbaud.AppFx.WebAPI.DataListServices.ValidateDataListOutputColumnCount(6, dataListRowValues, WebsiteSponsorshipCheckoutSourceMainDataList.SpecId)

Me.[_SORTORDER] = Blackbaud.AppFx.DataListUtility.DataListStringValueToByte(dataListRowValues(0))

Me.[_APPEALID] = Blackbaud.AppFx.DataListUtility.DataListStringValueToGuid(dataListRowValues(1))

Me.[_SOLICITORID] = Blackbaud.AppFx.DataListUtility.DataListStringValueToGuid(dataListRowValues(2))

Me.[_SOURCEDESC] = dataListRowValues(3)

Me.[_HASADDITIONALINFORMATION] = Blackbaud.AppFx.DataListUtility.DataListStringValueToBool(dataListRowValues(4))

Me.[_ADDITIONALINFORMATIONCAPTION] = dataListRowValues(5)

Me.[_ADDITIONALINFORMATIONDATALISTID] = Blackbaud.AppFx.DataListUtility.DataListStringValueToGuid(dataListRowValues(6))


            End Sub

        End Class

#End Region



    End Namespace

