using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Enuns;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Aplication.Services
{
    public class MotorAcessibilidadeService: IMotorAcessibilidadeService
    {
        private readonly IMotorAcessibilidade _motorAcessibilidade;
        private readonly IRelatorioService _relatorioService;

        public MotorAcessibilidadeService(IMotorAcessibilidade motorAcessibilidade, IRelatorioService relatorioService)
        {
            _motorAcessibilidade = motorAcessibilidade;
            _relatorioService = relatorioService;
        }

        public async Task<ResultadosTestes> IniciarTeste(string urlServico, string subDominio)
        {
            var resultadoTeste = new List<ResultadoValidacao>();
            var analiseResultadoPreliminarValidacao = new List<AnalisePreviaResultadoTeste>();
            string[] dominios = subDominio.Split(',');

            if (string.IsNullOrEmpty(urlServico))
                return null;

            if(dominios.Length==0 || string.IsNullOrEmpty(subDominio))
            {
                
                resultadoTeste= await _motorAcessibilidade.ValidarAcessibilidade(urlServico);

            }
            else
            {
                resultadoTeste.AddRange(await _motorAcessibilidade.ValidarAcessibilidade(urlServico));
                foreach (var pathSubDominio in dominios)
                {
                    resultadoTeste.AddRange(await _motorAcessibilidade.ValidarAcessibilidade(urlServico + "/" + pathSubDominio));
                }
            }

            if (resultadoTeste.Count == 0)
                return null;


            analiseResultadoPreliminarValidacao.AddRange(_relatorioService.GerarBaseRelatorio(resultadoTeste));

            var resultadosValidacoes = new ResultadosTestes()
            {
                analisesPreliminaresValidacoes=analiseResultadoPreliminarValidacao,
                resultadosValidacoes=resultadoTeste
            };


            return resultadosValidacoes;
        }

    }
}
