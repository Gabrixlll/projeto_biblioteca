using MinhaBiblioteca.Models;

namespace MinhaBiblioteca.Repositories
{
    public interface IMaterialRepository
    {
        IEnumerable<MaterialBiblioteca> ObterTodos();
        MaterialBiblioteca? ObterPorId(int Id);
        MaterialBiblioteca Adicionar(MaterialBiblioteca material);
        bool Atualizar(int Id, MaterialBiblioteca material);
        bool Remover (int Id);
    }
}