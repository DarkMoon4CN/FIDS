using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Model
{
    public class DisplaysInforStates
    {
        
        private int displayID = 0;
        private string ip = "";
        private string displayName = "";
        private int isPrimary = 0;
        private string  connectedTime = "";
        private string Status = "";
        private string exceptionMsg = "";

        public int DisplayID
        {
            get
            {
                return displayID;
            }

            set
            {
                displayID = value;
            }
        }

        public string Ip
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return displayName;
            }

            set
            {
                displayName = value;
            }
        }

        public int IsPrimary
        {
            get
            {
                return isPrimary;
            }

            set
            {
                isPrimary = value;
            }
        }

        public string ConnectedTime
        {
            get
            {
                return connectedTime;
            }

            set
            {
                connectedTime = value;
            }
        }

        public string Status1
        {
            get
            {
                return Status;
            }

            set
            {
                Status = value;
            }
        }

        public string ExceptionMsg
        {
            get
            {
                return exceptionMsg;
            }

            set
            {
                exceptionMsg = value;
            }
        }
    }
}
