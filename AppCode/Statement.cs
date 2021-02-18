using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_System_Application
{
    class Statement : AccountManagement
    {        

        public override void Controller()
        {

            // Displaying the view, based on Login.txt file in View Folder
            Show();

            // Verifying basic inputs
            while (VerifylInputs() == true)                
            {

                // Setting the view values to the object
                InitialiseInstanceParameters();

                if (FindAccount(out string err) == true)
                {

                    string[] accountInformation = TransactionLog.Count > 5 ? new string[14] : new string[9 + TransactionLog.Count];

                    accountInformation[0] = $"Account Found: {AccountNumber}";                    
                    accountInformation[1] = $"First Name: {FirstName}";
                    accountInformation[2] = $"Last Name: {LastName}";
                    accountInformation[3] = $"Address: {Address}";
                    accountInformation[4] = $"Phone: {Phone}";
                    accountInformation[5] = $"Email: {Email}";
                    accountInformation[6] = $"Account Balance: {AccountBalance}";
                    accountInformation[7] = $"";
                    accountInformation[8] = "       DATE        | TRANSACTION |    AMOUNT   |    BALANCE   ";
                    string[] fields = accountInformation[8].Split("|");

                    // Unlike Deposit, Withdraw, and delete account, the statement will display aditionally the last 5 transactions
                    int j = 9;
                    for (int i = TransactionLog.Count <= 5 ? 0 : TransactionLog.Count - 5; i < TransactionLog.Count; i++)
                    {
                        Transaction tr = TransactionLog[i];

                        try
                        {
                            // The purpose here is to fill the values with spaces, to show the information nicely, without set the cursor in particular positions
                            accountInformation[j] = string.Format("{0}|{1}{2}|{3}{4}|{5}{6}",
                                                                    tr.TransactionDate.ToString("yyyy/MM/dd HH:mm:ss"),
                                                                    tr.TransactionType, string.Format(" ").PadRight(fields[1].Length - tr.TransactionType.ToString().Length, ' '),
                                                                    tr.Amount, string.Format(" ").PadRight(fields[2].Length - tr.Amount.ToString().Length, ' '),
                                                                    tr.Balance, string.Format(" ").PadRight(fields[1].Length - tr.Balance.ToString().Length, ' '));
                        }
                        catch (ArgumentOutOfRangeException)
                        {

                            // Printing the values in a really ugly way because there is no more space
                            accountInformation[j] = string.Format("{0}|{1}|{2}|{3}",
                                                                    tr.TransactionDate.ToString("yyyy/MM/dd HH:mm:ss"),
                                                                    tr.TransactionType,
                                                                    tr.Amount,
                                                                    tr.Balance);

                        }

                        j += 1;
                    }
                    
                    MessageBody(accountInformation);

                    // Double check to email the statement
                    if (MessageBox("Email Statement? (Y/N)?", ControlMessage.YesNo, AlignmentType.Left) == "Y")
                    {
                        // Now it is possible to send the information
                        if (GenerateStatement($"Statement Account:{AccountNumber}", out err) == true)
                        {
                            MessageBox("Statement sent successfully!", ControlMessage.Null, AlignmentType.Left);
                            System.Threading.Thread.Sleep(3000);
                            return;
                        }
                        else
                        {
                            MessageBox(err, ControlMessage.Error, AlignmentType.Left);
                        }
                    }
                }
                else
                {
                    MessageBox(err, ControlMessage.Error, AlignmentType.Left);
                }

                if (MessageBox("Check another account (Y/N)?", ControlMessage.YesNo, AlignmentType.Left) == "N")
                {
                    return;
                }
                else 
                {
                    bodyParameterList[0].Value = string.Empty;
                    Show();
                }
            }

        }

        public override void InitialiseInstanceParameters()
        {
            AccountNumber = int.Parse(bodyParameterList[0].Value);
        }
    }
}
