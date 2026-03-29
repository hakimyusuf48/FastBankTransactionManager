using System;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using System.Xml.Serialization;

namespace FastBankTransactionManager
{
    class Transaction
    {
        public int TransactionID { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public int AccountNumber { get; set; }
    }
    class FastBank
    {
        List<Transaction> transaction = new List<Transaction>();

        public void AddTransaction() 
        {
            //User information
            Console.WriteLine("Enter Transaction Type (deposit, withdrawal, transfer): ");
            string transactionType = Console.ReadLine().ToLower();
            Console.WriteLine("Enter Amount: ");
            double amount = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Enter Account Number: ");
            int accountNumber = Convert.ToInt32(Console.ReadLine());  
                      
            //Add to list
            double balance = transaction.Where(t => t.AccountNumber == accountNumber).Sum(t => t.Amount);

            switch (transactionType)
            {
                case "deposit": 
                    transaction.Add(new Transaction { TransactionID = transaction.Count + 1, Type = transactionType, Amount = amount, AccountNumber = accountNumber });
                    Console.WriteLine("Transaction Added. Transaction ID: " + transaction.Count);
                    break;
                case "withdrawal":
                    if (balance >= amount)
                    {
                        transaction.Add(new Transaction { TransactionID = transaction.Count + 1, Type = transactionType, Amount = -amount, AccountNumber = accountNumber });
                        Console.WriteLine("Transaction Added. Transaction ID: " + transaction.Count);
                        Console.WriteLine("Transaction Added.");
                    }
                    else
                    {
                        Console.WriteLine($"Insufficient Funds");
                    }
                    break;
                case "transfer":
                    Console.WriteLine("Enter Receiver Account Number: ");
                    int receiverAccount = Convert.ToInt32(Console.ReadLine());
                    transaction.Add(new Transaction { TransactionID = transaction.Count + 1, Type = "transfer-out", Amount = -amount, AccountNumber = accountNumber });
                    transaction.Add(new Transaction { TransactionID = transaction.Count + 1, Type = "transfer-in", Amount = amount, AccountNumber = receiverAccount });
                    Console.WriteLine($"Transfer of {amount} from Account {accountNumber} to Account {receiverAccount} completed.");
                    Console.WriteLine("Transaction Added. Transaction ID: " + transaction.Count);
                    break;
                default:
                    Console.WriteLine("Invalid Transaction Type.");
                    break;
            }
        }
        public void RemoveTransaction()
        {
            //User input
            Console.WriteLine("Enter Transaction ID to Remove: ");
            int transactionID = Convert.ToInt32(Console.ReadLine());

            //Remove at index
            int tID = transaction.FindIndex(t => t.TransactionID == transactionID);

            if (tID != -1)
            {
                transaction.RemoveAt(tID);
                Console.WriteLine("Transaction Removed.");
            }
            else
            {
                Console.WriteLine("Transaction Not Found.");
            }

        }
        public void SearchTransaction()
        {
            Console.WriteLine("Enter Account Number to Search: ");
            int accountNumber = Convert.ToInt32(Console.ReadLine());

            var results = transaction.Where(t => t.AccountNumber == accountNumber).ToList();

            if (results.Count > 0)
            {
                foreach (Transaction t in results)
                    Console.WriteLine($"ID: {t.TransactionID}, Type: {t.Type}, Amount: {t.Amount}, Account: {t.AccountNumber}");
                    double total = results.Sum(t => t.Amount);
                    Console.WriteLine($"Total Balance for Account {accountNumber}: {total}");
            }
            else
            {
                Console.WriteLine("No Transactions Found for this Account.");
            }
        }
        public void DisplayAllTransactions()
        {
            foreach (var t in transaction)
            {
                Console.WriteLine($"Transaction ID: {t.TransactionID}, Type: {t.Type}, Amount: {t.Amount}, Account Number: {t.AccountNumber}");
            }
        }
        public async Task ProcessConcurrentTransactions()
        {
            await Task.Delay(100); // simulate DB call
            AddTransaction();
        }
        public void DisplayTransactionSummary()
        {
            Console.WriteLine($"Total Transactions: {transaction.Count}");
            Console.WriteLine($"Total Amount: {transaction.Sum(t => t.Amount)}");
        }
    }
    class Program 
    {
        static async Task Main(string[] args)
        {
            //Constructor
            FastBank bank = new FastBank();

            //Menu Loop
            int choice = 0;
            while (choice != 7) {
                //Menu
                string menu = "1. Add Transaction \n" +
                    "2. Remove Transaction \n" +
                    "3. Search Transaction \n" +
                    "4. Display All Transactions \n" +
                    "5. Process Concurrent Transactions \n" +
                    "6. Display Transaction Summary \n" +
                    "7. Exit \n" +
                    "Choose an option: \n";
                Console.WriteLine(menu);
                choice = Convert.ToInt32(Console.ReadLine());

                //Implemnetation of the methods
                switch (choice)
                {
                    case 1:
                        bank.AddTransaction();
                        break;
                    case 2:
                        bank.RemoveTransaction();
                        break;
                    case 3:
                        bank.SearchTransaction();
                        break;
                    case 4:
                        bank.DisplayAllTransactions();
                        break;
                    case 5:
                        await bank.ProcessConcurrentTransactions();
                        break;
                    case 6:
                        bank.DisplayTransactionSummary();
                        break;
                    case 7:
                        break;
                }
            }
        }
    }
}