﻿namespace WebExpress.WebIndex.Test.Document
{
    /// <summary>
    /// Factory class for creating unit test documents of type UnitTestIndexTestDocumentD.
    /// </summary>
    public class UnitTestIndexTestDocumentFactoryD : UnitTestIndexTestDocumentFactory
    {
        /// <summary>
        /// Generates a list of test data for unit testing.
        /// </summary>
        /// <returns>
        /// A list of objects.
        /// </returns>
        public static IEnumerable<UnitTestIndexTestDocumentD> GenerateTestData()
        {
            int id = 0;

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Emma",
                LastName = "Smith",
                PhoneNumber = "123-456-7890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Description = "aliqua do tempor ornare morbi eiusmod lorem ipsum phasellus enim aliquet ornare phasellus leo ipsum eu morbi leo tincidunt tincidunt sit sed laoreet amet incididunt eu taciti et et elit netus leo et semper vivamus sagittis vivamus fermentum netus scelerisque phasellus lorem tristique tristique taciti adipiscing aliquet labore mollis dolor morbi adipiscing ipsum tincidunt ut vivamus netus netus ut aenean labore dolore",
                Salutation = "Ms.",
                Address = "123 Main St."
            };

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Liam",
                LastName = "Johnson",
                PhoneNumber = "234-567-8901",
                DateOfBirth = new DateTime(1985, 2, 2),
                Description = "lorem scelerisque ornare mollis quis malesuada eiusmod mollis eiusmod leo labore sit dolore consectetur eu incididunt scelerisque scelerisque vivamus phasellus phasellus quis et morbi leo sit sem tincidunt amet amet incididunt incididunt",
                Salutation = "Mr.",
                Address = "456 Elm St."
            };

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Olivia",
                LastName = "Williams",
                PhoneNumber = "523-446-9890",
                DateOfBirth = new DateTime(1991, 1, 16),
                Description = "lorem malesuada sit malesuada elit quis tristique ipsum placerat netus enim et et phasellus mollis aliquet do enim labore incididunt dolore",
                Salutation = "Ms.",
                Address = "923 Main St."
            };

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Noah",
                LastName = "Jones",
                PhoneNumber = "834-517-8933",
                DateOfBirth = new DateTime(1975, 8, 2),
                Description = "labore aliquet fermentum dictum tempor magna labore consectetur ipsum malesuada scelerisque ornare vivamus amet tempor aliqua lectus aenean amet fermentum vivamus adipiscing",
                Salutation = "Mr.",
                Address = "756 Elm St."
            };

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Isabella",
                LastName = "Miller",
                PhoneNumber = "342-983-9256",
                DateOfBirth = new DateTime(1961, 8, 6),
                Description = "lorem ipsum aenean labore sed consectetur sem sed enim incididunt semper enim sem leo consectetur leo fermentum elit morbi do amet aenean tristique sed egestas consectetur",
                Salutation = "Ms.",
                Address = "154 Main St."
            };

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Sophia",
                LastName = "Brown",
                PhoneNumber = "823-406-7821",
                DateOfBirth = new DateTime(1979, 7, 13),
                Description = "vivamus phasellus phasellus quis et morbi leo sit sem tincidunt amet amet incididunt incididunt dolor fermentum do aliqua incididunt labore sem adipiscing enim",
                Salutation = "Ms.",
                Address = "456 Elm St."
            };

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Xantia",
                LastName = "Garcia",
                PhoneNumber = "345-093-9352",
                DateOfBirth = new DateTime(1981, 6, 30),
                Description = GenerateLoremIpsum(id * 2 % 1000),
                Salutation = "Mr.",
                Address = "14 Maple Ave."
            };

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Ava",
                LastName = "Martinez",
                PhoneNumber = "345-093-9352",
                DateOfBirth = new DateTime(1961, 9, 20),
                Description = GenerateLoremIpsum(id * 2 % 1000),
                Salutation = "Ms.",
                Address = "154 Oak Ave."
            };

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "James",
                LastName = "Davis",
                PhoneNumber = "937-482-8352",
                DateOfBirth = new DateTime(1952, 3, 15),
                Description = GenerateLoremIpsum(id * 2 % 1000),
                Salutation = "Mr.",
                Address = "536 Pine St."
            };

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Noah",
                LastName = "Smith",
                PhoneNumber = "631-378-2956",
                DateOfBirth = new DateTime(1952, 3, 15),
                Description = GenerateLoremIpsum(id * 2 % 1000),
                Salutation = "Mr.",
                Address = "756 Elm St."
            };

            yield return new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Isabella",
                LastName = "Johnson",
                PhoneNumber = "245-5239-3325",
                DateOfBirth = new DateTime(1955, 6, 13),
                Description = GenerateLoremIpsum(id * 2 % 1000),
                Salutation = "Ms.",
                Address = "154 Main St."
            };

            // Add more test data here
            for (int i = 0; i < 100; i++)
            {
                yield return new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    PhoneNumber = $"{i:000}-000-0000",
                    DateOfBirth = new DateTime(2000, 1, 1).AddDays(i),
                    Description = GenerateLoremIpsum(100),
                    Salutation = i % 2 == 0 ? "Mr." : "Ms.",
                    Address = $"{i} {GenerateSreet(i)}"
                };
            }
        }
    }
}
