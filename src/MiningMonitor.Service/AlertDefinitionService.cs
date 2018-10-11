﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MiningMonitor.Data;
using MiningMonitor.Model.Alerts;

namespace MiningMonitor.Service
{
    public class AlertDefinitionService : IAlertDefinitionService
    {
        private readonly IRepository<AlertDefinition> _alertDefinitionCollection;

        public AlertDefinitionService(IRepository<AlertDefinition> alertDefinitionCollection)
        {
            _alertDefinitionCollection = alertDefinitionCollection;
        }

        public async Task<IEnumerable<AlertDefinition>> GetAllAsync(CancellationToken token = default)
        {
            return await _alertDefinitionCollection.FindAllAsync(token);
        }

        public async Task<IEnumerable<AlertDefinition>> GetEnabledAsync(CancellationToken token = default)
        {
            return await _alertDefinitionCollection.FindAsync(a => a.Enabled, token);
        }

        public async Task<IEnumerable<AlertDefinition>> GetByMinerAsync(Guid minerId, CancellationToken token = default)
        {
            return await _alertDefinitionCollection.FindAsync(a => a.MinerId == minerId, token);
        }

        public async Task<AlertDefinition> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _alertDefinitionCollection.FindByIdAsync(id, token);
        }

        public async Task AddAsync(AlertDefinition alertDefinition, CancellationToken token = default)
        {
            alertDefinition.Id = Guid.NewGuid();
            alertDefinition.Created = DateTime.UtcNow;

            if (alertDefinition.Enabled)
                alertDefinition.LastEnabled = DateTime.UtcNow;

            await _alertDefinitionCollection.InsertAsync(alertDefinition, token);
        }

        public async Task<bool> UpdateAsync(AlertDefinition alertDefinition, CancellationToken token = default)
        {
            var current = await _alertDefinitionCollection.FindByIdAsync(alertDefinition.Id, token);
            if (current == null)
                return false;

            alertDefinition.MinerId = current.MinerId;
            alertDefinition.Created = current.Created;
            alertDefinition.LastScan = current.LastScan;
            alertDefinition.Updated = DateTime.UtcNow;
            if (alertDefinition.Parameters.AlertType != current.Parameters.AlertType)
                alertDefinition.Parameters = current.Parameters;

            if (!current.Enabled && alertDefinition.Enabled)
                alertDefinition.LastEnabled = DateTime.UtcNow;
            else
                alertDefinition.LastEnabled = current.LastEnabled;
            
            return await _alertDefinitionCollection.UpdateAsync(alertDefinition, token);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
        {
            return await _alertDefinitionCollection.DeleteAsync(id, token);
        }

        public async Task<bool> MarkScannedAsync(Guid id, DateTime scanTime, CancellationToken token = default)
        {
            var alertDefinition = await _alertDefinitionCollection.FindByIdAsync(id, token);
            if (alertDefinition == null)
                return false;

            alertDefinition.LastScan = scanTime;
            await _alertDefinitionCollection.UpdateAsync(alertDefinition, token);

            return true;
        }
    }
}
