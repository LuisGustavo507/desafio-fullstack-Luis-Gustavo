# 🌦️ WebClima - Desafio Técnico

Aplicação fullstack para consulta e registro de informações climáticas por coordenadas geográficas ou nome de cidade.

- ✅ Backend (.NET 8)
- ✅ Frontend (Vue 3)
- ✅ PostgreSQL
- ✅ Migrations automáticas
- ✅ Health Check
- ✅ Swagger com autenticação JWT

## 🚀 Como Executar

A aplicação pode ser executada com apenas dois comandos:

```bash
docker pull darl1ngx/webclima:latest
docker run -p 8080:8080 darl1ngx/webclima:latest
```

## 🌐 Acessos

| Serviço  | URL                              | Descrição                  |
|----------|----------------------------------|----------------------------|
| Frontend | http://localhost:8080            | Interface Web (Vue 3)      |
| Swagger  | http://localhost:8080/swagger    | Documentação da API        |
| Health   | http://localhost:8080/health     | Health Check da aplicação  |

---

## 🧭 Primeiros Passos

1. Acesse 👉 [http://localhost:8080/login](http://localhost:8080/login)
2. Crie uma conta na tela de login
3. Faça login com as credenciais criadas
4. Registre a temperatura de uma cidade:
   - Informando o **nome da cidade**, ou
   - Informando **latitude e longitude**
5. Consulte o histórico de registros na aba **"Consultar Histórico"**

---

## 🔐 Autenticação

A API utiliza autenticação **JWT**.

Para testar endpoints protegidos via Swagger:

1. Acesse 👉 [http://localhost:8080/swagger](http://localhost:8080/swagger)
2. Clique em **Authorize**
3. Insira o token no formato:

```
Bearer SEU_TOKEN_AQUI
```

---

## 🩺 Health Check

Endpoint disponível em:

```
GET /health
```

Retorna:

- Status da aplicação
- Status do PostgreSQL
- Status da API externa (OpenWeather)
- Tempo total de execução

---

## 🏗️ Arquitetura

A aplicação foi estruturada seguindo princípios de:

- **Clean Architecture**
- Separação de camadas (Domain, Application, Infrastructure)
- Injeção de dependência
- Repository Pattern
- Unit of Work
- Resiliência com Retry e Circuit Breaker
- Health Checks
- Autenticação JWT

## 📌 Requisitos

> **Docker instalado.**

---

## 📦 Docker Hub

Imagem disponível em:

```
darl1ngx/webclima:latest
```

🔗 [https://hub.docker.com/r/darl1ngx/webclima](https://hub.docker.com/r/darl1ngx/webclima)
