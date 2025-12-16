using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace ODataHttpClient.Tests
{
    public class DefinitionOrderer : ITestCaseOrderer
    {
        public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
            where TTestCase : notnull, ITestCase
        {
            // TestOrder属性があればその順序で、なければ定義順序を保持
            var orderedCases = testCases
                .Select((testCase, index) => new
                {
                    TestCase = testCase,
                    Order = GetOrder(testCase) ?? index
                })
                .OrderBy(x => x.Order)
                .Select(x => x.TestCase)
                .ToList();

            return orderedCases;
        }

        private static int? GetOrder(ITestCase testCase)
        {
            try
            {
                // xUnit v3では、テストメソッド名から属性を取得
                var methodName = testCase.TestCaseDisplayName;
                var className = testCase.TestClassName;
                
                // アセンブリから対応する型とメソッドを探す
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var types = assembly.GetTypes();
                        var matchingType = types.FirstOrDefault(t => 
                            t.FullName == className || t.Name == className);
                        
                        if (matchingType != null)
                        {
                            var methods = matchingType.GetMethods();
                            var matchingMethod = methods.FirstOrDefault(m => 
                                methodName.Contains(m.Name));
                            
                            if (matchingMethod != null)
                            {
                                var orderAttribute = matchingMethod.GetCustomAttribute<TestOrderAttribute>();
                                return orderAttribute?.Order;
                            }
                        }
                    }
                    catch
                    {
                        // アセンブリの型取得に失敗した場合はスキップ
                    }
                }
            }
            catch
            {
                // 属性の取得に失敗した場合はnullを返す
            }
            
            return null;
        }
    }
}
