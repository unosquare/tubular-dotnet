namespace Unosquare.Tubular.GenericModels
{
    using System;

    /// <summary>
    /// Represents a basic generic API Result object
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the affected count.
        /// </summary>
        public int AffectedCount { get; set; }

        /// <summary>
        /// Create a new valid result with an optional message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>An ApiResult with status OK</returns>
        public static ApiResult Ok(string message = null) => new ApiResult { Status = nameof(Ok), Message = message };

        /// <summary>
        /// Create a new invalid result with an exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>An ApiResult with status Error</returns>
        public static ApiResult Error(Exception ex) => new ApiResult { Status = nameof(Error), Message = ex.Message };
    }
}