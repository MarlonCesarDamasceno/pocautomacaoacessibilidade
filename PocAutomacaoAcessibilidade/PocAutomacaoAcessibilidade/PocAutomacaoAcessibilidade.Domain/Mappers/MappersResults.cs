using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using Selenium.Axe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Mappers
{
    public static class MappersResults
    {

        public static List<ResultadoValidacao> MapperToResultadoValidacao(AxeResult axeResult)
        {
            var problemasEncontrados = new List<ResultadoValidacao>();
            ResultadoValidacao resultados = null;


            
            foreach (var obtemProblemasEncontrados in axeResult.Violations)
            {
                resultados = new ResultadoValidacao()
                {
                    StatusTestes=StatusTesteEnum.Falhas,
                    Descricao = obtemProblemasEncontrados.Description,
                    //DiretrizWCAG = obtemProblemasEncontrados.Tags[2],
                    PilarWCAG = obtemProblemasEncontrados.Tags[1],
                    TipoProblema = obtemProblemasEncontrados.Tags[0],
                    IdErro = obtemProblemasEncontrados.Id,
                    Impacto = obtemProblemasEncontrados.Impact,
                    HTML=string.Join(",", obtemProblemasEncontrados.Nodes.Select(x=>x.Html)),
                    IDErroComponente=string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c=>c.Id))),
                    ImpactoErroComponente=string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Impact))),
                    Mensagem=string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Message))),
                    Seletor=string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x=>x.Target.Select(c=>c.Selector))),
                    ComponentRelacionado=string.Join(",", obtemProblemasEncontrados.Nodes.Select(x => x.Any.Select(c=>c.RelatedNodes.SelectMany(v=>v.Html))))
                };

                    problemasEncontrados.Add(resultados);
            }

            foreach (var obtemProblemasEncontrados in axeResult.Passes)
            {
                resultados = new ResultadoValidacao()
                {
                    StatusTestes = StatusTesteEnum.Sucessos,
                    Descricao = obtemProblemasEncontrados.Description,
                    //DiretrizWCAG = obtemProblemasEncontrados.Tags[2],
                    PilarWCAG = obtemProblemasEncontrados.Tags[1],
                    TipoProblema = obtemProblemasEncontrados.Tags[0],
                    IdErro = obtemProblemasEncontrados.Id,
                    Impacto = obtemProblemasEncontrados.Impact,
                    HTML = string.Join(",", obtemProblemasEncontrados.Nodes.Select(x => x.Html)),
                    IDErroComponente = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Id))),
                    ImpactoErroComponente = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Impact))),
                    Mensagem = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Message))),
                    Seletor = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.Target.Select(c => c.Selector))),
                    ComponentRelacionado = string.Join(",", obtemProblemasEncontrados.Nodes.Select(x => x.Any.Select(c => c.RelatedNodes.SelectMany(v => v.Html))))
                };

                problemasEncontrados.Add(resultados);
            }


            foreach (var obtemProblemasEncontrados in axeResult.Incomplete)
            {
                resultados = new ResultadoValidacao()
                {
                    StatusTestes = StatusTesteEnum.Incompletos,
                    Descricao = obtemProblemasEncontrados.Description,
                    //DiretrizWCAG = obtemProblemasEncontrados.Tags[2],
                    PilarWCAG = obtemProblemasEncontrados.Tags[1],
                    TipoProblema = obtemProblemasEncontrados.Tags[0],
                    IdErro = obtemProblemasEncontrados.Id,
                    Impacto = obtemProblemasEncontrados.Impact,
                    HTML = string.Join(",", obtemProblemasEncontrados.Nodes.Select(x => x.Html)),
                    IDErroComponente = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Id))),
                    ImpactoErroComponente = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Impact))),
                    Mensagem = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Message))),
                    Seletor = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.Target.Select(c => c.Selector))),
                    ComponentRelacionado = string.Join(",", obtemProblemasEncontrados.Nodes.Select(x => x.Any.Select(c => c.RelatedNodes.SelectMany(v => v.Html))))
                };

                problemasEncontrados.Add(resultados);
            }

            foreach (var obtemProblemasEncontrados in axeResult.Inapplicable)
            {
                resultados = new ResultadoValidacao()
                {
                    StatusTestes = StatusTesteEnum.NaoAplicados,
                    Descricao = obtemProblemasEncontrados.Description,
                    //DiretrizWCAG = obtemProblemasEncontrados.Tags[2],
                    PilarWCAG = obtemProblemasEncontrados.Tags[1],
                    TipoProblema = obtemProblemasEncontrados.Tags[0],
                    IdErro = obtemProblemasEncontrados.Id,
                    Impacto = obtemProblemasEncontrados.Impact,
                    HTML = string.Join(",", obtemProblemasEncontrados.Nodes.Select(x => x.Html)),
                    IDErroComponente = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Id))),
                    ImpactoErroComponente = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Impact))),
                    Mensagem = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.All.Select(c => c.Message))),
                    Seletor = string.Join(",", obtemProblemasEncontrados.Nodes.SelectMany(x => x.Target.Select(c => c.Selector))),
                    ComponentRelacionado = string.Join(",", obtemProblemasEncontrados.Nodes.Select(x => x.Any.Select(c => c.RelatedNodes.SelectMany(v => v.Html))))
                };

                problemasEncontrados.Add(resultados);
            }




            var saida = JsonSerializer.Serialize(problemasEncontrados);
            Console.WriteLine("Resultado da conversão: " + saida);
            return problemasEncontrados;
        }

    }
}