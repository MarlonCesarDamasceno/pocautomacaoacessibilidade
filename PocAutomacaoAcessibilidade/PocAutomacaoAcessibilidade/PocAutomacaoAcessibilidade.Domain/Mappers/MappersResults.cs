using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS.Relatorios;
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

        public static List<ResultadoValidacao> MapperToResultadoValidacao(AxeResult axeResult, string urlServico, int controlaTesteDisparado)
        {
            var problemasEncontrados = new List<ResultadoValidacao>();
            ResultadoValidacao resultados = null;



            foreach (var obtemProblemasEncontrados in axeResult.Violations)
            {
                resultados = new ResultadoValidacao()
                {
                    QuantidadeTestePorDominio = controlaTesteDisparado,
                    ServicoTestado = urlServico,
                    StatusTestes = StatusTesteEnum.Falhas,
                    Descricao = obtemProblemasEncontrados.Description,
                    DiretrizWCAG = obtemProblemasEncontrados.Tags[1]!= "best-practice"? obtemProblemasEncontrados.Tags[2]: "Não aplicado",
                    PilarWCAG = obtemProblemasEncontrados.Tags[1],
                    TipoProblema = obtemProblemasEncontrados.Tags[0],
                    IdErro = obtemProblemasEncontrados.Id,
                    Impacto = obtemProblemasEncontrados.Impact,
                    HTML = obtemProblemasEncontrados.Nodes.Select(x=>x.Html).ToList(),
                    IDErroComponente = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Any.Any() ? x.Any.Select(c => c.Id) : x.All.Select(x => x.Id)).ToList(),
                    ImpactoErroComponente = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Any.Any() ? x.Any.Select(c => c.Impact) : x.All.Select(x => x.Impact)).ToList(),
                    Mensagem = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Any.Any() ? x.Any.Select(c => c.Message) : x.All.Select(x => x.Message)).ToList(),
                    Seletor = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Target.Select(c => c.Selector)).ToList()
                    //ComponentRelacionado = obtemProblemasEncontrados.Nodes.Select(x => x.Any.SelectMany(c => c.RelatedNodes.SelectMany(v => v.Html))).ToList()
                };

                problemasEncontrados.Add(resultados);
            }

            foreach (var obtemProblemasEncontrados in axeResult.Passes)
            {
                resultados = new ResultadoValidacao()
                {
                    QuantidadeTestePorDominio = controlaTesteDisparado,
                    StatusTestes = StatusTesteEnum.Sucessos,
                    Descricao = obtemProblemasEncontrados.Description,
                    DiretrizWCAG = obtemProblemasEncontrados.Tags[1] != "best-practice" ? obtemProblemasEncontrados.Tags[2] : "Não aplicado",
                    PilarWCAG = obtemProblemasEncontrados.Tags[1],
                    TipoProblema = obtemProblemasEncontrados.Tags[0],
                    IdErro = obtemProblemasEncontrados.Id,
                    Impacto = obtemProblemasEncontrados.Impact,
                    HTML = obtemProblemasEncontrados.Nodes.Select(x => x.Html).ToList(),
                    IDErroComponente = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Any.Any() ? x.Any.Select(c => c.Id) : x.All.Select(x => x.Id)).ToList(),
                    ImpactoErroComponente = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Any.Any() ? x.Any.Select(c => c.Impact) : x.All.Select(x => x.Impact)).ToList(),
                    Mensagem = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Any.Any() ? x.Any.Select(c => c.Message) : x.All.Select(x => x.Message)).ToList(),
                    Seletor = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Target.Select(c => c.Selector)).ToList()
                    //ComponentRelacionado = obtemProblemasEncontrados.Nodes.Select(x => x.Any.SelectMany(c => c.RelatedNodes.SelectMany(v => v.Html))).ToList()
                };


                problemasEncontrados.Add(resultados);
            }


            foreach (var obtemProblemasEncontrados in axeResult.Incomplete)
            {

                resultados = new ResultadoValidacao()
                {
                    QuantidadeTestePorDominio = controlaTesteDisparado,
                    ServicoTestado = urlServico,
                    StatusTestes = StatusTesteEnum.Incompletos,
                    Descricao = obtemProblemasEncontrados.Description,
                    DiretrizWCAG = obtemProblemasEncontrados.Tags[1] != "best-practice" ? obtemProblemasEncontrados.Tags[2] : "Não aplicado",
                    PilarWCAG = obtemProblemasEncontrados.Tags[1],
                    TipoProblema = obtemProblemasEncontrados.Tags[0],
                    IdErro = obtemProblemasEncontrados.Id,
                    Impacto = obtemProblemasEncontrados.Impact,
                
                    HTML = obtemProblemasEncontrados.Nodes.Select(x => x.Html).ToList(),
                    IDErroComponente = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Any.Any() ? x.Any.Select(c => c.Id) : x.All.Select(x => x.Id)).ToList(),
                    ImpactoErroComponente = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Any.Any() ? x.Any.Select(c => c.Impact) : x.All.Select(x => x.Impact)).ToList(),
                    Mensagem = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Any.Any() ? x.Any.Select(c => c.Message) : x.All.Select(x => x.Message)).ToList(),
                    Seletor = obtemProblemasEncontrados.Nodes.SelectMany(x => x.Target.Select(c => c.Selector)).ToList()
                    //ComponentRelacionado = obtemProblemasEncontrados.Nodes.Select(x => x.Any.SelectMany(c => c.RelatedNodes.SelectMany(v => v.Html))).ToList()
                };
    


            

                problemasEncontrados.Add(resultados);
            }




            var saida = JsonSerializer.Serialize(problemasEncontrados);
            Console.WriteLine("Resultado da conversão: " + saida);
            return problemasEncontrados;
        }



    }
}