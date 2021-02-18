using System.Text.RegularExpressions;

namespace Bank_System_Application
{
    /// <summary>
    /// Template for each interface: Stores the parameters required by each attribute that require user manipultion.
    /// </summary>
    public sealed class BodyParameter
    {
        public string AttributeName { get; set; }
        public string DataType { get; set; }
        public int MaxLength { get; set; }
        public bool Nulleable { get; set; }
        public string RegexValidation { get; set; }
        public string Visualisation { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        private string value = string.Empty;
        public string Value
        {
            get { return value; }
            set
            {
                // Based on regex validation, each value is verified each time in order to preserve the data integrity.
                // Just only one point of user data validation, instead of verification in all the clases.
                // It guarantess that once users press enter, values are parse directly.
                if (RegexValidation.Length > 0)
                {
                    Regex r = new Regex(RegexValidation);
                    if (r.IsMatch(value) == false)
                    {
                        return;                        
                    }
                }
                this.value = string.Empty;
                switch (DataType)
                {
                    case "char":
                    case "string":
                        this.value = value;
                        break;

                    case "int":                        
                        if (int.TryParse(value, out _) == true)
                        {
                            this.value = value;
                        }
                        break;
                    case "ulong":
                        if (ulong.TryParse(value, out _) == true)
                        {
                            this.value = value;
                        }
                        break;
                    case "decimal":                        
                        if (decimal.TryParse(value, out _) == true)
                        {
                            this.value = value;
                        }
                        break;
                    case "double":                        
                        if (double.TryParse(value, out _) == true)
                        {
                            this.value = value;
                        }
                        break;
                    case "float":                        
                        if (float.TryParse(value, out _) == true)
                        {
                            this.value = value;
                        }
                        break;
                    case "DateTime":
                        break;
                    default:                        
                        break;
                }        
                
            }
        }

    }
}
