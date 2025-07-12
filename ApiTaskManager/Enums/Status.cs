using System.ComponentModel;

namespace ApiTaskManager.Enums;

public enum Status
{
    [Description("Não Iniciado")]
    NaoIniciado = 0,
    [Description("Pendente")]
    Pendente = 1,
    [Description("Em Andamento")]
    EmAndamento = 2,
    [Description("Concluida")]
    Concluida = 3,
    [Description("Cancelado")]
    Cancelado = 4,
}
