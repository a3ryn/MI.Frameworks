/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;

namespace Shared.Core.Common.CustomTypes
{
    public class Reason
    {
        public Exception Exception { get; set; }
        public List<string> Details { get; set; }
        public string Source { get; set; }    
    }

    public class Result
    {
        public bool IsError { get; set; }

        public string Message { get; set; }
        public Reason Reason { get; set; }

    }

    public class Result<T> : Result
    {
        public T Payload { get; set; }
    }
}
