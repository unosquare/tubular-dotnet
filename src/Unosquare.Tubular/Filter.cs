using System;

namespace Unosquare.Tubular
{
    /// <summary>
    /// Represents a Tubular filter (by column).
    /// 
    /// This object is only used to be serialized/deserialized between
    /// the API and Tubular.
    /// </summary>
    [Obsolete("This will be removed in future versions.")]
    public class Filter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Filter"/> class.
        /// </summary>
        public Filter()
        {
            Operator = CompareOperators.None;
            Name = string.Empty;
            Text = string.Empty;
        }

        /// <summary>
        /// Filter name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Filter search text.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Filter search params.
        /// </summary>
        public string[]? Argument { get; set; }

        /// <summary>
        /// Filter's operator.
        /// </summary>
        public CompareOperators Operator { get; set; }

        /// <summary>
        /// Flags if the Filter is applied.
        /// </summary>
        public bool HasFilter { get; set; }
    }
}