﻿using System;
using System.Collections.Generic;

using MiningMonitor.Alerts.Scanners;
using MiningMonitor.Common;
using MiningMonitor.Model;
using MiningMonitor.Model.Alerts;

namespace MiningMonitor.Alerts
{
    public class Scan : IScan
    {
        private readonly Miner _miner;
        private readonly IAlertScanner _scanner;
        private readonly DateTime _scanTime;

        public Scan(AlertDefinition alertDefinition, Miner miner, IAlertScanner scanner, DateTime scanTime)
        {
            Definition = alertDefinition;
            _miner = miner;
            _scanner = scanner;
            _scanTime = scanTime;
        }

        public AlertDefinition Definition { get; }
        public Period ScanPeriod => _scanner.CalculateScanPeriod(Definition, _scanTime);

        public bool EndAlert(Alert alert, IEnumerable<Snapshot> snapshots)
        {
            return _scanner.EndAlert(Definition, _miner, alert, snapshots, _scanTime);
        }

        public ScanResult PerformScan(IEnumerable<Alert> activeAlerts, IEnumerable<Snapshot> snapshots)
        {
            return _scanner.PerformScan(activeAlerts, Definition, _miner, snapshots, _scanTime);
        }
    }
}