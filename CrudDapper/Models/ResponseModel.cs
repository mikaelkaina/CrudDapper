namespace CrudDapper.Models
{
    public class ResponseModel <M>
    {
        public M? Dados { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}
