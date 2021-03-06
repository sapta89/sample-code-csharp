﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;

namespace net.authorize.sample
{
    public class PayPalPriorAuthorizationCapture
    {
        public static ANetApiResponse Run(String ApiLoginID, String ApiTransactionKey, string TransactionID)
        {
            Console.WriteLine("PayPal Prior Authorization Transaction");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey
            };

            var payPalType = new payPalType
            {
                cancelUrl = "http://www.merchanteCommerceSite.com/Success/TC25262",
                successUrl = "http://www.merchanteCommerceSite.com/Success/TC25262",     // the url where the user will be returned to            
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = payPalType };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.priorAuthCaptureTransaction.ToString(),    // capture a prior authorization
                payment     = paymentType,
                amount      = 19.45m,
                refTransId  = TransactionID                                                     // the TransID value that was returned from the first AuthOnlyTransaction call
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            //validate
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response.transactionResponse != null)
                {
                    Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                }
            }
            else if(response != null)
            {
                Console.WriteLine("Error: " + response.messages.message[0].code + "  " + response.messages.message[0].text);
                if (response.transactionResponse != null)
                {
                    Console.WriteLine("Transaction Error : " + response.transactionResponse.errors[0].errorCode + " " + response.transactionResponse.errors[0].errorText);
                }
            }

            return response;
        }
    }
}
