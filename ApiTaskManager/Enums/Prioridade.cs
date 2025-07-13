using System.ComponentModel;

namespace ApiTaskManager.Enums;

public enum Prioridade
{
    [Description("Baixa")]
    Baixa = 1,
    [Description("Média")]
    Media = 2,
    [Description("Alta")]
    Alta = 3
}
