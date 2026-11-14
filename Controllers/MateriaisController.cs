using Microsoft.AspNetCore.Mvc;
using MinhaBiblioteca.Models;
using MinhaBiblioteca.Repositories;

namespace MinhaBiblioteca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriaisController : ControllerBase
    {
        private readonly IMaterialRepository _repository;

        public MateriaisController(IMaterialRepository repository)
        {
            _repository = repository;
        }

        private string? ValidarMaterial(MaterialBiblioteca material)
        {
            if (string.IsNullOrWhiteSpace(material.Titulo))
                return "O título do material não pode estar vazio.";

            if (material.AnoPublicacao < 0)
                return "O ano de publicação não pode ser menor que 0.";

            return null;
        }

        [HttpGet]
        public IActionResult GetTodos()
        {
            return Ok(_repository.ObterTodos());
        }

        [HttpGet("{id}")]
        public IActionResult GetPorId(int id)
        {
            var material = _repository.ObterPorId(id);
            if (material == null) return NotFound();

            return Ok(material);
        }

        [HttpPost("livro")]
        public IActionResult CadastrarLivro([FromBody] Livro livro)
        {
            var erroMaterial = ValidarMaterial(livro);
            if (erroMaterial != null) return BadRequest(erroMaterial);

            if (string.IsNullOrWhiteSpace(livro.Autor))
                return BadRequest("Autor do livro não pode estar vazio.");

            if (livro.Paginas <= 0)
                return BadRequest("O número de páginas deve ser maior que 0.");

            var novoLivro = _repository.Adicionar(livro);
            
            return CreatedAtAction(nameof(GetPorId), new { Id = novoLivro.Id }, novoLivro);
        }

        [HttpPost("revista")]
        public IActionResult CadastrarRevista([FromBody] Revista revista)
        {
            var erroMaterial = ValidarMaterial(revista);
            if (erroMaterial != null) return BadRequest(erroMaterial);

            if (revista.Edicao <= 0)
                return BadRequest("O número da edição deve ser maior que 0.");
            if (revista.MesPublicacao > 12 || revista.MesPublicacao < 1)
                return BadRequest("O mês da publicação deve estar entre 1 e 12.");

            var novaRevista = _repository.Adicionar(revista);
            return CreatedAtAction(nameof(GetPorId), new { Id = novaRevista.Id }, novaRevista);
        }

        [HttpPut("livro/{id}")]
        public IActionResult AtualizarLivro(int id, [FromBody] Livro livro)
        {
            var erroMaterial = ValidarMaterial(livro);

            if (erroMaterial != null) return BadRequest(erroMaterial);

            if (string.IsNullOrWhiteSpace(livro.Autor))
                return BadRequest("O autor do livro não pode estar vazio.");

            if (livro.Paginas <= 0)
                return BadRequest("O número de páginas deve ser maior que 0.");

            var livroAtualizado = _repository.Atualizar(id, livro);
            if (!livroAtualizado) return NotFound();

            return Ok(livro);
        }

        [HttpPut("revista/{id}")]
        public IActionResult AtualizarRevista(int id, [FromBody] Revista revista)
        {
            var erroMaterial = ValidarMaterial(revista);

            if (erroMaterial != null) return BadRequest(erroMaterial);

            if (revista.Edicao <= 0)
                return BadRequest("O número da edição deve ser maior que 0.");
            if (revista.MesPublicacao > 12 || revista.MesPublicacao < 1)
                return BadRequest("O mês da publicação deve estar entre 1 e 12.");

            var revistaAtualizado = _repository.Atualizar(id, revista);
            if (!revistaAtualizado) return NotFound();

            return Ok(revista);
        }

        [HttpDelete("{id}")]
        public IActionResult ExcluirMaterial(int id)
        {
            var removido = _repository.Remover(id);
            if (!removido) return NotFound();
                        
            return NoContent(); 
        }

        [HttpGet("{id}/descricao")]
        public IActionResult MostrarDescricao(int id)
        {
            var material = _repository.ObterPorId(id);
            if (material == null) return NotFound();
            
            var resposta = new
            {
                id = material.Id,
                tipo = material.GetType().Name,
                descricao = material.Descricao(),
                prazoEmprestimo = material.CalcularPrazo()
            };

            return Ok(resposta);
        }

        [HttpPost("{id}/emprestar")]
        public IActionResult EmprestarMaterial(int id)
        {
            var material = _repository.ObterPorId(id);
            if (material == null) return NotFound("Material não encontrado.");

            if (material is not IEmprestimo itemEmprestavel)
            {
                return BadRequest("Este material não possui a capacidade de ser emprestado."); 
            }
            
            itemEmprestavel.Emprestar();

            return Ok($"O livro '{material.Titulo}' pode ser emprestado por {material.CalcularPrazo()} dias.");
        }
    }
}