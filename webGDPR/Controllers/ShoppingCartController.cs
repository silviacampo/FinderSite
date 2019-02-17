//https://github.com/braintree/braintree_dotnet
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNetCore.Mvc;

namespace webGDPR.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            var gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = "4k9zfcyqwx7hxm3q",
                PublicKey = "srmvzzcx587k9m55",
                PrivateKey = "45d9c1fc489e981a17964d3cc007c510"
            };

            var clientToken = gateway.ClientToken.Generate(
                new ClientTokenRequest
                {
                    CustomerId = "12"
                }
            );

            TransactionRequest request = new TransactionRequest
            {
                Amount = 1000.00M,
                PaymentMethodNonce = "nonceFromTheClient",
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;
                Console.WriteLine("Success!: " + transaction.Id);
            }
            else if (result.Transaction != null)
            {
                Transaction transaction = result.Transaction;
                Console.WriteLine("Error processing transaction:");
                Console.WriteLine("  Status: " + transaction.Status);
                Console.WriteLine("  Code: " + transaction.ProcessorResponseCode);
                Console.WriteLine("  Text: " + transaction.ProcessorResponseText);
            }
            else
            {
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    Console.WriteLine("Attribute: " + error.Attribute);
                    Console.WriteLine("  Code: " + error.Code);
                    Console.WriteLine("  Message: " + error.Message);
                }
            }


            return View();
        }

        public IActionResult CheckoutReturn()
        {
            return View();
        }
    }
}