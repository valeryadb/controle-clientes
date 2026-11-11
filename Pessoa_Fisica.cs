using System.Globalization;

namespace ControleClientes
{
    /// <summary>Cliente pessoa física: herda de Clientes e acrescenta CPF, RG e data de nascimento.</summary>
    public class Pessoa_Fisica : Clientes
    {
        private string cpf = "";
        private string rg = "";
        private DateTime dataNascimento;

        public string Cpf
        {
            get => cpf;
            set => cpf = Validacao.Cpf(value);
        }

        public string Rg
        {
            get => rg;
            set => rg = Validacao.Rg(value);
        }

        public DateTime DataNascimento
        {
            get => dataNascimento;
            set => dataNascimento = Validacao.DataNascimento(value);
        }

        public override string Tipo => "Pessoa Física";

        // Pessoa física usa o cálculo padrão da classe Clientes (10%).

        public override string Exibir()
        {
            return $"[{Tipo}] CPF: {Validacao.FormatarCpf(Cpf)} | RG: {Rg} | Nascimento: {DataNascimento:dd/MM/yyyy}\n   " + base.Exibir();
        }

        public override string ParaLinhaArquivo()
        {
            var inv = CultureInfo.InvariantCulture;
            return string.Join(";", "PF", Nome, Endereco, Cpf, Rg, DataNascimento.ToString("dd/MM/yyyy", inv),
                Valor.ToString(inv), Valor_Imposto.ToString(inv), Total.ToString(inv));
        }

        public static Pessoa_Fisica DaLinhaArquivo(string[] c)
        {
            var inv = CultureInfo.InvariantCulture;
            var pf = new Pessoa_Fisica
            {
                Nome = c[1], Endereco = c[2], Cpf = c[3], Rg = c[4],
                DataNascimento = DateTime.ParseExact(c[5], "dd/MM/yyyy", inv)
            };
            pf.CarregarValores(float.Parse(c[6], inv), float.Parse(c[7], inv), float.Parse(c[8], inv));
            return pf;
        }
    }
}
