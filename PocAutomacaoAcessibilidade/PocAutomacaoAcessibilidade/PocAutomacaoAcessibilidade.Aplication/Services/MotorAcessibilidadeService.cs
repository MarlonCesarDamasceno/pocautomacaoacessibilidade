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

        public async Task<AnalisePreviaResultadoTeste> IniciarTeste(string urlServico)
        {

            if (string.IsNullOrEmpty(urlServico))
                return null;
            var resultadoTeste = await _motorAcessibilidade.ValidarAcessibilidade(urlServico);

            if (resultadoTeste.Count == 0)
                return null;

            return _relatorioService.GerarBaseRelatorio(resultadoTeste);
        }

    }
}
