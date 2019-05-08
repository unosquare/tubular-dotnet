namespace Unosquare.Tubular.ObjectModel
{
    /// <summary>
    /// Represents a multiple serie chart response.
    /// </summary>
    /// <typeparam name="T">The type of numeric data.</typeparam>
    public class MultipleSerieChartResponse<T>
    {
        /// <summary>
        /// The chart response data.
        /// </summary>
        public T[][] Data { get; set; }
        
        /// <summary>
        /// The chart labels.
        /// </summary>
        public string[] Labels { get; set; }

        /// <summary>
        /// The chart series.
        /// </summary>
        public string[] Series { get; set; }
    }
}