using ApiTaskManager.Enums;
using ApiTaskManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data.Common;

namespace ApiTaskManager.Data;

public class ProjetosDAL(ApiDbContext context) : BaseDAL(context)
{
    public List<string> GetNameOfAllProjects()
    {
        return [.. context.Projetos.Select(p => p.Nome)];
    }

    public List<string> GetProjectsByStatus(Status status)
    {
        return [.. context.Projetos.Where(p => p.Status == status).Select(p => p.Nome)];
    }

    public Projeto? GetById(int id)
    {
        return context.Projetos.Find(id);
    }

    public Projeto? GetByIdWithTasks(int idProjeto)
    {
        return context.Projetos.Include(p => p.Tarefas).First(p => p.Id == idProjeto);
    }

    public int CreateProject(Projeto projeto)
    {
        context.Projetos.Add(projeto);
        context.SaveChanges();

        return projeto.Id;
    }

    public void UpdateProject(Projeto projeto)
    {
        context.Projetos.Add(projeto);
        context.SaveChanges();
    }
}
