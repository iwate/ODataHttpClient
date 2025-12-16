using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCollectionOrderer(typeof(ODataHttpClient.Tests.CollectionDefinitionOrderer))]
