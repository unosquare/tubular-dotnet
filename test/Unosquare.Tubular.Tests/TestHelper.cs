namespace Unosquare.Tubular.Tests
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using Database;
    using System;

    [TestFixture]
    public class TestHelper
    {
        private const int PageSize = 10;
        private const string SearchText = "Name - 1";

        private static void SimpleListTest(bool ignoreTimezone, int page = 1, int setSize = 445)
        {
            var dataSource = SampleEntities.GenerateData(setSize).AsQueryable();
            var data = dataSource.Skip(PageSize * page).Take(PageSize).ToList();
            const int timezoneOffset = 300;

            if (PageSize * page + PageSize < setSize)
                Assert.AreEqual(data.Count, PageSize, "Set has 10 items");

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = PageSize * page,
                Search = new Filter(),
                Columns = Thing.GetColumns(),
                TimezoneOffset = timezoneOffset
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(page + 1, response.CurrentPage);
            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");
            Assert.AreEqual(data.First().Id, response.Payload.First().First(), "Same first item");

            Assert.That(
                ignoreTimezone ? data.First().Date : data.First().Date.AddMinutes(-timezoneOffset),
                Is.EqualTo(response.Payload.First()[2]).Within(10).Seconds,
                "Same date at first item");

            Assert.AreEqual(dataSource.Count(), response.TotalRecordCount, "Total rows matching");
        }

        [Test]
        public void SimpleList([Range(0, 21)] int page, [Range(430, 450)] int setSize)
        {
            SimpleListTest(false, page, setSize);
        }

        [Test]
        public void SimpleListIgnoreTimezoneOffset()
        {
            // Ignore timezone adjustment
            TubularDefaultSettings.AdjustTimezoneOffset = false;
            SimpleListTest(true);
            TubularDefaultSettings.AdjustTimezoneOffset = true;
        }

        [Test]
        public void SimpleFilter()
        {
            const int filter = 95;
            var dataSource = SampleEntities.GenerateData().AsQueryable();
            var filterCount = dataSource.Where(x => x.Id > filter);
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithIdFilter(filter.ToString(), CompareOperators.Gt)
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            Assert.AreEqual(filterCount.Count(), response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void SimpleSort()
        {
            var dataSource = SampleEntities.GenerateData().AsQueryable().OrderBy(x => x.Date);
            var data = dataSource.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithSort()
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");
            Assert.AreEqual(data.First().Id, response.Payload.First().First(), "Same first item");

            Assert.AreEqual(dataSource.Count(), response.TotalRecordCount, "Total rows matching");
        }

        [Test]
        public void SimpleSearch()
        {
            var dataSource = SampleEntities.GenerateData().AsQueryable();
            var data = dataSource.Where(x => x.Name.Contains(SearchText)).Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter
                {
                    Operator = CompareOperators.Auto,
                    Text = SearchText
                },
                Columns = Thing.GetColumns()
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");
            Assert.AreEqual(data.First().Id, response.Payload.First().First(), "Same first item");

            Assert.AreEqual(dataSource.Count(x => x.Name.Contains(SearchText)), response.FilteredRecordCount,
                "Total filtered rows matching");
        }

        [Test]
        public void TakeAll()
        {
            var dataSource = SampleEntities.GenerateData().AsQueryable();
            var data = dataSource.ToList();

            var request = new GridDataRequest
            {
                Take = -1,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumns()
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");
            Assert.AreEqual(data.First().Id, response.Payload.First().First(), "Same first item");

            Assert.AreEqual(dataSource.Count(), response.TotalRecordCount, "Total rows matching");
        }

        [Test]
        public void TakeNone()
        {
            var dataSource = SampleEntities.GenerateData().AsQueryable();
            var data = dataSource.ToList();

            var request = new GridDataRequest
            {
                Take = 0,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumns()
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(request.Take, response.Payload.Count, "Same items requested");
            Assert.AreEqual(data.Count, response.TotalRecordCount, "Total rows matching");
        }

        [Test]
        public void TestListSimpleSearch()
        {
            var dataSource = new List<Thing>
            {
                new Thing {Name = SearchText + "1"},
                new Thing {Name = SearchText.ToLower() + "2"},
                new Thing {Name = SearchText.ToUpper() + "3"},
                new Thing {Name = SearchText + "4"},
                new Thing {Name = "ODOR"}
            };

            var data =
                dataSource.Where(x => x.Name.ToLowerInvariant().Contains(SearchText.ToLowerInvariant()))
                    .Take(PageSize)
                    .ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 30,
                Search = new Filter
                {
                    Operator = CompareOperators.Auto,
                    Text = SearchText
                },
                Columns = Thing.GetColumns()
            };


            var response = request.CreateGridDataResponse(dataSource.AsQueryable());

            Assert.AreEqual(dataSource.Count, response.TotalRecordCount, "Same length");
            Assert.AreEqual(data.First().Id, response.Payload.First().First(), "Same first item");
            Assert.AreEqual(data.Count, response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void TestSimpleSearch2()
        {
            var dataSource = new List<Thing>();

            for (var i = 0; i < 422; i++)
            {
                dataSource.Add(new Thing { Color = "red" });
                dataSource.Add(new Thing { Color = "blue" });
                dataSource.Add(new Thing { Color = "yellow" });
            }

            var columns = new[]
            {
                new GridColumn
                {
                    Name = "Color",
                    Sortable = true,
                    Searchable = true,
                    DataType = DataType.String,
                    SortOrder = 2
                },
                new GridColumn {Name = "Id"}
            };
            var request = new GridDataRequest
            {
                Columns = columns,
                TimezoneOffset = 300,
                Take = 100,
                Skip = 300,
                Search = new Filter
                {
                    Operator = CompareOperators.Auto,
                    Text = "red"
                }

            };
            var response = request.CreateGridDataResponse(dataSource.AsQueryable());
            Assert.AreEqual(4, response.CurrentPage);
        }

        [Test]
        public void TestArraySimpleSearch()
        {
            var dataSource = new[]
            {
                new Thing {Name = SearchText + "1"},
                new Thing {Name = SearchText.ToLower() + "2"},
                new Thing {Name = SearchText.ToUpper() + "3"},
                new Thing {Name = SearchText + "4"},
                new Thing {Name = "ODOR"}
            };

            var data =
                dataSource.Where(x => x.Name.ToLowerInvariant().Contains(SearchText.ToLowerInvariant()))
                    .Take(PageSize)
                    .ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter
                {
                    Operator = CompareOperators.Auto,
                    Text = SearchText
                },
                Columns = Thing.GetColumns()
            };

            var response = request.CreateGridDataResponse(dataSource.AsQueryable());

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");
            Assert.AreEqual(data.First().Id, response.Payload.First().First(), "Same first item");

            Assert.AreEqual(dataSource.Count(x => x.Name.ToLowerInvariant().Contains(SearchText.ToLowerInvariant())),
                response.FilteredRecordCount, "Total filtered rows matching");
        }

        [Test]
        public void TestSimpleAggregate()
        {
            var dataSource = SampleEntities.GenerateData().AsQueryable();
            var data = dataSource.Take(PageSize).ToList();

            Assert.AreEqual(PageSize, data.Count, "Set has 10 items");

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithAggregate()
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");
            Assert.AreEqual(data.First().Id, response.Payload.First().First(), "Same first item");

            Assert.AreEqual(dataSource.Sum(x => x.Number), response.AggregationPayload["Number"],
                "Same average number");
            Assert.AreEqual(dataSource.Sum(x => x.DecimalNumber), (decimal)response.AggregationPayload["DecimalNumber"],
                "Same average decimal number");
            Assert.AreEqual(dataSource.Max(x => x.Name), response.AggregationPayload["Name"],
                "Same max name");
            Assert.That(dataSource.Min(x => x.Date),
                Is.EqualTo(response.AggregationPayload["Date"]).Within(10).Seconds,
                "Same min date");
        }

        [Test]
        public void TestMultipleAggregate()
        {
            var dataSource = SampleEntities.GenerateData().AsQueryable();
            var data = dataSource.Take(PageSize).ToList();

            Assert.AreEqual(PageSize, data.Count, "Set has 10 items");

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithMultipleCounts()
            };

            var response = request.CreateGridDataResponse(dataSource);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");
            Assert.AreEqual(data.First().Id, response.Payload.First().First(), "Same first item");

            Assert.AreEqual(dataSource.Select(x => x.Id).Distinct().Count(), (int)response.AggregationPayload["Id"],
                "Id same distinct count");
            Assert.AreEqual(dataSource.Select(x => x.Number).Distinct().Count(),
                (int)response.AggregationPayload["Number"], "Number same distinct count");
            Assert.AreEqual(dataSource.Select(x => x.DecimalNumber).Distinct().Count(),
                (int)response.AggregationPayload["DecimalNumber"], "DecimalNumber same distinct count");
            Assert.AreEqual(dataSource.Select(x => x.Name).Distinct().Count(), (int)response.AggregationPayload["Name"],
                "Name same distinct count");
            Assert.AreEqual(dataSource.Select(x => x.Date).Distinct().Count(), (int)response.AggregationPayload["Date"],
                "Date same distinct count");
            Assert.AreEqual(dataSource.Select(x => x.IsShipped).Distinct().Count(), (int)response.AggregationPayload["IsShipped"],
                "IsShipped same distinct count");
        }

        private class MyDateClass
        {
            public DateTime Date { get; set; }

            public DateTime? NullableDate { get; set; }
        }

        [Test]
        public void NullableDateAdjustTimeZone()
        {
            const int offset = 30;
            var now = DateTime.Now;
            var date = new MyDateClass { Date = now, NullableDate = now };
            var actual = (MyDateClass)date.AdjustTimeZone(offset);

            Assert.AreEqual(now.AddMinutes(-offset), actual.Date, "Non-nullable date adjusted");
            Assert.IsNotNull(actual.NullableDate);
            Assert.AreEqual(now.AddMinutes(-offset), actual.NullableDate.Value, "Nullable date with value adjusted");

            date = new MyDateClass { Date = now, NullableDate = null };
            actual = (MyDateClass)date.AdjustTimeZone(offset);

            Assert.IsNull(actual.NullableDate, "Nullable date adjusted");
        }

        private static IQueryable FormatOutput(IQueryable q)
        {
            var list = new List<Thing>();

            foreach (var i in q)
            {
                var item = i as Thing;

                if (item?.Color == "blue")
                    item.Color = "darkblue";

                list.Add(item);
            }

            return list.AsQueryable();
        }

        [Test]
        public void ProcessResponseSubsetTest()
        {
            const string filter = "blue";
            var dataSource = SampleEntities.GenerateData().AsQueryable();
            var filterCount = dataSource.Where(x => x.Color == filter);
            var data = filterCount.Take(PageSize).ToList();

            var request = new GridDataRequest
            {
                Take = PageSize,
                Skip = 0,
                Search = new Filter(),
                Columns = Thing.GetColumnsWithColorFilter(filter, CompareOperators.Equals)
            };

            var response = request.CreateGridDataResponse(dataSource, FormatOutput);

            Assert.AreEqual(data.Count, response.Payload.Count, "Same length");

            foreach (var item in response.Payload)
                Assert.AreNotEqual(filter, item[4], "Different color");

            foreach (var item in response.Payload)
                Assert.AreEqual("darkblue", item[4], "Same color");

        }
    }
}