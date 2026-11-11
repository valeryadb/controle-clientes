using System.Globalization;

namespace ControleClientes
{
    /// <summary>Regras de validação dos dados digitados pelo usuário.</summary>
    public static class Validacao
    {
        public static string TextoObrigatorio(string? texto, string campo)
        {
            texto = texto?.Trim() ?? "";
            if (texto.Length == 0)
                throw new ArgumentException($"{campo} é obrigatório.");
            if (texto.Contains(';'))
                throw new ArgumentException($"{campo} não pode conter o caractere ';'.");
            if (texto.Length > 100)
                throw new ArgumentException($"{campo} pode ter no máximo 100 caracteres.");
            return texto;
        }

        public static string SomenteNumeros(string? texto) =>
            new string((texto ?? "").Where(char.IsDigit).ToArray());

        public static string Cpf(string? valor)
        {
            string cpf = SomenteNumeros(valor);
            if (cpf.Length != 11)
                throw new ArgumentException("CPF deve ter 11 números.");
            if (cpf.Distinct().Count() == 1)
                throw new ArgumentException("CPF inválido (todos os números iguais).");

            int[] pesos1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] pesos2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            if (Digito(cpf, pesos1) != cpf[9] - '0' || Digito(cpf, pesos2) != cpf[10] - '0')
                throw new ArgumentException("CPF inválido (dígitos verificadores não conferem).");
            return cpf;
        }

        public static string Cnpj(string? valor)
        {
            string cnpj = SomenteNumeros(valor);
            if (cnpj.Length != 14)
                throw new ArgumentException("CNPJ deve ter 14 números.");
            if (cnpj.Distinct().Count() == 1)
                throw new ArgumentException("CNPJ inválido (todos os números iguais).");

            int[] pesos1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] pesos2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            if (Digito(cnpj, pesos1) != cnpj[12] - '0' || Digito(cnpj, pesos2) != cnpj[13] - '0')
                throw new ArgumentException("CNPJ inválido (dígitos verificadores não conferem).");
            return cnpj;
        }

        // Cálculo do dígito verificador (módulo 11), usado por CPF e CNPJ
        private static int Digito(string numeros, int[] pesos)
        {
            int soma = 0;
            for (int i = 0; i < pesos.Length; i++)
                soma += (numeros[i] - '0') * pesos[i];
            int resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        public static string Rg(string? valor)
        {
            string rg = (valor ?? "").Trim();
            int digitos = SomenteNumeros(rg).Length;
            if (digitos < 5 || digitos > 14)
                throw new ArgumentException("RG deve ter entre 5 e 14 números.");
            return TextoObrigatorio(rg, "RG");
        }

        public static string Ie(string? valor)
        {
            string ie = (valor ?? "").Trim();
            if (ie.Equals("ISENTO", StringComparison.OrdinalIgnoreCase))
                return "ISENTO";
            int digitos = SomenteNumeros(ie).Length;
            if (digitos < 8 || digitos > 14)
                throw new ArgumentException("Inscrição Estadual deve ter entre 8 e 14 números (ou ISENTO).");
            return TextoObrigatorio(ie, "Inscrição Estadual");
        }

        public static DateTime DataNascimento(DateTime data)
        {
            if (data.Date > DateTime.Today)
                throw new ArgumentException("A data de nascimento não pode ser no futuro.");
            if (data.Date < DateTime.Today.AddYears(-130))
                throw new ArgumentException("Data de nascimento inválida (mais de 130 anos).");
            return data.Date;
        }

        public static DateTime LerData(string? texto)
        {
            if (!DateTime.TryParseExact((texto ?? "").Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime data))
                throw new ArgumentException("Data inválida. Use o formato dd/mm/aaaa (ex.: 25/03/2001).");
            return data;
        }

        public static float LerValor(string? texto)
        {
            // Aceita "1500,50", "1.500,50" ou "1500.50"
            string t = (texto ?? "").Trim().Replace("R$", "").Trim();
            if (t.Contains(','))
                t = t.Replace(".", "");      // ponto é separador de milhar
            else
                t = t.Replace('.', ',');     // ponto usado como vírgula decimal
            if (!float.TryParse(t, NumberStyles.Number, new CultureInfo("pt-BR"), out float v))
                throw new ArgumentException("Valor inválido. Digite apenas números (ex.: 1500,50).");
            if (v <= 0)
                throw new ArgumentException("O valor deve ser maior que zero.");
            if (v > 1_000_000_000)
                throw new ArgumentException("Valor muito alto.");
            return v;
        }

        public static string FormatarCpf(string c) => $"{c[..3]}.{c[3..6]}.{c[6..9]}-{c[9..]}";
        public static string FormatarCnpj(string c) => $"{c[..2]}.{c[2..5]}.{c[5..8]}/{c[8..12]}-{c[12..]}";
    }
}
