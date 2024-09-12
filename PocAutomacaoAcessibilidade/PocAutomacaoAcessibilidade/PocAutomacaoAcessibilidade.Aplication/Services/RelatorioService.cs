using OfficeOpenXml;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS.Relatorios;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Enuns;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Aplication.Services
{
    public class RelatorioService : IRelatorioService
    {

        public List<AnalisePreviaResultadoTeste> GerarBaseRelatorio(List<ResultadoValidacao> resultadoValidacaos)
        {
            var relatorioBasico = new List<AnalisePreviaResultadoTeste>();


            int maxQuantidadeTestePorDominio = resultadoValidacaos.Max(x => x.QuantidadeTestePorDominio);

            //fazer iteração para validar se há teste para mais de um dominio para gerar o relatorio basico.
            for (int selecioneTestePorDominio = 1; selecioneTestePorDominio <= maxQuantidadeTestePorDominio; selecioneTestePorDominio++)
            {
                var baseRelatorio = new AnalisePreviaResultadoTeste();
                var obtemRelatorioPorDominio = resultadoValidacaos.Where(x => x.QuantidadeTestePorDominio == selecioneTestePorDominio).ToList();
                foreach (var relatorio in obtemRelatorioPorDominio)
                {
                    baseRelatorio.ServicoTestado = relatorio.ServicoTestado;

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
                        case var impacto when impacto == ImpactoEnum.serious.ToString() && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.ImpactoSerio++;
                            break;
                        case var impacto when impacto == ImpactoEnum.critical.ToString() && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.ImpactoCritico++;
                            break;
                        case var impacto when impacto == ImpactoEnum.moderate.ToString() && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.ImpactoModerado++;
                            break;
                    }

                    switch (relatorio.TipoProblema)
                    {
                        case var tipoErros when tipoErros == TiposErros.CatAria && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.RelateAriaRoles++;
                            break;

                        case var tiposErros when tiposErros == TiposErros.CatColor && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.RelateContrast++;
                            break;


                        case var tiposErros when tiposErros == TiposErros.CatForms && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.RelateForm++;
                            break;

                        case var tiposErros when tiposErros == TiposErros.CatImages && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.RelateImagem++;
                            break;


                        case var tiposErros when tiposErros == TiposErros.CatKeyboard && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.RelateTeclado++;
                            break;


                        case var tiposErros when tiposErros == TiposErros.CatLanguage && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.RelateLang++;
                            break;


                        case var tiposErros when tiposErros == TiposErros.CatLinks && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.RelateLink++;
                            break;


                        case var tiposErros when tiposErros == TiposErros.CatStructure && relatorio.StatusTestes == StatusTesteEnum.Falhas:
                            baseRelatorio.RelateDocEstrutura++;
                            break;
                    }

                }
                relatorioBasico.Add(baseRelatorio);
                baseRelatorio = null;

            }
            return relatorioBasico;
        }

        public bool ExportarRelatorioParaExcel(List<ResultadoValidacao> validacoes)
        {

            var primeiraPaginaRelatorio = validacoes.Min(x => x.QuantidadeTestePorDominio);
            var ultimaPaginaRelatorio = validacoes.Max(x => x.QuantidadeTestePorDominio);
            int linha = 5;
            int ct = 1;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excel = new ExcelPackage())
            {

                for (int i = primeiraPaginaRelatorio; i <= ultimaPaginaRelatorio; i++)
            {
                var filtroPaginaRelatorio = validacoes.Where(x => x.QuantidadeTestePorDominio == i);
                string nomeAbaRelatorio = filtroPaginaRelatorio.Select(x => x.ServicoTestado).FirstOrDefault();

                    try
                    {
                        ExcelWorksheet plan = excel.Workbook.Worksheets.Add("Relatorio Automação Acessibilidade");
                        CriarAbaRelatorio(plan, nomeAbaRelatorio);
                        CriarHeaderColunas(plan);

                        foreach (var itensResultados in filtroPaginaRelatorio)
                        {
                            if (itensResultados.IDErroComponente.Count == 0)
                            {
                                PreencherRelatorio(plan, linha, ct, itensResultados.IdErro, itensResultados.Descricao, ConcatenarMensagens(itensResultados.HTML), "N/A", itensResultados.Descricao, itensResultados.Impacto, DefinirStatusTeste(itensResultados.StatusTestes));
                                linha++;
                                ct++;
                            }
                            else
                            {
                                for (int obterResultadosPorComponentes = 0; obterResultadosPorComponentes <= itensResultados.IDErroComponente.Count-1; obterResultadosPorComponentes++)
                                {
                                    PreencherRelatorio(plan, linha, ct, itensResultados.IDErroComponente[obterResultadosPorComponentes], itensResultados.Mensagem[obterResultadosPorComponentes], itensResultados.HTML[obterResultadosPorComponentes], itensResultados.Seletor[obterResultadosPorComponentes], itensResultados.Descricao, itensResultados.ImpactoErroComponente[obterResultadosPorComponentes], DefinirStatusTeste(itensResultados.StatusTestes));
                                    linha++;
                                    ct++;
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(ex.Message);
                    }
                    SalvarPlanilha(excel, "RelatorioAcessibilidade");
                }
            }
            return true;
        }

        private ExcelWorksheet CriarAbaRelatorio(ExcelWorksheet plan, string nomeServicoTestado)
        {
            plan.Cells["a1"].Value = "M6 Acessibilidade relatório completo";
            plan.Cells["a2"].Value = "Serviço analisado";
            plan.Cells["a3"].Value = "Data";
            plan.Cells["d2"].Value = nomeServicoTestado;
            plan.Cells["d3"].Value = System.DateTime.Now.ToString("AA/mm/AAAA");
            plan.Cells["a1:h1"].Merge = true;
            plan.Cells["a1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            plan.Cells["a2:c2"].Merge = true;
            plan.Cells["a2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            plan.Cells["d2:h2"].Merge = true;
            plan.Cells["d2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            plan.Cells["a3:c3"].Merge = true;
            plan.Cells["a3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            plan.Cells["d3:h3"].Merge = true;
            plan.Cells["d3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            return plan;
        }

        private ExcelWorksheet CriarHeaderColunas(ExcelWorksheet plan)
        {

            plan.Cells["a4"].Value = "CT";
            plan.Cells["b4"].Value = "ID";
            plan.Cells["c4"].Value = "Descrição geral";
            plan.Cells["d4"].Value = "Componente";
            plan.Cells["e4"].Value = "Seletor Componente";
            plan.Cells["f4"].Value = "Descrição";
            plan.Cells["g4"].Value = "Impacto";
            plan.Cells["h4"].Value = "Status do teste";

            return plan;
        }


        private ExcelWorksheet PreencherRelatorio(ExcelWorksheet plan, int linha, int CT, string Id, string descricaoGeral, string componente, string seletorComponente, string descricao, string impacto, string statusTeste)
        {
            plan.Cells[$"a{linha}"].Value = $"CT{CT}";
            plan.Cells[$"b{linha}"].Value = Id;
            plan.Cells[$"c{linha}"].Value = descricao;
            plan.Cells[$"d{linha}"].Value = componente;
            plan.Cells[$"e{linha}"].Value = seletorComponente;
            plan.Cells[$"f{linha}"].Value = descricao;
            plan.Cells[$"g{linha}"].Value = impacto;
            plan.Cells[$"h{linha}"].Value = statusTeste;

            return plan;
        }

        private void SalvarPlanilha(ExcelPackage package, string nomeArquivo)
        {
            FileInfo file = new FileInfo(@"C:\Users\Dell\Documents\" + nomeArquivo + ".xlsx");
            package.SaveAs(file);

        }

        private string DefinirStatusTeste(StatusTesteEnum statusteste)
        {
            string statusDoTeste = string.Empty;
            switch (statusteste)
            {
                case StatusTesteEnum.Falhas:
                    statusDoTeste = StatusTesteEnum.Falhas.ToString();
                    break;

                case StatusTesteEnum.Incompletos:
                    statusDoTeste = StatusTesteEnum.Incompletos.ToString();
                    break;

                case StatusTesteEnum.Sucessos:
                    statusDoTeste = StatusTesteEnum.Sucessos.ToString();
                    break;

            }

            return statusDoTeste;
        }


        private string ConcatenarMensagens(List<string> mensagem)
        {
            return string.Join("\n", mensagem);
        }
    }
}
