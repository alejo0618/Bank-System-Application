using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_System_Application
{
    public class MainMenu : View
    {
        private int userChoice;

        public override void Controller()
        {
            // Displaying the view, based on Login.txt file in View Folder
            Show();
            
            // Verifying basic inputs
            while (VerifylInputs() == true)
            {
                // Setting the view values to the object
                InitialiseInstanceParameters();

                switch (userChoice)
                {
                    case 1:
                        CreateAccount createAccount = new CreateAccount();
                        createAccount.Controller();
                        break;
                    case 2:
                        FindAccount findAccount = new FindAccount();
                        findAccount.Controller();
                        break;
                    case 3:
                        Deposit deposit = new Deposit();
                        deposit.Controller();
                        break;
                    case 4:
                        Withdraw withdraw = new Withdraw();
                        withdraw.Controller();
                        break;
                    case 5:
                        Statement statement = new Statement();
                        statement.Controller();
                        break;
                    case 6:
                        DeleteAccount deleteAccount = new DeleteAccount();
                        deleteAccount.Controller();
                        break;
                    case 7:
                        return;
                    default:
                        break;
                }
                
                DumpBodyParameters();
                Show();

            }

        }

        public override void InitialiseInstanceParameters()
        {
            // There is only one parameter: User option choice (1 - 7)
            userChoice = int.Parse(bodyParameterList[0].Value);            
        }

    }
}
