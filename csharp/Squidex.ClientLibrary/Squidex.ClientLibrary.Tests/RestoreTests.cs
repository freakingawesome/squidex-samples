// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class RestoreTests
    {
        private readonly IBackupsClient backupsClient;

        public RestoreTests()
        {
            backupsClient = TestClient.ClientManager.CreateBackupsClient();
        }

        [Fact]
        public async Task Should_invoke_restore()
        {
            var now = DateTime.UtcNow.AddMinutes(-1);

            BackupJobDto backup = null;
            try
            {
                await backupsClient.PostBackupAsync(TestClient.AppName);

                while (true)
                {
                    var backups = await backupsClient.GetBackupsAsync(TestClient.AppName);

                    backup = backups.Items.FirstOrDefault(x => x.Started >= now && (x.Status == JobStatus.Failed || x.Status == JobStatus.Completed));

                    if (backup != null)
                    {
                        break;
                    }

                    await Task.Delay(1000);
                }

                Assert.Equal(JobStatus.Completed, backup.Status);

                var url = new Uri(new Uri(TestClient.ServerUrl), backup._links["download"].Href);

                await Task.Delay(2000);

                await backupsClient.PostRestoreAsync(new RestoreRequestDto { Url = url, Name = $"{TestClient.AppName}-2" });
            }
            finally
            {
                if (backup != null)
                {
                    await backupsClient.DeleteBackupAsync(TestClient.AppName, backup.Id.ToString());
                }
            }
        }
    }
}
