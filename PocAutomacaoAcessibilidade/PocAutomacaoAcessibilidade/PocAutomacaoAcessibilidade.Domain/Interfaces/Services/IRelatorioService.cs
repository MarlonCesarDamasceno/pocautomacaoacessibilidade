using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces.Services
{
    public interface IRelatorioService
    {
        bool ExportarRelatorioParaExcel(List<ResultadoValidacao> validacoes);
        List<AnalisePreviaResultadoTeste> GerarBaseRelatorio(List<ResultadoValidacao> resultadoValidacaos);
    }
}
