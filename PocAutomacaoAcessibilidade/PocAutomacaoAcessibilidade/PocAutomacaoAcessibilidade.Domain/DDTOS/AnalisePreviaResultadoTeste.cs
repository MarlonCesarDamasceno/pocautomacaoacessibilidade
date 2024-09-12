using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS
{
    public class AnalisePreviaResultadoTeste
    {
        public string ServicoTestado { get; set; }
        public int Falhas { get; set; }
        public int ImpactoSerio { get; set; }
        public int ImpactoModerado { get; set; }
        public int ImpactoBaixo { get; set; }
        public int ImpactoCritico { get; set; }
        public int Sucessos { get; set; }
        public int NaoAplicados { get; set; }
        public int Incompletos { get; set; }
        public int RelateContrast { get; set; }
        public int RelateAriaRoles { get; set; }
        public int RelateDocEstrutura { get; set; }
        public int RelateForm { get; set; }
        public int RelateLink { get; set; }
        public int RelateTeclado { get; set; }
        public int RelateImagem { get; set; }
        public int RelateLang { get; set; }






    }
}
