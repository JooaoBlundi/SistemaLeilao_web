namespace SistemaLeilao_web.Models
{   
    public class UsuarioModel
    {
        public long Id { get; set; }
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public string? Cpf { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Sexo { get; set; }
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? Agencia { get; set; }
        public string? Conta { get; set; }
        public string? ChavePix { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
