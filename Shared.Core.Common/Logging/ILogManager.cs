/*
This source file is under MIT License (MIT)
Copyright (c) 2017 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;

namespace Shared.Core.Common.Logging
{
    /*
    This source file is under MIT License (MIT)
    Copyright (c) 2016 Mihaela Iridon
    https://opensource.org/licenses/MIT
    */

    public interface ILogManager
    {
        ILogger GetLogger(string name);
        ILogger GetLogger<T>() where T : class;
        ILogger GetLogger(Type t);
        void Shutdown();
    }
}
