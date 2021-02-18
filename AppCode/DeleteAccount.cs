using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_System_Application
{
    class DeleteAccount : AccountManagement
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

                    // Double check before delete the account!
                    if (MessageBox("Do you want to delete the account (Y/N)?", ControlMessage.YesNo, AlignmentType.Left) == "Y")
                    {
                        // Now it's possible to delete it.
                        if (DeleteAccount(out err) == true)
                        {
                            MessageBox(string.Format("Account deleted successfully!", AccountNumber), ControlMessage.Null, AlignmentType.Center);
                        }
                        else
                        {                            
                            MessageBox(err, ControlMessage.Error, AlignmentType.Left);
                        }

                        System.Threading.Thread.Sleep(3000);
                        return;
                    }
                }
                else
                {
                    // Account not found, displaying an error message
                    MessageBox(err, ControlMessage.Error, AlignmentType.Left);
                }

                if (MessageBox("Delete another account (Y/N)?", ControlMessage.YesNo, AlignmentType.Left) == "Y")
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

        public override void InitialiseInstanceParameters()
        {
            AccountNumber = int.Parse(bodyParameterList[0].Value);
        }
    }
}
