using SControl.Business.Intefaces;
using SControl.Business.Models;
using SControl.Data.Context;

namespace SControl.Data.Repository;

public class CursoRepository : Repository<Curso>, ICursoRepository
{
    public CursoRepository(MeuDbContext context) : base(context) { }
}