using SControl.Business.Models;

namespace SControl.Business.Intefaces;

public interface ICursoService : IDisposable
{
    Task Adicionar(Curso cursp);
    Task Atualizar(Curso curso);
    Task Remover(Guid id);
}