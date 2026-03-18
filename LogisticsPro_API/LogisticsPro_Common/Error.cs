using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsPro_Common.Common
{
    public class Error
    {
        public Error(ErrorType type)
        {
            Type = type;
        }

        public Error(ErrorType type, string message)
        {
            Type = type;
            _message = message;
        }

        public Error(string key, string message)
        {
            Key = key;
            _message = message;
            Type = ErrorType.BAD_REQUEST;
        }

        public ErrorType Type;

        public string Key;

        private string _message;

        public string Message
        {
            get
            {
                switch (this.Type)
                {
                    case ErrorType.INTERNAL:
                        return "Internal Error";

                    default:
                        return _message;
                }
            }
        }
    }
}
