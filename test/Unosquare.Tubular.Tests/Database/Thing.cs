namespace Unosquare.Tubular.Tests.Database
{
    using ObjectModel;

    public class Thing
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Number { get; set; }

        public decimal DecimalNumber { get; set; }

        public string Category { get; set; }

        public string Color { get; set; }

        public System.DateTime Date { get; set; }

        public System.DateTime? NullableDate { get; set; }

        public bool IsShipped { get; set; }

        public static GridColumn[] GetColumnsWithAggregate()
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Number", Aggregate = AggregationFunction.Sum},
                new GridColumn {Name = "DecimalNumber", Aggregate = AggregationFunction.Sum},
                new GridColumn {Name = "Name", Aggregate = AggregationFunction.Max},
                new GridColumn {Name = "Date", Aggregate = AggregationFunction.Min}
            };
        }

        public static GridColumn[] GetColumnsWithMultipleCounts()
        {
            return new[]
            {
                new GridColumn {Name = "Id", Aggregate = AggregationFunction.DistinctCount},
                new GridColumn {Name = "Number", Aggregate = AggregationFunction.DistinctCount},
                new GridColumn {Name = "DecimalNumber", Aggregate = AggregationFunction.DistinctCount},
                new GridColumn {Name = "Name", Aggregate = AggregationFunction.DistinctCount},
                new GridColumn {Name = "Date", Aggregate = AggregationFunction.DistinctCount},
                new GridColumn {Name = "IsShipped", Aggregate = AggregationFunction.DistinctCount}
            };
        }

        public static GridColumn[] GetColumns()
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name", Searchable = true},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"}
            };
        }

        public static GridColumn[] GetColumnsWithSort()
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date", Sortable = true, SortDirection = SortDirection.Ascending, SortOrder = 1},
                new GridColumn {Name = "IsShipped"}
            };
        }

        public static GridColumn[] GetColumnsWithIdFilter(string filter, CompareOperators oper)
        {
            return new[]
            {
                new GridColumn
                {
                    Name = "Id",
                    Filter = new Filter() {Text = filter, Operator = oper},
                    DataType = Tubular.DataType.Numeric
                },
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"}
            };
        }

        public static GridColumn[] GetColumnsWithColorFilter(string filter, CompareOperators oper)
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"},
                new GridColumn
                {
                    Name = "Color",
                    Filter = new Filter() { Text = filter, Operator = oper },
                    DataType = Tubular.DataType.String
                }
            };
        }

        public static GridColumn[] GetColumnsWithBetweenFilter(string filter, string[] arguments)
        {
            return new[]
            {
                new GridColumn
                {
                    Name = "Id",
                    Filter = new Filter() {Text = filter, Argument = arguments, Operator = CompareOperators.Between},
                    DataType = Tubular.DataType.Numeric
                },
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"}
            };
        }

        public static GridColumn[] GetColumnsWithMultipleFilter(string [] arguments, CompareOperators oper)
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"},
                 new GridColumn
                {
                    Name = "Color",
                    Filter = new Filter() { Argument = arguments, Operator = oper },
                    DataType = Tubular.DataType.String
                }
            };
        }

        public static GridColumn[] GetColumnsWithDateFilter(string filter, CompareOperators oper, Tubular.DataType dataType)
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "IsShipped"},
                new GridColumn
                {
                    Name = "Date",
                    Filter = new Filter() {Text = filter, Operator = oper, },
                    DataType = dataType
                }
            };
        }

        public static GridColumn[] GetColumnsWithNumberFilter(string filter, CompareOperators oper)
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "IsShipped"},
                new GridColumn
                {
                    Name = "DecimalNumber",
                    Filter = new Filter() {Text = filter, Operator = oper},
                    DataType = Tubular.DataType.Numeric
                }
            };
        }

        public static GridColumn[] GetColumnsWithBooleanFilter(string filter, CompareOperators oper)
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {
                    Name = "IsShipped",
                    Searchable = false,
                    Filter = new Filter() { Text = filter, Operator = oper,  },
                    DataType = Tubular.DataType.Boolean,
                }
            };
        }

        public static GridColumn[] GetColumnsWithAggregateDouble(AggregationFunction aggregation)            
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Number", Aggregate = aggregation},
                new GridColumn {Name = "DecimalNumber" },
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"}
            };
        }

        public static GridColumn[] GetColumnsWithAggregateInt(AggregationFunction aggregation)
        {
            return new[]
            {
                new GridColumn {Name = "Id", Aggregate = aggregation},
                new GridColumn {Name = "Number"},
                new GridColumn {Name = "DecimalNumber" },
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"}
            };
        }

        public static GridColumn[] GetColumnsWithColorFilter(string filter, CompareOperators oper)
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "Bool"},
                new GridColumn
                {
                    Name = "Color",
                    Filter = new Filter() { Text = filter, Operator = oper },
                    DataType = Tubular.DataType.String
                }
            };
        }

        public static GridColumn[] GetColumnsWithBetweenFilter(string filter, string[] arguments)
        {
            return new[]
            {
                new GridColumn
                {
                    Name = "Id",
                    Filter = new Filter() {Text = filter, Argument = arguments, Operator = CompareOperators.Between},
                    DataType = Tubular.DataType.Numeric
                },
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "Bool"}
            };
        }

        public static GridColumn[] GetColumnsWithMultipleFilter(string [] arguments, CompareOperators oper)
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "Bool"},
                 new GridColumn
                {
                    Name = "Color",
                    Filter = new Filter() { Argument = arguments, Operator = oper },
                    DataType = Tubular.DataType.String
                }
            };
        }

        public static GridColumn[] GetColumnsWithDateFilter(string filter, CompareOperators oper, Tubular.DataType dataType)
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "Bool"},
                new GridColumn
                {
                    Name = "Date",
                    Filter = new Filter() {Text = filter, Operator = oper},
                    DataType = dataType
                }
            };
        }

        public static GridColumn[] GetColumnsWithNumberFilter(string filter, CompareOperators oper)
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"},
                new GridColumn {Name = "Bool"},
                new GridColumn
                {
                    Name = "DecimalNumber",
                    Filter = new Filter() {Text = filter, Operator = oper},
                    DataType = Tubular.DataType.Numeric
                }
            };
        }

        public static GridColumn[] GetColumnsWithAggregateDouble(AggregationFunction aggregation)            
        {
            return new[]
            {
                new GridColumn {Name = "Id"},
                new GridColumn {Name = "Number", Aggregate = aggregation},
                new GridColumn {Name = "DecimalNumber" },
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"}
            };
        }

        public static GridColumn[] GetColumnsWithAggregateInt(AggregationFunction aggregation)
        {
            return new[]
            {
                new GridColumn {Name = "Id", Aggregate = aggregation},
                new GridColumn {Name = "Number"},
                new GridColumn {Name = "DecimalNumber" },
                new GridColumn {Name = "Name"},
                new GridColumn {Name = "Date"}
            };
        }

    }
}