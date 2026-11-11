using System.Globalization;

namespace ControleClientes
{
    /// <summary>Cliente pessoa jurídica: herda de Clientes e acrescenta CNPJ e Inscrição Estadual (IE).</summary>
    public class Pessoa_Juridica : Clientes
    {
        private string cnpj = "";
        private string ie = "";

        public string Cnpj
        {
            get => cnpj;
            set => cnpj = Validacao.Cnpj(value);
        }

        public string Ie
        {
            get => ie;
            set => ie = Validacao.Ie(value);
        }

        public override string Tipo => "Pessoa Jurídica";

        // Polimorfismo: pessoa jurídica paga um percentual de imposto diferente (20%)
        public override float PercentualImposto => 20f;

        public override void Pagar_Imposto(float v)
        {
            if (v <= 0)
                throw new ArgumentException("O valor deve ser maior que zero.");

            valor = v;
            valor_imposto = valor * PercentualImposto / 100; // 20%
            total = valor + valor_imposto;
        }

        public override string Exibir()
        {
            return $"[{Tipo}] CNPJ: {Validacao.FormatarCnpj(Cnpj)} | IE: {Ie}\n   " + base.Exibir();
        }

        public override string ParaLinhaArquivo()
        {
            var inv = CultureInfo.InvariantCulture;
            return string.Join(";", "PJ", Nome, Endereco, Cnpj, Ie,
                Valor.ToString(inv), Valor_Imposto.ToString(inv), Total.ToString(inv));
        }

        public static Pessoa_Juridica DaLinhaArquivo(string[] c)
        {
            var inv = CultureInfo.InvariantCulture;
            var pj = new Pessoa_Juridica { Nome = c[1], Endereco = c[2], Cnpj = c[3], Ie = c[4] };
            pj.CarregarValores(float.Parse(c[5], inv), float.Parse(c[6], inv), float.Parse(c[7], inv));
            return pj;
        }
    }
}
