using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_System_Application
{
    class Withdraw : AccountManagement
    {
        private double amount = double.NaN;
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

                    string[] accountInformation = new string[8];
                    accountInformation[0] = $"Account Found: {AccountNumber}";
                    accountInformation[1] = $"First Name: {FirstName}";
                    accountInformation[2] = $"Last Name: {LastName}";
                    accountInformation[3] = $"Address: {Address}";
                    accountInformation[4] = $"Phone: {Phone}";
                    accountInformation[5] = $"Email: {Email}";
                    accountInformation[6] = "";
                    accountInformation[7] = $"Account Balance: {AccountBalance}";
                    MessageBody(accountInformation);

                    if (AccountBalance <= 0)
                    {
                        MessageBox("The account does not have funds to withdraw", ControlMessage.Error, AlignmentType.Center);
                        System.Threading.Thread.Sleep(3000);
                        return;
                    }

                    while (amount <= 0 || double.IsNaN(amount) == true)
                    {
                        if (double.IsNaN(amount) == false)
                        {
                            MessageBox("Only positive values are accepted", ControlMessage.Error, AlignmentType.Left);
                        }
                        amount = double.Parse(MessageBox("Amount ($):", ControlMessage.doubleInput, AlignmentType.Left));
                    }

                    if (WithdrawFunds(amount, out err) == true)
                    {
                        MessageBox($"Withdraw successfull! New balance:{AccountBalance}", ControlMessage.Null, AlignmentType.Left);
                    }
                    else
                    {
                        MessageBox(err, ControlMessage.Null, AlignmentType.Left);
                    }

                    // Before return the control to the main menu, the console is freeze to display the last message properly
                    System.Threading.Thread.Sleep(3000);
                    return;

                }
                else
                {
                    // Account not found, displaying an error message
                    MessageBox(err, ControlMessage.Error, AlignmentType.Left);
                    if (MessageBox("Retry (Y/N)?", ControlMessage.YesNo, AlignmentType.Left) == "Y")
                    {
                        bodyParameterList[0].Value = string.Empty;
                        Show();
                    }
                    else
                    {
                        return;
                    }
                }
            }

        }

        public override void InitialiseInstanceParameters()
        {
            AccountNumber = int.Parse(bodyParameterList[0].Value);
            // Amount is a value obtained in a 2nd step, so it's not assigned here
            //amount = double.Parse(bodyParameterList[0].Value);
        }
    }
}
