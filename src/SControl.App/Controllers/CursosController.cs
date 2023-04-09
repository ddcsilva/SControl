using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SControl.App.Extensions;
using SControl.App.ViewModels;
using SControl.Business.Intefaces;
using SControl.Business.Models;

namespace SControl.App.Controllers
{
    [Authorize]
    public class CursosController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CursosController> _logger;
        private readonly ICursoRepository _cursoRepository;
        private readonly ICursoService _cursoService;

        public CursosController(IMapper mapper,
                                ICursoRepository cursoRepository,
                                ICursoService cursoService,
                                INotificador notificador,
                                ILogger<CursosController> logger) : base(notificador)
        {
            _mapper = mapper;
            _cursoRepository = cursoRepository;
            _cursoService = cursoService;
            _logger = logger;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var cursos = await _cursoRepository.ObterTodos();

            if (!cursos.Any())
            {
                _logger.LogInformation("A lista de cursos está vazia.");
            }

            var cursosViewModel = _mapper.Map<IEnumerable<CursoViewModel>>(cursos);

            return View(cursosViewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var cursoViewModel = await ObterCurso(id);

            if (cursoViewModel == null)
            {
                return NotFound();
            }

            return View(cursoViewModel);
        }

        [ClaimsAuthorize("Curso", "Adicionar")]
        [Route("novo-curso")]
        public IActionResult Create()
        {
            return View();
        }

        [ClaimsAuthorize("Curso", "Adicionar")]
        [Route("novo-curso")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CursoViewModel cursoViewModel)
        {
            if (!ModelState.IsValid) return View(cursoViewModel);

            var curso = _mapper.Map<Curso>(cursoViewModel);
            await _cursoService.Adicionar(curso);

            if (!OperacaoValida()) return View(cursoViewModel);

            return RedirectToAction("Index");
        }

        [ClaimsAuthorize("Curso", "Editar")]
        [Route("editar-curso/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var cursoViewModel = await ObterCurso(id);

            if (cursoViewModel == null)
            {
                return NotFound();
            }

            return View(cursoViewModel);
        }

        [ClaimsAuthorize("Curso", "Editar")]
        [Route("editar-curso/{id:guid}")]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CursoViewModel cursoViewModel)
        {
            if (id != cursoViewModel.Id) return NotFound();

            if (!ModelState.IsValid) return View(cursoViewModel);

            var curso = _mapper.Map<Curso>(cursoViewModel);
            await _cursoService.Atualizar(curso);

            if (!OperacaoValida()) return View(await ObterCurso(id));

            return RedirectToAction("Index");
        }

        [ClaimsAuthorize("Curso", "Excluir")]
        [Route("excluir-curso/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cursoViewModel = await ObterCurso(id);

            if (cursoViewModel == null)
            {
                return NotFound();
            }

            return View(cursoViewModel);
        }

        [ClaimsAuthorize("Curso", "Excluir")]
        [Route("excluir-curso/{id:guid}")]
        [HttpDelete, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var curso = await ObterCurso(id);

            if (curso == null) return NotFound();

            await _cursoService.Remover(id);

            if (!OperacaoValida()) return View(curso);

            return RedirectToAction("Index");
        }

        private async Task<CursoViewModel> ObterCurso(Guid id)
        {
            var curso = await _cursoRepository.ObterPorId(id);
            return _mapper.Map<CursoViewModel>(curso);
        }
    }
}
