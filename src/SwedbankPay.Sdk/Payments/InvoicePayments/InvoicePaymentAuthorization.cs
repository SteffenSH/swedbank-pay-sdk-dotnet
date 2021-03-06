﻿using System;

namespace SwedbankPay.Sdk.Payments.InvoicePayments
{
    public class InvoicePaymentAuthorization : IdLink
    {
        public InvoicePaymentAuthorization(
                             AuthorizationTransaction transaction)
        {
            Transaction = transaction;
        }

        public AuthorizationTransaction Transaction { get; }
    }

    public class AuthorizationTransaction : IdLink
    {
        public AuthorizationTransaction(DateTime created,
                                        DateTime updated,
                                        string type,
                                        State state,
                                        string number,
                                        Amount amount,
                                        Amount vatAmount,
                                        string description,
                                        string payeeReference,
                                        bool isOperational,
                                        OperationList operations,
                                        string failedReason = null,
                                        string failedActivityName = null,
                                        string failedErrorCode = null,
                                        string failedErrorDescription = null,
                                        ProblemResponse problem = null)
        {
            Created = created;
            Updated = updated;
            Type = type;
            State = state;
            Number = number;
            Amount = amount;
            VatAmount = vatAmount;
            Description = description;
            PayeeReference = payeeReference;
            FailedReason = failedReason;
            FailedActivityName = failedActivityName;
            FailedErrorCode = failedErrorCode;
            FailedErrorDescription = failedErrorDescription;
            IsOperational = isOperational;
            Problem = problem;
            Operations = operations;
        }


        public Amount Amount { get; }
        public DateTime Created { get; }
        public string Description { get; }
        public string FailedActivityName { get; }
        public string FailedErrorCode { get; }
        public string FailedErrorDescription { get; }
        public string FailedReason { get; }
        public bool IsOperational { get; }
        public string Number { get; }
        public OperationList Operations { get; }
        public string PayeeReference { get; }
        public ProblemResponse Problem { get; }
        public State State { get; }
        public string Type { get; }
        public DateTime Updated { get; }
        public Amount VatAmount { get; }
    }
}