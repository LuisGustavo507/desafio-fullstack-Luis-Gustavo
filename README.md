# Web Clima API

## Visão Geral

Web Clima API é uma solução para consulta de condições climáticas.

---

## Stack Tecnológica

| Camada         | Tecnologia           |
|----------------|----------------------|
| Backend        | .NET 8 (C#)          |
| Frontend       | Vue.js 3, PrimeVue   |
| Banco de Dados | PostgreSQL           |
| Infraestrutura | Docker, Nginx        |

---

## Arquitetura de Resiliência

### Padrão Unit of Work

- **Garantia de integridade transacional**: Todas as operações de banco são agrupadas em unidades atômicas, evitando inconsistências.
- **Separação de responsabilidades**: Facilita manutenção e testes.

### Resiliência com Polly

| Recurso         | Descrição                                                                 |
|-----------------|---------------------------------------------------------------------------|
| Retry Policy    | Requisições externas à OpenWeather são automaticamente reexecutadas em caso de falhas transitórias. |
| Circuit Breaker | Protege o sistema contra sobrecarga e falhas em cascata, isolando serviços indisponíveis.   

---

## Observabilidade e Monitoramento

- **Health Checks**: Endpoint `/health` expõe o estado da API e do banco de dados.
- **Swagger**: Documentação interativa e testes de endpoints.

---

## Segurança

- **JWT Authentication**: Protege endpoints sensíveis, como histórico de consultas, garantindo acesso apenas a usuários autenticados.

---

## Infraestrutura

- **Docker Compose**: Orquestração completa, incluindo Nginx como gateway/proxy reverso.
- **Arquivo `.env`**: Configuração centralizada.

## Imagens Docker Hub

| Serviço   | Imagem Docker Hub                  |
|-----------|------------------------------------|
| API       | `darl1ngx/webclima-backend`        |
| Frontend  | `darl1ngx/webclima-frontend`       |

## Como Executar

### Pré-requisitos

- Docker instalado e rodando

# Exemplo de configuração do arquivo .env (dados fictícios)

OpenWeather__ApiKey=abcdef1234567890abcdef1234567890

ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=webclima_demo;Username=webclima_user;Password=webclima_pass

JWT_KEY=DemoSecretKeyForJWTSigningPurposes2026!@#$%

JWT_ISSUER=WebClimaDemoAPI

JWT_AUDIENCE=WebClimaDemoClient

### Subir a aplicação completa

Na raiz do projeto, execute:

docker compose up -d --build

Serviços principais:

| Serviço   | URL                          | Descrição                        |
|-----------|------------------------------|----------------------------------|
| Frontend  | http://localhost:5173        | Interface web (Vue 3)            |
| API       | http://localhost:5110        | Backend (.NET 8)                 |
| Swagger   | http://localhost:5110/swagger| Documentação da API              |
| Health    | http://localhost:5110/health | Health check da aplicação        |
| Banco     | localhost:5432               | PostgreSQL 17                    |

### Primeiros passos

1. Acesse [http://localhost:5173/login](http://localhost:5173/login)
2. Crie uma conta na tela de login
3. Faça login com as credenciais criadas
4. Registre a temperatura de uma cidade com o nome e/ou coordenadas
5. Consulte o histórico de temperaturas na aba "Consultar Histórico"

---

## Endpoints da API

| Método | Endpoint                         | Descrição                                                                 |
|--------|----------------------------------|---------------------------------------------------------------------------|
| POST   | `/api/weather/registrar`         | Cria um novo usuário (necessário para autenticação)                       |
| POST   | `/api/weather/login`             | Autentica usuário e retorna token JWT                                     |
| GET    | `/api/weather/consulta`          | Consulta clima por cidade e latitude/longitude                            |
| GET    | `/api/weather/historico`         | Busca histórico de consultas do usuário autenticado                       |
| GET    | `/health`                        | Health check da API e banco de dados                                      |
| GET    | `/swagger`                       | Documentação interativa da API                                            |

---

## Executando os Testes

### Testes de Integração (.NET)

Execute os testes via Visual Studio ou CLI:

dotnet test WebClima.Testes

- Os testes de integração simulam requisições reais, validando persistência, autenticação e endpoints.

---


