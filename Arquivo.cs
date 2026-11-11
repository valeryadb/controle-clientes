using System.Text;

namespace ControleClientes
{
    /// <summary>Grava e lê os clientes em arquivo de texto (um cliente por linha, campos separados por ';').</summary>
    public static class Arquivo
    {
        public const string Caminho = "clientes.txt";

        public static void Salvar(List<Clientes> clientes)
        {
            var linhas = clientes.Select(c => c.ParaLinhaArquivo());
            File.WriteAllLines(Caminho, linhas, Encoding.UTF8);
        }

        public static List<Clientes> Carregar()
        {
            var lista = new List<Clientes>();
            if (!File.Exists(Caminho))
                return lista;

            int numeroLinha = 0;
            foreach (string linha in File.ReadAllLines(Caminho, Encoding.UTF8))
            {
                numeroLinha++;
                if (string.IsNullOrWhiteSpace(linha)) continue;
                try
                {
                    string[] campos = linha.Split(';');
                    if (campos[0] == "PF" && campos.Length == 9)
                        lista.Add(Pessoa_Fisica.DaLinhaArquivo(campos));
                    else if (campos[0] == "PJ" && campos.Length == 8)
                        lista.Add(Pessoa_Juridica.DaLinhaArquivo(campos));
                    else
                        throw new FormatException("formato desconhecido");
                }
                catch (Exception ex)
                {
                    // Uma linha corrompida não impede o sistema de abrir
                    Console.WriteLine($"Aviso: linha {numeroLinha} do arquivo ignorada ({ex.Message}).");
                }
            }
            return lista;
        }
    }
}
