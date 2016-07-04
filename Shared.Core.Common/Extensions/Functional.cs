﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared.Core.Common.Extensions
{
    //source: http://stackoverflow.com/questions/13041674/create-func-or-action-for-any-method-using-reflection-in-c
    public static class DelegateBuilder
    {
        public static T BuildDelegate<T>(this MethodInfo method, params object[] missingParamValues)
        {
            var queueMissingParams = new Queue<object>(missingParamValues);

            var dgtMi = typeof(T).GetMethod("Invoke");
            var dgtRet = dgtMi.ReturnType;
            var dgtParams = dgtMi.GetParameters();

            var paramsOfDelegate = dgtParams
                .Select(tp => Expression.Parameter(tp.ParameterType, tp.Name))
                .ToArray();

            var methodParams = method.GetParameters();

            if (method.IsStatic)
            {
                var paramsToPass = methodParams
                    .Select((p, i) => CreateParam(paramsOfDelegate, i, p, queueMissingParams))
                    .ToArray();

                var expr = Expression.Lambda<T>(
                    Expression.Call(method, paramsToPass),
                    paramsOfDelegate);

                return expr.Compile();
            }
            else
            {
                try
                {
                    var paramThis = Expression.Convert(paramsOfDelegate[0], method.DeclaringType);

                    var paramsToPass = methodParams
                        .Select((p, i) => CreateParam(paramsOfDelegate, i + 1, p, queueMissingParams))
                        .ToArray();

                    var expr = Expression.Lambda<T>(
                        Expression.Call(paramThis, method, paramsToPass),
                        paramsOfDelegate);

                    return expr.Compile();
                }
                catch (Exception e)
                {
                    
                    throw;
                }
                
            }
        }

        private static Expression CreateParam(IList<ParameterExpression> paramsOfDelegate, 
            int i, ParameterInfo callParamType, Queue<object> queueMissingParams)
        {
            if (i < paramsOfDelegate.Count)
                return Expression.Convert(paramsOfDelegate[i], callParamType.ParameterType);

            if (queueMissingParams.Count > 0)
                return Expression.Constant(queueMissingParams.Dequeue());

            return Expression.Constant(callParamType.ParameterType.IsValueType 
                                        ? Activator.CreateInstance(callParamType.ParameterType) 
                                        : null);
        }
    }
}