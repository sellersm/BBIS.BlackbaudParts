Imports System.Web
Imports System.Web.Services
Imports System.Web.SessionState
Imports Blackbaud.Web.Content.Core
Imports WebSupergoo
Imports Blackbaud.Web.Content.Portal.Components
Imports Blackbaud.Web.Content.Portal
Imports Blackbaud.Web.Content
Imports WebSupergoo.ABCpdf9

Public NotInheritable Class ddpdfHandler
    Implements IHttpHandler
    Implements IRequiresSessionState

    Public Const PDFCACHEKEY As String = "DDPDF_KEY"

    Friend Const AbcPdfLicenseKey As String = "WuJbSzVR9+1XpyYSM/zSObfMSeMZVIAhqc27lbe0Qh5YqX6WOi69N5FqZD0ymwcPbtYw9TpFriAX1acnTrd9xjkZWATfWHBOrCgxVcOyR0LmwU3y8sASXYXiZV2yYWFgGO5JXMpc4RMiTBdqvC+aIgQeMguWUm64urYb8Fdqe/vA2rMpt1mHyH/H+ZFJiDVTYxcP+AQ38nidSQE6G6BIum12ilPOYMYAft+eCkjdWreUeQ=="

    Private Const NAMETOKEN As String = "OrgName"
    Private Const ADDRESSTOKEN As String = "OrgAddress"
    Private Const GUARANTEETOKEN As String = "Guarantee"
    Private Const INSTRUCTOKEN As String = "Instructions"
    Private Const SERVICENUMBERTOKEN As String = "SN"

    Private ServiceUserID As String
    Private OrgName As String
    Private Address1 As String
    Private Address2 As String
    Private CityPostCode As String
    Private Instruction As String

    Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
        Get
            Return True
        End Get
    End Property

    Public Function ReadIntoBuffer(ByVal oStream As IO.Stream, ByVal iNumChars As Integer, ByVal buffer As Byte(), ByRef iBufferOffset As Integer, ByVal Data As Byte(), ByVal iTokenLen As Integer) As Integer

        oStream.Read(buffer, iBufferOffset, iNumChars)
        For i As Integer = 0 To Data.Length - 1
            buffer(iBufferOffset + iNumChars + i) = Data(i)
        Next
        oStream.Position += iTokenLen
        iBufferOffset += (iNumChars + Data.Length)

    End Function

    Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
        If context.Request.QueryString.Item("DDReceipt") IsNot Nothing AndAlso context.Request.QueryString.Item("DDReceipt").ToLower = "true" Then
            BuildReceipt(context)
        ElseIf context.Request.QueryString.Item("id") IsNot Nothing Then
            SendGenericPdf(context)
        Else
            BuildBill(context)
        End If
        Return
    End Sub

    Private Sub BuildReceipt(ByRef context As System.Web.HttpContext)
        Dim qString As String = context.Request.QueryString("q")
        qString = Web.Content.Common.Encryption.Encryptor.Decrypt(qString)
        Dim qs() As String = qString.Split(New Char() {"_"c})

        Dim transactionType As DonationCaptureDisplay.eTransactionType = CType(CInt(qs(1)), DonationCaptureDisplay.eTransactionType)
        Dim transactionID As Integer = CInt(qs(0))
        Dim oGift As Core.Data.GiftInformation
        Select Case transactionType
            Case DonationCaptureDisplay.eTransactionType.donation
                Dim oTran As Core.Data.ShelbyDonationTran = Core.Data.Transactions.LoadDonation(transactionID)
                oGift = oTran.Gift
            Case DonationCaptureDisplay.eTransactionType.eventregistration
                Dim oTran As Core.Data.ShelbyEventRegTran = Core.Data.Transactions.LoadEventRegistration(transactionID)
                oGift = oTran.Gift
            Case DonationCaptureDisplay.eTransactionType.membership
                Dim oTran As Core.Data.ShelbyMembershipTran = Core.Data.Transactions.LoadMembershipRegistration(transactionID)
                oGift = oTran.Gift
        End Select

        Dim oAssembly As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly
        ABCpdf8.XSettings.InstallRedistributionLicense(AbcPdfLicenseKey)
        Dim pdfdoc As New ABCpdf8.Doc
        Dim pdfStream As IO.Stream
        Dim pdfmStream As New IO.MemoryStream

        pdfStream = oAssembly.GetManifestResourceStream(oAssembly.GetName.Name & ".DDReceipt3.pdf")

        pdfdoc.Read(pdfStream)
        pdfStream.Dispose()

        Dim instruction As String = "Please pay %1 Direct Debits from the account detailed in this Instruction subject to the safeguards assured by the Direct Debit Guarantee. I understand that this Instruction may remain with %1 and, if so, details will be passed electronically to my Bank/Building Society."
        instruction = instruction.Replace("%1", PortalSettings.Current.Client.Name)

        pdfdoc.Form.Fields(0).Kids(0).Kids("instruction[0]").Value = instruction
        pdfdoc.Form.Fields(0).Kids(0).Kids("orgNameAddress[0]").Value = PortalSettings.Current.Client.Name & vbCrLf & _
            PortalSettings.Current.Client.Address1 & vbCrLf & _
            PortalSettings.Current.Client.Address2 & vbCrLf & _
            PortalSettings.Current.Client.City & " " & PortalSettings.Current.Client.PostCode
        pdfdoc.Form.Fields(0).Kids(0).Kids("day[0]").Value = Now.Day.ToString
        pdfdoc.Form.Fields(0).Kids(0).Kids("month[0]").Value = Now.Month.ToString
        pdfdoc.Form.Fields(0).Kids(0).Kids("year[0]").Value = Now.Year.ToString.Substring(2, 2)


        With oGift.DirectDebit

            pdfdoc.Form.Fields(0).Kids(0).Kids("bankName[0]").Value = oGift.DirectDebit.FinancialInstitution
            pdfdoc.Form.Fields(0).Kids(0).Kids("accountHolder[0]").Value = oGift.DirectDebit.AccountHolderName

            'sort code
            If .RoutingNumber.Length > 0 Then pdfdoc.Form.Fields(0).Kids(0).Kids("s1[0]").Value = .RoutingNumber(0)
            If .RoutingNumber.Length > 1 Then pdfdoc.Form.Fields(0).Kids(0).Kids("s2[0]").Value = .RoutingNumber(1)
            If .RoutingNumber.Length > 3 Then pdfdoc.Form.Fields(0).Kids(0).Kids("s3[0]").Value = .RoutingNumber(3)
            If .RoutingNumber.Length > 4 Then pdfdoc.Form.Fields(0).Kids(0).Kids("s4[0]").Value = .RoutingNumber(4)
            If .RoutingNumber.Length > 6 Then pdfdoc.Form.Fields(0).Kids(0).Kids("s5[0]").Value = .RoutingNumber(6)
            If .RoutingNumber.Length > 7 Then pdfdoc.Form.Fields(0).Kids(0).Kids("s6[0]").Value = .RoutingNumber(7)

            'account number
            If .AccountNumber.Length > 0 Then pdfdoc.Form.Fields(0).Kids(0).Kids("a1[0]").Value = .AccountNumber(0)
            If .AccountNumber.Length > 1 Then pdfdoc.Form.Fields(0).Kids(0).Kids("a2[0]").Value = .AccountNumber(1)
            If .AccountNumber.Length > 2 Then pdfdoc.Form.Fields(0).Kids(0).Kids("a3[0]").Value = .AccountNumber(2)
            If .AccountNumber.Length > 3 Then pdfdoc.Form.Fields(0).Kids(0).Kids("a4[0]").Value = .AccountNumber(3)
            If .AccountNumber.Length > 4 Then pdfdoc.Form.Fields(0).Kids(0).Kids("a5[0]").Value = .AccountNumber(4)
            If .AccountNumber.Length > 5 Then pdfdoc.Form.Fields(0).Kids(0).Kids("a6[0]").Value = .AccountNumber(5)
            If .AccountNumber.Length > 6 Then pdfdoc.Form.Fields(0).Kids(0).Kids("a7[0]").Value = .AccountNumber(6)
            If .AccountNumber.Length > 7 Then pdfdoc.Form.Fields(0).Kids(0).Kids("a8[0]").Value = .AccountNumber(7)
        End With

        'originator id
        With PortalSettings.Current.Client.OrigID
            If .Length > 0 Then pdfdoc.Form.Fields(0).Kids(0).Kids("o1[0]").Value = .ToCharArray(0, 1)
            If .Length > 1 Then pdfdoc.Form.Fields(0).Kids(0).Kids("o2[0]").Value = .ToCharArray(1, 1)
            If .Length > 2 Then pdfdoc.Form.Fields(0).Kids(0).Kids("o3[0]").Value = .ToCharArray(2, 1)
            If .Length > 3 Then pdfdoc.Form.Fields(0).Kids(0).Kids("o4[0]").Value = .ToCharArray(3, 1)
            If .Length > 4 Then pdfdoc.Form.Fields(0).Kids(0).Kids("o5[0]").Value = .ToCharArray(4, 1)
            If .Length > 5 Then pdfdoc.Form.Fields(0).Kids(0).Kids("o6[0]").Value = .ToCharArray(5, 1)
        End With

        pdfdoc.Save(pdfmStream)
        With context.Response
            .ContentType = "application/pdf"
            .AppendHeader("title", "Direct Debit Instructions")
            .AppendHeader("content-disposition", "inline")
            .AppendHeader("filename", "DirectDebit.pdf")
            .OutputStream.Write(pdfmStream.ToArray, 0, CInt(pdfmStream.Length))
        End With
        pdfmStream.Dispose()
        pdfdoc.Clear()
        Return
    End Sub

    Private Sub BuildBill(ByRef context As System.Web.HttpContext)
        Dim oAssembly As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly
        Dim inStream As IO.Stream
        Dim outStream As IO.MemoryStream
        ABCpdf8.XSettings.InstallRedistributionLicense(AbcPdfLicenseKey)
        Dim instructions As New ABCpdf8.Doc()

        Try
            Try
                outStream = New IO.MemoryStream(DirectCast(DataObject.GetObjectFromCache(PDFCACHEKEY), Byte()))
            Catch
                outStream = Nothing
            End Try

            If outStream Is Nothing OrElse outStream.Length = 0 Then

                outStream = New IO.MemoryStream

                'tomke 9/29/11 170390 License info is built differently in v8 of abcpdf
                'instructions.SetInfo(0, "License", AbcPdfLicenseKey)

                inStream = oAssembly.GetManifestResourceStream(oAssembly.GetName.Name & ".DDInstructions.pdf")

                instructions.Read(inStream)

                GetOrgDetails()

                For Each fieldName In instructions.Form.GetFieldNames
                    Dim field As ABCpdf8.Objects.Field = instructions.Form(fieldName)
                    field.Focus()
                    instructions.Font = instructions.AddFont("ArialMT")

                    Select Case fieldName
                        Case NAMETOKEN
                            instructions.HPos = 0.5
                            instructions.VPos = 0.5
                            instructions.FontSize = 13
                            instructions.AddText(OrgName)
                            instructions.HPos = 0
                            instructions.VPos = 0
                        Case ADDRESSTOKEN
                            instructions.FontSize = 8

                            Dim addressLines As String = String.Concat(OrgName, vbCrLf, _
                                                                       Address1, vbCrLf, _
                                                                       Address2, vbCrLf, _
                                                                       CityPostCode, vbCrLf)

                            instructions.AddText(addressLines)

                        Case INSTRUCTOKEN
                            instructions.FontSize = 5
                            instructions.AddText(Instruction)
                        Case GUARANTEETOKEN
                            instructions.FontSize = 5

                            Dim guarantee As String = String.Concat("x This Guarantee is offered by all banks and building societies that accept instructions to pay Direct Debits ", vbCrLf, _
                            "x If there are any changes to the amount, date or frequency of your Direct Debit ", OrgName, " will notify you 10 working days in advance of your account being debited or as otherwise agreed. If you request ", OrgName, " to collect a payment, confirmation of the amount and date will be given to you at the time of the request.", vbCrLf, _
                            "x If an error is made in the payment of your Direct Debit, by ", OrgName, " or your bank or building society you are entitled to a full and immediate refund of the amount paid from your bank or building society", vbCrLf, _
                            "–  If you receive a refund you are not entitled to, you must pay it back when ", OrgName, " asks you to", vbCrLf, _
                            "x You can cancel a Direct Debit at any time by simply contacting your bank or building society. Written confirmation may be required. Please also notify us.")

                            instructions.AddText(guarantee)
                        Case Else
                            If fieldName.Contains(SERVICENUMBERTOKEN) Then
                                Dim SNDigit As Integer = CInt(fieldName.Replace(SERVICENUMBERTOKEN, String.Empty))
                                If ServiceUserID.Length > SNDigit Then
                                    instructions.FontSize = 10
                                    instructions.HPos = 0.5
                                    instructions.AddText(ServiceUserID(SNDigit))
                                    instructions.HPos = 0
                                End If
                            End If
                    End Select

                    instructions.Delete(field.ID)

                Next

                instructions.Save(outStream)
                DataObject.AddToCache(outStream.ToArray, PDFCACHEKEY, 60)
            End If

            With context.Response
                .ContentType = "application/pdf"
                .AppendHeader("title", "Direct Debit Instructions")
                .AppendHeader("content-disposition", "inline")
                .AppendHeader("filename", "DirectDebit.pdf")
                .OutputStream.Write(outStream.ToArray, 0, CInt(outStream.Length))
            End With

        Catch ex As Exception
            context.Response.StatusCode = 404

        Finally
            If outStream IsNot Nothing Then
                outStream.Close()
            End If
            If inStream IsNot Nothing Then
                inStream.Close()
            End If
            instructions.Clear()
        End Try

        Return
    End Sub

    Private Sub GetOrgDetails()
        OrgName = PortalSettings.Current.Client.Name
        ServiceUserID = PortalSettings.Current.Client.OrigID
        Address1 = PortalSettings.Current.Client.Address1
        Address2 = PortalSettings.Current.Client.Address2
        CityPostCode = String.Concat(PortalSettings.Current.Client.City, " ", PortalSettings.Current.Client.PostCode)
        Instruction = String.Concat("Please pay ", PortalSettings.Current.Client.Name, " Direct Debits from the account detailed in this Instruction subject to the safeguards assured by the Direct Debit Guarantee. I understand this Instruction may remain with ", PortalSettings.Current.Client.Name, " and, if so, details will be passed electronically to my Bank/Building Society.")
    End Sub

    Private Sub SendGenericPdf(ByRef context As System.Web.HttpContext)

        Dim filePath = context.Request.FilePath
        Dim startIndex = filePath.LastIndexOf("/") + 1
        Dim endIndex = filePath.ToLower().IndexOf(".ddpdf")
        Dim filename = filePath.Substring(startIndex, endIndex - startIndex)
        Dim title = context.Request.QueryString("title") & String.Empty
        Dim id = context.Request.QueryString("id") & String.Empty

        Dim pdfData = Core.Data.TempData.Fetch(id)

        If (Not String.IsNullOrEmpty(pdfData.TextData)) Then
            pdfData.BinaryData = Html2Pdf(pdfData.TextData, String.Empty)
        End If

        If pdfData.BinaryData IsNot Nothing Then
            context.Response.ContentType = "application/pdf"
            context.Response.AppendHeader("title", title)

            'CHF 04/22/2008 07:52 PM   Content-disposition "attachment" was causing issues with IE.  Just display it "inline".
            'context.Response.AppendHeader("content-disposition", "attachment; filename=" & filename)
            context.Response.AppendHeader("content-disposition", "inline; filename=" & filename)

            context.Response.OutputStream.Write(pdfData.BinaryData, 0, pdfData.BinaryData.Length)
        Else
            context.Response.StatusCode = 404
        End If

    End Sub

    Friend Shared Function Html2Pdf(ByVal html As String, ByVal password As String) As Byte()
        ABCpdf8.XSettings.InstallRedistributionLicense(AbcPdfLicenseKey)
        Dim pdfDoc As New ABCpdf8.Doc()
        pdfDoc.Rect.Inset(20, 20)
        'sterling CR328655-031110
        'give this 30 seconds rather than the default 15
        pdfDoc.HtmlOptions.Timeout = 30000

        If Not String.IsNullOrEmpty(Trim(html)) Then
            'CHF 05/21/2008 05:03 PM.  Sometimes not all of the HTML gets rendered.  This seems to help.
            html &= "<br/ >"

            'CHF:  This method doesn't handle all html tags.  For instance, it doesn't handle images.
            'pdfDoc.AddHtml(html)

            'This is a little better but only works when CSS and images are referenced with an absolute url.
            'sterling CR327184-121609
            'adding the base element to the head of the HTML allows this to work with relative links
            'and bypasses all the extra work done below
            Dim id As Integer
            Try
                id = pdfDoc.AddImageHtml(AddBaseElementToHTML(html))
            Catch ex As Internal.PDFException
                'CR320252-060209 - Do not remove this try catch unless the new changes do not break this CR.
                'An exception is thrown if the HTML is blank.  Just eat it.
                LogErrorToDB(ex, False)
            End Try
            'This will work with relative URLs
            'Dim id As Integer
            'Try
            '	id = pdfDoc.AddImageUrl(HtmlPreview.GetUrlForHtml(html, String.Empty, False))
            'Catch ex As ABCpdf8.Internal.PDFException
            '	'An exception is thrown if the HTML is blank.  Just eat it.
            'End Try

            Do While pdfDoc.Chainable(id)
                pdfDoc.Page = pdfDoc.AddPage()
                id = pdfDoc.AddImageToChain(id)
            Loop
            For i As Integer = 1 To pdfDoc.PageCount
                pdfDoc.PageNumber = i
                pdfDoc.Flatten()
            Next
        End If

        pdfDoc.Encryption.Type = 2
        pdfDoc.Encryption.OwnerPassword = Core.Common.RandomString(20)
        pdfDoc.Encryption.Password = password
        pdfDoc.Encryption.CanAssemble = False
        pdfDoc.Encryption.CanChange = False
        pdfDoc.Encryption.CanCopy = True
        pdfDoc.Encryption.CanEdit = False
        pdfDoc.Encryption.CanExtract = True
        pdfDoc.Encryption.CanFillForms = False

        Using pdfStream As New IO.MemoryStream()
            pdfDoc.Save(pdfStream)
            Return pdfStream.ToArray()
        End Using
    End Function
    'sterling CR327184-121609
    'adding the base element to the head of the HTML allows the ABCPDF generator to work 
    'when passed html with relative links in it
    Private Shared Function AddBaseElementToHTML(ByVal sHTML As String) As String
        Dim oBuilder As New System.Text.StringBuilder

        oBuilder.AppendLine("<html>")
        oBuilder.AppendLine("<head>")

        'GregWa CR329114-041210 - https does not allow images to display properly in an image, either due to a limitation in pdfs or the pdf library
        '<base> is only used for images according to Sterling, which should impact only images
        oBuilder.AppendLine(String.Concat("<base href=""", URLBuilder.BaseUrlPrefix(False), """ />"))

        If ConditionalContent.HTMLContainsConditionalContent(sHTML) Then
            oBuilder.AppendLine(String.Format("<link type=""text/css"" href=""{0}"" rel=""stylesheet"" />", "Client/Styles/cuteeditor.css"))
        End If

        oBuilder.AppendLine("</head>")
        oBuilder.AppendLine("<body>")
        oBuilder.AppendLine(sHTML)
        oBuilder.AppendLine("</body>")
        oBuilder.AppendLine("</html>")

        Return oBuilder.ToString
    End Function

    Friend Shared Function GetUrlForPdf(ByVal html As String, ByVal filenameForPdf As String) As String
        Return GetUrlForPdf(html, filenameForPdf, URLBuilder.IsSSL)
    End Function

    Friend Shared Function GetUrlForPdf(ByVal html As String, ByVal filenameForPdf As String, ByVal useSsl As Boolean) As String
        Dim id = Guid.NewGuid()
        Core.Data.TempData.Set(id, html, TimeSpan.FromMinutes(1))
        Dim url = String.Format("{0}{1}.ddpdf?id={2}", URLBuilder.BaseUrlPrefix(useSsl), filenameForPdf, id)
        If useSsl Then
            Return url.Replace("http://", "https://")
        Else
            Return url.Replace("https://", "http://")
        End If
    End Function

End Class
