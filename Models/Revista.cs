namespace MinhaBiblioteca.Models
{
    public class Revista : MaterialBiblioteca
    {
        public int Edicao { get; set; }
        public byte MesPublicacao { get; set; }

        public override int CalcularPrazo()
        {
            return 7;
        }

        public override string Descricao()
        {
            return $"Título: {Titulo} | Publicação: {MesPublicacao}/{AnoPublicacao} | Edição: {Edicao}.";
        }
    }
}