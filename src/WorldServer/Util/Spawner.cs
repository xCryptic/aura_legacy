using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Aura.Data;
using Aura.World.World;
using Aura.Shared.Network;
using Aura.Shared.Util;

namespace Aura.World.Util
{
    /// <summary>
    /// Was originally abstract+constructorless, but compile time error when being
    /// used in SpawnerDb.
    /// </summary>
    public class NamedValue
    {
        public SpawnLocationType Type { get; protected set; }

        protected string _name = null;
        public string Name
        {
            get { return _name; }
            protected set { _name = value; }
        }

        public NamedValue() { }
    }

    public class NamedPoint : NamedValue
    {
        public Point Point { get; private set; }

        public NamedPoint(string name, uint x, uint y)
            : this(name, new Point(x, y))
        {
        }

        public NamedPoint(string name, Point point)
        {
            this.Type = SpawnLocationType.Point;

            this.Name = name;
            this.Point = point;
        }
    }

    public class NamedLine : NamedValue
    {
        public Line Line { get; private set; }

        public NamedLine(string name, Point p1, Point p2)
            : this(name, new Line(p1, p2))
        {
        }

        public NamedLine(string name, Line line)
        {
            this.Type = SpawnLocationType.Line;

            this.Name = name;
            this.Line = line;
        }
    }

    public class NamedPolygon : NamedValue
    {
        public SpawnRegion Polygon { get; private set; }

        public NamedPolygon(string name, params Point[] points)
            : this(name, new SpawnRegion(points))
        {
        }

        public NamedPolygon(string name, SpawnRegion poly)
        {
            this.Type = SpawnLocationType.Polygon;

            this.Name = name;
            this.Polygon = poly;
        }
    }
    
    public class SpawnerDb : DatabaseCSVIndexed<string, NamedValue>
    {
        protected override void ReadEntry(CSVEntry entry)
        {
            if (entry.Count < 2)
                throw new FieldCountException(2);
            
            var c = entry.ReadString();
            var name = entry.ReadString();
            if (c.Equals("P")) // Point
            {
                //if(entry.Remaining < 2)
                //    throw new FieldCountException(4);

                //var x = entry.ReadUInt(); var y = entry.ReadUInt();
                var points = this.ReadPoints(entry, 1);
                this.Entries.Add(name, new NamedPoint(name, points[0]));
            }
            else if (c.Equals("L")) // Line
            {
                //if (entry.Remaining < 4)
                //    throw new FieldCountException(6);

                var points = this.ReadPoints(entry, 2);
                this.Entries.Add(name, new NamedLine(name, points[0], points[1]));
            }
            else if (c.Equals("R")) // Rectangle
            {
                //if (entry.Remaining < 8)
                //    throw new FieldCountException(10);

                var points = this.ReadPoints(entry, 4);
                this.Entries.Add(name, new NamedPolygon(name, points));
            }
            else if (c.Equals("Y")) // Polygon
            {
                var points = this.ReadPoints(entry);
                this.Entries.Add(name, new NamedPolygon(name, points));
            }
            else return;
        }

        private Point[] ReadPoints(CSVEntry entry, int count = 0)
        {
            if (count > 0)
            {
                if ((entry.Remaining + 1) < (count * 2))
                    throw new FieldCountException(2 + (int)(count * 2));

                Point[] points = new Point[count];
                for (int i = 0; i < count; i++)
                {
                    var x = entry.ReadUInt(); var y = entry.ReadUInt();
                    points[i] = new Point(x, y);
                }
                return points;
            }
            else // Recursive call, read as many as possible
            {
                int rem = ((entry.Remaining + 1) / 2);
                if (rem > 0) return this.ReadPoints(entry, rem);
                else return new Point[0];
            }
        }
    }

    // Not much more than an idea atm
    public class Spawner
    {
        private static SortedDictionary<uint, SpawnerDb> _dbs
            = new SortedDictionary<uint, SpawnerDb>();

        private SpawnerDb _db = null;

        public Spawner(uint regionId)
        {
            _db = this.GetDbOrNull(regionId);
            if (_db == null)
                throw new KeyNotFoundException("No named spawns exist for region with Id " + regionId);
        }

        private SpawnerDb GetDbOrNull(uint regionId)
        {
            SpawnerDb db = null;
            _dbs.TryGetValue(regionId, out db);
            return db;
        }

        public static void Load(string dataPath)
        {
            string path = dataPath + "/spawner";
            if (!Directory.Exists(path))
            {
                Logger.Info("Spawner directory not found: " + path);
                return;
            }
            var files = Directory.EnumerateFiles(path);
            ulong eCount = 0, fCount = 0;
            foreach (var file in files)
            {
                var s = Path.GetFileName(file).Split(new char[] { '_' });
                uint regionId = 0;

                // Check if filename starts with {number}_
                // Ex. 100_spawner.txt
                if(s.Length > 1 && UInt32.TryParse(s[0], out regionId))
                {
                    SpawnerDb db = null;
                    bool exists = _dbs.TryGetValue(regionId, out db); // If already exists, get existing

                    if (db == null)
                    {
                        db = new SpawnerDb();
                        if (!exists) _dbs.Add(regionId, db);
                        else _dbs[regionId] = db;
                    }

                    db.Load(file, false);
                    ServerUtil.PrintDataWarnings(db.Warnings);

                    eCount += (ulong)db.Entries.Count; // Entry count
                    fCount++; // File count
                }
            }

            Logger.Info("Read " + eCount + " named spawns from " + fCount + " files");
        }

        /// <summary>
        /// Get a spawner via region Id, or null if none exists.
        /// </summary>
        /// <param name="regionId">Region Id of spawner to get</param>
        /// <returns>Spawner for a specified region, or null if none exists</returns>
        public static Spawner Get(uint regionId)
        {
            Spawner s = null;
            try { s = new Spawner(regionId); }
            catch (KeyNotFoundException) { }
            return s;
        }

        private NamedValue Value(string name)
        {
            NamedValue v = null;
            _db.Entries.TryGetValue(name, out v);
            return v;
        }

        public Point Point(string name)
        {
            var v = this.Value(name);
            if (v != null && v.Type == SpawnLocationType.Point)
                return ((NamedPoint)v).Point;
            throw new KeyNotFoundException("Specified Point does not exist: '" + name + "'");
        }

        public Line Line(string name)
        {
            var v = this.Value(name);
            if (v != null && v.Type == SpawnLocationType.Line)
                return ((NamedLine)v).Line;
            throw new KeyNotFoundException("Specified Line does not exist: '" + name + "'");
        }
    }
}