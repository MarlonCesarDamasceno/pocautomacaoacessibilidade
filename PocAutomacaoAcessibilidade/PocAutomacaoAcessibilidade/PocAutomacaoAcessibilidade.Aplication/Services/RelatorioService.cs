using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS.Relatorios;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Enuns;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces;
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
        private readonly IMotorAcessibilidade _motorAcessibilidade;

        public RelatorioService(ILogger<RelatorioService> logger, IMotorAcessibilidade motorAcessibilidade)
        {
            _logger = logger;
            _motorAcessibilidade = motorAcessibilidade;

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

                var obterFalhas = obtemRelatorioPorDominio.Where(x => x.StatusTestes == StatusTesteEnum.Falhas).ToList();

                baseRelatorio.Falhas = obterFalhas.Sum(x => x.Mensagem.Count);

                baseRelatorio.ImpactoSerio = obterFalhas.SelectMany(x => x.ImpactoErroComponente).Count(impacto => impacto == ImpactoEnum.serious.ToString());



                baseRelatorio.ImpactoBaixo = obterFalhas.SelectMany(x => x.ImpactoErroComponente).Count(impacto => impacto == ImpactoEnum.minor.ToString());

                baseRelatorio.ImpactoCritico = obterFalhas.SelectMany(x => x.ImpactoErroComponente).Count(impacto => impacto == ImpactoEnum.critical.ToString());

                baseRelatorio.ImpactoModerado = obterFalhas.SelectMany(x => x.ImpactoErroComponente).Count(impacto => impacto == ImpactoEnum.moderate.ToString());

                baseRelatorio.RelateAriaRoles = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatRoles);

                baseRelatorio.RelateContrast = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatColor);

                baseRelatorio.RelateDocEstrutura = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatStructure);

                baseRelatorio.RelateForm = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatForms);

                baseRelatorio.RelateImagem = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatImages);

                baseRelatorio.RelateLang = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatLanguage);

                baseRelatorio.RelateLink = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatLinks);

                baseRelatorio.RelateTeclado = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatKeyboard);

                baseRelatorio.RelateSemantics = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatSemantic);

                baseRelatorio.RelateSensory = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatSensory);

                baseRelatorio.RelateAlternative = obterFalhas.Select(x => x.TipoProblema).Count(y => y == TiposErros.CatAria);

                baseRelatorio.ServicoTestado = obterFalhas.FirstOrDefault().ServicoTestado;

                var obterSucessos = obtemRelatorioPorDominio.Where(x => x.StatusTestes == StatusTesteEnum.Sucessos).ToList();

                baseRelatorio.Sucessos = obterSucessos.Sum(x => x.Mensagem.Count);

                var obterIncompletos = obtemRelatorioPorDominio.Where(x => x.StatusTestes == StatusTesteEnum.Incompletos).ToList();

                baseRelatorio.Incompletos = obterIncompletos.Sum(x => x.Mensagem.Count);

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
                        linha = 6;

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
                            else if (itensResultados.HTML.Count == itensResultados.Seletor.Count && itensResultados.HTML.Count != itensResultados.IDErroComponente.Count)
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
            plan.Cells["a1:n1"].Merge = true;
            plan.Cells["a1"].Style.Font.Bold = true;
            plan.Cells["a1"].Style.Font.Size = 16;
            plan.Cells["a1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            plan.Cells["a1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            plan.Cells["a1"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue); // Fundo azul claro
            plan.Cells["a1"].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black); // Bordas espessas

            
            plan.Cells["a2"].Value = "Serviço analisado";
            plan.Cells["a2:f2"].Merge = true;
            plan.Cells["a2"].Style.Font.Bold = true;
            plan.Cells["a2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            plan.Cells["a2"].Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Fundo cinza claro
            plan.Cells["a2"].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black); // Bordas espessas
            plan.Cells["a2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // Nome do serviço analisado
            plan.Cells["g2"].Value = nomeServicoTestado;
            plan.Cells["g2:n2"].Merge = true;
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
            plan.Cells["g3:n3"].Merge = true;
            plan.Cells["g3"].Style.Font.Bold = true;
            plan.Cells["g3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            plan.Cells["g3"].Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Fundo cinza claro
            plan.Cells["g3"].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black); // Bordas espessas
            plan.Cells["g3"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            plan.Cells["a4"].Value = "Analista";
            plan.Cells["a4:f4"].Merge = true;
            plan.Cells["a4"].Style.Font.Bold = true;
            plan.Cells["a4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            plan.Cells["a4"].Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Fundo cinza claro
            plan.Cells["a4"].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black); // Bordas espessas
            plan.Cells["a4"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


            plan.Cells["g4"].Value = Utils.Utils.ObterNomeUsuario();
            plan.Cells["g4:n4"].Merge = true;
            plan.Cells["g4"].Style.Font.Bold = true;
            plan.Cells["g4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            plan.Cells["g4"].Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Fundo cinza claro
            plan.Cells["g4"].Style.Border.BorderAround(ExcelBorderStyle.Thick, Color.Black); // Bordas espessas
            plan.Cells["g4"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


            return plan;


        }

        private ExcelWorksheet CriarHeaderColunas(ExcelWorksheet plan)
        {
            plan.Cells["a5"].Value = "CT";
            plan.Cells["b5"].Value = "ID";
            plan.Cells["c5"].Value = "Descrição geral";
            plan.Cells["d5"].Value = "Componente";
            plan.Cells["e5"].Value = "Seletor Componente";
            plan.Cells["f5"].Value = "Descrição";
            plan.Cells["g5"].Value = "Descrição do do problema baseado em IA";
            plan.Cells["h5"].Value = "Aplicabilidade de correção";
            plan.Cells["i5"].Value = "Impacto";
            plan.Cells["j5"].Value = "Status do teste";
            plan.Cells["k5"].Value = "Diretriz WCAG";
            plan.Cells["l5"].Value = "Pilar WCAG";
            plan.Cells["m5"].Value = "Categoria";
            plan.Cells["n5"].Value = "Data hora teste";

            // Aplicando a formatação
            using (var range = plan.Cells["a5:n5"])
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
            plan.Cells["a5:n5"].AutoFitColumns();


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
            plan.Cells[$"g{linha}"].Value = _motorAcessibilidade.ObterDescricaoErro(Id);
            plan.Cells[$"h{linha}"].Value = _motorAcessibilidade.ObterAplicabilidadeDeCodigo(Id);
            plan.Cells[$"i{linha}"].Value = impacto;
            plan.Cells[$"j{linha}"].Value = statusTeste;
            plan.Cells[$"k{linha}"].Value = diretrizWcag;
            plan.Cells[$"l{linha}"].Value = pilarWcag;
            plan.Cells[$"m{linha}"].Value = categoria;
            plan.Cells[$"n{linha}"].Value = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); // Corrigido o formato da hora


            // Aplicando a formatação para as células de resultados
            using (var range = plan.Cells[$"a{linha}:n{linha}"])
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

            var nomeArquivo = "RelatorioAcessibilidade_" + Utils.Utils.ExtrairNomeDominiio(url) + "_" + dataAtual + ".xlsx";
            var diretorio = Utils.Utils.DefinirDiretorioRelatorio() + "\\" + nomeArquivo;

            FileInfo file = new FileInfo(diretorio);
            package.SaveAs(file);
            _logger.LogInformation($"Planilha salva em: {diretorio}");

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
