﻿using WebExpress.WebIndex.Test.Document;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureIndexE : UnitTestIndexFixture
    {
        /// <summary>
        /// Returns the test data.
        /// </summary>
        public List<UnitTestIndexTestDocumentE> TestData { get; } = UnitTestIndexTestDocumentFactoryE.GenerateTestData();

        /// <summary>
        /// Returns a random document item.
        /// </summary>
        public UnitTestIndexTestDocumentE RandomItem => TestData[Rand.Next(TestData.Count)];

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureIndexE()
        {
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
