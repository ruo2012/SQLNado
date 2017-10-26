﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlNado
{
    public sealed class SQLiteIndex
    {
        internal SQLiteIndex(SQLiteTable table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            Table = table;
        }

        public SQLiteTable Table { get; }
        [SQLiteColumn(Name = "seq")]
        public int Ordinal { get; internal set; }
        [SQLiteColumn(Name = "unique")]
        public bool IsUnique { get; internal set; }
        [SQLiteColumn(Name = "partial")]
        public bool IsPartial { get; internal set; }
        public string Name { get; internal set; }
        public string Origin { get; internal set; }

        public IEnumerable<SQLiteColumn> Columns
        {
            get
            {
                var list = IndexColumns.ToList();
                list.Sort();
                foreach (var col in list)
                {
                    if (col.Name == null)
                        continue;

                    var column = Table.GetColumn(col.Name);
                    if (column != null)
                        yield return column;
                }
            }
        }

        public IEnumerable<SQLiteIndexColumn> IndexColumns
        {
            get
            {
                var options = new SQLiteLoadOptions<SQLiteIndexColumn>(Table.Database);
                options.GetInstanceFunc = (t, s, o) => new SQLiteIndexColumn(this);
                return Table.Database.Load("PRAGMA index_xinfo(" + Name + ")", options);
            }
        }

        public override string ToString() => Name;
    }
}
