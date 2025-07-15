# ApiTaskManager

API para gerenciamento de tarefas da equipe.

---

## 📌 Descrição

O projeto **ApiTaskManager** fornece uma interface REST para controle e acompanhamento de tarefas da equipe.

---
## Usuários de Teste

Ao iniciar a aplicação, dois usuários são automaticamente inseridos no banco de dados para facilitar os testes:

| Nome    | Cargo   |
|---------|---------|
| Gerente | gerente |
| Dev     | dev     |

---
## ❓ Perguntas pendentes ao Project Owner

- Qual setor utilizará a API? Apenas projetos, ou também implantação e sustentação?
- Quais métricas são relevantes para os gerentes acompanharem?
- Deve existir um endpoint específico para informar tarefas bloqueadas?
- Deve haver suporte a divisão de tarefas por projeto?
- Existem regras de prioridade entre tarefas?

---

## 🌟 Melhorias observadas

- Chaveamento de relatórios para todos os níveis (dev, líder, gerente).
- Criação de dependência entre tarefas (ex: tarefa A bloqueia tarefa B).
- Suporte a múltiplas pessoas atribuídas a uma mesma tarefa.
- Endpoint para upload e vínculo de documentos a tarefas.
- Parâmetro maleável para limitar o número de tarefas por pessoa.

---

## 🛠️ Configuração do Banco de Dados

A string de conexão `DefaultConnection` é configurada via variável de ambiente no `docker-compose.yml`. Exemplo:

```yaml
ConnectionStrings__DefaultConnection=Server=MEUSERVIDOR:1433;Database=SEUBANCO;User Id=USUARIODB;Password=SENHADB;TrustServerCertificate=True;
