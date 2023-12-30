namespace WebExpress.WebIndex.Test.Index
{
    public class UnitTestIndexTestMockB : UnitTestIndexTestDocument
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public bool New { get; set; }

        public static List<UnitTestIndexTestMockB> GenerateTestData()
        {
            var testDataList = new List<UnitTestIndexTestMockB>();

            // Add more test data here
            for (int i = 0; i < 10000; i++)
            {
                testDataList.Add(new UnitTestIndexTestMockB
                {
                    Id = Guid.NewGuid(),
                    Name = $"Name_{i}",
                    Summary = $"Der Name_{i}",
                    Description = GenerateLoremIpsum(1000),
                    Date = DateTime.Now.AddMonths(i % 12),
                    Price = i,
                    New = i % 2 != 0
                });
            }

            return testDataList;
        }

        public override string ToString()
        {
            return string.Format("({0}:{1},{2},{3},{4})", Id, Name, Date.ToShortDateString(), Price, New).Trim();
        }
    }
}
