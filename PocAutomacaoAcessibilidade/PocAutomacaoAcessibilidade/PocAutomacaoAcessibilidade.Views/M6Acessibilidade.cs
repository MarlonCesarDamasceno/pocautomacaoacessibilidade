using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces;
using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Views
{
    public partial class M6Acessibilidade : Form
    {
        private List<ResultadoValidacao> relatorioFinal = new List<ResultadoValidacao>();
        private readonly IMotorAcessibilidadeService _motorAcessibilidadeService;
        private readonly IRelatorioService _relatorioService;
        private Label lblUrl;
        private TextBox txtUrl;
        private Label lblsubDominios;
        private TextBox txtSubDominio;
        private RichTextBox ResultadosTestes;
        private Button btnIniciarTeste;
        private Button btnExportarRelatorio;
        public M6Acessibilidade(IMotorAcessibilidadeService motorAcessibilidadeService, IRelatorioService relatorioService)
        {
            _motorAcessibilidadeService = motorAcessibilidadeService;
            _relatorioService = relatorioService;

            InitializeComponent();
            IniciarTela();

        }

        private void IniciarTela()
        {
            // Configurações da tela
            this.Text = "M6Acessibilidade Automação";
            this.Width = 900;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Label para URL
            lblUrl = new Label();
            lblUrl.Text = "Url do serviço que será testado:";
            lblUrl.Top = 20;
            lblUrl.Left = 10;
            lblUrl.Width = 300;

            // TextBox para URL
            txtUrl = new TextBox();
            txtUrl.Top = 50;
            txtUrl.Left = 10;
            txtUrl.Width = 400;
            txtUrl.Text = "https://";

            // Label para Subdomínios
            lblsubDominios = new Label();
            lblsubDominios.Text = "Subdomínios (separados por vírgulas):";
            lblsubDominios.Top = 90;
            lblsubDominios.Left = 10;
            lblsubDominios.Width = 400;

            // TextBox para Subdomínios
            txtSubDominio = new TextBox();
            txtSubDominio.Top = 120;
            txtSubDominio.Left = 10;
            txtSubDominio.Width = 400;

            // Botão para iniciar o teste
            btnIniciarTeste = new Button();
            btnIniciarTeste.Text = "Iniciar Teste";
            btnIniciarTeste.Top = 160;
            btnIniciarTeste.Left = 10;
            btnIniciarTeste.Width = 150;
            btnIniciarTeste.Click += async (sender, e) => await IniciarTesteAsync();

            // RichTextBox para exibir os resultados dos testes
            ResultadosTestes = new RichTextBox();
            ResultadosTestes.Top = 200;
            ResultadosTestes.Left = 10;
            ResultadosTestes.Width = 550;
            ResultadosTestes.Height = 200;
            ResultadosTestes.ReadOnly = true;
            ResultadosTestes.IsAccessible = true;
            ResultadosTestes.WordWrap = true;
            ResultadosTestes.SelectionFont = new Font("Arial", 12, FontStyle.Regular);
            ResultadosTestes.SelectionColor = Color.Black;


            // Exibir mensagem padrão na caixa de resultados
            ExibirMensagemPadrao();

            // Botão para exportar relatório
            btnExportarRelatorio = new Button();
            btnExportarRelatorio.Text = "Exportar Relatório";
            btnExportarRelatorio.Top = 420;
            btnExportarRelatorio.Left = 10;
            btnExportarRelatorio.Width = 150;
            btnExportarRelatorio.Click += new EventHandler(ExportarRelatorio);

            // Adicionando controles à tela
            this.Controls.Add(lblUrl);
            this.Controls.Add(txtUrl);
            this.Controls.Add(lblsubDominios);
            this.Controls.Add(txtSubDominio);
            this.Controls.Add(btnIniciarTeste);
            this.Controls.Add(ResultadosTestes);
            this.Controls.Add(btnExportarRelatorio);

        }

        private async Task IniciarTesteAsync()
        {
            if (Utils.Utils.ObterServicoTesteRegistrado()==null) 
            {
              await  IniciarTeste();
            }

            if(Utils.Utils.ValidarAlteracaoAlvoTeste(txtUrl.Text))
            {
                MessageBox.Show("Atenção, identifiquei que houve mudança no alvo do teste. Os dados anteriores serão sobreescritos.");
                txtSubDominio.Text = "";
                relatorioFinal = null;

                await IniciarTeste();
            }


        }

        private async Task IniciarTeste()
        {
            Utils.Utils.RegistrarServicoTestado(txtUrl.Text);
            MessageBox.Show($"Atenção, o teste está sendo iniciado para o serviço {txtUrl.Text}");
            var resultadosTestes = await _motorAcessibilidadeService.IniciarTeste(txtUrl.Text, txtSubDominio.Text);
            GerarRelatorioResultadoTeste(resultadosTestes);


        }
        private void GerarRelatorioResultadoTeste(ResultadosTestes resultados)
        {
            ResultadosTestes.Clear();
            relatorioFinal = resultados.resultadosValidacoes;

            foreach (var analiseResultados in resultados.analisesPreliminaresValidacoes)
            {

                ResultadosTestes.AppendText("M6 Acessibilidade\n\n");
                ResultadosTestes.SelectionFont = new Font("Arial", 14, FontStyle.Bold);
                ResultadosTestes.AppendText("Resumo do Teste Realizado\n\n");

                
                ResultadosTestes.SelectionFont = new Font("Arial", 14, FontStyle.Regular);


                ResultadosTestes.AppendText($"Data: {System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}\n");
                ResultadosTestes.AppendText($"Site Analisado: {analiseResultados.ServicoTestado}\n\n");

                
                ResultadosTestes.AppendText($"Quantidade de Componentes que Não Passaram no Teste: {analiseResultados.Falhas}\n");
                ResultadosTestes.AppendText($"Quantidade de Impactos Críticos: {analiseResultados.ImpactoCritico}\n");
                ResultadosTestes.AppendText($"Quantidade de Impactos Moderados: {analiseResultados.ImpactoModerado}\n");
                ResultadosTestes.AppendText($"Quantidade de Impactos Sérios: {analiseResultados.ImpactoSerio}\n");
                ResultadosTestes.AppendText($"Quantidade de Impactos Baixos: {analiseResultados.ImpactoBaixo}.\n");


                ResultadosTestes.AppendText("\nAnálise de Estimativa por Falha de Diretrizes da WCAG\n\n");
                ResultadosTestes.AppendText($"Contrastes: {analiseResultados.RelateContrast} apontamentos.\n");
                ResultadosTestes.AppendText($"Aria-role: {analiseResultados.RelateAriaRoles} apontamentos.\n");
                ResultadosTestes.AppendText($"Imagens: {analiseResultados.RelateImagem} apontamentos.\n");
                ResultadosTestes.AppendText($"Textos Alternativos: {analiseResultados.RelateAlternative} apontamentos.\n");
                ResultadosTestes.AppendText($"Semântica: {analiseResultados.RelateSemantics} apontamentos.\n");
                ResultadosTestes.AppendText($"Sensorial: {analiseResultados.RelateSensory} apontamentos.\n");
                ResultadosTestes.AppendText($"Estrutura: {analiseResultados.RelateDocEstrutura} apontamentos.\n");
                ResultadosTestes.AppendText($"Formulários: {analiseResultados.RelateForm} apontamentos.\n");
                ResultadosTestes.AppendText($"Idioma: {analiseResultados.RelateLang} apontamento.\n");
                ResultadosTestes.AppendText($"Links: {analiseResultados.RelateLink} apontamentos.\n");
                ResultadosTestes.AppendText($"Teclado: {analiseResultados.RelateTeclado} apontamentos.\n\n");

                
                ResultadosTestes.AppendText($"Status de Casos de Sucesso e Dados que Não Pudemos Testar\n\n");
                ResultadosTestes.AppendText($"Quantidade de Componentes que Passaram no Teste: {analiseResultados.Sucessos}.\n");
                ResultadosTestes.AppendText($"Quantidade de Dados que Não Estão Aplicados para Testes: {analiseResultados.NaoAplicados}.\n");
                ResultadosTestes.AppendText($"Quantidade de Testes que Não Pudemos Analisar Automaticamente e Que Apresentam Potencial de Falhas e Precisam Ser Analisados Manualmente: {analiseResultados.Incompletos}.\n\n");
            }

            
            ResultadosTestes.SelectionFont = new Font("Arial", 14, FontStyle.Regular);
            ResultadosTestes.AppendText("Esses resultados são apenas dados preliminares do teste realizado. Gere o relatório completo para uma análise mais profunda dos problemas encontrados.\nMCD Acessibilidade. Desenvolvido por Marlon Cesar Damasceno.");
        }






        private void ExibirMensagemPadrao()
        {
            
            ResultadosTestes.AppendText("M6 Acessibilidade\n\n");


            ResultadosTestes.AppendText(
                "Ao iniciar o teste, seu navegador será aberto e o software irá coletar as validações de acessibilidade, " +
                "exibindo um status geral dos problemas encontrados. Você poderá exportar uma planilha que conterá todos os " +
                "problemas encontrados, bem como os componentes que causaram a falha.\n\n" +
                "Insira a URL no formato: https:// ou http://site.com.br e clique em 'Iniciar Teste'. Aguarde até que os resultados " +
                "sejam exibidos.\n\n"
            );

            
            ResultadosTestes.AppendText("MCD Acessibilidade. Desenvolvido por Marlon Cesar Damasceno.");
        }

        private void ExportarRelatorio(object sender, EventArgs e)
        {
            MessageBox.Show("Iniciando a exportação do relatório para Excel");
            var exportarRelatorio = _relatorioService.ExportarRelatorioParaExcel(relatorioFinal);

            if (exportarRelatorio)
            {
                MessageBox.Show("Relatório exportado com sucesso.");
            }
            else
            {
                MessageBox.Show("Falha ao exportar relatório.");
            }
        }


    }
}
