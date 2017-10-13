/*
This source file is under MIT License (MIT)
Copyright (c) 2015 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;

namespace Shared.Core.Common.CustomTypes
{
    public class Fraction : IEquatable<Fraction>
    {
        public Fraction(int numerator)
        {
            Numerator = numerator;
        }

        public Fraction(int numerator, int denominator) 
            : this(numerator)
        {
            Denominator = denominator;
        }

        public static Fraction Create(int numerator, int denominator = 1)
        {
            return new Fraction(numerator, denominator);
        }

        public static Fraction Create(int? numerator)
        {
            if (numerator.HasValue)
                return new Fraction(numerator.Value);
            return null;
        }

        public int Numerator { get; }
        public int Denominator { get; } = 1;

        public bool Equals(Fraction other)
        {
            return 
                other != null &&
                Numerator == other.Numerator && 
                Denominator == other.Denominator;
        }

        public override string ToString()
        {
            return $"{Numerator}/{Denominator}";
        }
    }
}
