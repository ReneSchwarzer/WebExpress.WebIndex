namespace WebExpress.WebIndex.Test.Document
{
    /// <summary>
    /// Represents a test document for a person.
    /// </summary>
    public class UnitTestIndexTestDocumentFactoryA : UnitTestIndexTestDocumentFactory
    {
        /// <summary>
        /// Generates a list of test data for unit testing.
        /// </summary>
        /// <returns>
        /// A list of objects.
        /// </returns>
        public static List<UnitTestIndexTestDocumentA> GenerateTestData()
        {
            var testDataList = new[]
            {
                new UnitTestIndexTestDocumentA { Id = new Guid("b2e8a5c3-1f6d-4e7b-9e1f-8c1a9d0f2b4a"), Text = "Hello Helena!"},
                new UnitTestIndexTestDocumentA { Id = new Guid("c7d8f9e0-3a2b-4c5d-8e6f-9a1b0c2d4e5f"), Text = "Hello Helena, Hello Helge!"},
                new UnitTestIndexTestDocumentA { Id = new Guid("3b597820-f1cb-480c-878f-e70585bbc84d"), Text = "Dear John!"},
                new UnitTestIndexTestDocumentA { Id = new Guid("0d740124-7707-4b1a-a9f1-3a365a15f60b"), Text = "Hello Bob!"},
                new UnitTestIndexTestDocumentA { Id = new Guid("e7925c45-72aa-4dda-82dd-18b72b656170"), Text = "Greetings Bob!"},
                new UnitTestIndexTestDocumentA { Id = new Guid("b017b72c-94b5-46d6-952e-09ee63457210"), Text = "Dear John, Alice, and Bob!"},
                new UnitTestIndexTestDocumentA { Id = new Guid("77251abf-437e-4408-97a7-55545d1622b5"), Text = "Hello Alice, Bob, and Charlie!"},
                new UnitTestIndexTestDocumentA { Id = new Guid("d3ded093-69be-458b-abeb-7516c970371b"), Text = "Hi Bob, Charlie, and Dave!"},
                new UnitTestIndexTestDocumentA { Id = new Guid("a8901fac-aaef-483b-aba8-dba74e36e7fc"), Text = "Dear Helge and Helena!"},
                new UnitTestIndexTestDocumentA { Id = new Guid("3f3d7066-a925-42ac-90f7-ef100afb8460"), Text = "Hello Helena! Dear Helena! Good day Helena!"},
            };

            return [.. testDataList];
        }
    }
}
