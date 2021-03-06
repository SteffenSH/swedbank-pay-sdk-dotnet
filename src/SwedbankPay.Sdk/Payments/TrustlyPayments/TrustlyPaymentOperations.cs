﻿using SwedbankPay.Sdk.Extensions;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SwedbankPay.Sdk.Payments.TrustlyPayments
{
    public class TrustlyPaymentOperations : OperationsBase
    {
        public TrustlyPaymentOperations(OperationList operations, HttpClient client)
        {
            foreach (var httpOperation in operations)
            {
                switch (httpOperation.Rel.Value)
                {
                    case PaymentResourceOperations.RedirectSale:
                        RedirectSale = httpOperation;
                        break;

                    case PaymentResourceOperations.ViewSales:
                        ViewSale = httpOperation;
                        break;

                    case PaymentResourceOperations.UpdatePaymentAbort:
                        Abort = async payload =>
                            await client.SendAsJsonAsync<TrustlyPaymentResponse>(httpOperation.Method, httpOperation.Href, payload);
                        break;

                    case PaymentResourceOperations.CreateReversal:
                        Reverse = async payload =>
                            await client.SendAsJsonAsync<ReversalResponse>(httpOperation.Method, httpOperation.Href, payload);
                        break;

                    case PaymentResourceOperations.CreateCancellation:
                        Cancel = async payload =>
                            await client.SendAsJsonAsync<CancellationResponse>(httpOperation.Method, httpOperation.Href, payload);
                        break;
                    default:
                        break;
                }
                this.Add(httpOperation.Rel, httpOperation);
            }
        }

        public HttpOperation RedirectSale { get; }
        public HttpOperation ViewSale { get; }
        public Func<PaymentAbortRequest, Task<TrustlyPaymentResponse>> Abort { get; }
        public Func<TrustlyPaymentReversalRequest, Task<ReversalResponse>> Reverse { get; }
        public Func<TrustlyPaymentCancelRequest, Task<CancellationResponse>> Cancel { get; }
    }
}