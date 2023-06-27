using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS4_Syscon_Tools
{
    class UpdateProcessEventArgs : EventArgs
    {
        private int value;
        private string message;

        public int Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }

        public UpdateProcessEventArgs(int val, string msg) {
            this.Value = val;
            this.Message = msg;

        }


        
    }
}
