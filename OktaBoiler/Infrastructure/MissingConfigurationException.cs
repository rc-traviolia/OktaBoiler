﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OktaBoiler.Infrastructure
{
    public class MissingConfigurationException : Exception
    {
        public MissingConfigurationException(string message) : base(message)
        {
                
        }
    }
}
