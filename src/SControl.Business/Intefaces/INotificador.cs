using SControl.Business.Notificacoes;

namespace SControl.Business.Intefaces;

public interface INotificador
{
    bool TemNotificacao();
    List<Notificacao> ObterNotificacoes();
    void Handle(Notificacao notificacao);
}