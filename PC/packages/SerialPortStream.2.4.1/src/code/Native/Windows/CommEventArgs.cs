﻿// Copyright © Jason Curl 2012-2023
// Sources at https://github.com/jcurl/SerialPortStream
// Licensed under the Microsoft Public License (Ms-PL)

namespace RJCP.IO.Ports.Native.Windows
{
    using System;

    internal class CommEventArgs : EventArgs
    {
        private readonly Kernel32.SerialEventMask m_EventType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventType">The event results.</param>
        public CommEventArgs(Kernel32.SerialEventMask eventType)
        {
            m_EventType = eventType;
        }

        /// <summary>
        /// The event bit field.
        /// </summary>
        public Kernel32.SerialEventMask EventType
        {
            get { return m_EventType; }
        }
    }
}
