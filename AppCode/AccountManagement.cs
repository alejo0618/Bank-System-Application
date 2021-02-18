using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Mail;

namespace Bank_System_Application
{
    public abstract class AccountManagement : View
    {
        // Folder where all the accounts are stored.
        private const string accountsPath = @"..\..\..\AppData\Accounts\";

        // Constant to identify the .txt file format (Regarding personal information)
        private const string accountFormatPi = "NAME||LAST NAME||ADDRESS||PHONE||EMAIL||BALANCE";
        private const string accountFormatTr = "DATE||TRANSACTION||AMOUNT||BALANCE";

        // Accounts with lenght = 6.
        // Current account number is a class variable to allow all the instances know the value
        protected static int currentAccountNumber;

        protected int AccountNumber { get; set; }
        protected string FirstName { get; set; }
        protected string LastName { get; set; }
        protected string Address { get; set; }
        protected ulong Phone { get; set; }
        protected string Email { get; set; }
        protected double AccountBalance { get; set; }

        // Small type that does not provide any behaviour. Useful to group and access easily each transaction
        protected struct Transaction
        {
            public enum Type { Deposit, Withdraw };
            public DateTime TransactionDate { get; set; }
            public Type TransactionType { get; set; }
            public double Amount { get; set; }
            public double Balance { get; set; }

        }

        protected List<Transaction> TransactionLog = new List<Transaction>();
        

        // Static constructor used to initialise static variables like currentAccountNumber
        static AccountManagement()
        {
            currentAccountNumber = 0;
            // Reading all the accounts created within the directory to define the next account id to create.
            string[] accounts = Directory.GetFiles(accountsPath, "*.txt", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < accounts.Length; i++)
            {
                string account = Path.GetFileName(accounts[i]);
                account = account.Replace(".txt", "");
                // Validate the account lenght = 6. Also, all the characters must be int to set the current account number
                if (account.Length >= 6 && int.TryParse(account, out int tempInt) == true && tempInt > currentAccountNumber)
                {
                    currentAccountNumber = tempInt;
                }
            }

            // Set the max current account number when there are accounts created. Otherwise, the value 100k is set by default.
            currentAccountNumber = currentAccountNumber > 0 ? currentAccountNumber + 1 : 100000;

        }

        /// <summary>
        /// Create an account. The account number is generated automatically by the static constructor
        /// </summary>
        /// <param name="err">Error message</param>
        /// <returns></returns>
        protected bool CreateAccount(out string err)
        {
            AccountNumber = currentAccountNumber;
            if (SaveAccount(out err) == true)
            {
                // Once a new file has been created, the current account number is increased by 1
                currentAccountNumber += 1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Store the object information in a .txt file
        /// </summary>
        /// <param name="err">Error message</param>
        /// <returns>Return true if the account is stored successfully. Otherwise, return false</returns>
        protected bool SaveAccount(out string err)
        {
            err = string.Empty;            
            string[] accountFile = new string[4 + TransactionLog.Count];

            accountFile[0] = accountFormatPi;
            accountFile[1] = $"{FirstName}||{LastName}||{Address}||{Phone}||{Email}||{AccountBalance}";
            accountFile[2] = string.Empty;
            accountFile[3] = accountFormatTr;

            int i = 4;
            foreach (Transaction tr in TransactionLog)
            {
                
                accountFile[i] = $"{tr.TransactionDate:yyyy/MM/dd HH:mm:ss}||{tr.TransactionType}||{tr.Amount}||{tr.Balance}";
                i += 1;
            }

            try
            {                
                string path = string.Format("{0}{1}.txt", accountsPath, AccountNumber);
                File.WriteAllLines(path, accountFile);
            }
            catch (Exception)
            {
                err = $"Error trying to save/update the file: {AccountNumber}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method tries to find the AccountNumber within the folder AppData/Accounts.
        /// Also, it verifies when a file is properly formated to read all the information.
        /// </summary>
        /// <param name="err">Error Message</param>
        /// <returns>Return true if the account is found and the format fits. Otherwise, return false</returns>
        protected bool FindAccount(out string err)
        {
            // All the elements are deleted. Otherwise, it will store rubish
            TransactionLog.Clear();            

            // Reading all the accounts created within the directory to define the next account id to create.
            string[] accounts = Directory.GetFiles(accountsPath, "*.txt", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < accounts.Length; i++)
            {
                string account = Path.GetFileName(accounts[i]);
                account = account.Replace(".txt", "");

                // Validate the account lenght = 6. Also, all the characters must be int to set the current account number
                if (account.Length >= 6 && int.TryParse(account, out int tempInt) == true && tempInt == AccountNumber)
                {
                    // The file could be readed without try-catch because Directory.GetFiles is realible enough.
                    string[] lines = File.ReadAllLines(accounts[i]);

                    // Verifying the file integrity in two steps: A. Basic Validation B. Content Validation

                    // A. Basic Validation
                    // 1. Min lenght: 3 rows. (Based on CreateAccount Method)
                    // 2. Verifying the strings stored previously Account Format Personal Information and Transactions
                    if (lines.Length < 4 || lines[0] != accountFormatPi || lines[3] != accountFormatTr)
                    {
                        err = "Account information corrupted!";
                        return false;
                    }

                    // B. Content Validation
                    string[] accountPi = lines[1].Split("||");
                    if (accountPi.Length < 5 || ulong.TryParse(accountPi[3],out ulong phone) == false || 
                        double.TryParse(accountPi[5], out double balance) == false)
                    {
                        err = "Account information corrupted!";
                        return false;
                    }

                    // Now it is posssible to assign the values related to Personal Information
                    FirstName = accountPi[0];
                    LastName = accountPi[1];
                    Address = accountPi[2];
                    Phone = phone;
                    Email = accountPi[4];
                    AccountBalance = balance;

                    // Now it is posssible to assign the values related to transaction Log
                    for (int j = 4; j < lines.Length; j++)
                    {
                        string[] accountTr = lines[j].Split("||");
                        
                        // Initialising the object before adding it to the log
                        Transaction tr = new Transaction
                        {
                            TransactionDate = DateTime.Parse(accountTr[0]),
                            TransactionType = (Transaction.Type) Enum.Parse(typeof(Transaction.Type), accountTr[1]),
                            Amount = double.Parse(accountTr[2]),
                            Balance = double.Parse(accountTr[3]),
                        };

                        // Finally, the register log is added to the list
                        TransactionLog.Add(tr);
                    }

                    // All good!
                    err = string.Empty;
                    return true;
                }
            }

            err = "Account not found!";
            return false;
        }

        /// <summary>
        /// Method that deposit an amount in the account number file
        /// </summary>
        /// <param name="amount">Amount to deposit</param>
        /// <param name="err">Error message</param>
        /// <returns>Return true if the account is stored successfully. Otherwise, return false</returns>
        protected bool DepositFunds(double amount, out string err)
        {
            // Note: This method must be called once an account has been founded

            AccountBalance += amount;

            // Initialising the object before adding it to the log
            Transaction tr = new Transaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = Transaction.Type.Deposit,
                Amount = amount,
                Balance = AccountBalance
            };
            
            TransactionLog.Add(tr);
            return SaveAccount(out err);
            
        }

        /// <summary>
        /// Method that withdraw an amount from the account number file associated
        /// </summary>
        /// <param name="amount">Amount to withdraw</param>
        /// <param name="err">Error message</param>
        /// <returns>Return true if the account is stored successfully.
        /// Return false when there is no enough funds to withdraw</returns>
        protected bool WithdrawFunds(double amount, out string err)
        {
            // Note: This method must be called once an account has been founded

            if (AccountBalance < amount)
            {
                err = "The account does not have enough funds!";
                return false;
            }

            AccountBalance -= amount;

            // Initialising the object before adding it to the log
            Transaction tr = new Transaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = Transaction.Type.Withdraw,
                Amount = -amount,
                Balance = AccountBalance
            };
            
            TransactionLog.Add(tr);
            return SaveAccount(out err);
        }

        /// <summary>
        /// Method that deletes an account number file associated
        /// </summary>        
        /// <param name="err">Error message</param>
        /// <returns>Return true if the account is deleted successfully.        
        protected bool DeleteAccount(out string err)
        {

            // Just find the account before continue
            if (FindAccount(out err) == false)
            {
                return false;
            }

            try
            {
                string path = string.Format("{0}{1}.txt", accountsPath, AccountNumber);
                File.Delete(path);                
            }
            catch (Exception)
            {
                err = "It is not possible to delete the account at this moment!";
                return false;
            }

            err = "Account Deleted successfully!";
            return true;
        }

        /// <summary>
        /// Method that sends an email to the account associated
        /// </summary>
        /// <param name="subject">Message title</param>
        /// <param name="err">Error message</param>
        /// <returns>Return true if the account statement is generated successfully. 
        protected bool GenerateStatement(string subject, out string err)
        {
            // Stub method
            err = "Statement generated successfully!";
            return true;

        }

    }
}
