//This class is a simple wrapper around the BBNCExtensions.API.Transactions.PaymentArgs class.
//Just poke in all the transaction info that you have (probably from a web form somewhere)
//Then, call the "GeneratePaymentArgs()" method and pass the result to a RecordDonation call.
//Example usage (This example assumes it is being used from within a DisplayPart):
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
//Dim donationAmount As Decimal = Convert.ToDecimal(txbAmount.Text)
//create a new BBPSPaymentInfo object, and tell it all about our transaction
//Dim payment As BBPSPaymentInfo = New BBPSPaymentInfo()
//payment.DemoMode = True
//payment.MerchantAccountID = 2 'get this from the BBIS->Administration->Merchant Accounts
//payment.SkipCardValidation = True
//payment.AddDesignationInfo(donationAmount, "Test designation comment", 1)
//payment.AppealID = 1
//payment.Comments = "Test payment comment"
//payment.PaymentMethod = BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod.CreditCard
//payment.CreditCardCSC = txbCCV2.Text
//payment.CreditCardExpirationMonth = ddlExpireMonth.SelectedValue
//payment.CreditCardExpirationYear = ddlExpireYear.SelectedValue
//payment.CreditCardHolderName = txbName.Text
//payment.CreditCardNumber = txbCardNum.Text
//payment.CreditCardType = CType(ddlCardType.SelectedValue, BBNCExtensions.Interfaces.Services.CreditCardType)
//payment.DonorStreetAddress = txbStreetAddress.Text
//payment.DonorCity = txbCity.Text
//payment.DonorStateProvince = ddlState.SelectedValue
//payment.DonorZIP = txbZip.Text
//payment.EmailAddress = txbEmail.Text
//actually process the paymen there (and get a reply containing the details of what happened)
//Dim paymentReply As BBNCExtensions.API.Transactions.Donations.RecordDonationReply = Me.API.Transactions.RecordDonation(payment.GeneratePaymentArgs())
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    public class BBPSPaymentInfo
    {
        private bool demoMode;
        public bool DemoMode
        {
            get { return this.demoMode; }
            set { this.demoMode = value; }
        }

        private int merchantAcctID;
        public int MerchantAcctID
        {
            get { return this.merchantAcctID; }
            set { this.merchantAcctID = value; }
        }

        private bool skipCardValidation;
        public bool SkipCardValidation
        {
            get { return this.skipCardValidation; }
            set { this.skipCardValidation = value; }
        }

        private bool skipCreateGiftTransaction;
        public bool SkipCreateGiftTransaction
        {
            get { return this.skipCreateGiftTransaction; }
            set { this.skipCreateGiftTransaction = value; }
        }        

        private List<BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo> designationInfo;
        public List<BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo> DesignationInfo
        {
            get { return this.designationInfo; }
            set { this.designationInfo = value; }
        }

        private string comments;
        public string Comments
        {
            get { return this.comments; }
            set { this.comments = value; }
        }

        private int appealID;
        public int AppealID
        {
            get { return this.appealID; }
            set { this.appealID = value; }
        }

        private BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod paymentMethod;
        public BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod PaymentMethod
        {
            get { return this.paymentMethod; }
            set { this.paymentMethod = value; }
        }

        private string creditCardCSC;
        public string CreditCardCSC
        {
            get { return this.creditCardCSC; }
            set { this.creditCardCSC = value; }
        }

        private int creditCardExpirationMonth;
        public int CreditCardExpirationMonth
        {
            get { return this.creditCardExpirationMonth; }
            set { this.creditCardExpirationMonth = value; }
        }

        private int creditCardExpirationYear;
        public int CreditCardExpirationYear
        {
            get { return this.creditCardExpirationYear; }
            set { this.creditCardExpirationYear = value; }
        }

        private string creditCardHolderName;
        public string CreditCardHolderName
        {
            get { return this.creditCardHolderName; }
            set 
            {
                this.creditCardHolderName = value;
                string[] splitName = value.Split(' ');
                if (splitName.Length > 0)
                {
                    this.firstName = splitName[0];
                    if (splitName.Length > 1)
                    {
                        this.lastName = splitName[1];
                    }
                }
            }
        }

        private string creditCardNumber;
        public string CreditCardNumber
        {
            get { return this.creditCardNumber; }
            set { this.creditCardNumber = value; }
        }

        private BBNCExtensions.Interfaces.Services.CreditCardType creditCardType;
        public BBNCExtensions.Interfaces.Services.CreditCardType CreditCardType
        {
            get { return this.creditCardType; }
            set { this.creditCardType = value; }
        }

        private string firstName;
        public string FirstName
        {
            get { return this.firstName; }
            set { this.firstName = value; }
        }

        private string lastName;
        public string LastName
        {
            get { return this.lastName; }
            set { this.lastName = value; }
        }

        private string donorCity;
        public string DonorCity
        {
            get { return this.donorCity; }
            set { this.donorCity = value; }
        }

        private string donorStateProvince;
        public string DonorStateProvince
        {
            get { return this.donorStateProvince; }
            set { this.donorStateProvince = value; }
        }

        private string donorStreetAddress;
        public string DonorStreetAddress
        {
            get { return this.donorStreetAddress; }
            set { this.donorStreetAddress = value; }
        }

        private string donorZIP;
        public string DonorZIP
        {
            get { return this.donorZIP; }
            set { this.donorZIP = value; }
        }

        private string emailAddress;
        public string EmailAddress
        {
            get { return this.emailAddress; }
            set { this.emailAddress = value; }
        }

        private Guid bbpid;
        public Guid Bbpid
        {
            get { return this.bbpid; }
            set { this.bbpid = value; }
        }

        private string message = string.Empty;
        public string StatusMessage
        {
            get { return this.message; }
        }

        public BBPSPaymentInfo()
        {
            this.designationInfo = new List<BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo>();
            this.paymentMethod = BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod.CreditCard;
            this.skipCardValidation = true;
        }

        public struct PaymentReplyInterpretation
        {
            public bool Success;
            public string Message;
        }

        public void AddDesignationInfo(decimal amount, string description, int backOfficeId)
        {
            BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo designation = new BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo();
            designation.Amount = amount;
            designation.BackofficeId = backOfficeId;
            designation.Description = description;
            this.designationInfo.Add(designation);
        }

        public BBNCExtensions.API.Transactions.PaymentArgs GeneratePaymentArgs()
        {
            BBNCExtensions.API.Transactions.PaymentArgs results = new BBNCExtensions.API.Transactions.PaymentArgs();
            results.SkipCreateGiftTransaction = this.skipCreateGiftTransaction;
            results.DemoMode = this.demoMode;
            results.MerchantAccountId = this.merchantAcctID;
            results.BBPS_MerchantAccountID = this.bbpid;
            results.SkipCardNumberValidation = this.skipCardValidation;            

            foreach (BBNCExtensions.API.Transactions.PaymentArgs.DesignationInfo d in this.designationInfo)
            {
                results.Designations.Add(d);
            }
            results.Comments = this.comments;
            results.AppealID = this.appealID;

            results.PaymentMethod = this.paymentMethod;
            if (this.paymentMethod == BBNCExtensions.API.Transactions.PaymentArgs.ePaymentMethod.CreditCard)
            {
                results.CreditCardCSC = this.creditCardCSC;
                results.CreditCardExpirationMonth = this.creditCardExpirationMonth;
                results.CreditCardExpirationYear = this.creditCardExpirationYear;
                results.CreditCardHolderName = this.creditCardHolderName;
                results.CreditCardType = this.creditCardType;
                results.CreditCardNumber = this.creditCardNumber;
            }

            string[] nameParts = this.creditCardHolderName.Split(' ');
            results.FirstName = nameParts[0];
            results.LastName = nameParts[nameParts.Length - 1];
            results.DonorAddress.City = this.donorCity;
            results.DonorAddress.StateProvince = this.donorStateProvince;
            results.DonorAddress.StreetAddress = this.donorStreetAddress;
            results.DonorAddress.ZIP = this.donorZIP;
            results.EmailAddress = this.emailAddress;

            return results;
        }

        public PaymentReplyInterpretation InterpretPaymentReply(BBNCExtensions.API.Transactions.Donations.RecordDonationReply reply)
        {
            PaymentReplyInterpretation results = new PaymentReplyInterpretation();
            results.Message = "";
            results.Success = false;            

            try
            {
                if (reply.CreditCardAuthorizationResponse != null)
                {
                    switch (reply.CreditCardAuthorizationResponse.ResultCode)
                    {
                        case BBNCExtensions.Interfaces.Services.ECreditCardResultCode.Approved:
                            results.Success = true;
                            results.Message += "Transaction was approved";
                            break;
                        case BBNCExtensions.Interfaces.Services.ECreditCardResultCode.BBValidationError:
                            results.Success = false;
                            results.Message += String.Format("Error processing credit card. {0}", reply.CreditCardAuthorizationResponse.BBValidationErrorCode.ToString());
                            break;
                        case BBNCExtensions.Interfaces.Services.ECreditCardResultCode.GateWayDecline:
                            results.Success = false;
                            results.Message += "Error processing credit card. Gateway declined.";
                            break;
                    }
                }
                else
                {
                    results.Success = true;
                    results.Message = "Pledges and Demo mode transactions are always approved.";
                }
            }
            catch (ArgumentOutOfRangeException exRange)
            {
                results.Message += exRange.ParamName;
            }
            catch (ArgumentException exAug)
            {
                results.Message += exAug.Message;
            }
            catch (BBNCExtensions.API.WebServiceException exWebservice)
            {
                results.Message += exWebservice.Message;
            }
            catch (BBNCExtensions.API.Transactions.Donations.IncompleteDonationTransaction exIncomplete)
            {
                results.Message += String.Concat(exIncomplete.Message, " [", exIncomplete.InnerException.Message, "]");
            }
            catch (BBNCExtensions.API.Transactions.Donations.RecordDonationException exDonation)
            {
                results.Message += exDonation.Message;
            }
            catch (Exception ex)
            {
                results.Message += ex.Message;
            }
            return results;
        }
    }
