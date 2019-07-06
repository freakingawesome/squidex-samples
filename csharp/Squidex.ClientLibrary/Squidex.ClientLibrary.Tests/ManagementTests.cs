// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class ManagementTests
    {
        private readonly ISchemasClient schemasClient;

        public ManagementTests()
        {
            schemasClient = TestClient.ClientManager.CreateSchemasClient();
        }

        [Fact]
        public async Task Should_query_schemas()
        {
            var schemas = await schemasClient.GetSchemasAsync(TestClient.AppName);

            Assert.NotEmpty(schemas.Items);
        }

        [Fact]
        public async Task Should_query_schema()
        {
            var schema = await schemasClient.GetSchemaAsync(TestClient.AppName, TestClient.SchemaName);

            Assert.NotNull(schema);
        }

        [Fact]
        public async Task Should_create_schema()
        {
            SchemaDetailsDto schema = null;
            try
            {
                schema = await schemasClient.PostSchemaAsync(TestClient.AppName, new CreateSchemaDto
                {
                    Name = "new-schema",
                    Properties = new SchemaPropertiesDto
                    {
                        Label = "New Schema"
                    },
                    Fields = new List<UpsertSchemaFieldDto>
                    {
                        new UpsertSchemaFieldDto
                        {
                            Name = "String",
                            Properties = new StringFieldPropertiesDto
                            {
                                IsRequired = true
                            }
                        }
                    },
                    IsPublished = true
                });
            }
            finally
            {
                if (schema != null)
                {
                    await schemasClient.DeleteSchemaAsync(TestClient.AppName, schema.Name);
                }
            }
        }
    }
}
