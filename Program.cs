using System;

namespace Bank_System_Application
{
    class Program
    {
        static void Main(string[] args)
        {
            // Basic console configuration: New Title
            Console.Title = "Banking System";
            // Basic console configuration: Backgroun color and white font
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            // Calling the Entry point view
            Login MenuLogin = new Login();
            MenuLogin.Controller();

            // Finally, cleaning the console window.
            Console.Clear();
        }
    }
}
