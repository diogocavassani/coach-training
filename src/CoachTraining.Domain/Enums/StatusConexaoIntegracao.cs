namespace CoachTraining.Domain.Enums;

public enum StatusConexaoIntegracao
{
    NaoConectado = 0,
    Conectado = 1,
    ErroAutorizacao = 2,
    RequerReconexao = 3,
    Desconectado = 4
}
