﻿using System;

namespace SwedbankPay.Sdk.Payments.SwishPayments
{
    public class SaleListItem
    {
        public SaleListItem(DateTime date,
                            string paymentRequestToken,
                            string payerAlias,
                            string swishPaymentReference,
                            string swishStatus,
                            Uri id,
                            Transaction transaction)
        {
            Date = date;
            PaymentRequestToken = paymentRequestToken;
            PayerAlias = payerAlias;
            SwishPaymentReference = swishPaymentReference;
            SwishStatus = swishStatus;
            Id = id;
            Transaction = transaction;
        }


        public DateTime Date { get; }
        public Uri Id { get; }
        public string PayerAlias { get; }
        public string PaymentRequestToken { get; }
        public string SwishPaymentReference { get; }
        public string SwishStatus { get; }
        public Transaction Transaction { get; }
    }
}