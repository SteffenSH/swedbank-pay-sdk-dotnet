﻿using Newtonsoft.Json;

namespace SwedbankPay.Sdk.Payments.CardPayments
{
    public class CardPaymentAuthorizationRequestTransaction
    {
        [JsonConstructor]
        protected internal CardPaymentAuthorizationRequestTransaction(string cardNumber,
                                                    int cardExpiryMonth,
                                                    int cardExpiryYear,
                                                    string cardVerificationCode = null,
                                                    string cardHolderName = null)
        {
            CardNumber = cardNumber;
            CardExpiryMonth = cardExpiryMonth;
            CardExpiryYear = cardExpiryYear;
            CardVerificationCode = cardVerificationCode;
            CardHolderName = cardHolderName;
        }


        public int CardExpiryMonth { get; }
        public int CardExpiryYear { get; }
        public string CardHolderName { get; }

        public string CardNumber { get; }
        public string CardVerificationCode { get; }
    }
}