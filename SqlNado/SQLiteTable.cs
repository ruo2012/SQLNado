﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SqlNado
{
    [SQLiteTable(Name = "sqlite_master")]
    public sealed class SQLiteTable
    {
        internal SQLiteTable(SQLiteDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));

            Database = database;
        }

        [Browsable(false)] // remove from tablestring dumps
        public SQLiteDatabase Database { get; }
        public string Type { get; internal set; }
        public string Name { get; internal set; }
        [SQLiteColumn(Name = "tbl_name")]
        public string TableName { get; internal set; }
        public int RootPage { get; internal set; }
        public string Sql { get; internal set; }

        public IEnumerable<SQLiteColumn> Columns
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                    return Enumerable.Empty<SQLiteColumn>();

                var options = new SQLiteLoadOptions<SQLiteColumn>(Database);
                options.CreateInstanceFunc = (t, o) => new SQLiteColumn(this);
                return Database.Load("PRAGMA table_info(" + SQLiteStatement.EscapeName(Name) + ")", options);
            }
        }

        public override string ToString() => Name;
    }
}