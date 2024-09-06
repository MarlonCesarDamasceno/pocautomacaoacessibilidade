using Newtonsoft.Json;
using Selenium.Axe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS
{
    public class ResultadoValidacao
    {
        public StatusTesteEnum StatusTestes { get; set; }
        public string IdErro { get; set; }
        public string Descricao { get; set; }
        public string Impacto { get; set; }
        public string TipoProblema { get; set; }
        public string PilarWCAG { get; set; }
        public string DiretrizWCAG { get; set; }
        public string Seletor { get; set; }
        public string HTML { get; set; }
        public string ImpactoErroComponente { get; set; }
        public string IDErroComponente { get; set; }
        public string Mensagem { get; set; }
        public string ComponentRelacionado { get; set; }




    }


    public class Targets
    {
        public string Seletor { get; set; }
        public string HTML { get; set;  }
        public string Impacto { get; set; }
        public string ID { get; set; }
        public string Mensagem { get; set; }
        public string ComponentRelacionado { get; set; }





        

    }

}
