using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseSharingApp
{
    class Program
    {
        static void Main()
        {
            // Step 1: Get input from the user
            Console.WriteLine("Enter the number of people:");
            int numPeople = int.Parse(Console.ReadLine());

            // Create a dictionary to store names and payments
            Dictionary<string, double> payments = new Dictionary<string, double>();

            for (int i = 0; i < numPeople; i++)
            {
                Console.WriteLine($"Enter the name of person {i + 1}:");
                string name = Console.ReadLine();

                Console.WriteLine($"Enter how much {name} has paid:");
                double payment = double.Parse(Console.ReadLine());

                // Store the name and payment in the dictionary
                payments[name] = payment;
            }

            // Step 2: Calculate total payments
            double totalPayments = payments.Values.Sum();

            // Step 3: Calculate average payment
            double averagePayment = totalPayments / numPeople;

            // Step 4: Calculate net balances
            Dictionary<string, double> netBalances = new Dictionary<string, double>();
            foreach (var kvp in payments)
            {
                string name = kvp.Key;
                double payment = kvp.Value;

                // Calculate net balance (payment - average payment)
                double netBalance = payment - averagePayment;
                netBalances[name] = netBalance;
            }

            // Step 5: Determine payments
            List<Tuple<string, string, double>> transactions = DetermineTransactions(netBalances);

            // Step 6: Display results
            Console.WriteLine("\nTransactions:");
            foreach (var transaction in transactions)
            {
                string from = transaction.Item1;
                string to = transaction.Item2;
                double amount = transaction.Item3;

                Console.WriteLine($"{from} should pay {to} {amount:C2}");
            }
        }

        static List<Tuple<string, string, double>> DetermineTransactions(Dictionary<string, double> netBalances)
        {
            // Create lists of debtors (people who owe) and creditors (people who are owed)
            List<(string name, double balance)> debtors = netBalances
                .Where(kvp => kvp.Value < 0)
                .Select(kvp => (kvp.Key, kvp.Value))
                .ToList();

            List<(string name, double balance)> creditors = netBalances
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => (kvp.Key, kvp.Value))
                .ToList();

            List<Tuple<string, string, double>> transactions = new List<Tuple<string, string, double>>();

            // Iterate through debtors and creditors and match them up to determine transactions
            foreach (var debtor in debtors)
            {
                string debtorName = debtor.name;
                double amountOwed = Math.Abs(debtor.balance);

                while (amountOwed > 0)
                {
                    var creditor = creditors.First();
                    string creditorName = creditor.name;
                    double amountAvailable = creditor.balance;

                    // Determine the amount to transfer
                    double transferAmount = Math.Min(amountOwed, amountAvailable);

                    // Record the transaction
                    transactions.Add(Tuple.Create(debtorName, creditorName, transferAmount));

                    // Update net balances
                    netBalances[debtorName] += transferAmount;
                    netBalances[creditorName] -= transferAmount;

                    // Update the amounts owed and available
                    amountOwed -= transferAmount;
                    amountAvailable -= transferAmount;

                    // Update the creditor in the list
                    creditors[0] = (creditorName, amountAvailable);

                    // Remove the creditor from the list if their balance reaches zero
                    if (amountAvailable == 0)
                    {
                        creditors.RemoveAt(0);
                    }
                }
            }

            return transactions;
        }
    }
}
