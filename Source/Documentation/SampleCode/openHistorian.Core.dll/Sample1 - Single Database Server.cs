﻿using System;
using System.IO;
using NUnit.Framework;
using openHistorian;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace SampleCode.openHistorian.Server.dll
{
    [TestFixture]
    public class Sample1
    {
        [Test]
        public void CreateScadaDatabase()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            var key = new HistorianKey();
            var value = new HistorianValue();

            var db = new HistorianDatabaseInstance();
            db.IsNetworkHosted = false;
            db.InMemoryArchive = false;
            db.Paths = new[] { @"c:\temp\Scada\" };

            using (var server = new HistorianServer(db))
            {
                var database = server.GetDefaultDatabase();
                for (ulong x = 0; x < 1000; x++)
                {
                    key.Timestamp = x;
                    database.Write(key, value);
                }

                database.HardCommit();
            }
        }

        [Test]
        public void TestReadData()
        {
            var db = new HistorianDatabaseInstance();
            db.InMemoryArchive = false;
            db.ConnectionString = "port=1234";
            db.Paths = new[] { @"c:\temp\Scada\" };

            using (var server = new HistorianServer(db))
            {
                var database = server.GetDefaultDatabase();
                using (var reader = database.OpenDataReader())
                {
                    var stream = reader.Read(10, 800 - 1);
                    while (stream.Read())
                    {
                        Console.WriteLine(stream.CurrentKey.Timestamp);
                    }
                }
            }
        }
    }
}