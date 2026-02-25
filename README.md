🌦️ WebClima - Desafio Técnico

O WebClima é uma solução robusta para consulta e monitoramento meteorológico, construída com foco em resiliência, escalabilidade e boas práticas de arquitetura.
🚀 Como Executar

A aplicação está totalmente "dockerizada". Você não precisa instalar dependências locais (Node, .NET, Python, etc.), apenas o Docker.

    Baixe a imagem:
    Bash

    docker pull darl1ngx/webclima:latest

    Execute o container:
    Bash

    docker run -p 8080:8080 darl1ngx/webclima:latest

    Acesse:

        Frontend/App: http://localhost:8080

        API Health: http://localhost:8080/health

🛠️ Arquitetura e Tecnologias

O projeto foi desenvolvido seguindo padrões rigorosos de engenharia de software para garantir manutenção facilitada e alta disponibilidade.
Padrões de Design

    Clean Architecture: Separação clara entre as camadas de Domain, Application e Infrastructure.

    Repository Pattern & Unit of Work: Abstração da camada de dados para consistência e testabilidade.

    Injeção de Dependência: Para um acoplamento fraco entre os componentes.

Resiliência e Monitoramento

    Circuit Breaker & Retry: Estratégias para lidar com falhas temporárias em APIs externas.

    Health Checks: Monitoramento em tempo real da saúde do sistema.

    Autenticação JWT: Segurança no acesso aos endpoints da API.

🔍 Diagnóstico do Sistema (Health Check)

O endpoint GET /health fornece um status detalhado da saúde da aplicação, incluindo:
Componente	Descrição
Application	Status geral da aplicação
PostgreSQL	Conectividade com o banco de dados
OpenWeather API	Disponibilidade da integração externa
Execution Time	Tempo total de processamento da requisição
🐳 Observações sobre o Docker

A imagem Docker disponível no Hub (darl1ngx/webclima) é uma solução All-in-One:

    Backend & Frontend: Integrados no mesmo ciclo de vida.

    Banco de Dados: PostgreSQL configurado internamente.

    Migrations: O esquema do banco é criado e atualizado automaticamente ao subir o container.

    Zero Config: Sem necessidade de variáveis de ambiente manuais para o funcionamento básico.

📌 Requisitos

    Docker instalado e rodando.
