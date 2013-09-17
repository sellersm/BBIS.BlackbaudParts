'This class is a simple wrapper around the BBNCExtensions.API.Transactions.PaymentArgs class.
'Just poke in all the transaction info that you have (probably from a web form somewhere)
'Then, call the "GeneratePaymentArgs()" method and pass the result to a RecordDonation call.
'Example usage (This example assumes it is being used from within a DisplayPart):
'=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
'Dim donationAmount As Decimal = Convert.ToDecimal(txbAmount.Text)
''create a new BBPSPaymentInfo object, and tell it all about our transaction
'Dim payment As BBPSPaymentInfo = New BBPSPaymentInfo()
'payment.DemoMode = True
'payment.MerchantAccountID = 2 'get this from the BBIS->Administration->Merchant Accounts
'payment.SkipCardValidation = True
'payment.AddDesignationInfo(donationAmount, "Test designation comment", 1)
'payment.AppealID = 1
'payment.Comments = "Test payment comment"
'payment.PaymentMethod = BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod.CreditCard
'payment.CreditCardCSC = txbCCV2.Text
'payment.CreditCardExpirationMonth = ddlExpireMonth.SelectedValue
'payment.CreditCardExpirationYear = ddlExpireYear.SelectedValue
'payment.CreditCardHolderName = txbName.Text
'payment.CreditCardNumber = txbCardNum.Text
'payment.CreditCardType = CType(ddlCardType.SelectedValue, BBNCExtensions.Interfaces.Services.CreditCardType)
'payment.DonorStreetAddress = txbStreetAddress.Text
'payment.DonorCity = txbCity.Text
'payment.DonorStateProvince = ddlState.SelectedValue
'payment.DonorZIP = txbZip.Text
'payment.EmailAddress = txbEmail.Text
''actually process the paymen there (and get a reply containing the details of what happened)
'Dim paymentReply As BBNCExtensions.API.Transactions.Donations.RecordDonationReply = Me.API.Transactions.RecordDonation(payment.GeneratePaymentArgs())

Public Class BBPSPaymentInfo
    Private m_DemoMode As Boolean = True
    Private m_MerchantAcctID As Integer = 0
    Private m_SkipCardValidation As Boolean = True
    Private m_DesignationInfo As New List(Of BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo)
    Private m_Comments As String = ""
    Private m_AppealID As Integer = 0
    Private m_PaymentMethod As BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod = BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod.CreditCard
    Private m_CreditCardCSC As String = ""
    Private m_CreditCardExpirationMonth As Integer = 0
    Private m_CreditCardExpirationYear As Integer = 0
    Private m_CreditCardHolderName As String = ""
    Private m_CreditCardNumber As String = ""
    Private m_CreditCardType As BBNCExtensions.Interfaces.Services.CreditCardType = Nothing
    Private m_FirstName As String = ""
    Private m_LastName As String = ""
    Private m_DonorCity As String = ""
    Private m_DonorStateProvince As String = ""
    Private m_DonorStreetAddress As String = ""
    Private m_DonorZIP As String = ""
    Private m_EmailAddress As String = ""
    Private m_Message As String = ""

    Public Structure PaymentReplyInterpretation
        Public Success As Boolean
        Public Message As String
    End Structure

    Public Property DemoMode As Boolean
        Get
            Return m_DemoMode
        End Get
        Set(ByVal value As Boolean)
            m_DemoMode = value
        End Set
    End Property

    Public Property MerchantAccountID As Integer
        Get
            Return m_MerchantAcctID
        End Get
        Set(ByVal value As Integer)
            m_MerchantAcctID = value
        End Set
    End Property

    Public Property SkipCardValidation As Boolean
        Get
            Return m_SkipCardValidation
        End Get
        Set(ByVal value As Boolean)
            m_SkipCardValidation = value
        End Set
    End Property

    Public Property DesignationInfo As List(Of BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo)
        Get
            Return m_DesignationInfo
        End Get
        Set(ByVal value As List(Of BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo))
            m_DesignationInfo = value
        End Set
    End Property

    Public Property Comments As String
        Get
            Return m_Comments
        End Get
        Set(ByVal value As String)
            m_Comments = value
        End Set
    End Property

    Public Property AppealID As Integer
        Get
            Return m_AppealID
        End Get
        Set(ByVal value As Integer)
            m_AppealID = value
        End Set
    End Property

    Public Property PaymentMethod As BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod
        Get
            Return m_PaymentMethod
        End Get
        Set(ByVal value As BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod)
            m_PaymentMethod = value
        End Set
    End Property

    Public Property CreditCardCSC As String
        Get
            Return m_CreditCardCSC
        End Get
        Set(ByVal value As String)
            m_CreditCardCSC = value
        End Set
    End Property

    Public Property CreditCardExpirationMonth As Integer
        Get
            Return m_CreditCardExpirationMonth
        End Get
        Set(ByVal value As Integer)
            m_CreditCardExpirationMonth = value
        End Set
    End Property

    Public Property CreditCardExpirationYear As Integer
        Get
            Return m_CreditCardExpirationYear
        End Get
        Set(ByVal value As Integer)
            m_CreditCardExpirationYear = value
        End Set
    End Property

    Public Property CreditCardHolderName As String
        Get
            Return m_CreditCardHolderName
        End Get
        Set(ByVal value As String)
            'redefine "firstname" and "lastname" based on the full name
            m_CreditCardHolderName = value
            Dim splitname As String()
            splitname = value.Split(" ")
            If splitname.Length > 0 Then
                m_FirstName = splitname(0)
                If splitname.Length = 1 Then
                    m_LastName = ""
                Else
                    m_FirstName = splitname(0)
                End If
            End If
        End Set
    End Property

    Public Property CreditCardNumber As String
        Get
            Return m_CreditCardNumber
        End Get
        Set(ByVal value As String)
            m_CreditCardNumber = value
        End Set
    End Property

    Public Property CreditCardType As BBNCExtensions.Interfaces.Services.CreditCardType
        Get
            Return m_CreditCardType
        End Get
        Set(ByVal value As BBNCExtensions.Interfaces.Services.CreditCardType)
            m_CreditCardType = value
        End Set
    End Property

    Public Property FirstName As String
        Get
            Return m_FirstName
        End Get
        Set(ByVal value As String)
            m_FirstName = value
        End Set
    End Property

    Public Property LastName As String
        Get
            Return m_LastName
        End Get
        Set(ByVal value As String)
            m_LastName = value
        End Set
    End Property

    Public Property DonorCity As String
        Get
            Return m_DonorCity
        End Get
        Set(ByVal value As String)
            m_DonorCity = value
        End Set
    End Property

    Public Property DonorStateProvince As String
        Get
            Return m_DonorStateProvince
        End Get
        Set(ByVal value As String)
            m_DonorStateProvince = value
        End Set
    End Property

    Public Property DonorStreetAddress As String
        Get
            Return m_DonorStreetAddress
        End Get
        Set(ByVal value As String)
            m_DonorStreetAddress = value
        End Set
    End Property

    Public Property DonorZIP As String
        Get
            Return m_DonorZIP
        End Get
        Set(ByVal value As String)
            m_DonorZIP = value
        End Set
    End Property

    Public Property EmailAddress As String
        Get
            Return m_EmailAddress
        End Get
        Set(ByVal value As String)
            m_EmailAddress = value
        End Set
    End Property

    Public ReadOnly Property StatusMessage As String
        Get
            Return m_Message
        End Get
    End Property

    Public Sub AddDesignationInfo(ByVal Amount As Decimal, ByVal Description As String, ByVal BackOfficeID As Integer)
        Try
            Dim newdes As New BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo
            newdes.Amount = Amount
            newdes.BackofficeId = BackOfficeID
            newdes.Description = Description
            DesignationInfo.Add(newdes)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function GeneratePaymentArgs() As BBNCExtensions.API.Transactions.PaymentArgs
        'translate the properties the caller has passed in into a proper PaymentArgs object

        'Ready to save donation. Start over with empty payment arguments object and fill
        'with data from form.
        Dim Result As New BBNCExtensions.API.Transactions.PaymentArgs

        'always return "approved" from CC gateway (don't actually clear the card)
        Result.DemoMode = DemoMode

        'selected mechant account id from bbnc - can get this number from the merchant acocunt display in admin 
        Result.MerchantAccountId = MerchantAccountID

        'for testing purposes allows any mindless string of data as acceptable as the cc number.
        Result.SkipCardNumberValidation = SkipCardValidation

        'add selected designation
        For Each des As BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo In DesignationInfo
            Result.Designations.Add(des)
        Next
        'You can accept comments from the user, or hard code some if you like
        Result.Comments = Comments

        'we selected the appeal to use in the part's editor
        'any Appeal ID is ok.  Would probably configure this in the part's editor.
        Result.AppealID = AppealID

        'only taking credit card pmts in this sample, 
        'but check out the other options...BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod enum
        Result.PaymentMethod = PaymentMethod
        Result.CreditCardCSC = CreditCardCSC
        Result.CreditCardExpirationMonth = CreditCardExpirationMonth
        Result.CreditCardExpirationYear = CreditCardExpirationYear
        Result.CreditCardHolderName = CreditCardHolderName
        Result.CreditCardNumber = CreditCardNumber
        Result.CreditCardType = CreditCardType

        Dim nameParts As String() = CreditCardHolderName.Split(" ")
        Result.FirstName = nameParts(0)
        Result.LastName = nameParts(nameParts.Length - 1)
        Result.DonorAddress.City = DonorCity
        Result.DonorAddress.StateProvince = DonorStateProvince
        Result.DonorAddress.StreetAddress = DonorStreetAddress
        Result.DonorAddress.ZIP = DonorZIP
        Result.EmailAddress = EmailAddress

        Return Result
    End Function

    Public Function InterpretPaymentReply(ByVal reply As BBNCExtensions.API.Transactions.Donations.RecordDonationReply) As PaymentReplyInterpretation
        Dim Result As New PaymentReplyInterpretation
        Result.Message = ""
        Result.Success = False
        Try
            If reply.CreditCardAuthorizationResponse IsNot Nothing Then
                Select Case reply.CreditCardAuthorizationResponse.ResultCode
                    Case BBNCExtensions.Interfaces.Services.ECreditCardResultCode.Approved
                        Result.Success = True
                        Result.Message += "Transaction was approved"
                    Case BBNCExtensions.Interfaces.Services.ECreditCardResultCode.BBValidationError
                        Result.Success = False
                        Result.Message += String.Format("Error processing credit card. {0}", reply.CreditCardAuthorizationResponse.BBValidationErrorCode.ToString)
                    Case BBNCExtensions.Interfaces.Services.ECreditCardResultCode.GateWayDecline
                        Result.Success = False
                        Result.Message += "Error processing credit card. Gateway declined."
                    Case Else
                        Result.Success = False
                        Result.Message += "An unknown error occurred while processing the transaction."
                End Select
            Else
                'in demo mode, or non-credit card purchase (pledge)
                Result.Success = True
                Result.Message += "Pledges and Demo mode transactions are always approved."
            End If
        Catch exArgOutOfRange As ArgumentOutOfRangeException
            Result.Message += exArgOutOfRange.ParamName
        Catch exArg As ArgumentException
            Result.Message += exArg.Message
        Catch exWeb As BBNCExtensions.API.WebServiceException
            Result.Message += exWeb.Message
        Catch exIncomplete As BBNCExtensions.API.Transactions.Donations.IncompleteDonationTransaction
            'this is rare.  It means the card was cleared but a network or other error prevented the donation transaction
            'from being recorded in BBIS. Do NOT want to force user to resubmit form - exit gracefully and figure out what to do
            Result.Message += String.Concat(exIncomplete.Message, " [", exIncomplete.InnerException.Message, "]")
        Catch exAPI As BBNCExtensions.API.Transactions.Donations.RecordDonationException
            Result.Message += exAPI.Message
        Catch ex As Exception
            Result.Message += ex.Message
        End Try
        Return Result
    End Function
End Class
