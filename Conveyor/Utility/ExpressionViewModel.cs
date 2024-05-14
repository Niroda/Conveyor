namespace Conveyor.Utility
{
    /// <summary>
    /// A view model to be passed to the server that holds the encrypted expression
    /// </summary>
    public class ExpressionViewModel
    {
        /// <summary>
        /// Encrypted expression that is going to be passed to the server ..
        /// </summary>
        public string Expression { get; set; }
    }
}
