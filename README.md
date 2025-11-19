# Ganho de Capital - Capital Gains Tax Calculator

Aplicação de linha de comando (CLI) que calcula o imposto a ser pago sobre lucros ou prejuízos de operações no mercado financeiro de ações.

## Descrição

O programa recebe operações de compra e venda de ações através da entrada padrão (stdin) em formato JSON e retorna os valores de imposto calculados através da saída padrão (stdout).

### Regras de Negócio

- **Imposto sobre Lucro**: 20% sobre o lucro obtido na operação de venda
- **Preço Médio Ponderado**: Calculado automaticamente a cada compra usando a fórmula:
  ```
  nova-media-ponderada = ((quantidade-atual * media-atual) + (quantidade-comprada * preco-compra)) / (quantidade-atual + quantidade-comprada)
  ```
- **Prejuízos**: São acumulados e deduzidos dos lucros futuros
- **Isenção**: Operações com valor total ≤ R$ 20.000,00 não pagam imposto, mas prejuízos ainda são acumulados
- **Arredondamento**: Valores são arredondados para 2 casas decimais

## Estrutura do Projeto

O projeto segue uma arquitetura em camadas com separação clara de responsabilidades:

```
cgc/
├── src/
│   ├── cgc.core/                            # Camada de domínio/lógica de negócio
|   |   └── Application/ 
|   |       ├── CapitalGainsCalculator.cs    # Lógica principal de cálculo
│   │   └── Entities/
│   │       ├── CalculatorState.cs           # Entidade de armazenamento de estado
│   │       ├── TaxResult.cs                 # Resultado do cálculo de imposto
│   │       ├── Transaction.cs               # Entidade de transação
│   │   └── Factory/
│   │       ├── TransactionFactory.cs        # Entidade concreta de criação
│   │   └── Interfaces/
│   │       ├── ITransactionStrategy.cs      # Interface de transação
│   │   └── Services/
│   │       ├── BuyTransactionStrategy.cs    # Processamento de compra
│   │       ├── SellTransactionStrategy.cs   # Processamento de venda
│   └── cgc.console/                         # Aplicação de linha de comando
│       └── Program.cs                       # Ponto de entrada da aplicação
└── tests/
    └── cgc.tests/                           # Testes unitários e de integração
        └── CaseTests.cs
```

## Decisões Técnicas e Arquiteturais

### 1. Separação de Responsabilidades

O projeto está dividido em duas camadas principais:

- **cgc.core**: Contém toda a lógica de negócio relacionada ao cálculo de ganhos de capital. Esta camada é independente e pode ser facilmente testada e reutilizada.
- **cgc.console**: Aplicação de linha de comando que utiliza a camada core. Responsável apenas por I/O (entrada/saída).

### 2. Tipos de Dados

- **decimal**: Utilizado para todos os cálculos monetários para garantir precisão e evitar problemas de arredondamento com tipos ponto flutuante.

### 3. Tratamento de Estado

O estado da aplicação (posição atual, média ponderada, prejuízos acumulados) é gerenciado em memória pela classe `CapitalGainsCalculator`. Cada linha de entrada representa uma simulação independente, garantindo que o estado não seja compartilhado entre diferentes execuções.

### 4. Testes

O projeto inclui testes unitários para validar a lógica de negócio e testes de integração que validam todos os casos de teste fornecidos na especificação, garantindo que a aplicação funciona corretamente de ponta a ponta.

## Pré-requisitos

- .NET 8.0 SDK ou superior
- Sistema operacional Unix ou macOS (ou Windows com WSL)

## Como Compilar e Executar

### Compilar o Projeto

```bash
dotnet build
```

### Executar a Aplicação

```bash
dotnet run --project src/cgc.console/cgc.console.csproj
```

Ou após compilar:

```bash
cd src/cgc.console/bin/Debug/net8.0
./cgc.console
```

### Exemplo de Uso

A aplicação lê da entrada padrão. Você pode fornecer entrada de duas formas:

**Usando pipe:**
```bash
echo '[{"operation":"buy","unit-cost":10.00,"quantity":10000},{"operation":"sell","unit-cost":20.00,"quantity":5000}]' | dotnet run --project src/cgc.console/cgc.console.csproj
```

**Usando redirecionamento de arquivo:**
```bash
dotnet run --project src/cgc.console/cgc.console.csproj < input.txt
```

**Entrada múltipla (uma linha por simulação):**
```bash
cat <<EOF | dotnet run --project src/cgc.console/cgc.console.csproj
[{"operation":"buy","unit-cost":10.00,"quantity":10000},{"operation":"sell","unit-cost":20.00,"quantity":5000}]
[{"operation":"buy","unit-cost":10.00,"quantity":100},{"operation":"sell","unit-cost":15.00,"quantity":50}]
EOF
```

## Como Executar os Testes

### Executar Todos os Testes

```bash
dotnet test
```

### Executar com Detalhes

```bash
dotnet test --verbosity normal
```

### Executar com Cobertura de Código

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Cobertura de Testes

O projeto inclui testes que cobrem:

1. **Testes de Integração** (`CaseTests.cs`):
   - Todos os casos de teste fornecidos na especificação (Casos #1 a #9)
   - Validação end-to-end desde a entrada JSON até a saída JSON

## Bibliotecas Utilizadas

O projeto utiliza apenas bibliotecas padrão do .NET e as seguintes dependências:

- **System.Text.Json**: Para serialização/deserialização JSON (incluído no .NET)
- **xUnit**: Framework de testes (apenas para projeto de testes)
- **Microsoft.NET.Test.Sdk**: SDK de testes (apenas para projeto de testes)

## Notas Adicionais

### Validação de Entrada

Conforme especificado, assume-se que a entrada JSON está sempre bem formatada e não contém erros. A aplicação não realiza validação extensiva de entrada, mas trata casos básicos como linhas vazias ou listas vazias.

### Performance

A aplicação processa transações sequencialmente em memória, sem necessidade de banco de dados ou persistência.

### Tratamento de Erros

Embora a especificação indique que não haverá entradas inválidas, o código trata graciosamente casos como:
- Linhas vazias
- Listas vazias
- Operações desconhecidas (retorna imposto zero)

## Construção e Execução em Container (Docker)

Para facilitar a execução em diferentes ambientes, é possível criar uma imagem Docker:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet build -c Release

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/src/cgc.console/bin/Release/net8.0 .
ENTRYPOINT ["dotnet", "cgc.console.dll"]
```

Para construir e executar:
```bash
docker build -t cgc .
echo '[{"operation":"buy","unit-cost":10.00,"quantity":10000},{"operation":"sell","unit-cost":20.00,"quantity":5000}]' | docker run -i cgc
```

