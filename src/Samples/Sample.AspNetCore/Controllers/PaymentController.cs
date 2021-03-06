﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Sample.AspNetCore.Data;
using Sample.AspNetCore.Extensions;
using Sample.AspNetCore.Models;

using SwedbankPay.Sdk;

using System;
using System.Linq;
using System.Threading.Tasks;

using SwedbankPay.Sdk.Payments;

namespace Sample.AspNetCore.Controllers
{
    public class PaymentController : Controller
    {
        private readonly Cart cartService;
        private readonly StoreDbContext context;
        private readonly ISwedbankPayClient swedbankPayClient;


        public PaymentController(Cart cartService, StoreDbContext context, ISwedbankPayClient swedbankPayClient)
        {
            this.cartService = cartService;
            this.context = context;
            this.swedbankPayClient = swedbankPayClient;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AbortPaymentOrder(string paymentOrderId)
        {
            try
            {
                var paymentOrder = await this.swedbankPayClient.PaymentOrders.Get(new Uri(paymentOrderId, UriKind.RelativeOrAbsolute));

                var response = await paymentOrder.Operations.Abort();

                TempData["AbortMessage"] = $"Payment Order: {response.PaymentOrderResponseObject.Id} has been {response.PaymentOrderResponseObject.State}";
                this.cartService.PaymentOrderLink = null;

                return RedirectToAction(nameof(Index), "Products");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction(nameof(Index), "Orders");
            }
        }


        [HttpGet]
        public async Task<ActionResult> PaymentOrderCancel(string paymentOrderId)
        {
            try
            {
                var paymentOrder = await this.swedbankPayClient.PaymentOrders.Get(new Uri(paymentOrderId, UriKind.RelativeOrAbsolute));
                
                if (paymentOrder.Operations.Cancel != null)
                {
                    var cancelRequest = new SwedbankPay.Sdk.PaymentOrders.PaymentOrderCancelRequest(DateTime.Now.Ticks.ToString(), "Cancelling parts of the total amount");
                    var response = await paymentOrder.Operations.Cancel(cancelRequest);
                    TempData["CancelMessage"] = $"Payment has been cancelled: {response.Cancellation.Transaction.Id}";
                    this.cartService.PaymentOrderLink = null;
                }
                else
                {
                    TempData["ErrorMessage"] = "Operation not available";
                }

                return RedirectToAction("Details", "Orders");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction("Details", "Orders");
            }
        }



        [HttpGet]
        public async Task<ActionResult> Cancel(string paymentId, PaymentInstrument instrument)
        {
            try
            {
                switch (instrument)
                {
                    case PaymentInstrument.CreditCard:
                        await PaymentHelper.CancelCreditCardPayment(paymentId, this.swedbankPayClient, TempData, this.cartService);
                        break;
                    case PaymentInstrument.Trustly:
                        await PaymentHelper.CancelTrustlyPayment(paymentId, this.swedbankPayClient, TempData, this.cartService);
                        break;
                }

                return RedirectToAction("Details", "Orders");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction("Details", "Orders");
            }
        }


        [HttpGet]
        public async Task<IActionResult> PaymentOrderCapture(string paymentOrderId)
        {
            try
            {
                var transActionRequestObject = await GetCaptureRequest("Capturing the authorized payment");
                var paymentOrder = await this.swedbankPayClient.PaymentOrders.Get(new Uri(paymentOrderId, UriKind.RelativeOrAbsolute));

                var response = await paymentOrder.Operations.Capture(transActionRequestObject);

                TempData["CaptureMessage"] =
                    $"{response.Capture.Transaction.Id}, {response.Capture.Transaction.State}, {response.Capture.Transaction.Type}";

                this.cartService.PaymentOrderLink = null;

                return RedirectToAction("Details", "Orders");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction("Details", "Orders");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Capture(string paymentId, PaymentInstrument instrument)
        {
            try
            {
                var description = "Capturing the authorized payment";
                var transActionRequestObject = await GetCaptureRequest(description);

                switch (instrument)
                {
                    case PaymentInstrument.CreditCard:
                        var cardPayment = await this.swedbankPayClient.Payments.CardPayments.Get(new Uri(paymentId, UriKind.RelativeOrAbsolute));

                        var order = await this.context.Orders.Include(l => l.Lines).ThenInclude(p => p.Product).FirstOrDefaultAsync();
                        var orderItems = order.Lines.ToOrderItems();

                        var captureRequest = new SwedbankPay.Sdk.Payments.CardPayments.CardPaymentCaptureRequest(Amount.FromDecimal(order.Lines.Sum(e => e.Quantity * e.Product.Price)), Amount.FromDecimal(0), orderItems.ToList(), description, DateTime.Now.Ticks.ToString());
                        var response = await cardPayment.Operations.Capture(captureRequest);
                        TempData["CaptureMessage"] = $"{response.Capture.Transaction.Id}, {response.Capture.Transaction.State}, {response.Capture.Transaction.Type}";
                        break;
                }

                this.cartService.PaymentLink = null;
                return RedirectToAction("Details", "Orders");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction("Details", "Orders");
            }
        }


        [HttpPost]
        public void OnCompleted(string paymentLinkId)
        {
            if (this.cartService.CartLines != null && this.cartService.CartLines.Any())
            {
                var products = this.cartService.CartLines.Select(p => p.Product);
                this.context.Products.AttachRange(products);

                this.context.Orders.Add(new Order
                {
                    PaymentOrderLink = this.cartService.PaymentOrderLink != null ? new Uri(this.cartService.PaymentOrderLink, UriKind.RelativeOrAbsolute) : null,
                    PaymentLink = !string.IsNullOrWhiteSpace(paymentLinkId) ? new Uri(paymentLinkId, UriKind.RelativeOrAbsolute) : null,
                    Instrument = this.cartService.Instrument,
                    Lines = this.cartService.CartLines.ToList()
                });
                this.context.SaveChanges(true);
                this.cartService.Clear();
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentOrderReversal(string paymentOrderId)
        {
            try
            {
                var transActionRequestObject = await GetReversalRequest("Reversing the capture amount");
                var paymentOrder = await this.swedbankPayClient.PaymentOrders.Get(new Uri(paymentOrderId, UriKind.RelativeOrAbsolute));

                var response = await paymentOrder.Operations.Reverse.Invoke(transActionRequestObject);

                TempData["ReversalMessage"] =
                    $"{response.Reversal.Transaction.Id}, {response.Reversal.Transaction.Type}, {response.Reversal.Transaction.State}";
                this.cartService.PaymentOrderLink = null;

                return RedirectToAction("Details", "Orders");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction("Details", "Orders");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Reversal(string paymentId, PaymentInstrument instrument)
        {
            try
            {
                var description = "Reversing the captured amount";
                ReversalResponse response = null;

                var order = await this.context.Orders.Include(l => l.Lines).ThenInclude(p => p.Product).FirstOrDefaultAsync();
                switch (instrument)
                {
                    case PaymentInstrument.Swish:
                        response = await PaymentHelper.ReverseSwishPayment(paymentId, order, description, this.swedbankPayClient);
                        break;
                    case PaymentInstrument.CreditCard:
                        response = await PaymentHelper.ReverseCreditCardPayment(paymentId, order, description, this.swedbankPayClient);
                        break;
                    case PaymentInstrument.Trustly:
                        response = await PaymentHelper.ReverseTrustlyPayment(paymentId, order, description, this.swedbankPayClient);
                        break;
                }

                TempData["ReversalMessage"] = $"{response.Reversal.Transaction.Id}, {response.Reversal.Transaction.Type}, {response.Reversal.Transaction.State}";
                this.cartService.PaymentOrderLink = null;

                return RedirectToAction("Details", "Orders");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Something unexpected happened. {e.Message}";
                return RedirectToAction("Details", "Orders");
            }
        }


        private async Task<SwedbankPay.Sdk.PaymentOrders.PaymentOrderCaptureRequest> GetCaptureRequest(string description)
        {
            var order = await this.context.Orders.Include(l => l.Lines).ThenInclude(p => p.Product).FirstOrDefaultAsync();
            var orderItems = order.Lines.ToOrderItems();

            return new SwedbankPay.Sdk.PaymentOrders.PaymentOrderCaptureRequest(Amount.FromDecimal(order.Lines.Sum(e => e.Quantity * e.Product.Price)),
                                                                  Amount.FromDecimal(0), orderItems.ToList(), description, DateTime.Now.Ticks.ToString());
        }

        private async Task<SwedbankPay.Sdk.PaymentOrders.PaymentOrderReversalRequest> GetReversalRequest(string description)
        {
            var order = await this.context.Orders.Include(l => l.Lines).ThenInclude(p => p.Product).FirstOrDefaultAsync();
            var orderItems = order.Lines.ToOrderItems();

            return new SwedbankPay.Sdk.PaymentOrders.PaymentOrderReversalRequest(Amount.FromDecimal(order.Lines.Sum(e => e.Quantity * e.Product.Price)),
                                                              Amount.FromDecimal(0), orderItems.ToList(), description, DateTime.Now.Ticks.ToString());
        }
    }
}