// Clase utilizada para devolver errores con un formato estándar definido por el TP

namespace Products.API.Models
{
    public class ApiError
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Detail { get; set; }
        public string Instance { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public ApiError()
        {
            Type = "";
            Title = "";
            Detail = "";
            Instance = "";
            ErrorCode = "";
            ErrorMessage = "";
        }
    }
}