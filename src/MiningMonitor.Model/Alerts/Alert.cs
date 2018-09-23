﻿using System;

using LiteDB;

namespace MiningMonitor.Model.Alerts
{
    public class Alert
    {
        [BsonId(autoId: false)]
        public Guid Id { get; set; }
        public Guid MinerId { get; set; }
        public Guid AlertDefinitionId { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public bool Acknowledged { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
    }
}