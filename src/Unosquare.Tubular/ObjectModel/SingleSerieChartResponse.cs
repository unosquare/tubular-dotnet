namespace Unosquare.Tubular.ObjectModel
{
    /// <summary>
    /// Creates a single-serie chart reponse.
    /// </summary>
    /// <typeparam name="T">The type of numeric data.</typeparam>
    public class SingleSerieChartResponse<T>
    {
        /// <summary>
        /// The chart response data.
        /// </summary>
        public T[] Data { get; set; }

        /// <summary>
        /// The chart labels.
        /// </summary>
        public string[] Labels { get; set; }

        /// <summary>
        /// The serie name.
        /// </summary>
        public string SerieName { get; set; }
    }
}
