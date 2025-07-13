using System.ComponentModel;

namespace ApiTaskManager.Enums;

public enum Status
{
    [Description("Pendente")]
    Pendente = 1,
    [Description("Em Andamento")]
    EmAndamento = 2,
    [Description("Concluida")]
    Concluida = 3,
}
