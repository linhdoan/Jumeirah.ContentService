using EPiServer.Find;
using EPiServer.Find.Api;
using EPiServer.Find.Helpers.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EPiServer.Find.Cms
{
    public class XhtmlStringProjectionProcessor : IProjectionProcessor
    {
        internal const string XhtmlStringDefaultValueMethodName = "AsViewedByAnonymous";

        public Expression PreSearch(Expression selector, SearchRequestBody searchRequestBody, IClientConventions clientConventions, Language language)
        {
            return ExpressionExtensions.Replace<MethodCallExpression>(ExpressionExtensions.Replace<MethodCallExpression>(selector, (Func<MethodCallExpression, bool>)(x =>
            {
                if (x.Method.Name == "AsHighlighted")
                    return x.Method.ReflectedType == typeof(XhtmlStringProjectionExtensions);
                else
                    return false;
            }), new Func<MethodCallExpression, Expression>(this.ReplaceAsHighlightedCall)), (Func<MethodCallExpression, bool>)(x =>
            {
                if (x.Method.Name == "AsCropped")
                    return x.Method.ReflectedType == typeof(XhtmlStringProjectionExtensions);
                else
                    return false;
            }), new Func<MethodCallExpression, Expression>(this.ReplaceAsCroppedCall));
        }

        /// <summary>
        /// Replaces calls to AsHighlighted on XhtmlStrings with a call to AsHighlighted on an indexed methods return value.
        ///             For instance it rewrited x.MainBody.AsHighlighted() to x.MainBody.AsViewedByAnonymous().AsHighlighted()
        /// 
        /// </summary>
        /// <param name="original"/>
        /// <returns/>
        private Expression ReplaceAsHighlightedCall(Expression original)
        {
            MethodCallExpression methodCallExpression1 = (MethodCallExpression)original;
            MethodCallExpression methodCallExpression2 = Expression.Call(typeof(XhtmlStringExtensions).GetMethod("AsViewedByAnonymous"), methodCallExpression1.Arguments[0]);
            return (Expression)Expression.Call((Expression)null, typeof(ProjectionExtensions).GetMethod("AsHighlighted", Enumerable.ToArray<Type>(Enumerable.Concat<Type>((IEnumerable<Type>)new Type[1]
      {
        typeof (string)
      }, Enumerable.Select<ParameterInfo, Type>(Enumerable.Skip<ParameterInfo>((IEnumerable<ParameterInfo>)methodCallExpression1.Method.GetParameters(), 1), (Func<ParameterInfo, Type>)(x => x.ParameterType))))), Enumerable.ToArray<Expression>(Enumerable.Concat<Expression>((IEnumerable<Expression>)new Expression[1]
      {
        (Expression) methodCallExpression2
      }, Enumerable.Skip<Expression>((IEnumerable<Expression>)methodCallExpression1.Arguments, 1))));
        }

        /// <summary>
        /// Replaces calls to AsCropped on XhtmlStrings with a call to AsCropped on an indexed methods return value.
        ///             For instance it rewrited x.MainBody.AsCropped() to x.MainBody.AsViewedByAnonymous().AsCropped()
        /// 
        /// </summary>
        /// <param name="original"/>
        /// <returns/>
        private Expression ReplaceAsCroppedCall(Expression original)
        {
            MethodCallExpression methodCallExpression1 = (MethodCallExpression)original;
            MethodCallExpression methodCallExpression2 = Expression.Call(typeof(XhtmlStringExtensions).GetMethod("AsViewedByAnonymous"), methodCallExpression1.Arguments[0]);
            return (Expression)Expression.Call((Expression)null, typeof(ProjectionExtensions).GetMethod("AsCropped", Enumerable.ToArray<Type>(Enumerable.Concat<Type>((IEnumerable<Type>)new Type[1]
      {
        typeof (string)
      }, Enumerable.Select<ParameterInfo, Type>(Enumerable.Skip<ParameterInfo>((IEnumerable<ParameterInfo>)methodCallExpression1.Method.GetParameters(), 1), (Func<ParameterInfo, Type>)(x => x.ParameterType))))), Enumerable.ToArray<Expression>(Enumerable.Concat<Expression>((IEnumerable<Expression>)new Expression[1]
      {
        (Expression) methodCallExpression2
      }, Enumerable.Skip<Expression>((IEnumerable<Expression>)methodCallExpression1.Arguments, 1))));
        }

        public Expression PostSearch(Expression executable, SearchHit<JObject> searchHit, IClientConventions clientConventions, Language language)
        {
            return ExpressionExtensions.Replace<MethodCallExpression>(ExpressionExtensions.Replace<MethodCallExpression>(executable, (Func<MethodCallExpression, bool>)(x =>
            {
                if (x.Method.Name == "AsHighlighted")
                    return x.Method.ReflectedType == typeof(XhtmlStringProjectionExtensions);
                else
                    return false;
            }), new Func<MethodCallExpression, Expression>(this.ReplaceAsHighlightedCall)), (Func<MethodCallExpression, bool>)(x =>
            {
                if (x.Method.Name == "AsCropped")
                    return x.Method.ReflectedType == typeof(XhtmlStringProjectionExtensions);
                else
                    return false;
            }), new Func<MethodCallExpression, Expression>(this.ReplaceAsCroppedCall));
        }
    }
}
