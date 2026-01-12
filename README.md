# StockQuoteAlert - Teste Técnico INOA

**StockQuoteAlert** é uma aplicação de console desenvolvida em **.NET 8** para **monitoramento em tempo real de ativos da B3**.  
O sistema consulta periodicamente a API da **Brapi**, avalia os preços de mercado com base em **alvos de compra e venda definidos pelo usuário** e dispara **alertas por e-mail** quando as condições são atingidas.

O projeto foi construído com foco em **Clean Code**, **baixo acoplamento**, **testabilidade** e **boas práticas de engenharia de software**.

---

## Funcionalidades

- **Monitoramento Periódico**
  - Consulta automática das cotações em intervalos configuráveis (padrão: 1 minuto).

- **Tomada de Decisão Inteligente**
  - Envia alertas apenas quando:
    - Preço ≥ alvo de **venda**
    - Preço ≤ alvo de **compra**

- **Notificações por E-mail**
  - Envio de alertas via **SMTP**, compatível com:
    - Qualquer servidor SMTP

- **Graceful Shutdown**
  - Suporte completo a `CancellationToken`, garantindo encerramento seguro do processo.

---

## Tech Stack & Arquitetura

- **Runtime:** .NET 8  
- **Arquitetura:** Console App com princípios de Clean Architecture  
- **Injeção de Dependência:** `Microsoft.Extensions.DependencyInjection`  
- **Logging:** Log estruturado no console  
- **HTTP Client:** `HttpClient` com abstrações para testabilidade  
- **E-mail:** `MailKit` + `MimeKit`  
- **Testes Unitários:**  
  - `xUnit`
  - `Moq`
  - Cobertura de:
    - Regras de negócio
    - Integração com API (mockada)
    - Serviço de e-mail (mockado)

---

## Configuração

### 1️° Clonar o repositório

```bash
git clone <url-do-repositorio>
cd StockQuoteAlert
```

---

### 2️° Configurar o `appsettings.json`

Crie um arquivo `appsettings.json` na raiz do projeto (ou utilize **User Secrets**) com a seguinte estrutura:

```json
{
  "BrapiSettings": {
    "ApiKey": "SUA_CHAVE_BRAPI",
    "BaseUrl": "https://brapi.dev/api/quote/"
  },
  "SmtpSettings": {
    "Server": "sandbox.smtp.mailtrap.io",
    "Port": "2525",
    "Username": "seu_usuario",
    "Password": "sua_password",
    "SenderEmail": "alert@stockmonitor.com",
    "SenderName": "Stock Monitor",
    "TargetEmail": "seu_email@destino.com"
  }
}
```

> **Importante:**  
> Nunca versionar credenciais sensíveis. Recomenda-se o uso de **User Secrets** ou variáveis de ambiente.

---

##Como Executar

No diretório raiz do projeto, execute:

```powershell
dotnet run -- <TICKER> <PRECO_VENDA> <PRECO_COMPRA>
```

### Exemplo

```powershell
dotnet run -- PETR4 35.50 22.10
```

Neste cenário:

- Um e-mail será enviado se **PETR4 ≥ R$ 35,50**
- Um e-mail será enviado se **PETR4 ≤ R$ 22,10**

---

## Executando os Testes

A aplicação possui **testes automatizados** que garantem a integridade da lógica de negócio, **sem chamadas reais à API ou envio de e-mails**.

```powershell
dotnet test
```

---

## Decisões de Projeto

- **Separação de Responsabilidades**
  - Lógica de negócio desacoplada de infraestrutura
- **Alta Testabilidade**
  - Uso extensivo de interfaces e mocks
- **Extensibilidade**
  - Fácil inclusão de novos canais de notificação (ex: SMS, WhatsApp)
- **Resiliência**
  - Controle de execução com `CancellationToken`

---

