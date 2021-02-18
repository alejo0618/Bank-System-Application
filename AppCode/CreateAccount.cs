using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_System_Application
{
    class CreateAccount : AccountManagement
    {        

        public override void Controller()
        {

            // Displaying the view, based on Login.txt file in View Folder
            Show();

            // Verifying basic inputs
            while (VerifylInputs() == true)                
            {
                if (MessageBox("Is the information correct (Y/N)?", ControlMessage.YesNo, AlignmentType.Left) == "Y")
                {
                    // Setting the view values to the object
                    InitialiseInstanceParameters();

                    // Specialised validation regarding email account:
                    // Regex email is too complex. For that reason, a simple validation is performed before the account creation.                   

                    if (Email.Contains('@') && (Email.Contains("gmail.com") || Email.Contains("outlook.com") || Email.Contains("uts.edu.au")))
                    {

                        // Creating an account
                        if (CreateAccount(out string err) == true)
                        {
                            // Displaying confirmation to the user
                            MessageBox(string.Format("Account created: {0}. The information will be send via email", AccountNumber), ControlMessage.Null, AlignmentType.Left);
                            System.Threading.Thread.Sleep(1000);

                            // When user does not provide a valid email, a message is displayed.
                            if (GenerateStatement("Creation Account statement", out err) == true)
                            {
                                MessageBox(err, ControlMessage.Null, AlignmentType.Center);
                            }
                            else
                            {
                                MessageBox(err, ControlMessage.Error, AlignmentType.Center);
                            }

                            System.Threading.Thread.Sleep(1000);

                        }
                        else
                        {
                            MessageBox(err, ControlMessage.Null, AlignmentType.Left);
                        }
                        return;
                    }
                    else
                    {
                        // Email error
                        MessageBox($"Email invalid! ", ControlMessage.Null, AlignmentType.Left);
                    }                    
                }
            }

        }

        public override void InitialiseInstanceParameters()
        {
            FirstName = bodyParameterList[0].Value;
            LastName = bodyParameterList[1].Value;
            Address = bodyParameterList[2].Value;
            Phone = ulong.Parse(bodyParameterList[3].Value);
            Email = bodyParameterList[4].Value;
        }
    }
}
