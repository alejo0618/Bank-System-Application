using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_System_Application
{
    class FindAccount : AccountManagement
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
