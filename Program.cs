using System.Globalization;
using System.Text;
using ControleClientes;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("pt-BR"); // R$ e vírgula decimal

List<Clientes> clientes = Arquivo.Carregar();
Console.WriteLine("=== ClientLab - Sistema de Controle de Clientes ===");
Console.WriteLine($"{clientes.Count} cliente(s) carregado(s) do arquivo {Arquivo.Caminho}.");

bool executando = true;
while (executando)
{
    Console.WriteLine();
    Console.WriteLine("1 - Cadastrar Pessoa Física");
    Console.WriteLine("2 - Cadastrar Pessoa Jurídica");
    Console.WriteLine("3 - Listar clientes");
    Console.WriteLine("4 - Pagar imposto");
    Console.WriteLine("5 - Excluir cliente");
    Console.WriteLine("0 - Sair");
    Console.Write("Escolha uma opção: ");
    string? opcao = Console.ReadLine();
    if (opcao == null) break; // fim da entrada

    switch (opcao.Trim())
    {
        case "1": CadastrarPessoaFisica(); break;
        case "2": CadastrarPessoaJuridica(); break;
        case "3": Listar(); break;
        case "4": PagarImposto(); break;
        case "5": Excluir(); break;
        case "0": executando = false; break;
        default: Console.WriteLine("Opção inválida! Digite um número de 0 a 5."); break;
    }
}
Console.WriteLine("Sistema encerrado.");

// ---------- Funções do menu ----------

void CadastrarPessoaFisica()
{
    Console.WriteLine("\n--- Cadastro de Pessoa Física ---");
    var pf = new Pessoa_Fisica();
    if (!Ler("Nome: ", t => pf.Nome = t)) return;
    if (!Ler("Endereço: ", t => pf.Endereco = t)) return;
    if (!Ler("CPF (somente números ou 000.000.000-00): ", t =>
        {
            string cpf = Validacao.Cpf(t);
            if (clientes.OfType<Pessoa_Fisica>().Any(c => c.Cpf == cpf))
                throw new ArgumentException("Já existe um cliente com este CPF.");
            pf.Cpf = cpf;
        })) return;
    if (!Ler("RG: ", t => pf.Rg = t)) return;
    if (!Ler("Data de nascimento (dd/mm/aaaa): ", t => pf.DataNascimento = Validacao.LerData(t))) return;

    clientes.Add(pf);
    Arquivo.Salvar(clientes);
    Console.WriteLine("Pessoa física cadastrada com sucesso!");
}

void CadastrarPessoaJuridica()
{
    Console.WriteLine("\n--- Cadastro de Pessoa Jurídica ---");
    var pj = new Pessoa_Juridica();
    if (!Ler("Nome / Razão social: ", t => pj.Nome = t)) return;
    if (!Ler("Endereço: ", t => pj.Endereco = t)) return;
    if (!Ler("CNPJ (somente números ou 00.000.000/0000-00): ", t =>
        {
            string cnpj = Validacao.Cnpj(t);
            if (clientes.OfType<Pessoa_Juridica>().Any(c => c.Cnpj == cnpj))
                throw new ArgumentException("Já existe um cliente com este CNPJ.");
            pj.Cnpj = cnpj;
        })) return;
    if (!Ler("Inscrição Estadual (ou ISENTO): ", t => pj.Ie = t)) return;

    clientes.Add(pj);
    Arquivo.Salvar(clientes);
    Console.WriteLine("Pessoa jurídica cadastrada com sucesso!");
}

void Listar()
{
    Console.WriteLine("\n--- Clientes cadastrados ---");
    if (clientes.Count == 0)
    {
        Console.WriteLine("Nenhum cliente cadastrado.");
        return;
    }
    for (int i = 0; i < clientes.Count; i++)
        Console.WriteLine($"{i + 1}. {clientes[i].Exibir()}"); // polimorfismo: cada tipo exibe seus dados
}

void PagarImposto()
{
    Clientes? cliente = EscolherCliente("pagar o imposto");
    if (cliente == null) return;

    if (!Ler($"Valor da compra/serviço de {cliente.Nome} (R$): ",
            t => cliente.Pagar_Imposto(Validacao.LerValor(t)))) return;

    Arquivo.Salvar(clientes);
    Console.WriteLine($"Imposto de {cliente.PercentualImposto}% ({cliente.Tipo}): {cliente.Valor_Imposto:C}");
    Console.WriteLine($"Total a pagar: {cliente.Total:C}");
}

void Excluir()
{
    Clientes? cliente = EscolherCliente("excluir");
    if (cliente == null) return;

    Console.Write($"Confirma a exclusão de {cliente.Nome}? (S/N): ");
    if ((Console.ReadLine() ?? "").Trim().ToUpper() == "S")
    {
        clientes.Remove(cliente);
        Arquivo.Salvar(clientes);
        Console.WriteLine("Cliente excluído.");
    }
    else
    {
        Console.WriteLine("Exclusão cancelada.");
    }
}

Clientes? EscolherCliente(string acao)
{
    Listar();
    if (clientes.Count == 0) return null;

    Clientes? escolhido = null;
    Ler($"Número do cliente para {acao}: ", t =>
    {
        if (!int.TryParse(t, out int n) || n < 1 || n > clientes.Count)
            throw new ArgumentException($"Digite um número entre 1 e {clientes.Count}.");
        escolhido = clientes[n - 1];
    });
    return escolhido;
}

// Lê um valor e repete a pergunta enquanto o dado for inválido (até 3 tentativas)
bool Ler(string pergunta, Action<string> aplicar)
{
    for (int tentativa = 1; tentativa <= 3; tentativa++)
    {
        Console.Write(pergunta);
        string? entrada = Console.ReadLine();
        if (entrada == null) return false;
        try
        {
            aplicar(entrada);
            return true;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
    Console.WriteLine("Muitas tentativas inválidas. Operação cancelada.");
    return false;
}
