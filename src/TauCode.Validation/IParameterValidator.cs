﻿using System.Collections.Generic;

namespace TauCode.Validation
{
    public interface IParameterValidator
    {
        IDictionary<string, object> Parameters { get; set; }
    }
}
