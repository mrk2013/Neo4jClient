﻿using System.Collections.Generic;
using NUnit.Framework;
using Neo4jClient.Gremlin;

namespace Neo4jClient.Test.Gremlin
{
    [TestFixture]
    public class TableTests
    {
        [Test]
        public void TableShouldAppendStepToQuery()
        {
            var query = new NodeReference(123)
                .OutV<object>()
                .As("foo")
                .Table<TableResult>();

            Assert.IsInstanceOf<GremlinProjectionEnumerable<TableResult>>(query);
            Assert.IsInstanceOf<IEnumerable<TableResult>>(query);
            Assert.IsInstanceOf<IGremlinQuery>(query);

            var enumerable = (IGremlinQuery)query;
            Assert.AreEqual("g.v(p0).outV.as(p1).table(new Table())", enumerable.QueryText);
            Assert.AreEqual(123, enumerable.QueryParameters["p0"]);
            Assert.AreEqual("foo", enumerable.QueryParameters["p1"]);
        }

        [Test]
        public void TableShouldAppendStepToQueryWithClosures()
        {
            var query = new NodeReference(123)
                .OutV<Foo>()
                .As("foo")
                .InV<Bar>()
                .As("bar")
                .Table<TableResult, Foo, Bar>(
                    foo => foo.SomeText,
                    bar => bar.SomeNumber
                );

            Assert.IsInstanceOf<GremlinProjectionEnumerable<TableResult>>(query);
            Assert.IsInstanceOf<IEnumerable<TableResult>>(query);
            Assert.IsInstanceOf<IGremlinQuery>(query);

            var enumerable = (IGremlinQuery)query;
            Assert.AreEqual("g.v(p0).outV.as(p1).inV.as(p2).table(new Table()){it[p3]}{it[p4]}", enumerable.QueryText);
            Assert.AreEqual(123, enumerable.QueryParameters["p0"]);
            Assert.AreEqual("foo", enumerable.QueryParameters["p1"]);
            Assert.AreEqual("bar", enumerable.QueryParameters["p2"]);
            Assert.AreEqual("SomeText", enumerable.QueryParameters["p3"]);
            Assert.AreEqual("SomeNumber", enumerable.QueryParameters["p4"]);
        }

        public class Foo
        {
            public string SomeText { get; set; }
        }

        public class Bar
        {
            public int SomeNumber { get; set; }
        }

        public class TableResult
        {
        }
    }
}