# Gerenciador de Pedidos

Uma API para gerenciamento de pedidos, clientes e produtos.

## Tecnologias Utilizadas

### Backend
- .NET Core
- Entity Framework Core
- SQL Server
- Serilog (logs)

### Testes
- xUnit 
- FluentAssertions 
- Bogus 
- NSubstitute 

## Instalação

1. Clone o repositório:
   ```bash
   git clone https://github.com/joylopes/gerenciador-pedido.git

2. Navegue até o diretório do projeto:
   cd gerenciador-pedido

3. Configure o banco de dados no appsettings.json.

4. Restaure as dependências:
   dotnet restore
   
5. Execute as migrações do banco de dados:
   dotnet ef database update

6. Inicie a aplicação:
   dotnet run
   
## Uso
A API estará disponível em http://localhost:5000. Você pode testar os endpoints usando Postman ou Swagger.

## Testes
Para executar os testes:
  dotnet test

## Contato
Para mais informações ou esclarecimentos, entre em contato:
  - Autor: Joice Lopes
  - GitHub: https://github.com/joylopes
  - E-mail: joylopes@gmal.com




