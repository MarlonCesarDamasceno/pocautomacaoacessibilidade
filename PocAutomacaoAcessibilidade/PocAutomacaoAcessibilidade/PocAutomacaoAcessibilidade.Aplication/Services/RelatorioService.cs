using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS.Relatorios;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Enuns;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Aplication.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly ILogger<RelatorioService> _logger;

        public RelatorioService(ILogger<RelatorioService> logger)
        {
            _logger = logger;
        }

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
                

            }
            return relatorioBasico;
        }

        public bool ExportarRelatorioParaExcel(List<ResultadoValidacao> validacoes)
        {
            _logger.LogInformation("Iniciando importação para excel.");

            var primeiraPaginaRelatorio = validacoes.Min(x => x.QuantidadeTestePorDominio);
            var ultimaPaginaRelatorio = validacoes.Max(x => x.QuantidadeTestePorDominio);
            int linha = 0;
            int ct = 0;

            _logger.LogInformation($"Primeira página: {primeiraPaginaRelatorio}. Ultima página: {ultimaPaginaRelatorio}");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excel = new ExcelPackage())
            {

                for (int i = primeiraPaginaRelatorio; i <= ultimaPaginaRelatorio; i++)
                {
                    var filtroPaginaRelatorio = validacoes.Where(x => x.QuantidadeTestePorDominio == i).ToList();
                    string nomeAbaRelatorio = filtroPaginaRelatorio.Select(x => x.ServicoTestado).FirstOrDefault();
                    _logger.LogInformation($"Serviço testado: {filtroPaginaRelatorio.FirstOrDefault().ServicoTestado}. Página atual: {filtroPaginaRelatorio.FirstOrDefault().QuantidadeTestePorDominio}");
                
                    try
                    {
                        _logger.LogInformation($"Criando nova aba de resultados: Serviço testado: {filtroPaginaRelatorio.FirstOrDefault().ServicoTestado}. Página atual: {filtroPaginaRelatorio.FirstOrDefault().QuantidadeTestePorDominio} nova aba: {nomeAbaRelatorio}");
                        ExcelWorksheet plan = excel.Workbook.Worksheets.Add(nomeAbaRelatorio);
                        CriarAbaRelatorio(plan, nomeAbaRelatorio);
                        CriarHeaderColunas(plan);
                        _logger.LogInformation("Definindo contador de linhas e casos de testes para posição inicial. 4 e 1.");
                        ct = 1;
                        linha = 5;

                        foreach (var itensResultados in filtroPaginaRelatorio)
                        {
                            _logger.LogInformation($"Iniciando iteracao do foreach para obter resultado filtrado. {itensResultados.ServicoTestado}.  {itensResultados.QuantidadeTestePorDominio}");
                            if (itensResultados.IDErroComponente.Count == 0)
                            {
                                _logger.LogInformation($"{itensResultados.IDErroComponente.Count}. Preencher excel com valores de erros principais.");
                                PreencherRelatorio(plan, linha, ct, itensResultados.IdErro, itensResultados.Descricao, ConcatenarMensagens(itensResultados.HTML), "N/A", itensResultados.Descricao, itensResultados.Impacto, DefinirStatusTeste(itensResultados.StatusTestes), itensResultados.DiretrizWCAG, itensResultados.PilarWCAG, itensResultados.TipoProblema);
                                linha++;
                                ct++;
                            }
                            else if(itensResultados.HTML.Count==itensResultados.Seletor.Count &&itensResultados.HTML.Count !=itensResultados.IDErroComponente.Count)
                            {
                                    _logger.LogInformation($"{itensResultados.HTML.Count}. Preencher excel com valores de html e seletor fixo pois há apenas um item dentro do array retornado.");
                                for (int obterResultadosPorComponentes = 0; obterResultadosPorComponentes <= itensResultados.IDErroComponente.Count - 1; obterResultadosPorComponentes++)
                                {
                                    _logger.LogInformation($"Iniciado preencher dados por valores de componentes. Quantidades: idComponentes: {itensResultados.IDErroComponente.Count}, impacto: {itensResultados.ImpactoErroComponente.Count}, mensagem: {itensResultados.Mensagem.Count}, html: {itensResultados.HTML.Count}, seletor: {itensResultados.Seletor.Count}, idErro: {itensResultados.IDErroComponente.Count}. iteração atual: {obterResultadosPorComponentes}");

                                    PreencherRelatorio(plan, linha, ct, itensResultados.IDErroComponente[obterResultadosPorComponentes], itensResultados.Mensagem[obterResultadosPorComponentes], ConcatenarMensagens(itensResultados.HTML), ConcatenarMensagens(itensResultados.Seletor), itensResultados.Descricao, itensResultados.ImpactoErroComponente[obterResultadosPorComponentes], DefinirStatusTeste(itensResultados.StatusTestes), itensResultados.DiretrizWCAG, itensResultados.PilarWCAG, itensResultados.TipoProblema);
                                    linha++;
                                    ct++;

                                }
                            }
                            else
                            {
                                for (int obterResultadosPorComponentes = 0; obterResultadosPorComponentes <= itensResultados.IDErroComponente.Count - 1; obterResultadosPorComponentes++)
                                {
                                    _logger.LogInformation($"Iniciado preencher dados por valores de componentes. Quantidades: idComponentes: {itensResultados.IDErroComponente.Count}, impacto: {itensResultados.ImpactoErroComponente.Count}, mensagem: {itensResultados.Mensagem.Count}, html: {itensResultados.HTML.Count}, seletor: {itensResultados.Seletor.Count}, idErro: {itensResultados.IDErroComponente.Count}. iteração atual: {obterResultadosPorComponentes}");

                                    PreencherRelatorio(plan, linha, ct, itensResultados.IDErroComponente[obterResultadosPorComponentes], itensResultados.Mensagem[obterResultadosPorComponentes], itensResultados.HTML[obterResultadosPorComponentes], itensResultados.Seletor[obterResultadosPorComponentes], itensResultados.Descricao, itensResultados.ImpactoErroComponente[obterResultadosPorComponentes], DefinirStatusTeste(itensResultados.StatusTestes), itensResultados.DiretrizWCAG, itensResultados.PilarWCAG, itensResultados.TipoProblema);
                                    linha++;
                                    ct++;
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Falhou.");
                        throw new ArgumentException(ex.Message);
                    }
                    SalvarPlanilha(excel, validacoes.FirstOrDefault().ServicoTestado);
                }
            }
            return true;
        }

        private ExcelWorksheet CriarAbaRelatorio(ExcelWorksheet plan, string nomeServicoTestado)
        {
            plan.Cells["a1"].Value = "MCD Acessibilidade relatório completo";
            plan.Cells["a1:l1"].Merge = true;
            plan.Cells["a1"].Style.Font.Bold = true;
            plan.Cells["a1"].Style.Font.Size = 16;
            plan.Cells["a1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            plan.Cells["a1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            plan.Cells["a1"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue); // Fundo azul claro
            plan.Cells["a1"].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black); // Bordas espessas

            // Serviço analisado
            plan.Cells["a2"].Value = "Serviço analisado";
            plan.Cells["a2:f2"].Merge = true;
            plan.Cells["a2"].Style.Font.Bold = true;
            plan.Cells["a2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            plan.Cells["a2"].Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Fundo cinza claro
            plan.Cells["a2"].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black); // Bordas espessas
            plan.Cells["a2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // Nome do serviço analisado
            plan.Cells["g2"].Value = nomeServicoTestado;
            plan.Cells["g2:l2"].Merge = true;
            plan.Cells["g2"].Style.Font.Bold = true;
            plan.Cells["g2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            plan.Cells["g2"].Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Fundo cinza claro
            plan.Cells["g2"].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black); // Bordas espessas
            plan.Cells["g2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // Data do relatório
            plan.Cells["a3"].Value = "Data";
            plan.Cells["a3:f3"].Merge = true;
            plan.Cells["a3"].Style.Font.Bold = true;
            plan.Cells["a3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            plan.Cells["a3"].Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Fundo cinza claro
            plan.Cells["a3"].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black); // Bordas espessas
            plan.Cells["a3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // Data atual formatada
            plan.Cells["g3"].Value = System.DateTime.Now.ToString("dd/MM/yyyy");
            plan.Cells["g3:l3"].Merge = true;
            plan.Cells["g3"].Style.Font.Bold = true;
            plan.Cells["g3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            plan.Cells["g3"].Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Fundo cinza claro
            plan.Cells["g3"].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black); // Bordas espessas
            plan.Cells["g3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

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
            plan.Cells["i4"].Value = "Diretriz WCAG";
            plan.Cells["j4"].Value = "Pilar WCAG";
            plan.Cells["k4"].Value = "Categoria";
            plan.Cells["l4"].Value = "Data hora teste";

            // Aplicando a formatação
            using (var range = plan.Cells["a4:l4"])
            {
                range.Style.Font.Bold = true; // Negrito
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Alinhamento central
                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Alinhamento vertical central
                range.Style.Fill.PatternType = ExcelFillStyle.Solid; // Tipo de preenchimento sólido
                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue); // Cor de fundo azul claro
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Color.SetColor(Color.Black); // Cor da borda superior
                range.Style.Border.Bottom.Color.SetColor(Color.Black); // Cor da borda inferior
                range.Style.Border.Left.Color.SetColor(Color.Black); // Cor da borda esquerda
                range.Style.Border.Right.Color.SetColor(Color.Black); // Cor da borda direita
            }

            // Ajuste automático de largura das colunas
            plan.Cells["a4:l4"].AutoFitColumns();


            return plan;
        }


        private ExcelWorksheet PreencherRelatorio(ExcelWorksheet plan, int linha, int CT, string Id, string descricaoGeral, string componente, string seletorComponente, string descricao, string impacto, string statusTeste, string diretrizWcag, string pilarWcag, string categoria)
        {
            plan.Cells[$"a{linha}"].Value = $"CT{CT}";
            plan.Cells[$"b{linha}"].Value = Id;
            plan.Cells[$"c{linha}"].Value = descricao;
            plan.Cells[$"d{linha}"].Value = componente;
            plan.Cells[$"e{linha}"].Value = seletorComponente;
            plan.Cells[$"f{linha}"].Value = descricao;
            plan.Cells[$"g{linha}"].Value = impacto;
            plan.Cells[$"h{linha}"].Value = statusTeste;
            plan.Cells[$"i{linha}"].Value = diretrizWcag;
            plan.Cells[$"j{linha}"].Value = pilarWcag;
            plan.Cells[$"k{linha}"].Value = categoria;
            plan.Cells[$"l{linha}"].Value = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); // Corrigido o formato da hora

            // Aplicando a formatação para as células de resultados
            using (var range = plan.Cells[$"a{linha}:l{linha}"])
            {
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Alinhamento central
                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Alinhamento vertical central
                range.Style.Font.Name = "Arial"; // Fonte Arial
                range.Style.Font.Size = 11; // Tamanho da fonte
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Color.SetColor(Color.Black); // Cor da borda superior
                range.Style.Border.Bottom.Color.SetColor(Color.Black); // Cor da borda inferior
                range.Style.Border.Left.Color.SetColor(Color.Black); // Cor da borda esquerda
                range.Style.Border.Right.Color.SetColor(Color.Black); // Cor da borda direita
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.White); // Fundo branco para as células de resultados
            }

            // Ajustar a largura das colunas para os dados
            plan.Cells[$"a{linha}:l{linha}"].AutoFitColumns();

            return plan;
        }

        private void SalvarPlanilha(ExcelPackage package, string url)
        {
            var dataAtual = System.DateTime.Now.ToString("yyyy-MM-HH-mm");

            var nomeArquivo = "RelatorioAcessibilidade_" + Utils.Utils.ExtrairNomeDominiio(url) + "_" +dataAtual+ ".xlsx";

            FileInfo file = new FileInfo(@"C:\Users\Dell\Documents\" + nomeArquivo);
                package.SaveAs(file);
            _logger.LogInformation("Planilha salva");

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
