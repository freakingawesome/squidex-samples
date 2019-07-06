// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary.Tests
{
    public sealed class ClientQueryFixture : IDisposable
    {
        public SquidexClient<TestEntity, TestEntityData> Client { get; } = TestClient.Build();

        public ClientQueryFixture()
        {
            Task.Run(async () =>
            {
                await CreateAppAsync();
                await CreateSchemaAsync();

                var items = await Client.GetAsync();

                if (items.Total > 10)
                {
                    foreach (var item in items.Items)
                    {
                        await Client.DeleteAsync(item);
                    }

                    items.Total = 0;
                }

                if (items.Total == 0)
                {
                    for (var i = 10; i > 0; i--)
                    {
                        await Client.CreateAsync(new TestEntityData { Value = i }, true);

                        await Task.Delay(1000);
                    }
                }
            }).Wait();
        }

        private static async Task CreateSchemaAsync()
        {
            var schemas = TestClient.ClientManager.CreateSchemasClient();

            try
            {
                await schemas.PostSchemaAsync(TestClient.AppName, new CreateSchemaDto
                {
                    Name = TestClient.SchemaName,
                    Fields = new List<UpsertSchemaFieldDto>
                    {
                        new UpsertSchemaFieldDto
                        {
                            Name = TestClient.FieldName,
                            Properties = new NumberFieldPropertiesDto()
                        }
                    },
                    IsPublished = true
                });
            }
            catch (SquidexManagementException ex)
            {
                if (ex.StatusCode != 400)
                {
                    throw;
                }
            }
        }

        private static async Task CreateAppAsync()
        {
            var apps = TestClient.ClientManager.CreateAppsClient();

            try
            {
                await apps.PostAppAsync(new CreateAppDto
                {
                    Name = TestClient.AppName
                });
            }
            catch (SquidexManagementException ex)
            {
                if (ex.StatusCode != 400)
                {
                    throw;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
