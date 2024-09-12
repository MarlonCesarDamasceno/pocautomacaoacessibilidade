using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocAutomacaoAcessibilidade.PocAutomacaoAcessibilidade.Domain.DDTOS.Relatorios
{
    public class BaseRelatorioComponente
    {
        public int ID { get; set; }
        public string IDProblema { get; set; }
        public string Impacto { get; set; }
        public string Componente { get; set; }
        public string Seletor { get; set; }
        public  string Descricao { get; set; }
        public string PilarWCAG { get; set; }
        public string DiretrizWCAG { get; set; }
        public string Categoria { get; set; }
        public string StatusTeste { get; set; }
    }
}
