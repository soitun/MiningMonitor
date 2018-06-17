﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MiningMonitor.Service;

namespace MiningMonitor.BackgroundWorker.Maintenance
{
    public class Purge : IBackgroundWorker
    {
        private readonly ISettingsService _settingsService;
        private readonly ISnapshotService _snapshotService;
        private readonly ILogger<Purge> _logger;

        public Purge(ISettingsService settingsService, ISnapshotService snapshotService, ILogger<Purge> logger)
        {
            _settingsService = settingsService;
            _snapshotService = snapshotService;
            _logger = logger;
        }

        public async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => DoWork(), cancellationToken);
        }

        public void DoWork()
        {
            var (_, enablePurgeSetting) = _settingsService.GetSetting("EnablePurge");
            var (_, purgeAgeMinutesSetting) = _settingsService.GetSetting("PurgeAgeMinutes");

            if (!bool.TryParse(enablePurgeSetting, out var enablePurge) || !int.TryParse(purgeAgeMinutesSetting, out var purgeAgeMinutes) || !enablePurge)
                return;

            var purgeCutoff = DateTime.Now - TimeSpan.FromMinutes(purgeAgeMinutes);
            _logger.LogInformation($"Purging snapshot data before {purgeCutoff:MM/dd/yy H:mm}");

            var purgedCount = _snapshotService.DeleteOld(purgeCutoff);

            _logger.LogInformation($"Purged {purgedCount} snapshots.");
        }
    }
}