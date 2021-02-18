using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Bank_System_Application
{
    /// <summary>
    /// A view instance has no meaning by itself, it as meaning only when it is used as a base class.
    /// It shares default method implementations across the set of data types that derive from it.
    /// </summary>
    public abstract class View
    {
        // Variables created to handle interfaz dimensions
        private const int viewMargin = 30;  // Space between the left window and the View border
        private const int viewWidth = 70;   // Total view width. From border (╔) to border (╗) 
        private const int viewHeight = 24;  // Total view height. From border to border.
        private const int viewHalfWidth = viewWidth / 2;

        // Variables created to handle the message display
        // All the variables are declared readonly because they are initialise in the constructor.
        private readonly int yPosControlMessage;
        private readonly int yPosBodyMessage;
        private readonly int lengthLongestAtt;
        private readonly int lengthLongestValue;

        // List of elements which create the basic interfaz. Usefull when a interfaz needs to be cleaned.
        private readonly List<string> dom = new List<string>();
        // List of parameters that a user needs to fill in order to upload the information to the object instance
        protected readonly List<BodyParameter> bodyParameterList = new List<BodyParameter>();

        /// <summary>
        /// // Enum to define the type of border to visualise
        /// // Top: a Horizontal upper line is written
        /// // Bottom: A horizontal bottom line is written
        /// // Empty: Just the vertical edges are written.
        /// // Div: A horizontal line is written.
        /// </summary>
        private enum BorderType {Top,Bottom,Empty,Div};

        /// <summary>
        /// Enum to define the type of alignment required to display information.
        /// </summary>
        protected enum AlignmentType { Left, Center, Right};

        /// <summary>
        /// Enum to define the type of message required to display information.
        /// // Inspired in Winforms MessageBox Type.
        /// // Error: Error message. No input is required.
        /// // Null: Basic output. No input is required.
        /// // YesNo: Yes / No. An input is required
        /// // doubleInput: Double. An input is required.
        /// </summary>
        protected enum ControlMessage { Error, Null, YesNo, doubleInput };

        /// <summary>
        /// Constructor in charge to initialise the menu components, and all its variables required.
        /// This approach is inspired in a basic implementation of Model-View-Controller framework.
        /// The view (or user interface) is stored in a separate file with the SAME name, with different extension (.txt)
        /// The process consists on 4 steps:
        /// 1. Read the View File: To initialise the interface layout and its attributes
        /// 2. Load Body attributes: Brigde to connect the interface intormation with object attributes
        /// 3. Draw the content layout: All the layout is stored in dom variable.
        /// </summary>
        public View()
        {                    
            Console.Clear();
            // Finding the view layout through relative path.
            string relativefilePath = string.Format(@"..\..\..\Views\{0}.txt", this.GetType().Name);
            if (!File.Exists(relativefilePath))
            {
                throw new Exception(string.Format(@"View '{0}' layout not found. ", this.GetType().Name));
            }

            // Reading the view template at once
            string[] lines = File.ReadAllLines(relativefilePath);

            // Verifying the content. At least the file must contain 2 rows.
            // Further validations in process
            if (lines.Length < 1)
            {
                throw new Exception(string.Format(@"View '{0}' layout does not contain the appropiate format. ", this.GetType().Name));
            }
            
            //--------------------------
            // -- Reading the content --
            //--------------------------

            //----------------------------------------
            // -- Row 1: Header, Subheader, and footer
            //----------------------------------------
            string[] titlesArray = lines[0].Split("||");
            if (lines.Length < 2)
            {
                throw new Exception(string.Format(@"Header, Subheader, and footer are required in '{0}' layout. It does not contain the appropiate format. ", this.GetType().Name));
            }

            // Getting each pair attribure/value.
            string[] headerArray = titlesArray[0].Split(':');
            string[] subheaderArray = titlesArray[1].Split(':');
            string[] footerArray = titlesArray[2].Split(':');

            // Verifying the dimension of each array              
            string header = headerArray[1];
            string subheader = subheaderArray[1];
            string footer = footerArray[1];

            //----------------------------
            // -- Row 2: Body atributes --
            //----------------------------

            for (int i = 1; i < lines.Length; i++)
            {
                // 
                BodyParameter bodyParameter = new BodyParameter();

                string[] bodyArray = lines[i].Split("||");
                if (bodyArray.Length < 6)
                {
                    throw new Exception(string.Format(@"ASD ASD ASD are required in '{0}' layout. It does not contain the appropiate format. ", this.GetType().Name));
                }

                string[] paramArray;

                paramArray = bodyArray[0].Split(':');
                bodyParameter.AttributeName = paramArray[1];
                
                paramArray = bodyArray[1].Split(':');                
                bodyParameter.DataType = paramArray[1];

                paramArray = bodyArray[2].Split(':');                
                bodyParameter.MaxLength = int.Parse(paramArray[1]);

                paramArray = bodyArray[3].Split(':');
                bodyParameter.Nulleable = bool.Parse(paramArray[1]);

                paramArray = bodyArray[4].Split(':');
                bodyParameter.RegexValidation = paramArray[1];                

                paramArray = bodyArray[5].Split(':');
                bodyParameter.Visualisation = paramArray[1];

                bodyParameter.Value = string.Empty;

                bodyParameterList.Add(bodyParameter);

                // Based on the attribute parameters, the longest value is recorder to facilitate the view drawing.
                if (lengthLongestAtt < bodyParameter.AttributeName.Length)
                {
                    lengthLongestAtt = bodyParameter.AttributeName.Length;
                }

                if (lengthLongestValue < bodyParameter.MaxLength)
                {
                    lengthLongestValue = bodyParameter.MaxLength;
                }

            }

            //--------------------------
            // -- Drawing the content --
            //--------------------------

            // Printing the top border
            Border(BorderType.Top, true);

            // Printing the header
            AlignContent(header, AlignmentType.Center, true);

            // Printing the border between header and subheader (or content when subheader is null)
            Border(BorderType.Div, true);

            if (subheader != "")
            {
                AlignContent(subheader.Split('#'), AlignmentType.Center, true);
            }

            ///////////////////////////////
            // Printing Margins pre-body //
            ///////////////////////////////

            // The Row position is stored to keep a reference in special cases when is required to print information
            // For Example, an account is founded, the user information is displayed in the body
            yPosBodyMessage = dom.Count;

            // Minus 5 because those are the footer's rows.
            int bodyMargnis = (viewHeight - bodyParameterList.Count - dom.Count - 5) / 2;
            Border(BorderType.Empty, bodyMargnis, true);

            // Printing Body
            foreach (BodyParameter param in bodyParameterList)
            {
                // Displaying content aligned to left
                AlignContent(string.Format("{0}{1}", param.AttributeName,
                                    string.Format(":").PadLeft(lengthLongestAtt - param.AttributeName.Length + 1, ' '))
                                    , AlignmentType.Left, true);
                
                // As DOM is a copy of the information in console, the position of character ':' is stored
                // to know exactly where is the x,y location of each attribute.
                param.X = dom[dom.Count - 1].IndexOf(':') + 2;
                param.Y = dom.Count - 1;
                
            }

            // Printing Margins post-body            
            Border(BorderType.Empty, bodyMargnis, true);

            // Printing the border between body and control footer
            Border(BorderType.Div, true);
            // Footer

            // 
            yPosControlMessage = dom.Count;
            Border(BorderType.Empty, true);
            Border(BorderType.Empty, true);
            AlignContent("↑↓: Select Item        Enter: Select         Esc: Quit/Escape", AlignmentType.Center, true);
            Border(BorderType.Bottom, true);

        }

        /// <summary>
        /// Method that creates a single line in order to define the user boundaries
        /// </summary>
        /// <param name="border">Kind of border to display</param>
        /// <param name="addBorderToDom">Indicates if the border needs to be added to the dom object</param>
        /// <returns></returns>
        private string Border(BorderType border, bool addBorderToDom)
        {
            string content = string.Empty;

            switch (border)
            {
                case BorderType.Top:
                    content = string.Format("{0}{1}{2}", 
                                            string.Format(" ").PadRight(viewMargin, ' '),
                                            string.Format("╔").PadRight(viewHalfWidth, '═'),
                                            string.Format("╗").PadLeft(viewHalfWidth, '═'));
                    break;

                case BorderType.Bottom:
                    content = string.Format("{0}{1}{2}", 
                                            string.Format(" ").PadRight(viewMargin, ' '),
                                            string.Format("╚").PadRight(viewHalfWidth, '═'),
                                            string.Format("╝").PadLeft(viewHalfWidth, '═'));
                    break;

                case BorderType.Empty:
                    content = string.Format("{0}{1}{2}", 
                                            string.Format(" ").PadRight(viewMargin, ' '),
                                            string.Format("║").PadRight(viewHalfWidth, ' '),
                                            string.Format("║").PadLeft(viewHalfWidth, ' '));
                    break;
                case BorderType.Div:
                    content = string.Format("{0}{1}{2}", 
                                            string.Format(" ").PadRight(viewMargin, ' '),
                                            string.Format("╠").PadRight(viewHalfWidth, '═'),
                                            string.Format("╣").PadLeft(viewHalfWidth, '═'));
                    break;
                default:
                    break;
            }

            if (addBorderToDom == true)
            {
                dom.Add(content);
            }

            return content;

        }


        /// <summary>
        /// Method that creates a single line in order to define the user boundaries
        /// </summary>
        /// <param name="border">Kind of border to display</param>
        /// <param name="count">Amount of borders to display</param>
        /// <param name="addBorderToDom">Indicates if the border needs to be added to the dom object</param>
        private string[] Border(BorderType border, int count, bool addBorderToDom)
        {
            string[] content = new string[count];
            
            for (int i = 0; i < count; i++)
            {
                content[i] = Border(border, addBorderToDom);
            }

            return content;
        }

        /// <summary>
        /// Method who sets where is the place to write the user information
        /// </summary>
        /// <param name="value">Information to display in console</param>
        /// <param name="alignment">Position to display the information</param>
        /// <param name="addContentToDom">Indicates if the content needs to be added to the dom object</param>
        /// <returns></returns>
        protected string AlignContent(string value, AlignmentType alignment, bool addContentToDom)
        {

            string content = string.Empty;
            switch (alignment)
            {
                case AlignmentType.Left:

                    if (viewWidth - value.Length - 5 > 0)
                    {
                        content = string.Format("{0}{1}{2}{3}",
                                                string.Format(" ").PadRight(viewMargin, ' '),
                                                string.Format("║").PadRight(5, ' '),
                                                value,
                                                string.Format("║").PadLeft(viewWidth - value.Length - 5, ' '));
                    }
                    else
                    {
                        content = string.Format("{0}{1}{2}{3}",
                                                string.Format(" ").PadRight(viewMargin, ' '),
                                                string.Format("║").PadRight(5, ' '),
                                                value.Substring(0, viewWidth - 6),
                                                string.Format("║"));
                    }

                    break;
                case AlignmentType.Center:
                    int titleOffset = value.Length % 2 == 0 ? 0 : 1;

                    if (viewHalfWidth - (value.Length / 2) > 0)
                    {
                        content = string.Format("{0}{1}{2}{3}",
                                                string.Format(" ").PadRight(viewMargin, ' '),
                                                string.Format("║").PadRight(viewHalfWidth - (value.Length / 2), ' '),
                                                value,
                                                string.Format("║").PadLeft(viewHalfWidth - (value.Length / 2) - titleOffset, ' '));
                    }
                    else
                    {
                        // Alignment out of boundaries. To left by default
                        AlignContent(value, AlignmentType.Left, addContentToDom);
                    }

                    break;
                case AlignmentType.Right:
                    throw new Exception("Alignment not created!");
                    //break;
            }

            if (addContentToDom == true)
            {
                dom.Add(content);
            }
            return content;
        }
        /// <summary>
        /// Method who sets where is the place to write the user information
        /// </summary>
        /// <param name="values">Array of information to display in console</param>
        /// <param name="alignment">Position to display the information</param>
        /// <param name="addContentToDom">Indicates if the content needs to be added to the dom object</param>
        /// <returns></returns>
        protected string[] AlignContent(string[] values, AlignmentType alignment, bool addContentToDom)
        {
            string[] result = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = AlignContent(values[i], alignment, addContentToDom);
            }

            return result;
        }

        /// <summary>
        /// CORE METHOD: Method in charge to handle the user inputs. Its purpose is to validate whether
        /// the user input is a control key such us: Up, Down, enter, Esc or a meaningful character.
        /// All the interfaces share almost the same approach: A user fills the form. Then, the information is
        /// validated, and if everything is correct, all the parameters are updated to the object. Otherwise,
        /// an error message is displayed.
        /// </summary>
        /// <returns></returns>
        protected bool VerifylInputs()
        {            
            
            ConsoleKeyInfo usrInput;
            int currentParameterPosition = 0;

            // Setting the Console cursor in the first body parameter location.
            BodyParameter currentParameter = this.bodyParameterList[currentParameterPosition];
            Console.SetCursorPosition(currentParameter.X + currentParameter.Value.Length, currentParameter.Y);

            do
            {                
                usrInput = Console.ReadKey(true);

                // Verifying if the user input is an special content or just information
                if ((int)usrInput.KeyChar < 32)
                {
                    // SPECIAL CONTROL CHARACTERS

                    // ENTER: All the information must be validated
                    if (usrInput.Key == ConsoleKey.Enter)
                    {
                        // Veryfing if the body parametes could be nullable
                        if (this.VerifyBodyParameters() == true)
                        {
                            return true;
                        }
                        else
                        {
                            // Returning the cursor to the same position, before the user press enter
                            Console.SetCursorPosition(currentParameter.X + currentParameter.Value.Length, currentParameter.Y);
                        }
                    }
                    else
                    {
                        // UP
                        if (usrInput.Key == ConsoleKey.UpArrow && currentParameterPosition - 1 >= 0)
                        {                            
                            currentParameterPosition -= 1;
                            currentParameter = this.bodyParameterList[currentParameterPosition];                            
                        }
                        // DOWN
                        else if ((usrInput.Key == ConsoleKey.DownArrow || usrInput.Key == ConsoleKey.Tab) && currentParameterPosition + 1 < this.bodyParameterList.Count)
                        {                            
                            currentParameterPosition += 1;
                            currentParameter = this.bodyParameterList[currentParameterPosition];                            
                            
                        }
                        // BACKSPACE
                        else if (usrInput.Key == ConsoleKey.Backspace && currentParameter.Value.Length > 0)
                        {
                            currentParameter.Value = currentParameter.Value.Substring(0, currentParameter.Value.Length - 1);
                        }
                        
                        Console.SetCursorPosition(currentParameter.X, currentParameter.Y);                       
                        Console.Write(string.Format(" ").PadLeft(viewMargin + viewWidth - currentParameter.X - 1, ' '));
                        Console.SetCursorPosition(currentParameter.X, currentParameter.Y);

                        if (currentParameter.Visualisation == "Plain")
                        {
                            Console.Write(currentParameter.Value);
                        }
                        else if (currentParameter.Value.Length > 0)
                        {
                            Console.Write(string.Format("*").PadLeft(currentParameter.Value.Length, '*'));
                        }
                    }
                }
                // 
                else if(currentParameter.Value.Length < currentParameter.MaxLength)
                {
                    // SIMPLE CHARACTERS

                    currentParameter.Value += usrInput.KeyChar.ToString();

                    Console.SetCursorPosition(currentParameter.X, currentParameter.Y);
                    Console.Write(string.Format(" ").PadLeft(viewMargin + viewWidth - currentParameter.X - 1, ' '));
                    Console.SetCursorPosition(currentParameter.X, currentParameter.Y);

                    if (currentParameter.Visualisation == "Plain")
                    {
                        Console.Write(currentParameter.Value);
                    }
                    else if (currentParameter.Value.Length > 0)
                    {
                        Console.Write(string.Format("*").PadLeft(currentParameter.Value.Length, '*'));
                    }
                }


                Console.SetCursorPosition(currentParameter.X, currentParameter.Y);

                if (currentParameter.Visualisation == "Plain")
                {
                    Console.Write(currentParameter.Value);
                }
                else if (currentParameter.Value.Length > 0)
                {
                    Console.Write(string.Format("*").PadLeft(currentParameter.Value.Length, '*'));
                }

                // Verifying until user press ESC
            } while (usrInput.Key != ConsoleKey.Escape);
            return false;
        }

        /// <summary>
        /// Method that cleans all the body parameters stored previously
        /// </summary>
        protected void DumpBodyParameters()
        {
            foreach (BodyParameter param in this.bodyParameterList)
            {
                param.Value = string.Empty;
            }
        }

        /// <summary>
        /// Method that verifies if a value could be null or not. Once a value is null, a message is displayed
        /// to indicate to the user where an action is required.
        /// </summary>
        /// <returns></returns>
        private bool VerifyBodyParameters()
        {
            bool result = true;

            foreach (BodyParameter param in this.bodyParameterList)
            {
                if (param.Nulleable == false && param.Value.Length == 0)
                {
                    Console.SetCursorPosition(param.X + lengthLongestValue + 2, param.Y);
                    Console.Write("Empty value!");
                    result = false;
                }
            }

            return result;

        }

        /// <summary>
        /// Method that displays all the interface layout
        /// </summary>
        public void Show()
        {
            Console.Clear();
            foreach (string str in dom)
            {
                Console.WriteLine(str);
            }
        }

        /// <summary>
        /// Display a message in the body section. (After first div section)
        /// </summary>
        /// <param name="message">Array of information to display in one line</param>
        protected void MessageBody(string message)
        {
            Console.SetCursorPosition(0, yPosBodyMessage);
            Console.Write(AlignContent(message, AlignmentType.Left, false));
        }

        /// <summary>
        /// Display a message in the body section. (After first div section)
        /// </summary>
        /// <param name="message">Array of information to display line by line</param>
        protected void MessageBody(string[] message)
        {
            Console.SetCursorPosition(0, yPosBodyMessage);
            foreach (var msg in message)
            {
                Console.WriteLine(AlignContent(msg, AlignmentType.Left, false));
            }            
        }

        /// <summary>
        /// Rudimentary implementation of windows forms message box. 
        /// Its purpose is to display a message and wait for input (if it is required).
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="ctrl">Message box configuration: Null, Error, YesNo</param>
        /// <param name="alignment">Message alignment</param>
        /// <returns></returns>
        protected string MessageBox(string message, ControlMessage ctrl, AlignmentType alignment)
        {            
            
            ConsoleKeyInfo usrInput;
            string strInput = string.Empty;
            string strMsgOutput;
            string err = string.Empty;

            //Console.ForegroundColor = ConsoleColor.Yellow;

            switch (ctrl)
            {
                case ControlMessage.Null:

                    // Control messages are displaying in the same line
                    Console.SetCursorPosition(0, yPosControlMessage);
                    Console.Write(AlignContent(message, alignment, false));
                    break;

                case ControlMessage.Error:

                    // Error messages are displaying one line after
                    Console.SetCursorPosition(0, yPosControlMessage + 1);
                    Console.Write(AlignContent(message, alignment, false));
                    break;

                case ControlMessage.YesNo:

                    // Control messages are displaying in the same line                        
                    strMsgOutput = AlignContent(message, alignment, false);
                    do
                    {
                        Console.SetCursorPosition(0, yPosControlMessage);
                        Console.Write(strMsgOutput);
                        Console.SetCursorPosition(strMsgOutput.IndexOf(message) + message.Length + 2, yPosControlMessage);
                        usrInput = Console.ReadKey(false);
                        strInput = usrInput.KeyChar.ToString().ToUpper();
                        
                    } while (strInput != "Y" && strInput != "N");
                    break;

                case ControlMessage.doubleInput:

                    strMsgOutput = AlignContent(message, alignment, false);

                    while (true)
                    {
                        // Printing an error when the client wrote wrong numbers
                        if (err != "")
                        {                                                    
                            Console.SetCursorPosition(0, yPosControlMessage + 1);
                            Console.Write(AlignContent(err, alignment, false));
                        }                       

                        // Displaying the same message (if the client wrote wrong numbers)
                        Console.SetCursorPosition(0, yPosControlMessage);
                        Console.Write(strMsgOutput);
                        Console.SetCursorPosition(strMsgOutput.IndexOf(message) + message.Length + 2, yPosControlMessage);
                        strInput = Console.ReadLine();

                        // Deleting the error messages, simply overriding with spaces
                        Console.SetCursorPosition(0, yPosControlMessage + 1);                        
                        Console.Write(Border(BorderType.Empty, false));

                        if (double.TryParse(strInput, out double _) == true)
                        {
                            break;
                        }
                        else
                        {
                            err = "Invalid argument!";
                        }
                    }                    
                    break;
                default:
                    break;
            }

            return strInput;

        }

        /// <summary>
        /// Specialised validation that must be implemented in each of the inherited classes.
        /// This method will implement further validations, actions, and another calls in regards to the class
        /// </summary>
        public abstract void Controller();

        /// <summary>
        /// This project is inspired in the MVC approach. For that reason, in one point the view information
        /// must be updated in the object. This method performs this activity, connecting all the body parameters
        /// manually with attribute instance names.
        /// </summary>
        public abstract void InitialiseInstanceParameters();


    }
}
