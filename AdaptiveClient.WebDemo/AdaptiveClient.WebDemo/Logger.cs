using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaptiveClient.WebDemo
{
    public static class Logger
    {
        private static string _Message;
        public static string Message
        {
            get => _Message;
            set => _Message = value;
        }
    }
}
