# Task Manager API

API REST para gestão de tarefas desenvolvida em .NET 6, Entity Framework Core InMemory, Swagger e testes automatizados com xUnit.

O objetivo do projeto é demonstrar uma API backend simples, organizada e fácil de manter, sem criar uma arquitetura maior do que o problema exige.

## Tecnologias utilizadas

- .NET 6
- ASP.NET Core Web API
- Entity Framework Core InMemory
- Swagger / Swashbuckle
- xUnit
- Injeção de dependência nativa do ASP.NET Core
- Logging com `ILogger`

## Como rodar o projeto

Na raiz do projeto, execute:

```bash
dotnet restore TaskManager.sln
dotnet build TaskManager.sln
dotnet run --project src/TaskManager.Api/TaskManager.Api.csproj
```

Por padrão, o projeto sobe em:

```text
http://localhost:5226
```

Se você usar um SDK mais recente, pode aparecer um aviso dizendo que `net6.0` saiu do suporte da Microsoft. O projeto foi mantido em .NET 6 para seguir o desafio técnico.

## Swagger

Com a API rodando, acesse:

```text
http://localhost:5226/swagger
```

O Swagger permite testar os endpoints de criação, listagem, busca, atualização e exclusão de tarefas.

## Como executar os testes

```bash
dotnet test TaskManager.sln
```

Os testes ficam no projeto `tests/TaskManager.Tests` e cobrem a camada de service, onde estão as principais regras de negócio.

## Endpoints

| Método | Rota | Descrição |
| --- | --- | --- |
| POST | `/api/tasks` | Cria uma tarefa |
| GET | `/api/tasks` | Lista tarefas, com filtros opcionais |
| GET | `/api/tasks/{id}` | Busca uma tarefa por ID |
| PUT | `/api/tasks/{id}` | Atualiza uma tarefa |
| DELETE | `/api/tasks/{id}` | Exclui uma tarefa |

Filtros disponíveis na listagem:

```text
GET /api/tasks?status=Pendente
GET /api/tasks?dueDate=2026-05-25
GET /api/tasks?status=Concluida&dueDate=2026-05-25
```

Status aceitos:

- `Pendente`
- `EmProgresso`
- `Concluida`

Exemplo de criação:

```json
{
  "title": "Estudar projeto",
  "description": "Revisar service e controller",
  "dueDate": "2026-05-25",
  "status": "Pendente"
}
```

## Estrutura de pastas

```text
src/
  TaskManager.Api/
  TaskManager.Application/
  TaskManager.Domain/
  TaskManager.Infrastructure/

tests/
  TaskManager.Tests/
```

## Arquitetura

Foi utilizada uma separação em camadas para manter as responsabilidades bem definidas. A camada de API fica responsável pelos endpoints, a camada Application concentra as regras de negócio, a camada Domain mantém os modelos principais e contratos, e a camada Infrastructure cuida da persistência com Entity Framework Core InMemory.

A ideia foi manter uma organização clara, mas sem exagerar em padrões. Para este CRUD, não foi criado Unit of Work nem uma Clean Architecture completa, porque isso aumentaria a complexidade sem trazer ganho real para o escopo do desafio.

## Responsabilidade de cada camada

`TaskManager.Api`

- Controllers
- Configuração da aplicação
- Swagger
- Middleware global de erros
- Registro de dependências
- Retornos HTTP adequados

`TaskManager.Application`

- DTOs de entrada e saída
- Interface e implementação do service
- Validações de regra de negócio
- Fluxo principal das operações

`TaskManager.Domain`

- Entidade `TaskItem`
- Enum `TaskItemStatus`
- Contrato do repositório
- Exceções usadas no fluxo da aplicação

`TaskManager.Infrastructure`

- `TaskManagerDbContext`
- Configuração do EF Core InMemory
- Implementação do repositório

`TaskManager.Tests`

- Testes unitários do service
- Fake repository para isolar a lógica de negócio

## Por que EF Core InMemory

O EF Core InMemory foi usado porque o desafio não exige banco real. Ele permite testar o fluxo de persistência sem instalar SQL Server, Oracle ou qualquer banco local.


## Tratamento de erros

O projeto usa um middleware global para tratar exceções. Assim, os controllers não precisam repetir `try/catch`.

Exemplo de resposta:

```json
{
  "message": "Tarefa não encontrada",
  "statusCode": 404
}
```

Mapeamento principal:

- `BusinessValidationException` retorna `400 Bad Request`
- `NotFoundException` retorna `404 Not Found`
- Erros inesperados retornam `500 Internal Server Error`

## Logging

O projeto usa `ILogger` para registrar operações importantes:

- criação de tarefa
- atualização de tarefa
- exclusão de tarefa
- tentativa de acessar tarefa inexistente
- erro inesperado no middleware

Os logs foram direcionados para console/debug e o ruído do Entity Framework foi reduzido no `appsettings`.

## Testes

Os testes foram organizados em torno do `TaskService`, porque é nele que ficam as regras principais. O repositório usado nos testes é um fake em memória, criado no próprio projeto de testes.

Cenários cobertos:

- criar tarefa com sucesso
- não criar tarefa sem título
- não criar tarefa com data anterior à data atual
- buscar tarefa existente
- buscar tarefa inexistente
- atualizar tarefa existente
- não atualizar tarefa inexistente
- excluir tarefa existente
- não excluir tarefa inexistente
- listar tarefas filtrando por status
- listar tarefas filtrando por data de vencimento

## Publicação no GitHub

Crie um repositório vazio no GitHub com:

- Repository name: `task-manager-api-teste`
- Description: `API REST para gestão de tarefas desenvolvida em .NET com EF Core InMemory, Swagger, testes e boas práticas.`
- Visibility: `Public`
- Initialize with README: `Não`
- Add .gitignore: `Não`
- Choose a license: `None`

Depois, na raiz do projeto:

```bash
git remote add origin https://github.com/alanbritodossantos/task-manager-api-teste.git
git branch -M main
git push -u origin main
```


