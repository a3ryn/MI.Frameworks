/*
This source file is under MIT License (MIT)
Copyright (c) 2020 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;

namespace Shared.Frameworks.Caching
{
    public class CachingConfig
    {
        public CacheExpirationConfig Expiration { get; set; } = new CacheExpirationConfig(); //default

        public TimeSpan ExpirationTimeSpan => CacheExpirationConfig.GetTimeSpan(Expiration);

        public static TimeSpan DefaultExpiration => new TimeSpan(24, 0, 0);
    }

    public class CacheExpirationConfig
    {
        public int Days { get; set; }
        public int Hours { get; set; } = 24;
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        public TimeSpan GetTimeSpan()
            => GetTimeSpan(this);

        public static TimeSpan GetTimeSpan(CacheExpirationConfig e)
            => new TimeSpan(e.Days, e.Hours, e.Minutes, e.Seconds);
    }
}
