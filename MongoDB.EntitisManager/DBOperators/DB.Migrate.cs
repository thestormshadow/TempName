﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MongoDB.EntitiesManager
{
    public partial class DB
    {
        /// <summary>
        /// Executes migration classes that implement the IMigration interface in the correct order to transform the database.
        /// <para>TIP: Write classes with names such as: _001_rename_a_field.cs, _002_delete_a_field.cs, etc. and implement IMigration interface on them. Call this method at the startup of the application in order to run the migrations.</para>
        /// </summary>
        public static void Migrate()
        {
            var types = Assembly.GetCallingAssembly()
                                .GetTypes()
                                .Where(t => t.GetInterfaces().Contains(typeof(IMigration)));

            if (!types.Any())
                throw new InvalidOperationException("Didn't find any classes that implement IMigrate interface.");

            var lastMigration = Find<Migration>()
                    .Sort(m => m.Number, Order.Descending)
                    .Limit(1)
                    .Execute()
                    .SingleOrDefault();

            var lastMigNum = lastMigration != null ? lastMigration.Number : 0;

            var migrations = new SortedDictionary<int, IMigration>();

            foreach (var t in types)
            {
                var success = int.TryParse(t.Name.Split('_')[1], out int migNum);

                if (!success)
                    throw new InvalidOperationException("Failed to parse migration number from the class name. Make sure to name the migration classes like: _001_some_migration_name.cs");

                if (migNum > lastMigNum)
                    migrations.Add(migNum, (IMigration)Activator.CreateInstance(t));
            }

            var sw = new Stopwatch();

            foreach (var migration in migrations)
            {
                sw.Start();
                migration.Value.Upgrade();
                var mig = new Migration
                {
                    Number = migration.Key,
                    Name = migration.Value.GetType().Name,
                    TimeTakenSeconds = sw.Elapsed.TotalSeconds
                };
                Save(mig);
                sw.Stop();
                sw.Reset();
            }
        }
    }
}
