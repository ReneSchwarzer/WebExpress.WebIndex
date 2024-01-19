namespace WebExpress.WebIndex.Test.Index
{
    public class UnitTestIndexTestMockA : UnitTestIndexTestDocument
    {
        public string Name { get; set; }

        public static List<UnitTestIndexTestMockA> GenerateTestData()
        {
            var testDataList = new List<UnitTestIndexTestMockA>
            {
                new()
                {
                    Id = Guid.Parse("ED242C79-E41B-4214-BFBC-C4673E87433B"),
                    Name = "Noah"
                },
                new()
                {
                    Id = Guid.Parse("CC4CE993-658C-4E7A-B7A5-FF03E6E0E973"),
                    Name = "Noah"
                },
                new()
                {
                    Id = Guid.Parse("DABC3985-0E5B-4C18-A1C5-BEAD46AE9CFC"),
                    Name = "Emma"
                },
                new()
                {
                    Id = Guid.Parse("E8CE186F-A00D-49A0-9DAA-CCE7B99C79A2"),
                    Name = "Liam"
                },
                new()
                {
                    Id = Guid.Parse("D50774B3-5D95-4FB4-97FB-D107DD6FB9A0"),
                    Name = "Olivia"
                },
                new()
                {
                    Id = Guid.Parse("21051515-457F-4CC1-8175-8422A233C179"),
                    Name = "Isabella"
                },
                new()
                {
                    Id = Guid.Parse("4BAB72B9-C52A-44F6-ADD5-D50A601F6C8B"),
                    Name = "Sophia"
                },
                new()
                {
                    Id = Guid.Parse("2E7AC878-D2A3-42E7-808E-F38F30D0AD19"),
                    Name = "Xantia"
                },
                new()
                {
                    Id = Guid.Parse("B3FDD146-8070-441B-99DF-827670C70BC8"),
                    Name = "Ava"
                },
                new()
                {
                    Id = Guid.Parse("43F9AA18-273E-4A09-A943-B81832F6302A"),
                    Name = "James"
                },
                new()
                {
                    Id = Guid.Parse("7B922A2A-2658-4A4E-B7EB-EB29F679F7AD"),
                    Name = "Isabella"
                },
                new ()
                {
                    Id = Guid.Parse("5ABB1742-BEE1-45F4-BB6A-8AF6A3465FA3"),
                    Name = "Berta"
                },
                new ()
                {
                    Id = Guid.Parse("BBDE531A-EB43-46DB-8576-284DBB93D254"),
                    Name = "Cäsar"
                },
                new ()
                {
                    Id = Guid.Parse("87CD42D7-169E-4329-8F6E-255F510EE013"),
                    Name = "Doris"
                },
                new ()
                {
                    Id = Guid.Parse("A3E4DDCB-5E79-4BF9-8ECA-FEA791E6DD4A"),
                    Name = "Anton"
                },
                new ()
                {
                    Id = Guid.Parse("2D242F03-27FC-4DA2-A70A-0AAC3D94A69F"),
                    Name = "Emil"
                },
                new ()
                {
                    Id = Guid.Parse("3B30F151-6B01-4E3C-85F2-CCB048387F76"),
                    Name = "Fina"
                },
                new ()
                {
                    Id = Guid.Parse("3CAE888A-EC88-44D8-BA7B-ABBD0681F0AB"),
                    Name = "Kerstin"
                },
                new ()
                {
                    Id = Guid.Parse("5D07AABE-3F7C-448C-B571-5A32F358E722"),
                    Name = "Gustav"
                },
                new ()
                {
                    Id = Guid.Parse("124E9CDD-1EFB-40C0-B2FC-38AA9BCABAC5"),
                    Name = "Hans"
                },
                new ()
                {
                    Id = Guid.Parse("65C26230-9157-4B22-95EF-A91FA112AEEC"),
                    Name = "Ines"
                },
                new ()
                {
                    Id = Guid.Parse("336118D9-BE1C-482B-9FF7-724058119220"),
                    Name = "Jana"
                },
                new ()
                {
                    Id = Guid.Parse("4DA00D0E-1E2A-4BAF-A95A-2E4DB46C4366"),
                    Name = "Anton"
                },
                new ()
                {
                    Id = Guid.Parse("5046CB91-049C-4F03-B822-B7C1B961FB08"),
                    Name = "Karin"
                },
                new ()
                {
                    Id = Guid.Parse("160A6CBA-5F74-4D0D-939A-5E0764276B04"),
                    Name = "Lars"
                }


            };

            return testDataList;
        }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
