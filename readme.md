# Transaction Management Service

Microsserviço para gerenciamento de transações financeiras desenvolvido em C# com ASP.NET Core, seguindo os princípios da **Clean Architecture**.

## **Status do Projeto - CONCLUÍDO ✅**

### ✅ **Funcionalidades Implementadas:**
- ✅ **Clean Architecture** - Separação clara de responsabilidades em camadas
- ✅ **API REST** - Endpoints completos para transações
- ✅ **Azure Service Bus** - Messaging assíncrono funcionando
- ✅ **MongoDB Local** - Persistência real em banco de dados local
- ✅ **Dependency Injection** - Inversão de controle configurada
- ✅ **Swagger Documentation** - Interface de teste automática
- ✅ **Testes Unitários** - Cobertura da camada de aplicação
- ✅ **Configuration Management** - Settings externalizadas

## **Arquitetura Clean Architecture**

```
TransactionManagementService/
├── 📁 Domain/                    # Entidades e interfaces de negócio
│   ├── Entities/
│   │   └── Transaction.cs
│   └── Interfaces/
│       ├── ITransactionRepository.cs
│       └── IMessageService.cs
├── 📁 Application/               # Regras de negócio e casos de uso
│   ├── DTOs/
│   │   ├── CreateTransactionDto.cs
│   │   └── TransactionDto.cs
│   └── Services/
│       ├── ITransactionService.cs
│       └── TransactionService.cs
├── 📁 Infrastructure/            # Implementações técnicas
│   ├── Configuration/
│   ├── Repositories/
│   │   └── TransactionRepository.cs
│   └── Services/
│       └── ServiceBusService.cs
├── 📁 API/                      # Controllers e configuração Web
│   ├── Controllers/
│   │   └── TransactionsController.cs
│   ├── Program.cs
│   └── appsettings.json
└── 📁 Tests/                    # Testes unitários
    └── Services/
        └── TransactionServiceTests.cs
```

## **Como Executar**

### Pré-requisitos
- .NET 6.0 ou superior
- MongoDB Community Edition (local)
- Git

### 1. Instalar MongoDB Local
```bash
# Download: https://www.mongodb.com/try/download/community
# Durante instalação, marcar: "Install MongoDB as a Service"
# Verificar se está rodando:
Get-Service MongoDB
```

### 2. Clonar e Executar
```bash
# Clone o repositório
git clone https://github.com/seu-usuario/transaction-management-service.git
cd transaction-management-service

# Execute a aplicação
dotnet run --project TransactionManagementService.API

# Acesse no navegador
http://localhost:5138
```

## **Configuração do Banco de Dados**

### **MongoDB Local:**
- **Host:** `localhost:27017`
- **Database:** `TransactionDb` (criado automaticamente)
- **Collection:** `Transactions` (criado automaticamente)
- **Dados iniciais:** 3 transações de exemplo inseridas na primeira execução

### **Logs de inicialização esperados:**
```
Tentando conectar no MongoDB: mongodb://localhost:27017
✅ MongoDB LOCAL conectado com sucesso!
📁 Database: TransactionDb
📄 Collection: Transactions
📊 Collection vazia - inserindo dados de exemplo...
✅ Inseridos 3 registros de exemplo no MongoDB
Azure Service Bus conectado com sucesso!
```

## **Endpoints da API**

### **Base URL:** `http://localhost:5138/api/transactions`

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/transactions` | Lista todas as transações |
| GET | `/api/transactions/{id}` | Busca transação por ID |
| POST | `/api/transactions` | Cria nova transação |

### **Exemplo de Uso:**

#### **GET /api/transactions**
```bash
curl -X GET "http://localhost:5138/api/transactions"
```

**Resposta:**
```json
[
  {
    "id": "64f8b1c2d4e5f6a7b8c9d0e1",
    "accountId": "ACC001",
    "amount": 1500.00,
    "type": 1,
    "description": "Salário mensal",
    "createdAt": "2025-07-23T10:30:00Z",
    "status": 1
  }
]
```

#### **POST /api/transactions**
```bash
curl -X POST "http://localhost:5138/api/transactions" \
  -H "Content-Type: application/json" \
  -d '{
    "accountId": "ACC001",
    "amount": 100.50,
    "type": 1,
    "description": "Nova transação"
  }'
```

**Resposta:**
```json
{
  "id": "64f8b1c2d4e5f6a7b8c9d0e2",
  "accountId": "ACC001",
  "amount": 100.50,
  "type": 1,
  "description": "Nova transação",
  "createdAt": "2025-07-26T14:22:00Z",
  "status": 0
}
```

## **Configurações**

### **Enums de Transação:**
- **TransactionType:** `0` = Debit, `1` = Credit
- **TransactionStatus:** `0` = Pending, `1` = Completed, `2` = Failed

### **Configuração de Serviços (appsettings.json):**
```json
{
  "DatabaseSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "TransactionDb",
    "TransactionsCollectionName": "Transactions"
  },
  "ServiceBusSettings": {
    "ConnectionString": "Endpoint=sb://transacoes-servicebus-luizaugusto.servicebus.windows.net/...",
    "QueueName": "transacoes-queue"
  }
}
```

## **Executar Testes**

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "TransactionServiceTests"
```

## **Como Funciona**

### **Fluxo de uma Transação:**
1. **Cliente** envia POST para `/api/transactions`
2. **API** valida os dados recebidos
3. **Service** processa a transação
4. **Repository** salva no **MongoDB Local**
5. **Service Bus** envia mensagem assíncrona para Azure
6. **API** retorna transação criada com ID gerado

### **Persistência de Dados:**
- **MongoDB Local** - Dados persistem entre reinicializações
- **Fallback Automático** - Se MongoDB falhar, usa dados em memória
- **Logs Detalhados** - Todas as operações são logadas

## **Monitoramento**

### **Logs da Aplicação:**
- `Tentando conectar no MongoDB: mongodb://localhost:27017`
- `MongoDB LOCAL conectado com sucesso!`
- `[MONGODB] Buscando todas as transações`
- `[MONGODB] Transação salva: [id]`
- `Azure Service Bus conectado com sucesso!`
- `Mensagem enviada para Service Bus: [messageId]`

## **Critérios de Avaliação Atendidos**

| Critério | Status | Observação |
|----------|--------|------------|
| **Funcionalidade** | ✅ | API cria e lista transações corretamente |
| **Integração** | ✅ | MongoDB Local + Azure Service Bus funcionais |
| **Arquitetura** | ✅ | Clean Architecture implementada |
| **Testes** | ✅ | Testes unitários funcionais |
| **Repositório** | ✅ | Código organizado, README claro |

## **Tecnologias Utilizadas**

- **.NET 6** - Framework principal
- **ASP.NET Core** - Web API
- **MongoDB Local** - Banco de dados NoSQL
- **Azure Service Bus** - Messaging assíncrono
- **Swagger/OpenAPI** - Documentação automática
- **xUnit** - Framework de testes
- **Moq** - Mocking para testes
- **FluentAssertions** - Assertions expressivas

## **Visualizar Dados (Opcional)**

### **MongoDB Compass:**
1. Baixe MongoDB Compass (se não instalou junto)
2. Conecte em: `mongodb://localhost:27017`
3. Acesse: `TransactionDb` → `Transactions`
4. Visualize suas transações em tempo real!

## **Sobre o Desenvolvimento**

Este projeto foi desenvolvido seguindo as melhores práticas de:
- **Clean Architecture** de Robert C. Martin
- **SOLID Principles**
- **Dependency Injection**
- **Repository Pattern**
- **Test-Driven Development (TDD)**

---

## **Projeto Finalizado com Sucesso!**

**Desenvolvido para o TalentLab - BSF** 

**Data:** Julho 2025  
**Autor:** Luiz Augusto  
**Versão:** 1.0 - Completo com MongoDB Local ✅

### **Funcionalidades Demonstradas:**
✅ API REST completamente funcional  
✅ Persistência real em MongoDB  
✅ Messaging com Azure Service Bus  
✅ Clean Architecture implementada  
✅ Testes unitários funcionais  
✅ Documentação Swagger automática