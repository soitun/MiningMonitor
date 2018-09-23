﻿using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;

using MiningMonitor.Model;

namespace MiningMonitor.Service
{
    public class SnapshotService : ISnapshotService
    {
        private readonly LiteCollection<Snapshot> _collection;

        public SnapshotService(LiteCollection<Snapshot> collection)
        {
            _collection = collection;
            _collection.EnsureIndex(s => s.MinerId);
            _collection.EnsureIndex(s => s.SnapshotTime);
        }

        public IEnumerable<Snapshot> GetAll()
        {
            return _collection.FindAll();
        }

        public IEnumerable<Snapshot> GetByMiner(Guid minerId, DateTime? from, DateTime? to, TimeSpan interval)
        {
            var end = to ?? DateTime.UtcNow;
            var start = from ?? end.AddMinutes(-60);
            var snapshots = _collection.Find(snapshot => snapshot.MinerId == minerId && snapshot.SnapshotTime >= start && snapshot.SnapshotTime <= end);

            return snapshots.FillGaps(start, end, interval).ToList();
        }

        public void Add(Snapshot snapshot)
        {
            snapshot.Id = Guid.NewGuid();

            _collection.Insert(snapshot);
        }

        public void Upsert(Snapshot snapshot)
        {
            _collection.Upsert(snapshot);
        }

        public void Delete(Guid snapshotId)
        {
            _collection.Delete(snapshotId);
        }

        public void DeleteByMiner(Guid minerId)
        {
            _collection.Delete(s => s.MinerId == minerId);
        }

        public int DeleteOld(DateTime cutoff)
        {
            return _collection.Delete(s => s.SnapshotTime < cutoff);
        }
    }
}