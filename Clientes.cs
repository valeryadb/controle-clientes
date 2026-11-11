namespace ControleClientes
{
    /// <summary>
    /// Classe base (abstração) com os dados comuns a todos os clientes.
    /// Pessoa_Fisica e Pessoa_Juridica herdam desta classe.
    /// </summary>
    public abstract class Clientes
    {
        private string nome = "";
        private string endereco = "";

        // Atributos protegidos (#): só a própria classe e as classes filhas acessam
        protected float valor;
        protected float valor_imposto;
        protected float total;

        // Encapsulamento: a validação acontece dentro da propriedade
        public string Nome
        {
            get => nome;
            set => nome = Validacao.TextoObrigatorio(value, "Nome");
        }

        public string Endereco
        {
            get => endereco;
            set => endereco = Validacao.TextoObrigatorio(value, "Endereço");
        }

        public float Valor => valor;
        public float Valor_Imposto => valor_imposto;
        public float Total => total;

        /// <summary>Percentual de imposto do cliente. Cada tipo de pessoa define o seu.</summary>
        public virtual float PercentualImposto => 10f;

        /// <summary>
        /// Calcula o imposto sobre o valor informado e o total a pagar.
        /// É "virtual" para que as classes filhas possam alterar o cálculo (polimorfismo).
        /// </summary>
        public virtual void Pagar_Imposto(float v)
        {
            if (v <= 0)
                throw new ArgumentException("O valor deve ser maior que zero.");

            valor = v;
            valor_imposto = valor * PercentualImposto / 100;
            total = valor + valor_imposto;
        }

        /// <summary>Restaura os valores lidos do arquivo.</summary>
        protected void CarregarValores(float v, float imposto, float tot)
        {
            valor = v;
            valor_imposto = imposto;
            total = tot;
        }

        public abstract string Tipo { get; }

        /// <summary>Exibe os dados do cliente. Cada filha mostra seus próprios campos.</summary>
        public virtual string Exibir()
        {
            string impostos = valor > 0
                ? $"Valor: {valor:C} | Imposto ({PercentualImposto}%): {valor_imposto:C} | Total: {total:C}"
                : "Imposto ainda não pago";
            return $"Nome: {Nome} | Endereço: {Endereco}\n   {impostos}";
        }

        /// <summary>Converte o cliente em uma linha de texto para gravar no arquivo.</summary>
        public abstract string ParaLinhaArquivo();
    }
}
