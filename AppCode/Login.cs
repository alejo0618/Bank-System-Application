using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bank_System_Application
{
    public class Login : View
    {
        private string user;
        private string password;

        public override void Controller()
        {
            // Displaying the view, based on Login.txt file in View Folder
            Show();

            // Verifying basic inputs
            while (VerifylInputs() == true)
            {
                // Setting the view values to the object
                InitialiseInstanceParameters();

                // Specialised validations (using now instance variables): 
                // 1. Verifying credentials in .txt file
                if (VerifyUserPassword() == true)
                {
                    MainMenu mainMenu = new MainMenu();
                    mainMenu.Controller();
                    return;
                }
                else
                {
                    // Displaying error message
                    MessageBox("User and/or password are incorrect!", ControlMessage.Null, AlignmentType.Center);
                }
            }
        }

        public override void InitialiseInstanceParameters()
        {
            user = bodyParameterList[0].Value;
            password = bodyParameterList[1].Value;          
        }

        /// <summary>
        /// Specialised validation: The method validates the inputs user and password and check the information
        /// whithin the folder AppData/login.txt.
        /// </summary>
        /// <returns>Return true when both parameters are founded. Otherwise return false</returns>
        private bool VerifyUserPassword()
        {
            // Finding the view layout through relative path.
            string fileName = "login.txt";
            string relativefilePath = string.Format(@"..\..\..\AppData\{0}",fileName);
            if (!File.Exists(relativefilePath))
            {
                throw new Exception(string.Format(@"File '{0}' not found. Imposible to validate user and password! ", fileName));
            }

            // Reading the longin.txt at once
            string[] usrPwdList = File.ReadAllLines(relativefilePath);
            if (usrPwdList[0] != "USER||PASSWORD")
            {
                throw new Exception(string.Format(@"File '{0}' does not contain the proper format of user and password! ", fileName));
            }
            for (int i = 1; i < usrPwdList.Length; i++)
            {
                string[] usrPwd = usrPwdList[i].Split("||");
                if (usrPwd.Length != 2)
                {
                    throw new ArgumentOutOfRangeException();
                }
                
                if (user == usrPwd[0] && password == usrPwd[1])
                {
                    return true;
                }
            }

            return false;

        }

    }
}
