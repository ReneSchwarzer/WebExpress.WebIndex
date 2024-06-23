namespace WebExpress.WebIndex.Test.Document
{
    /// <summary>
    /// Represents a test document for a person.
    /// </summary>
    public class UnitTestIndexTestDocumentFactoryE : UnitTestIndexTestDocumentFactory
    {
        /// <summary>
        /// Generates a list of test data for unit testing.
        /// </summary>
        /// <returns>
        /// A list of objects.
        /// </returns>
        public static List<UnitTestIndexTestDocumentE> GenerateTestData()
        {
            var testDataList = new List<UnitTestIndexTestDocumentE>
            {
                new()
                {
                    Id = Guid.Parse("ed242c79-e41b-4214-bfbc-c4673e87433b"),
                    Name = "Noah"
                },
                new()
                {
                    Id = Guid.Parse("cc4ce993-658c-4e7a-b7a5-ff03e6e0e973"),
                    Name = "Noah"
                },
                new()
                {
                    Id = Guid.Parse("dabc3985-0e5b-4c18-a1c5-bead46ae9cfc"),
                    Name = "Emma"
                },
                new()
                {
                    Id = Guid.Parse("e8ce186f-a00d-49a0-9daa-cce7b99c79a2"),
                    Name = "Liam"
                },
                new()
                {
                    Id = Guid.Parse("d50774b3-5d95-4fb4-97fb-d107dd6fb9a0"),
                    Name = "Olivia"
                },
                new()
                {
                    Id = Guid.Parse("21051515-457f-4cc1-8175-8422a233c179"),
                    Name = "Isabella"
                },
                new()
                {
                    Id = Guid.Parse("4bab72b9-c52a-44f6-add5-d50a601f6c8b"),
                    Name = "Sophia"
                },
                new()
                {
                    Id = Guid.Parse("2e7ac878-d2a3-42e7-808e-f38f30d0ad19"),
                    Name = "Xantia"
                },
                new()
                {
                    Id = Guid.Parse("b3fdd146-8070-441b-99df-827670c70bc8"),
                    Name = "Ava"
                },
                new()
                {
                    Id = Guid.Parse("43f9aa18-273e-4a09-a943-b81832f6302a"),
                    Name = "James"
                },
                new()
                {
                    Id = Guid.Parse("7b922a2a-2658-4a4e-b7eb-eb29f679f7ad"),
                    Name = "Isabella"
                },
                new ()
                {
                    Id = Guid.Parse("5abb1742-bee1-45f4-bb6a-8af6a3465fa3"),
                    Name = "Berta"
                },
                new ()
                {
                    Id = Guid.Parse("bbde531a-eb43-46db-8576-284dbb93d254"),
                    Name = "Cäsar"
                },
                new ()
                {
                    Id = Guid.Parse("87cd42d7-169e-4329-8f6e-255f510ee013"),
                    Name = "Doris"
                },
                new ()
                {
                    Id = Guid.Parse("a3e4ddcb-5e79-4bf9-8eca-fea791e6dd4a"),
                    Name = "Anton"
                },
                new ()
                {
                    Id = Guid.Parse("2d242f03-27fc-4da2-a70a-0aac3d94a69f"),
                    Name = "Emil"
                },
                new ()
                {
                    Id = Guid.Parse("3b30f151-6b01-4e3c-85f2-ccb048387f76"),
                    Name = "Fina"
                },
                new ()
                {
                    Id = Guid.Parse("3cae888a-ec88-44d8-ba7b-abbd0681f0ab"),
                    Name = "Kerstin"
                },
                new ()
                {
                    Id = Guid.Parse("5d07aabe-3f7c-448c-b571-5a32f358e722"),
                    Name = "Gustav"
                },
                new ()
                {
                    Id = Guid.Parse("124e9cdd-1efb-40c0-b2fc-38aa9bcabac5"),
                    Name = "Hans"
                },
                new ()
                {
                    Id = Guid.Parse("65c26230-9157-4b22-95ef-a91fa112aeec"),
                    Name = "Ines"
                },
                new ()
                {
                    Id = Guid.Parse("336118d9-be1c-482b-9ff7-724058119220"),
                    Name = "Jana"
                },
                new ()
                {
                    Id = Guid.Parse("4da00d0e-1e2a-4baf-a95a-2e4db46c4366"),
                    Name = "Anton"
                },
                new ()
                {
                    Id = Guid.Parse("5046cb91-049c-4f03-b822-b7c1b961fb08"),
                    Name = "Karin"
                },
                new ()
                {
                    Id = Guid.Parse("160a6cba-5f74-4d0d-939a-5e0764276b04"),
                    Name = "Lars"
                }


            };

            return testDataList;
        }
    }
}
