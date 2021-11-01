using Commons.Interfaces;
using Commons.Utils;
using Commons.Entities;
using Commons.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Entities.Context
{
    public class BaseDbContext<T> : DbContext where T : DbContext
    {
        private const string ENTITIES_PUBLIC_CHANGE_RECORD = "Entities.Public.ChangeRecord";
        private const string ENTITIES_PUBLIC_CHANGE_RECORD_TRACKER = "Entities.Public.ChangeRecordTracker";

        private object _previous { get; set; }

        public BaseDbContext(DbContextOptions<T> options)
            : base(options)
        {
        }

        public DbSet<ChangeRecord> ChangeRecord { get; set; }
        public DbSet<ChangeRecordTracker> ChangeRecordTracker { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("public");

            modelBuilder.ApplyConfiguration(new ChangeRecordConfiguration());
            modelBuilder.ApplyConfiguration(new ChangeRecordTrackerConfiguration());
        }

        /// <summary>
        /// Get all data flow entries
        /// </summary>
        private List<EntityEntry> _entries
        {
            get
            {
                return ChangeTracker.Entries()?.ToList();
            }
        }

        /// <summary>
        /// Get modified data flow entries
        /// </summary>
        private List<EntityEntry> _modified
        {
            get
            {
                return ChangeTracker.Entries()?.Where(e => e.State == EntityState.Modified)?.ToList();
            }
        }

        /// <summary>
        /// Get added data flow entries
        /// </summary>
        private List<EntityEntry> _added
        {
            get
            {
                return ChangeTracker.Entries()?.Where(e => e.State == EntityState.Added)?.ToList();
            }
        }

        /// <summary>
        /// Custom 'SaveChanges' implementation by cfsant
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            try
            {
                if (!this.SaveChangesValidate())
                {
                    return base.SaveChanges();
                }

                this.Added();
                this.Modified();

                return base.SaveChanges();
            }
            finally
            {
                this._previous = null;
            }
        }

        /// <summary>
        /// Custom validation method for 'SaveChanges'
        /// </summary>
        /// <returns></returns>
        private bool SaveChangesValidate()
        {
            var entries = ChangeTracker.Entries()?.ToList();
            if (entries == null || entries == default || entries.Count < 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Data ENTRY flow
        /// </summary>
        /// <returns></returns>
        private void Added()
        {
            var added = this._added;
            if (!this.AddedValidate(added))
            {
                return;
            }

            added.ForEach(entry => AddEntry(entry));
        }

        private void AddEntry(EntityEntry entry)
        {
            if (!this.AddEntryValidate(entry))
            {
                return;
            }

            if (this.AddChangeRecord(entry))
            {
                return;
            }

            ((IBase)entry.Entity).Insertion = DateTime.Now;
            ((IBase)entry.Entity).PublisherId = Guid.Empty;
        }

        private bool AddEntryValidate(EntityEntry entry)
        {
            if (entry == null || entry == default)
            {
                return false;
            }

            if (entry.Entity == null || entry.Entity == default)
            {
                return false;
            }

            if (!typeof(IBase).IsAssignableFrom(entry.Entity.GetType()))
            {
                return false;
            }

            return true;
        }

        private bool AddedValidate(List<EntityEntry> added)
        {
            if (added == null || added == default || added.Count < 1)
            {
                return false;
            }

            return true;
        }

        private bool AddChangeRecord(EntityEntry entry)
        {
            if (!this.AddChangeRecordValidate(entry))
            {
                return false;
            }

            try
            {
                this.AddChangeRecordPrep(entry);

                return this.AddChangeRecordProc(entry);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool AddChangeRecordValidate(EntityEntry entry)
        {
            if (!ENTITIES_PUBLIC_CHANGE_RECORD.ToLower().Equals(entry.Entity.GetType().ToString().ToLower()))
            {
                return false;
            }

            return true;
        }

        private void AddChangeRecordPrep(EntityEntry entry)
        {
            ((IBase)entry.Entity).Change = DateTime.Now;
        }

        private void ModifiedChangeRecordPrep(EntityEntry entry)
        {
            ((IBase)entry.Entity).Change = DateTime.Now;
        }

        private bool AddChangeRecordProc(EntityEntry entry)
        {
            var data = base.Add(entry.Entity);
            if (data == null || data == default)
            {
                return false;
            }

            if (data.Entity == null || data.Entity == default)
            {
                return false;
            }

            if (!typeof(IBase).IsAssignableFrom(data.Entity.GetType()))
            {
                return false;
            }

            if (((IBase)data.Entity).Id == null || ((IBase)data.Entity).Id == default)
            {
                return false;
            }

            return true;
        }

        private bool ModifiedChangeRecordProc(EntityEntry entry)
        {
            var data = base.Update(entry.Entity);
            if (data == null || data == default)
            {
                return false;
            }

            if (data.Entity == null || data.Entity == default)
            {
                return false;
            }

            if (!typeof(IBase).IsAssignableFrom(data.Entity.GetType()))
            {
                return false;
            }

            if (((IBase)data.Entity).Id == null || ((IBase)data.Entity).Id == default)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Data UPDATE flow
        /// </summary>
        /// <returns></returns>
        private void Modified()
        {
            var modified = _modified;
            if (!this.ModifiedValidate(modified))
            {
                return;
            }

            modified.ForEach(entry => ModifiedEntry(entry));
        }

        private bool ModifiedValidate(List<EntityEntry> modified)
        {
            if (modified == null || modified == default || modified.Count < 1)
            {
                return false;
            }

            return true;
        }

        private void ModifiedEntry(EntityEntry entry)
        {
            var prev = Mapper.CreateWithValues(entry.OriginalValues, entry.Entity.GetType());
            if (prev == null)
            {
                return;
            }

            if (!this.ModifiedEntryValidate(entry))
            {
                return;
            }

            if (this.ModifiedChangeRecord(entry))
            {
                return;
            }

            ((IBase)entry.Entity).Change = DateTime.Now;

            this.Tracker(entry);
        }

        private bool ModifiedChangeRecord(EntityEntry entry)
        {
            if (!this.ModifiedChangeRecordValidate(entry))
            {
                return false;
            }

            try
            {
                this.ModifiedChangeRecordPrep(entry);

                return this.ModifiedChangeRecordProc(entry);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ModifiedChangeRecordValidate(EntityEntry entry)
        {
            if (!ENTITIES_PUBLIC_CHANGE_RECORD.ToLower().Equals(entry.Entity.GetType().ToString().ToLower()))
            {
                return false;
            }

            return true;
        }

        private bool ModifiedEntryValidate(EntityEntry entry)
        {
            if (entry == null || entry == default)
            {
                return false;
            }

            if (entry.Entity == null || entry.Entity == default)
            {
                return false;
            }

            if (!typeof(IBase).IsAssignableFrom(entry.Entity.GetType()))
            {
                return false;
            }

            if (((IBase)entry.Entity).PublisherId == default(Guid?))
            {
                return false;
            }

            return true;
        }

        private void Tracker(EntityEntry entry)
        {
            ChangeRecord entityChangeRecord = this.GetChangeRecord(entry);
            if (entityChangeRecord == null || entityChangeRecord == default)
            {
                return;
            }

            this.TrackerPrevious(entityChangeRecord);

            var entryChangeRecord = base.Add(entityChangeRecord);
            if (entryChangeRecord == null || entryChangeRecord == default)
            {
                return;
            }

            if (entryChangeRecord.Entity == null || entryChangeRecord.Entity == default)
            {
                return;
            }

            entityChangeRecord = Mapper.SoftCopy<ChangeRecord>(entryChangeRecord.Entity);
            if (entityChangeRecord == null || entityChangeRecord.Id == default)
            {
                return;
            }

            ChangeRecordTracker entityChangeRecordTracker = GetChangeRecordTracker(entityChangeRecord);
            if (entityChangeRecordTracker == null || entityChangeRecordTracker == default)
            {
                return;
            }

            var entryChangeRecordTracker = base.Add(entityChangeRecordTracker);
            if (entryChangeRecordTracker == null || entryChangeRecordTracker == default)
            {
                return;
            }

            if (entryChangeRecordTracker.Entity == null || entryChangeRecordTracker.Entity == default)
            {
                return;
            }
        }

        private ChangeRecordTracker GetChangeRecordTracker(ChangeRecord changeRecord)
        {
            ChangeRecordTracker tracker = new ChangeRecordTracker();
            tracker.Name = changeRecord.Name;
            tracker.RecordIdentifier = changeRecord.RecordIdentifier;
            tracker.ChangeRecordId = (Guid)changeRecord.Id;
            tracker.OwnerId = changeRecord.OwnerId;
            tracker.PublisherId = changeRecord.PublisherId;

            return tracker;
        }

        private ChangeRecord GetChangeRecord(EntityEntry entry)
        {
            ChangeRecord record = new ChangeRecord();

            var serializedPrevious = JsonConvert.SerializeObject(this._previous);
            if (string.IsNullOrEmpty(serializedPrevious) || string.IsNullOrWhiteSpace(serializedPrevious))
            {
                return null;
            }

            var serializedCurrent = JsonConvert.SerializeObject(entry.Entity);
            if (string.IsNullOrEmpty(serializedCurrent) || string.IsNullOrWhiteSpace(serializedCurrent))
            {
                return null;
            }

            record.Name = entry.Entity.GetType().ToString();
            record.RecordIdentifier = (Guid)((IBase)entry.Entity).Id;
            record.SerializedPrevious = serializedPrevious;
            record.SerializedCurrent = serializedCurrent;
            record.OwnerId = ((IBase)entry.Entity).PublisherId;
            record.PublisherId = (Guid)((IBase)entry.Entity).PublisherId;

            return record;
        }

        public override EntityEntry Update(object entity)
        {
            this._previous = this.AsNoTracking(entity.GetType(), ((IBase)entity).Id);

            var result = base.Update(entity);

            return result;
        }

        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            var result = base.Update<TEntity>(entity);

            this._previous = Mapper.CreateWithValues(result.OriginalValues, result.Entity.GetType());

            return result;
        }

        public EntityEntry<TEntity> Update<TEntity>(TEntity entity, bool undo) where TEntity : class
        {
            var result = base.Update<TEntity>(entity);

            this._previous = this.AsNoTracking(entity.GetType(), ((IBase)entity).Id);

            return result;
        }

        private void TrackerPrevious(ChangeRecord entity)
        {
            var last = this.ChangeRecord.Where(x => x.RecordIdentifier == entity.RecordIdentifier && x.State == true)?.FirstOrDefault();
            if (last == null || last == default)
            {
                return;
            }

            last.State = false;
            last.PublisherId = entity.PublisherId;

            this.ChangeRecord.Update(last);
        }

        private object AsNoTracking(Type type, Guid? id)
        {
            object entity = default(object);

            try
            {
                entity = base.Find(type, id);
            }
            catch (Exception)
            {
                entity = default(object);
            }

            if (entity == null || entity == default(object))
            {
                return entity;
            }

            base.Entry(entity).State = EntityState.Detached;

            return Mapper.SoftCopy(entity, entity.GetType());
        }
    }
}