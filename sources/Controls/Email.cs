using System.ComponentModel;

namespace EmailClientApplication.Controls
{
    public class Email: IDataErrorInfo
    {
        public string EmailAddress { get; set; }

        public string this[string columnName]
        {
            get
            {
                return null; //"Error"; }
            }        
        }

        public string Error { get; private set; }
    }
}