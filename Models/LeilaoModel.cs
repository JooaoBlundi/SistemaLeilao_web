using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Required for IFormFile
using System.Collections.Generic;

namespace SistemaLeilao_web.Models
{
    public class LeilaoModel
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preço inicial é obrigatório")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço inicial deve ser positivo")]
        public decimal PrecoInicial { get; set; }

        // For handling file uploads from the form
        [Required(ErrorMessage = "Imagens são obrigatórias")]
        [MinLength(3, ErrorMessage = "É necessário no mínimo 3 imagens")]
        public List<IFormFile> Imagens { get; set; } = new List<IFormFile>();

        // Optional fields (might be set by API or have defaults)
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        // Properties to hold response/status from API if needed
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}

