using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Enuns;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Aplication.Services
{
    public class RelatorioService: IRelatorioService
    {

        public AnalisePreviaResultadoTeste GerarBaseRelatorio(List<ResultadoValidacao> resultadoValidacaos)
        {
            var baseRelatorio = new AnalisePreviaResultadoTeste();
            foreach (var relatorio in resultadoValidacaos)
            {
                switch (relatorio.StatusTestes)
                {
                    case StatusTesteEnum.Falhas:
                        baseRelatorio.Falhas++;
                        break;

                    case StatusTesteEnum.Sucessos:
                        baseRelatorio.Sucessos++;
                        break;

                    case StatusTesteEnum.NaoAplicados:
                        baseRelatorio.NaoAplicados++;
                        break;

                    case StatusTesteEnum.Incompletos:
                        baseRelatorio.Incompletos++;
                        break;
                }

                switch (relatorio.Impacto)
                {
                    case var impacto when impacto == ImpactoEnum.Serious.ToString():
                        baseRelatorio.ImpactoSerio++;
                        break;
                    case var impacto when impacto == ImpactoEnum.Critical.ToString():
                        baseRelatorio.ImpactoCritico++;
                        break;
                    case var impacto when impacto == ImpactoEnum.Moderate.ToString():
                        baseRelatorio.ImpactoModerado++;
                        break;
                }

                switch (relatorio.TipoProblema)
                {
                    case TiposErros.CatAria:
                        baseRelatorio.RelateAriaRoles++;
                        break;

                    case TiposErros.CatColor:
                        baseRelatorio.RelateContrast++;
                        break;


                    case TiposErros.CatForms:
                        baseRelatorio.RelateForm++;
                        break;

                    case TiposErros.CatImages:
                        baseRelatorio.RelateImagem++;
                        break;


                    case TiposErros.CatKeyboard:
                        baseRelatorio.RelateTeclado++;
                        break;


                    case TiposErros.CatLanguage:
                        baseRelatorio.RelateLang++;
                        break;


                    case TiposErros.CatLinks:
                        baseRelatorio.RelateLink++;
                        break;


                    case TiposErros.CatStructure:
                        baseRelatorio.RelateDocEstrutura++;
                        break;
                }
            }
            return baseRelatorio;
        }


    }
}
