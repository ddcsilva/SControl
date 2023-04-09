using SControl.Business.Intefaces;
using SControl.Business.Models;
using SControl.Business.Models.Validations;

namespace SControl.Business.Services;

public class CursoService : BaseService, ICursoService
{
    private readonly ICursoRepository _cursoRepository;

    public CursoService(ICursoRepository cursoRepository, INotificador notificador) : base(notificador)
    {
        _cursoRepository = cursoRepository;
    }

    public async Task Adicionar(Curso curso)
    {
        if (!ExecutarValidacao(new CursoValidation(), curso)) return;

        if (_cursoRepository.Buscar(c => c.Nome == curso.Nome).Result.Any())
        {
            Notificar("Já existe um curso com este nome informado.");
            return;
        }

        await _cursoRepository.Adicionar(curso);
    }

    public async Task Atualizar(Curso curso)
    {
        if (!ExecutarValidacao(new CursoValidation(), curso)) return;

        if (_cursoRepository.Buscar(c => c.Nome == curso.Nome && c.Id != curso.Id).Result.Any())
        {
            Notificar("Já existe um curso com este nome informado.");
            return;
        }

        await _cursoRepository.Atualizar(curso);
    }

    public async Task Remover(Guid id)
    {
        await _cursoRepository.Remover(id);
    }

    public void Dispose()
    {
        _cursoRepository?.Dispose();
    }
}
