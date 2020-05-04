using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq;

namespace Aspose.Cloud.Marketplace.Services
{
    /// <summary>
    /// Implements Dynamic string interpolation
    /// ex: "config[\"AsposeCloud:AppSid\"]"
    ///     "DateTime.Now.ToString(\"dd-MM-yyyy\")"
    /// Supports 'config' param mapped to IConfiguration configuration,
    /// 'userObj' mapped to userObj
    /// </summary>
    public class ConfigurationExpression : IConfigurationExpression
    {
        private readonly IConfiguration _configuration;
        private readonly object _userObj;
        public ConfigurationExpression(IConfiguration configuration, object userObj = null)
        {
            _configuration = configuration;
            _userObj = userObj;
        }
        public string this[string index] => ConfigurationExpressionEvaluator(_configuration.GetValue<string>(index));

        public string Get(string name, string @default = null) => ConfigurationExpressionEvaluator(_configuration.GetValue(name, @default));

        public string Evaluate(string value) => ConfigurationExpressionEvaluator(value);

        /// <summary>
        /// Replaces all occurances of { } with evaluated values
        /// </summary>
        /// <param name="value">input</param>
        /// <returns></returns>
        public string ConfigurationExpressionEvaluator(string value)
        {
            List<ParameterExpression> expressions = new List<ParameterExpression>()
            {
                Expression.Parameter(typeof(IConfiguration), "config"),     // config.Option1
                Expression.Parameter(typeof(Env), "env")                    // env["DB_NAME"]
            };

            List<object> parameters = new List<object>()
            {
                _configuration,
                Env.Instance
            };
            if (null != _userObj)
            {
                expressions.Add(Expression.Parameter(_userObj.GetType(), "userObj"));
                parameters.Add(_userObj);
            }
            
            ParsingConfig config = new ParsingConfig
            {
                CustomTypeProvider = new CustomDynamicTypeProvider()
            };
            return null == value ? null : 
                Regex.Replace(value, @"{(.+?)}",
                    match => {
                        var expression = DynamicExpressionParser
                            .ParseLambda(config, expressions.ToArray(), null, match.Groups[1].Value);
                        return (expression.Compile().DynamicInvoke(
                            parameters.ToArray()
                            ) ?? "").ToString();
                    });
        }
    }

    /// <summary>
    /// Shortcut for Environment.GetEnvironmentVariable()
    /// </summary>
    public class Env
    {
        public string this[string variable] => Environment.GetEnvironmentVariable(variable);
        static Env() => Instance = new Env();
        public static Env Instance { get; }
    }

    public class CustomDynamicTypeProvider : DefaultDynamicLinqCustomTypeProvider
    {
        public override HashSet<Type> GetCustomTypes() => new[]
        {
            typeof(Environment) // allow expressions like     DB_NAME={Environment.GetEnvironmentVariable(\"DB_NAME\")}
        }.ToHashSet();
    }
}
