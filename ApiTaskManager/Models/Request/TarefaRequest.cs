﻿using ApiTaskManager.Enums;

namespace ApiTaskManager.Models.Request
{
    public class TarefaRequest
    {
        public required string Titulo { get; set; }
        public required string Descricao { get; set; }
        public DateTime DataDeVencimento { get; set; }
        public required string UsuarioResponsavel { get; set; }
        public Prioridade Prioridade { get; set; }
        public string CriadoPor { get; set; }
    }
}
