using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace ODataHttpClient.Tests
{
    public class CollectionDefinitionOrderer : ITestCollectionOrderer
    {
        public IReadOnlyCollection<TTestCollection> OrderTestCollections<TTestCollection>(IReadOnlyCollection<TTestCollection> testCollections)
            where TTestCollection : ITestCollection
        {
            // TestOrder属性があればその順序で、なければ定義順序を保持
            var orderedCollections = testCollections
                .Select((collection, index) => new
                {
                    Collection = collection,
                    Order = GetOrder(collection) ?? index
                })
                .OrderBy(x => x.Order)
                .Select(x => x.Collection)
                .ToList();

            return orderedCollections;
        }

        private static int? GetOrder<TTestCollection>(TTestCollection testCollection)
            where TTestCollection : ITestCollection
        {
            try
            {
                // xUnit v3では、コレクション名からアセンブリ内の型を検索
                var displayName = testCollection.TestCollectionDisplayName;
                
                // アセンブリから対応する型を探す
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var types = assembly.GetTypes();
                        var matchingType = types.FirstOrDefault(t => 
                            t.FullName == displayName || t.Name == displayName);
                        
                        if (matchingType != null)
                        {
                            var orderAttribute = matchingType.GetCustomAttribute<TestOrderAttribute>();
                            return orderAttribute?.Order;
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
