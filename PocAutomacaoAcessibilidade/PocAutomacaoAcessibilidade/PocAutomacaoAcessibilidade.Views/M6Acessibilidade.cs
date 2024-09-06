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
        private readonly IMotorAcessibilidadeService _motorAcessibilidadeService;
        private Label lblUrl;
        private TextBox txtUrl;
        private RichTextBox ResultadosTestes;
        private Button btnIniciarTeste;
        private Button btnExportarRelatorio;
        public M6Acessibilidade(IMotorAcessibilidadeService motorAcessibilidadeService)
        {
            _motorAcessibilidadeService = motorAcessibilidadeService;
            
            InitializeComponent();
            IniciarTela();
            
        }

        private void IniciarTela()
        {
            this.Text = "M6Acessibilidade Automação";
            this.Width = 900;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            
            lblUrl = new Label();
            lblUrl.Text = "Url do serviço que será testado:";
            lblUrl.Top = 20;
            lblUrl.Left = 10;
            lblUrl.Width = 200;

            
            txtUrl = new TextBox();
            txtUrl.Top = 50;
            txtUrl.Left = 10;
            txtUrl.Width = 400;
            txtUrl.Text = "https://";
            

            
            btnIniciarTeste = new Button();
            btnIniciarTeste.Text = "Iniciar Teste";
            btnIniciarTeste.Top = 90;
            btnIniciarTeste.Left = 10;
            btnIniciarTeste.Width = 100;
            btnIniciarTeste.Click += async (sender, e) => await IniciarTesteAsync();

            // RichTextBox para exibir resultados dos testes
            ResultadosTestes = new RichTextBox();
            ResultadosTestes.Top = 130;
            ResultadosTestes.Left = 10;
            ResultadosTestes.Width = 550;
            ResultadosTestes.Height = 200;
            ResultadosTestes.ReadOnly = true;
            ResultadosTestes.IsAccessible = true;
            ResultadosTestes.WordWrap = true;
            ResultadosTestes.SelectionFont = new Font("arial", 12, FontStyle.Regular);
            ResultadosTestes.SelectionColor = Color.Black;



            ExibirMensagemPadrao();

            // Botão para exportar o relatório
            btnExportarRelatorio = new Button();
            btnExportarRelatorio.Text = "Exportar Relatório";
            btnExportarRelatorio.Top = 320;
            btnExportarRelatorio.Left = 10;
            btnExportarRelatorio.Width = 150;
            //btnExportarRelatorio.Click += ExportarRelatorio;


            this.Controls.Add(lblUrl);
            this.Controls.Add(txtUrl);
            this.Controls.Add(btnIniciarTeste);
            this.Controls.Add(ResultadosTestes);
            this.Controls.Add(btnExportarRelatorio);
            

        }

        private async Task IniciarTesteAsync()
        {
            MessageBox.Show($"Atenção, o teste está sendo iniciado para o serviço {txtUrl.Text}");
            var resultadosTestes = await _motorAcessibilidadeService.IniciarTeste(txtUrl.Text);
            GerarRelatorioResultadoTeste(resultadosTestes);
        }

        private void GerarRelatorioResultadoTeste(AnalisePreviaResultadoTeste analiseResultados)
        {
            ResultadosTestes.Clear();

            ResultadosTestes.AppendText("M6Acessibilidade\n\n");
            ResultadosTestes.SelectionFont=new Font("arial", 14, FontStyle.Bold);
            ResultadosTestes.AppendText("Resumo  do teste realizado\n\n");
            ResultadosTestes.SelectionFont = new Font("arial", 14, FontStyle.Regular);

            ResultadosTestes.AppendText($"Data: {System.DateTime.Now.ToString("dd/MM/yyyy HH:MM:ss")}\nSite analizado: {txtUrl.Text}\n\n");
            ResultadosTestes.AppendText($"Quantidade de componentes que não passaram no teste: {analiseResultados.Falhas}\nQuantidade de impactos críticos: {analiseResultados.ImpactoCritico}\nQuantidade de impactos moderados: {analiseResultados.ImpactoModerado}\nQuantidades de impactos sérios: {analiseResultados.ImpactoSerio}\nQuantidade de impactos baixos: {analiseResultados.ImpactoBaixo}.");
            ResultadosTestes.AppendText($"Análise de falha por diretrizes da WCAG \n\n Contrastes: {analiseResultados.RelateContrast} apontamentos.\n Aria-role: {analiseResultados.RelateAriaRoles} apontamentos.\nImagens: {analiseResultados.RelateImagem} apontamentos.\nEstrutura: {analiseResultados.RelateDocEstrutura} apontamentos.\nFormulários: {analiseResultados.RelateForm}. apontamentos.\nIdioma: {analiseResultados.RelateLang}.apontamento.\nLinks: {analiseResultados.RelateLink} apontamentos.\nTeclado: {analiseResultados.RelateTeclado} apontamentos.\n");
            ResultadosTestes.AppendText($"Status de casos de sucessos e dados que não puderam ser testados.\n\nQuantidade de componentes que passaram no teste: {analiseResultados.Sucessos}.\nQuantidade de dados que não estão aplicados para testes: {analiseResultados.NaoAplicados}.\nQuantidade de testes incompletos: {analiseResultados.Incompletos}.\n\nPara ter acesso completo em quais componentes causaram as falhas, exporte o relatório detalhado para conferir os itens que não passaram no teste.");
            

        }

        private void ExibirMensagemPadrao()
        {

            ResultadosTestes.AppendText("M6Acessibilidade\n\n");
            ResultadosTestes.AppendText(" Ao iniciar o teste, seu navegador será aberto e o software irá coletar as validações de acessibilidades e exibir um status geral dos problemas encontrados. Você poderá exportar uma planilha que irá conter todos os problemas encontrados bem como os componentes que causaram a falha. \nInsira a url no formato: https ou http://site.com.br e ir em iniciar teste. Aguarde até que os resultados sejam exibidos.");


        }

        private void ExportarRelatorio(object sender, EventHandler e)
        {
            
        }


    }
}
