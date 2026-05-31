// Errores personalizados

namespace Products.API.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(string message)
            : base(message)
        {
        }
    }
}