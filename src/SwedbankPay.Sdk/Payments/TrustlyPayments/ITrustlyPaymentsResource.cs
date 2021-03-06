﻿using System;
using System.Threading.Tasks;

namespace SwedbankPay.Sdk.Payments.TrustlyPayments
{
    public interface ITrustlyPaymentsResource
    {
        /// <summary>
        ///     Creates a new Trustly payment
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <returns></returns>
        Task<TrustlyPayment> Create(TrustlyPaymentRequest paymentRequest);

        /// <summary>
        ///     Creates a new Trustly payment
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <param name="paymentExpand"></param>
        /// <returns></returns>
        Task<TrustlyPayment> Create(TrustlyPaymentRequest paymentRequest, PaymentExpand paymentExpand);

        ///// <summary>
        /////     Gets an existing swish payment.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="paymentExpand"></param>
        ///// <returns></returns>
        Task<TrustlyPayment> Get(Uri id);


        ///// <summary>
        /////     Gets an existing swish payment.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="paymentExpand"></param>
        ///// <returns></returns>
        Task<TrustlyPayment> Get(Uri id, PaymentExpand paymentExpand);
    }
}
