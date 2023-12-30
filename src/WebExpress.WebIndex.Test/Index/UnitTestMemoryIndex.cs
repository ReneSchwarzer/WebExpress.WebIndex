﻿using System.Diagnostics;
using System.Globalization;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    [Collection("UnitTestIndexCollectionFixture")]
    public class UnitTestMemoryIndex(UnitTestIndexFixture fixture, ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; private set; } = output;
        protected UnitTestIndexFixture Fixture { get; set; } = fixture;

        [Fact]
        public void Register()
        {
            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), 11, IndexType.Memory);

            Assert.NotNull(Fixture.IndexManager.GetIndexDocument<UnitTestIndexTestMockA>());
        }

        [Fact]
        public void Clear()
        {
            var testData = UnitTestIndexTestMockA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), 11, IndexType.Memory);
            Fixture.IndexManager.ReIndex(testData);
            Fixture.IndexManager.Clear<UnitTestIndexTestMockA>();

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Noah'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.False(item.Any());
        }

        [Fact]
        public void ReIndexTestDataA()
        {
            var testData = UnitTestIndexTestMockA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), ushort.MaxValue, IndexType.Memory);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Noah'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(2, item.Count());
        }

        [Fact]
        public void ReIndexTestDataB()
        {
            var testData = UnitTestIndexTestMockB.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockB>(CultureInfo.GetCultureInfo("en"), ushort.MaxValue, IndexType.Memory);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockB>("description = 'phasellus'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.True(item.Any());
        }

        [Fact]
        public void ReIndexTestDataC()
        {
            var stopWatch = new Stopwatch();
            var itemCount = 1000;
            var wordCount = 1000;
            var vocabulary = 40000;
            var wordLength = 10;

            var testData = UnitTestIndexTestMockC.GenerateTestData(itemCount, wordCount, vocabulary, wordLength).ToList();

            Output.WriteLine($"ReIndex {itemCount:#,##0} items, {vocabulary:#,##0} vocabulary and {wordLength:#,##0} word length");

            Fixture.IndexManager.Register<UnitTestIndexTestMockC>(CultureInfo.GetCultureInfo("en"));

            // preparing for a measurement
            var begin = Fixture.GetUsedMemory();
            stopWatch.Start();

            Fixture.IndexManager.ReIndex(testData);

            // stop measurement
            stopWatch.Stop();
            var end = Fixture.GetUsedMemory();
            var elapsedReindex = stopWatch.Elapsed;
            var usedReindex = (end - begin) / 1024 / 1024; // in MB

            // preparing for a measurement
            begin = Fixture.GetUsedMemory();
            stopWatch.Start();
            Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockC>("Text ~ 'abcdaaaaaa'");

            // stop measurement
            stopWatch.Stop();
            end = Fixture.GetUsedMemory();
            var elapsedCollect = stopWatch.Elapsed;
            var usedCollect = (end - begin) / 1024 / 1024; // in MB

            Output.WriteLine($"ReIndex take: {elapsedReindex}");
            Output.WriteLine("ReIndex ram used: " + Convert.ToDouble(usedReindex).ToString("0.##") + " MB");

            Output.WriteLine($"Collect take: {elapsedCollect}");
            Output.WriteLine("Collect ram used: " + Convert.ToDouble(usedCollect).ToString("0.##") + " MB");
        }

        [Fact]
        public void ReIndexTestDataD()
        {
            var testData = UnitTestIndexTestMockD.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockD>(CultureInfo.GetCultureInfo("en"), ushort.MaxValue, IndexType.Memory);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockD>("firstname = 'Noah' and lastname = 'Smith'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.True(item.Any());
        }
    }
}
