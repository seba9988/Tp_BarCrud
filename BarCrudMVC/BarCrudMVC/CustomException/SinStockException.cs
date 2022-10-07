using System.Runtime.Serialization;

namespace BarCrudMVC.CustomException
{
    [Serializable()]
    public class SinStockException: Exception
    {
        public SinStockException() { }
        public SinStockException(string message) : base(message) { }
        public SinStockException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
        protected SinStockException(SerializationInfo info,
           StreamingContext context) : base(info, context)
        {
        }
    }
}
    