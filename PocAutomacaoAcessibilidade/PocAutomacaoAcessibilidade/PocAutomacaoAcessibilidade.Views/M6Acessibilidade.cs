using PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.Interfaces;
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
        private readonly IMotorAcessibilidade _motorAcessibilidade;
        private Label lblUrl;
        private TextBox txtUrl;
        private Button btnIniciarTeste;
        public M6Acessibilidade(IMotorAcessibilidade motorAcessibilidade)
        {
            _motorAcessibilidade = motorAcessibilidade;
            InitializeComponent();
            IniciarTela();
        }

        private void IniciarTela()
        {
            this.Text = "M6Acessibilidade Automação";
            this.Width = 400;
            this.Height = 300;

            lblUrl = new Label();
            lblUrl.Text = "Informe a url para que será disparado o teste.";
            lblUrl.Top = 20;
            lblUrl.Left = 10;
            txtUrl = new TextBox();
            txtUrl.Text = "Informe a url";
            this.Controls.Add(lblUrl);
            this.Controls.Add(txtUrl);

            btnIniciarTeste = new Button();
            btnIniciarTeste.Text = "Iniciar teste";
            btnIniciarTeste.Click += async (sender, e) => await IniciarTesteAsync();
            this.Controls.Add(btnIniciarTeste);


        }

        private async Task IniciarTesteAsync()
        {
            await _motorAcessibilidade.ValidarAcessibilidade(txtUrl.Text);
        }
    }
}
