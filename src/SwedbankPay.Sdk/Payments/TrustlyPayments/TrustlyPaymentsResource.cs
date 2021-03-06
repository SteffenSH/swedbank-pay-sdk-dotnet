﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SwedbankPay.Sdk.Payments.TrustlyPayments
{
    public class TrustlyPaymentsResource : ResourceBase, ITrustlyPaymentsResource
    {
        public TrustlyPaymentsResource(HttpClient httpClient) : base(httpClient)
        {
        }

        public Task<TrustlyPayment> Create(TrustlyPaymentRequest paymentRequest)
        {
            return Create(paymentRequest, PaymentExpand.None);
        }

        public Task<TrustlyPayment> Create(TrustlyPaymentRequest paymentRequest, PaymentExpand paymentExpand)
        {
            return TrustlyPayment.Create(paymentRequest, this.httpClient, GetExpandQueryString(paymentExpand));
        }

        public Task<TrustlyPayment> Get(Uri id)
        {
            return Get(id, PaymentExpand.None);
        }

        public Task<TrustlyPayment> Get(Uri id, PaymentExpand paymentExpand)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return TrustlyPayment.Get(id, this.httpClient, GetExpandQueryString(paymentExpand));
        }
    }
}
