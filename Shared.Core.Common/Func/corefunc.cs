/*
This source file is under MIT License (MIT)
Copyright (c) 2016 Chris A. Moore
https://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Shared.Core.Common
{
    /// <summary>
    /// Common functions intended to reduce the syntactic noise endemic to C#
    /// </summary>
    public static class corefunc
    {
        private static readonly HashSet<string> TrueValues = new HashSet<string>(new[] { "true", "t", "1", "y", "+" });
        private static readonly HashSet<string> FalseValues = new HashSet<string>(new[] { "false", "f", "0", "n", "-" });

        /// <summary>
        /// Helper to parse a boolean value in a more reasonable fashion than the intrinsic <see cref="bool.Parse(string)"/> method
        /// </summary>
        /// <param name="s">The text to parse</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        private static bool ParseBool(string s)
        {
            var key = s.ToLower();
            if (TrueValues.Contains(key))
                return true;
            else if (FalseValues.Contains(key))
                return false;
            else
                throw new ArgumentException($"Could not interpret {s} as a boolean value");
        }


        /// <summary>
        /// Indexes a few primitive parsers
        /// </summary>
        private static IDictionary<Type, Func<string, object>> parsers = new Dictionary<Type, Func<string, object>>
        {
            //[typeof(Date)] = s => Date.Parse(s),
            [typeof(DateTime)] = s => DateTime.Parse(s),
            [typeof(Guid)] = s => Guid.Parse(s),
            [typeof(decimal)] = s => decimal.Parse(s, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign),
            [typeof(TimeSpan)] = s => TimeSpan.Parse(s),
            [typeof(bool)] = s => ParseBool(s),
            [typeof(int)] = s => int.Parse(s),
            [typeof(short)] = s => short.Parse(s),
            [typeof(long)] = s => long.Parse(s)
        };

        /// <summary>
        /// Parses the supplied value
        /// </summary>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="s">The value to parse</param>
        /// <returns></returns>
        /// <remarks>
        /// Obviously, this is rather limited...
        /// </remarks>
        public static T parse<T>(string s) => (T)parsers[typeof(T)](s);

        /// <summary>
        /// Parses an enumeration
        /// </summary>
        /// <typeparam name="T">The enumeration type</typeparam>
        /// <param name="value">The value of the enumeration</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T enum_parse<T>(string value)  => (T)Enum.Parse(typeof(T), value, true);

        /// <summary>
        /// Forms a union from the supplied elements
        /// </summary>
        /// <typeparam name="T">The sequence element type</typeparam>
        /// <param name="a">The first sequence</param>
        /// <param name="b">The second sequence</param>
        /// <param name="c">The third sequence</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> union<T>(IEnumerable<T> a, IEnumerable<T> b, IEnumerable<T> c) =>
            a.Union(b).Union(c);

        /// <summary>
        /// Forms a union from the supplied elements
        /// </summary>
        /// <typeparam name="T">The sequence element type</typeparam>
        /// <param name="a">The first sequence</param>
        /// <param name="b">The second sequence</param>
        /// <param name="c">The third sequence</param>
        /// <param name="d">The fourth sequence</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> union<T>(IEnumerable<T> a, IEnumerable<T> b, IEnumerable<T> c, IEnumerable<T> d) =>
            a.Union(b).Union(c).Union(d);

        /// <summary>
        /// Forms a union from the supplied elements
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="x">An input sequence</param>
        /// <param name="y">An arbitrary number of elements</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> union<T>(IEnumerable<T> x, params T[] y) => x.Union(y);

        /// <summary>
        /// Forms a union from the supplied elements
        /// </summary>
        /// <typeparam name="T">The sequence element type</typeparam>
        /// <param name="x">The first sequence</param>
        /// <param name="y">The second sequence</param>
        /// <param name="z">An arbitrary number of elements</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> union<T>(IEnumerable<T> x, IEnumerable<T> y, params T[] z) =>
            union(x.Union(y), z);

        /// <summary>
        /// Casts the sequence items to the specified type
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="items">The sequence of items to cast</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> cast<T>(IEnumerable items) => items.Cast<T>();


        /// <summary>
        /// Negates the input value
        /// </summary>
        /// <param name="value">The value of the input</param>
        /// <returns></returns>
        /// <remarks>
        /// This function is defined to as to remove ambiguity when needed between the C# intrinsic (!)
        /// operator and the optional value (!) operator that yields an encapsulated value from
        /// within an optional value
        /// </remarks>
        [DebuggerStepThrough]
        public static bool not(bool value) => !value;

        /// <summary>
        /// Creates a complied regular expression from the supplied pattern
        /// </summary>
        /// <param name="pattern">The pattern to be interpreted as a regular expression/></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Regex regex(string pattern) => new Regex(pattern, RegexOptions.Compiled);

        //MI added
        public static IEnumerable<S> Map<T, S>(this IEnumerable<T> data, Func<T, S> f) => data.Select(f);  //or Apply(f)

        //public static IEnumerable<S> Mapi<T, S>(this IEnumerable<T> data, Func<int, T, S> f) => data.ApplyI(i, f);


        /// <summary>
        /// I wish didn't have to declare a class to do this..
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        class PrintedList<T> : List<T>
        {
            public PrintedList(IEnumerable<T> items)
                : base(items)
            {

            }
            public override string ToString()
            {
                var text = concat(this.Take(3).Map(x => x.ToString()), "; ");
                return this.Count > 3 ? text + "..." : text;
            }
        }

        /// <summary>
        /// Yields a <see cref="IReadOnlyList{T}"/> realization instance from an input sequence
        /// </summary>
        /// <typeparam name="T">The input item type</typeparam>
        /// <param name="items">The input items</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IReadOnlyList<T> list<T>(IEnumerable<T> items) => 
            new PrintedList<T>((items ?? new T[] { }));

        /// <summary>
        /// Yields a <see cref="IReadOnlyList{T}"/> realization instance from provided input sequences
        /// </summary>
        /// <typeparam name="T">The input item type</typeparam>
        /// <param name="items">The input items</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IReadOnlyList<T> list<T>(IEnumerable<T> items, params T[] more) =>
            new PrintedList<T>((union((items ?? new T[] { }),more)));

        /// <summary>
        /// Yields a <see cref="IReadOnlyList{T}"/> instance from an input sequence
        /// </summary>
        /// <typeparam name="T">The input item type</typeparam>
        /// <param name="parms">The input items</param>
        /// <returns></returns>
        public static IReadOnlyList<T> list<T>(params T[] parms) =>
            new PrintedList<T>(parms);

        /// <summary>
        /// Creates a <see cref="Tuple{T1, T2}"/> instance
        /// </summary>
        /// <typeparam name="T1">The type of the first item</typeparam>
        /// <typeparam name="T2">The type of the second item</typeparam>
        /// <param name="x1">The first item</param>
        /// <param name="x2">The second item</param>
        /// <returns></returns>
        public static Tuple<T1, T2> tuple<T1, T2>(T1 x1, T2 x2) => Tuple.Create(x1,x2);

        /// <summary>
        /// Creates tuple with item of type object in the second position
        /// </summary>
        /// <typeparam name="T">The type of the first item</typeparam>
        /// <param name="x1">The first item</param>
        /// <param name="x2">The second item</param>
        /// <returns></returns>
        public static Tuple<T, object> tupleo<T>(T x1, object x2) => Tuple.Create(x1, x2);

        /// <summary>
        /// Transforms a dictionary into a set of tuples
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="pairs">Transforms a KVP sequence into a sequence of tuples</param>
        /// <returns></returns>
        public static IEnumerable<Tuple<TKey, TValue>> tuples<TKey, TValue>(IEnumerable<KeyValuePair<TKey,TValue>> pairs)
        {
            foreach(var pair in pairs)
            {
                yield return tuple(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Transforms a generic dictionary to a sequence of KVP
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="d">The dictionary to transform</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> kvp<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> d) =>
            d;

        /// <summary>
        /// Applies a function to an input sequence to yield a transformed output sequence
        /// </summary>
        /// <typeparam name="TSource">The type of input element</typeparam>
        /// <typeparam name="TResult">The type of output element</typeparam>
        /// <param name="src">The source sequence</param>
        /// <param name="f">The mapping function</param>
        internal static IReadOnlyList<TResult> Apply<TSource, TResult>(this IEnumerable<TSource> src, Func<TSource, TResult> f) =>
            list(src.Select(item => f(item)));

        /// <summary>
        /// Applies a function to an input sequence to yield a transformed output sequence
        /// </summary>
        /// <typeparam name="TSource">The type of input element</typeparam>
        /// <typeparam name="TResult">The type of output element</typeparam>
        /// <param name="src">The source sequence</param>
        /// <param name="f">The mapping function</param>
        /// <param name="max">The maximum number of elements from the sequence to map</param>      
        internal static IReadOnlyList<TResult> ApplyI<TSource, TResult>(this IEnumerable<TSource> src, int max, Func<int, TSource, TResult> f)
        {
            var dstList = new List<TResult>();
            var srcList = src.ToList();
            for (int i = 0; i < max; i++)
                dstList.Add(f(i, srcList[i]));
            return dstList;
        }

        /// <summary>
        /// Applies the supplied function to each element in the input sequence
        /// </summary>
        /// <typeparam name="T">The input sequence item type</typeparam>
        /// <typeparam name="S">The output sequence item type</typeparam>
        /// <param name="seq">The sequence to transform</param>
        /// <param name="f">The transformation function</param>
        /// <returns></returns>
        public static IReadOnlyList<S> map<T, S>(IEnumerable<T> seq, Func<T, S> f) => 
            seq.Apply(f);

        /// <summary>
        /// Applies the supplied function to each element in the input sequence
        /// </summary>
        /// <typeparam name="T">The input sequence item type</typeparam>
        /// <typeparam name="S">The output sequence item type</typeparam>
        /// <param name="seq">The sequence to transform</param>
        /// <param name="f">The transformation function</param>
        /// <returns></returns>
        //public static IReadOnlyList<S> mapi<T, S>(IEnumerable<T> seq, Func<int, T, S> f) => seq.Mapi(f);

        /// <summary>
        /// Applies the supplied function to each element in the input sequence up to a specified maximum
        /// number of sequence elements
        /// </summary>
        /// <typeparam name="T">The input sequence item type</typeparam>
        /// <typeparam name="S">The output sequence item type</typeparam>
        /// <param name="seq">The sequence to transform</param>
        /// <param name="f">The transformation function</param>
        /// <returns></returns>
        //public static IReadOnlyList<S> mapi<T, S>(IEnumerable<T> seq, int max, Func<int, T, S> f) =>
            //seq.Mapi(max, f);


        /// <summary>
        /// Associates a type with an action parameterized by that type
        /// </summary>
        /// <typeparam name="T">The type with which to associate the action</typeparam>
        /// <param name="action">The action</param>
        /// <returns></returns>
        public static Tuple<Type, Action<object>> map<T>(Action<T> action) =>  
            tuple<Type,Action<object>>(typeof(T), x => action((T)x));

        /// <summary>
        /// Creates a <see cref="KeyValuePair{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="T1">The type of the key</typeparam>
        /// <typeparam name="T2">The type of the value</typeparam>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public static KeyValuePair<T1, T2> kvp<T1, T2>(T1 key, T2 value) => 
            new KeyValuePair<T1, T2>(key, value);

        /// <summary>
        /// Converts any instance to an object
        /// </summary>
        /// <param name="o">The object to convert</param>
        /// <returns></returns>
        /// <remarks>
        /// This may initially appear useless; however it is used a shortcut whenever 
        /// the actual <see cref="object"/> type is required, e.g., when constructing a list 
        /// of objects from heterogeneous instances. Otherwise, you would have to write
        /// (object)x and increase the noise by a factor of 3.
        /// </remarks>
        public static object box(object o) => o;

        /// <summary>
        /// Represents an input sequence as a <see cref="IReadOnlyDictionary{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="items">The sequence to represent as a dictionary</param>
        /// <returns></returns>
        public static IReadOnlyDictionary<TKey, TValue> dict<TKey, TValue>(IEnumerable<Tuple<TKey, TValue>> items) =>
            items.ToDictionary(x => x.Item1, x => x.Item2);

        /// <summary>
        /// Represents an input sequence as a <see cref="IReadOnlyDictionary{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="items">The sequence to represent as a dictionary</param>
        /// <returns></returns>
        public static IReadOnlyDictionary<TKey, TValue> dict<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items) =>
            items.ToDictionary(x => x.Key, x => x.Value);


        /// <summary>
        /// Represents an input sequence as a <see cref="IReadOnlyDictionary{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<TKey, TValue> dict<TKey, TValue>(params Tuple<TKey, TValue>[] items) =>
            items.ToDictionary(x => x.Item1, x => x.Item2);

        /// <summary>
        /// Represents an input sequence as a <see cref="IReadOnlyDictionary{TKey, TValue}"/>
        /// </summary>
        /// <typeparam name="TSource">The input sequence item type</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="source">The input sequence</param>
        /// <param name="keySelector">The key selector</param>
        /// <param name="valueSelector">The value selector</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Dictionary<TKey, TValue> dict<TSource, TKey, TValue>(
            IEnumerable<TSource> source, 
            Func<TSource, TKey> keySelector, 
            Func<TSource, TValue> valueSelector) => source.ToDictionary(keySelector, valueSelector);

        /// <summary>
        /// Shorthand for the <see cref="string.IsNullOrEmpty(string)"/> method
        /// </summary>
        /// <param name="subject">The string to evaluate</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool isBlank(string subject) => String.IsNullOrWhiteSpace(subject);

        /// <summary>
        /// Shorthand for the negation of the <see cref="string.IsNullOrEmpty(string)"/> method
        /// </summary>
        /// <param name="subject">The string to evaluate</param>
        /// <returns></returns>
        public static bool isNotBlank(string subject) => not(isBlank(subject));

        /// <summary>
        /// A string-specific coalescing operation
        /// </summary>
        /// <param name="subject">The subject string</param>
        /// <param name="replace">The replacement value if blank</param>
        /// <returns></returns>
        public static string ifBlank(string subject, string replace) => isBlank(subject) ? replace : subject;


        /// <summary>
        /// If subject is not null, invokes its ToString() method; otherwise, returns an empty string
        /// </summary>
        /// <typeparam name="T">The subject type</typeparam>
        /// <param name="subject">The subject</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string toString<T>(T subject) => subject != null ? subject.ToString() : String.Empty;

        /// <summary>
        /// Concatenates a sequence of strings, separating adjacent items with a comma
        /// </summary>
        /// <param name="items">The items to concatenate</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string concat(IEnumerable<string> items) => string.Join(",", items);
            //TextUtilities.Concat(items, ",");

        /// <summary>
        /// Does what you would expect when supplying a sequence of characters to a 
        /// concatenation function (!)
        /// </summary>
        /// <param name="chars">The characters to concatenate</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string concat(this IEnumerable<char> chars) => 
            new string(chars.ToArray());

        /// <summary>
        /// Does what you would expect when supplying a sequence of characters to a 
        /// concatenation function (!)
        /// </summary>
        /// <param name="chars">The characters to concatenate</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string concat(this char[] chars) => new string(chars);

        /// <summary>
        /// Concatenates a sequence of strings, separating adjacent items with supplied separator
        /// </summary>
        /// <param name="items">The items to concatenate</param>
        /// <param name="separator">The separator</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string concat(this IEnumerable<string> items, string separator) => string.Join(separator, items);
            //TextUtilities.Concat(items, separator);


        /// <summary>
        /// Appends the tail to the head
        /// </summary>
        /// <param name="head">The first part of the string</param>
        /// <param name="tail">The last part of the string</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string append(string head, string tail) => head + tail;

        /// <summary>
        /// Appends the tail to the head, separating the two by a new line
        /// </summary>
        /// <param name="head">The first part of the string</param>
        /// <param name="tail">The last part of the string</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string appendl(string head, string tail) => head + System.Environment.NewLine + tail;


        /// <summary>
        /// Concatenates an arbitrary number of sequences
        /// </summary>
        /// <typeparam name="T">The sequence item type</typeparam>
        /// <param name="sequences">The sequence to concatenate</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> concat<T>(params IEnumerable<T>[] sequences) => sequences.SelectMany(x => x);


        /// <summary>
        /// Gets the current system time
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static DateTime now() => DateTime.Now;

        /// <summary>
        /// Converts an input value to a value of a specified type
        /// </summary>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="value">The source Value</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T convert<T>(object value) => (T) Convert.ChangeType(value, typeof(T));


        /// <summary>
        /// Determines whether a supplied object is null
        /// </summary>
        /// <param name="o">The object to examine</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool isNull(object o) => o == null;

        /// <summary>
        /// Determines whether a supplied object is not null
        /// </summary>
        /// <param name="o">The object to examine</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool isNotNull(object o) => o != null;

        /// <summary>
        /// Creates a <see cref="lazy{T}(Func{T})"/>.
        /// </summary>
        /// <param name="factory">A function that creates an instance of the lazy object</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Lazy<T> lazy<T>(Func<T> factory) => new Lazy<T>(factory);

        /// <summary>
        /// Raises a <see cref="FileNotFoundException"/> if the specified file does not exist
        /// </summary>
        /// <param name="path">The path to the file</param>
        public static void require_file(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("A required file was not found", path);
        }

        /// <summary>
        /// Invokes the first function if the supplied object is not null and the second if it is
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <typeparam name="TResult">The function result type</typeparam>
        /// <param name="o">The object to test</param>
        /// <param name="f1">The non-null evaluator</param>
        /// <param name="f2">The null evaluator</param>
        /// <returns></returns>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult ifNotNull<T, TResult>(T o, Func<T, TResult> f1, Func<TResult> f2)
            where T : class => isNotNull(o) ? f1(o) : f2();


        /// <summary>
        /// Invokes the supplied function if the file exists; otherwise, raises a <see cref="FileNotFoundException"/> exception
        /// </summary>
        /// <typeparam name="T">The return type of the function</typeparam>
        /// <param name="path">The path to the file</param>
        /// <param name="ifPresent">The function to invoke if the file exists</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T require_file<T>(string path, Func<string,T> ifPresent)
        {
            require_file(path);
            return ifPresent(path);
        }

        /// <summary>
        /// Returns the minimum value between the supplied arguments
        /// </summary>
        /// <param name="x">The first argument</param>
        /// <param name="y">The second argument</param>
        /// <returns></returns>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int min(int x, int y) => Math.Min(x, y);

        /// <summary>
        /// Returns the maximum value between the supplied arguments
        /// </summary>
        /// <param name="x">The first argument</param>
        /// <param name="y">The second argument</param>
        /// <returns></returns>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int max(int x, int y) => Math.Max(x, y);


        /// <summary>
        /// Retrieves the identified <see cref="PropertyInfo"/>
        /// </summary>
        /// <param name="o">The object on which the property is defined</param>
        /// <param name="propname"></param>
        /// <returns></returns>
        private static PropertyInfo GetProperty(this object o, string propname) =>
            o.GetType().GetProperty(propname,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Retrieves the identified <see cref="MethodInfo"/>
        /// </summary>
        /// <param name="o">The object on which the method is defined</param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static MethodInfo GetMethod(this object o, string name) =>
            o.GetType().GetMethod(name, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Retrieves an object's public properties
        /// </summary>
        /// <param name="o">The object</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<PropertyInfo> props(object o) =>
            o == null ? new PropertyInfo[] { } : o.GetType().GetProperties();


        
        /// <summary>
        /// Gets the public properties defined on the supplied type
        /// </summary>
        /// <typeparam name="T">The type to examine</typeparam>
        /// <returns></returns>
        public static IReadOnlyList<PropertyInfo> props<T>() =>
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        /// <summary>
        /// Gets the value of the identified property
        /// </summary>
        /// <param name="o">The object on which the property is defined</param>
        /// <param name="propname">The name of the property</param>
        /// <returns></returns>
        public static object propval(object o, string propname) =>
            o.GetProperty(propname)?.GetValue(o);

        /// <summary>
        /// Gets the value of the identified property
        /// </summary>
        /// <typeparam name="T">The property value type</typeparam>
        /// <param name="o">The object on which the property is defined</param>
        /// <param name="propname">The name of the property</param>
        /// <returns></returns>
        public static T propval<T>(object o, string propname) =>
            (T)propval(o, propname);

        /// <summary>
        /// Sets the identified property on the object to the supplied value
        /// </summary>
        /// <param name="o">The object whose property will be set</param>
        /// <param name="propname">The property that will be set</param>
        /// <param name="value">The value of the property</param>
        public static void propval(object o, string propname, object value) => 
            o.GetProperty(propname)?.SetValue(o, value);


        /// <summary>
        /// Gets the CLR runtime type of the identified property
        /// </summary>
        /// <param name="o">An instance of the type on which the property is defined</param>
        /// <param name="propname">The name of the property</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Type proptype(object o, string propname) =>
            o.GetType().GetProperty(propname).PropertyType;

        /// <summary>
        /// Casts the object to the specified type
        /// </summary>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="o">The source object</param>
        /// <returns></returns>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T cast<T>(object o) => (T)o;

        /// <summary>
        /// Dynamically invokes a named method on an object
        /// </summary>
        /// <param name="o">The object that defines the method</param>
        /// <param name="methodName">The method to invoke</param>
        /// <param name="parms">The parameters to pass to the method</param>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void invoke(object o, string methodName, params object[] parms) =>
            o.GetMethod(methodName).Invoke(o, parms);

        /// <summary>
        /// Creates a <see cref="HashSet{T}"/> from the supplied sequence
        /// </summary>
        /// <typeparam name="T">The sequence element type</typeparam>
        /// <param name="items">The sequence</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static HashSet<T> set<T>(IEnumerable<T> items) => new HashSet<T>(items);

        /// <summary>
        /// Conditionally invokes an action if the subject is of the required type
        /// </summary>
        /// <typeparam name="T">The potential subject type</typeparam>
        /// <param name="subject">The subject</param>
        /// <param name="action">The action to conditionally invoke</param>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void match_type<T>(object subject, Action<T> action)
        {
            if (subject is T)
                action((T)subject);
        }

        /// <summary>
        /// Conditionally invokes an action based on the subject type
        /// </summary>
        /// <typeparam name="T1">A potential subject type</typeparam>
        /// <typeparam name="T2">A potential subject type</typeparam>
        /// <param name="o">The subject</param>
        /// <param name="action1">An action to conditionally invoke</param>
        /// <param name="action2">An action to conditionally invoke</param>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void match_type<T1, T2>(object o, Action<T1> action1, Action<T2> action2)
        {
            if (o is T1)
                action1((T1)o);
            else if (o is T2)
                action2((T2)o);
        }

        /// <summary>
        /// Conditionally invokes an action based on the subject type
        /// </summary>
        /// <typeparam name="T1">A potential subject type</typeparam>
        /// <typeparam name="T2">A potential subject type</typeparam>
        /// <typeparam name="T3">A potential subject type</typeparam>
        /// <param name="o">The subject</param>
        /// <param name="action1">An action to conditionally invoke</param>
        /// <param name="action2">An action to conditionally invoke</param>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void match_type<T1, T2,T3>(object o, Action<T1> action1, Action<T2> action2, Action<T3> action3)
        {
            if (o is T1)
                action1((T1)o);
            else if (o is T2)
                action2((T2)o);
            else if (o is T3)
                action3((T3)o);
        }


        /// <summary>
        /// Iterates over items sequentially or in parallel and invokes the supplied action for each one
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="items">The items</param>
        /// <param name="action">The action to apply</param>
        /// <param name="pll">Whether parallel iteration should be invoked</param>
        [DebuggerStepThrough]
        public static void iter<T>(IEnumerable<T> items, Action<T> action, bool pll = false)
        {
            if(pll)
                items.AsParallel().ForAll(item => action(item));
            else
                foreach (var item in items)
                    action(item);
        }

        /// <summary>
        /// Iterates over items sequentially in enumeration order and invokes the supplied action for each one
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="items">The items</param>
        /// <param name="action">The action to apply</param>
        [DebuggerStepThrough]
        public static void iteri<T>(IEnumerable<T> items, Action<int,T> action)
        {
            int i = 0;
            foreach (var item in items)
                action(i++, item);
        }

        /// <summary>
        /// Verifies that the argument value is not null and, if so, throws a <see cref="ArgumentException"/>
        /// </summary>
        /// <typeparam name="T">The argument type</typeparam>
        /// <param name="argName">The name of the argument</param>
        /// <param name="argValue">The value of the argument</param>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T require_arg<T>(string argName, T argValue) where T : class
        {
            if (argValue == null)
                throw new ArgumentNullException(argName);
            return argValue;
        }

        /// <summary>
        /// Creates an array from the supplied items
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="items">The items from which to construct the array</param>
        /// <returns></returns>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] array<T>(params T[] items) => items.ToArray();

        /// <summary>
        /// Creates an array of <see cref="Type"/> references from the supplied type parameters
        /// </summary>
        /// <typeparam name="T1">The first type parameter</typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Type[] type_array<T1>() =>
            new Type[] { typeof(T1) };

        /// <summary>
        /// Creates an array of <see cref="Type"/> references from the supplied type parameters
        /// </summary>
        /// <typeparam name="T1">The first type parameter</typeparam>
        /// <typeparam name="T2">The second type parameter</typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Type[] type_array<T1, T2>() =>
            new Type[] { typeof(T1), typeof(T2) };

        /// <summary>
        /// Creates an array of <see cref="Type"/> references from the supplied type parameters
        /// </summary>
        /// <typeparam name="T1">The first type parameter</typeparam>
        /// <typeparam name="T2">The second type parameter</typeparam>
        /// <typeparam name="T3">The third type parameter</typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Type[] type_array<T1, T2, T3>() =>
            new Type[] { typeof(T1), typeof(T2), typeof(T3) };

        /// <summary>
        /// Creates an array of <see cref="Type"/> references from the supplied type parameters
        /// </summary>
        /// <typeparam name="T1">The first type parameter</typeparam>
        /// <typeparam name="T2">The second type parameter</typeparam>
        /// <typeparam name="T3">The third type parameter</typeparam>
        /// <typeparam name="T4">The fourth type parameter</typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Type[] type_array<T1, T2, T3, T4>() =>
            new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };

        /// <summary>
        /// Creates an array of <see cref="Type"/> references from the supplied type parameters
        /// </summary>
        /// <typeparam name="T1">The first type parameter</typeparam>
        /// <typeparam name="T2">The second type parameter</typeparam>
        /// <typeparam name="T3">The third type parameter</typeparam>
        /// <typeparam name="T4">The fourth type parameter</typeparam>
        /// <typeparam name="T5">The fifth type parameter</typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Type[] type_array<T1, T2, T3, T4, T5>() =>
            new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };

        /// <summary>
        /// Executes the one function if a condition is true and another should it be false
        /// </summary>
        /// <param name="condition">Specifies whether some condition is true</param>
        /// <param name="ifTrue">The action to invoke when condition is true</param>
        /// <param name="ifFalse">The action to invoke when condition is false</param>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void iif(bool condition, Action ifTrue, Action ifFalse)
        {
            if (condition)
                ifTrue();
            else
                ifFalse();
        }

        /// <summary>
        /// Extracts the first element of a 2-tuple
        /// </summary>
        /// <typeparam name="T1">The type of the first element</typeparam>
        /// <typeparam name="T2">The type of the second element</typeparam>
        /// <param name="x">The value of the tuple</param>
        /// <returns></returns>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T1 first<T1, T2>(Tuple<T1, T2> x) => x.Item1;

        /// <summary>
        /// Extracts the second element of a 2-tuple
        /// </summary>
        /// <typeparam name="T1">The type of the first element</typeparam>
        /// <typeparam name="T2">The type of the second element</typeparam>
        /// <param name="x">The value of the tuple</param>
        /// <returns></returns>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 second<T1, T2>(Tuple<T1, T2> x) => x.Item2;

        /// <summary>
        /// Extracts the first element of a 3-tuple
        /// </summary>
        /// <typeparam name="T1">The type of the first element</typeparam>
        /// <typeparam name="T2">The type of the second element</typeparam>
        /// <typeparam name="T3">The type of the third element</typeparam>
        /// <param name="x">The value of the tuple</param>
        /// <returns></returns>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T1 first<T1, T2, T3>(Tuple<T1, T2, T3> x) => x.Item1;

        /// <summary>
        /// Extracts the second element of a 3-tuple
        /// </summary>
        /// <typeparam name="T1">The type of the first element</typeparam>
        /// <typeparam name="T2">The type of the second element</typeparam>
        /// <typeparam name="T3">The type of the third element</typeparam>
        /// <param name="x">The value of the tuple</param>
        /// <returns></returns>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 second<T1, T2, T3>(Tuple<T1, T2, T3> x) => x.Item2;

        /// <summary>
        /// Extracts the third element of a 3-tuple
        /// </summary>
        /// <typeparam name="T1">The type of the first element</typeparam>
        /// <typeparam name="T2">The type of the second element</typeparam>
        /// <typeparam name="T3">The type of the third element</typeparam>
        /// <param name="x">The value of the tuple</param>
        /// <returns></returns>
        [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T3 third<T1, T2, T3>(Tuple<T1, T2, T3> x) => x.Item3;

    }

}
